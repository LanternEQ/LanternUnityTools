namespace Lantern.Editor.Importers
{
    public static class DoorImporter
    {/*
        public static void CreateDoorInstances(string shortname, ZoneMeshSunlightValues sunlightValues, Transform doorRoot)
        {
            if (sunlightValues == null)
            {
                return;
            }
            
            var doorService = ServiceFactory.Get<DoorService>();

            if (doorService == null)
            {
                var bootstrapper = new ServiceBootstrapper(false, null);

                doorService = ServiceFactory.Get<DoorService>();

                if (doorService == null)
                {
                    Debug.LogError("ZoneImporter: Unable to load zone doors. Cannot bootstrap services!");
                    return;
                }
            }

            IEnumerable<Doors> doors = doorService.GetDoorsForZone(shortname);

            Dictionary<int, ClickableDoor> spawnedClickableDoors = new Dictionary<int, ClickableDoor>();

            foreach (Doors door in doors)
            {
                // Skip invisible doors
                if (door.opentype == 54)
                {
                    continue;
                }

                string prefabLoadPath = PathHelper.GetSavePath(shortname, AssetImportType.Objects) + door.name + ".prefab";

                // Get the door mesh
                GameObject loadedDoor = AssetDatabase.LoadAssetAtPath<GameObject>(prefabLoadPath);

                if (loadedDoor == null)
                {
                    Debug.LogError("ZoneImporter: Unable to find the door object asset: " +
                                   door.name + ".obj");
                    continue;
                }
                
                GameObject spawnedDoor = (GameObject) PrefabUtility.InstantiatePrefab(loadedDoor, doorRoot.transform);

                spawnedDoor.layer = LanternLayers.Door;

                spawnedDoor.transform.position = new Vector3(door.pos_y,
                    door.pos_z, door.pos_x);

                spawnedDoor = ImportHelper.FixModelParent(spawnedDoor, doorRoot.transform);

                ImportHelper.FixCloneNameAppend(spawnedDoor);

                spawnedDoor.transform.rotation =
                    Quaternion.Euler(0.0f, RotationHelper.GetEqToLanternRotation(-door.heading), 0.0f);
                
                DoorId doorScript = spawnedDoor.AddComponent<DoorId>();
                doorScript.Id = door.id;

                spawnedDoor.tag = "Clickable";

                ClickableDoor cd = spawnedDoor.AddComponent<ClickableDoor>();                
                cd.SetLockPick(door.lockpick);
                cd.SetKeyItem(door.keyitem);
                cd.SetTriggerDoor(door.triggerdoor);
                cd.SetParameter(door.door_param);
                cd.SetWidth(door.width);
                cd.SetHeading(door.heading);
                cd.SetOpenType(door.opentype); // Perform Last

                spawnedClickableDoors.Add(door.doorid, cd);

                var vcsn = spawnedDoor.GetComponent<VertexColorSetterNew>();
                List<Color> colors = new List<Color>();
                var zoneValues = Object.FindObjectOfType<ZoneMeshSunlightValues>();
                RaycastHelper.TryGetSunlightValueRuntime(spawnedDoor.transform.position, zoneValues, out var sunlightA);
                colors.Add(new Color(0, 0, 0, sunlightA));
                vcsn.SetColorData(colors);
            }

            // Loop Thru Spawned Doors and Link Any Triggered Doors
            foreach (ClickableDoor cd in spawnedClickableDoors.Values)
            {
                int triggeredDoor = cd.GetTriggerDoor();

                if (triggeredDoor != 0)
                {
                    if (spawnedClickableDoors.ContainsKey(triggeredDoor))
                    {
                        cd.SetLinkedDoor(spawnedClickableDoors[triggeredDoor]);
                    }
                }
            }
        }*/
    }
}