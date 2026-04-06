using UnityEngine;

public class SyncShootGun : BaseWeaponScript
{
	public virtual void PushBulletEvent()
	{
		if (parentPhotonView.isMine)
		{
			int num = 0;
			if (shootType == ShootType.SingleOne)
			{
				num = 0;
			}
			else if (shootType == ShootType.ByQueue)
			{
				num = shootedBulletsCount % bulletPivots.Length;
			}
			base.ammo--;
			shootRPCDelegate.PushBulletForRPC(bulletPivots[num].position, bulletPivots[num].eulerAngles, 0f);
			shootedBulletsCount++;
		}
	}

	public override void ShootFromRPC(Vector3 pos, Vector3 rot, float timeSince)
	{
		base.ShootFromRPC(pos, rot, timeSince);
	}
}
