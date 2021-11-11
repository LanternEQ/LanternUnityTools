using System.Collections.Generic;
using UnityEngine;

namespace Lantern.EQ
{
    /// <summary>
    /// Manages characters and their variants.
    /// Only used for non-playable races
    /// </summary>
    public class NonPlayableVariantHandler : VariantHandler
    {
        [SerializeField]
        List<AdditionalMaterials> _alternateSkins = new List<AdditionalMaterials>();

        private Dictionary<string, List<Material>> _additionalMaterials;
        
        protected override void Awake()
        {
            base.Awake();
            CreateAlternateMaterials();
        }
        
        private void CreateAlternateMaterials()
        {
            _additionalMaterials = new Dictionary<string, List<Material>>();
            foreach (AdditionalMaterials group in _alternateSkins)
            {
                string character = string.Empty, skinId = string.Empty, partName = string.Empty;

                foreach (var material in group.materials)
                {
                    if (material == null)
                    {
                        continue;
                    }
                    
                    string materialName = material.name.Split('_')[1];
                    ParseCharacterSkin(materialName, out character, out skinId, out partName);

                    if (!_additionalMaterials.ContainsKey(partName))
                    {
                        _additionalMaterials[partName] = new List<Material>();
                    }
                    
                    _additionalMaterials[partName].Add(material);
                }
            }
        }

        public void SetCurrentActiveVariant(int texture, int helmTexture)
        {
            HandleMainMeshes(helmTexture);
            SetActiveMeshFromGroup(_secondaryMeshes, helmTexture);
            SetMeshMaterials(texture);
            
            if (CharacterSounds != null)
            {
                CharacterSounds.SetCurrentVariant(texture);
            }
        }

        private void SetMeshMaterials(int index)
        {
            SetMeshMaterialsInGroup(_mainMeshes, index);
            SetMeshMaterialsInGroup(_secondaryMeshes, index);
        }

        private void SetMeshMaterialsInGroup(List<GameObject> meshes, int textureId)
        {
            if (_alternateSkins.Count == 0)
            {
                return;
            }
            
            foreach (var mesh in meshes)
            {
                if (!mesh.activeSelf)
                {
                    continue;
                }

                List<int> indices = mesh.GetComponent<UsedIndices>().Indices;

                List<Material> materials = new List<Material>();

                if (textureId >= _alternateSkins.Count)
                {
                    continue;
                }
                
                foreach (var index in indices)
                {
                    if (index >= _alternateSkins[textureId].materials.Length)
                    {
                        Debug.LogError("Issue here");
                        continue;
                    }
                    
                    materials.Add(_alternateSkins[textureId].materials[index]);
                }
                
                mesh.GetComponent<SkinnedMeshRenderer>().materials = materials.ToArray();
            }
        }
        
        public void SetAdditionalMaterials(List<Material[]> materials)
        {
            foreach(Material[] material in materials)
            {
                AdditionalMaterials am = new AdditionalMaterials
                {
                    materials = material
                };
                
                _alternateSkins.Add(am);
            }
        }
    }
}
