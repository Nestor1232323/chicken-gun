using System.Collections;
using UnityEngine;

public class PlaneController : Vehicle, IShootable
{
	private Rigidbody rb;

	private Vector2 delta;

	private float liftForce;

	[SerializeField]
	private float maxSpeed = 45f;

	public Transform FumePivot;

	private GameObject Fire;

	private GameObject Explosion;

	protected override void Awake()
	{
		base.Awake();
		if (gunController != null)
		{
			gunController.shootRPCDelegate = this;
			gunController.parentPhotonView = base.photonView;
		}
		rb = GetComponent<Rigidbody>();
	}

	protected override IEnumerator Start()
	{
		if (base.photonView.isMine)
		{
			Invoke("DisablePhysics", 3f);
		}
		_ = base.photonView.isMine;
		return base.Start();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		Move(delta.x, delta.y, 0f, liftForce, airBrakes: false);
		LimitSpeed();
	}

	public void Move(float rollInput, float pitchInput, float yawInput, float throttleInput, bool airBrakes)
	{
	}

	public void SetDelta(Vector2 delta)
	{
		this.delta = delta;
	}

	public void SetAccel(float force)
	{
		liftForce = force;
	}

	[PunRPC]
	public override void ApplyDamageRPC(int dmg, int fromWhom)
	{
		base.ApplyDamageRPC(dmg, fromWhom);
		if (base.photonView.isMine)
		{
			PhotonNetwork.RPC(base.photonView, "UpdateHp", PhotonTargets.AllBuffered, false, _hp, fromWhom);
		}
	}

	[PunRPC]
	protected override void UpdateHp(float newVal, int fromWho)
	{
		base.UpdateHp(newVal, fromWho);
		if (base.hp == 0f)
		{
			PhotonView photonView = PhotonView.Find(fromWho);
			if (photonView != null && photonView.isMine && photonView.GetComponent<CharacterMotor>() != null)
			{
				photonView.GetComponent<CharacterMotor>().AddExp(expPoints);
			}
			StartCoroutine(OnCrash());
		}
		else if (base.hp <= maxHP * 0.3f && Fire == null)
		{
			Fire = Object.Instantiate(WeaponsPoolManager.instance.carFire, FumePivot.position, FumePivot.rotation);
			Fire.transform.SetParent(FumePivot);
		}
	}

	private IEnumerator OnCrash()
	{
		if (isBusyByPlayer && myDriver != null && myDriver.photonView.isMine)
		{
			CarOrPlayerSwitcher.instance.GetOutFromCar();
		}
		yield return null;
		Object.Instantiate(WeaponsPoolManager.instance.carExplosion, FumePivot.position, FumePivot.rotation);
		if (base.photonView.isMine)
		{
			AddCrashPhysics();
		}
		yield return new WaitForSeconds(2.7f);
		if (base.photonView.isMine)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
	}

	private void AddCrashPhysics()
	{
		GetComponent<Rigidbody>().AddTorque(Vector3.up * 160f, ForceMode.Impulse);
	}

	public override void EnablePhysics(bool enable)
	{
		base.EnablePhysics(enable);
		GetComponent<Rigidbody>().isKinematic = !enable;
		rb.useGravity = enable;
		if (base.photonView.isMine)
		{
			CancelInvoke("DisablePhysics");
			Invoke("DisablePhysics", 5f);
		}
	}

	public override void OnPlayerSitMe(CharacterMotor player)
	{
		base.OnPlayerSitMe(player);
		PhotonNetwork.RPC(base.photonView, "OnPlayerSitMeRPC", PhotonTargets.AllBuffered, false, player.photonView.viewID);
	}

	[PunRPC]
	public override void OnPlayerSitMeRPC(int playerViewId)
	{
		if (base.photonView.isMine)
		{
			CancelInvoke("DisablePhysics");
		}
		PhotonView photonView = PhotonView.Find(playerViewId);
		if (photonView != null)
		{
			if (photonView.isMine)
			{
				EnablePhysics(enable: true);
			}
			else
			{
				EnablePhysics(enable: false);
			}
		}
		isBusyByPlayer = true;
	}

	public override void OnPlayerLeaveMe(CharacterMotor player)
	{
		base.OnPlayerLeaveMe(player);
		PhotonNetwork.RPC(base.photonView, "OnPlayerLeaveMeRPC", PhotonTargets.AllBuffered, false);
	}

	[PunRPC]
	public override IEnumerator OnPlayerLeaveMeRPC()
	{
		isBusyByPlayer = false;
		liftForce = -1f;
		yield return new WaitForSeconds(4f);
		if (!isBusyByPlayer && base.photonView.isMine)
		{
			EnablePhysics(enable: false);
		}
		else if (isBusyByPlayer && !base.photonView.isMine)
		{
			EnablePhysics(enable: false);
		}
	}

	public void StartShoot(float holdStrength)
	{
		PhotonNetwork.RPC(base.photonView, "StartShootRPC", PhotonTargets.All, false);
	}

	[PunRPC]
	public void StartShootRPC()
	{
		gunController.StartShooting();
	}

	public void StopShooting()
	{
		PhotonNetwork.RPC(base.photonView, "StopShootRPC", PhotonTargets.All, false);
	}

	[PunRPC]
	public void StopShootRPC()
	{
		gunController.StopShooting();
	}

	public void PushBulletForRPC(Vector3 pos, Vector3 rot, float timeSinceGameStart)
	{
		PhotonNetwork.RPC(base.photonView, "PushBulletRPC", PhotonTargets.All, false, pos, rot, timeSinceGameStart);
	}

	[PunRPC]
	public void PushBulletRPC(Vector3 pos, Vector3 rot, float timeSinceGameStart)
	{
		gunController.ShootFromRPC(pos, rot, timeSinceGameStart);
	}

	public Transform CamTarget()
	{
		return base.transform;
	}

	private void LimitSpeed()
	{
		if (rb.velocity.sqrMagnitude > maxSpeed * maxSpeed)
		{
			rb.velocity = GetComponent<Rigidbody>().velocity.normalized * maxSpeed;
		}
	}
}
