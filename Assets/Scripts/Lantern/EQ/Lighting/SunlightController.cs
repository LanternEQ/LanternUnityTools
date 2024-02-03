using Lantern.EQ.Environment;
using UnityEngine;

namespace Lantern.EQ.Lighting
{
    public class SunlightController : MonoBehaviour
    {
        [SerializeField]
        private Transform _sunTransform;

        [SerializeField]
        private Light _light;

        public void UpdateTime(float time)
        {
            float angle = CalculateSunAngle(time);
            _sunTransform.localRotation = Quaternion.Euler(angle, 0f, 0f);
            _light.color = WorldLightColor.Evaluate(time);
        }

        private float CalculateSunAngle(float time)
        {
            float angle = Mathf.Lerp(-90f, 270f, time);
            return angle;
        }
    }
}
