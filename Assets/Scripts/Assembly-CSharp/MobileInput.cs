using System;
using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Input/Mobile Input Setter", 1)]
public class MobileInput : MonoBehaviour
{
	public ScreenOrientation screenRot = ScreenOrientation.LandscapeLeft;

	[NonSerialized]
	public float accel;

	[NonSerialized]
	public float brake;

	[NonSerialized]
	public float steer;

	[NonSerialized]
	public float ebrake;

	[NonSerialized]
	public bool boost;

	private void Start()
	{
		Screen.autorotateToPortrait = screenRot == ScreenOrientation.Portrait || screenRot == ScreenOrientation.AutoRotation;
		Screen.autorotateToPortraitUpsideDown = screenRot == ScreenOrientation.PortraitUpsideDown || screenRot == ScreenOrientation.AutoRotation;
		Screen.autorotateToLandscapeRight = screenRot == ScreenOrientation.LandscapeRight || screenRot == ScreenOrientation.LandscapeLeft || screenRot == ScreenOrientation.AutoRotation;
		Screen.autorotateToLandscapeLeft = screenRot == ScreenOrientation.LandscapeLeft || screenRot == ScreenOrientation.LandscapeLeft || screenRot == ScreenOrientation.AutoRotation;
		Screen.orientation = screenRot;
	}

	public void SetAccel(float f)
	{
		accel = Mathf.Clamp01(f);
	}

	public void SetBrake(float f)
	{
		brake = Mathf.Clamp01(f);
	}

	public void SetSteer(float f)
	{
		steer = Mathf.Clamp(f, -1f, 1f);
	}

	public void SetEbrake(float f)
	{
		ebrake = Mathf.Clamp01(f);
	}

	public void SetBoost(bool b)
	{
		boost = b;
	}
}
