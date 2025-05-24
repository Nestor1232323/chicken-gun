using System.Collections;
using UnityEngine;

public class FlameThrowerGun : BaseWeaponScript
{
	[SerializeField]
	private ParticleSystem fireFlameAttack;

	[SerializeField]
	private ParticleSystem fireFlameIdle;

	[SerializeField]
	private float attackDist = 5f;

	protected override IEnumerator Start()
	{
		return base.Start();
	}

	protected override void Awake()
	{
		base.Awake();
	}

	public override void PushBullet()
	{
		if (!fireFlameAttack.isPlaying)
		{
			fireFlameIdle.Stop();
			fireFlameAttack.Play();
		}
		if (Physics.Linecast(base.transform.position + base.transform.forward * 0.25f, base.transform.position + base.transform.forward * attackDist, out var hitInfo))
		{
			DamageReciver2 component = hitInfo.collider.GetComponent<DamageReciver2>();
			if (component != null)
			{
				component.Damage(base.damage, base.parentViewID);
			}
		}
		else if (Physics.Linecast(base.transform.position + base.transform.forward * 0.25f, base.transform.position + base.transform.forward * attackDist + base.transform.right * 0.4f, out hitInfo))
		{
			DamageReciver2 component2 = hitInfo.collider.GetComponent<DamageReciver2>();
			if (component2 != null)
			{
				component2.Damage(base.damage, base.parentViewID);
			}
		}
		else if (Physics.Linecast(base.transform.position + base.transform.forward * 0.25f, base.transform.position + base.transform.forward * attackDist - base.transform.right * 0.4f, out hitInfo))
		{
			DamageReciver2 component3 = hitInfo.collider.GetComponent<DamageReciver2>();
			if (component3 != null)
			{
				component3.Damage(base.damage, base.parentViewID);
			}
		}
		else if (Physics.Linecast(base.transform.position + base.transform.forward * 0.25f, base.transform.position + base.transform.forward * attackDist + base.transform.up * 0.4f, out hitInfo))
		{
			DamageReciver2 component4 = hitInfo.collider.GetComponent<DamageReciver2>();
			if (component4 != null)
			{
				component4.Damage(base.damage, base.parentViewID);
			}
		}
	}

	public override void StartShooting(float holdStrength = 0f)
	{
		base.StartShooting(holdStrength);
	}

	public override void StopShooting()
	{
		base.StopShooting();
		fireFlameAttack.Stop();
		fireFlameIdle.Play();
	}
}
