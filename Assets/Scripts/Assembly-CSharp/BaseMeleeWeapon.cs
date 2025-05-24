using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BaseMeleeWeapon : BaseWeaponScript
{
	[SerializeField]
	public float damagePerHit = 12f;

	[SerializeField]
	private LayerMask hitLayers;

	protected override void Awake()
	{
		base.Awake();
	}

	protected override IEnumerator Start()
	{
		yield return base.Start();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
	}

	public override void StartShooting(float holdStrength = 0f)
	{
		base.StartShooting(holdStrength);
	}

	public override void StopShooting()
	{
		base.StopShooting();
	}

	public virtual void ApplyDamege()
	{
		shootSound.Play();
		Transform transform = bulletPivots[0];
		Vector3 one = Vector3.one;
		if (parentPhotonView.isMine)
		{
			one = FPSCamera.instance.GetCamLookBulletPoint();
			one = transform.position + (one - transform.position).normalized * 1.4f;
		}
		else
		{
			one = transform.position + transform.forward * 1.4f;
		}
		if (!Physics.Linecast(transform.position, one, out var hitInfo, hitLayers, QueryTriggerInteraction.Collide))
		{
			return;
		}
		DamageReciver2 component = hitInfo.collider.GetComponent<DamageReciver2>();
		if (component != null)
		{
			if (hitInfo.collider.CompareTag("BodyPart"))
			{
				WeaponsPoolManager.instance.ShowBloodParticles(hitInfo.point, hitInfo.normal);
			}
			component.Damage(damagePerHit, base.parentViewID);
		}
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private IEnumerator _003CStart_003E__BaseCallProxy0()
	{
		return base.Start();
	}
}
