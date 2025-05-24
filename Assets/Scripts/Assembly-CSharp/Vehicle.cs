using System.Collections;
using Photon;
using UnityEngine;

[RequireComponent(typeof(DamageReciver2))]
public class Vehicle : Photon.MonoBehaviour, IDestroyable, IPlayerSittableVehicle
{
	protected float _hp;

	protected SmoothMove smoothMove;

	public bool isClone;

	[HideInInspector]
	public DamageReciver2 damageReciver;

	public CharacterMotor myDriver;

	public bool isBusyByPlayer;

	public VehicleType vehicleType;

	public float maxHP;

	public Transform PlayerPlacePivot;

	public int expPoints = 10;

	public BaseWeaponScript gunController;

	public Transform hudPivot;

	protected bool crashed;

	public float hp => _hp;

	public bool Alive => _hp > 0f;

	public bool IsMine => base.photonView.isMine;

	public bool AreFreeForSitting
	{
		get
		{
			isBusyByPlayer = !IsPlayerPlaceEmpty();
			if (!Alive)
			{
				return false;
			}
			if (!IsPlayerPlaceEmpty())
			{
				return false;
			}
			if (GetComponent<InventoryObject>() != null)
			{
				return !GetComponent<InventoryObject>().isSelected;
			}
			return true;
		}
	}

	protected virtual void Awake()
	{
		smoothMove = GetComponent<SmoothMove>();
		damageReciver = GetComponent<DamageReciver2>();
		if (damageReciver == null)
		{
			damageReciver = base.gameObject.AddComponent<DamageReciver2>();
		}
	}

	protected virtual IEnumerator Start()
	{
		_hp = maxHP;
		isClone = !base.photonView.isMine;
		yield return null;
		UpdateWeakDevice();
	}

	protected virtual void Update()
	{
	}

	protected virtual void LateUpdate()
	{
		if (isBusyByPlayer && myDriver != null)
		{
			myDriver.transform.position = PlayerPlacePivot.transform.position;
			myDriver.transform.rotation = PlayerPlacePivot.transform.rotation;
		}
	}

	protected virtual void FixedUpdate()
	{
	}

	protected virtual void OnDestroy()
	{
	}

	protected virtual void OnCollisionEnter(Collision collision)
	{
	}

	protected virtual void OnTriggerEnter(Collider other)
	{
	}

	public virtual void ApplyDamage(float val, int fromWhom)
	{
		if (base.photonView != null && _hp > 0f)
		{
			PhotonNetwork.RPC(base.photonView, "ApplyDamageRPC", PhotonTargets.All, false, (int)val, fromWhom);
		}
	}

	[PunRPC]
	public virtual void ApplyDamageRPC(int dmg, int fromWhom)
	{
		if (base.photonView.isMine && _hp != 0f)
		{
			_hp -= dmg;
			if (_hp < 0f)
			{
				_hp = 0f;
			}
		}
	}

	public virtual void ApplyHeal(float val)
	{
		_hp += val;
	}

	[PunRPC]
	protected virtual void UpdateHp(float newVal, int fromWho)
	{
		_hp = newVal;
	}

	public virtual void OnPlayerSitMe(CharacterMotor player)
	{
	}

	[PunRPC]
	public virtual void OnPlayerSitMeRPC(int playerViewId)
	{
		if (!base.photonView.isMine)
		{
			HUDManager.instance.CreateCarTeamIndicator(this);
		}
	}

	public virtual void OnPlayerLeaveMe(CharacterMotor player)
	{
		if (!base.photonView.isMine)
		{
			HUDManager.instance.RemoveCarTeamIndicator(this);
		}
	}

	public virtual IEnumerator OnPlayerLeaveMeRPC()
	{
		yield break;
	}

	protected virtual void OnOwnerTransfer(bool hasBecomeClone)
	{
		if (hasBecomeClone)
		{
			EnablePhysics(enable: false);
		}
	}

	public virtual void EnablePhysics(bool enable)
	{
	}

	private bool IsPlayerPlaceEmpty()
	{
		return PlayerPlacePivot.childCount == 0;
	}

	public virtual bool Shootable()
	{
		return gunController != null;
	}

	protected virtual void DisablePhysics()
	{
		if (!isBusyByPlayer)
		{
			EnablePhysics(enable: false);
		}
	}

	protected virtual void UpdateWeakDevice()
	{
		if (Device.isWeakDevice)
		{
			Projector componentInChildren = GetComponentInChildren<Projector>();
			if (componentInChildren != null)
			{
				componentInChildren.gameObject.SetActive(value: false);
			}
		}
	}
}
