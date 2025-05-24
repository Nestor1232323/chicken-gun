using UnityEngine;

public interface IIShootableThrowable : IShootable
{
	void ThrowBulletForRPC(Vector3 pos, Vector3 rot, float strength);
}
