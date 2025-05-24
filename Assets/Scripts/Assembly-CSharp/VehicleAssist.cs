using System;
using UnityEngine;

[RequireComponent(typeof(VehicleParent))]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Vehicle Controllers/Vehicle Assist", 1)]
public class VehicleAssist : MonoBehaviour
{
	private Transform tr;

	private Rigidbody rb;

	private VehicleParent vp;

	[Header("Drift")]
	[Tooltip("Variables are multiplied based on the number of wheels grounded out of the total number of wheels")]
	public bool basedOnWheelsGrounded;

	private float groundedFactor;

	[Tooltip("How much to assist with spinning while drifting")]
	public float driftSpinAssist;

	public float driftSpinSpeed;

	public float driftSpinExponent = 1f;

	[Tooltip("Automatically adjust drift angle based on steer input magnitude")]
	public bool autoSteerDrift;

	public float maxDriftAngle = 70f;

	private float targetDriftAngle;

	[Tooltip("Adjusts the force based on drift speed, x-axis = speed, y-axis = force")]
	public AnimationCurve driftSpinCurve = AnimationCurve.Linear(0f, 0f, 10f, 1f);

	[Tooltip("How much to push the vehicle forward while drifting")]
	public float driftPush;

	[Tooltip("Straighten out the vehicle when sliding slightly")]
	public bool straightenAssist;

	[Header("Downforce")]
	public float downforce = 1f;

	public bool invertDownforceInReverse;

	public bool applyDownforceInAir;

	[Tooltip("X-axis = speed, y-axis = force")]
	public AnimationCurve downforceCurve = AnimationCurve.Linear(0f, 0f, 20f, 1f);

	[Header("Roll Over")]
	[Tooltip("Automatically roll over when rolled over")]
	public bool autoRollOver;

	[Tooltip("Roll over with steer input")]
	public bool steerRollOver;

	[NonSerialized]
	public bool rolledOver;

	[Tooltip("Distance to check on sides to see if rolled over")]
	public float rollCheckDistance = 1f;

	public float rollOverForce = 1f;

	[Tooltip("Maximum speed at which vehicle can be rolled over with assists")]
	public float rollSpeedThreshold;

	[Header("Air")]
	[Tooltip("Increase angular drag immediately after jumping")]
	public bool angularDragOnJump;

	private float initialAngularDrag;

	private float angDragTime;

	public float fallSpeedLimit = float.PositiveInfinity;

	public bool applyFallLimitUpwards;

	private void Start()
	{
		tr = base.transform;
		rb = GetComponent<Rigidbody>();
		vp = GetComponent<VehicleParent>();
		initialAngularDrag = rb.angularDrag;
	}

	private void FixedUpdate()
	{
		if (vp.groundedWheels > 0)
		{
			groundedFactor = ((!basedOnWheelsGrounded) ? 1 : (vp.groundedWheels / ((!vp.hover) ? vp.wheels.Length : vp.hoverWheels.Length)));
			angDragTime = 20f;
			rb.angularDrag = initialAngularDrag;
			if (driftSpinAssist > 0f)
			{
				ApplySpinAssist();
			}
			if (driftPush > 0f)
			{
				ApplyDriftPush();
			}
		}
		else if (angularDragOnJump)
		{
			angDragTime = Mathf.Max(0f, angDragTime - Time.timeScale * TimeMaster.inverseFixedTimeFactor);
			rb.angularDrag = ((!(angDragTime > 0f) || !((double)vp.upDot > 0.5)) ? initialAngularDrag : 10f);
		}
		if (downforce > 0f)
		{
			ApplyDownforce();
		}
		if (autoRollOver || steerRollOver)
		{
			RollOver();
		}
		if (Mathf.Abs(vp.localVelocity.y) > fallSpeedLimit && (vp.localVelocity.y < 0f || applyFallLimitUpwards))
		{
			rb.AddRelativeForce(Vector3.down * vp.localVelocity.y, ForceMode.Acceleration);
		}
	}

	private void ApplySpinAssist()
	{
		float num = 0f;
		if (autoSteerDrift)
		{
			int num2 = 0;
			if (vp.steerInput != 0f)
			{
				num2 = (int)Mathf.Sign(vp.steerInput);
			}
			targetDriftAngle = (((float)num2 == Mathf.Sign(vp.localVelocity.x)) ? ((float)num2) : vp.steerInput) * (0f - maxDriftAngle);
			Vector3 normalized = new Vector3(vp.localVelocity.x, 0f, vp.localVelocity.z).normalized;
			Vector3 normalized2 = new Vector3(Mathf.Sin(targetDriftAngle * ((float)Math.PI / 180f)), 0f, Mathf.Cos(targetDriftAngle * ((float)Math.PI / 180f))).normalized;
			Vector3 vector = normalized - normalized2;
			num = vector.magnitude * Mathf.Sign(vector.z) * (float)num2 * driftSpinSpeed - vp.localAngularVel.y * Mathf.Clamp01(Vector3.Dot(normalized, normalized2)) * 2f;
		}
		else
		{
			num = vp.steerInput * driftSpinSpeed * ((!(vp.localVelocity.z < 0f)) ? 1f : ((!vp.accelAxisIsBrake) ? Mathf.Sign(F.MaxAbs(vp.accelInput, 0f - vp.brakeInput)) : Mathf.Sign(vp.accelInput)));
		}
		rb.AddRelativeTorque(new Vector3(0f, (num - vp.localAngularVel.y) * driftSpinAssist * driftSpinCurve.Evaluate(Mathf.Abs(Mathf.Pow(vp.localVelocity.x, driftSpinExponent))) * groundedFactor, 0f), ForceMode.Acceleration);
		float num3 = Vector3.Dot(tr.right, rb.velocity.normalized);
		if (straightenAssist && vp.steerInput == 0f && Mathf.Abs(num3) < 0.1f && vp.sqrVelMag > 5f)
		{
			rb.AddRelativeTorque(new Vector3(0f, num3 * 100f * Mathf.Sign(vp.localVelocity.z) * driftSpinAssist, 0f), ForceMode.Acceleration);
		}
	}

	private void ApplyDownforce()
	{
		if (vp.groundedWheels > 0 || applyDownforceInAir)
		{
			rb.AddRelativeForce(new Vector3(0f, downforceCurve.Evaluate(Mathf.Abs(vp.localVelocity.z)) * (0f - downforce) * ((!applyDownforceInAir) ? groundedFactor : 1f) * ((!invertDownforceInReverse) ? 1f : Mathf.Sign(vp.localVelocity.z)), 0f), ForceMode.Acceleration);
			if (invertDownforceInReverse && vp.localVelocity.z < 0f)
			{
				rb.AddRelativeTorque(new Vector3(downforceCurve.Evaluate(Mathf.Abs(vp.localVelocity.z)) * downforce * ((!applyDownforceInAir) ? groundedFactor : 1f), 0f, 0f), ForceMode.Acceleration);
			}
		}
	}

	private void RollOver()
	{
		if (vp.groundedWheels == 0 && vp.velMag < rollSpeedThreshold && (double)vp.upDot < 0.8 && rollCheckDistance > 0f)
		{
			if (Physics.Raycast(tr.position, vp.upDir, out var hitInfo, rollCheckDistance, GlobalControl.groundMaskStatic) || Physics.Raycast(tr.position, vp.rightDir, out hitInfo, rollCheckDistance, GlobalControl.groundMaskStatic) || Physics.Raycast(tr.position, -vp.rightDir, out hitInfo, rollCheckDistance, GlobalControl.groundMaskStatic))
			{
				rolledOver = true;
			}
			else
			{
				rolledOver = false;
			}
		}
		else
		{
			rolledOver = false;
		}
		if (rolledOver)
		{
			if (steerRollOver && vp.steerInput != 0f)
			{
				rb.AddRelativeTorque(new Vector3(0f, 0f, (0f - vp.steerInput) * rollOverForce), ForceMode.Acceleration);
			}
			else if (autoRollOver)
			{
				rb.AddRelativeTorque(new Vector3(0f, 0f, (0f - Mathf.Sign(vp.rightDot)) * rollOverForce), ForceMode.Acceleration);
			}
		}
	}

	private void ApplyDriftPush()
	{
		float f = ((!vp.accelAxisIsBrake) ? (vp.accelInput - vp.brakeInput) : vp.accelInput) * Mathf.Abs(vp.localVelocity.x) * driftPush * groundedFactor * (1f - Mathf.Abs(Vector3.Dot(vp.forwardDir, rb.velocity.normalized)));
		rb.AddForce(vp.norm.TransformDirection(new Vector3(Mathf.Abs(f) * Mathf.Sign(vp.localVelocity.x), Mathf.Abs(f) * Mathf.Sign(vp.localVelocity.z), 0f)), ForceMode.Acceleration);
	}
}
