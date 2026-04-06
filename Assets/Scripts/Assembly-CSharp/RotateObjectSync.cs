using UnityEngine;

public class RotateObjectSync : MonoBehaviour
{
	public float speed = 30f;

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.RotateAround(base.transform.position, Vector3.up, Time.deltaTime * speed);
	}
}
