using System.Collections.Generic;
using System.IO;
using System.Linq;
using Infrastructure.EQ.TextParser;
using Lantern.EQ.Animation;
using Lantern.EQ.Characters;
using Lantern.EQ.Data;
using Lantern.EQ.Editor.Helpers;
using Lantern.EQ.Equipment;
using Lantern.EQ.Helpers;
using Lantern.EQ.Lighting;
using Lantern.EQ.Sound;
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

        /// <summary>
        /// Data for the sounds each model uses
        /// </summary>
        private static List<ModelSound> _modelSounds;

        /// <summary>
        /// Loaded animations, faster than finding them on disk
        /// </summary>
        private static Dictionary<string, AnimationClip> _animations;

        /// <summary>
        /// Unity relative paths to animation text files
        /// </summary>
        private static List<string> _animationPaths;

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
            LoadData();
            var startTime = EditorApplication.timeSinceStartup;
            TextureHelper.CopyTextures(_zoneShortname, AssetImportType.Characters);
            ActorStaticImporter.ImportList("characters", AssetImportType.Characters, PostProcess);
            ActorSkeletalImporter.ImportList("characters", AssetImportType.Characters, PostProcessSkeletal);
            ImportHelper.TagAllAssetsForBundles(PathHelper.GetAssetBundleContentPath()+ "Characters", "characters");
            EditorUtility.DisplayDialog("CharacterImport",
                $"Character import finished in {(int)(EditorApplication.timeSinceStartup - startTime)} seconds", "OK");
            Cleanup();
        }

        private static void LoadData()
        {
            _modelSounds = RaceSoundsDataParser.GetModelSounds();
            _animations = new Dictionary<string, AnimationClip>();
            CreateModelAnimationLink();
            _animationPaths = AnimationImporter.LoadAnimationPaths(_zoneShortname, AssetImportType.Characters);
        }

        private static void Cleanup()
        {
            _modelSounds.Clear();
            _animations.Clear();
            _modelSounds.Clear();
            _animationPaths.Clear();
        }

        private static void PostProcess(GameObject obj)
        {
            string modelAsset = obj.name;
            var characterModel = obj.AddComponent<CharacterModel>();

            var lightSetter = obj.AddComponent<AmbientLightSetterDynamic>();
            lightSetter.FindRenderers();

            // Disabled in 0.1.5
            //LoadCharacterSounds(obj, _modelSounds);

            characterModel.SetReferences(null, null, null, lightSetter,
                obj.GetComponent<CharacterSounds>(),
                obj.GetComponent<CharacterAnimationLogic>());
        }

        private static void PostProcessSkeletal(GameObject skeleton)
        {
            string modelAsset = skeleton.name;
            var characterModel = skeleton.AddComponent<CharacterModel>();

            VariantHandler variantHandler = null;

            var cac = skeleton.AddComponent<CharacterAnimationController>();
            var cal = skeleton.AddComponent<CharacterAnimationLogic>();
            cal.InitializeImport();
            var ap = skeleton.AddComponent<SkeletonAttachPoints>();
            ap.FindAttachPoints();
            var e3d = skeleton.AddComponent<Equipment3dHandler>();
            e3d.SetSkeletonAttachPoints(ap);

            // TODO: Is this still needed?
            var animatedObject = skeleton.GetComponent<ObjectAnimation>();
            DestroyImmediate(animatedObject);

            var animation = skeleton.GetComponent<Animation>();

            // Find all meshes for this model
            string path = _zoneShortname == "characters"
                ? PathHelper.GetEqAssetPath() + "Characters"
                : $"{PathHelper.GetEqAssetPath()}/{_zoneShortname}/Characters";
            string[] meshGuidPaths = AssetDatabase.FindAssets(modelAsset,
                new[] { path });

            if (meshGuidPaths == null || meshGuidPaths.Length == 0)
            {
                Debug.LogError("No mesh guids found for model " + modelAsset);
                return;
            }

            // Find animations
            if (animation != null)
            {
                LoadModelAnimations(animation, modelAsset, AssetImportType.Characters);

                if (_animationModelSources.ContainsKey(modelAsset))
                {
                    LoadModelAnimations(animation, _animationModelSources[modelAsset], AssetImportType.Characters);
                }
            }

            // Initialize the animation controller
            cac.InitializeImport();

            variantHandler = skeleton.GetComponent<VariantHandler>();

            if (RaceHelper.IsPlayableRace(modelAsset))
            {
                FindEquipmentTextures(modelAsset, variantHandler);
                FindAdditionalFaces(modelAsset, variantHandler.GetLastPrimaryMesh(), variantHandler);
            }

            var vertexColorDebug = skeleton.AddComponent<AmbientLightSetterDynamic>();
            vertexColorDebug.FindRenderers();

            LoadCharacterSounds(skeleton, _modelSounds);

            characterModel.SetReferences(ap, variantHandler as Equipment2dHandler, e3d, vertexColorDebug,
                skeleton.GetComponent<CharacterSounds>(),
                skeleton.GetComponent<CharacterAnimationLogic>());
        }

        private static void LoadModelAnimations(Animation animation, string animationBase, AssetImportType type)
        {
            string prefix = animationBase + "_";

            foreach (var path in _animationPaths)
            {
                string fileName = Path.GetFileNameWithoutExtension(path);

                if (!fileName.StartsWith(prefix))
                {
                    continue;
                }

                var animClip = AnimationImporter.GetAnimationClip(_zoneShortname, fileName, path, animationBase, type,
                    ref _animations);

                if (animClip == null)
                {
                    Debug.LogError("Unable to import animation: " + fileName);
                }

                animation.AddClip(animClip, animClip.name);

                if (type == AssetImportType.Characters)
                {
                    if (!AnimationHelper.TrySplitAnimationName(fileName, out _, out var at))
                    {
                        continue;
                    }

                    if (!AnimationHelper.IsReverseAnimationNeeded(at))
                    {
                        continue;
                    }

                    animClip = AnimationImporter.GetAnimationClip(_zoneShortname, fileName, path, animationBase, type,
                        ref _animations, true);

                    if (animClip != null)
                    {
                        animation.AddClip(animClip, animClip.name);
                    }
                }
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
                if (materials[i] == null)
                {
                    continue;
                }

                textures[i] = TextureHelper.FindEquipmentVariant(materials[i].GetTexture("_BaseMap"), textureIndex,
                    requiredString);
            }

            if (textures.All(x => x == null))
            {
                return;
            }

            handler.SetEquipmentTextures(mesh, armorIndex, textures);
        }

        private static void FindAdditionalFaces(string modelName, GameObject lastPrimaryMesh,
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
                if (sharedMaterials[i] == null)
                {
                    continue;
                }

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
                    Texture variant = TextureHelper.FindFaceVariant(modelName, firstMaterials[i], index);

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
            string modelName = skeleton.name;
            int? raceId = RaceHelper.GetRaceIdFromModelName(skeleton.name);

            if (!raceId.HasValue)
            {
                return;
            }

            CharacterSoundsBase soundBaseScript = null;

            List<ModelSound> validSounds = GetAllSoundsForRace(raceId.Value, soundData);

            if (validSounds.Count == 0)
            {
                soundBaseScript = skeleton.AddComponent<CharacterSoundsBaseEmpty>();
                return;
            }
            else
            {
                soundBaseScript = skeleton.AddComponent<CharacterSounds>();
            }

            // Handle gender variants
            if (validSounds.Count > 1)
            {
                GenderId? gender = RaceHelper.GetRaceGenderFromModelName(modelName.ToUpper());

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
                AddSoundToCharacterSounds(validSound.Attack, CharacterSoundType.Attack, soundBaseScript,
                    validSound.VariantId);
                AddSoundToCharacterSounds(validSound.GetHit1, CharacterSoundType.GetHit, soundBaseScript,
                    validSound.VariantId);
                AddSoundToCharacterSounds(validSound.GetHit2, CharacterSoundType.GetHit, soundBaseScript,
                    validSound.VariantId);
                AddSoundToCharacterSounds(validSound.GetHit3, CharacterSoundType.GetHit, soundBaseScript,
                    validSound.VariantId);
                AddSoundToCharacterSounds(validSound.GetHit4, CharacterSoundType.GetHit, soundBaseScript,
                    validSound.VariantId);
                AddSoundToCharacterSounds(validSound.Death, CharacterSoundType.Death, soundBaseScript,
                    validSound.VariantId);
                AddSoundToCharacterSounds(validSound.Drown, CharacterSoundType.Drown, soundBaseScript,
                    validSound.VariantId);
                AddSoundToCharacterSounds(validSound.Idle1, CharacterSoundType.Idle, soundBaseScript,
                    validSound.VariantId);
                AddSoundToCharacterSounds(validSound.Idle2, CharacterSoundType.Idle, soundBaseScript,
                    validSound.VariantId);
                AddSoundToCharacterSounds(validSound.Loop, CharacterSoundType.Loop, soundBaseScript,
                    validSound.VariantId);
                AddSoundToCharacterSounds(validSound.Walking, CharacterSoundType.Walking, soundBaseScript,
                    validSound.VariantId);
                AddSoundToCharacterSounds(validSound.Running, CharacterSoundType.Running, soundBaseScript,
                    validSound.VariantId);
                AddSoundToCharacterSounds(validSound.Jump, CharacterSoundType.Jump, soundBaseScript,
                    validSound.VariantId);
                AddSoundToCharacterSounds(validSound.SAttack, CharacterSoundType.SAttack, soundBaseScript,
                    validSound.VariantId);
                AddSoundToCharacterSounds(validSound.TAttack, CharacterSoundType.TAttack, soundBaseScript,
                    validSound.VariantId);

                // Sitting test
                CharacterAnimationController animationsController =
                    skeleton.GetComponent<CharacterAnimationController>();

                if (animationsController == null)
                {
                    return;
                }

                if (animationsController.HasAnimation("p02"))
                {
                    string sittingSound = modelName.ToLower() == "ske" ? "Skel_Std.wav" : "StepCrch.WAV";
                    AddSoundToCharacterSounds(sittingSound, CharacterSoundType.Sit, soundBaseScript,
                        validSound.VariantId);
                }

                // Crouch walk
                if (animationsController.HasAnimation("l06"))
                {
                    AddSoundToCharacterSounds("StepCrch.WAV", CharacterSoundType.Crouch, soundBaseScript,
                        validSound.VariantId);
                }

                // Treading swim
                if (animationsController.HasAnimation("l09"))
                {
                    AddSoundToCharacterSounds("WatTrd_1.WAV", CharacterSoundType.Treading, soundBaseScript,
                        validSound.VariantId);
                }

                // Moving swim
                if (animationsController.HasAnimation("p06"))
                {
                    AddSoundToCharacterSounds("WatTrd_2.WAV", CharacterSoundType.Swim, soundBaseScript,
                        validSound.VariantId);
                }

                // Kneel
                if (animationsController.HasAnimation("p05"))
                {
                    AddSoundToCharacterSounds("StepCrch.WAV", CharacterSoundType.Kneel, soundBaseScript,
                        validSound.VariantId);
                }

                // Kick
                if (animationsController.HasAnimation("c01"))
                {
                    AddSoundToCharacterSounds("Kick1.WAV", CharacterSoundType.Kick, soundBaseScript,
                        validSound.VariantId);
                }

                // Pierce
                if (animationsController.HasAnimation("c02"))
                {
                    AddSoundToCharacterSounds("Stab.WAV", CharacterSoundType.Pierce, soundBaseScript,
                        validSound.VariantId);
                }

                // 2H slash
                if (animationsController.HasAnimation("c03"))
                {
                    AddSoundToCharacterSounds("SwingBig.WAV", CharacterSoundType.TwoHandSlash, soundBaseScript,
                        validSound.VariantId);
                }

                // 2H blunt
                if (animationsController.HasAnimation("c04"))
                {
                    AddSoundToCharacterSounds("Impale.WAV", CharacterSoundType.TwoHandBlunt, soundBaseScript,
                        validSound.VariantId);
                }

                // Bash?
                if (animationsController.HasAnimation("c07"))
                {
                    AddSoundToCharacterSounds("BashShld.WAV", CharacterSoundType.Bash, soundBaseScript,
                        validSound.VariantId);
                }

                // Archery
                if (animationsController.HasAnimation("c09"))
                {
                    AddSoundToCharacterSounds("BowDraw.WAV", CharacterSoundType.Archery, soundBaseScript,
                        validSound.VariantId);
                }

                // Flying kick
                if (animationsController.HasAnimation("t07"))
                {
                    AddSoundToCharacterSounds("RndKick.WAV", CharacterSoundType.FlyingKick, soundBaseScript,
                        validSound.VariantId);
                }

                // Rapid punch
                if (animationsController.HasAnimation("t08"))
                {
                    AddSoundToCharacterSounds("Punch1.WAV", CharacterSoundType.RapidPunch, soundBaseScript,
                        validSound.VariantId);
                }

                // Large punch
                if (animationsController.HasAnimation("t09"))
                {
                    AddSoundToCharacterSounds("Punch1.WAV", CharacterSoundType.LargePunch, soundBaseScript,
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
            CharacterSoundsBase soundBaseScript, int variant)
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
                AssetDatabase.LoadAssetAtPath<AudioClip>(PathHelper.GetAssetBundleContentPath()+ "Sound/" + realSoundId);

            if (soundClip != null)
            {
                soundBaseScript.AddSoundClip(characterSoundType, soundClip, variant);
            }
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
    }
}
