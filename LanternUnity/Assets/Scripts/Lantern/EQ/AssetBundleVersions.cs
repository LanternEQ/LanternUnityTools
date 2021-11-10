using System;
using System.Collections.Generic;
using Lantern.Services;

namespace Lantern.Global.AssetBundles
{
    public static class AssetBundleVersions
    {
        public static Dictionary<GlobalAssetBundleId, Version> Versions =
            new Dictionary<GlobalAssetBundleId, Version>()
            {
                {GlobalAssetBundleId.Sprites, new Version(0, 1, 4)},
                {GlobalAssetBundleId.Sound, new Version(0, 1, 0)},
                {GlobalAssetBundleId.Music, new Version(0, 1, 0)},
                {GlobalAssetBundleId.Shaders, new Version(0, 1, 4)},
                {GlobalAssetBundleId.Characters, new Version(0, 1, 4)},
                {GlobalAssetBundleId.Construct, new Version(0, 1, 4)},
                {GlobalAssetBundleId.Zones, new Version(0, 1, 4)},
                {GlobalAssetBundleId.Ui_Dev, new Version(0, 1, 4)},
                {GlobalAssetBundleId.Ui_Titanium, new Version(0, 1, 4)},
                {GlobalAssetBundleId.Ui_Lantern, new Version(0, 1, 4)},
                {GlobalAssetBundleId.Ui_Classic, new Version(0, 1, 4)},
                {GlobalAssetBundleId.Startup, new Version(0, 1, 4)},
                {GlobalAssetBundleId.Sky, new Version(0, 1, 0)},
                {GlobalAssetBundleId.Equipment, new Version(0, 1, 4)},
                {GlobalAssetBundleId.Ui_Debug, new Version(0, 1, 4)},
                {GlobalAssetBundleId.Login_Dev, new Version(0, 1, 5)},
                {GlobalAssetBundleId.Login_Classic, new Version(0, 1, 5)},
                {GlobalAssetBundleId.CharacterSelect_Classic, new Version(0, 1, 5)},
                {GlobalAssetBundleId.CharacterSelect_Dev, new Version(0, 1, 5)},
            };

        public static HashSet<GlobalAssetBundleId> RequiredBundles =
            new HashSet<GlobalAssetBundleId>
            {
                GlobalAssetBundleId.Shaders,
                GlobalAssetBundleId.Construct,
                GlobalAssetBundleId.Ui_Dev,
                GlobalAssetBundleId.Ui_Debug,
            };

        public static Version GetVersion(GlobalAssetBundleId bundleId)
        {
            if(!Versions.ContainsKey(bundleId))
            {
                return null;
            }

            return Versions[bundleId]; 
        }

        public static GlobalAssetBundleId? GetBundleIdFromName(string name)
        {
            name = name.ToLower();
            if (ZoneHelper.IsValidZoneShortname(name))
            {
                return GlobalAssetBundleId.Zones;
            }
            
            var values = Enum.GetValues(typeof(GlobalAssetBundleId));
            foreach (GlobalAssetBundleId value in values)
            {
                if (name != value.ToString().ToLower())
                {
                    continue;
                }

                return value;
            }

            return null;
        }
        
        public static string GetGlobalBundleName(GlobalAssetBundleId bundleId)
        {
            if (!Versions.ContainsKey(bundleId))
            {
                return string.Empty;
            }
            
            return bundleId.ToString().ToLower() + "-" +
                   Versions[bundleId].ToString().Replace('.', '_');
        }

        public static string GetZoneBundleName(string shortname)
        {
            return shortname + "-" +
                   Versions[GlobalAssetBundleId.Zones].ToString().Replace('.', '_');
        }
    }
}