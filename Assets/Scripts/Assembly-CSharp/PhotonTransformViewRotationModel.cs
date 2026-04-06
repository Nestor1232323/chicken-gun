using System;

[Serializable]
public class PhotonTransformViewRotationModel
{
	public enum InterpolateOptions
	{
		Disabled = 0,
		RotateTowards = 1,
		Lerp = 2
	}

	public bool SynchronizeEnabled;

	public InterpolateOptions InterpolateOption = InterpolateOptions.RotateTowards;

	public float InterpolateRotateTowardsSpeed = 180f;

	public float InterpolateLerpSpeed = 5f;
}
