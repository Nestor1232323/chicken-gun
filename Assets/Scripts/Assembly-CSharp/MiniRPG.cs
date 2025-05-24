using UnityEngine;

public class MiniRPG : SyncShootGun
{
	public GameObject missileInBarrel;

	public override void PushBullet()
	{
		if (base.ammo == 0)
		{
			Reload();
			shootSound.Stop();
		}
		else if (parentPhotonView.isMine)
		{
			base.ammo--;
			isShooting = true;
			Quaternion quaternion = Quaternion.LookRotation(GameController.instance.OurPlayer.playerWeaponManager.bulletTargetPoint - bulletPivots[0].position);
			GameController.instance.OurPlayer.PushBulletForRPC(bulletPivots[0].position, quaternion.eulerAngles, 0f);
		}
	}

	public override void ShootFromRPC(Vector3 pos, Vector3 rot, float timeSince)
	{
		base.ShootFromRPC(pos, rot, timeSince);
		if (isLoopSound)
		{
			shootStartTime = Time.time;
		}
		else
		{
			shootSound.PlayOneShot(shootSound.clip);
		}
		ThrowCartrige();
		if (missileInBarrel != null)
		{
			missileInBarrel.SetActive(base.ammo > 0);
		}
	}
}
