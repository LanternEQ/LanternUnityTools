using UnityEngine;

namespace Lantern.EQ.Lighting
{
    /// <summary>
    /// Sets the global ambient light of a zone
    /// Works with the EQSimpleLit shader only
    /// </summary>
    public class AmbientLightSetterGlobal : MonoBehaviour
    {
        [SerializeField]
        private Color _globalLightColor;

        private static readonly int AmbientLight = Shader.PropertyToID("_AmbientLight");

        private void Awake()
        {
            Shader.SetGlobalColor(AmbientLight, _globalLightColor);
        }

#if UNITY_EDITOR
        public void SetGlobalLightColor(Color color)
        {
            _globalLightColor = color;
        }
#endif
    }
}
