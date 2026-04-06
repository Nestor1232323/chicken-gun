using System;
using UnityEngine;

[Serializable]
public class PhotonTransformViewPositionModel
{
	public enum InterpolateOptions
	{
		Disabled = 0,
		FixedSpeed = 1,
		EstimatedSpeed = 2,
		SynchronizeValues = 3,
		Lerp = 4
	}

	public enum ExtrapolateOptions
	{
		Disabled = 0,
		SynchronizeValues = 1,
		EstimateSpeedAndTurn = 2,
		FixedSpeed = 3
	}

	public bool SynchronizeEnabled;

	public bool TeleportEnabled = true;

	public float TeleportIfDistanceGreaterThan = 3f;

	public InterpolateOptions InterpolateOption = InterpolateOptions.EstimatedSpeed;

	public float InterpolateMoveTowardsSpeed = 1f;

	public float InterpolateLerpSpeed = 1f;

	public float InterpolateMoveTowardsAcceleration = 2f;

	public float InterpolateMoveTowardsDeceleration = 2f;

	public AnimationCurve InterpolateSpeedCurve = new AnimationCurve(new Keyframe(-1f, 0f, 0f, float.PositiveInfinity), new Keyframe(0f, 1f, 0f, 0f), new Keyframe(1f, 1f, 0f, 1f), new Keyframe(4f, 4f, 1f, 0f));

	public ExtrapolateOptions ExtrapolateOption;

	public float ExtrapolateSpeed = 1f;

	public bool ExtrapolateIncludingRoundTripTime = true;

	public int ExtrapolateNumberOfStoredPositions = 1;

	public bool DrawErrorGizmo = true;
}
