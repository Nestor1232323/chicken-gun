using System.Collections;
using UnityEngine;

public class HorseController : Vehicle, IShootable
{
	private CharacterController characterController;

	[SerializeField]
	private Animator animator;

	private float accel;

	private float steer;

	public CarCamParams camFollowSettings;

	[SerializeField]
	private float maxSpeed = 11f;

	[SerializeField]
	private float maxRotationSpeed = 2.1f;

	private float currentSpeed;

	private Vector3 prevPos = Vector3.zero;

	protected override void Awake()
	{
		characterController = GetComponent<CharacterController>();
		animator = GetComponentInChildren<Animator>();
	}

	protected override IEnumerator Start()
	{
		animator.SetFloat("moveK", 0f);
		return base.Start();
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
	}

	private IEnumerator OnCrash()
	{
		if (isBusyByPlayer && myDriver != null && myDriver.photonView.isMine)
		{
			CarOrPlayerSwitcher.instance.GetOutFromCar();
		}
		animator.CrossFade("Die", 0.1f);
		yield return new WaitForSeconds(1.7f);
		if (base.photonView.isMine)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
	}

	protected override void Update()
	{
		base.Update();
		UpdateSpeed();
		Move();
		if (currentSpeed < 0.03f)
		{
			animator.SetFloat("moveK", 0f);
		}
		else
		{
			animator.SetFloat("moveK", 1f);
		}
	}

	private void UpdateSpeed()
	{
		currentSpeed = (base.transform.position - prevPos).sqrMagnitude * 70f;
		prevPos = base.transform.position;
	}

	protected virtual void Move()
	{
		base.transform.Rotate(base.transform.up, (!(accel >= 0f)) ? ((0f - steer) * maxRotationSpeed) : (steer * maxRotationSpeed));
		characterController.SimpleMove(base.transform.forward * accel * maxSpeed);
	}

	public void SetAccel(float accel)
	{
		this.accel = accel;
	}

	public void SetSteer(float steer)
	{
		this.steer = steer;
	}

	public override void OnPlayerSitMe(CharacterMotor player)
	{
		base.OnPlayerSitMe(player);
		PhotonNetwork.RPC(base.photonView, "OnPlayerSitMeRPC", PhotonTargets.AllBuffered, false, player.photonView.viewID);
	}

	[PunRPC]
	public override void OnPlayerSitMeRPC(int playerViewId)
	{
		base.OnPlayerSitMeRPC(playerViewId);
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
		accel = 0f;
		steer = 0f;
		isBusyByPlayer = false;
		yield return null;
		animator.SetFloat("moveK", 0f);
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
}
