using System;
using System.Collections.Generic;
using Lantern.EQ.Animation;

namespace Lantern.EQ.Equipment
{
    public static class EquipmentHelper
    {
        public static SkeletonPoints GetSkeletonAttachPoint(Equipment3dSlot slot)
        {
            switch (slot)
            {
                case Equipment3dSlot.MainHand:
                    return SkeletonPoints.HandRight;
                case Equipment3dSlot.OffHand:
                case Equipment3dSlot.Ranged:
                    return SkeletonPoints.HandLeft;
                case Equipment3dSlot.Shield:
                    return SkeletonPoints.Shield;
                case Equipment3dSlot.Helm:
                    return SkeletonPoints.Head;
                default:
                    throw new ArgumentOutOfRangeException(nameof(slot), slot, null);
            }
        }

        public static string GetVeliousHelmet(string modelName)
        {
            if (string.IsNullOrEmpty(modelName) || !_veliousHelmets.ContainsKey(modelName))
                return null;

            return _veliousHelmets[modelName];
        }

        // https://i.imgur.com/zSkreBF.jpeg
        private static readonly Dictionary<string, string> _veliousHelmets = new Dictionary<string, string>()
        {
            { "HUM", "IT627" },
            { "HUF", "IT620" },
            { "BAM", "IT537" },
            { "BAF", "IT530" },
            { "ERM", "IT575" },
            { "ERF", "IT570" },
            { "ELM", "IT565" },
            { "ELF", "IT561" },
            { "HIM", "IT605" },
            { "HIF", "IT600" },
            { "DAM", "IT545" },
            { "DAF", "IT540" },
            { "HAM", "IT595" },
            { "HAF", "IT590" },
            { "DWM", "IT557" },
            { "DWF", "IT550" },
            { "TRM", "IT655" },
            { "TRF", "IT650" },
            { "OGM", "IT645" },
            { "OGF", "IT640" },
            { "HOM", "IT615" },
            { "HOF", "IT610" },
            { "GNM", "IT585" },
            { "GNF", "IT580" },
            { "IKM", "IT635" },
            { "IKF", "IT630" },
        };
    }
}
