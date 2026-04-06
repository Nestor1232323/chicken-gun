using System;

[Serializable]
public class SuspensionToggledProperty
{
	public enum Properties
	{
		steerEnable = 0,
		steerInvert = 1,
		driveEnable = 2,
		driveInvert = 3,
		ebrakeEnable = 4,
		skidSteerBrake = 5
	}

	public Properties property;

	public bool toggled;
}
