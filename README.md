# LanternUnityImporter
EverQuest zone importer for the Unity Engine.

# Overview
The Unity Importer project allows you to import the extracted zone content into Unity. The process is completed automated and handles spawning the zone, objects and fixing all material and shader references. It has been tested on Unity 2018.2.x but may work with newer versions as well.

# How To Use
1. Download the Unity project from the GitHub repo.
2. Open the project in Unity.
3. Put the folder of the zone you have extracted (will match the zone shortname) into the ZoneExports folder in the Unity project.
4. In the top bar, select `EQ->Editor->Import Zone` or press `ALT + Z`.
5. Enter the shortname (e.g. arena, gfaydark) of the zone contents you have added to the ZoneExports folder.
6. Click `Import`

The editor may take a while to import and fix all assets depending on the size of the zone. If you come across a zone that fails to load or looks incorrect, please create a bug report.
