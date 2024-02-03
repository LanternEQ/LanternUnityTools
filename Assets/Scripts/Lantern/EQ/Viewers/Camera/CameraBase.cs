using UnityEngine;

namespace Lantern.EQ.Viewers
{
    public abstract class CameraBase : MonoBehaviour
    {
        [SerializeField]
        protected Camera Camera;

        public Camera GetCamera()
        {
            return Camera;
        }
    }
}
