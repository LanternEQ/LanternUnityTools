using UnityEngine;

namespace Lantern.Editor.Importers
{
    public static class PathHelper
    {
        public static string GetRootAssetPath()
        {
            #if LANTERN_CLIENT
            return "Assets/ZoneAssets/Classic/";
            #endif
            return "Assets/EQAssets/";
        }
        public static string GetLoadPath(string zoneName, AssetImportType assetImportType)
        {
            if (zoneName == "characters")
            {
                return GetRootAssetPath() + "characters/";
            }
            if (zoneName == "equipment" || assetImportType == AssetImportType.Equipment)
            {
                return GetRootAssetPath() + "equipment/";
            }
            
            string loadPath = GetRootAssetPath() + zoneName + "/";

            switch (assetImportType)
            {
                case AssetImportType.Zone:
                    loadPath += "Zone/";
                    break;
                case AssetImportType.Objects:
                    loadPath += "Objects/";
                    break;
                case AssetImportType.Characters:
                    loadPath += "Characters/";
                    break;
                case AssetImportType.Sky:
                    break;
            }

            return loadPath;
        }

        public static string GetSavePath(string zoneName, AssetImportType assetImportType, bool includeEndSlash = true)
        {
            if (assetImportType == AssetImportType.Characters)
            {
                return "Assets/Content/AssetsToBundle/Characters" + (includeEndSlash ? "/" : "");
            }
            if (assetImportType == AssetImportType.Equipment)
            {
                return "Assets/Content/AssetsToBundle/Equipment" + (includeEndSlash ? "/" : "");
            }

            string savePath = "Assets/Content/AssetsToBundle/Zones/" + zoneName + "/";

            switch (assetImportType)
            {
                case AssetImportType.Zone:
                    savePath += "Zone";
                    break;
                case AssetImportType.Objects:
                    savePath += "Objects";
                    break;
                case AssetImportType.Sky:
                    break;
            }

            if (includeEndSlash)
            {
                savePath += "/";
            }

            return savePath;
        }

        public static string GetRootSavePath(string zoneName, bool includeEndSlash = true)
        {
            string savePath = "Assets/Content/AssetsToBundle/Zones/" + zoneName;

            if (includeEndSlash)
            {
                savePath += "/";
            }

            return savePath;
        }

        public static string GetRootLoadPath(string zoneName, bool includeEndSlash = true)
        {
            string loadPath = GetRootAssetPath() + zoneName;

            if (includeEndSlash)
            {
                loadPath += "/";
            }

            return loadPath;
        }

        public static string GetClientDataPath()
        {
            return "Assets/Content/ClientData/";
        }

        public static string GetSystemPathFromUnity(string unityPath)
        {
            // Trims the Assets prefix as it's included in the data path as well
            string newPath = Application.dataPath + unityPath.Substring(6);
            return newPath;
        }
    }
}