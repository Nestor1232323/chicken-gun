using UnityEngine;

public class BL_Bullet : MonoBehaviour
{
	public float velocity = 100f;

	public float life = 2f;

	public float impactImpulseForce = 50f;

	[HideInInspector]
	public BL_Turret turretParent;

	private TrailRenderer _trailRenderer;

	private ParticleSystem _particleSystem;

	private Transform _transform;

	private AudioSource _audioSource;

	private bool _hasHit;

	private void Awake()
	{
		_trailRenderer = base.gameObject.GetComponent<TrailRenderer>();
		_particleSystem = base.gameObject.GetComponent<ParticleSystem>();
		_transform = base.gameObject.GetComponent<Transform>();
		_audioSource = base.gameObject.GetComponent<AudioSource>();
	}

	private void OnEnable()
	{
		Invoke("Destroy", life);
		if (_trailRenderer != null)
		{
			_trailRenderer.Clear();
		}
		if (_particleSystem != null)
		{
			_particleSystem.Clear();
		}
		_particleSystem.Play();
		_hasHit = false;
		if (_audioSource != null)
		{
			_audioSource.Play();
		}
	}

	private void OnDisable()
	{
		CancelInvoke();
	}

	private void Destroy()
	{
		base.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		if (_hasHit)
		{
			return;
		}
		base.transform.Translate(Vector3.forward * velocity * Time.deltaTime);
		if (Physics.Raycast(_transform.position, _transform.forward, out var hitInfo, 2f + velocity * 0.02f))
		{
			_ = hitInfo.collider;
			Vector3 eulerAngles = Vector3.Reflect(hitInfo.point - _transform.position, hitInfo.normal);
			GameObject obj = hitInfo.collider.gameObject;
			Vector3 vector = obj.transform.position - _transform.position;
			Rigidbody component = obj.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.AddForceAtPosition(vector.normalized * impactImpulseForce, hitInfo.point, ForceMode.Impulse);
			}
			_transform.position = hitInfo.point;
			velocity = 0f;
			obj.SendMessage("Hit", null, SendMessageOptions.DontRequireReceiver);
			_particleSystem.Stop();
			_trailRenderer.Clear();
			if (_audioSource != null)
			{
				_audioSource.Stop();
			}
			Invoke("Destroy", 1f);
			_hasHit = true;
			if (turretParent != null)
			{
				turretParent.Impact(hitInfo.point, eulerAngles);
			}
		}
	}
}
