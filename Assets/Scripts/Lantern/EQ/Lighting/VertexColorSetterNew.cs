using System.Collections.Generic;
using Lantern.Data;
using Lantern.EQ;
using Lantern.Helpers;
using UnityEngine;

namespace Lantern.Logic
{
    public class VertexColorSetterNew : MonoBehaviour
    {
        [SerializeField] 
        private List<Color> _colors = new List<Color>();
    
        [SerializeField] 
        private List<MeshFilter> _meshFilters = new List<MeshFilter>();
        
        [SerializeField] 
        private List<Renderer> _meshRenderers = new List<Renderer>();

        [SerializeField] private ZoneMeshSunlightValues _sunlightValues;
        
        private void OnBecameVisible()
        {
            if (_colors == null || _meshFilters == null)
            {
                return;
            }
            
            ApplyColorData();
        }

        public void SetColorData(List<Color> data)
        {
            _colors = data;
            FindMeshFilters();
            ApplyColorData();
        }

        private void ApplyColorData()
        {
            if (_colors.Count == 0)
            {
                if (_sunlightValues == null)
                {
                    _sunlightValues = FindObjectOfType<ZoneMeshSunlightValues>();
                }

                RaycastHelper.TryGetSunlightValueEditor(transform.position, 5.0f,
                    _sunlightValues, out var sunlightValue);
                _colors.Add(new Color(0, 0, 0, sunlightValue));
            }
            
            if (_colors.Count == 1)
            {
                Color singleColor = _colors[0];

                MaterialPropertyBlock block = new MaterialPropertyBlock();
                
                foreach (var renderer in _meshRenderers)
                {
                    renderer.GetPropertyBlock(block);
                    block.SetFloat("_DynamicSunlight", singleColor.a);
                    renderer.SetPropertyBlock(block);
                    renderer.gameObject.layer = LanternLayers.ObjectsDynamicLit;
                }
                
                return;
            }

            var mesh = _meshFilters[0].mesh;

            int toAdd = mesh.vertices.Length - _colors.Count;

            if(toAdd > 0)
            {
                // In some rare cases, there are fewer colors than vertices.
                // The solution that seems visually correct is to consider
                // these extra colors as black. North Freeport has a few
                // of these issues.
                for (int i = 0; i < toAdd; ++i)
                {
                    _colors.Add(new Color(Random.value, Random.value, Random.value, 0.0f));
                }
            }
            else if (toAdd < 0)
            {
                // In other cases there are more colors than vertices
                // We can just trim the color size. This has been verified visually
                // with the statues in South Qeynos in the arena.
                _colors = _colors.GetRange(0, mesh.vertices.Length);
            }
            
            if (mesh.vertices.Length != _colors.Count)
            {
                Debug.LogError($"Unable to set colors for object {mesh.name}. Incorrect count.");
                return;
            }
            
            mesh.SetColors(_colors);

            foreach (var renderer in _meshRenderers)
            {
                renderer.gameObject.layer = LanternLayers.ObjectsStaticLit;
            }
        }

        public void FindMeshFilters()
        {
            // The GetComponentInChildren will include the current transform as well
            var childMesh = GetComponentsInChildren<MeshFilter>();
            if (childMesh != null && childMesh.Length != 0)
            {
                _meshFilters.AddRange(childMesh);
            }
            
            var childRenderer = GetComponentsInChildren<Renderer>();
            if (childRenderer != null && childRenderer.Length != 0)
            {
                _meshRenderers.AddRange(childRenderer);
            }
        }

        public List<Color> GetColors()
        {
            return _colors;
        }
    }
}
