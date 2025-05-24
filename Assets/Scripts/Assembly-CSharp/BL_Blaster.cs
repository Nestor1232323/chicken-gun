using UnityEngine;

public class BL_Blaster : MonoBehaviour
{
	public enum State
	{
		IDLE = 0,
		RECOIL = 1,
		RETURN = 2
	}

	[Header("BLASTER PARAMETERS")]
	public Transform muzzle;

	public float recoilDistance;

	private float _recoilDuration = 0.1f;

	private float _returnDuration = 0.9f;

	private float _recoilSpeed = 1f;

	private float _timerRecoil;

	private Transform _transform;

	private Vector3 _orgLocalPosition;

	private Vector3 _recoilLocalPosition;

	private State _state;

	private BL_MuzzleFlash[] _muzzleFlashes;

	private void Awake()
	{
		_transform = base.gameObject.transform;
		_orgLocalPosition = _transform.localPosition;
		_recoilLocalPosition = _orgLocalPosition - new Vector3(0f, 0f, recoilDistance);
		RefreshMuzzleFlash();
	}

	private void Update()
	{
		float num = 0f;
		switch (_state)
		{
		case State.RECOIL:
			num = (Time.time - _timerRecoil) / (_recoilDuration / _recoilSpeed);
			_transform.localPosition = Vector3.Lerp(_orgLocalPosition, _recoilLocalPosition, num / _recoilDuration);
			if (num >= 1f)
			{
				_state = State.RETURN;
				_timerRecoil = Time.time;
			}
			break;
		case State.RETURN:
			num = (Time.time - _timerRecoil) / (_returnDuration / _recoilSpeed);
			_transform.localPosition = Vector3.Lerp(_recoilLocalPosition, _orgLocalPosition, num / _returnDuration);
			if (num >= 1f)
			{
				_state = State.IDLE;
				_timerRecoil = 0f;
			}
			break;
		}
	}

	public void Fire(float _rateOfFire)
	{
		_recoilSpeed = _rateOfFire / 60f;
		_state = State.RECOIL;
		_timerRecoil = Time.time;
		if (_muzzleFlashes.Length == 0)
		{
			RefreshMuzzleFlash();
		}
		BL_MuzzleFlash[] muzzleFlashes = _muzzleFlashes;
		foreach (BL_MuzzleFlash bL_MuzzleFlash in muzzleFlashes)
		{
			if (bL_MuzzleFlash != null)
			{
				bL_MuzzleFlash.Flash();
			}
		}
	}

	public void RefreshMuzzleFlash()
	{
		_muzzleFlashes = base.gameObject.GetComponentsInChildren<BL_MuzzleFlash>();
	}
}
