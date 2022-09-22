using UnityEngine;

namespace Lantern.EQ.Lighting
{
    /// <summary>
    /// Holds vertex color and index info for zone mesh
    /// Used to calculate ambient light for actors in the world
    /// </summary>
    public class ZoneAmbientLightValues : MonoBehaviour
    {
        /// <summary>
        /// Zone mesh filter
        /// </summary>
        [SerializeField]
        private MeshFilter _meshFilter;

        /// <summary>
        /// Vertex color values from zone mesh
        /// </summary>
        private Color[] _vertexValues;

        /// <summary>
        /// Index values from zone mesh
        /// </summary>
        private int[] _indexValues;

        private void Awake()
        {
            var sharedMesh = _meshFilter.sharedMesh;
            _indexValues = sharedMesh.triangles;
            _vertexValues = sharedMesh.colors;
        }

        public int GetVertex(int index)
        {
#if UNITY_EDITOR
            if (_indexValues == null || _indexValues.Length == 0)
            {
                _indexValues = _meshFilter.sharedMesh.triangles;
            }
#endif

            return _indexValues[index];
        }

        public float GetIntensityValue(int indexValue)
        {
#if UNITY_EDITOR
            if (_vertexValues == null || _vertexValues.Length == 0)
            {
                _vertexValues = _meshFilter.sharedMesh.colors;
            }
#endif

            if (indexValue < 0 || indexValue >= _vertexValues.Length)
            {
                return 0f;
            }

            return _vertexValues[indexValue].a;
        }

        #if UNITY_EDITOR
        public void SetMeshFilter(MeshFilter meshFilter)
        {
            _meshFilter = meshFilter;
        }
        #endif
    }
}
