using System;
using UnityEngine;

[RequireComponent(typeof(VehicleParent))]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Stunt/Flip Control", 2)]
public class FlipControl : MonoBehaviour
{
	private Transform tr;

	private Rigidbody rb;

	private VehicleParent vp;

	public bool disableDuringCrash;

	public Vector3 flipPower;

	[Tooltip("Continue spinning if input is stopped")]
	public bool freeSpinFlip;

	[Tooltip("Stop spinning if input is stopped and vehicle is upright")]
	public bool stopFlip;

	[Tooltip("How quickly the vehicle will rotate upright in air")]
	public Vector3 rotationCorrection;

	private Quaternion velDir;

	[Tooltip("Distance to check for ground for reference normal for rotation correction")]
	public float groundCheckDistance = 100f;

	[Tooltip("Minimum dot product between ground normal and global up direction for rotation correction")]
	public float groundSteepnessLimit = 0.5f;

	[Tooltip("How quickly the vehicle will dive in the direction it's soaring")]
	public float diveFactor;

	private void Start()
	{
		tr = base.transform;
		rb = GetComponent<Rigidbody>();
		vp = GetComponent<VehicleParent>();
	}

	private void FixedUpdate()
	{
		if (vp.groundedWheels == 0 && (!vp.crashing || (vp.crashing && !disableDuringCrash)))
		{
			velDir = Quaternion.LookRotation(GlobalControl.worldUpDir, rb.velocity);
			if (flipPower != Vector3.zero)
			{
				ApplyFlip();
			}
			if (stopFlip)
			{
				ApplyStopFlip();
			}
			if (rotationCorrection != Vector3.zero)
			{
				ApplyRotationCorrection();
			}
			if (diveFactor > 0f)
			{
				Dive();
			}
		}
	}

	private void ApplyFlip()
	{
		Vector3 torque = ((!freeSpinFlip) ? new Vector3((vp.pitchInput == 0f || !(Mathf.Abs(vp.localAngularVel.x) > 1f) || Math.Sign(vp.pitchInput * Mathf.Sign(flipPower.x)) == Math.Sign(vp.localAngularVel.x)) ? (vp.pitchInput * flipPower.x - vp.localAngularVel.x * (1f - Mathf.Abs(vp.pitchInput)) * Mathf.Abs(flipPower.x)) : ((0f - vp.localAngularVel.x) * Mathf.Abs(flipPower.x)), (vp.yawInput == 0f || !(Mathf.Abs(vp.localAngularVel.y) > 1f) || Math.Sign(vp.yawInput * Mathf.Sign(flipPower.y)) == Math.Sign(vp.localAngularVel.y)) ? (vp.yawInput * flipPower.y - vp.localAngularVel.y * (1f - Mathf.Abs(vp.yawInput)) * Mathf.Abs(flipPower.y)) : ((0f - vp.localAngularVel.y) * Mathf.Abs(flipPower.y)), (vp.rollInput == 0f || !(Mathf.Abs(vp.localAngularVel.z) > 1f) || Math.Sign(vp.rollInput * Mathf.Sign(flipPower.z)) == Math.Sign(vp.localAngularVel.z)) ? (vp.rollInput * flipPower.z - vp.localAngularVel.z * (1f - Mathf.Abs(vp.rollInput)) * Mathf.Abs(flipPower.z)) : ((0f - vp.localAngularVel.z) * Mathf.Abs(flipPower.z))) : new Vector3(vp.pitchInput * flipPower.x, vp.yawInput * flipPower.y, vp.rollInput * flipPower.z));
		rb.AddRelativeTorque(torque, ForceMode.Acceleration);
	}

	private void ApplyStopFlip()
	{
		Vector3 zero = Vector3.zero;
		zero.x = ((vp.pitchInput * flipPower.x != 0f) ? 0f : (Mathf.Pow(Mathf.Clamp01(vp.upDot), Mathf.Clamp(10f - Mathf.Abs(vp.localAngularVel.x), 2f, 10f)) * 10f));
		zero.y = ((vp.yawInput * flipPower.y != 0f || !(vp.sqrVelMag > 5f)) ? 0f : (Mathf.Pow(Mathf.Clamp01(Vector3.Dot(vp.forwardDir, velDir * Vector3.up)), Mathf.Clamp(10f - Mathf.Abs(vp.localAngularVel.y), 2f, 10f)) * 10f));
		zero.z = ((vp.rollInput * flipPower.z != 0f) ? 0f : (Mathf.Pow(Mathf.Clamp01(vp.upDot), Mathf.Clamp(10f - Mathf.Abs(vp.localAngularVel.z), 2f, 10f)) * 10f));
		rb.AddRelativeTorque(new Vector3((0f - vp.localAngularVel.x) * zero.x, (0f - vp.localAngularVel.y) * zero.y, (0f - vp.localAngularVel.z) * zero.z), ForceMode.Acceleration);
	}

	private void ApplyRotationCorrection()
	{
		float num = vp.forwardDot;
		float num2 = vp.rightDot;
		float f = vp.upDot;
		if (groundCheckDistance > 0f && Physics.Raycast(tr.position, (-GlobalControl.worldUpDir + rb.velocity).normalized, out var hitInfo, groundCheckDistance, GlobalControl.groundMaskStatic) && Vector3.Dot(hitInfo.normal, GlobalControl.worldUpDir) >= groundSteepnessLimit)
		{
			num = Vector3.Dot(vp.forwardDir, hitInfo.normal);
			num2 = Vector3.Dot(vp.rightDir, hitInfo.normal);
			f = Vector3.Dot(vp.upDir, hitInfo.normal);
		}
		rb.AddRelativeTorque(new Vector3((vp.pitchInput * flipPower.x != 0f) ? 0f : (num * (1f - Mathf.Abs(num2)) * rotationCorrection.x - vp.localAngularVel.x * Mathf.Pow(f, 2f) * 10f), (vp.yawInput * flipPower.y != 0f || !(vp.sqrVelMag > 10f)) ? 0f : (Vector3.Dot(vp.forwardDir, velDir * Vector3.right) * Mathf.Abs(f) * rotationCorrection.y - vp.localAngularVel.y * Mathf.Pow(f, 2f) * 10f), (vp.rollInput * flipPower.z != 0f) ? 0f : ((0f - num2) * (1f - Mathf.Abs(num)) * rotationCorrection.z - vp.localAngularVel.z * Mathf.Pow(f, 2f) * 10f)), ForceMode.Acceleration);
	}

	private void Dive()
	{
		rb.AddTorque(velDir * Vector3.left * Mathf.Clamp01(vp.velMag * 0.01f) * Mathf.Clamp01(vp.upDot) * diveFactor, ForceMode.Acceleration);
	}
}
