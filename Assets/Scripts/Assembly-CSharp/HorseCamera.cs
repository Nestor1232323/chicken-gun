using UnityEngine;

public class HorseCamera : BaseCamera
{
	private Transform target;

	private CarCamParams camFollowSettings;

	private byte typeOfCameraView;

	public float initNearDistance = 8f;

	private float distance;

	private float rotationDamping = 5.5f;

	private float distanceDamping = 2f;

	private float wantedDistance;

	private float deltaTime;

	private Vector3 fixedCarPos;

	private float FixTime;

	public GameObject car;

	public override void Enable()
	{
		base.enabled = true;
		Camera.main.transform.SetParent(null);
	}

	public override void Disable()
	{
		base.enabled = false;
	}

	public void SeTarget(Transform target)
	{
		this.target = target;
		camFollowSettings = target.GetComponent<HorseController>().camFollowSettings;
	}

	private void Start()
	{
		rotationDamping = 7f;
		distance = initNearDistance;
		wantedDistance = initNearDistance;
	}

	public void SetType(byte type)
	{
		typeOfCameraView = type;
		if (typeOfCameraView == 0)
		{
			initNearDistance = 8f;
		}
		else if (typeOfCameraView == 1)
		{
			initNearDistance = 15f;
		}
		else if (typeOfCameraView == 2)
		{
			fixedCarPos = base.transform.position;
			FixTime = Time.time;
		}
	}

	private void LateUpdate()
	{
		deltaTime = Time.fixedDeltaTime;
		if (typeOfCameraView == 0)
		{
			NearToCarSmoothFollow();
		}
		if (typeOfCameraView == 1)
		{
			NearToCarSmoothFollow();
		}
		if (typeOfCameraView == 2)
		{
			FixedToArenaSmoothFollow();
		}
	}

	public void updateDistance(float carSpeedCoefficient)
	{
		wantedDistance = initNearDistance * (1f + carSpeedCoefficient);
	}

	private void NearToCarSmoothFollow()
	{
		if ((bool)target)
		{
			float y = target.eulerAngles.y;
			float b = target.position.y + camFollowSettings.height;
			float y2 = base.transform.eulerAngles.y;
			float y3 = base.transform.position.y;
			y2 = Mathf.LerpAngle(y2, y, rotationDamping * deltaTime);
			distance = Mathf.Lerp(camFollowSettings.diastance, wantedDistance, distanceDamping * deltaTime);
			y3 = Mathf.Lerp(y3, b, 7f * deltaTime);
			Quaternion quaternion = Quaternion.Euler(0f, y2, 0f);
			base.transform.position = target.position;
			base.transform.position -= quaternion * Vector3.forward * distance;
			base.transform.position = new Vector3(base.transform.position.x, y3, base.transform.position.z);
			base.transform.LookAt(new Vector3(target.position.x, target.position.y + camFollowSettings.height, target.position.z));
			base.transform.SetLocalEulerX(camFollowSettings.angle);
		}
	}

	private void AwayFromCarSmoothFollow()
	{
	}

	private void FixedToArenaSmoothFollow()
	{
		base.transform.position = Vector3.Lerp(fixedCarPos, new Vector3(0f, 15f, 0f), Time.time - FixTime);
		base.transform.LookAt(new Vector3(0f, 5f, 50f));
	}
}
