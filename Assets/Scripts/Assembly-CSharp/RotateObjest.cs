using UnityEngine;

public class RotateObjest : MonoBehaviour
{
	public Vector3 angles;

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.Rotate(angles * Time.deltaTime);
	}

	public void Stop()
	{
		base.enabled = false;
	}

	public void StartRotating()
	{
		base.enabled = true;
	}

	public void SetSpeed(Vector3 speeds)
	{
		angles = speeds;
	}
}
