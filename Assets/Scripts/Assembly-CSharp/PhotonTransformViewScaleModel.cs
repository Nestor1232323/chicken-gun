using System;

[Serializable]
public class PhotonTransformViewScaleModel
{
	public enum InterpolateOptions
	{
		Disabled = 0,
		MoveTowards = 1,
		Lerp = 2
	}

	public bool SynchronizeEnabled;

	public InterpolateOptions InterpolateOption;

	public float InterpolateMoveTowardsSpeed = 1f;

	public float InterpolateLerpSpeed;
}
