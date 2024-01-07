using System;
using System.Collections.Generic;
using Lantern.EQ.Animation;
using Lantern.EQ.Audio;

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

        public static EquipmentSound GetSoundForEquipment(int itemNumber)
        {
            switch (itemNumber)
            {
                case 0:
                    return EquipmentSound.HandToHand;
                case 4:
                    return EquipmentSound.Bow;
                case 61:
                    return EquipmentSound.Whip;
                case 1:
                case 2:
                case 3:
                case 5:
                case 7:
                case 9:
                case 16:
                case 17:
                case 19:
                case 20:
                case 23:
                case 24:
                case 25:
                case 30:
                case 34:
                case 35:
                case 37:
                case 39:
                case 40:
                case 41:
                case 42:
                case 43:
                case 44:
                case 50:
                case 51:
                case 53:
                case 57:
                case 58:
                case 59:
                case 60:
                case 62:
                    return EquipmentSound.Slashing;
                case int x when x is >= 200 and <= 300:
                    return EquipmentSound.Bash;
                default:
                    return EquipmentSound.Blunt;
            }
        }
    }
}
