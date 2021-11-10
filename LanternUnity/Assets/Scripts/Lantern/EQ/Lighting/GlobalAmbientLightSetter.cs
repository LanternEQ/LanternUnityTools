using UnityEngine;

namespace Lantern.EQ
{
    public class GlobalAmbientLightSetter : MonoBehaviour
    {
        [SerializeField]
        private Color _globalLightColor;

        private void Awake()
        {
            Shader.SetGlobalColor("_AmbientLight", _globalLightColor);
        }

        public void SetGlobalLightColor(Color color)
        {
            _globalLightColor = color;
        }
    }
}
