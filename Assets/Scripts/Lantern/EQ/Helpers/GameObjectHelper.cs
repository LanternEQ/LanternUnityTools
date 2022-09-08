using UnityEngine;

namespace Lantern.EQ.Helpers
{
    public static class GameObjectHelper
    {
        public static void CleanName(GameObject gameObject)
        {
            gameObject.name = gameObject.name.Replace("(Clone)", string.Empty);
        }

        public static GameObject CreateNewGameObjectAsChild(string name, GameObject parent)
        {
            var go = new GameObject(name);
            go.transform.parent = parent.transform;
            return go;
        }

        public static Color32 GetColorFromInstanceId(int instanceId)
        {
            if (instanceId == 0)
            {
                return Color.white;
            }
            byte[] bytes = System.BitConverter.GetBytes(instanceId);
            return new Color32(bytes[0], bytes[1], bytes[2], bytes[3]);
        }
    }
}
