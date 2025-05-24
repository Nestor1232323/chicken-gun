using System;
using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Drivetrain/Transmission/Continuous Transmission", 1)]
public class ContinuousTransmission : Transmission
{
	[Tooltip("Lerp value between min ratio and max ratio")]
	[Range(0f, 1f)]
	public float targetRatio;

	public float minRatio;

	public float maxRatio;

	[NonSerialized]
	public float currentRatio;

	public bool canReverse;

	[NonSerialized]
	public bool reversing;

	[Tooltip("How quickly the target ratio changes with manual shifting")]
	public float manualShiftRate = 0.5f;

	private void FixedUpdate()
	{
		health = Mathf.Clamp01(health);
		if (maxRPM == -1f)
		{
			maxRPM = targetDrive.curve.keys[targetDrive.curve.length - 1].time * 1000f;
		}
		if (health > 0f)
		{
			if (automatic && vp.groundedWheels > 0)
			{
				targetRatio = (1f - vp.burnout) * Mathf.Clamp01(Mathf.Abs(targetDrive.feedbackRPM) / Mathf.Max(0.01f, maxRPM * Mathf.Abs(currentRatio)));
			}
			else if (!automatic)
			{
				targetRatio = Mathf.Clamp01(targetRatio + (vp.upshiftHold - vp.downshiftHold) * manualShiftRate * Time.deltaTime);
			}
		}
		reversing = canReverse && vp.burnout == 0f && vp.localVelocity.z < 1f && (vp.accelInput < 0f || (vp.brakeIsReverse && vp.brakeInput > 0f));
		currentRatio = Mathf.Lerp(minRatio, maxRatio, targetRatio) * (float)((!reversing) ? 1 : (-1));
		newDrive.curve = targetDrive.curve;
		newDrive.rpm = targetDrive.rpm / currentRatio;
		newDrive.torque = Mathf.Abs(currentRatio) * targetDrive.torque;
		SetOutputDrives(currentRatio);
	}
}
