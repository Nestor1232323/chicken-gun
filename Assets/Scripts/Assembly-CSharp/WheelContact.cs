using UnityEngine;

public class WheelContact
{
	public bool grounded;

	public Collider col;

	public Vector3 point;

	public Vector3 normal;

	public Vector3 relativeVelocity;

	public float distance;

	public float surfaceFriction;

	public int surfaceType;
}
