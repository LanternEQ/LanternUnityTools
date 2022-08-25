using System.Collections.Generic;
using System.Linq;
using Lantern.EQ.Animation;
using Lantern.Helpers;
using UnityEngine;

namespace Lantern.EQ
{
    public class Equipment3dHandler : MonoBehaviour
    {
        private bool _isPlayer;
        private SunlightSetterDynamic _dynamicLightSetter;


        private int _instanceId;
        [SerializeField] 
        private SkeletonAttachPoints _attachPoints;

        private Dictionary<SkeletonPoints, GameObject> _spawnedEquipment;

        private void Awake()
        {
            _dynamicLightSetter = GetComponent<SunlightSetterDynamic>();
        }

        public void Initialize(bool isPlayer, int instanceId)
        {
            _isPlayer = isPlayer;
            _instanceId = instanceId;
            
            if (_attachPoints == null)
            {
                _attachPoints = GetComponent<SkeletonAttachPoints>();
            }

            _dynamicLightSetter = GetComponent<SunlightSetterDynamic>();

            _spawnedEquipment = new Dictionary<SkeletonPoints, GameObject>();

            if (isPlayer)
            {
                //_inventoryService = ServiceFactory.Get<InventoryService>();
                
                if (_dynamicLightSetter != null)
                {
                    _dynamicLightSetter.SetIsPlayer();
                }
            }
        }
        
        public void OnDestroy()
        {
            RemoveItemIfExists(SkeletonPoints.Head);
            RemoveItemIfExists(SkeletonPoints.Shield);
            RemoveItemIfExists(SkeletonPoints.HandLeft);
            RemoveItemIfExists(SkeletonPoints.HandRight);
        }

        public void EquipmentDebug(GameObject item)
        {
            SpawnItemInSlot(SkeletonPoints.HandRight, item);
        }

        public void SetSkeletonAttachPoints(SkeletonAttachPoints sap)
        {
            _attachPoints = sap;
        }
        
        private static Dictionary<Equipment3dSlot, SkeletonPoints> _slotMapping = new Dictionary<Equipment3dSlot, SkeletonPoints>
        {
            {Equipment3dSlot.MainHand, SkeletonPoints.HandRight},
            {Equipment3dSlot.Shield, SkeletonPoints.Shield},
            {Equipment3dSlot.OffHand, SkeletonPoints.HandLeft},
        };

        public void OnSlotItemChanged(GameObject item, Equipment3dSlot attachPoint)
        {
            var skelepoint = _slotMapping[attachPoint];
            
            if (skelepoint == SkeletonPoints.None)
            {
                return;
            }
            
            // Head is considered a valid visible equip slot and for some reason, the model is a bag.
            // If we see that a bag is the equip graphic, we ignore it.
            //if (item?.idfile == "IT63")
            //{
            //    return;
           // }

            if (item == null)
            {
                RemoveItemIfExists(skelepoint);

                if (skelepoint == SkeletonPoints.HandLeft)
                {
                    RemoveItemIfExists(SkeletonPoints.Shield);
                }
            }
            else
            {
                if (skelepoint == SkeletonPoints.HandLeft)
                {
                    RemoveItemIfExists(SkeletonPoints.Shield);
                }
                
                if (skelepoint == SkeletonPoints.Shield)
                {
                    RemoveItemIfExists(SkeletonPoints.HandLeft);
                }
                
                SpawnItemInSlot(skelepoint, item);
            }
        }

        public void SpawnItemInSlot(SkeletonPoints point, GameObject item)
        {
            if (_spawnedEquipment == null)
            {
                _spawnedEquipment = new Dictionary<SkeletonPoints, GameObject>();
            }
            
            if (_spawnedEquipment.ContainsKey(point))
            {
                Debug.LogWarning("ITEM ALREADY IN attach point: " + point);
                RemoveItemIfExists(point);
            }

            var attachPoint = _attachPoints.GetAttachPoint(point);

            if (attachPoint == null)
            {
                Debug.LogError("Attach point not found for skeleton: " + point);
                return;
            }

            var instantiated = Instantiate(item, attachPoint);
            instantiated.layer = _dynamicLightSetter.gameObject.layer;
            GameObjectHelper.RemoveCloneAppend(instantiated);

            // TODO: Put into helper class
            byte[] bytes = System.BitConverter.GetBytes(_instanceId);
            Color32 targetColor = new Color32(bytes[0], bytes[1], bytes[2], bytes[3]);

            MeshRenderer mr = instantiated.GetComponent<MeshRenderer>();

            var materials = mr.sharedMaterials;

            var childSetter = instantiated.AddComponent<SunlightSetterChild>();
            if (_dynamicLightSetter != null)
            {
                _dynamicLightSetter.AddSunlightChild(childSetter);
            }
            
            var pb = new MaterialPropertyBlock();

            // Setting 
            /*for (int i = 0; i < materials.Length; i++)
            {
                mr.GetPropertyBlock(pb, i);
                pb.SetColor("_TargetColor", targetColor);
                mr.SetPropertyBlock(pb, i);
            }*/
            
            _spawnedEquipment[point] = instantiated;
        }

        public void SpawnNpcEquipment(GameObject item1, GameObject item2, SkeletonPoints item1Slot, SkeletonPoints item2Slot)
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

        public SkeletonPoints RemoveItemIfEquipped(string item)
        {
            if (_spawnedEquipment != null)
            {
                var entries = _spawnedEquipment.ToArray();
                foreach (var entry in entries)
                {
                    if (entry.Value.name == item)
                    {
                        if (RemoveItemIfExists(entry.Key))
                            return entry.Key;
                    }
                }
            }

            return SkeletonPoints.None;
        }

        private bool RemoveItemIfExists(SkeletonPoints point)
        {
            if (_spawnedEquipment == null || !_spawnedEquipment.ContainsKey(point))
            {
                return false;
            }
            
            var spawnedEquipment = _spawnedEquipment[point];
            var childSetter = spawnedEquipment.GetComponent<SunlightSetterChild>();

            if (childSetter != null && _dynamicLightSetter != null)
            {
                _dynamicLightSetter.RemoveSunlightChild(childSetter);
            }

            Destroy(spawnedEquipment);
            _spawnedEquipment.Remove(point);

            return true;
        }
    }
}
