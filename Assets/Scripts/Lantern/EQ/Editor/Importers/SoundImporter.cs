using System;
using System.Collections.Generic;
using Infrastructure.EQ.TextParser;
using Lantern.EQ.Audio;
using Lantern.EQ.Editor.Helpers;
using Lantern.EQ.Lantern;
using UnityEditor;
using UnityEngine;

namespace Lantern.EQ.Editor.Importers
{
    public static class SoundImporter
    {
        public static void CreateSoundInstances(string sound2dTextAssetPath, string sound3dTextAssetPath,
            Transform soundRoot)
        {
            ImportHelper.LoadTextAsset(sound2dTextAssetPath, out var sound2dInstanceList);

            if (!string.IsNullOrEmpty(sound2dInstanceList))
            {
                GameObject sound2dTriggerPrefab = GetSound2dTriggerPrefab();

                if (sound2dTriggerPrefab == null)
                {
                    Debug.LogError("Could not load sound 2d trigger!");
                }
                else
                {
                    var parsedSound2dLines = TextParser.ParseTextByDelimitedLines(sound2dInstanceList, ',');

                    foreach (var instance in parsedSound2dLines)
                    {
                        CreateSound2dInstance(sound2dTriggerPrefab, instance, soundRoot);
                    }
                }
            }

            ImportHelper.LoadTextAsset(sound3dTextAssetPath, out var sound3dInstanceList);

            if (!string.IsNullOrEmpty(sound3dInstanceList))
            {
                GameObject sound3dTriggerPrefab = GetSound3dTriggerPrefab();

                if (sound3dTriggerPrefab == null)
                {
                    Debug.LogError("Could not load sound 2d trigger!");
                }
                else
                {
                    var parsedSound3dLines = TextParser.ParseTextByDelimitedLines(sound3dInstanceList, ',');

                    foreach (var instance in parsedSound3dLines)
                    {
                        CreateSound3dInstance(sound3dTriggerPrefab, instance, soundRoot);
                    }
                }
            }
        }

        private static void CreateSound2dInstance(GameObject sound2dTriggerPrefab, List<string> soundLines, Transform parent)
        {
            if (soundLines.Count != 11)
            {
                Debug.LogError("SoundImporter: Unable to parse sound2D. Unexpected argument count");
                return;
            }

            float x = Convert.ToSingle(soundLines[0]);
            float y = Convert.ToSingle(soundLines[1]);
            float z = Convert.ToSingle(soundLines[2]);
            float radius = Convert.ToSingle(soundLines[3]);
            string clipNameDay = soundLines[4];
            string clipNameNight = soundLines[5];
            int cooldownDay = Convert.ToInt32(soundLines[6]);
            int cooldownNight = Convert.ToInt32(soundLines[7]);
            int cooldownRandom = Convert.ToInt32(soundLines[8]);
            float volumeDay = Convert.ToSingle(soundLines[9]);
            float volumeNight = Convert.ToSingle(soundLines[10]);

            GameObject soundTrigger = (GameObject)PrefabUtility.InstantiatePrefab(sound2dTriggerPrefab);
            soundTrigger.transform.parent = parent;
            soundTrigger.transform.position = new Vector3(x, y, z);

            var soundData = new Sound2dData
            {
                ClipNameDay = clipNameDay,
                ClipNameNight = clipNameNight,
                CooldownDay = cooldownDay,
                CooldownNight = cooldownNight,
                CooldownRandom = cooldownRandom,
                VolumeDay = volumeDay,
                VolumeNight = volumeNight
            };

            var script = soundTrigger.GetComponent<SoundTrigger2d>();
            script.SetData(soundData, LanternTags.Player, radius);
        }

        private static void CreateSound3dInstance(GameObject sound3dTriggerPrefab, List<string> soundLines, Transform parent)
        {
            if (soundLines.Count != 9)
            {
                Debug.LogError("SoundImporter: Unable to parse sound3D. Unexpected argument count");
                return;
            }

            float x = Convert.ToSingle(soundLines[0]);
            float y = Convert.ToSingle(soundLines[1]);
            float z = Convert.ToSingle(soundLines[2]);
            float radius = Convert.ToSingle(soundLines[3]);
            string clipName = soundLines[4];
            int cooldown = Convert.ToInt32(soundLines[5]);
            int cooldownRandom = Convert.ToInt32(soundLines[6]);
            float volume = Convert.ToSingle(soundLines[7]);
            int multiplier = Convert.ToInt32(soundLines[8]);

            GameObject soundTrigger = (GameObject)PrefabUtility.InstantiatePrefab(sound3dTriggerPrefab);
            soundTrigger.transform.parent = parent;
            soundTrigger.transform.position = new Vector3(x, y, z);

            var soundData = new Sound3dData
            {
                ClipName = clipName,
                Cooldown = cooldown,
                CooldownRandom = cooldownRandom,
                Volume = volume,
                Multiplier = multiplier
            };

            var script = soundTrigger.GetComponent<SoundTrigger3d>();
            script.SetData(soundData, LanternTags.Player, radius);
        }

        private static GameObject GetSound2dTriggerPrefab()
        {
            var prefabPath = "Assets/Content/Features/Game/SoundTrigger2d.prefab";
            return AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        }

        private static GameObject GetSound3dTriggerPrefab()
        {
            var prefabPath = "Assets/Content/Features/Game/SoundTrigger3d.prefab";
            return AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        }
    }
}
