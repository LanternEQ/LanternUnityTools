using System.IO;
using Lantern.EQ.Data;
using Lantern.EQ.Editor.Helpers;
using Lantern.EQ.Helpers;
using Lantern.EQ.Lantern;
using Lantern.EQ.Viewers;
using UnityEditor;
using UnityEngine;

namespace Lantern.EQ.Editor.Importers
{
    public static class DoorImporter
    {
        public static void CreateDoorInstances(string shortname, Transform doorRoot)
        {
            var databaseLoader = new DatabaseLoader(Path.Combine(Application.streamingAssetsPath, "Database"));
            var doors = databaseLoader.GetDatabase()
                ?.Table<Doors>().Where(x => x.zone == shortname && x.opentype != EqConstants.DoorOpenTypeInvisible);

            foreach (var d in doors)
            {
                string prefabLoadPath = PathHelper.GetSavePath(shortname, AssetImportType.Objects) + d.name + ".prefab";
                var loadedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabLoadPath);

                if (loadedPrefab == null)
                {
                    Debug.LogError($"Unable to load door prefab: {d.name}");
                    continue;
                }

                var spawnedObject = Object.Instantiate(loadedPrefab);
                spawnedObject.transform.position =
                    new Vector3(d.pos_y, d.pos_z, d.pos_x);
                spawnedObject.transform.rotation =
                    Quaternion.Euler(0.0f, RotationHelper.GetEqToLanternRotation(-d.heading), 0.0f);
                spawnedObject.transform.localScale = Vector3.one;
                spawnedObject.transform.parent = doorRoot.transform;
            }
        }
    }
}
