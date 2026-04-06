using UnityEngine;

public class BaseGrenade : BaseBulletScript
{
	public bool explodeOnContact;

	public float damageRadius = 5f;

	protected virtual void Start()
	{
	}

	protected override void Update()
	{
	}

	protected virtual void Explode()
	{
		OnRecycle();
	}

	protected virtual void DamageInRadius()
	{
		if (damage < 0f || (parentView != null && !parentView.isMine))
		{
			return;
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position, damageRadius);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].GetComponent<IDamageReciver>() != null)
			{
				array[i].GetComponent<IDamageReciver>().Damage(damage, parentView.viewID);
			}
		}
	}

	public virtual void Throw(float strength, float torque)
	{
		GetComponent<Rigidbody>().velocity = base.transform.forward * strength;
		GetComponent<Rigidbody>().AddTorque(new Vector3(0f, 0f, torque), ForceMode.Impulse);
	}

	protected virtual void OnCollisionEnter(Collision collision)
	{
		if (explodeOnContact || collision.collider.CompareTag("Player"))
		{
			CancelInvoke("Explode");
			Explode();
		}
	}

	protected override void OnTriggerEnter(Collider collider)
	{
	}
}
