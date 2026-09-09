using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Lantern.EQ.Viewers;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Lantern.EQ.Editor.Importers
{
    public class ExportBundles : LanternEditorWindow
    {
        private bool _organizeInSubfolders = true;

        private static readonly List<string> Text1 = new()
        {
            "Copies all built asset bundles to a separate folder one level up from the project in:",
            "\f../AssetBundles/",
            "Optionally, the bundles can be organized into separate folders.",
        };

        private static readonly List<string> Text2 = new()
        {
            "This process will delete all existing assets in the destination folder!",
        };

        [MenuItem("EQ/Assets/Export Bundles", false, 101)]
        public static void ExportBundles2()
        {
            GetWindow<ExportBundles>("Export Bundles", typeof(EditorWindow));
        }

        private void OnGUI()
        {
            DrawInfoBox(Text1, "d_console.infoicon");
            DrawInfoBox(Text2, "d_console.warnicon");
            _organizeInSubfolders = GUILayout.Toggle(_organizeInSubfolders, "Organize In Subfolders");

            if (GUILayout.Button("Start Export"))
            {
                Export();
            }

            if (GUILayout.Button("Open Export Folder"))
            {
                OpenFolderExplorer();
            }
        }

        private void Export()
        {
            DeleteExistingBundles();
            var assetBundlePath = AssetBundleHelper.GetAssetBundlePath();
            var files = Directory.GetFiles(assetBundlePath, "*.*", SearchOption.AllDirectories)
                .Where(f => !f.EndsWith(".meta")).ToList();

            string rootFolderPath = GetBundlePath();
            string globalsFolderPath = Path.Combine(rootFolderPath, "Global");
            string zonesFolderPath = Path.Combine(rootFolderPath, "Zones");

            if (_organizeInSubfolders)
            {
                Directory.CreateDirectory(globalsFolderPath);
                Directory.CreateDirectory(zonesFolderPath);
            }

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);

                // Skip the manifest as there are no cross bundle dependencies
                if (fileName.StartsWith("AssetBundles"))
                {
                    continue;
                }

                bool isGlobal = AssetBundleHelper.IsGlobalBundle(fileName);
                string destinationFolder = isGlobal ? globalsFolderPath : zonesFolderPath;
                File.Copy(file, Path.Combine(destinationFolder, fileName));
            }

            OpenFolderExplorer();
        }

        private static string GetProjectRootPath()
        {
            return Directory.GetParent(Application.dataPath)?.FullName;
        }

        private static string GetBundlePath()
        {
            return Path.Combine(GetProjectRootPath(), "Builds", "AssetBundles");
        }

        private static void DeleteExistingBundles()
        {
            var path = GetBundlePath();

            if (!Directory.Exists(path))
            {
                return;
            }

            DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        private static void OpenFolderExplorer()
        {
            var bundlePath = GetBundlePath();
            if (!Directory.Exists(bundlePath))
            {
                Directory.CreateDirectory(bundlePath);
            }

            if (Directory.Exists(bundlePath))
            {
                if (System.Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    Process.Start("explorer.exe", bundlePath);
                }
                else if (System.Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    Process.Start("open", bundlePath);
                }
                else
                {
                    Debug.Log("Unsupported operating system.");
                }
            }
            else
            {
                Debug.Log("Project root folder does not exist.");
            }
        }
    }
}
