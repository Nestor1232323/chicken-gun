using UnityEngine;

[RequireComponent(typeof(VehicleParent))]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Vehicle Controllers/Vehicle Balance", 4)]
public class VehicleBalance : MonoBehaviour
{
	private Transform tr;

	private Rigidbody rb;

	private VehicleParent vp;

	private float actualPitchInput;

	private Vector3 targetLean;

	private Vector3 targetLeanActual;

	[Tooltip("Lean strength along each axis")]
	public Vector3 leanFactor;

	[Range(0f, 0.99f)]
	public float leanSmoothness;

	[Tooltip("Adjusts the roll based on the speed, x-axis = speed, y-axis = roll amount")]
	public AnimationCurve leanRollCurve = AnimationCurve.Linear(0f, 0f, 10f, 1f);

	[Tooltip("Adjusts the pitch based on the speed, x-axis = speed, y-axis = pitch amount")]
	public AnimationCurve leanPitchCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[Tooltip("Adjusts the yaw based on the speed, x-axis = speed, y-axis = yaw amount")]
	public AnimationCurve leanYawCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[Tooltip("Speed above which endos (forward wheelies) aren't allowed")]
	public float endoSpeedThreshold;

	[Tooltip("Exponent for pitch input")]
	public float pitchExponent;

	[Tooltip("How much to lean when sliding sideways")]
	public float slideLeanFactor = 1f;

	private void Start()
	{
		tr = base.transform;
		rb = GetComponent<Rigidbody>();
		vp = GetComponent<VehicleParent>();
	}

	private void FixedUpdate()
	{
		actualPitchInput = ((vp.wheels.Length != 1) ? Mathf.Clamp(vp.pitchInput, -1f, (!(vp.velMag > endoSpeedThreshold)) ? 1 : 0) : 0f);
		if (vp.groundedWheels > 0 && leanFactor != Vector3.zero)
		{
			ApplyLean();
		}
	}

	private void ApplyLean()
	{
		if (vp.groundedWheels > 0)
		{
			Vector3 direction = vp.norm.InverseTransformDirection((!(Vector3.Dot(vp.wheelNormalAverage, GlobalControl.worldUpDir) <= 0f)) ? Vector3.Lerp(GlobalControl.worldUpDir, vp.wheelNormalAverage, Mathf.Abs(Vector3.Dot(vp.norm.up, GlobalControl.worldUpDir)) * 2f) : vp.wheelNormalAverage);
			Debug.DrawRay(tr.position, vp.norm.TransformDirection(direction), Color.white);
			targetLean = new Vector3(Mathf.Lerp(direction.x, Mathf.Clamp((0f - vp.rollInput) * leanFactor.z * leanRollCurve.Evaluate(Mathf.Abs(vp.localVelocity.z)) + Mathf.Clamp(vp.localVelocity.x * slideLeanFactor, (0f - leanFactor.z) * slideLeanFactor, leanFactor.z * slideLeanFactor), 0f - leanFactor.z, leanFactor.z), Mathf.Max(Mathf.Abs(F.MaxAbs(vp.steerInput, vp.rollInput)))), Mathf.Pow(Mathf.Abs(actualPitchInput), pitchExponent) * Mathf.Sign(actualPitchInput) * leanFactor.x, direction.z * (1f - Mathf.Abs(F.MaxAbs(actualPitchInput * leanFactor.x, vp.rollInput * leanFactor.z))));
		}
		else
		{
			targetLean = vp.upDir;
		}
		targetLeanActual = Vector3.Lerp(targetLeanActual, vp.norm.TransformDirection(targetLean), (1f - leanSmoothness) * Time.timeScale * TimeMaster.inverseFixedTimeFactor).normalized;
		Debug.DrawRay(tr.position, targetLeanActual, Color.black);
		rb.AddTorque(vp.norm.right * (0f - (Vector3.Dot(vp.forwardDir, targetLeanActual) * 20f - vp.localAngularVel.x)) * 100f * ((vp.wheels.Length != 1) ? leanPitchCurve.Evaluate(Mathf.Abs(actualPitchInput)) : 1f), ForceMode.Acceleration);
		rb.AddTorque(vp.norm.forward * ((vp.groundedWheels != 1) ? 0f : (vp.steerInput * leanFactor.y - vp.norm.InverseTransformDirection(rb.angularVelocity).z)) * 100f * leanYawCurve.Evaluate(Mathf.Abs(vp.steerInput)), ForceMode.Acceleration);
		rb.AddTorque(vp.norm.up * ((0f - Vector3.Dot(vp.rightDir, targetLeanActual)) * 20f - vp.localAngularVel.z) * 100f, ForceMode.Acceleration);
		if (vp.groundedWheels == 1 && leanFactor.y > 0f)
		{
			rb.AddTorque(vp.norm.TransformDirection(new Vector3(0f, 0f, vp.steerInput * leanFactor.y - vp.norm.InverseTransformDirection(rb.angularVelocity).z)), ForceMode.Acceleration);
		}
	}
}
