using UnityEngine;

namespace Lantern.Helpers
{
    public static class GameObjectHelper
    {
        public static void RemoveCloneAppend(GameObject gameObject)
        {
            if (!gameObject.name.EndsWith("(Clone)"))
            {
                return;
            }

            gameObject.name = gameObject.name.Replace("(Clone)", string.Empty);
        }
    }
}
