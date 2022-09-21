using System;
using System.Collections.Generic;
using System.Linq;
using Lantern.EQ.Animation;
using Lantern.EQ.Helpers;
using Lantern.EQ.Lighting;
using UnityEngine;

namespace Lantern.EQ.Equipment
{
    /// <summary>
    /// Handles equipping and management of 3d items on a character model
    /// </summary>
    public class Equipment3dHandler : MonoBehaviour
    {
        private AmbientLightSetterDynamic _dynamicLightSetter;

        private int _instanceId;

        [SerializeField]
        private SkeletonAttachPoints _attachPoints;

        private Dictionary<Equipment3dSlot, EquipmentModel> _spawnedEquipment;
        private int _renderLayer;
        private Action<Renderer> _updateModelCallback;

        private void Awake()
        {
            _dynamicLightSetter = GetComponent<AmbientLightSetterDynamic>();
        }

        public void Initialize(bool isPlayer, int instanceId)
        {
            _instanceId = instanceId;

            if (_attachPoints == null)
            {
                _attachPoints = GetComponent<SkeletonAttachPoints>();
            }

            _dynamicLightSetter = GetComponent<AmbientLightSetterDynamic>();
            _spawnedEquipment = new Dictionary<Equipment3dSlot, EquipmentModel>();
        }

        public void OnDestroy()
        {
            foreach (Equipment3dSlot slot in Enum.GetValues(typeof(Equipment3dSlot)))
            {
                RemoveItemIfExists(slot);
            }
        }

        public void EquipmentDebug(GameObject item)
        {
            SpawnItemInSlot(Equipment3dSlot.MainHand, item);
        }

        public void SetSkeletonAttachPoints(SkeletonAttachPoints sap)
        {
            _attachPoints = sap;
        }

        public void SpawnItemInSlot(Equipment3dSlot point, GameObject item)
        {
            _spawnedEquipment ??= new Dictionary<Equipment3dSlot, EquipmentModel>();

            if (_spawnedEquipment.ContainsKey(point))
            {
                Debug.LogWarning("ITEM ALREADY IN attach point: " + point);
                RemoveItemIfExists(point);
            }

            var attachPoint = _attachPoints.GetAttachPoint(EquipmentHelper.GetSkeletonAttachPoint(point));

            if (attachPoint == null)
            {
                Debug.LogError("Attach point not found for skeleton: " + point);
                return;
            }

            var instantiated = Instantiate(item, attachPoint);
            instantiated.layer = _renderLayer;
            GameObjectHelper.CleanName(instantiated);

            if (point == Equipment3dSlot.Ranged)
            {
                DisableItemIfExists(Equipment3dSlot.MainHand);
                DisableItemIfExists(Equipment3dSlot.OffHand);
                DisableItemIfExists(Equipment3dSlot.Shield);
            }
            else if (point == Equipment3dSlot.MainHand || point == Equipment3dSlot.OffHand || point == Equipment3dSlot.Shield)
            {
                DisableItemIfExists(Equipment3dSlot.Ranged);
            }

            var childSetter = instantiated.AddComponent<AmbientLightSetterChild>();
            if (_dynamicLightSetter != null)
            {
                _dynamicLightSetter.AddChild(childSetter);
            }

            if (instantiated.TryGetComponent<EquipmentModel>(out var equipmentModel))
            {
                _spawnedEquipment[point] = equipmentModel;

                foreach (var rend in equipmentModel.Renderers)
                {
                    _updateModelCallback?.Invoke(rend);
                }
            }

            UpdateLayerValues();
        }

        public void SpawnNpcEquipment(GameObject item1, GameObject item2, Equipment3dSlot item1Slot, Equipment3dSlot item2Slot)
        {
            if (item1 != null)
            {
                SpawnItemInSlot(item1Slot, item1);
            }

            if (item2)
            {
                SpawnItemInSlot(item2Slot, item2);
            }
        }

        public Equipment3dSlot RemoveItemIfEquipped(string item)
        {
            if (_spawnedEquipment != null)
            {
                item = item.ToLower();
                var entries = _spawnedEquipment.ToArray();
                foreach (var entry in entries)
                {
                    if (entry.Value.name == item && RemoveItemIfExists(entry.Key))
                    {
                        return entry.Key;
                    }
                }
            }

            return Equipment3dSlot.Helm;
        }

        public bool RemoveItemIfExists(Equipment3dSlot point)
        {
            if (_spawnedEquipment == null || !_spawnedEquipment.ContainsKey(point))
            {
                return false;
            }

            var spawnedEquipment = _spawnedEquipment[point];
            var childSetter = spawnedEquipment.GetComponent<AmbientLightSetterChild>();

            if (childSetter != null && _dynamicLightSetter != null)
            {
                _dynamicLightSetter.RemoveSunlightChild(childSetter);
            }

            Destroy(spawnedEquipment.gameObject);
            _spawnedEquipment.Remove(point);

            UpdateItemVisibility();

            return true;
        }

        private void UpdateItemVisibility()
        {
            if (DoesItemExist(Equipment3dSlot.MainHand) || DoesItemExist(Equipment3dSlot.OffHand) || DoesItemExist(Equipment3dSlot.Shield))
            {
                EnableItemIfExists(Equipment3dSlot.MainHand);
                EnableItemIfExists(Equipment3dSlot.OffHand);
                EnableItemIfExists(Equipment3dSlot.Shield);
                DisableItemIfExists(Equipment3dSlot.Ranged);
            }
            else
            {
                EnableItemIfExists(Equipment3dSlot.Ranged);
            }
        }

        public bool EnableItemIfExists(Equipment3dSlot slot)
        {
            if (_spawnedEquipment == null || !_spawnedEquipment.ContainsKey(slot))
            {
                return false;
            }

            _spawnedEquipment[slot].gameObject.SetActive(true);

            return true;
        }

        public bool DisableItemIfExists(Equipment3dSlot slot)
        {
            if (_spawnedEquipment == null || !_spawnedEquipment.ContainsKey(slot))
            {
                return false;
            }

            _spawnedEquipment[slot].gameObject.SetActive(false);

            return true;
        }

        public bool DoesItemExist(Equipment3dSlot slot)
        {
            return _spawnedEquipment.ContainsKey(slot);
        }

        public void SetLayer(int layer)
        {
            _renderLayer = layer;
            UpdateLayerValues();
        }

        private void UpdateLayerValues()
        {
            if (_spawnedEquipment == null)
            {
                return;
            }

            foreach (var e in _spawnedEquipment.Values)
            {
                e.SetLayer(_renderLayer);
            }
        }

        public void SetModelUpdateCallback(Action<Renderer> onNewActiveModel)
        {
            _updateModelCallback = onNewActiveModel;
        }
    }
}
