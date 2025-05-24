using UnityEngine;

public class spin : MonoBehaviour
{
	[Range(0f, 1000f)]
	public float RotationSpeed;

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.Rotate(Vector3.up * Time.deltaTime * RotationSpeed);
	}
}
