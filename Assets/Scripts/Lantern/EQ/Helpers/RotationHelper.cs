using UnityEngine;

namespace Lantern.EQ.Helpers
{
    public static class RotationHelper
    {
        public static float GetEqToLanternRotation(float rotation)
        {
            return rotation * 1.0f / 512.0f * 360.0f;
        }

        public static float GetLanternToEqRotation(float rotation)
        {
            return rotation * 1.0f / 360f * 512f / 2f;
        }

        public static float GetLanternToEqPlayerRotation(float rotation)
        {
            rotation %= 360f;
            rotation -= 90f;
            rotation /= 360f;
            rotation = (1 - rotation) * 256f;
            rotation %= 256f;
            if (rotation < 0)
            {
                rotation += 256f;
            }
            return rotation;
        }

        public static float GetEqToLanternPlayerRotation(float rotation)
        {
            if (rotation < 0)
            {
                rotation += 256f;
            }
            rotation %= 256f;
            rotation = (256 - rotation) / 256f;
            rotation *= 360f;
            rotation += 90f;
            rotation %= 360;
            return rotation;
        }

        public static float RotateWithWrap(float angle, float delta)
        {
            angle += delta;
            while (angle < 0)
            {
                angle += 360;
            }

            return angle % 360f;
        }

        public static float GetRotationToTarget(Vector3 currentPosition, Vector3 targetPosition)
        {
            Vector3 direction = PositionHelper.GetEqToLanternPosition(targetPosition, true) -
                                PositionHelper.GetEqToLanternPosition(currentPosition, true);
            direction.y = 0f; // Ensure rotation only around the y-axis

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                return targetRotation.eulerAngles.y;
            }

            return 0f;
        }
    }
}
