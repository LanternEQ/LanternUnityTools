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
                {LanternAssetBundleId.Characters, new Version(0, 1, 6)},
                {LanternAssetBundleId.Construct, new Version(0, 1, 6)},
                {LanternAssetBundleId.UI_Lantern, new Version(0, 1, 6)},
                {LanternAssetBundleId.ClientData, new Version(0, 1, 6)},
                {LanternAssetBundleId.Zones, new Version(0, 1, 5)},
                {LanternAssetBundleId.Equipment, new Version(0, 1, 5)},
                {LanternAssetBundleId.Sprites, new Version(0, 1, 5)},
                {LanternAssetBundleId.Startup, new Version(0, 1, 5)},
                {LanternAssetBundleId.Sky, new Version(0, 1, 5)},
                {LanternAssetBundleId.CharacterSelect_Classic, new Version(0, 1, 5)},
                {LanternAssetBundleId.Defaults, new Version(0, 1, 5)},
                {LanternAssetBundleId.Sound, new Version(0, 1, 0)},
                {LanternAssetBundleId.Music, new Version(0, 1, 0)},

                // Unused/old
                {LanternAssetBundleId.UI_Titanium, new Version(0, 1, 4)},
                {LanternAssetBundleId.UI_Debug, new Version(0, 1, 4)},
                {LanternAssetBundleId.Login_Dev, new Version(0, 1, 5)},
                {LanternAssetBundleId.Login_Classic, new Version(0, 1, 5)},

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
