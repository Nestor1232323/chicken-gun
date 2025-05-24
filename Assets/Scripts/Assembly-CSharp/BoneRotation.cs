using UnityEngine;

public class BoneRotation : MonoBehaviour
{
	[SerializeField]
	private Transform bone;

	[SerializeField]
	private Vector3 wantedRotation;

	public bool isLocal;

	private void LateUpdate()
	{
		if (!(bone == null))
		{
			if (isLocal)
			{
				bone.localEulerAngles = wantedRotation;
			}
			else
			{
				bone.eulerAngles = wantedRotation;
			}
		}
	}

	public void SetZAngle(float val)
	{
		wantedRotation.z = val;
	}
}
