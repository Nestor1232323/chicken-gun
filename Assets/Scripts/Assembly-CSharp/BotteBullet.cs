using UnityEngine;

public class BotteBullet : BaseGrenade
{
	protected override void Start()
	{
		base.Start();
		Invoke("Explode", 4.15f);
	}

	public override void Throw(float strength, float torque)
	{
		Rigidbody component = GetComponent<Rigidbody>();
		component.velocity = base.transform.forward * 10f;
		component.AddTorque(new Vector3(189f, 43f, 218f), ForceMode.Impulse);
	}

	protected override void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.GetComponent<DamageReciver2>() != null)
		{
			collision.collider.GetComponent<DamageReciver2>().Damage(damage, parentView.viewID);
		}
	}
}
