using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lantern
{
    [Serializable]
    public class AdditionalMaterials
    {
        public Material[] materials;
    }
        
    [Serializable]
    public class AdditionalTextures
    {
        public Texture[] Textures;
    }
    
    [Serializable]
    public class SkinTextures
    {
        public int SkinId;
        public Texture[] Textures;
    }
    
    /// <summary>
    /// Manages characters and their variants.
    /// Only used for non-playable races
    /// </summary>
    public abstract class VariantHandler : MonoBehaviour
    {
        [SerializeField]
        protected List<GameObject> _mainMeshes = new List<GameObject>();
        
        [SerializeField]
        protected List<GameObject> _secondaryMeshes = new List<GameObject>();

        protected CharacterSounds CharacterSounds;

        protected virtual void Awake()
        {
            CharacterSounds = GetComponent<CharacterSounds>();
        }
        
        public static void ParseCharacterSkin(string materialName, out string character, out string skinId,
            out string partName)
        {
            if (materialName.Length < 9)
            {
                character = string.Empty;
                skinId = string.Empty;
                partName = string.Empty;
                return;
            }
            
            character = materialName.Substring(0, 3);
            skinId = materialName.Substring(5, 2);
            partName = materialName.Substring(3, 2) + materialName.Substring(7, 2);
        }

        protected void HandleMainMeshes(int helmTexture)
        {
            if (helmTexture != 0)
            {
                _mainMeshes.Last().SetActive(false);
            }
            else
            {
                foreach (var mesh in _mainMeshes)
                {
                    mesh.SetActive(true);
                }
            }
        }

        protected void SetActiveMeshFromGroup(List<GameObject> meshes, int activeIndex)
        {
            activeIndex--;
            for (var i = 0; i < meshes.Count; i++)
            {
                int index = activeIndex;

                if (index >= meshes.Count)
                {
                    Debug.LogError("Trying to use an index that is out of range");
                    index = 0;
                }
                
                for (int j = 0; j < meshes.Count; ++j)
                {
                    meshes[j].SetActive(j == index);
                }
            }
        }

        public void AddPrimaryMesh(GameObject mesh)
        {
            _mainMeshes.Add(mesh);
        }
        
        public void AddSecondaryMesh(GameObject mesh)
        {
            _secondaryMeshes.Add(mesh);
        }

        public GameObject GetLastPrimaryMesh()
        {
            return _mainMeshes.LastOrDefault();
        }
    }
}
