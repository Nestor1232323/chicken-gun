using UnityEngine;

public class BaseCamera : MonoBehaviour
{
	public virtual void Enable()
	{
		base.enabled = true;
	}

	public virtual void Disable()
	{
	}

	public virtual void SetTarget(Transform target)
	{
	}

	public virtual void EnterSniperMode()
	{
	}
}
