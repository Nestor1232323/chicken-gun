using UnityEngine;

public class CarCloneAssist : MonoBehaviour
{
	public float axesOffset = 0.4f;

	public float speed;

	public float steer;

	public WheelType wheelType;

	public Transform[] FrontWheelBodyes;

	public Transform[] BackWheelBodies;

	public AudioSource engineAudioSource;

	public Transform wheelPivot;

	private CarController car;

	private void Awake()
	{
		car = GetComponent<CarController>();
		ReplaceWheels();
	}

	private void Start()
	{
		if (engineAudioSource != null)
		{
			engineAudioSource.loop = true;
			engineAudioSource.spatialBlend = 1f;
			engineAudioSource.maxDistance = 12f;
		}
		if (!base.gameObject.GetComponent<PhotonView>().isMine)
		{
			base.transform.position = Vector3.zero;
		}
	}

	private void Update()
	{
		UpdateWheels();
	}

	private void UpdateWheels()
	{
		if (!car.IsMine)
		{
			for (int i = 0; i < FrontWheelBodyes.Length; i++)
			{
				FrontWheelBodyes[i].SetLocalEulerY(steer * 32f);
				FrontWheelBodyes[i].Rotate(FrontWheelBodyes[i].forward, speed * Time.deltaTime * 900f * (float)((i % 2 != 0) ? 1 : (-1)), Space.World);
			}
			for (int j = 0; j < BackWheelBodies.Length; j++)
			{
				BackWheelBodies[j].Rotate(BackWheelBodies[j].forward, speed * Time.deltaTime * 900f * (float)((j % 2 != 0) ? 1 : (-1)), Space.World);
			}
		}
	}

	private void ReplaceWheels()
	{
		Transform[] frontWheelPivots = GetComponent<CarController>().FrontWheelPivots;
		Transform[] backWheelPivots = GetComponent<CarController>().BackWheelPivots;
		FrontWheelBodyes = new Transform[frontWheelPivots.Length];
		BackWheelBodies = new Transform[backWheelPivots.Length];
		float num = 1f;
		for (int i = 0; i < frontWheelPivots.Length; i++)
		{
			Transform transform = frontWheelPivots[i].GetChild(0).Find("rim");
			num = frontWheelPivots[i].GetComponentInChildren<Wheel>().tireRadius * 2f;
			GameObject gameObject = Object.Instantiate(DataModel.instance.GetWheelByType(car.carInfo.wheelType));
			GeneralUtils.SetLayerRecursively(gameObject, LayerMask.NameToLayer("Vehicles"));
			gameObject.transform.SetParent(transform);
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = new Vector3(num, num, car.carInfo.wheelWidth);
			gameObject.name = "newTire";
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.SetPositionY(wheelPivot.position.y);
			FrontWheelBodyes[i] = gameObject.transform;
			Object.Destroy(transform.GetComponent<MeshRenderer>());
			Object.Destroy(transform.GetComponent<MeshFilter>());
			Object.Destroy(transform.GetChild(0).gameObject);
		}
		for (int j = 0; j < backWheelPivots.Length; j++)
		{
			Transform transform2 = backWheelPivots[j].GetChild(0).Find("rim");
			num = backWheelPivots[j].GetComponentInChildren<Wheel>().tireRadius * 2f;
			GameObject gameObject2 = Object.Instantiate(DataModel.instance.GetWheelByType(car.carInfo.wheelType));
			GeneralUtils.SetLayerRecursively(gameObject2, LayerMask.NameToLayer("Vehicles"));
			gameObject2.transform.SetParent(transform2);
			gameObject2.transform.localRotation = Quaternion.identity;
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localScale = new Vector3(num, num, car.carInfo.wheelWidth);
			gameObject2.transform.SetParent(transform2);
			gameObject2.name = "newTire";
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.SetPositionY(wheelPivot.position.y);
			BackWheelBodies[j] = gameObject2.transform;
			Object.Destroy(transform2.GetComponent<MeshRenderer>());
			Object.Destroy(transform2.GetComponent<MeshFilter>());
			Object.Destroy(transform2.GetChild(0).gameObject);
		}
	}

	public void OnPhysicsToggle(bool enabledPhysics)
	{
		Transform[] frontWheelBodyes = FrontWheelBodyes;
		foreach (Transform transform in frontWheelBodyes)
		{
			if (enabledPhysics)
			{
				transform.localPosition = Vector3.zero;
				continue;
			}
			Vector3 vector = car.transform.InverseTransformPoint(wheelPivot.position);
			Vector3 position = car.transform.InverseTransformPoint(transform.position);
			position.y = vector.y;
			transform.position = car.transform.TransformPoint(position);
		}
		frontWheelBodyes = BackWheelBodies;
		foreach (Transform transform2 in frontWheelBodyes)
		{
			if (enabledPhysics)
			{
				transform2.localPosition = Vector3.zero;
				continue;
			}
			Vector3 vector2 = car.transform.InverseTransformPoint(wheelPivot.position);
			Vector3 position2 = car.transform.InverseTransformPoint(transform2.position);
			position2.y = vector2.y;
			transform2.position = car.transform.TransformPoint(position2);
		}
	}

	public void StopEngineSound(bool stop)
	{
		if (engineAudioSource != null)
		{
			if (stop)
			{
				engineAudioSource.Stop();
			}
			else
			{
				engineAudioSource.Play();
			}
		}
	}
}
