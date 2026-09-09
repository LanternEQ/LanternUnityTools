using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Lantern.EQ.Editor
{
    public abstract class LanternEditorWindow : EditorWindow
    {
        private static Dictionary<string, Texture2D> _icons;
        private static GUIStyle _folderStyle;
        private static GUIStyle _normalTextStyle;
        private float _startTime;
        protected float MinHeight = 300;

        protected LanternEditorWindow()
        {
            _icons = new Dictionary<string, Texture2D>();
            SetMinHeight(MinHeight);
        }

        protected void SetMinHeight(float minHeight)
        {
            MinHeight = minHeight;
            minSize = new Vector2(375, MinHeight);
        }

        protected void OnEnable()
        {
            _folderStyle = new GUIStyle(EditorStyles.label);
            _folderStyle.fontStyle = FontStyle.Bold;
            _folderStyle.normal.textColor = Color.white;
            _folderStyle.padding = new RectOffset(4, 4, 2, 2);

            _normalTextStyle = new GUIStyle(EditorStyles.label);
            _normalTextStyle.wordWrap = true;
        }

        protected static void DrawHorizontalLine()
        {
            GUILayout.Space(5);
            float padding = 10f;
            float lineHeight = 1f;
            float lineWidth = EditorGUIUtility.currentViewWidth - (2 * padding);
            Rect lineRect = GUILayoutUtility.GetRect(lineWidth, lineHeight);
            lineRect.x += padding;
            lineRect.width -= 2 * padding;
            EditorGUI.DrawRect(lineRect, Color.grey);
            GUILayout.Space(5);
        }

        protected static void DrawToggle(string label, ref bool value)
        {
            EditorGUILayout.BeginHorizontal();
            value = EditorGUILayout.Toggle(value, GUILayout.Width(15));
            GUILayout.Label(label);
            EditorGUILayout.EndHorizontal();
        }

        protected static void DrawTextField(string label, ref string value)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(label);
            value = EditorGUILayout.TextField(value);
            EditorGUILayout.EndHorizontal();
        }

        protected static void DrawInfoBox(List<string> lines, string iconString = "", bool smallIcon = false)
        {
            if (lines == null || lines.Count == 0)
            {
                return;
            }

            var infoBoxStyle = new GUIStyle(GUI.skin.box)
            {
                normal =
                {
                    textColor = Color.white
                }
            };

            int iconSize = smallIcon ? 20 : 40;
            const int iconMargin = 5;
            GUILayout.BeginVertical(infoBoxStyle);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(iconMargin);

            if (iconString != string.Empty)
            {
                var icon = GetIcon(iconString);
                GUILayout.Box(icon, GUILayout.Width(iconSize), GUILayout.Height(iconSize));
            }

            EditorGUILayout.BeginVertical();
            GUILayout.Space(5);

            foreach (var t in lines)
            {
                GUILayout.Label(t, t.StartsWith("\f") ? _folderStyle : _normalTextStyle);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        protected static bool DrawButton(string label)
        {
            return GUILayout.Button(label);
        }

        protected void DrawEnumPopup<T>(string label, ref T enumValue) where T : Enum
        {
            enumValue = (T)EditorGUILayout.EnumPopup(label, enumValue);
        }

        private static Texture2D GetIcon(string iconString)
        {
            if (_icons.TryGetValue(iconString, out var icon)) return icon;
            var texture2D = EditorGUIUtility.IconContent(iconString).image as Texture2D;
            _icons[iconString] = texture2D;
            return _icons[iconString];
        }

        protected void StartImport()
        {
            Close();
            _startTime = (int)EditorApplication.timeSinceStartup;
        }

        protected int FinishImport()
        {
            return (int)(EditorApplication.timeSinceStartup - _startTime);
        }
    }
}
