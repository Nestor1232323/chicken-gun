using UnityEngine;

[RequireComponent(typeof(VehicleParent))]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Input/Mobile Input Getter", 2)]
public class MobileInputGet : MonoBehaviour
{
	private VehicleParent vp;

	private MobileInput setter;

	public float steerFactor = 1f;

	public float flipFactor = 1f;

	public bool useAccelerometer = true;

	[Tooltip("Multiplier for input addition based on rate of change of input")]
	public float deltaFactor = 10f;

	private Vector3 accelerationPrev;

	private Vector3 accelerationDelta;

	private void Start()
	{
		vp = GetComponent<VehicleParent>();
		setter = Object.FindObjectOfType<MobileInput>();
	}

	private void FixedUpdate()
	{
		if ((bool)setter)
		{
			accelerationDelta = Input.acceleration - accelerationPrev;
			accelerationPrev = Input.acceleration;
			vp.SetAccel(setter.accel);
			vp.SetBrake(setter.brake);
			vp.SetEbrake(setter.ebrake);
			vp.SetBoost(setter.boost);
			if (useAccelerometer)
			{
				vp.SetSteer((Input.acceleration.x + accelerationDelta.x * deltaFactor) * steerFactor);
				vp.SetYaw(Input.acceleration.x * flipFactor);
				vp.SetPitch((0f - Input.acceleration.z) * flipFactor);
			}
			else
			{
				vp.SetSteer(setter.steer);
			}
		}
	}
}
