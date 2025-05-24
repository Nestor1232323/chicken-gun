using UnityEngine;

public class PooledObject : MonoBehaviour
{
	public virtual void OnSpawn()
	{
	}

	public virtual void OnRecycle()
	{
		base.gameObject.Recycle();
	}
}
