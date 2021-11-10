using System.Collections;
using System.Collections.Generic;
using Lantern.Logic;
using UnityEngine;

public class MeshAnimatedVertices : MonoBehaviour
{
	[SerializeField]
	private List<Mesh> _meshes;

	private List<Mesh> _copiedMeshes;

	[SerializeField]
	private float _delay = 0.25f;

	private MeshFilter _meshFilter;
	private MeshRenderer _meshRenderer;

	private int _currentIndex = 0;

	// Use this for initialization
	void Start ()
	{
		CopyMeshes();
		_meshFilter = GetComponent<MeshFilter>();
		_meshRenderer = GetComponent<MeshRenderer>();
		StartCoroutine(SwapAnimatedModels());
	}

	private void CopyMeshes()
	{
		var vertexColorSetter = GetComponent<VertexColorSetterNew>();

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

	private IEnumerator SwapAnimatedModels()
	{
		while (true)
		{
			_meshFilter.mesh = _copiedMeshes[_currentIndex];
			yield return new WaitForSeconds(_delay);
			_currentIndex++;
			_currentIndex %= _meshes.Count;
		}
	}

	public void SetData(List<Mesh> meshes, float delay)
	{
		_meshes = meshes;		
		_delay = 0.001f * delay;
	}

	public List<Mesh> GetMeshes()
	{
		return _meshes;
	}
}
