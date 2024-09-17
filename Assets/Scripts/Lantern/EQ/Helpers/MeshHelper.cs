using UnityEngine;

namespace Lantern.EQ.Helpers
{
    public static class MeshHelper
    {
        public static void RotateMesh(Mesh mesh, float rotationDegrees)
        {
            Vector3[] vertices = mesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = Quaternion.Euler(0f, rotationDegrees, 0f) * vertices[i];
            }
            mesh.SetVertices(vertices);
        }
    }
}
