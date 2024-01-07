using System;
using System.Collections.Generic;
using Lantern.EQ.Zone;

namespace Lantern.EQ.AssetBundles
{
    /// <summary>
    /// Provides info for LanternEQ asset bundle versioning.
    /// Leave this alone unless you know what you're doing.
    /// </summary>
    public static class AssetBundleVersions
    {
        public static Dictionary<LanternAssetBundleId, Version> Versions =
            new Dictionary<LanternAssetBundleId, Version>()
            {
                {LanternAssetBundleId.Music_Audio, new Version(0, 1, 7)},
                {LanternAssetBundleId.Music_Midi, new Version(0, 1, 7)},
                {LanternAssetBundleId.Characters, new Version(0, 1, 7)},
                {LanternAssetBundleId.Construct, new Version(0, 1, 7)},
                {LanternAssetBundleId.UI_Lantern, new Version(0, 1, 7)},
                {LanternAssetBundleId.ClientData, new Version(0, 1, 7)},
                {LanternAssetBundleId.Zones, new Version(0, 1, 7)},
                {LanternAssetBundleId.Equipment, new Version(0, 1, 7)},
                {LanternAssetBundleId.Sprites, new Version(0, 1, 7)},
                {LanternAssetBundleId.Startup, new Version(0, 1, 7)},
                {LanternAssetBundleId.Sky, new Version(0, 1, 7)},
                {LanternAssetBundleId.CharacterSelect_Classic, new Version(0, 1, 7)},
                {LanternAssetBundleId.Defaults, new Version(0, 1, 7)},
                {LanternAssetBundleId.Sound, new Version(0, 1, 7)},

                // Unused/old
                {LanternAssetBundleId.UI_Titanium, new Version(0, 1, 7)},
                {LanternAssetBundleId.UI_Debug, new Version(0, 1, 7)},
                {LanternAssetBundleId.Login_Dev, new Version(0, 1, 7)},
                {LanternAssetBundleId.Login_Classic, new Version(0, 1, 7)},

                // Removed
                //{LanternAssetBundleId.CharacterSelect_Dev, new Version(0, 1, 5)},
                //{LanternAssetBundleId.Shaders, new Version(0, 1, 5)},
                //{LanternAssetBundleId.UI_Dev, new Version(0, 1, 5)},
                //{LanternAssetBundleId.UI_Classic, new Version(0, 1, 4)},
            };

        public static Version GetVersion(LanternAssetBundleId bundleId)
        {
            if(!Versions.ContainsKey(bundleId))
            {
                return null;
            }

            return Versions[bundleId];
        }

        public static LanternAssetBundleId? GetBundleIdFromName(string name)
        {
            name = name.ToLower();
            if (ZoneHelper.IsValidZoneShortname(name))
            {
                return LanternAssetBundleId.Zones;
            }

            var values = Enum.GetValues(typeof(LanternAssetBundleId));
            foreach (LanternAssetBundleId value in values)
            {
                if (name != value.ToString().ToLower())
                {
                    continue;
                }

                return value;
            }

            return null;
        }
    }
}
