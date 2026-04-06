using UnityEngine;

public class BL_Box : MonoBehaviour
{
	public GameObject prefabExplosion;

	public int hp = 10;

	public Vector3 targetLocation = new Vector3(0f, 0f, 30f);

	public float force;

	public float maxVelocity;

	private Rigidbody _rigidbody;

	private Transform _transform;

	private int _orgHp;

	private void Start()
	{
		_rigidbody = base.gameObject.GetComponent<Rigidbody>();
		_transform = base.gameObject.transform;
		_orgHp = hp;
	}

	private void FixedUpdate()
	{
		if (_rigidbody.velocity.magnitude < maxVelocity)
		{
			_rigidbody.AddForce((targetLocation - _transform.position).normalized * force, ForceMode.Force);
		}
	}

	public void Hit()
	{
		hp--;
		if (hp <= 0)
		{
			Destroy();
		}
	}

	private void Destroy()
	{
		Object.Instantiate(prefabExplosion, _transform.position, Quaternion.identity);
		hp = _orgHp;
		_rigidbody.velocity = Vector3.zero;
		_transform.position = new Vector3(Random.Range(-100, 100), 40f, 150f);
	}
}
