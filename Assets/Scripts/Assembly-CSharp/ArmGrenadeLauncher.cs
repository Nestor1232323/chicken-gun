using UnityEngine;

public class ArmGrenadeLauncher : SyncShootGun
{
	[SerializeField]
	private GameObject grenadeBulletPlaceholder;

	public float holdStrength;

	public override void StartShooting(float holdStrength = 0f)
	{
		base.StartShooting(holdStrength);
		this.holdStrength = holdStrength;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (grenadeBulletPlaceholder != null)
		{
			grenadeBulletPlaceholder.SetActive(base.ammo > 0);
		}
	}

	public override void PushBullet()
	{
		if (parentPhotonView.isMine)
		{
			if (base.ammo > 0)
			{
				isShooting = true;
				if (bulletPivots[0] != null)
				{
					Quaternion rotation = bulletPivots[0].rotation;
					GameController.instance.OurPlayer.ThrowBulletForRPC(bulletPivots[0].position, rotation.eulerAngles, holdStrength * 8f + 5f);
				}
				base.ammo--;
			}
			else
			{
				shootSound.Stop();
				if (!shootSound.isPlaying)
				{
					shootSound.PlayOneShot(dryFireClip);
				}
			}
			if (grenadeBulletPlaceholder != null)
			{
				grenadeBulletPlaceholder.SetActive(base.ammo > 0);
			}
		}
		if (parentPhotonView.isMine)
		{
			GameController.instance.OurPlayer.StopShooting();
		}
	}

	public override void ThrowFromRPC(Vector3 pos, Vector3 rot, float strength)
	{
		base.ThrowFromRPC(pos, rot, strength);
		if (!shootSound.isPlaying)
		{
			shootSound.Play();
		}
		BaseGrenade baseGrenade = WeaponsPoolManager.instance.CreateBullet<BaseGrenade>(bulletType, pos, Quaternion.Euler(rot), parentPhotonView, parentPhotonView.isMine);
		baseGrenade.parentView = parentPhotonView;
		baseGrenade.Throw(strength, 11f);
	}

	public override bool OnFindAmmo()
	{
		base.ammo += _findedAmmoBonusCount;
		if (grenadeBulletPlaceholder != null)
		{
			grenadeBulletPlaceholder.SetActive(value: true);
		}
		return true;
	}
}
