using UnityEngine;

public class CamTarget : MonoBehaviour
{
	public Transform target;

	private float camSpeed = 6f;

	private Vector3 lerpPos;

	private void Update()
	{
		lerpPos = (target.position - base.transform.position) * Time.deltaTime * camSpeed;
		base.transform.position += lerpPos;
	}
}
