using System;
using System.Collections.Generic;
using Lantern.Logic;
using UnityEditor;
using UnityEngine;

namespace Lantern.Editor.Importers
{
    public static class MusicImporter
    {
        /*public static void CreateMusicInstances(string musicTextAssetPath, Transform musicRoot)
        {
            string musicInstanceList = string.Empty;

            ImportHelper.LoadTextAsset(musicTextAssetPath, out musicInstanceList);

            if (string.IsNullOrEmpty(musicInstanceList))
            {
                return;
            }

            GameObject musicTriggerPrefab = GetMusicTriggerPrefab();
            
            if (musicTriggerPrefab == null)
            {
                Debug.LogError("MusicImporter: Cannot get music trigger prefab.");
                return;
            }

            var parsedMusicLines = TextParser.ParseTextByDelimitedLines(musicInstanceList, ',');
            
            foreach (var instance in parsedMusicLines)
            {
                CreateMusicInstance(musicTriggerPrefab, instance, musicRoot);
            }
        }

        private static void CreateMusicInstance(GameObject musicTriggerPrefab, List<string> musicData, Transform parent)
        {
            if (musicData.Count != 9)
            {
                Debug.LogError("MusicImporter: Unable to parse music line. Unexpected item count");
                return;
            }
            
            MusicTrigger musicTrigger =
                ((GameObject) PrefabUtility.InstantiatePrefab(musicTriggerPrefab)).GetComponent<MusicTrigger>();

            musicTrigger.transform.parent = parent;
            musicTrigger.gameObject.layer = 2; // Ignore Raycast Layer
            
            float x = Convert.ToSingle(musicData[0]);
            float y = Convert.ToSingle(musicData[1]);
            float z = Convert.ToSingle(musicData[2]);

            musicTrigger.transform.position = new Vector3(x, y, z);

            float radius = Convert.ToSingle(musicData[3]);
            string trackNameDay = musicData[4];
            string trackNameNight = musicData[5];
            int loopCountDay = Convert.ToInt32(musicData[6]);
            int loopCountNight = Convert.ToInt32(musicData[7]);
            int fadeOutTimeMs = Convert.ToInt32(musicData[8]);

            musicTrigger.SetData(radius, trackNameDay, trackNameNight, loopCountDay, loopCountNight, fadeOutTimeMs);
        }

        private static GameObject GetMusicTriggerPrefab()
        {
            var musicTriggerPrefabPath = "Assets/Content/Prefabs/MusicTrigger.Prefab";
            return AssetDatabase.LoadAssetAtPath<GameObject>(musicTriggerPrefabPath);
        }*/
    }
}
