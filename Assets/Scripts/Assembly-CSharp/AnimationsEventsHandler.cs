using UnityEngine;

public class AnimationsEventsHandler : MonoBehaviour
{
	public CharacterMotor characterMotor;

	public GameObject grenade;

	public void ShootEvent()
	{
		characterMotor.PushBullet();
	}

	public void MeleeAttackEvent()
	{
		characterMotor.ApplyMeleeAttack();
	}

	public void TwrowGrenadeEvent()
	{
		if ((bool)grenade)
		{
			grenade.SetActive(value: false);
		}
		characterMotor.ThrowGrenadeAnimEvent();
	}

	public void TwrowGrenadeEventEnd()
	{
		if ((bool)grenade)
		{
			grenade.SetActive(value: false);
		}
		characterMotor.ThrowGrenadeAnimEventEnd();
	}

	public void ShootCycleEnded()
	{
		characterMotor.characterAnimation.ShootCycleEnded();
		characterMotor.playerWeaponManager.CurrentWeapon.OnShootCycleEnded();
	}

	public void PlayShootSound()
	{
		characterMotor.playerWeaponManager.CurrentWeapon.PlayShootSound();
	}

	public void PlayReloadSound()
	{
		if (characterMotor.playerWeaponManager.CurrentWeapon != null)
		{
			characterMotor.playerWeaponManager.CurrentWeapon.PlayReloadSound();
		}
	}

	public void HideCurrentGun()
	{
	}

	public void OnReloadFinished()
	{
		if (characterMotor.playerWeaponManager.CurrentWeapon != null)
		{
			characterMotor.playerWeaponManager.CurrentWeapon.OnReloadFinished();
		}
	}

	public void ShowArrow()
	{
		((Bow)characterMotor.playerWeaponManager.CurrentWeapon).ShowArrow();
	}

	public void HideArrow()
	{
		((Bow)characterMotor.playerWeaponManager.CurrentWeapon).HideArrow();
	}
}
