using System;
using Lantern.Editor.Importers;
using UnityEngine;

namespace Lantern.EQ.Editor.Helpers
{
    public static class PathHelper
    {
        public static string GetEqAssetPath()
        {
            return "Assets/EQAssets/";
        }
        public static string GetLoadPath(string zoneName, AssetImportType assetImportType)
        {
            if (zoneName == "characters")
            {
                return GetEqAssetPath() + "characters/";
            }
            if (zoneName == "equipment" || assetImportType == AssetImportType.Equipment)
            {
                return GetEqAssetPath() + "equipment/";
            }

            string loadPath = GetEqAssetPath() + zoneName + "/";

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

        public static string GetAssetBundleContentPath()
        {
            return "Assets/Content/AssetBundleContent/";
        }

        public static string GetSavePath(string zoneName, AssetImportType assetImportType, bool includeEndSlash = true)
        {
            switch (assetImportType)
            {
                case AssetImportType.Characters:
                case AssetImportType.Equipment:
                case AssetImportType.Sky:
                {
                    return GetAssetBundleContentPath() + assetImportType + (includeEndSlash ? "/" : "");
                }
            }

            string savePath = GetAssetBundleContentPath() + "Zones/" + zoneName + "/";

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
            string savePath = GetAssetBundleContentPath() + "Zones/" + zoneName;

            if (includeEndSlash)
            {
                savePath += "/";
            }

            return savePath;
        }

        public static string GetRootLoadPath(string zoneName, bool includeEndSlash = true)
        {
            string loadPath = GetEqAssetPath() + zoneName;

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

        public static string GetUnityPathFromSystem(string systemPath)
        {
            var index = systemPath.IndexOf("Assets/", StringComparison.Ordinal);
            return index < 0 ? string.Empty : systemPath.Substring(index);
        }
    }
}
