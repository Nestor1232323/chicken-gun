using System.Collections.Generic;
using UnityEngine;

public class BL_Turret : MonoBehaviour
{
	[Header("AUDIO")]
	[Tooltip("1 or more sound effects (multiple sound effects will be alternated with each firing using round robin method).")]
	public AudioClip[] sfxFire;

	public float volumeFire = 1f;

	[Tooltip("1 or more sound effects (multiple sound effects will be alternated with each impact using round robin method).")]
	public AudioClip[] sfxImpact;

	public float volumeImpact = 1f;

	[Header("TURRET PROPERTIES")]
	public float rateOfFire = 300f;

	public bool fireSequential;

	public float rotationSpeed = 10f;

	public bool lockYAxis = true;

	public GameObject muzzleFlashPrefab;

	[Header("BULLET PROPERTIES")]
	public GameObject bulletPrefab;

	public float bulletVelocity = 100f;

	public float bulletLife = 2f;

	public GameObject impactPrefab;

	private int impactPoolSize = 8;

	private List<GameObject> _bullets = new List<GameObject>();

	private List<GameObject> _impacts = new List<GameObject>();

	private BL_Blaster[] _blasters;

	private int _bulletPoolSize = 120;

	private float _timerFire;

	private int _counter;

	private bool _fire;

	private Transform _transform;

	private Vector3 _target;

	private int _sfxRoundRobinFire;

	private int _sfxRoundRobinImpact;

	private void Awake()
	{
		GetBlasters();
		SetMuzzleFlash(muzzleFlashPrefab);
		_target = Vector3.forward;
		PoolBullets();
		PoolImpacts();
	}

	private void Update()
	{
		Vector3 forward = _target - base.transform.position;
		if (lockYAxis)
		{
			forward.y = 0f;
		}
		Quaternion b = Quaternion.LookRotation(forward);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * rotationSpeed);
	}

	public void Aim(Vector3 _newTarget)
	{
		_target = _newTarget;
	}

	public void Fire(bool _useRoundRobin = true)
	{
		if (fireSequential)
		{
			if (Time.time > _timerFire + 60f / rateOfFire / (float)_blasters.Length)
			{
				_counter++;
				if (_counter >= _blasters.Length)
				{
					_counter = 0;
				}
				_timerFire = Time.time;
				_fire = true;
			}
		}
		else if (Time.time > _timerFire + 60f / rateOfFire)
		{
			_timerFire = Time.time;
			_fire = true;
		}
		BL_Blaster[] blasters = _blasters;
		foreach (BL_Blaster bL_Blaster in blasters)
		{
			if (!_fire || (fireSequential && (!fireSequential || !(bL_Blaster == _blasters[_counter]))))
			{
				continue;
			}
			bL_Blaster.Fire(rateOfFire);
			if (sfxFire.Length != 0 && sfxFire[_sfxRoundRobinFire] != null)
			{
				AudioSource.PlayClipAtPoint(sfxFire[_sfxRoundRobinFire], bL_Blaster.muzzle.position, volumeFire);
			}
			GameObject gameObject = null;
			for (int j = 0; j < _bullets.Count; j++)
			{
				if (!_bullets[j].activeInHierarchy)
				{
					gameObject = _bullets[j];
					break;
				}
			}
			if (gameObject == null && bulletPrefab != null)
			{
				gameObject = Object.Instantiate(bulletPrefab);
				_bullets.Add(gameObject);
			}
			if (gameObject != null && bL_Blaster.muzzle != null)
			{
				gameObject.transform.position = bL_Blaster.muzzle.position;
				gameObject.transform.rotation = bL_Blaster.muzzle.rotation;
				gameObject.SetActive(value: true);
				BL_Bullet component = gameObject.GetComponent<BL_Bullet>();
				component.turretParent = this;
				component.velocity = bulletVelocity;
				component.life = bulletLife;
			}
			if (_useRoundRobin)
			{
				_sfxRoundRobinFire++;
			}
			if (_sfxRoundRobinFire >= sfxFire.Length)
			{
				_sfxRoundRobinFire = 0;
			}
		}
		_fire = false;
	}

	public string GetCurrentSFXName()
	{
		if (sfxFire.Length == 0)
		{
			return string.Empty;
		}
		return sfxFire[_sfxRoundRobinFire].name.ToUpper();
	}

	private void GetBlasters()
	{
		_blasters = base.gameObject.GetComponentsInChildren<BL_Blaster>();
	}

	public void PoolBullets()
	{
		_bulletPoolSize = Mathf.RoundToInt(rateOfFire / 60f * bulletLife * 2f) + 1;
		for (int i = 0; i < _bullets.Count; i++)
		{
			Object.Destroy(_bullets[i], bulletLife);
		}
		_bullets.Clear();
		_bullets = new List<GameObject>();
		for (int j = 0; j < _bulletPoolSize; j++)
		{
			if (bulletPrefab != null)
			{
				GameObject gameObject = Object.Instantiate(bulletPrefab);
				gameObject.GetComponent<BL_Bullet>().life = bulletLife;
				gameObject.GetComponent<BL_Bullet>().turretParent = this;
				gameObject.SetActive(value: false);
				_bullets.Add(gameObject);
			}
		}
	}

	public void PoolImpacts()
	{
		for (int i = 0; i < _impacts.Count; i++)
		{
			Object.Destroy(_impacts[i]);
		}
		_impacts.Clear();
		_impacts = new List<GameObject>();
		for (int j = 0; j < impactPoolSize; j++)
		{
			if (impactPrefab != null)
			{
				GameObject gameObject = Object.Instantiate(impactPrefab);
				gameObject.SetActive(value: false);
				_impacts.Add(gameObject);
			}
		}
	}

	public void Impact(Vector3 _position, Vector3 _eulerAngles)
	{
		if (sfxImpact.Length != 0)
		{
			_sfxRoundRobinImpact++;
			if (_sfxRoundRobinImpact >= sfxImpact.Length)
			{
				_sfxRoundRobinImpact = 0;
			}
			AudioSource.PlayClipAtPoint(sfxImpact[_sfxRoundRobinImpact], _position, volumeImpact);
		}
		if (_impacts.Count == 0)
		{
			return;
		}
		for (int i = 0; i < impactPoolSize; i++)
		{
			if (!_impacts[i].activeInHierarchy)
			{
				_impacts[i].transform.position = _position;
				_impacts[i].transform.forward = _eulerAngles;
				_impacts[i].SetActive(value: true);
				break;
			}
		}
	}

	public void SetImpact(GameObject _impactPrefab)
	{
		impactPrefab = _impactPrefab;
		PoolImpacts();
	}

	public void SetBullet(GameObject _bulletPrefab)
	{
		bulletPrefab = _bulletPrefab;
		PoolBullets();
	}

	public void SetMuzzleFlash(GameObject _muzzleFlashPrefab)
	{
		muzzleFlashPrefab = _muzzleFlashPrefab;
		if (_blasters.Length == 0)
		{
			return;
		}
		BL_Blaster[] blasters = _blasters;
		foreach (BL_Blaster bL_Blaster in blasters)
		{
			if (bL_Blaster.muzzle != null)
			{
				if (bL_Blaster.muzzle.transform.childCount > 0)
				{
					Object.Destroy(bL_Blaster.muzzle.transform.GetChild(0).gameObject);
				}
				if (muzzleFlashPrefab != null)
				{
					GameObject obj = Object.Instantiate(muzzleFlashPrefab, bL_Blaster.transform);
					obj.transform.parent = bL_Blaster.muzzle;
					obj.transform.position = bL_Blaster.muzzle.position;
					obj.transform.rotation = bL_Blaster.muzzle.rotation;
					bL_Blaster.RefreshMuzzleFlash();
				}
			}
		}
	}
}
