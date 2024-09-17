using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.EQ.TextParser;
using Lantern.EQ.Animation;
using Lantern.EQ.AssetBundles;
using Lantern.EQ.Equipment;
using Lantern.EQ.Helpers;
using Lantern.EQ.Viewers;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
#endif

namespace Lantern.Legacy.CharacterViewer
{
    public class CharacterViewerRoot : MonoBehaviour
    {
        [SerializeField]
        private GameObject _uiCanvas;

        [SerializeField]
        private TextMeshProUGUI _modelNameText;

        [SerializeField]
        private TextMeshProUGUI _raceIdText;

        [SerializeField]
        private TextMeshProUGUI _modelGenderText;

        [SerializeField]
        private TextMeshProUGUI _modelVariantText;

        [SerializeField]
        private TextMeshProUGUI _cameraZoomText;

        [SerializeField]
        private TextMeshProUGUI _cameraHeightText;

        [SerializeField]
        private TMP_InputField _raceInputField;

        private List<CharacterModelDefinition> _modelData = new List<CharacterModelDefinition>();

        private Equipment2dHandler _e2dHandler;

        private int _currentModelId = 0;
        private Gender _currentModelGender = Gender.Male;
        private CharacterModelDefinition _currentModelData;

        private int _currentSkin;
        private int _skinCountMax;
        private int _currentFace;
        private int _faceCountMax;
        private int _currentHead;
        private int _headCountMax;
        private int _currentArmor;
        private int[] _armorSkinIds = { 0, 1, 2, 3, 4, 17, 18, 19, 20, 21, 22, 23 };

        private bool _isUiHidden = false;

        private enum Gender
        {
            Male = 0,
            Female = 1,
            Neutral = 2,
        }

        [SerializeField]
        private OrbitCamera _camera;

        private AssetBundleLoader _assetBundleLoader;
        private GameObject _currentModel;

        [SerializeField]
        private GameObject _gotoWindow;

        [SerializeField]
        private List<Material> _backgrounds;

        private int _currentBackgroundIndex;

        [SerializeField]
        private CharacterViewerTracks _tracks;

        private int _previousTouchCount = 0;

        [SerializeField] private Button _gotoButton;


        private class CharacterModelDefinition
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public string Male { get; set; }
            public string Female { get; set; }
            public string Neutral { get; set; }
            public float CameraDistance { get; set; }
            public float YOffset { get; set; }

            public List<Vector2Int> SkinGroupsMale { get; set; }
            public List<Vector2Int> SkinGroupsFemale { get; set; }
            public List<Vector2Int> SkinGroupsNeutral { get; set; }

            public string DefaultAnimation { get; set; }

        }

        private int _raceIdInput;

        private void Start()
        {
            RenderSettings.skybox = _backgrounds[0];

            _assetBundleLoader = new AssetBundleLoader();
            LoadModelList();
            SpawnNewModel();
            // ServiceFactory.Get<LoadingScreenService>().SetLoadingScreenVisibility(false);
        }

        private void LoadModelList()
        {
            var text = Resources.Load<TextAsset>("RaceData-CharacterViewer");

            if (text == null)
            {
                return;
            }

            var lines = TextParser.ParseTextByDelimitedLines(text.text, ',');

            for (int i = 0; i < lines.Count; ++i)
            {
                if (i == 0)
                {
                    continue;
                }

                List<Vector2Int> skinsMale = new List<Vector2Int>();
                ParseSkins(skinsMale, lines[i][8]);

                List<Vector2Int> skinsFemale = new List<Vector2Int>();
                ParseSkins(skinsFemale, lines[i][9]);

                List<Vector2Int> skinsNeutral = new List<Vector2Int>();
                ParseSkins(skinsNeutral, lines[i][10]);

                _modelData.Add(new CharacterModelDefinition
                {
                    Id = Convert.ToInt32(lines[i][0]),
                    Name = lines[i][1],
                    Code = lines[i][2],
                    Male = lines[i][3],
                    Female = lines[i][4],
                    Neutral = lines[i][5],
                    CameraDistance = lines[i][6] == string.Empty ? 10.0f : Convert.ToSingle(lines[i][6]),
                    YOffset = lines[i][7] == string.Empty ? 0.0f : Convert.ToSingle(lines[i][7]),
                    SkinGroupsMale = skinsMale,
                    SkinGroupsFemale = skinsFemale,
                    SkinGroupsNeutral = skinsNeutral,
                    DefaultAnimation = lines[i][11]
                });
            }
        }

        private void ParseSkins(List<Vector2Int> skins, string skinString)
        {
            var skinGroups = skinString.Split('_');

            if (skinString != string.Empty)
            {
                foreach (var skin in skinGroups)
                {
                    var skinList = skin.Split(';');

                    if (skinList.Length != 2)
                    {
                        Debug.LogError("Error parsing skin list");
                        continue;
                    }

                    skins.Add(new Vector2Int {x = Convert.ToInt32(skinList[0]), y = Convert.ToInt32(skinList[1])});
                }
            }
        }

        public void ChangeVariantButton()
        {
            if (IsPlayableRace())
            {
                ChangeModelFace();
            }
            else
            {
                ChangeModelSkin();
            }
        }

        private void Update()
        {
            CheckForInput();
            UpdateCameraUiInfo();
        }

        private void CheckForInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
#if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
#else
                Application.Quit();
#endif
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ChangeCurrentModel(true);
                return;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ChangeCurrentModel(false);
                return;
            }

            int currentTouch = Input.touchCount;
            if (Input.GetKeyDown(KeyCode.V))
            {
                ChangeModelSkin();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                ChangeModelFace();
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                ChangeModelArmor();
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                ChangeModelHead();
            }

            if (Input.GetKeyDown(KeyCode.BackQuote) || (_previousTouchCount != 3 && currentTouch == 3))
            {
                bool isActive = !_gotoWindow.activeSelf;
                _gotoWindow.SetActive(isActive);

                _camera._inputActive = !isActive;
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (_gotoWindow.activeSelf)
                {
                    GotoRaceId();
                }
            }

            if (Input.GetKeyDown(KeyCode.F10))
            {
                ToggleUiHidden();
            }

            _previousTouchCount = currentTouch;
        }

        private void ToggleUiHidden()
        {
            _isUiHidden = !_isUiHidden;

            _uiCanvas.SetActive(!_isUiHidden);
        }

        public void ChangeBackgroundClearColor(bool increment)
        {
            if (increment)
            {
                _currentBackgroundIndex++;
            }
            else
            {
                _currentBackgroundIndex--;
            }

            if (_currentBackgroundIndex < 0)
            {
                _currentBackgroundIndex = _backgrounds.Count - 1;
            }
            else
            {
                _currentBackgroundIndex %= _backgrounds.Count;
            }

            RenderSettings.skybox = _backgrounds[_currentBackgroundIndex];
        }

        private void ChangeModelSkin()
        {
            if (_currentModel == null)
            {
                return;
            }

            List<Vector2Int> skinGroups = GetSkinGroupForCurrentGender();

            if (skinGroups.Count < 1)
            {
                return;
            }

            _currentSkin++;

            if (_currentSkin >= skinGroups.Count)
            {
                _currentSkin = 0;
            }

            var variantSwapper = _currentModel.GetComponent<NonPlayableVariantHandler>();

            if (variantSwapper)
            {
                variantSwapper.SetCurrentActiveVariant(skinGroups[_currentSkin].x, skinGroups[_currentSkin].y);
            }

            UpdateVariantText();
        }

        private void ChangeModelFace()
        {
            if (_currentModel == null || _e2dHandler == null)
            {
                return;
            }

            _currentFace++;

            if (_currentFace >= _faceCountMax)
            {
                _currentFace = 0;
            }

            _e2dHandler.SetFaceId(_currentFace);
            UpdateVariantText();
        }

        private void ChangeModelHead()
        {
            if (_currentModel == null || _e2dHandler == null)
            {
                return;
            }

            _currentHead++;
            if (_currentHead >= _headCountMax)
            {
                _currentHead = 0;
            }

            _e2dHandler.SetArmorSetActive(GetArmorSkin(), _currentHead);
        }

        private void ChangeModelArmor()
        {
            if (_currentModel == null || _e2dHandler == null)
            {
                return;
            }

            _currentArmor++;

            if (IsMonkArmor() && !IsMonkRace())
            {
                _currentArmor++;
            }

            if (_currentArmor >= _armorSkinIds.Count())
            {
                _currentArmor = 0;
            }

            _e2dHandler.SetArmorSetActive(GetArmorSkin(), _currentHead);
        }

        private List<Vector2Int> GetSkinGroupForCurrentGender()
        {
            switch (_currentModelGender)
            {
                case Gender.Male:
                    return _currentModelData.SkinGroupsMale;
                case Gender.Female:
                    return _currentModelData.SkinGroupsFemale;
                case Gender.Neutral:
                    return _currentModelData.SkinGroupsNeutral;
            }

            return null;
        }

        public void GotoRaceId()
        {
            if (_raceIdInput < 0 || _raceIdInput >= _modelData.Count)
            {
                _raceInputField.Select();
                _raceInputField.ActivateInputField();
                return;
            }

            int targetRaceId = -1;
            for (var i = 0; i < _modelData.Count; i++)
            {
                var raceEntry = _modelData[i];
                if (raceEntry.Id == _raceIdInput)
                {
                    targetRaceId = i;
                }
            }

            if (targetRaceId == -1)
            {
                _raceInputField.Select();
                _raceInputField.ActivateInputField();
                return;
            }

            Gender gender;
            var targetModelData = _modelData[targetRaceId];
            if (!string.IsNullOrEmpty(targetModelData.Male))
            {
                gender = Gender.Male;
            }
            else if (!string.IsNullOrEmpty(targetModelData.Female))
            {
                gender = Gender.Female;
            }
            else if (!string.IsNullOrEmpty(targetModelData.Neutral))
            {
                gender = Gender.Neutral;
            }
            else
            {
                return;
            }

            _currentModelId = targetRaceId;
            _currentModelGender = gender;
            SpawnNewModel();
            _gotoWindow.SetActive(false);
            _raceInputField.text = string.Empty;
            _camera._inputActive = true;
        }

        public void CloseGotoWindow()
        {
            if (_gotoWindow.activeSelf)
            {
                _gotoWindow.SetActive(false);
                _camera._inputActive = true;
            }
        }

        public void NewRaceIdEntered(string newId)
        {
            if (newId == string.Empty)
            {
                _raceIdInput = -1;
                _gotoButton.interactable = false;
                return;
            }

            _raceIdInput = Convert.ToInt32(newId);
            _gotoButton.interactable = IsValidRaceId(_raceIdInput);
        }

        private bool IsValidRaceId(int raceIdInput)
        {
            foreach (var entry in _modelData)
            {
                if (entry.Id == raceIdInput)
                {
                    if (entry.Male != string.Empty || entry.Female != string.Empty || entry.Neutral != string.Empty)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsPlayableRace()
        {
            return RaceHelper.PlayableRaceIds.Contains(_currentModelData.Id);
        }

        private bool IsMonkArmor()
        {
            var armorSkin = GetArmorSkin();
            return armorSkin == 4 || armorSkin == 23;
        }

        private bool IsMonkRace()
        {
            return _currentModelData != null && (_currentModelData.Id == 1 || _currentModelData.Id == 128);
        }

        private int GetArmorSkin()
        {
            return _currentArmor < _armorSkinIds.Count() ? _armorSkinIds[_currentArmor] : 0;
        }

        private void UpdateCameraUiInfo()
        {
            _cameraZoomText.text = _camera.CurrentDistance.ToString("0.0");
            _cameraHeightText.text = _camera._targetPosition.y.ToString("0.0");
        }

        public void ChangeCurrentModel(bool up)
        {
            int newModelId = _currentModelId;
            Gender newGenderId = _currentModelGender;
            string newRaceString = string.Empty;

            do
            {
                switch (newGenderId)
                {
                    case Gender.Male:
                        if (up)
                        {
                            newGenderId = Gender.Female;
                            newRaceString = _modelData[newModelId].Female;
                        }
                        else
                        {
                            newGenderId = Gender.Neutral;
                            newModelId -= 1;

                            if (newModelId < 0)
                            {
                                newModelId = _modelData.Count - 1;
                            }

                            newRaceString = _modelData[newModelId].Neutral;
                        }

                        break;
                    case Gender.Female:
                        newGenderId = up ? Gender.Neutral : Gender.Male;
                        newRaceString = up ? _modelData[newModelId].Neutral : _modelData[newModelId].Male;
                        break;
                    case Gender.Neutral:
                        if (up)
                        {
                            newGenderId = Gender.Male;
                            newModelId += 1;

                            if (newModelId >= _modelData.Count)
                            {
                                newModelId = 0;
                            }
                            newRaceString = _modelData[newModelId].Male;
                        }
                        else
                        {
                            newGenderId = Gender.Female;
                            newRaceString = _modelData[newModelId].Female;
                        }

                        break;
                }
            } while (newRaceString == string.Empty);

            _currentModelId = newModelId;
            _currentModelGender = newGenderId;
            _currentSkin = 0;
            _currentFace = 0;
            _currentHead = 0;
            _currentArmor = 0;
            _faceCountMax = 0;
            _headCountMax = 0;

            SpawnNewModel();
        }

        public void UpdateVariantText()
        {
            if (IsPlayableRace())
            {

                _modelVariantText.text = "Face " + (_currentFace + 1) + "/" + _faceCountMax;
            }
            else
            {
                _modelVariantText.text = "Variant " + (_currentSkin + 1) + "/" + _skinCountMax;
            }
        }

        private void SpawnNewModel()
        {
            if (_currentModel)
            {
                Destroy(_currentModel.gameObject);
                _currentModel = null;
            }

            _currentModelData = _modelData[_currentModelId];
            string newShortName = string.Empty;
            string newFullName = _currentModelData.Name;

            switch (_currentModelGender)
            {
                case Gender.Male:
                    newShortName = _currentModelData.Male;
                    _skinCountMax = _currentModelData.SkinGroupsMale.Count;
                    break;
                case Gender.Female:
                    newShortName = _currentModelData.Female;
                    _skinCountMax = _currentModelData.SkinGroupsFemale.Count;
                    break;
                case Gender.Neutral:
                    newShortName = _currentModelData.Neutral;
                    _skinCountMax = _currentModelData.SkinGroupsNeutral.Count;
                    break;
            }

            int realId = _currentModelData.Id;
            float cameraDistance = _currentModelData.CameraDistance;
            float yOffset = _currentModelData.YOffset;

            _modelNameText.text = newFullName;
            _raceIdText.text = realId + $" - {newShortName}" + " (" + _currentModelGender.ToString().Substring(0, 1) +
                               ")";

            var model = _assetBundleLoader.LoadAsset<GameObject>
                (LanternAssetBundleId.Characters, newShortName + ".prefab");

            if (model == null)
            {
                return;
            }

            _currentModel = Instantiate(model);

            _e2dHandler = _currentModel.GetComponent<Equipment2dHandler>();
            if (_e2dHandler != null)
            {
                _faceCountMax = _e2dHandler.GetFaceCount();
                _headCountMax = _e2dHandler.GetHeadCount();
            }

            UpdateVariantText();

            if (newShortName == "EYE")
            {
                _currentModel.transform.rotation = Quaternion.Euler(0f, -90, 0f);
            }

            var uac = _currentModel.GetComponent<CharacterAnimationController>();

            string defaultAnimation = string.Empty;

            if (uac != null)
            {
                defaultAnimation = _currentModelData.DefaultAnimation;

                if (string.IsNullOrEmpty(defaultAnimation))
                {
                    defaultAnimation = "p01";
                }

                var defaultAnimationType = AnimationHelper.GetAnimationType(defaultAnimation);
                if (defaultAnimationType != null)
                {
                    uac.SetNewConstantState((AnimationType)defaultAnimationType, 1);
                }
            }

            _camera.SetNewTarget(cameraDistance, yOffset);

            var skinGroup = GetGenderVariants();

            if (skinGroup == null
                || skinGroup.Count == 0)
            {
                Debug.LogError("Missing variant definitions for: " + newShortName);
                return;
            }

            var defaultSkin = skinGroup[0];

            NonPlayableVariantHandler handler = _currentModel.GetComponent<NonPlayableVariantHandler>();

            if (handler != null)
            {
                handler.SetCurrentActiveVariant(defaultSkin.x, defaultSkin.y);
            }

            PopulateAnimationList();

            if (defaultAnimation != string.Empty)
            {
                _tracks.HighlightItem(defaultAnimation);
            }
        }

        private List<Vector2Int> GetGenderVariants()
        {
            switch (_currentModelGender)
            {
                case Gender.Male:
                    return _currentModelData.SkinGroupsMale;
                case Gender.Female:
                    return _currentModelData.SkinGroupsFemale;
                case Gender.Neutral:
                    return _currentModelData.SkinGroupsNeutral;
            }

            return null;
        }

        private void PopulateAnimationList()
        {
            if (_currentModel == null)
            {
                _tracks.ClearList();
                return;
            }

            Dictionary<string, string> clips = new Dictionary<string, string>();
            CharacterAnimationController controller = _currentModel.GetComponent<CharacterAnimationController>();

            Animation animation = _currentModel.GetComponent<Animation>();

            if (animation == null)
            {
                _tracks.ClearList();
                return;
            }

            List<string> clipNames = new List<string>();

            foreach (AnimationState clip in animation)
            {
                clipNames.Add(clip.name);
            }

            clipNames = clipNames.OrderBy(str => str.Split('_').Last()).ToList();

            foreach (string clipName in clipNames)
            {
                string animationType = clipName.Split('_')[1];
                string animationName = "<b>" + AnimationHelper.GetAnimationName(animationType)  + "</b>"+ $"\n({clipName})";
                if (!clips.ContainsKey(animationType))
                {
                    clips.Add(animationType, animationName);
                }
            }

            _tracks.RefreshList(clips,  (AnimationType animationType) => controller.PlayOneShotAnimation(animationType));
        }
    }
}
