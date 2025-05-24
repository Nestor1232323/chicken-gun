using System;

[Serializable]
public class TPSCamSettings
{
	public float normalDistanceFromPivotToCamera = 1f;

	public float maxTopAng = 75f;

	public float maxBottompAng = -85f;

	public float pivotShoulderLength = 0.9f;

	public float pivotHeight = 1.58f;

	public float pivotXAngle = 3.28f;

	public float camDistanceSpeed = 3f;

	public CamMode mode;
}
