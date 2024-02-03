using System.Collections.Generic;
using System.IO;
using System.Linq;
using Infrastructure.EQ.TextParser;
using Lantern.EQ.Animation;
using Lantern.EQ.Characters;
using Lantern.EQ.Editor.Helpers;
using Lantern.EQ.Equipment;
using Lantern.EQ.Helpers;
using Lantern.EQ.Lighting;
using Lantern.EQ.Sound;
using UnityEditor;
using UnityEngine;

namespace Lantern.EQ.Editor.Importers
{
    public class CharacterImporter : LanternEditorWindow
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

        private bool _importAllCharacters = true;

        private static readonly List<string> Lines1 = new()
        {
            "This process creates character prefabs from intermediate EverQuest data.",
            "Importing all characters can take more than an hour.",
            "You can choose to import all characters or just characters from a specific zone.",
        };

        private static readonly List<string> Lines2A = new()
        {
            "EverQuest character data must be located in:",
            "\fAssets/EQAssets/Characters/",
        };

        private static readonly List<string> Lines2B = new()
        {
            "EverQuest character data (one character folder per zone) must be located in:",
            "\fAssets/EQAssets/",
        };

        private static readonly List<string> Lines3 = new()
        {
            "Character prefabs will be created in:",
            "\fAssets/Content/AssetBundleContent/Characters/"
        };

        private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");

        [MenuItem("EQ/Assets/Import Characters &c", false, 2)]
        public static void ShowImportDialog()
        {
            GetWindow<CharacterImporter>("Import Characters", typeof(EditorWindow));
        }

        /// <summary>
        /// Draws the settings window for the character importer
        /// </summary>
        private void OnGUI()
        {
            DrawInfoBox(Lines1, "d_console.infoicon");
            DrawInfoBox(_importAllCharacters ? Lines2A : Lines2B, "d_Collab.FolderConflict");
            DrawInfoBox(Lines3, "d_Collab.FolderMoved");
            DrawHorizontalLine();
            DrawToggle("Import All Characters", ref _importAllCharacters);

            if (!_importAllCharacters)
            {
                DrawTextField("Zone Name:", ref _zoneShortname);
            }

            if (DrawButton("Import"))
            {
                Import();
            }
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

        private static void PostProcess(GameObject character)
        {
            var characterModel = character.AddComponent<CharacterModel>();
            var lightSetter = character.AddComponent<AmbientLightSetterDynamic>();
            lightSetter.FindRenderers();
            characterModel.SetReferences(null, null, null, lightSetter,
                character.GetComponent<CharacterSoundLogic>(),
                character.GetComponent<CharacterAnimationLogic>());
        }

        private static void PostProcessSkeletal(GameObject character)
        {
            string modelAsset = character.name;
            var characterModel = character.AddComponent<CharacterModel>();

            var cac = character.AddComponent<CharacterAnimationController>();
            var cal = character.AddComponent<CharacterAnimationLogic>();
            cal.InitializeImport();
            var ap = character.AddComponent<SkeletonAttachPoints>();
            ap.FindAttachPoints();
            var e3d = character.AddComponent<Equipment3dHandler>();
            e3d.SetSkeletonAttachPoints(ap);
            character.AddComponent<CharacterSoundLogic>();

            // TODO: Is this still needed?
            var animatedObject = character.GetComponent<ObjectAnimation>();
            DestroyImmediate(animatedObject);

            var animation = character.GetComponent<UnityEngine.Animation>();

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

                if (_animationModelSources.TryGetValue(modelAsset, out var source))
                {
                    LoadModelAnimations(animation, source, AssetImportType.Characters);
                }
            }

            // Initialize the animation controller
            cac.InitializeImport();

            var variantHandler = character.GetComponent<VariantHandler>();

            if (RaceHelper.IsPlayableRaceModel(modelAsset))
            {
                FindEquipmentTextures(modelAsset, variantHandler);
                FindAdditionalFaces(modelAsset, variantHandler.GetLastPrimaryMesh(), variantHandler);
            }

            var vertexColorDebug = character.AddComponent<AmbientLightSetterDynamic>();
            vertexColorDebug.FindRenderers();

            characterModel.SetReferences(ap, variantHandler as Equipment2dHandler, e3d, vertexColorDebug,
                character.GetComponent<CharacterSoundLogic>(),
                character.GetComponent<CharacterAnimationLogic>());
        }

        private static void LoadModelAnimations(UnityEngine.Animation animation, string animationBase, AssetImportType type)
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

            GetEquipmentVariantsForIndex(modelAsset, 0, 0, mainMesh, pvh);
            GetEquipmentVariantsForIndex(modelAsset, 1, 1, mainMesh, pvh);
            GetEquipmentVariantsForIndex(modelAsset, 2, 2, mainMesh, pvh);
            GetEquipmentVariantsForIndex(modelAsset, 3, 3, mainMesh, pvh);
            GetEquipmentVariantsForIndex(modelAsset, 4, 4, mainMesh, pvh);
            GetEquipmentVariantsForIndex(modelAsset, 17, 17, mainMesh, pvh);
            GetEquipmentVariantsForIndex(modelAsset, 18, 18, mainMesh, pvh);
            GetEquipmentVariantsForIndex(modelAsset, 19, 19, mainMesh, pvh);
            GetEquipmentVariantsForIndex(modelAsset, 20, 20, mainMesh, pvh);
            GetEquipmentVariantsForIndex(modelAsset, 21, 21, mainMesh, pvh);
            GetEquipmentVariantsForIndex(modelAsset, 22, 22, mainMesh, pvh);
            GetEquipmentVariantsForIndex(modelAsset, 23, 23, mainMesh, pvh);

            var robeMesh = pvh.GetRobeMesh();

            if (robeMesh == null)
            {
                return;
            }

            // First skin has all materials (hands + feet)
            GetEquipmentVariantsForIndex(modelAsset, 10, 4, robeMesh, pvh);
            GetEquipmentVariantsForIndex(modelAsset, 11, 5, robeMesh, pvh, "clk");
            GetEquipmentVariantsForIndex(modelAsset, 12, 6, robeMesh, pvh, "clk");
            GetEquipmentVariantsForIndex(modelAsset, 13, 7, robeMesh, pvh, "clk");
            GetEquipmentVariantsForIndex(modelAsset, 14, 8, robeMesh, pvh, "clk");
            GetEquipmentVariantsForIndex(modelAsset, 15, 9, robeMesh, pvh, "clk");
            GetEquipmentVariantsForIndex(modelAsset, 16, 10, robeMesh, pvh, "clk");

            // Erudite edge case
            if (modelAsset == "erm" || modelAsset == "erf")
            {
                var headMesh = pvh.GetHeadMesh(0);
                GetEquipmentVariantsForIndex(modelAsset, 0, 0, headMesh, pvh);
                GetEquipmentVariantsForIndex(modelAsset, 10, 4, headMesh, pvh, "clk");
                GetEquipmentVariantsForIndex(modelAsset, 11, 5, headMesh, pvh, "clk");
                GetEquipmentVariantsForIndex(modelAsset, 12, 6, headMesh, pvh, "clk");
                GetEquipmentVariantsForIndex(modelAsset, 13, 7, headMesh, pvh, "clk");
                GetEquipmentVariantsForIndex(modelAsset, 14, 8, headMesh, pvh, "clk");
                GetEquipmentVariantsForIndex(modelAsset, 15, 9, headMesh, pvh, "clk");
                GetEquipmentVariantsForIndex(modelAsset, 16, 10, headMesh, pvh, "clk");
            }
        }

        private static void GetEquipmentVariantsForIndex(string modelAsset, int armorIndex, int textureIndex, SkinnedMeshRenderer mesh,
            Equipment2dHandler handler, string requiredString = "")
        {
            var materials = mesh.sharedMaterials;
            Texture[] textures = new Texture[materials.Length];

            for (int i = 0; i < materials.Length; i++)
            {
                var material = materials[i];
                if (material == null)
                {
                    continue;
                }

                textures[i] = TextureHelper.FindEquipmentVariant(modelAsset, material.GetTexture(BaseMap), textureIndex,
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

                firstMaterials[i] = sharedMaterials[i].GetTexture(BaseMap);
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
