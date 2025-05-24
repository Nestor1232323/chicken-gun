using System;

[Serializable]
public class VehicleInfo
{
	public string id;

	public string name;

	public float maxSpeed;

	public float MaxSpeedSqr => maxSpeed * maxSpeed;
}
