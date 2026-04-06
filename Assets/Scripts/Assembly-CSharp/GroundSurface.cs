using System;
using UnityEngine;

[Serializable]
public class GroundSurface
{
	public string name = "Surface";

	public bool useColliderFriction;

	public float friction;

	[Tooltip("Always leave tire marks")]
	public bool alwaysScrape;

	[Tooltip("Rims leave sparks on this surface")]
	public bool leaveSparks;

	public AudioClip tireSnd;

	public AudioClip rimSnd;

	public AudioClip tireRimSnd;
}
