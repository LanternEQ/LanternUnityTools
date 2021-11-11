using System;
using UnityEditor;
using UnityEngine;

namespace Lantern.Editor.Importers
{
    public static class ActorSkeletalImporter
    {
        public static void ImportList(string shortname, AssetImportType importType, Action<GameObject> postProcess = null)
        {
            string assetPath = PathHelper.GetLoadPath(shortname, importType) + "actors_skeletal.txt";

            if (!ImportHelper.LoadTextAsset(assetPath, out var actorListAsset))
            {
                return;
            }

            var actorListLines = TextParser.ParseTextByDelimitedLines(actorListAsset, ',');

            foreach (var actor in actorListLines)
            {
                Import(shortname, actor[1], importType, postProcess);
            }
        }

        private static void Import(string shortname, string assetName, AssetImportType importType,
            Action<GameObject> postProcess = null)
        {
            SkeletonImporter.Import(assetName, shortname, importType, postProcess);
            EditorUtility.UnloadUnusedAssetsImmediate();
        }
    }
}