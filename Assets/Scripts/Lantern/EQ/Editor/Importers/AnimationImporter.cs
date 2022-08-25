using System;
using System.Collections.Generic;
using System.IO;
using Lantern.Helpers;
using UnityEditor;
using UnityEngine;

namespace Lantern.Editor.Importers
{
    public static class AnimationImporter
    {
        public static AnimationClip CreateDefaultAnimations(string modelName, string path, AssetImportType importType,
            string shortname, bool isCharacterAnimation)
        {
            var pathToAnimationText = path;

            TextAsset animationAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(pathToAnimationText);

            if (animationAsset == null)
            {
                return null;
            }

            AnimationClip newClip =
                ImportAnimation(animationAsset, modelName, "pos", isCharacterAnimation);

            string savePath = PathHelper.GetSavePath(shortname, importType) + "Animations/";
            string folderPath = PathHelper.GetSystemPathFromUnity(savePath);
            Directory.CreateDirectory(folderPath);

            AssetDatabase.CreateAsset(newClip, savePath + $"{modelName}_pos.anim");

            return newClip;
        }
        
        public static AnimationClip ImportAnimation(TextAsset textAsset, string modelType, string animationName, bool isCharacterAnimation)
        {
            if (textAsset == null)
            {
                return null;
            }

            List<List<string>> parsedLines = TextParser.ParseTextByDelimitedLines(textAsset.text, ',');

            AnimationClip clip = new AnimationClip();

            if(animationName != "pos")
            {
                AnimationEvent animationStartedEvent = new AnimationEvent
                {
                    functionName = "AnimationPlayed",
                    time =  0f,
                    stringParameter = animationName.Split('_')[0]
                };
                
                AnimationUtility.SetAnimationEvents(clip, new []{animationStartedEvent});
            }
            
            AnimationCurve posXCurve = new AnimationCurve();
            AnimationCurve posYCurve = new AnimationCurve();
            AnimationCurve posZCurve = new AnimationCurve();
            AnimationCurve rotXCurve = new AnimationCurve();
            AnimationCurve rotYCurve = new AnimationCurve();
            AnimationCurve rotZCurve = new AnimationCurve();
            AnimationCurve rotWCurve = new AnimationCurve();
            AnimationCurve scaleCurve = new AnimationCurve();
            
            int frameCount = Convert.ToInt32(parsedLines[0][1]);
            parsedLines.RemoveAt(0);

            if (animationName == "pos")
            {
                frameCount = 1;
            }

            int totalTimeMs = Convert.ToInt32(parsedLines[0][1]);
            float totalTime = totalTimeMs == 0 ? 1.0f : totalTimeMs / 1000f;
            parsedLines.RemoveAt(0);
            
            string currentBoneRaw = string.Empty;
            foreach (var line in parsedLines)
            {
                if (line[0] != currentBoneRaw)
                {
                    if (currentBoneRaw != string.Empty)
                    {
                        clip.SetCurve(currentBoneRaw, typeof(Transform), "localPosition.x", posXCurve);
                        clip.SetCurve(currentBoneRaw, typeof(Transform), "localPosition.y", posYCurve);
                        clip.SetCurve(currentBoneRaw, typeof(Transform), "localPosition.z", posZCurve);
                        clip.SetCurve(currentBoneRaw, typeof(Transform), "localRotation.x", rotXCurve);
                        clip.SetCurve(currentBoneRaw, typeof(Transform), "localRotation.y", rotYCurve);
                        clip.SetCurve(currentBoneRaw, typeof(Transform), "localRotation.z", rotZCurve);
                        clip.SetCurve(currentBoneRaw, typeof(Transform), "localRotation.w", rotWCurve);
                        clip.SetCurve(currentBoneRaw, typeof(Transform), "localScale.x", scaleCurve);
                        clip.SetCurve(currentBoneRaw, typeof(Transform), "localScale.y", scaleCurve);
                        clip.SetCurve(currentBoneRaw, typeof(Transform), "localScale.z", scaleCurve);
                        
                        posXCurve.keys = null;
                        posYCurve.keys = null;
                        posZCurve.keys = null;
                        rotXCurve.keys = null;
                        rotYCurve.keys = null;
                        rotZCurve.keys = null;
                        rotWCurve.keys = null;
                        scaleCurve.keys = null;
                    }
                    
                    currentBoneRaw = line[0];
                }

                int frameNumber = Convert.ToInt32(line[1]);
                float xPos = Convert.ToSingle(line[2]);
                float yPos = Convert.ToSingle(line[3]);
                float zPos = Convert.ToSingle(line[4]);
                float xRot = Convert.ToSingle(line[5]);
                float yRot = Convert.ToSingle(line[6]);
                float zRot = Convert.ToSingle(line[7]);
                float wRot = Convert.ToSingle(line[8]);
                float scale = Convert.ToSingle(line[9]);

                int frameMs = Convert.ToInt32(line[10]);

                if (isCharacterAnimation)
                {
                    frameMs = totalTimeMs / frameCount;
                }
                
                float frameTime = frameMs == 0 ? 0.0f : frameMs * frameNumber / 1000f;

                posXCurve.AddKey(frameTime, xPos);
                posYCurve.AddKey(frameTime, yPos);
                posZCurve.AddKey(frameTime, zPos);
                rotXCurve.AddKey(frameTime, xRot);
                rotYCurve.AddKey(frameTime, yRot);
                rotZCurve.AddKey(frameTime, zRot);
                rotWCurve.AddKey(frameTime, wRot);
                scaleCurve.AddKey(frameTime, scale);

                // We need to duplicate the first frame as the last frame
                if (frameNumber == 0 && AnimationHelper.IsLoopingAnimation(animationName))
                {
                    frameTime = totalTime;
                    posXCurve.AddKey(frameTime, xPos);
                    posYCurve.AddKey(frameTime, yPos);
                    posZCurve.AddKey(frameTime, zPos);
                    rotXCurve.AddKey(frameTime, xRot);
                    rotYCurve.AddKey(frameTime, yRot);
                    rotZCurve.AddKey(frameTime, zRot);
                    rotWCurve.AddKey(frameTime, wRot);
                    scaleCurve.AddKey(frameTime, scale);
                }
            }
        
            clip.SetCurve(currentBoneRaw, typeof(Transform), "localPosition.x", posXCurve);
            clip.SetCurve(currentBoneRaw, typeof(Transform), "localPosition.y", posYCurve);
            clip.SetCurve(currentBoneRaw, typeof(Transform), "localPosition.z", posZCurve);
            clip.SetCurve(currentBoneRaw, typeof(Transform), "localRotation.x", rotXCurve);
            clip.SetCurve(currentBoneRaw, typeof(Transform), "localRotation.y", rotYCurve);
            clip.SetCurve(currentBoneRaw, typeof(Transform), "localRotation.z", rotZCurve);
            clip.SetCurve(currentBoneRaw, typeof(Transform), "localRotation.w", rotWCurve);
            clip.SetCurve(currentBoneRaw, typeof(Transform), "localScale.x", scaleCurve);
            clip.SetCurve(currentBoneRaw, typeof(Transform), "localScale.y", scaleCurve);
            clip.SetCurve(currentBoneRaw, typeof(Transform), "localScale.z", scaleCurve);

            // Clip needs to be marked as legacy to be used without an animator
            clip.legacy = true;
            
            clip.name = modelType + "_" + animationName;
            
            if (AnimationHelper.IsLoopingAnimation(animationName))
            {
                clip.wrapMode = WrapMode.Loop;
            }
            else
            {
                clip.wrapMode = WrapMode.Once;
            }

            // Clip needs to have this enabled to prevent model twitching
            clip.EnsureQuaternionContinuity();

            return clip;
        }
    }
}
