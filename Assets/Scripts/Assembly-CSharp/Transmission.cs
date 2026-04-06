using System;
using UnityEngine;

[RequireComponent(typeof(DriveForce))]
public abstract class Transmission : MonoBehaviour
{
	[Range(0f, 1f)]
	public float strength = 1f;

	[NonSerialized]
	public float health = 1f;

	protected VehicleParent vp;

	protected DriveForce targetDrive;

	protected DriveForce newDrive;

	public bool automatic;

	[Tooltip("Apply special drive to wheels for skid steering")]
	public bool skidSteerDrive;

	public DriveForce[] outputDrives;

	[Tooltip("Exponent for torque output on each wheel")]
	public float driveDividePower = 3f;

	[NonSerialized]
	public float maxRPM = -1f;

	public virtual void Start()
	{
		vp = (VehicleParent)F.GetTopmostParentComponent<VehicleParent>(base.transform);
		targetDrive = GetComponent<DriveForce>();
		newDrive = base.gameObject.AddComponent<DriveForce>();
	}

	protected void SetOutputDrives(float ratio)
	{
		if (outputDrives.Length <= 0)
		{
			return;
		}
		int num = 0;
		DriveForce[] array = outputDrives;
		foreach (DriveForce driveForce in array)
		{
			if (driveForce.active)
			{
				num++;
			}
		}
		float torqueFactor = Mathf.Pow(1f / (float)num, driveDividePower);
		float num2 = 0f;
		DriveForce[] array2 = outputDrives;
		foreach (DriveForce driveForce2 in array2)
		{
			if (driveForce2.active)
			{
				num2 += ((!skidSteerDrive) ? driveForce2.feedbackRPM : Mathf.Abs(driveForce2.feedbackRPM));
				driveForce2.SetDrive(newDrive, torqueFactor);
			}
		}
		targetDrive.feedbackRPM = num2 / (float)num * ratio;
	}

	public void ResetMaxRPM()
	{
		maxRPM = -1f;
	}
}
