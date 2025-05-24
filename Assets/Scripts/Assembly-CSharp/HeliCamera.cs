using UnityEngine;

public class HeliCamera : BaseCamera
{
	public Transform Target;

	public void SeTarget(Transform target)
	{
		Target = target;
		base.transform.SetParent(target);
		base.transform.localPosition = Vector3.zero;
		base.transform.localEulerAngles = Vector3.zero;
	}

	public override void Disable()
	{
		base.enabled = false;
	}
}
