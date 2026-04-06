using UnityEngine;

public class MolotovBullet : BaseGrenade
{
	public override void Throw(float strength, float torque)
	{
		Rigidbody component = GetComponent<Rigidbody>();
		component.velocity = base.transform.forward * 25f;
		component.AddTorque(new Vector3(199f, 114f, 298f), ForceMode.Impulse);
	}

	protected override void OnCollisionEnter(Collision collision)
	{
		WeaponsPoolManager.instance.ShowBulletHitFX(base.transform.position, Vector3.up, BulletType.Grenade);
		Explode();
		DamageInRadius();
	}
}
