using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Infrastructure.EQ.TextParser;
using Lantern.EQ.Audio;
using Lantern.EQ.Editor.Helpers;
using Lantern.EQ.Lantern;
using UnityEditor;
using UnityEngine;

namespace Lantern.EQ.Editor.Importers
{
    public static class MusicImporter
    {
        public static void CreateMusicInstances(string musicTextAssetPath, Transform musicRoot, string shortName)
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

            var trackNames = GetTrackNamesForZone(shortName);
            var parsedMusicLines = TextParser.ParseTextByDelimitedLines(musicInstanceList, ',');

            foreach (var instance in parsedMusicLines)
            {
                CreateMusicInstance(musicTriggerPrefab, instance, trackNames, musicRoot);
            }
        }

        private static void CreateMusicInstance(GameObject musicTriggerPrefab, List<string> musicLines, List<string> trackNames, Transform parent)
        {
            if (musicLines.Count != 9)
            {
                Debug.LogError("MusicImporter: Unable to parse music line. Unexpected item count");
                return;
            }

            var musicTrigger =
                ((GameObject) PrefabUtility.InstantiatePrefab(musicTriggerPrefab)).GetComponent<MusicTrigger>();

            musicTrigger.transform.parent = parent;
            musicTrigger.gameObject.layer = 2; // Ignore Raycast Layer

            float x = Convert.ToSingle(musicLines[0]);
            float y = Convert.ToSingle(musicLines[1]);
            float z = Convert.ToSingle(musicLines[2]);
            musicTrigger.transform.position = new Vector3(x, y, z);

            float radius = Convert.ToSingle(musicLines[3]);
            int trackIndexDay = Convert.ToInt32(musicLines[4]);
            int trackIndexNight = Convert.ToInt32(musicLines[5]);
            int playCountDay = Convert.ToInt32(musicLines[6]);
            int playCountNight = Convert.ToInt32(musicLines[7]);
            int fadeOutMs = Convert.ToInt32(musicLines[8]);

            var musicData = new MusicData()
            {
                TrackIndexDay = trackIndexDay,
                TrackIndexNight = trackIndexNight,
                PlayCountDay = playCountDay,
                PlayCountNight = playCountNight,
                FadeOutMsDay = fadeOutMs,
                FadeOutMsNight = fadeOutMs,
            };

            musicTrigger.SetData(LanternTags.Player, radius, musicData);
        }

        private static List<string> GetTrackNamesForZone(string shortname)
        {
            string assetPath = PathHelper.GetClientDataPath() + "music_tracks.txt";
            if (!ImportHelper.LoadTextAsset(assetPath, out var musicTrackFile))
            {
                return null;
            }

            var musicTrackLines = TextParser.ParseTextByDelimitedLines(musicTrackFile, ',');
            var trackNames = musicTrackLines.FirstOrDefault(x => x[0] == shortname);
            trackNames?.RemoveAt(0);
            return trackNames ?? new List<string>();
        }

        private static GameObject GetMusicTriggerPrefab()
        {
            var musicTriggerPrefabPath = "Assets/Content/Features/Game/MusicTrigger.Prefab";
            return AssetDatabase.LoadAssetAtPath<GameObject>(musicTriggerPrefabPath);
        }
    }
}
