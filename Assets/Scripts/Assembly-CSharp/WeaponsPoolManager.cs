using System.Collections;
using UnityEngine;

public class WeaponsPoolManager : MonoBehaviour
{
	public static WeaponsPoolManager instance;

	public GameObject GunBulletPrefab;

	public GameObject Missile_1Prefab;

	public GameObject GrenadePrefab;

	public GameObject SmokeGrenadePrefab;

	public GameObject MolotovGrenadePrefab;

	public GameObject ArrowPrefab;

	public GameObject BlasterBulletPrefab;

	public GameObject GrenadeGunBulletPrefab;

	public GameObject bulletHolePrefab;

	public GameObject playerHitBulletHole;

	public GameObject bloodParticlesPrefab;

	public GameObject bulletParticlesPrefab;

	public GameObject missile1ExplosionPrefab;

	public GameObject grenadeExplosionPrefab;

	public GameObject smokeGrenadeExplosionPrefab;

	public GameObject molotovGrenadeExplosionPrefab;

	public GameObject carFire;

	public GameObject carExplosion;

	public GameObject floorTriggerPrefab;

	public GameObject[] PlayerKillFXs;

	private BaseBulletScript bullet;

	private GameObject fx;

	private GameObject bulletParticles;

	private GameObject hole;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		GunBulletPrefab.CreatePool(30);
		BlasterBulletPrefab.CreatePool(10);
		Missile_1Prefab.CreatePool(5);
		ArrowPrefab.CreatePool(4);
		bloodParticlesPrefab.CreatePool(6);
		bulletParticlesPrefab.CreatePool(10);
		GrenadePrefab.CreatePool(3);
		GrenadeGunBulletPrefab.CreatePool(3);
		PlayerKillFXs[0].CreatePool(3);
	}

	public T CreateBullet<T>(BulletType bulletType, Vector3 pos, Quaternion rot, PhotonView parentPhotonView, bool isOriginal = false)
	{
		switch (bulletType)
		{
		case BulletType.GunBullet:
			bullet = GunBulletPrefab.Spawn(pos, rot).GetComponent<BaseBulletScript>();
			break;
		case BulletType.Missile_1:
			bullet = Missile_1Prefab.Spawn(pos, rot).GetComponent<BaseBulletScript>();
			break;
		case BulletType.Arrow:
			bullet = ArrowPrefab.Spawn(pos, rot).GetComponent<BaseBulletScript>();
			break;
		case BulletType.BlasterBullet:
			bullet = BlasterBulletPrefab.Spawn(pos, rot).GetComponent<BaseBulletScript>();
			break;
		case BulletType.Grenade:
			bullet = GrenadePrefab.Spawn(pos, rot).GetComponent<BaseBulletScript>();
			break;
		case BulletType.GrenadeGunBullet:
			bullet = GrenadeGunBulletPrefab.Spawn(pos, rot).GetComponent<BaseBulletScript>();
			break;
		}
		bullet.parentViewID = parentPhotonView.viewID;
		bullet.parentView = parentPhotonView;
		bullet.isOriginal = isOriginal;
		if (bullet != null)
		{
			bullet.OnSpawn();
			return bullet.GetComponent<T>();
		}
		return default(T);
	}

	public BaseBulletScript Shoot(BulletType bulletType, Vector3 pos, Quaternion rot, PhotonView parentPhotonView, bool isOriginal = false)
	{
		return CreateBullet<BaseBulletScript>(bulletType, pos, rot, parentPhotonView, isOriginal);
	}

	public void ShowBulletHitFX(Vector3 pos, Vector3 flowDir, BulletType bulletType)
	{
		switch (bulletType)
		{
		case BulletType.GunBullet:
		case BulletType.BlasterBullet:
			fx = ObjectPool.Spawn(bulletParticlesPrefab);
			fx.transform.position = pos;
			fx.transform.rotation = Quaternion.LookRotation(flowDir, Vector3.up);
			fx.transform.up = flowDir;
			StartCoroutine(RecycleAfter(0.8f, fx));
			break;
		case BulletType.Missile_1:
			fx = ObjectPool.Spawn(missile1ExplosionPrefab);
			fx.transform.position = pos;
			StartCoroutine(RecycleAfter(2f, fx));
			break;
		case BulletType.Grenade:
		case BulletType.GrenadeGunBullet:
			fx = ObjectPool.Spawn(grenadeExplosionPrefab);
			fx.transform.position = pos;
			StartCoroutine(RecycleAfter(2f, fx));
			break;
		case BulletType.SmokeGrenade:
			fx = ObjectPool.Spawn(smokeGrenadeExplosionPrefab);
			fx.transform.position = pos;
			StartCoroutine(RecycleAfter(14f, fx));
			break;
		case BulletType.MolotovGrenade:
			fx = ObjectPool.Spawn(molotovGrenadeExplosionPrefab);
			fx.transform.position = pos;
			StartCoroutine(RecycleAfter(6f, fx));
			break;
		}
	}

	public void ShowBloodParticles(Vector3 pos, Vector3 flowDir)
	{
		bulletParticles = ObjectPool.Spawn(bloodParticlesPrefab);
		bulletParticles.transform.position = pos;
		bulletParticles.transform.rotation = Quaternion.LookRotation(flowDir, Vector3.up);
		bulletParticles.transform.up = flowDir;
		StartCoroutine(RecycleAfter(1f, bulletParticles));
	}

	public void ShowBulletHole(RaycastHit hitInfo, bool isOnPlayer)
	{
		if (isOnPlayer)
		{
			hole = ObjectPool.Spawn(playerHitBulletHole);
		}
		else
		{
			hole = ObjectPool.Spawn(bulletHolePrefab);
		}
		hole.transform.position = hitInfo.point;
		hole.transform.rotation = Quaternion.LookRotation(hitInfo.normal, Vector3.up);
		hole.transform.up = hitInfo.normal;
		if (hitInfo.collider != null)
		{
			hole.transform.SetParent(hitInfo.collider.transform);
		}
	}

	private IEnumerator RecycleAfter(float time, GameObject go)
	{
		yield return new WaitForSeconds(time);
		if (go != null)
		{
			go.Recycle();
		}
	}

	public GameObject PlayPlayerKillFX()
	{
		GameObject gameObject = PlayerKillFXs[Random.Range(0, PlayerKillFXs.Length)].Spawn();
		StartCoroutine(RecycleAfter(1.9f, gameObject));
		return gameObject;
	}

	public GameObject SpawnGrenade(Vector3 pos, Quaternion dir, byte grenadeType)
	{
		GameObject result = null;
		switch (grenadeType)
		{
		case 2:
			result = instance.GrenadePrefab.Spawn(pos, dir);
			break;
		case 4:
			result = instance.SmokeGrenadePrefab.Spawn(pos, dir);
			break;
		case 5:
			result = instance.MolotovGrenadePrefab.Spawn(pos, dir);
			break;
		}
		return result;
	}
}
