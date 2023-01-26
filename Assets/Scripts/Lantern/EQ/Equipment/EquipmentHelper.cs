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
            { "hum", "IT627" },
            { "huf", "IT620" },
            { "bam", "IT537" },
            { "baf", "IT530" },
            { "erm", "IT575" },
            { "erf", "IT570" },
            { "elm", "IT565" },
            { "elf", "IT561" },
            { "him", "IT605" },
            { "hif", "IT600" },
            { "dam", "IT545" },
            { "daf", "IT540" },
            { "ham", "IT595" },
            { "haf", "IT590" },
            { "dwm", "IT557" },
            { "dwf", "IT550" },
            { "trm", "IT655" },
            { "trf", "IT650" },
            { "ogm", "IT645" },
            { "ogf", "IT640" },
            { "hom", "IT615" },
            { "hof", "IT610" },
            { "gnm", "IT585" },
            { "gnf", "IT580" },
            { "ikm", "IT635" },
            { "ikf", "IT630" },
        };
    }
}
