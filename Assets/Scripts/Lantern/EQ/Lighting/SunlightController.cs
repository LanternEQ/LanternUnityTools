using UnityEngine;

namespace Lantern
{
    public class SunlightController : MonoBehaviour
    {
        [SerializeField] 
        private Transform _sunTransform;

        private Vector3 _sunAngle;
    
        public void UpdateTime(float time)
        {
            _sunAngle.x = Mathf.Lerp(0f, 360f, time);
            _sunTransform.localRotation = Quaternion.Euler(_sunAngle);
        }
    }
}