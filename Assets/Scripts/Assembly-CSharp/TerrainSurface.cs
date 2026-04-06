using System;
using UnityEngine;

[RequireComponent(typeof(Terrain))]
[ExecuteInEditMode]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Ground Surface/Terrain Surface", 2)]
public class TerrainSurface : MonoBehaviour
{
	private Transform tr;

	private TerrainData terDat;

	private float[,,] terrainAlphamap;

	public int[] surfaceTypes = new int[0];

	[NonSerialized]
	public float[] frictions;

	private void Start()
	{
		tr = base.transform;
		if (!GetComponent<Terrain>().terrainData)
		{
			return;
		}
		terDat = GetComponent<Terrain>().terrainData;
		if (!Application.isPlaying)
		{
			return;
		}
		UpdateAlphamaps();
		frictions = new float[surfaceTypes.Length];
		for (int i = 0; i < frictions.Length; i++)
		{
			if (GroundSurfaceMaster.surfaceTypesStatic[surfaceTypes[i]].useColliderFriction)
			{
				frictions[i] = GetComponent<Collider>().material.dynamicFriction * 2f;
			}
			else
			{
				frictions[i] = GroundSurfaceMaster.surfaceTypesStatic[surfaceTypes[i]].friction;
			}
		}
	}

	private void Update()
	{
		if (!Application.isPlaying && (bool)terDat && surfaceTypes.Length != terDat.alphamapLayers)
		{
			ChangeSurfaceTypesLength();
		}
	}

	public void UpdateAlphamaps()
	{
		terrainAlphamap = terDat.GetAlphamaps(0, 0, terDat.alphamapWidth, terDat.alphamapHeight);
	}

	private void ChangeSurfaceTypesLength()
	{
		int[] array = surfaceTypes;
		surfaceTypes = new int[terDat.alphamapLayers];
		for (int i = 0; i < surfaceTypes.Length && i < array.Length; i++)
		{
			surfaceTypes[i] = array[i];
		}
	}

	public int GetDominantSurfaceTypeAtPoint(Vector3 pos)
	{
		Vector2 vector = new Vector2(Mathf.Clamp01((pos.z - tr.position.z) / terDat.size.z), Mathf.Clamp01((pos.x - tr.position.x) / terDat.size.x));
		float num = 0f;
		int num2 = 0;
		float num3 = 0f;
		for (int i = 0; i < terrainAlphamap.GetLength(2); i++)
		{
			num3 = terrainAlphamap[Mathf.FloorToInt(vector.x * (float)(terDat.alphamapWidth - 1)), Mathf.FloorToInt(vector.y * (float)(terDat.alphamapHeight - 1)), i];
			if (num3 > num)
			{
				num = num3;
				num2 = i;
			}
		}
		return surfaceTypes[num2];
	}

	public float GetFriction(int sType)
	{
		float result = 1f;
		for (int i = 0; i < surfaceTypes.Length; i++)
		{
			if (sType == surfaceTypes[i])
			{
				result = frictions[i];
				break;
			}
		}
		return result;
	}
}
