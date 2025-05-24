using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CarController : Vehicle, IShootable
{
	public CarInfo carInfo;

	public CarCamParams camFollowSettings;

	private HUDInfo HUD;

	public Transform FumePivot;

	public Transform[] FrontWheelPivots;

	public Transform[] BackWheelPivots;

	public Transform CloneCarWheelPivot;

	public VehicleParent vp;

	[HideInInspector]
	private CarCloneAssist carCloneAssist;

	public Transform HoodCamPivot;

	[HideInInspector]
	public float accel;

	[HideInInspector]
	public float steer;

	[HideInInspector]
	public float brake;

	[HideInInspector]
	public float pitch;

	[HideInInspector]
	public float roll;

	[HideInInspector]
	public float ebrake;

	private GameObject Fire;

	private GameObject Explosion;

	private float timeInStunt;

	public BoxCollider underCollider;

	public float CurrentSpeed => vp.velMag;

	public float CurrentSpeedSqr => vp.sqrVelMag;

	private InventoryCarObject MyInventory => GetComponent<InventoryCarObject>();

	protected override void Awake()
	{
		base.Awake();
		vp = GetComponent<VehicleParent>();
		carCloneAssist = base.gameObject.GetComponent<CarCloneAssist>();
		CreateUnderCollider();
		if (gunController != null)
		{
			gunController.shootRPCDelegate = this;
			gunController.parentPhotonView = base.photonView;
		}
		CheckOptimization();
	}

	protected override IEnumerator Start()
	{
		yield return base.Start();
		EnablePhysics(base.photonView.isMine);
		if (base.photonView.isMine)
		{
			Invoke("DisablePhysics", 2f);
		}
		yield return null;
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.rotation);
			stream.SendNext(vp.steerInput);
			float num = vp.sqrVelMag;
			if (vp.accelInput != 0f)
			{
				num *= 1f;
			}
			else if (vp.brakeInput != 0f)
			{
				num *= -1f;
			}
			stream.SendNext(num);
		}
		else
		{
			Vector3 recivedPlayerPos = (Vector3)stream.ReceiveNext();
			Quaternion recivedPlayerRot = (Quaternion)stream.ReceiveNext();
			if (smoothMove != null)
			{
				smoothMove.AddStreamPack(recivedPlayerPos, recivedPlayerRot);
			}
			if (carCloneAssist != null)
			{
				carCloneAssist.steer = (float)stream.ReceiveNext();
				carCloneAssist.speed = (float)stream.ReceiveNext();
			}
		}
	}

	protected override void Update()
	{
		base.Update();
		if (isClone && base.photonView.isMine)
		{
			OnOwnerTransfer(hasBecomeClone: false);
		}
		else if (!isClone && !base.photonView.isMine)
		{
			OnOwnerTransfer(hasBecomeClone: true);
		}
		isClone = !base.photonView.isMine;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (base.photonView.isMine)
		{
			MoveCar();
		}
		LimitCarSpeed();
	}

	private void MoveCar()
	{
		if (base.Alive && isBusyByPlayer)
		{
			vp.SetAccel(accel);
			vp.SetBrake(brake);
			vp.SetSteer(steer);
			vp.SetEbrake(ebrake);
		}
		else
		{
			vp.SetAccel(0f);
			vp.SetBrake(0f);
			vp.SetEbrake(0.8f);
		}
	}

	[PunRPC]
	public override void ApplyDamageRPC(int dmg, int fromWhom)
	{
		base.ApplyDamageRPC(dmg, fromWhom);
		if (base.photonView.isMine && !crashed)
		{
			PhotonNetwork.RPC(base.photonView, "UpdateHp", PhotonTargets.All, false, _hp, fromWhom);
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
			Fire.transform.SetParent(FumePivot);
		}
		if (HUD != null)
		{
			HUD.UpdateView(base.hp, maxHP);
		}
	}

	private IEnumerator OnCrash()
	{
		crashed = true;
		if (isBusyByPlayer && myDriver != null)
		{
			if (myDriver.photonView.isMine)
			{
				CarOrPlayerSwitcher.instance.GetOutFromCar();
			}
			HUDManager.instance.RemoveCarTeamIndicator(this);
		}
		yield return null;
		Object.Instantiate(WeaponsPoolManager.instance.carExplosion, FumePivot.position, FumePivot.rotation);
		if (base.photonView.isMine)
		{
			AddCrashPhysics();
		}
		if (HUD != null)
		{
			HUD.Show(show: false);
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

	protected override void OnCollisionEnter(Collision collision)
	{
		base.OnCollisionEnter(collision);
		if (base.photonView.isMine && CurrentSpeedSqr >= 4f && collision.collider.CompareTag("Player") && !collision.collider.GetComponent<PhotonView>().isMine)
		{
			Physics.IgnoreCollision(GetComponent<BoxCollider>(), collision.collider);
			if (myDriver != null)
			{
				collision.collider.GetComponent<DamageReciver2>().Damage(50f, myDriver.photonView.viewID);
			}
		}
	}

	private void LimitCarSpeed()
	{
		if (vp != null && vp.sqrVelMag > carInfo.MaxSpeedSqr)
		{
			GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * carInfo.maxSpeed;
		}
	}

	public void SetNewFragsVal(int val)
	{
		PhotonNetwork.RPC(base.photonView, "UpdateFragsCountRPC", PhotonTargets.All, false, val);
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

	private void CheckIsStunt()
	{
		if (Mathf.Abs(base.transform.eulerAngles.z) > 70f)
		{
			timeInStunt += Time.deltaTime;
			if (timeInStunt > 5f)
			{
				base.transform.rotation = Quaternion.Euler(new Vector3(base.transform.eulerAngles.x, base.transform.eulerAngles.y, 0f));
				timeInStunt = 0f;
			}
		}
		else
		{
			timeInStunt = 0f;
		}
	}

	public void OnMatchFinished()
	{
	}

	public override void OnPlayerSitMe(CharacterMotor player)
	{
		base.OnPlayerSitMe(player);
		PhotonNetwork.RPC(base.photonView, "OnPlayerSitMeRPC", PhotonTargets.All, false, player.photonView.viewID);
	}

	[PunRPC]
	public override void OnPlayerSitMeRPC(int playerViewId)
	{
		base.OnPlayerSitMeRPC(playerViewId);
		if (base.photonView.isMine)
		{
			CancelInvoke("DisablePhysics");
			carCloneAssist.StopEngineSound(stop: false);
			EnableShadow(enable: true);
		}
		PhotonView photonView = PhotonView.Find(playerViewId);
		if (photonView != null)
		{
			if (photonView.isMine)
			{
				EnablePhysics(enable: true);
			}
			myDriver = photonView.GetComponent<CharacterMotor>();
		}
		isBusyByPlayer = true;
	}

	public override void OnPlayerLeaveMe(CharacterMotor player)
	{
		base.OnPlayerLeaveMe(player);
		PhotonNetwork.RPC(base.photonView, "OnPlayerLeaveMeRPC", PhotonTargets.All, false);
	}

	[PunRPC]
	public override IEnumerator OnPlayerLeaveMeRPC()
	{
		isBusyByPlayer = false;
		ebrake = 1f;
		EnableShadow(enable: false);
		HUDManager.instance.RemoveCarTeamIndicator(this);
		yield return new WaitForSeconds(4f);
		if (!isBusyByPlayer && base.photonView.isMine)
		{
			EnablePhysics(enable: false);
		}
		else if (isBusyByPlayer && !base.photonView.isMine)
		{
			EnablePhysics(enable: false);
		}
		EnableShadow(enable: false);
	}

	public override void EnablePhysics(bool enable)
	{
		base.EnablePhysics(enable);
		carCloneAssist.enabled = !enable;
		GetComponent<FlipControl>().enabled = enable;
		GetComponent<VehicleAssist>().enabled = enable;
		GetComponent<VehicleParent>().enabled = enable;
		if (GetComponentInChildren<TireScreech>() != null)
		{
			GetComponentInChildren<TireScreech>().enabled = enable;
		}
		GetComponentInChildren<GasMotor>().enabled = enable;
		GetComponentInChildren<SteeringControl>().enabled = enable;
		GetComponentInChildren<GearboxTransmission>().enabled = enable;
		SuspensionPart[] componentsInChildren = GetComponentsInChildren<SuspensionPart>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = enable;
		}
		if (MyInventory.isSelected)
		{
			GetComponent<Rigidbody>().isKinematic = true;
			GetComponent<Rigidbody>().useGravity = false;
		}
		else
		{
			GetComponent<Rigidbody>().isKinematic = false;
			GetComponent<Rigidbody>().useGravity = true;
		}
		underCollider.enabled = !enable;
		Transform[] frontWheelPivots = FrontWheelPivots;
		foreach (Transform obj in frontWheelPivots)
		{
			obj.GetComponent<Suspension>().enabled = enable;
			obj.GetChild(0).GetComponent<Wheel>().enabled = enable;
			obj.GetChild(0).GetComponent<TireMarkCreate>().enabled = enable;
		}
		frontWheelPivots = BackWheelPivots;
		foreach (Transform obj2 in frontWheelPivots)
		{
			obj2.GetComponent<Suspension>().enabled = enable;
			obj2.GetChild(0).GetComponent<Wheel>().enabled = enable;
			obj2.GetChild(0).GetComponent<TireMarkCreate>().enabled = enable;
		}
		carCloneAssist.OnPhysicsToggle(enable);
		if (enable && base.photonView.isMine)
		{
			CancelInvoke("DisablePhysics");
			Invoke("DisablePhysics", 4f);
		}
		carCloneAssist.StopEngineSound(!enable);
	}

	protected override void DisablePhysics()
	{
		if (!isBusyByPlayer)
		{
			base.DisablePhysics();
			carCloneAssist.StopEngineSound(stop: true);
		}
	}

	private void CreateUnderCollider()
	{
		Vector3 size = GetComponent<BoxCollider>().size;
		underCollider = base.gameObject.AddComponent<BoxCollider>();
		float y = base.transform.InverseTransformPoint(carCloneAssist.wheelPivot.position).y;
		underCollider.center = new Vector3(0f, y, 0f);
		underCollider.size = new Vector3(size.x * 0.34f, FrontWheelPivots[0].GetComponentInChildren<Wheel>().tireRadius * 2f, size.z * 0.6f);
	}

	private void CheckOptimization()
	{
		EnableShadow(enable: false);
	}

	private void EnableShadow(bool enable)
	{
		Projector componentInChildren = base.gameObject.GetComponentInChildren<Projector>(includeInactive: true);
		if (componentInChildren != null)
		{
			componentInChildren.enabled = enable;
		}
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private IEnumerator _003CStart_003E__BaseCallProxy0()
	{
		return base.Start();
	}
}
