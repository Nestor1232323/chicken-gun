using UnityEngine;
using UnityEngine.UI;

public class CamControl : MonoBehaviour
{
	public Camera cam;

	public Transform pivot;

	public Transform lookPos;

	public Slider lookPosOffsetX;

	public Slider lookPosOffsetY;

	public Transform target;

	public Transform ground;

	public float rotateSpeed = 10f;

	public float tiltMax = 40f;

	public float tiltMin = 30f;

	private bool rotateEnable = true;

	private bool UIArea;

	public bool AutoRotate;

	private Vector3 rotation;

	public float[] zoom = new float[3];

	public float[] lookPosOffset = new float[3];

	public float smooth = 5f;

	private int zoomIdx;

	private void Update()
	{
		if (Input.GetMouseButton(0))
		{
			rotation.y = Input.GetAxis("Mouse X") * rotateSpeed;
			rotation.x = Input.GetAxis("Mouse Y") * rotateSpeed;
		}
		else
		{
			rotation = Vector3.zero;
		}
		if (rotateEnable && !UIArea)
		{
			CamRotate(rotation);
		}
		if (AutoRotate)
		{
			CamRotate(new Vector3(0f, rotateSpeed * 3f * Time.deltaTime, 0f));
		}
		if (Input.GetMouseButtonDown(1))
		{
			CamZoom();
		}
		cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoom[zoomIdx], Time.deltaTime * smooth);
		float x;
		float y;
		if ((bool)lookPosOffsetX)
		{
			x = Mathf.Lerp(lookPos.localPosition.x, lookPosOffsetX.value, Time.deltaTime * smooth);
			y = Mathf.Lerp(lookPos.localPosition.y, lookPosOffset[zoomIdx] + lookPosOffsetY.value, Time.deltaTime * smooth);
		}
		else
		{
			x = 0f;
			y = Mathf.Lerp(lookPos.localPosition.y, lookPosOffset[zoomIdx], Time.deltaTime * smooth);
		}
		lookPos.localPosition = new Vector3(x, y, lookPos.localPosition.z);
		cam.transform.LookAt(lookPos);
	}

	private void LateUpdate()
	{
		Vector3 position = ground.position;
		if (target.position.x - ground.position.x >= 5f)
		{
			position.x += 5f;
			ground.position = position;
		}
		else if (target.position.x - ground.position.x <= -5f)
		{
			position.x -= 5f;
			ground.position = position;
		}
		if (target.position.z - ground.position.z >= 5f)
		{
			position.z += 5f;
			ground.position = position;
		}
		else if (target.position.z - ground.position.z <= -5f)
		{
			position.z -= 5f;
			ground.position = position;
		}
		base.transform.position = target.position;
	}

	private void CamRotate(Vector3 rot)
	{
		float y = base.transform.rotation.eulerAngles.y + rot.y;
		base.transform.rotation = Quaternion.Euler(0f, y, 0f);
		Vector3 eulers = new Vector3(rot.x * 3f, 0f, 0f);
		pivot.Rotate(eulers, Space.Self);
		float x = pivot.localRotation.eulerAngles.x;
		if (x > 180f)
		{
			x -= 360f;
			if (x < 0f - tiltMin)
			{
				pivot.localRotation = Quaternion.Euler(0f - tiltMin, 0f, 0f);
			}
		}
		else if (x > tiltMax)
		{
			pivot.localRotation = Quaternion.Euler(tiltMax, 0f, 0f);
		}
	}

	public void CamZoom()
	{
		zoomIdx++;
		zoomIdx = (int)Mathf.Repeat(zoomIdx, zoom.Length);
	}

	public void RotateOption(bool enable)
	{
		rotateEnable = enable;
	}

	public void isUIArea(bool param)
	{
		UIArea = param;
	}
}
