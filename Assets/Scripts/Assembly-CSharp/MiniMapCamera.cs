using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
	public Transform target;

	public float height = 15f;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void LateUpdate()
	{
		if ((bool)target)
		{
			base.transform.position = new Vector3(target.position.x, target.position.y + height, target.position.z);
			base.transform.LookAt(target);
		}
	}
}
