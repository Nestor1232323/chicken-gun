using System;
using UnityEngine;

[RequireComponent(typeof(DriveForce))]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Drivetrain/Gas Motor", 0)]
public class GasMotor : Motor
{
	[Header("Performance")]
	[Tooltip("X-axis = RPM in thousands, y-axis = torque.  The rightmost key represents the maximum RPM")]
	public AnimationCurve torqueCurve = AnimationCurve.EaseInOut(0f, 0f, 8f, 1f);

	[Range(0f, 0.99f)]
	[Tooltip("How quickly the engine adjusts its RPMs")]
	public float inertia;

	[Tooltip("Can the engine turn backwards?")]
	public bool canReverse;

	private DriveForce targetDrive;

	[NonSerialized]
	public float maxRPM;

	public DriveForce[] outputDrives;

	[Tooltip("Exponent for torque output on each wheel")]
	public float driveDividePower = 3f;

	private float actualAccel;

	[Header("Transmission")]
	public GearboxTransmission transmission;

	[NonSerialized]
	public bool shifting;

	[Tooltip("Increase sound pitch between shifts")]
	public bool pitchIncreaseBetweenShift;

	public override void Start()
	{
		base.Start();
		targetDrive = GetComponent<DriveForce>();
		GetMaxRPM();
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		actualAccel = Mathf.Lerp((!vp.brakeIsReverse || !vp.reversing || !(vp.accelInput <= 0f)) ? vp.accelInput : vp.brakeInput, Mathf.Max(vp.accelInput, vp.burnout), vp.burnout);
		float f = ((!canReverse) ? Mathf.Clamp01(actualAccel) : actualAccel);
		actualInput = inputCurve.Evaluate(Mathf.Abs(f)) * Mathf.Sign(f);
		targetDrive.curve = torqueCurve;
		if (ignition)
		{
			float num = boostPowerCurve.Evaluate(Mathf.Abs(vp.localVelocity.z));
			targetDrive.rpm = Mathf.Lerp(targetDrive.rpm, actualInput * maxRPM * 1000f * ((!boosting) ? 1f : (1f + num)), (1f - inertia) * Time.timeScale);
			if (targetDrive.feedbackRPM > targetDrive.rpm)
			{
				targetDrive.torque = 0f;
			}
			else
			{
				targetDrive.torque = torqueCurve.Evaluate(targetDrive.feedbackRPM * 0.001f - ((!boosting) ? 0f : num)) * Mathf.Lerp(targetDrive.torque, power * (float)Mathf.Abs(Math.Sign(actualInput)), (1f - inertia) * Time.timeScale) * ((!boosting) ? 1f : (1f + num)) * health;
			}
			if (outputDrives.Length != 0)
			{
				float torqueFactor = Mathf.Pow(1f / (float)outputDrives.Length, driveDividePower);
				float num2 = 0f;
				DriveForce[] array = outputDrives;
				foreach (DriveForce driveForce in array)
				{
					num2 += driveForce.feedbackRPM;
					driveForce.SetDrive(targetDrive, torqueFactor);
				}
				targetDrive.feedbackRPM = num2 / (float)outputDrives.Length;
			}
			if ((bool)transmission)
			{
				shifting = transmission.shiftTime > 0f;
			}
			else
			{
				shifting = false;
			}
			return;
		}
		targetDrive.rpm = 0f;
		targetDrive.torque = 0f;
		targetDrive.feedbackRPM = 0f;
		shifting = false;
		if (outputDrives.Length != 0)
		{
			DriveForce[] array = outputDrives;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetDrive(targetDrive);
			}
		}
	}

	public override void Update()
	{
		if ((bool)snd && ignition)
		{
			airPitch = ((vp.groundedWheels <= 0 && actualAccel == 0f) ? Mathf.Lerp(airPitch, 0f, 0.5f * Time.deltaTime) : 1f);
			pitchFactor = ((actualAccel == 0f && vp.groundedWheels != 0) ? 0.5f : 1f) * ((!shifting) ? 1f : ((!pitchIncreaseBetweenShift) ? (Mathf.Min(transmission.shiftDelay, Mathf.Pow(transmission.shiftTime, 2f)) / transmission.shiftDelay) : Mathf.Sin(transmission.shiftTime / transmission.shiftDelay * (float)Math.PI))) * airPitch;
			targetPitch = Mathf.Abs(targetDrive.feedbackRPM * 0.001f / maxRPM) * pitchFactor;
		}
		base.Update();
	}

	public void GetMaxRPM()
	{
		maxRPM = torqueCurve.keys[torqueCurve.length - 1].time;
		if (outputDrives.Length == 0)
		{
			return;
		}
		DriveForce[] array = outputDrives;
		foreach (DriveForce driveForce in array)
		{
			driveForce.curve = targetDrive.curve;
			if ((bool)driveForce.GetComponent<Transmission>())
			{
				driveForce.GetComponent<Transmission>().ResetMaxRPM();
			}
		}
	}
}
