using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Lantern.Editor.Importers
{
    public static class SoundImporter
    {
        public static void CreateSoundInstances(string soundTextAssetPath, Transform soundRoot)
        {
            string soundInstanceList = string.Empty;

            ImportHelper.LoadTextAsset(soundTextAssetPath, out soundInstanceList);

            if (string.IsNullOrEmpty(soundInstanceList))
            {
                return;
            }

            GameObject sound2dTriggerPrefab = GetSound2dTriggerPrefab();
            GameObject sound3dTriggerPrefab = GetSound3dTriggerPrefab();

            if (sound2dTriggerPrefab == null || sound3dTriggerPrefab == null)
            {
                Debug.LogError("SoundImporter: Cannot get sound trigger prefabs.");
                return;
            }

            var parsedSoundLines = TextParser.ParseTextByDelimitedLines(soundInstanceList, ',');

            foreach (var instance in parsedSoundLines)
            {
                CreateSoundInstance(sound2dTriggerPrefab,
                    sound3dTriggerPrefab, instance, soundRoot);
            }
        }

        private static void CreateSoundInstance(GameObject sound2dTriggerPrefab,
            GameObject sound3dTriggerPrefab, List<string> soundData, Transform parent)
        {
            if (soundData.Count != 10)
            {
                Debug.LogError("SoundImporter: Unable to parse sound line. Unexpected item count");
                return;
            }

            int soundType = Convert.ToInt32(soundData[0]);
            float x = Convert.ToSingle(soundData[1]);
            float y = Convert.ToSingle(soundData[2]);
            float z = Convert.ToSingle(soundData[3]);
            float radius = Convert.ToSingle(soundData[4]);
            string clipNameDay = soundData[5];
            string clipNameNight = soundData[6];
            int cooldownDay = Convert.ToInt32(soundData[7]);
            int cooldownNight = Convert.ToInt32(soundData[8]);
            int cooldownRandom = Convert.ToInt32(soundData[9]);

            string dayClipPath = "Assets/Content/AssetsToBundle/Sound/" + clipNameDay + ".ogg";
            AudioClip dayClip = (AudioClip) AssetDatabase.LoadAssetAtPath(dayClipPath, typeof(AudioClip));

            string nightClipPath = "Assets/Content/AssetsToBundle/Sound/" + clipNameNight + ".ogg";
            AudioClip nightClip = (AudioClip) AssetDatabase.LoadAssetAtPath(nightClipPath, typeof(AudioClip));

            if (dayClip == null && nightClip == null)
            {
                if (clipNameDay != string.Empty)
                {
                    Debug.LogError("SoundImporter: Unable to load day clip: " + clipNameDay);
                }
                
                if (clipNameNight != string.Empty)
                {
                    Debug.LogError("SoundImporter: Unable to load night clip: " + clipNameNight);
                }
                
                return;
            }
            
            GameObject soundTrigger =
                (GameObject) PrefabUtility.InstantiatePrefab(soundType == 0
                    ? sound2dTriggerPrefab
                    : sound3dTriggerPrefab);

            soundTrigger.transform.parent = parent;
            soundTrigger.transform.position = new Vector3(x, y, z);

            /*if (soundType == 0)
            {
                var sound2dScript = soundTrigger.GetComponent<Sound2dTrigger>();
                sound2dScript.SetData(dayClip, nightClip, radius, cooldownDay, cooldownNight, cooldownRandom);
            }
            else
            {
                var sound3dScript = soundTrigger.GetComponent<Sound3dTrigger>();
                sound3dScript.SetData(dayClip, radius, cooldownDay, cooldownNight, cooldownRandom);
            }*/
        }

        private static GameObject GetSound2dTriggerPrefab()
        {
            var soundTriggerPrefabPath = "Assets/Content/Prefabs/Sound2dTrigger.Prefab";
            return AssetDatabase.LoadAssetAtPath<GameObject>(soundTriggerPrefabPath);
        }

        private static GameObject GetSound3dTriggerPrefab()
        {
            var soundTriggerPrefabPath = "Assets/Content/Prefabs/Sound3dTrigger.Prefab";
            return AssetDatabase.LoadAssetAtPath<GameObject>(soundTriggerPrefabPath);
        }
    }
}