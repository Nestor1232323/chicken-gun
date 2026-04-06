using UnityEngine;

public interface IShootable
{
	void StartShoot(float holdStrength = 0f);

	void StopShooting();

	void PushBulletForRPC(Vector3 pos, Vector3 rot, float timeSinceGameStart);
}
