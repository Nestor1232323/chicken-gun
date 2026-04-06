using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Ground Surface/Ground Surface Master", 0)]
public class GroundSurfaceMaster : MonoBehaviour
{
	public GroundSurface[] surfaceTypes;

	public static GroundSurface[] surfaceTypesStatic;

	private void Start()
	{
		surfaceTypesStatic = surfaceTypes;
	}
}
