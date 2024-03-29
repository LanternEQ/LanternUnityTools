﻿using System;
using Infrastructure.EQ.TextParser;
using Lantern.EQ.Editor.Helpers;
using UnityEditor;
using UnityEngine;

namespace Lantern.Editor.Importers
{
    public static class ActorStaticImporter
    {
        public static void ImportList(string shortname, AssetImportType importType,
            Action<GameObject> postProcess = null)
        {
            string assetPath = PathHelper.GetLoadPath(shortname, importType) + "actors_static.txt";

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

        public static void Import(string shortname, string assetName, AssetImportType importType,
            Action<GameObject> postProcess = null)
        {
            MeshImporter.Import(shortname, assetName + "_collision", null, null, importType, true, out _, false,
                postProcess);
            MeshImporter.Import(shortname, assetName, null, null, importType, false, out _, true, postProcess);
            EditorUtility.UnloadUnusedAssetsImmediate();
        }
    }
}
