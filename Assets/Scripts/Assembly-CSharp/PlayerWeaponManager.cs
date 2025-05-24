using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class PlayerWeaponManager : Photon.MonoBehaviour
{
	[HideInInspector]
	public BaseWeaponScript CurrentWeapon;

	[HideInInspector]
	public BaseWeaponScript gun;

	[HideInInspector]
	public int CurrentWeaponIndex;

	public Action WeaponSwitched;

	private List<BaseWeaponScript> BoughtWeapons;

	[HideInInspector]
	public Vector3 bulletTargetPoint;

	[SerializeField]
	private Transform gunsContainer;

	[HideInInspector]
	public int grenadesCount;

	public int smokeGrenadesCount;

	public int molotovGrenadesCount;

	[SerializeField]
	private int MAX_GRENADES;

	[SerializeField]
	private int MAX_SMOKE_GRENADES;

	[SerializeField]
	private int MAX_MOLOTOV_GRENADES;

	private List<BaseWeaponScript> buffedredGuns = new List<BaseWeaponScript>();

	[SerializeField]
	private string defaultBattleRoyaleGunId;

	private string SelectedGunId
	{
		get
		{
			if (string.IsNullOrEmpty(StorageController.instance.GetPlayerSelectedGun(GetComponent<ShopItem>().id)))
			{
				StorageController.instance.SetPlayerSelectedGun(GetComponent<ShopItem>().id, AllPlayerGuns()[0].GetComponent<ShopItem>().id);
			}
			return StorageController.instance.GetPlayerSelectedGun(GetComponent<ShopItem>().id);
		}
	}

	private void Awake()
	{
		BoughtWeapons = new List<BaseWeaponScript>();
		buffedredGuns.AddRange(gunsContainer.GetComponentsInChildren<BaseWeaponScript>(includeInactive: true));
		BoughtWeapons.AddRange(buffedredGuns);
		if (!base.photonView.isMine)
		{
			return;
		}
		if (MultiplayerController.gameType == GameMode.BattleRoyalePvP || MultiplayerController.gameType == GameMode.BattleRoyaleTeams)
		{
			BoughtWeapons.Find((BaseWeaponScript w) => w.gunId == defaultBattleRoyaleGunId).isActiveBattleRoyale = true;
			SelectWeapon(defaultBattleRoyaleGunId);
		}
		else
		{
			SelectWeapon(SelectedGunId);
		}
	}

	private void Start()
	{
		if (!base.photonView.isMine)
		{
			return;
		}
		if (WeaponSwitched != null)
		{
			WeaponSwitched();
		}
		foreach (BaseWeaponScript boughtWeapon in BoughtWeapons)
		{
			StorageController.instance.SetTempWeaponAmmo(boughtWeapon.GetComponent<ShopItem>().id, boughtWeapon.ammo);
		}
		grenadesCount = MAX_GRENADES;
		GameWindow.instance.SetGrenades(grenadesCount, smokeGrenadesCount, molotovGrenadesCount);
	}

	public void AddWeapon(string gunId)
	{
		BaseWeaponScript baseWeaponScript = buffedredGuns.Find((BaseWeaponScript g) => g.gunId == gunId);
		if (baseWeaponScript != null)
		{
			baseWeaponScript.isActiveBattleRoyale = true;
		}
		if (base.photonView.isMine)
		{
			WeaponSelectionWidget.instance.UpdateGunsList();
		}
	}

	public void SelectWeapon(string weaponID)
	{
		int num = AllPlayerGuns().FindIndex((BaseWeaponScript g) => g.GetComponent<ShopItem>().id == weaponID);
		if (num != -1)
		{
			SelectWeapon(num);
		}
	}

	private void SelectWeapon(int weaponID)
	{
		if (gun != null)
		{
			gun.Show(val: false);
		}
		PhotonNetwork.RPC(base.photonView, "CreateWeaponRPC", PhotonTargets.All, false, (byte)weaponID);
	}

	[PunRPC]
	public void CreateWeaponRPC(byte weaponId)
	{
		if (base.photonView.isMine)
		{
			CurrentWeaponIndex = weaponId;
		}
		if ((bool)gun)
		{
			gun.Show(val: false);
		}
		gun = BoughtWeapons[weaponId];
		gun.Show(val: true);
		CurrentWeapon = gun.GetComponent<BaseWeaponScript>();
		CurrentWeapon.parentPhotonView = base.photonView;
		if (base.photonView.isMine && (bool)GameWindow.instance)
		{
			GameWindow.instance.gameInputController.SetShootBtnMode(CurrentWeapon.shootBtnMode);
		}
		if (WeaponSwitched != null)
		{
			WeaponSwitched();
		}
	}

	public void SelectNextWeapon(int sign)
	{
		CurrentWeaponIndex += sign;
		if (CurrentWeaponIndex < 0)
		{
			CurrentWeaponIndex = BoughtWeapons.Count - 1;
		}
		else if (CurrentWeaponIndex > BoughtWeapons.Count - 1)
		{
			CurrentWeaponIndex = 0;
		}
		SelectWeapon(CurrentWeaponIndex);
	}

	public void SelectWeaponById(int id)
	{
		CurrentWeaponIndex = id;
		SelectWeapon(CurrentWeaponIndex);
	}

	public void HideCurrentGun()
	{
		CurrentWeapon.StopShooting();
		CurrentWeapon.Show(val: false);
	}

	public void OnRespawn()
	{
		if (CurrentWeapon != null)
		{
			CurrentWeapon.gameObject.SetActive(value: true);
		}
		grenadesCount = MAX_GRENADES;
		smokeGrenadesCount = MAX_SMOKE_GRENADES;
		molotovGrenadesCount = MAX_MOLOTOV_GRENADES;
		if (base.photonView.isMine && GameWindow.instance != null)
		{
			GameWindow.instance.SetGrenades(grenadesCount, smokeGrenadesCount, molotovGrenadesCount);
		}
	}

	public bool FindedGrenade(int count, BulletType grenadeType)
	{
		switch (grenadeType)
		{
		case BulletType.Grenade:
			if (grenadesCount >= MAX_GRENADES)
			{
				return false;
			}
			grenadesCount += count;
			break;
		case BulletType.SmokeGrenade:
			if (smokeGrenadesCount >= MAX_SMOKE_GRENADES)
			{
				return false;
			}
			smokeGrenadesCount += count;
			break;
		case BulletType.MolotovGrenade:
			if (molotovGrenadesCount >= MAX_MOLOTOV_GRENADES)
			{
				return false;
			}
			molotovGrenadesCount += count;
			break;
		}
		if (base.photonView.isMine && GameWindow.instance != null)
		{
			GameWindow.instance.SetGrenades(grenadesCount, smokeGrenadesCount, molotovGrenadesCount);
		}
		return true;
	}

	public float WeaponIndexForAnimator()
	{
		return CurrentWeapon.animIndex;
	}

	public Transform FirstBulletPivot()
	{
		if (CurrentWeapon == null)
		{
			return base.transform;
		}
		if (CurrentWeapon.bulletPivots.Length == 0)
		{
			return CurrentWeapon.transform;
		}
		return CurrentWeapon.bulletPivots[0];
	}

	public void ApplyMeleeAttack()
	{
		if (base.photonView.isMine)
		{
			((BaseMeleeWeapon)CurrentWeapon).ApplyDamege();
		}
	}

	public List<BaseWeaponScript> AllPlayerGuns()
	{
		if (BoughtWeapons == null)
		{
			BoughtWeapons = new List<BaseWeaponScript>();
			BoughtWeapons.AddRange(gunsContainer.GetComponentsInChildren<BaseWeaponScript>(includeInactive: true));
		}
		return BoughtWeapons;
	}

	public List<BaseWeaponScript> AllPlayerBoughtGuns()
	{
		List<BaseWeaponScript> list = new List<BaseWeaponScript>();
		list.AddRange(gunsContainer.GetComponentsInChildren<BaseWeaponScript>(includeInactive: true));
		list.RemoveAll((BaseWeaponScript gun) => !gun.myShopItem.IsBought);
		return list;
	}

	public void EquipGun(string gunId)
	{
		int num = 0;
		foreach (BaseWeaponScript item in AllPlayerGuns())
		{
			if (item.GetComponent<ShopItem>().id == gunId)
			{
				item.Show(val: true);
				GetComponent<CharacterAnimation>().SetGunIndex(num);
				CurrentWeapon = item;
			}
			else
			{
				item.Show(val: false);
			}
			num++;
		}
	}

	public void EquipSelectedGun()
	{
		EquipGun(SelectedGunId);
	}

	public void HideCurrentGun(bool hide)
	{
		if (CurrentWeapon != null)
		{
			CurrentWeapon.Show(!hide);
		}
	}
}
