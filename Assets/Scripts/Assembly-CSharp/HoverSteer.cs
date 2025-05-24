using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Hover/Hover Steer", 2)]
public class HoverSteer : MonoBehaviour
{
	private Transform tr;

	private VehicleParent vp;

	public float steerRate = 1f;

	private float steerAmount;

	[Tooltip("Curve for limiting steer range based on speed, x-axis = speed, y-axis = multiplier")]
	public AnimationCurve steerCurve = AnimationCurve.Linear(0f, 1f, 30f, 0.1f);

	[Tooltip("Horizontal stretch of the steer curve")]
	public float steerCurveStretch = 1f;

	public HoverWheel[] steeredWheels;

	[Header("Visual")]
	public bool rotate;

	public float maxDegreesRotation;

	public float rotationOffset;

	private float steerRot;

	private void Start()
	{
		tr = base.transform;
		vp = (VehicleParent)F.GetTopmostParentComponent<VehicleParent>(tr);
	}

	private void FixedUpdate()
	{
		float f = vp.localVelocity.z / steerCurveStretch;
		float num = steerCurve.Evaluate(Mathf.Abs(f));
		steerAmount = vp.steerInput * num;
		HoverWheel[] array = steeredWheels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].steerRate = steerAmount * steerRate;
		}
	}

	private void Update()
	{
		if (rotate)
		{
			steerRot = Mathf.Lerp(steerRot, steerAmount * maxDegreesRotation + rotationOffset, steerRate * 0.1f * Time.timeScale);
			tr.localEulerAngles = new Vector3(tr.localEulerAngles.x, tr.localEulerAngles.y, steerRot);
		}
	}
}
