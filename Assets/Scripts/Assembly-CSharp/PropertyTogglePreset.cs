using System;
using UnityEngine;

[Serializable]
public class PropertyTogglePreset
{
	[Tooltip("Limit the steering range of wheels based on SteeringControl's curve?")]
	public bool limitSteer = true;

	[Tooltip("Transmission is adjusted for skid steering?")]
	public bool skidSteerTransmission;

	[Tooltip("Must be equal to the number of wheels")]
	public IndividualPreset[] wheels;
}
