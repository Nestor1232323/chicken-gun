using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Vehicle Controllers/Steering Control", 2)]
public class SteeringControl : MonoBehaviour
{
	private Transform tr;

	private VehicleParent vp;

	public float steerRate = 0.1f;

	private float steerAmount;

	[Tooltip("Curve for limiting steer range based on speed, x-axis = speed, y-axis = multiplier")]
	public AnimationCurve steerCurve = AnimationCurve.Linear(0f, 1f, 30f, 0.1f);

	public bool limitSteer = true;

	[Tooltip("Horizontal stretch of the steer curve")]
	public float steerCurveStretch = 1f;

	public bool applyInReverse = true;

	public Suspension[] steeredWheels;

	[Header("Visual")]
	public bool rotate;

	public float maxDegreesRotation;

	public float rotationOffset;

	private float steerRot;

	private void Start()
	{
		tr = base.transform;
		vp = (VehicleParent)F.GetTopmostParentComponent<VehicleParent>(tr);
		steerRot = rotationOffset;
	}

	private void FixedUpdate()
	{
		float num = vp.localVelocity.z / steerCurveStretch;
		float num2 = ((!limitSteer) ? 1f : steerCurve.Evaluate((!applyInReverse) ? num : Mathf.Abs(num)));
		steerAmount = vp.steerInput * num2;
		Suspension[] array = steeredWheels;
		foreach (Suspension suspension in array)
		{
			suspension.steerAngle = Mathf.Lerp(suspension.steerAngle, steerAmount * suspension.steerFactor * (float)(suspension.steerEnabled ? 1 : 0) * (float)((!suspension.steerInverted) ? 1 : (-1)), steerRate * TimeMaster.inverseFixedTimeFactor * Time.timeScale);
		}
	}

	private void Update()
	{
		if (rotate)
		{
			steerRot = Mathf.Lerp(steerRot, steerAmount * maxDegreesRotation + rotationOffset, steerRate * Time.timeScale);
			tr.localEulerAngles = new Vector3(tr.localEulerAngles.x, tr.localEulerAngles.y, steerRot);
		}
	}
}
