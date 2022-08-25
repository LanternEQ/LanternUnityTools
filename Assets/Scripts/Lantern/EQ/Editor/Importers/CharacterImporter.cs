using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lantern.Editor.Helpers;
using Lantern.EQ;
using Lantern.EQ.Animation;
using Lantern.Helpers;
using Lantern.Logic;
using UnityEditor;
using UnityEngine;

namespace Lantern.Editor.Importers
{
    public class CharacterImporter : EditorWindow
    {
        /// <summary>
        /// Zone for which characters will be imported
        /// </summary>
        private static string _zoneShortname;

        /// <summary>
        /// A list of alternate animation sources for character models
        /// Loaded once at beginning of import
        /// </summary>
        private static Dictionary<string, string> _animationModelSources;

        private static List<ModelSound> _modelSounds;
        private static List<AnimationClip> _animations;

        [MenuItem("EQ/Import/Characters", false, 50)]
        public static void ShowImportDialog()
        {
            GetWindow(typeof(CharacterImporter), true, "Import Characters");
        }

        /// <summary>
        /// Draws the settings window for the character importer
        /// </summary>
        private void OnGUI()
        {
            int minHeight = 60;
            minSize = maxSize = new Vector2(225, minHeight);
            EditorGUIUtility.labelWidth = 100;
            _zoneShortname = EditorGUILayout.TextField("Zone Shortname", _zoneShortname);
            EditorGUILayout.Space();

            Rect r = EditorGUILayout.BeginHorizontal("Button");
            if (GUI.Button(r, GUIContent.none))
            {
                Close();
                Import();
            }

            GUILayout.Label("Import");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        private static void Import()
        {
            var startTime = EditorApplication.timeSinceStartup;
            _modelSounds = RaceSoundsDataParser.GetModelSounds();
            _animations = new List<AnimationClip>();
            CreateModelAnimationLink();
            TextureHelper.CopyTextures(_zoneShortname, AssetImportType.Characters);
            ActorStaticImporter.ImportList("characters", AssetImportType.Characters);
            ActorSkeletalImporter.ImportList("characters", AssetImportType.Characters, PostProcess);
            EditorUtility.DisplayDialog("CharacterImport",
                $"Character import finished in {(int) (EditorApplication.timeSinceStartup - startTime)} seconds", "OK");
        }

        private static void PostProcess(GameObject obj)
        {
            string modelAsset = obj.name;
            var skeleton = obj;
            VariantHandler variantHandler = null;

            skeleton.AddComponent<UniversalAnimationController>();
            var ap = skeleton.AddComponent<SkeletonAttachPoints>();
            ap.FindAttachPoints();
            var e3d = skeleton.AddComponent<Equipment3dHandler>();
            e3d.SetSkeletonAttachPoints(ap);
            var animatedObject = skeleton.GetComponent<AnimatedObject>();
            DestroyImmediate(animatedObject);

            var animation = skeleton.GetComponent<Animation>();
            string modelLink = GetAnimationModelLink(modelAsset);

            AddModelAnimations(animation, modelAsset, modelLink);

            skeleton.name = modelAsset;

            // Find all meshes for this model
            string path = _zoneShortname == "characters"
                ? PathHelper.GetRootAssetPath() + "Characters"
                : $"{PathHelper.GetRootAssetPath()}/{_zoneShortname}/Characters";
            string[] meshGuidPaths = AssetDatabase.FindAssets(modelAsset,
                new[] {path});

            if (meshGuidPaths == null || meshGuidPaths.Length == 0)
            {
                Debug.LogError("No mesh guids found for model " + modelAsset);
                return;
            }

            // Find animations
            string rootName = modelAsset;
            if (animation != null)
            {
                GetAdditionalAnimations(animation, rootName, AssetImportType.Characters);
                
                if (_animationModelSources.ContainsKey(rootName))
                {
                    GetAdditionalAnimations(animation, _animationModelSources[rootName], AssetImportType.Characters);
                }
            }

            GameObject lastPrimaryMesh = null;

            variantHandler = skeleton.GetComponent<VariantHandler>();

            if (RaceHelper.IsPlayableRace(modelAsset))
            {
                FindEquipmentTextures(modelAsset, variantHandler);
                FindAdditionalFaces(variantHandler.GetLastPrimaryMesh(), variantHandler);
            }

            var vertexColorDebug = skeleton.AddComponent<SunlightSetterDynamic>();
            vertexColorDebug.FindChildRenderers();

            LoadCharacterSounds(skeleton, _modelSounds);
        }

        public static void GetAdditionalAnimations(Animation animation, string animationBase, AssetImportType type)
        {
            var searchPath = PathHelper.GetLoadPath(_zoneShortname, type) + "Animations";
            var assets = AssetDatabase.FindAssets("t:textasset", new[] {searchPath});

            foreach (var assetGuids in assets)
            {
                string realPath = AssetDatabase.GUIDToAssetPath(assetGuids);
                string fileName = Path.GetFileNameWithoutExtension(realPath);

                if (!fileName.StartsWith(animationBase + "_"))
                {
                    continue;
                }
                
                // Try and find the animation first
                var existingPath = PathHelper.GetSavePath(_zoneShortname, type) + "Animations/" + fileName + ".anim";
                var animClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(existingPath);
                if (animClip != null)
                {
                    animation.AddClip(animClip, animClip.name);
                    continue;
                }
                    
                string text;
                if (!ImportHelper.LoadTextAsset(realPath, out text))
                {
                    continue;
                }

                var ta = new TextAsset(text);
                var newClip = AnimationImporter.ImportAnimation(ta, animationBase, fileName.Split('_')[1], true);
                animation.AddClip(newClip, newClip.name);

                string savePath = PathHelper.GetSavePath(_zoneShortname, type) +
                                  "Animations/";
                string folderPath = PathHelper.GetSystemPathFromUnity(savePath);
                Directory.CreateDirectory(folderPath);

                AssetDatabase.CreateAsset(newClip, savePath + $"{newClip.name}.anim");
            }
        }

        private static void FindEquipmentTextures(string modelAsset, VariantHandler variantHandler)
        {
            Equipment2dHandler pvh = variantHandler as Equipment2dHandler;

            if (pvh == null)
            {
                return;
            }

            pvh.ParsePlayableMeshes();

            var mainMesh = pvh.GetMainBodyMesh();

            if (mainMesh == null)
            {
                return;
            }

            GetEquipmentVariantsForIndex(0, 0, mainMesh, pvh);
            GetEquipmentVariantsForIndex(1, 1, mainMesh, pvh);
            GetEquipmentVariantsForIndex(2, 2, mainMesh, pvh);
            GetEquipmentVariantsForIndex(3, 3, mainMesh, pvh);
            GetEquipmentVariantsForIndex(4, 4, mainMesh, pvh);
            GetEquipmentVariantsForIndex(17, 17, mainMesh, pvh);
            GetEquipmentVariantsForIndex(18, 18, mainMesh, pvh);
            GetEquipmentVariantsForIndex(19, 19, mainMesh, pvh);
            GetEquipmentVariantsForIndex(20, 20, mainMesh, pvh);
            GetEquipmentVariantsForIndex(21, 21, mainMesh, pvh);
            GetEquipmentVariantsForIndex(22, 22, mainMesh, pvh);
            GetEquipmentVariantsForIndex(23, 23, mainMesh, pvh);

            var robeMesh = pvh.GetRobeMesh();

            if (robeMesh == null)
            {
                return;
            }

            // First skin has all materials (hands + feet)
            GetEquipmentVariantsForIndex(10, 4, robeMesh, pvh);
            GetEquipmentVariantsForIndex(11, 5, robeMesh, pvh, "clk");
            GetEquipmentVariantsForIndex(12, 6, robeMesh, pvh, "clk");
            GetEquipmentVariantsForIndex(13, 7, robeMesh, pvh, "clk");
            GetEquipmentVariantsForIndex(14, 8, robeMesh, pvh, "clk");
            GetEquipmentVariantsForIndex(15, 9, robeMesh, pvh, "clk");
            GetEquipmentVariantsForIndex(16, 10, robeMesh, pvh, "clk");

            if (modelAsset == "erm" || modelAsset == "erf")
            {
                var headMesh = pvh.GetHeadMesh(0);
                GetEquipmentVariantsForIndex(0, 0, headMesh, pvh);
                GetEquipmentVariantsForIndex(10, 4, headMesh, pvh, "clk");
                GetEquipmentVariantsForIndex(11, 5, headMesh, pvh, "clk");
                GetEquipmentVariantsForIndex(12, 6, headMesh, pvh, "clk");
                GetEquipmentVariantsForIndex(13, 7, headMesh, pvh, "clk");
                GetEquipmentVariantsForIndex(14, 8, headMesh, pvh, "clk");
                GetEquipmentVariantsForIndex(15, 9, headMesh, pvh, "clk");
                GetEquipmentVariantsForIndex(16, 10, headMesh, pvh, "clk");
            }
        }

        private static void GetEquipmentVariantsForIndex(int armorIndex, int textureIndex, SkinnedMeshRenderer mesh,
            Equipment2dHandler handler, string requiredString = "")
        {
            var materials = mesh.sharedMaterials;
            Texture[] textures = new Texture[materials.Length];

            for (int i = 0; i < materials.Length; i++)
            {
                textures[i] = TextureHelper.FindEquipmentVariant(materials[i].GetTexture("_BaseMap"), textureIndex,
                    requiredString);
            }

            if (textures.All(x => x == null))
            {
                return;
            }

            handler.SetEquipmentTextures(mesh, armorIndex, textures);
        }

        private static void FindAdditionalFaces(GameObject lastPrimaryMesh,
            VariantHandler nonPlayableVariantHandler)
        {
            if (lastPrimaryMesh == null)
            {
                return;
            }

            SkinnedMeshRenderer mr = lastPrimaryMesh.GetComponent<SkinnedMeshRenderer>();

            if (mr == null)
            {
                return;
            }

            List<Texture[]> faces = new List<Texture[]>();

            var sharedMaterials = mr.sharedMaterials;

            Texture[] firstMaterials = new Texture[sharedMaterials.Length];

            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                firstMaterials[i] = sharedMaterials[i].GetTexture("_BaseMap");
            }

            faces.Add(firstMaterials);

            int textureCount = firstMaterials.Length;
            int index = 1;

            while (true)
            {
                Texture[] newFace = new Texture[textureCount];
                for (int i = 0; i < firstMaterials.Length; i++)
                {
                    Texture variant = TextureHelper.FindFaceVariant(firstMaterials[i], index);

                    if (variant != null)
                    {
                        newFace[i] = variant;
                    }
                }

                bool foundNewTextures = newFace.Any(x => x != null);

                if (!foundNewTextures)
                {
                    break;
                }

                faces.Add(newFace);
                index++;
            }

            (nonPlayableVariantHandler as Equipment2dHandler)?.SetAdditionalFaces(faces);
        }

        private static void LoadCharacterSounds(GameObject skeleton, List<ModelSound> soundData)
        {
            // Sound script is added regardless
            var soundScript = skeleton.AddComponent<CharacterSounds>();
            string modelName = skeleton.name;
            int? raceId = RaceHelper.GetRaceIdFromModelName(skeleton.name);

            if (!raceId.HasValue)
            {
                return;
            }

            List<ModelSound> validSounds = GetAllSoundsForRace(raceId.Value, soundData);

            if (validSounds.Count == 0)
            {
                Debug.LogError("No race sound found for: " + modelName);
                return;
            }

            // Handle gender variants
            if (validSounds.Count > 1)
            {
                Gender? gender = RaceHelper.GetRaceGenderFromModelName(modelName.ToUpper());

                if (!gender.HasValue)
                {
                    Debug.LogError("No gender found for model: " + modelName);
                    return;
                }

                List<ModelSound> genderSounds = new List<ModelSound>();

                foreach (var soundEntry in validSounds)
                {
                    if (soundEntry.GenderId == gender.Value)
                    {
                        genderSounds.Add(soundEntry);
                    }
                }

                validSounds = genderSounds;

                if (validSounds.Count == 0)
                {
                    Debug.LogError($"CharacterImport: No sounds found for race and gender: {modelName} {gender.Value}");
                    return;
                }
            }

            foreach (var validSound in validSounds)
            {
                AddSoundToCharacterSounds(validSound.Attack, CharacterSoundType.Attack, soundScript,
                    validSound.VariantId);
                AddSoundToCharacterSounds(validSound.GetHit1, CharacterSoundType.GetHit, soundScript,
                    validSound.VariantId);
                AddSoundToCharacterSounds(validSound.GetHit2, CharacterSoundType.GetHit, soundScript,
                    validSound.VariantId);
                AddSoundToCharacterSounds(validSound.GetHit3, CharacterSoundType.GetHit, soundScript,
                    validSound.VariantId);
                AddSoundToCharacterSounds(validSound.GetHit4, CharacterSoundType.GetHit, soundScript,
                    validSound.VariantId);
                AddSoundToCharacterSounds(validSound.Death, CharacterSoundType.Death, soundScript,
                    validSound.VariantId);
                AddSoundToCharacterSounds(validSound.Drown, CharacterSoundType.Drown, soundScript,
                    validSound.VariantId);
                AddSoundToCharacterSounds(validSound.Idle1, CharacterSoundType.Idle, soundScript, validSound.VariantId);
                AddSoundToCharacterSounds(validSound.Idle2, CharacterSoundType.Idle, soundScript, validSound.VariantId);
                AddSoundToCharacterSounds(validSound.Loop, CharacterSoundType.Loop, soundScript, validSound.VariantId);
                AddSoundToCharacterSounds(validSound.Walking, CharacterSoundType.Walking, soundScript,
                    validSound.VariantId);
                AddSoundToCharacterSounds(validSound.Running, CharacterSoundType.Running, soundScript,
                    validSound.VariantId);
                AddSoundToCharacterSounds(validSound.Jump, CharacterSoundType.Jump, soundScript, validSound.VariantId);
                AddSoundToCharacterSounds(validSound.SAttack, CharacterSoundType.SAttack, soundScript,
                    validSound.VariantId);
                AddSoundToCharacterSounds(validSound.TAttack, CharacterSoundType.TAttack, soundScript,
                    validSound.VariantId);

                // Sitting test
                UniversalAnimationController animations = skeleton.GetComponent<UniversalAnimationController>();
                if (animations.HasAnimation("p02"))
                {
                    string sittingSound = modelName.ToLower() == "ske" ? "Skel_Std.wav" : "StepCrch.WAV";
                    AddSoundToCharacterSounds(sittingSound, CharacterSoundType.Sit, soundScript, validSound.VariantId);
                }

                // Crouch walk
                if (animations.HasAnimation("l06"))
                {
                    AddSoundToCharacterSounds("StepCrch.WAV", CharacterSoundType.Crouch, soundScript,
                        validSound.VariantId);
                }

                // Treading swim
                if (animations.HasAnimation("l09"))
                {
                    AddSoundToCharacterSounds("WatTrd_1.WAV", CharacterSoundType.Treading, soundScript,
                        validSound.VariantId);
                }

                // Moving swim
                if (animations.HasAnimation("p06"))
                {
                    AddSoundToCharacterSounds("WatTrd_2.WAV", CharacterSoundType.Swim, soundScript,
                        validSound.VariantId);
                }

                // Kneel
                if (animations.HasAnimation("p05"))
                {
                    AddSoundToCharacterSounds("StepCrch.WAV", CharacterSoundType.Kneel, soundScript,
                        validSound.VariantId);
                }

                // Kick
                if (animations.HasAnimation("c01"))
                {
                    AddSoundToCharacterSounds("Kick1.WAV", CharacterSoundType.Kick, soundScript, validSound.VariantId);
                }

                // Pierce
                if (animations.HasAnimation("c02"))
                {
                    AddSoundToCharacterSounds("Stab.WAV", CharacterSoundType.Pierce, soundScript, validSound.VariantId);
                }

                // 2H slash
                if (animations.HasAnimation("c03"))
                {
                    AddSoundToCharacterSounds("SwingBig.WAV", CharacterSoundType.TwoHandSlash, soundScript,
                        validSound.VariantId);
                }

                // 2H blunt
                if (animations.HasAnimation("c04"))
                {
                    AddSoundToCharacterSounds("Impale.WAV", CharacterSoundType.TwoHandBlunt, soundScript,
                        validSound.VariantId);
                }

                // Bash?
                if (animations.HasAnimation("c07"))
                {
                    AddSoundToCharacterSounds("BashShld.WAV", CharacterSoundType.Bash, soundScript,
                        validSound.VariantId);
                }

                // Archery
                if (animations.HasAnimation("c09"))
                {
                    AddSoundToCharacterSounds("BowDraw.WAV", CharacterSoundType.Archery, soundScript,
                        validSound.VariantId);
                }

                // Flying kick
                if (animations.HasAnimation("t07"))
                {
                    AddSoundToCharacterSounds("RndKick.WAV", CharacterSoundType.FlyingKick, soundScript,
                        validSound.VariantId);
                }

                // Rapid punch
                if (animations.HasAnimation("t08"))
                {
                    AddSoundToCharacterSounds("Punch1.WAV", CharacterSoundType.RapidPunch, soundScript,
                        validSound.VariantId);
                }

                // Large punch
                if (animations.HasAnimation("t09"))
                {
                    AddSoundToCharacterSounds("Punch1.WAV", CharacterSoundType.LargePunch, soundScript,
                        validSound.VariantId);
                }
            }
        }

        // Move this to the parser
        private static List<ModelSound> GetAllSoundsForRace(int raceId,
            List<ModelSound> soundData)
        {
            List<ModelSound> raceSounds = new List<ModelSound>();

            foreach (var entry in soundData)
            {
                if (entry.RaceId == raceId)
                {
                    raceSounds.Add(entry);
                }
            }

            return raceSounds;
        }

        private static void AddSoundToCharacterSounds(string soundName, CharacterSoundType characterSoundType,
            CharacterSounds soundScript, int variant)
        {
            if (string.IsNullOrEmpty(soundName))
            {
                return;
            }

            if (soundName.ToLower().Contains("null"))
            {
                return;
            }

            string realSoundId = soundName.ToLower().Substring(0, soundName.Length - 3) + "ogg";

            AudioClip soundClip =
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Content/AssetsToBundle/Sound/" + realSoundId);

            if (soundClip != null)
            {
                soundScript.AddSoundClip(characterSoundType, soundClip, variant);
            }
        }

        private static void AddModelAnimations(Animation animation, string modelAsset, string modelLink)
        {
            // Loop through the first time to find animations that match the model name
            foreach (AnimationClip clips in _animations)
            {
                if (!clips.name.StartsWith(modelAsset))
                {
                    continue;
                }

                animation.AddClip(clips, clips.name);
            }

            if (modelLink != string.Empty)
            {
                foreach (AnimationClip clips in _animations)
                {
                    if (!clips.name.StartsWith(modelLink))
                    {
                        continue;
                    }

                    // Do not add the same animation prefix if it already exists
                    if (DoesAnimationContainSameBaseAnimation(animation, clips.name))
                    {
                        continue;
                    }

                    animation.AddClip(clips, clips.name);
                }
            }
        }

        private static bool DoesAnimationContainSameBaseAnimation(Animation animation, string clipName)
        {
            foreach (AnimationState anim in animation)
            {
                if (anim.clip.name.Split('_')[1] == clipName.Split('_')[1])
                {
                    return true;
                }
            }

            return false;
        }

        private static void CreateModelAnimationLink()
        {
            _animationModelSources = new Dictionary<string, string>();
            var pathToModelInfo = PathHelper.GetClientDataPath() + "/animationsources.txt";
            var textLines = File.ReadAllText(pathToModelInfo);
            
            if (textLines.Length == 0)
            {
                Debug.LogError($"CharacterImporter: Could not find animation sources at path: {pathToModelInfo}");
                return;
            }

            var parsedLines = TextParser.ParseTextByDelimitedLines(textLines, ',');

            foreach (var line in parsedLines)
            {
                if (line.Count != 2)
                {
                    continue;
                }

                _animationModelSources[line[0].ToLower()] = line[1].ToLower();
            }
        }

        private static string GetAnimationModelLink(string modelName)
        {
            string link = modelName;
            if (_animationModelSources.ContainsKey(modelName) &&
                !string.IsNullOrEmpty(_animationModelSources[modelName]))
            {
                return _animationModelSources[modelName];
            }

            return link;
        }
    }
}