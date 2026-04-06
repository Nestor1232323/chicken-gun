using UnityEngine;

public class SpineRotation : MonoBehaviour
{
	[SerializeField]
	private Transform bone;

	[SerializeField]
	private Transform parentPivot;

	[SerializeField]
	private Transform spineRef;

	public Vector3 e;

	private void LateUpdate()
	{
	}

	public void SetSpineAngle(float val)
	{
	}

	public void SetEulers(Vector3 eulers)
	{
		e = eulers;
	}
}
