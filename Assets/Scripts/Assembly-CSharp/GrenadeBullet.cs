using UnityEngine;

public class GrenadeBullet : Missile
{
	[SerializeField]
	private bool explodeOnContact;

	private void Start()
	{
		base.transform.Rotate(base.transform.right, -6.15f, Space.World);
		Rigidbody component = GetComponent<Rigidbody>();
		component.velocity = base.transform.forward * speed;
		component.AddRelativeTorque(new Vector3(9.5f, 0f, 0f), ForceMode.Impulse);
	}

	protected override void Update()
	{
		base.Update();
	}

	protected override void UpdatePosition()
	{
	}

	protected override void OnTriggerEnter(Collider collider)
	{
	}

	private void OnCollisionEnter(Collision collision)
	{
		ShowBulletBoom();
	}

	public override void ShowBulletBoom()
	{
		base.ShowBulletBoom();
	}

	public override void OnSpawn()
	{
		base.OnSpawn();
	}
}
