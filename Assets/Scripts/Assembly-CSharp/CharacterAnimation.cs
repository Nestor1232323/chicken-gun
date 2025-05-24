using System;
using Photon;
using UnityEngine;

public class CharacterAnimation : Photon.MonoBehaviour
{
	public CharacterMotor characterMotor;

	public Animator animator;

	[SerializeField]
	public Transform topBody;

	private float bufSpeedK1;

	private float bufSpeedK2;

	public GameObject grenade;

	private void Awake()
	{
		PlayerWeaponManager playerWeaponManager = characterMotor.playerWeaponManager;
		playerWeaponManager.WeaponSwitched = (Action)Delegate.Combine(playerWeaponManager.WeaponSwitched, new Action(OnWeaponSwitched));
	}

	private void Start()
	{
	}

	private void OnDestroy()
	{
		PlayerWeaponManager playerWeaponManager = characterMotor.playerWeaponManager;
		playerWeaponManager.WeaponSwitched = (Action)Delegate.Remove(playerWeaponManager.WeaponSwitched, new Action(OnWeaponSwitched));
	}

	private void Update()
	{
		UpdateAtimatorParams();
	}

	private void UpdateAtimatorParams()
	{
		bufSpeedK2 = bufSpeedK1;
		bufSpeedK1 = characterMotor.GetRunDir();
		if (bufSpeedK2 != bufSpeedK1)
		{
			if (bufSpeedK1 == 0.5f)
			{
				characterMotor.footstepsAudio.loop = false;
			}
			else
			{
				characterMotor.footstepsAudio.loop = true;
				characterMotor.footstepsAudio.Play();
			}
			if (animator.isActiveAndEnabled)
			{
				animator.SetFloat("Speed_f", bufSpeedK1);
			}
		}
	}

	public void SetSpeedK(float val)
	{
		animator.SetFloat("Speed_f", val);
	}

	public void Shoot()
	{
		animator.Play("TopMovement.Attack");
		animator.SetBool("IsAttack", value: true);
	}

	public void UnShoot()
	{
		if (characterMotor.playerWeaponManager.CurrentWeapon == null)
		{
			return;
		}
		if (characterMotor.playerWeaponManager.CurrentWeapon.isReloading)
		{
			animator.SetBool("IsAttack", value: false);
		}
		else if (characterMotor.playerWeaponManager.CurrentWeapon.isLoopShooting)
		{
			if (!characterMotor.playerWeaponManager.CurrentWeapon.hasShootCycle)
			{
				animator.Play("TopMovement.Idle");
				animator.SetBool("IsAttack", value: false);
			}
			else
			{
				animator.SetBool("IsAttack", value: false);
			}
		}
	}

	public void ShootCycleEnded()
	{
		if (GameInputController.instance.autoshootEnabled)
		{
			if (!characterMotor.isNowShooting)
			{
				animator.Play("TopMovement.Idle");
				animator.SetBool("IsAttack", value: false);
			}
		}
		else
		{
			if (characterMotor.playerWeaponManager.CurrentWeapon.hasShootCycle && !characterMotor.playerWeaponManager.CurrentWeapon.isLoopShooting)
			{
				animator.Play("TopMovement.Idle");
			}
			animator.SetBool("IsAttack", value: false);
		}
	}

	public void OnDie()
	{
		animator.SetBool("IsAttack", value: false);
		animator.Play("Death.Death");
		animator.transform.SetLocalPositionY(0f);
		animator.SetBool("Death_b", value: true);
		BoneRotation[] componentsInChildren = base.transform.GetComponentsInChildren<BoneRotation>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = false;
		}
		SpineRotation[] componentsInChildren2 = base.transform.GetComponentsInChildren<SpineRotation>();
		for (int i = 0; i < componentsInChildren2.Length; i++)
		{
			componentsInChildren2[i].enabled = false;
		}
	}

	public void OnRespawn()
	{
		BoneRotation[] componentsInChildren = base.transform.GetComponentsInChildren<BoneRotation>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = true;
		}
		if (!base.photonView.isMine)
		{
			SpineRotation[] componentsInChildren2 = base.transform.GetComponentsInChildren<SpineRotation>();
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				componentsInChildren2[i].enabled = true;
			}
		}
		animator.SetBool("Death_b", value: false);
		animator.SetBool("IsAttack", value: false);
		animator.Play("Death.Alive");
	}

	public void Jump()
	{
		if (base.photonView.isMine)
		{
			JumpRPC();
			PhotonNetwork.RPC(base.photonView, "JumpRPC", PhotonTargets.Others, false);
		}
	}

	[PunRPC]
	public void JumpRPC()
	{
	}

	public void OnStartFlyingOnJetpack()
	{
		if (base.photonView.isMine)
		{
			OnStartFlyingOnJetpackRPC();
			PhotonNetwork.RPC(base.photonView, "OnStartFlyingOnJetpackRPC", PhotonTargets.Others, false);
		}
	}

	[PunRPC]
	public void OnStartFlyingOnJetpackRPC()
	{
	}

	public void OnGround()
	{
	}

	[PunRPC]
	public void OnGroundRPC()
	{
	}

	private void OnWeaponSwitched()
	{
		animator.SetFloat("WeaponIndex", characterMotor.playerWeaponManager.WeaponIndexForAnimator());
	}

	public void SetSpineEulers(float eulers)
	{
		topBody.SetLocalEulerX(eulers);
	}

	public void SetGunIndex(float index)
	{
		animator.SetFloat("WeaponIndex", index);
	}

	public void ThrowGrenade()
	{
		animator.Play("GrenadeThrow");
		if (grenade != null)
		{
			grenade.SetActive(value: true);
		}
	}

	public void PlayMenuState()
	{
	}

	[PunRPC]
	public void ReloadR()
	{
		if (IsInAttackMode())
		{
			animator.SetBool("IsAttack", characterMotor.playerWeaponManager.CurrentWeapon.continueSootAfterReload);
		}
		if ((bool)characterMotor.playerWeaponManager.CurrentWeapon)
		{
			characterMotor.playerWeaponManager.CurrentWeapon.OnReload();
		}
		animator.Play("TopMovement.Reload");
		_ = characterMotor.isNowShooting;
	}

	public bool IsInAttackMode()
	{
		return animator.GetBool("IsAttack");
	}

	public void EnterPauseMode()
	{
		animator.Play("TopMovement.Idle");
		animator.SetBool("IsAttack", value: false);
	}
}
