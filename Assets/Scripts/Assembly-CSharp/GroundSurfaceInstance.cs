using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Ground Surface/Ground Surface Instance", 1)]
public class GroundSurfaceInstance : MonoBehaviour
{
	[Tooltip("Which surface type to use from the GroundSurfaceMaster list of surface types")]
	public int surfaceType;

	[NonSerialized]
	public float friction;

	private void Start()
	{
		if (GroundSurfaceMaster.surfaceTypesStatic[surfaceType].useColliderFriction)
		{
			friction = GetComponent<Collider>().material.dynamicFriction * 2f;
		}
		else
		{
			friction = GroundSurfaceMaster.surfaceTypesStatic[surfaceType].friction;
		}
	}
}
