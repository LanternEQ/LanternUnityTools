using Lantern.EQ.Lantern;
using UnityEngine;

namespace Lantern.EQ.Helpers
{
    public static class PositionHelper
    {
        public static Vector3 GetSwappedYZPosition(Vector3 position)
        {
            return new Vector3 {x = position.x, y = position.z, z = position.y};
        }

        public static Vector3 GetLanternToEqPosition(Vector3 position, bool swapYZ = false)
        {
            return new Vector3 {x = position.x / LanternConstants.WorldScale, y = (swapYZ ? position.z : position.y)
                / LanternConstants.WorldScale, z = (swapYZ? position.y : position.z) / LanternConstants.WorldScale};
        }

        public static Vector3 GetEqToLanternPosition(Vector3 position, bool swapYZ = false)
        {
            return new Vector3 {x = position.x * LanternConstants.WorldScale, y = (swapYZ ? position.z : position.y)
                * LanternConstants.WorldScale, z = (swapYZ ? position.y : position.z) * LanternConstants.WorldScale};
        }

        public static Vector3 GetEqDatabaseToLanternPosition(Vector3 position)
        {
            return new Vector3 {x = position.y * LanternConstants.WorldScale, y = position.z * LanternConstants.WorldScale, z = position.x * LanternConstants.WorldScale};
        }

        public static Vector3 GetEqDatabaseToLanternPosition(float x, float y, float z)
        {
            return new Vector3 {x = y * LanternConstants.WorldScale, y = z * LanternConstants.WorldScale, z = x * LanternConstants.WorldScale};
        }

        public static Vector3 GetEqDatabaseToEqPosition(Vector3 position)
        {
            return new Vector3 {x = position.y, y = position.x, z = position.z};
        }

        public static Vector3 GetEqDatabaseToEqPosition(float x, float y, float z)
        {
            return new Vector3 {x = y, y = x, z = z};
        }
    }
}
