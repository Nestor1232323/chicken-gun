using System;
using System.Collections;
using Photon;
using UnityEngine;

public class BaseWeaponScript : Photon.MonoBehaviour
{
	[Serializable]
	public class GunInfo
	{
		[Serializable]
		public class GunParams
		{
			public float damage;

			public float damagePerSecond;

			public float firerate;

			public float accurancy;

			public int upgradePrice;
		}

		public int price;

		public bool isFreeItem;

		public string gunId;

		public BulletType bulletType;

		public Sprite gunIcon;

		[HideInInspector]
		public ShopItem myShopItem;

		[SerializeField]
		private GunParams[] gunParams;

		public Vector3 errorDelta;

		public string gunTitle;

		public float damage => gunParams[currentLevel].damage;

		public float damagePerSec => gunParams[currentLevel].damagePerSecond;

		public float firerate => gunParams[currentLevel].firerate;

		public float accurancy => gunParams[currentLevel].accurancy;

		public int maxUpgradeLevel => gunParams.Length;

		public int currentUpgradePrice => gunParams[currentLevel].upgradePrice;

		public int currentLevel
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public bool isFullyUpgraded => currentLevel == maxUpgradeLevel - 1;

		public void UpgradeSelf()
		{
			if (currentLevel < maxUpgradeLevel - 1)
			{
				currentLevel++;
			}
		}
	}

	public WeaponClass weaponClass;

	public WeaponMenuCategory weaponMenuCategory;

	public BulletType bulletType;

	public ShootBtnMode shootBtnMode;

	public Transform[] bulletPivots;

	public ShootType shootType;

	public bool showCrosshair = true;

	public bool hasSniperMode;

	public float animIndex;

	[HideInInspector]
	public bool isActiveBattleRoyale;

	public PhotonView parentPhotonView;

	public float shootDuration = 0.1f;

	public float autoshootAimDelay = 0.08f;

	public float pivotShoulderLengthFPS = 0.65f;

	public float pivotHeight = 1.5f;

	public float spineRotationOffset = -0.12f;

	[SerializeField]
	private int MAX_AMMO = 50;

	private int _ammo;

	[SerializeField]
	protected int _findedAmmoBonusCount = 6;

	public AudioSource shootSound;

	public AudioClip dryFireClip;

	public AudioClip reloadSound;

	protected bool isShooting;

	public bool isLoopSound;

	public bool isLoopShooting;

	public bool hasShootCycle;

	protected float shootStartTime;

	public Animator animator;

	public IShootable shootRPCDelegate;

	protected BaseBulletScript currentBullet;

	[SerializeField]
	private ParticleSystem[] muzzleFlashes;

	[SerializeField]
	private ParticleSystem shellFX;

	[SerializeField]
	private bool doNotOvveridePivotDir;

	public Transform fpsCameraPivot;

	public bool continueSootAfterReload = true;

	protected GunInfo _gunInfo;

	private CharacterMotor player;

	protected int shootedBulletsCount;

	public bool isReloading;

	private Quaternion rot;

	private Transform bulletPivot;

	private Vector3 locAngles;

	public float damage => gunInfo.damage;

	public int parentViewID => parentPhotonView.viewID;

	public string gunId => GetComponent<ShopItem>().id;

	public ShopItem myShopItem => gunInfo.myShopItem;

	public GunInfo gunInfo
	{
		get
		{
			if (_gunInfo == null)
			{
				_gunInfo = DataModel.instance.GetGunConfig(GetComponent<ShopItem>().id);
				_gunInfo.myShopItem = GetComponent<ShopItem>();
				bulletType = _gunInfo.bulletType;
				_gunInfo.myShopItem.price = _gunInfo.price;
				_gunInfo.myShopItem.icon = _gunInfo.gunIcon;
				_gunInfo.myShopItem.isFreeGood = gunInfo.isFreeItem;
			}
			return _gunInfo;
		}
		set
		{
			_gunInfo = value;
		}
	}

	public int ammo
	{
		get
		{
			return _ammo;
		}
		set
		{
			_ammo = value;
		}
	}

	protected virtual void Awake()
	{
		player = base.gameObject.GetComponentInParent<CharacterMotor>();
		if (player != null)
		{
			parentPhotonView = player.photonView;
		}
		ammo = MAX_AMMO;
	}

	public virtual void Show(bool val)
	{
		base.gameObject.SetActive(val);
	}

	private void CustomizeCamFollows()
	{
		if (player == null || !player.photonView.isMine)
		{
			return;
		}
		player.camFollowSettings = (TPSCamSettings[])player.defaultCamFollowSettings.Clone();
		if (GetComponent<TPSCamSettingsList>() == null)
		{
			CarOrPlayerSwitcher.instance.RefreshTPSCamFollowSettings(player.camFollowSettings);
			return;
		}
		TPSCamSettings[] camFollowSettings = GetComponent<TPSCamSettingsList>().camSettingsList;
		int i;
		for (i = 0; i < camFollowSettings.Length; i++)
		{
			if (camFollowSettings[i].normalDistanceFromPivotToCamera != 0f)
			{
				int num = -1;
				num = Array.FindIndex(player.camFollowSettings, (TPSCamSettings camFSet) => camFSet.mode == camFollowSettings[i].mode);
				if (num != -1)
				{
					player.camFollowSettings[num] = camFollowSettings[i];
				}
			}
		}
		CarOrPlayerSwitcher.instance.RefreshTPSCamFollowSettings(player.camFollowSettings);
	}

	protected virtual IEnumerator Start()
	{
		if (!(parentPhotonView == null))
		{
			if (parentPhotonView.isMine && GameWindow.instance != null)
			{
				GameWindow.instance.ShowCrosshair(showCrosshair);
			}
			if (parentPhotonView.isMine && GameWindow.instance != null)
			{
				GameWindow.instance.gameInputController.SetShootBtnMode(shootBtnMode);
				GameWindow.instance.gameInputController.ShowSniperModeBtn(hasSniperMode);
			}
			shootSound = GetComponent<AudioSource>();
			if (animator == null)
			{
				animator = GetComponent<Animator>();
			}
			if (!(GameController.instance == null))
			{
				shootSound = GetComponent<AudioSource>();
				UpdateAmmoUI();
				shootSound.loop = isLoopSound;
			}
		}
		yield break;
	}

	protected virtual void OnEnable()
	{
		isReloading = false;
		if (parentPhotonView != null && parentPhotonView.isMine)
		{
			UpdateAmmoUI();
			UpdateIconUI();
			if (GameWindow.instance != null)
			{
				GameWindow.instance.gameInputController.SetShootBtnMode(shootBtnMode);
				GameWindow.instance.gameInputController.ShowSniperModeBtn(hasSniperMode);
				GameWindow.instance.ShowCrosshair(showCrosshair);
			}
			CustomizeCamFollows();
		}
	}

	private void OnDisable()
	{
		if (shootSound != null)
		{
			shootSound.Stop();
		}
	}

	public virtual void StartShooting(float holdStrength = 0f)
	{
		if (shootSound != null && isLoopSound && ammo > 0)
		{
			shootSound.loop = isLoopSound;
			shootSound.Play();
			PlayMuzzleFlashes(play: true);
			PlayShellFX(play: true);
		}
		if ((bool)animator && ammo > 0)
		{
			animator.SetBool("Shoot", value: true);
			animator.Play("Shoot");
		}
	}

	public virtual void StopShooting()
	{
		isShooting = false;
		if (shootSound != null && shootSound.loop)
		{
			if (isReloading)
			{
				shootSound.loop = false;
			}
			else
			{
				shootSound.Stop();
			}
		}
		if ((bool)animator)
		{
			animator.SetBool("Shoot", value: false);
		}
		if (isLoopSound)
		{
			PlayMuzzleFlashes(play: false);
			PlayShellFX(play: false);
		}
	}

	public void StopReloading()
	{
		isReloading = false;
	}

	public virtual void PushBullet()
	{
		if (ammo == 0)
		{
			Reload();
		}
		else if (ammo > 0)
		{
			if (!isLoopSound)
			{
				PlayMuzzleFlashes(play: true);
				PlayShellFX(play: false);
				PlayShellFX(play: true);
			}
			if (isLoopSound)
			{
				shootStartTime = Time.time;
			}
			else
			{
				shootSound.PlayOneShot(shootSound.clip);
			}
			if (shootType == ShootType.SingleOne)
			{
				CreateBullet();
			}
			else if (shootType == ShootType.AllAtOne)
			{
				for (int i = 0; i < bulletPivots.Length; i++)
				{
					CreateBullet(i);
				}
			}
			else if (shootType == ShootType.ByQueue)
			{
				CreateBullet(shootedBulletsCount % bulletPivots.Length);
			}
			if (parentPhotonView.isMine)
			{
				ammo--;
				GameWindow.instance.SetAmmo(ammo);
			}
		}
		else
		{
			shootSound.Stop();
			if (!shootSound.isPlaying)
			{
				shootSound.PlayOneShot(dryFireClip);
			}
		}
	}

	public void Reload()
	{
		if (ammo != MAX_AMMO)
		{
			isReloading = true;
			if (parentPhotonView.isMine)
			{
				PhotonNetwork.RPC(parentPhotonView, "ReloadR", PhotonTargets.All, false);
			}
		}
	}

	public void OnReload()
	{
		StopShooting();
	}

	public void OnReloadFinished()
	{
		isReloading = false;
		ammo = MAX_AMMO;
		if (parentPhotonView.isMine)
		{
			GameWindow.instance.SetAmmo(ammo);
		}
		if (player.characterAnimation.IsInAttackMode() && continueSootAfterReload)
		{
			StartShooting();
		}
	}

	public void OnShootCycleEnded()
	{
		if (parentPhotonView.isMine && ammo == 0)
		{
			Reload();
		}
	}

	protected virtual void CreateBullet(int bulletPivotIndex = 0)
	{
		shootedBulletsCount++;
		isShooting = true;
		bulletPivot = bulletPivots[bulletPivotIndex];
		if (parentPhotonView.isMine)
		{
			if (doNotOvveridePivotDir)
			{
				rot = bulletPivots[bulletPivotIndex].rotation;
			}
			else
			{
				rot = Quaternion.LookRotation(GameController.instance.OurPlayer.playerWeaponManager.bulletTargetPoint - bulletPivot.position);
			}
		}
		else
		{
			rot = Quaternion.LookRotation(bulletPivot.forward);
		}
		currentBullet = WeaponsPoolManager.instance.Shoot(bulletType, bulletPivot.position, rot, parentPhotonView, parentPhotonView.isMine);
		locAngles.x = currentBullet.transform.localEulerAngles.x + gunInfo.errorDelta.x * (UnityEngine.Random.value - 0.5f);
		locAngles.y = currentBullet.transform.localEulerAngles.y + gunInfo.errorDelta.y * (UnityEngine.Random.value - 0.5f);
		locAngles.z = currentBullet.transform.localEulerAngles.z;
		currentBullet.transform.localEulerAngles = locAngles;
		currentBullet.SetDamage(damage);
	}

	public virtual void ShootFromRPC(Vector3 pos, Vector3 rot, float timeSince)
	{
		currentBullet = WeaponsPoolManager.instance.Shoot(bulletType, pos, Quaternion.Euler(rot), parentPhotonView, parentPhotonView.isMine);
		if (shootSound != null && !shootSound.isPlaying)
		{
			shootSound.Play();
		}
		currentBullet.SetDamage(damage);
		ThrowCartrige();
		if (parentPhotonView.isMine)
		{
			GameWindow.instance.SetAmmo(ammo);
		}
	}

	public virtual void ThrowFromRPC(Vector3 pos, Vector3 rot, float strength)
	{
	}

	public virtual bool OnFindAmmo()
	{
		ammo += _findedAmmoBonusCount;
		return true;
	}

	private void OnDestroy()
	{
	}

	protected void UpdateAmmoUI()
	{
		if (parentPhotonView.isMine && GameWindow.instance != null)
		{
			GameWindow.instance.SetAmmo(ammo);
		}
		if (GameController.instance != null && GameController.instance.OurPlayer != null && GameController.instance.OurPlayer.photonView.viewID == parentViewID)
		{
			WeaponSelectionWidget.instance.SetAmmoCount(ammo);
		}
	}

	protected void UpdateIconUI()
	{
		if (GameController.instance != null && GameController.instance.OurPlayer != null && GameController.instance.OurPlayer.photonView.viewID == parentViewID)
		{
			WeaponSelectionWidget.instance.SetWeaponIcon(myShopItem.icon);
		}
	}

	protected void ThrowCartrige()
	{
	}

	public void PlayShootSound()
	{
	}

	public void OnShootLoopEnd()
	{
	}

	public void PlayReloadSound()
	{
		if (reloadSound != null)
		{
			shootSound.PlayOneShot(reloadSound);
		}
	}

	private void PlayShellFX(bool play)
	{
		if (shellFX != null)
		{
			if (play)
			{
				shellFX.Play();
				return;
			}
			shellFX.Clear();
			shellFX.Stop();
		}
	}

	private void PlayMuzzleFlashes(bool play)
	{
		ParticleSystem[] array = muzzleFlashes;
		foreach (ParticleSystem particleSystem in array)
		{
			if (play)
			{
				particleSystem.Play();
			}
			else
			{
				particleSystem.Stop();
			}
		}
	}

	public void OnPlyerThrowGrenade()
	{
		shootSound.Stop();
		isReloading = false;
	}

	public void ResetAmmoCount()
	{
		ammo = MAX_AMMO;
		UpdateAmmoUI();
	}
}
