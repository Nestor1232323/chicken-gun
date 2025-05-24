using System.Collections;
using UnityEngine;

public class GunBullet : BaseBulletScript
{
	[SerializeField]
	private TrailRenderer tr;

	[SerializeField]
	protected bool makesHole = true;

	protected bool showHole;

	protected bool showBloodFX;

	protected IDamageReciver targetDamageReciver;

	[SerializeField]
	protected float recycleDelay = -1f;

	protected override void Update()
	{
		base.Update();
		CheckLifeTime();
	}

	public override void OnSpawn()
	{
		base.enabled = true;
		if (tr != null)
		{
			tr.Clear();
		}
	}

	public override void OnRecycle()
	{
		base.OnRecycle();
		targetDamageReciver = null;
	}

	private void OnEnable()
	{
	}

	public override void SetDamage(float val)
	{
		base.SetDamage(val);
		RaycastForTarget();
	}

	protected override void RaycastForTarget()
	{
		targetDamageReciver = null;
		Debug.DrawLine(base.transform.position, base.transform.position + base.transform.forward * 250f, Color.red);
		showHole = true;
		showBloodFX = false;
		if (!Physics.Linecast(base.transform.position, base.transform.position + base.transform.forward * 250f, out var hitInfo, targetLayers, QueryTriggerInteraction.Collide))
		{
			return;
		}
		if (hitInfo.collider.GetComponent<IDamageReciver>() != null)
		{
			showHole = false;
			if (hitInfo.collider.CompareTag("BodyPart"))
			{
				showBloodFX = true;
				if (isOriginal)
				{
					targetDamageReciver = hitInfo.collider.GetComponent<IDamageReciver>();
					HUDManager.instance.ShowTextDamage(damage, hitInfo.point);
				}
			}
			else
			{
				targetDamageReciver = hitInfo.collider.GetComponent<IDamageReciver>();
			}
		}
		StartCoroutine(DamageTargetAndBoomIE((hitInfo.point - base.transform.position).magnitude / speed, hitInfo, showHole, showBloodFX));
	}

	private IEnumerator DamageTargetAndBoomIE(float t, RaycastHit hitInfo, bool showHo, bool showBloodF)
	{
		yield return new WaitForSeconds(t);
		if (isOriginal && targetDamageReciver != null)
		{
			targetDamageReciver.Damage(damage, parentViewID);
		}
		ShowBulletBoom(hitInfo, showHo, showBloodF);
		if (recycleDelay < 0f)
		{
			yield return null;
		}
		else
		{
			if (hitInfo.collider != null)
			{
				base.transform.SetParent(hitInfo.collider.transform);
			}
			base.enabled = false;
			base.transform.position = hitInfo.point;
			yield return new WaitForSeconds(recycleDelay);
		}
		base.gameObject.Recycle();
	}

	public virtual void ShowBulletBoom(RaycastHit hitInfo, bool showHole, bool showBlood)
	{
		if (showBlood)
		{
			WeaponsPoolManager.instance.ShowBloodParticles(hitInfo.point, hitInfo.normal);
		}
		else
		{
			WeaponsPoolManager.instance.ShowBulletHitFX(hitInfo.point, hitInfo.normal, type);
		}
		if (showHole && makesHole)
		{
			WeaponsPoolManager.instance.ShowBulletHole(hitInfo, showBlood);
		}
	}
}
