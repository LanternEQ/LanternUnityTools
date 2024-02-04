using System.Collections.Generic;
using Lantern.EQ.AssetBundles;
using Lantern.EQ.Editor.Helpers;
using Lantern.EQ.Viewers;
using UnityEditor;
using UnityEngine;

namespace Lantern.EQ.Editor.Importers
{
    public class ValidateBundles : LanternEditorWindow
    {
        private const string FoundImage = "sv_icon_dot3_pix16_gizmo";
        private const string NotFoundImage = "sv_icon_dot6_pix16_gizmo";
        private const string NotFoundOptionalImage = "sv_icon_dot4_pix16_gizmo";

        private Vector2 _scrollPosition = Vector2.zero;

        private static readonly List<string> Lines1 = new()
        {
            "Validates that LanternEQ version compliant bundles exist on disk. If you do not see a bundle you have imported, rebuild bundles.",
            "This process only validates that the bundle exists. It does not verify the content of the bundle.",
        };

        [MenuItem("EQ/Assets/Validate Assets", false, 100)]
        private static void Init()
        {
            GetWindow<ValidateBundles>("Validate Bundles", typeof(EditorWindow));
        }

        private void OnGUI()
        {
            DrawInfoBox(Lines1, "d_console.infoicon");

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            DisplayGlobalBundleStatus(LanternAssetBundleId.Characters);
            DisplayGlobalBundleStatus(LanternAssetBundleId.Equipment);
            DisplayGlobalBundleStatus(LanternAssetBundleId.Sprites);
            DisplayGlobalBundleStatus(LanternAssetBundleId.Sky);
            DisplayGlobalBundleStatus(LanternAssetBundleId.Sound);
            DisplayGlobalBundleStatus(LanternAssetBundleId.Music_Audio);
            DisplayGlobalBundleStatus(LanternAssetBundleId.Music_Midi);
            DisplayGlobalBundleStatus(LanternAssetBundleId.Startup);
            DisplayGlobalBundleStatus(LanternAssetBundleId.ClientData);
            DisplayZoneBundleStatus(ZoneBatchType.Antonica, false);
            DisplayZoneBundleStatus(ZoneBatchType.Faydwer, false);
            DisplayZoneBundleStatus(ZoneBatchType.Odus, false);
            DisplayZoneBundleStatus(ZoneBatchType.Kunark, false);
            DisplayZoneBundleStatus(ZoneBatchType.Velious, false);
            DisplayZoneBundleStatus(ZoneBatchType.Planes, false);
            DisplayZoneBundleStatus(ZoneBatchType.Misc, true);
            EditorGUILayout.EndScrollView();
        }

        private void DisplayZoneBundleStatus(ZoneBatchType type, bool optional)
        {
            var zones = ImportHelper.GetBatchZoneNames(type);
            zones.Sort();

            for (int i = zones.Count - 1; i >= 0; i--)
            {
                if (AssetBundleHelper.DoesZoneBundleExist(zones[i]))
                {
                    zones.RemoveAt(i);
                }
            }

            if (zones.Count == 0)
            {
                DrawInfoBox(new List<string> { $"All {type} zone bundles found." }, FoundImage, true);
            }
            else
            {
                DrawInfoBox(new List<string> { $"Missing {type} zone bundles: {string.Join(", ", zones)}" },
                    optional ? NotFoundOptionalImage : NotFoundImage, true);
            }
        }

        private void DisplayGlobalBundleStatus(LanternAssetBundleId assetBundleId)
        {
            bool doesExist = AssetBundleHelper.DoesGlobalBundleExist(assetBundleId);

            if (doesExist)
            {
                DrawInfoBox(new List<string> { $"Bundle {assetBundleId} found." }, FoundImage, true);
            }
            else
            {
                DrawInfoBox(new List<string> { $"Bundle {assetBundleId} not found." }, NotFoundImage, true);
            }
        }
    }
}
