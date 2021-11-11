using System;
using System.Collections.Generic;
using System.Linq;
using Infastructure.SerializableDictionary;
using Lantern.EQ;
using UnityEngine;

namespace Lantern
{
    [Serializable]
    public class EquipmentTextures : SerializableDictionary<SkinnedMeshRenderer, Skins>
    {
    }
    
    [Serializable]
    public class Skins
    {
        public List<SkinTextures> SkinList;
    }

    public enum BodyType
    {
        Normal,
        Robe
    }

    /// <summary>
    /// Manages characters and their variants.
    /// Only used for non-playable races
    /// </summary>
    public class Equipment2dHandler : VariantHandler
    {
        [SerializeField] 
        private List<SkinnedMeshRenderer> _bodyMeshes = new List<SkinnedMeshRenderer>();
        
        [SerializeField] 
        private List<SkinnedMeshRenderer> _headMeshes = new List<SkinnedMeshRenderer>();
        
        [SerializeField]
        List<AdditionalTextures> _faceSkins = new List<AdditionalTextures>();

        [SerializeField]
        private EquipmentTextures _equipmentTextures =  new EquipmentTextures();

        private SkinnedMeshRenderer _currentBody;
        private SkinnedMeshRenderer _currentHelm;
        private BodyType _currentBodyType;

        protected override void Awake()
        {
            base.Awake();
            SetBodyType(BodyType.Normal);
            SetHelmType(0);
        }

        private void SetHelmType(int index)
        {
            if (index < 0 || index >= _headMeshes.Count)
            {
                return;
            }
            
            for (int i = 0; i < _headMeshes.Count; ++i)
            {
                _headMeshes[i].gameObject.SetActive(i == index);
            }

            _currentHelm = _headMeshes[index];
        }

        private void SetBodyType(BodyType type)
        {
            var previousBody = _currentBody;
            
            if (_bodyMeshes.Count <= 1)
            {
                _currentBody = _bodyMeshes[0];
                _currentBodyType = BodyType.Normal;
                return;
            }
            
            _bodyMeshes[0].gameObject.SetActive(type == BodyType.Normal);
            _bodyMeshes[1].gameObject.SetActive(type == BodyType.Robe);
            _currentBody = type == BodyType.Normal ? _bodyMeshes[0] : _bodyMeshes[1];
            _currentBodyType = type;

            if (previousBody != _currentBody)
            {
                GetComponent<SunlightSetterDynamic>().ForceUpdate();
            }
        }

        /// <summary>
        /// Sets entire texture
        /// Used for NPCs
        /// </summary>
        /// <param name="skinId"></param>
        /// <param name="helmId"></param>
        public void SetArmorSetActive(int skinId, int helmId)
        {

            if (IsRobeSkin(skinId))
            {
                SetBodyType(BodyType.Robe);
                SetActiveSkin(_currentBody, skinId, Color.white);
            }
            else
            {
                SetBodyType(BodyType.Normal);
                SetActiveSkin(_currentBody, skinId, Color.white);
            }

            for (int i = 0; i < _headMeshes.Count; i++)
            {
                _headMeshes[i].gameObject.SetActive(i == helmId);
            }
        }

        private bool IsRobeSkin(int skinId)
        {
            return skinId >= 10 && skinId <= 16;
        }

        private void SetActiveSkin(SkinnedMeshRenderer mesh, int skinId, Color color)
        {
            if (!_equipmentTextures.ContainsKey(mesh))
            {
                return;
            }

            var meshSkins = _equipmentTextures[mesh];
            var skin = meshSkins.SkinList.FirstOrDefault(x => x.SkinId == skinId);

            if (skin == null)
            {
                return;
            }
            
            var materials = mesh.sharedMaterials;

            MaterialPropertyBlock pb = new MaterialPropertyBlock();

            for (int i = 0; i < materials.Length; i++)
            {
                Texture texture = skin.Textures[i];
                
                if (texture == null)
                {
                    continue;
                }
                
                mesh.GetPropertyBlock(pb, i);
                pb.SetTexture("_BaseMap", texture);
                pb.SetColor("_BaseColor", color);
                mesh.SetPropertyBlock(pb, i);
            }
        }

        private void SetFace(int faceId)
        {
            if (_faceSkins == null || _faceSkins.Count == 0)
            {
                return;
            }
            
            if (faceId < 0 || faceId >= _faceSkins.Count)
            {
                faceId = 0;
            }
            
            var primaryMesh = _mainMeshes.LastOrDefault();

            if (primaryMesh == null)
            {
                return;
            }

            var meshRenderer = primaryMesh.GetComponent<SkinnedMeshRenderer>();

            if (meshRenderer == null)
            {
                return;
            }

            MaterialPropertyBlock pb = new MaterialPropertyBlock();

            for (int i = 0; i < meshRenderer.sharedMaterials.Length; i++)
            {
                meshRenderer.GetPropertyBlock(pb, i);
                if (_faceSkins[faceId].Textures[i] == null)
                {
                    continue;
                }
                
                pb.SetTexture("_BaseMap", _faceSkins[faceId].Textures[i]);
                meshRenderer.SetPropertyBlock(pb, i);
            }
        }

        // Editor only
        public void SetAdditionalFaces(List<Texture[]> materials)
        {
            if (materials == null)
            {
                return;
            }

            _faceSkins = new List<AdditionalTextures>();

            foreach (var textures in materials)
            {
                var at = new AdditionalTextures
                {
                    Textures = new Texture[textures.Length]
                };
                
                for (int i = 0; i < textures.Length; i++)
                {
                    at.Textures[i] = textures[i];
                }
                _faceSkins.Add(at);
            }
        }

        public void SetFaceId(int face)
        {
            SetFace(face);
        }

        // TODO: Editor only
        public void ParsePlayableMeshes()
        {
            var mainBodyMesh = _mainMeshes.FirstOrDefault()?.GetComponent<SkinnedMeshRenderer>();

            if (mainBodyMesh != null)
            {
                _bodyMeshes.Add(mainBodyMesh);
            }

            var robeMesh = _secondaryMeshes.FirstOrDefault()?.GetComponent<SkinnedMeshRenderer>();

            if (robeMesh != null && mainBodyMesh != null && mainBodyMesh.name + "01" == robeMesh.name)
            {
                _bodyMeshes.Add(robeMesh);
            }
            else
            {
                robeMesh = null;
            }

            // Looks for heads
            if (_mainMeshes.Count > 1)
            {
                _headMeshes.Add(_mainMeshes[1].GetComponent<SkinnedMeshRenderer>());
            }

            foreach (var mesh in _secondaryMeshes)
            {
                SkinnedMeshRenderer smr = mesh.GetComponent<SkinnedMeshRenderer>();
                
                if (smr != robeMesh)
                {
                    _headMeshes.Add(smr);
                }
            }
        }

        private string GetEquipSlotName(Equipment2dSlot slot)
        {
            switch (slot)
            {
                case Equipment2dSlot.Chest:
                    return gameObject.name +"ch";
                case Equipment2dSlot.Leg:
                    return gameObject.name +"lg";
                case Equipment2dSlot.Hand:
                    return gameObject.name +"hn";
                case Equipment2dSlot.UpperArm:
                    return gameObject.name +"ua";
                case Equipment2dSlot.Foot:
                    return gameObject.name +"ft";
                case Equipment2dSlot.Forearm:
                    return gameObject.name +"fa";
                default:
                    throw new ArgumentOutOfRangeException(nameof(slot), slot, null);
            }
        }
        
        public void SetArmorPiece(Equipment2dSlot slot, int skinId, Color color)
        {
            if (slot == Equipment2dSlot.Chest && IsRobeSkin(skinId))
            {
                SetRobeTexture(skinId, color);

                if (IsErudite())
                {
                    SetActiveSkin(_headMeshes.FirstOrDefault(), skinId, color);
                }
            }
            else if (slot == Equipment2dSlot.Head)
            {
                SetHelmType(skinId);
                TintMesh(_currentHelm, color, true);
                
                if (IsErudite())
                {
                    SetActiveSkin(_headMeshes.FirstOrDefault(), skinId, color);
                }
            }
            else
            {
                SetSinglePieceTexture(slot, skinId, color);
            }
        }

        private void SetRobeTexture(int skinId, Color color)
        {
            SetBodyType(skinId == 0 ? BodyType.Normal : BodyType.Robe);
            SetActiveSkin(_currentBody, skinId, color);

            if (IsErudite())
            {
                SetActiveSkin(_currentHelm, skinId, color);
            }
        }

        private bool IsErudite()
        {
            var mainMesh = GetMainBodyMesh();
            return mainMesh.name == "erf" || mainMesh.name == "erm";
        }

        private void SetSinglePieceTexture(Equipment2dSlot slot, int skinIndex, Color color)
        {
            if (_currentBody == null || !_equipmentTextures.ContainsKey(_currentBody))
            {
                return;
            }

            if (slot == Equipment2dSlot.Chest)
            {
                SetBodyType(BodyType.Normal);
            }

            if (!IsSlotVisible(slot))
            {
                return;
            }

            var skinMesh = GetMeshForSkinIndex(skinIndex);
            var skinEquip = _equipmentTextures[skinMesh];
            var skin = skinEquip.SkinList.Find(x => x.SkinId == skinIndex);

            var defaultEquip = _equipmentTextures[_currentBody];
            var defaultSkin = defaultEquip.SkinList.FirstOrDefault();
            
            if (skin == null || defaultSkin == null)
            {
                return;
            }
            
            var materials = _currentBody.sharedMaterials;

            MaterialPropertyBlock pb = new MaterialPropertyBlock();

            string targetSlotName = GetEquipSlotName(slot);
            
            for (var i = 0; i < materials.Length; i++)
            {
                if (!materials[i].name.Contains(targetSlotName))
                {
                    continue;
                }
                
                Texture texture = GetMatchingTextureInList(targetSlotName, materials[i].name.Substring(materials[i].name.Length - 2), defaultSkin.Textures[i], skin.Textures);

                if (texture == null)
                {
                    texture = defaultSkin.Textures[i];

                    if (texture == null)
                    {
                        continue;
                    }
                }
                
                _currentBody.GetPropertyBlock(pb, i);
                pb.SetTexture("_BaseMap", texture);
                pb.SetColor("_BaseColor", color);
                _currentBody.SetPropertyBlock(pb, i);
            }
        }

        private SkinnedMeshRenderer GetMeshForSkinIndex(int skinId)
        {
            foreach (var pair in _equipmentTextures)
            {
                foreach (var skin in pair.Value.SkinList)
                {
                    if (skin.SkinId == skinId)
                    {
                        return pair.Key;
                    }
                }
            }

            return null;
        }

        private bool IsSlotVisible(Equipment2dSlot slot)
        {
            var robeMesh = GetRobeMesh();
            if (robeMesh == null)
            {
                return true;
            }

            if (_currentBody != robeMesh)
            {
                return true;
            }
            
            switch(slot)
            {
                case Equipment2dSlot.Foot:
                case Equipment2dSlot.Head:
                case Equipment2dSlot.Hand:
                    return true;
                default:
                    return false;
            }
        }

        private void TintMesh(SkinnedMeshRenderer mesh, Color color, bool isHelm)
        {
            if (mesh == null)
            {
                return;
            }
            
            MaterialPropertyBlock pb = new MaterialPropertyBlock();
            var materials = mesh.sharedMaterials;
            SkinnedMeshRenderer baseMesh = isHelm ? _headMeshes[0] : _bodyMeshes[0];
            var baseMaterials = baseMesh.sharedMaterials;
            
            for (var i = 0; i < materials.Length; i++)
            {
                if (baseMaterials.Contains(materials[i]))
                {
                    continue;
                }
                
                mesh.GetPropertyBlock(pb, i);
                pb.SetColor("_BaseColor", color);
                mesh.SetPropertyBlock(pb, i);
            }
        }

        private Texture GetMatchingTextureInList(string slotName, string index, Texture sourceTexture, Texture[] textures)
        {
            if (sourceTexture == null)
            {
                return null;
            }
            
            foreach (var texture in textures)
            {
                if (texture == null)
                {
                    continue;
                }
                
                if (texture.name.StartsWith(slotName) && texture.name.EndsWith(index))
                {
                    return texture;
                }
            }
            
            return null;
        }

        public SkinnedMeshRenderer GetMainBodyMesh()
        {
            if (_bodyMeshes == null || _bodyMeshes.Count <= 0)
            {
                return null;
            }
            
            return _bodyMeshes[0];
        }
        
        public SkinnedMeshRenderer GetRobeMesh()
        {
            if (_bodyMeshes == null || _bodyMeshes.Count <= 1)
            {
                return null;
            }
            
            return _bodyMeshes[1];
        }

        public SkinnedMeshRenderer GetHeadMesh(int index)
        {
            if (_headMeshes == null || index < 0 || index >= _headMeshes.Count)
            {
                return null;
            }

            return _headMeshes[index];
        }

        public void SetEquipmentTextures(SkinnedMeshRenderer skinnedMeshRenderer, int index, Texture[] textures)
        {
            if (!_equipmentTextures.ContainsKey(skinnedMeshRenderer))
            {
                _equipmentTextures[skinnedMeshRenderer] = new Skins {SkinList = new List<SkinTextures>()};
            }
            
            _equipmentTextures[skinnedMeshRenderer].SkinList.Add(new SkinTextures{SkinId = index, Textures = textures});
        }

        public bool IsRobeMeshActive()
        {
            return _currentBodyType == BodyType.Robe;
        }
    }
}
