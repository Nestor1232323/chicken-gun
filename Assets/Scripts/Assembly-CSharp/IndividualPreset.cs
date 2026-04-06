using System;
using UnityEngine;

[Serializable]
public class IndividualPreset
{
	[Tooltip("Must be equal to the SuspensionPropertyToggle properties array length")]
	public bool[] preset;
}
