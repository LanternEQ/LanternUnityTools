using System.Collections.Generic;
using System.Linq;
using Lantern.EQ.Animation;
using UnityEngine;

namespace Lantern.EQ.Equipment
{
    /// <summary>
    /// A directory class for all equipment models to avoid runtime component querying
    /// </summary>
    public class EquipmentModel : MonoBehaviour
    {
        public List<Renderer> Renderers;
        public EquipmentAnimation EquipmentAnimation;

        public void SetLayer(int layer)
        {
            foreach (var r in Renderers)
            {
                r.gameObject.layer = layer;
            }
        }

#if UNITY_EDITOR
        public void SetReferences(EquipmentAnimation equipmentAnimation)
        {
            EquipmentAnimation = equipmentAnimation;
            Renderers = GetComponentsInChildren<Renderer>().ToList();
        }
#endif
    }
}
