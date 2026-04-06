using System;
using UnityEngine;

[Serializable]
public class Stunt
{
	public string name;

	public Vector3 rotationAxis;

	[Range(0f, 1f)]
	public float precision = 0.8f;

	public float scoreRate;

	public float multiplier = 1f;

	public float angleThreshold;

	[NonSerialized]
	public float progress;

	public float boostAdd;

	public Stunt(Stunt oldStunt)
	{
		name = oldStunt.name;
		rotationAxis = oldStunt.rotationAxis;
		precision = oldStunt.precision;
		scoreRate = oldStunt.scoreRate;
		angleThreshold = oldStunt.angleThreshold;
		multiplier = oldStunt.multiplier;
		boostAdd = oldStunt.boostAdd;
	}
}
