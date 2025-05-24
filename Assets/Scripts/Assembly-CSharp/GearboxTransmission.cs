using System;
using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Drivetrain/Transmission/Gearbox Transmission", 0)]
public class GearboxTransmission : Transmission
{
	public Gear[] gears;

	public int startGear;

	[NonSerialized]
	public int currentGear;

	private int firstGear;

	[NonSerialized]
	public float curGearRatio;

	public bool skipNeutral;

	[Tooltip("Calculate the RPM ranges of the gears in play mode.  This will overwrite the current values")]
	public bool autoCalculateRpmRanges = true;

	[Tooltip("Number of physics steps a shift should last")]
	public float shiftDelay;

	[NonSerialized]
	public float shiftTime;

	private Gear upperGear;

	private Gear lowerGear;

	private float upshiftDifference;

	private float downshiftDifference;

	[Tooltip("Multiplier for comparisons in automatic shifting calculations, should be 2 in most cases")]
	public float shiftThreshold;

	public override void Start()
	{
		base.Start();
		currentGear = Mathf.Clamp(startGear, 0, gears.Length - 1);
		GetFirstGear();
	}

	private void Update()
	{
		if (!automatic)
		{
			if (vp.upshiftPressed && currentGear < gears.Length - 1)
			{
				Shift(1);
			}
			if (vp.downshiftPressed && currentGear > 0)
			{
				Shift(-1);
			}
		}
	}

	private void FixedUpdate()
	{
		health = Mathf.Clamp01(health);
		shiftTime = Mathf.Max(0f, shiftTime - Time.timeScale * TimeMaster.inverseFixedTimeFactor);
		curGearRatio = gears[currentGear].ratio;
		float num = targetDrive.feedbackRPM / Mathf.Abs(curGearRatio);
		int num2 = 1;
		int num3 = 1;
		while ((skipNeutral || automatic) && gears[Mathf.Clamp(currentGear + num2, 0, gears.Length - 1)].ratio == 0f && currentGear + num2 != 0 && currentGear + num2 != gears.Length - 1)
		{
			num2++;
		}
		while ((skipNeutral || automatic) && gears[Mathf.Clamp(currentGear - num3, 0, gears.Length - 1)].ratio == 0f && currentGear - num3 != 0 && currentGear - num3 != 0)
		{
			num3++;
		}
		upperGear = gears[Mathf.Min(gears.Length - 1, currentGear + num2)];
		lowerGear = gears[Mathf.Max(0, currentGear - num3)];
		if (maxRPM == -1f)
		{
			maxRPM = targetDrive.curve.keys[targetDrive.curve.length - 1].time;
			if (autoCalculateRpmRanges)
			{
				CalculateRpmRanges();
			}
		}
		newDrive.curve = targetDrive.curve;
		if (curGearRatio == 0f || shiftTime > 0f)
		{
			newDrive.rpm = 0f;
			newDrive.torque = 0f;
		}
		else
		{
			newDrive.rpm = ((!automatic || !skidSteerDrive) ? targetDrive.rpm : (Mathf.Abs(targetDrive.rpm) * Mathf.Sign(vp.accelInput - ((!vp.brakeIsReverse) ? 0f : (vp.brakeInput * (1f - vp.burnout)))))) / curGearRatio;
			newDrive.torque = Mathf.Abs(curGearRatio) * targetDrive.torque;
		}
		upshiftDifference = gears[currentGear].maxRPM - upperGear.minRPM;
		downshiftDifference = lowerGear.maxRPM - gears[currentGear].minRPM;
		if (automatic && shiftTime == 0f && vp.groundedWheels > 0)
		{
			if (!skidSteerDrive && vp.burnout == 0f)
			{
				if (Mathf.Abs(vp.localVelocity.z) > 1f || vp.accelInput > 0f || (vp.brakeInput > 0f && vp.brakeIsReverse))
				{
					if (currentGear < gears.Length - 1 && (upperGear.minRPM + upshiftDifference * ((!(curGearRatio < 0f)) ? shiftThreshold : Mathf.Min(1f, shiftThreshold)) - num <= 0f || (curGearRatio <= 0f && upperGear.ratio > 0f && (!vp.reversing || (vp.accelInput > 0f && vp.localVelocity.z > curGearRatio * 10f)))) && (!(vp.brakeInput > 0f) || !vp.brakeIsReverse || !(upperGear.ratio >= 0f)) && (!(vp.localVelocity.z < 0f) || vp.accelInput != 0f))
					{
						Shift(1);
					}
					else if (currentGear > 0 && (num - (lowerGear.maxRPM - downshiftDifference * shiftThreshold) <= 0f || (curGearRatio >= 0f && lowerGear.ratio < 0f && (vp.reversing || ((vp.accelInput < 0f || (vp.brakeInput > 0f && vp.brakeIsReverse)) && vp.localVelocity.z < curGearRatio * 10f)))) && (!(vp.accelInput > 0f) || !(lowerGear.ratio <= 0f)) && (lowerGear.ratio > 0f || vp.localVelocity.z < 1f))
					{
						Shift(-1);
					}
				}
			}
			else if (currentGear != firstGear)
			{
				ShiftToGear(firstGear);
			}
		}
		SetOutputDrives(curGearRatio);
	}

	public void Shift(int dir)
	{
		if (health > 0f)
		{
			shiftTime = shiftDelay;
			currentGear += dir;
			while ((skipNeutral || automatic) && gears[Mathf.Clamp(currentGear, 0, gears.Length - 1)].ratio == 0f && currentGear != 0 && currentGear != gears.Length - 1)
			{
				currentGear += dir;
			}
			currentGear = Mathf.Clamp(currentGear, 0, gears.Length - 1);
		}
	}

	public void ShiftToGear(int gear)
	{
		if (health > 0f)
		{
			shiftTime = shiftDelay;
			currentGear = Mathf.Clamp(gear, 0, gears.Length - 1);
		}
	}

	public void CalculateRpmRanges()
	{
		bool flag = false;
		if (!Application.isPlaying)
		{
			GasMotor componentInChildren = F.GetTopmostParentComponent<VehicleParent>(base.transform).GetComponentInChildren<GasMotor>();
			if ((bool)componentInChildren)
			{
				maxRPM = componentInChildren.torqueCurve.keys[componentInChildren.torqueCurve.length - 1].time;
			}
			else
			{
				Debug.LogError("There is no <GasMotor> in the vehicle to get RPM info from.", this);
				flag = true;
			}
		}
		if (flag)
		{
			return;
		}
		float num = maxRPM * 1000f;
		for (int i = 0; i < gears.Length; i++)
		{
			float ratio = gears[Mathf.Max(i - 1, 0)].ratio;
			float ratio2 = gears[Mathf.Min(i + 1, gears.Length - 1)].ratio;
			if (gears[i].ratio < 0f)
			{
				gears[i].minRPM = num / gears[i].ratio;
				if (ratio2 == 0f)
				{
					gears[i].maxRPM = 0f;
				}
				else
				{
					gears[i].maxRPM = num / ratio2 + (num / ratio2 - gears[i].minRPM) * 0.5f;
				}
			}
			else if (gears[i].ratio > 0f)
			{
				gears[i].maxRPM = num / gears[i].ratio;
				if (ratio == 0f)
				{
					gears[i].minRPM = 0f;
				}
				else
				{
					gears[i].minRPM = num / ratio - (gears[i].maxRPM - num / ratio) * 0.5f;
				}
			}
			else
			{
				gears[i].minRPM = 0f;
				gears[i].maxRPM = 0f;
			}
			gears[i].minRPM *= 0.5f;
			gears[i].maxRPM *= 0.5f;
		}
	}

	public void GetFirstGear()
	{
		for (int i = 0; i < gears.Length; i++)
		{
			if (gears[i].ratio == 0f)
			{
				firstGear = i + 1;
				break;
			}
		}
	}
}
