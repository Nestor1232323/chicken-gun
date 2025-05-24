using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/AI/Vehicle Waypoint", 1)]
public class VehicleWaypoint : MonoBehaviour
{
	public VehicleWaypoint nextPoint;

	public float radius = 10f;

	[Tooltip("Percentage of a vehicle's max speed to drive at")]
	[Range(0f, 1f)]
	public float speed = 1f;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, radius);
		if ((bool)nextPoint)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(base.transform.position, nextPoint.transform.position);
		}
	}
}
