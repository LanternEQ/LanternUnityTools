using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lantern
{
    public class ZoneMeshSunlightValues : MonoBehaviour
    {
        private List<Color> _vertexValues = new List<Color>();
    
        private List<int> _indexValues = new List<int>();
    
        public void Awake()
        {
            var mf = GetComponent<MeshFilter>();
            _indexValues = mf.sharedMesh.triangles.ToList();
            _vertexValues = mf.sharedMesh.colors.ToList();
        }

        public int GetVertex(int index)
        {
            #if UNITY_EDITOR

            if (_indexValues == null || _indexValues.Count == 0)
            {
                var mf = GetComponent<MeshFilter>();
                _indexValues = mf.sharedMesh.triangles.ToList();
            }
                
            #endif
            
            return _indexValues[index];
        }

        public float GetIntensityValue(int indexValue)
        {
#if UNITY_EDITOR

            if (_vertexValues == null || _vertexValues.Count == 0)
            {
                var mf = GetComponent<MeshFilter>();
                _vertexValues = mf.sharedMesh.colors.ToList();
            }
                
#endif

            if (indexValue < 0 || indexValue >= _vertexValues.Count)
            {
                return 0f;
            }
            

            return _vertexValues[indexValue].a;
        }
    }
}
