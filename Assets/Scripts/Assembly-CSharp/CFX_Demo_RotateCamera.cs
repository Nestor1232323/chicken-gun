using UnityEngine;

public class CFX_Demo_RotateCamera : MonoBehaviour
{
	public static bool rotating = true;

	public float speed = 30f;

	public Transform rotationCenter;

	private void Update()
	{
		if (rotating)
		{
			base.transform.RotateAround(rotationCenter.position, Vector3.up, speed * Time.deltaTime);
		}
	}
}
