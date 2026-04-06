using System.Collections;
using UnityEngine;

public class ArmGrenade : BaseGrenade
{
	private Coroutine crt;

	protected override void Explode()
	{
		if (crt != null)
		{
			StopCoroutine(crt);
		}
		WeaponsPoolManager.instance.ShowBulletHitFX(base.transform.position, Vector3.up, type);
		DamageInRadius();
		base.Explode();
	}

	public override void Throw(float strength, float torque)
	{
		base.Throw(strength, torque);
		if (!explodeOnContact)
		{
			Invoke("Explode", lifeTime);
		}
		crt = StartCoroutine(PlayBeepSound());
	}

	private IEnumerator PlayBeepSound()
	{
		AudioSource ac = GetComponent<AudioSource>();
		ac.Play();
		yield return new WaitForSeconds(0.2f);
		ac.Play();
		yield return new WaitForSeconds(0.2f);
		ac.Play();
		yield return new WaitForSeconds(0.15f);
		ac.Play();
		yield return new WaitForSeconds(0.15f);
		ac.Play();
		yield return new WaitForSeconds(0.12f);
		ac.Play();
		yield return new WaitForSeconds(0.12f);
		ac.Play();
		yield return new WaitForSeconds(0.1f);
		ac.Play();
		yield return new WaitForSeconds(0.05f);
	}
}
