using System.Collections;
using System.Collections.Generic;
using Lantern.EQ.Lighting;
using UnityEngine;

namespace Lantern.EQ.Animation
{
    /// <summary>
    /// Logic for creating and updating mesh vertex animations.
    /// This will be deprecated in the future in favor of a vertex shader.
    /// </summary>
    public class MeshVertexAnimation : MonoBehaviour
    {
        [SerializeField]
        private List<Mesh> _meshes;

        [SerializeField]
        private float _delay = 0.25f;

        [SerializeField]
        private MeshFilter _meshFilter;

        private List<Mesh> _copiedMeshes;
        private int _currentIndex = 0;

        private void Awake()
        {
            CopyMeshes();
        }

        private void OnEnable()
        {
            StartCoroutine(DoMeshAnimation());
        }

        private void CopyMeshes()
        {
            var vertexColorSetter = GetComponent<VertexColorSetter>();

            if (vertexColorSetter == null)
            {
                _copiedMeshes = _meshes;
                return;
            }

            _copiedMeshes = new List<Mesh>();
            List<Color> colors = vertexColorSetter.GetColors();

            // Prevents dynamically lit animated meshes from
            // throwing error
            if (colors.Count != _meshes[0].vertexCount)
            {
                colors.Clear();
            }

            foreach (Mesh oldMesh in _meshes)
            {
                Mesh newMesh = new Mesh
                {
                    vertices = oldMesh.vertices,
                    triangles = oldMesh.triangles,
                    uv = oldMesh.uv,
                    normals = oldMesh.normals,
                    colors = colors.ToArray(),
                    tangents = oldMesh.tangents,
                    subMeshCount = oldMesh.subMeshCount,
                    indexFormat = oldMesh.indexFormat
                };

                for (int i = 0; i < newMesh.subMeshCount; ++i)
                {
                    newMesh.SetIndices(oldMesh.GetIndices(i), MeshTopology.Triangles, i);
                }

                _copiedMeshes.Add(newMesh);
            }
        }

        private IEnumerator DoMeshAnimation()
        {
            while (true)
            {
                _meshFilter.mesh = _copiedMeshes[_currentIndex];
                yield return new WaitForSeconds(_delay);
                _currentIndex++;
                _currentIndex %= _meshes.Count;
            }
        }

    #if UNITY_EDITOR
        public void SetData(List<Mesh> meshes, float delay)
        {
            _meshes = meshes;
            _delay = 0.001f * delay;
            _meshFilter = GetComponent<MeshFilter>();
        }
    #endif
    }
}
