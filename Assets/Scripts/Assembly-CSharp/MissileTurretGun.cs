using UnityEngine;

public class MissileTurretGun : SyncShootGun
{
	[SerializeField]
	protected GunInfo gunInfoOverrided;

	[SerializeField]
	protected bool useCamRaycastedBulletPoint;

	protected override void Awake()
	{
		base.Awake();
		_gunInfo = gunInfoOverrided;
	}

	public override void PushBulletEvent()
	{
		if (!parentPhotonView.isMine)
		{
			return;
		}
		int num = 0;
		if (shootType == ShootType.SingleOne)
		{
			num = 0;
			base.ammo--;
			CreateSyncBullet(num);
		}
		else if (shootType == ShootType.AllAtOne)
		{
			for (int i = 0; i < bulletPivots.Length; i++)
			{
				base.ammo--;
				CreateSyncBullet(i);
			}
		}
		else if (shootType == ShootType.ByQueue)
		{
			num = shootedBulletsCount % bulletPivots.Length;
			base.ammo--;
			CreateSyncBullet(num);
		}
	}

	protected virtual void CreateSyncBullet(int pivotIndex)
	{
		Vector3 eulerAngles = bulletPivots[pivotIndex].eulerAngles;
		if (useCamRaycastedBulletPoint)
		{
			Vector3 camLookBulletPoint = FPSCamera.instance.GetCamLookBulletPoint();
			eulerAngles = Quaternion.LookRotation(camLookBulletPoint - bulletPivots[pivotIndex].position, Vector3.up).eulerAngles;
		}
		shootRPCDelegate.PushBulletForRPC(bulletPivots[pivotIndex].position, eulerAngles, 0f);
		shootedBulletsCount++;
	}
}
