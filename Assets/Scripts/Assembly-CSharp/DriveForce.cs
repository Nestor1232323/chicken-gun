using System;
using UnityEngine;

[AddComponentMenu("RVP/C#/Drivetrain/Drive Force", 3)]
public class DriveForce : MonoBehaviour
{
	[NonSerialized]
	public float rpm;

	[NonSerialized]
	public float torque;

	[NonSerialized]
	public AnimationCurve curve;

	[NonSerialized]
	public float feedbackRPM;

	[NonSerialized]
	public bool active = true;

	public void SetDrive(DriveForce from)
	{
		rpm = from.rpm;
		torque = from.torque;
		curve = from.curve;
	}

	public void SetDrive(DriveForce from, float torqueFactor)
	{
		rpm = from.rpm;
		torque = from.torque * torqueFactor;
		curve = from.curve;
	}
}
