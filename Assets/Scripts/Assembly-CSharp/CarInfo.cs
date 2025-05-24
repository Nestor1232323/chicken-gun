using System;
using UnityEngine;

[Serializable]
public class CarInfo : VehicleInfo
{
	public float power;

	[HideInInspector]
	public string playerName;

	public float wheelWidth;

	public WheelType wheelType;
}
