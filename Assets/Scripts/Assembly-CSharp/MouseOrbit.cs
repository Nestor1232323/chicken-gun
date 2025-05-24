using UnityEngine;

public class MouseOrbit : MonoBehaviour
{
	public Transform target;

	private float distance = 15f;

	private float xSpeed = 4f;

	private float ySpeed = 1f;

	private float x;

	private float y = 2f;

	private void Start()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		x = eulerAngles.y;
		y = eulerAngles.x;
		if ((bool)GetComponent<Rigidbody>())
		{
			GetComponent<Rigidbody>().freezeRotation = true;
		}
	}

	private void LateUpdate()
	{
		distance += Input.GetAxis("Mouse ScrollWheel") * 5f;
		if (Input.GetKey(KeyCode.LeftAlt))
		{
			if (Input.GetMouseButton(1))
			{
				distance += Input.GetAxis("Mouse Y") * 0.5f;
			}
			if (Input.GetMouseButton(0))
			{
				x += Input.GetAxis("Mouse X") * xSpeed * 3f;
				y -= Input.GetAxis("Mouse Y") * ySpeed * 8f;
				y = ClampAngle(y);
				x = ClampAngle(x);
				base.transform.rotation = Quaternion.Euler(y, x, 0f);
			}
			if (Input.GetMouseButton(2))
			{
				float axis = Input.GetAxis("Mouse X");
				float axis2 = Input.GetAxis("Mouse Y");
				target.transform.position += base.transform.right * ((0f - axis) * 0.2f);
				target.transform.position += base.transform.up * ((0f - axis2) * 0.2f);
			}
		}
		base.transform.position = target.transform.position - base.transform.forward * distance;
	}

	private float ClampAngle(float angle)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return angle;
	}

	private void OnGUI()
	{
		string text = "ALT+LMB to orbit,   ALT+RMB to zoom,   ALT+MMB to pan";
		GUI.Label(new Rect(10f, 25f, 1000f, 20f), text);
	}
}
