using UnityEngine;

public class VertHandler : MonoBehaviour
{
	private Mesh mesh;

	private Vector3[] verts;

	private Vector3 vertPos;

	private GameObject[] handles;

	private void OnEnable()
	{
		mesh = GetComponent<MeshFilter>().mesh;
		verts = mesh.vertices;
		for (int i = 0; i < verts.Length; i++)
		{
			verts[i].x = 10f + Random.value * 5f;
		}
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
	}

	private void OnDisable()
	{
	}

	private void Update()
	{
	}
}
