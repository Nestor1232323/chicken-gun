using UnityEngine;

public class Missile : BaseBulletScript
{
	public float damageRadius = 5f;

	protected override void OnTriggerEnter(Collider collider)
	{
		if (!collider.isTrigger && (!(collider.GetComponent<PhotonView>() != null) || parentViewID != collider.GetComponent<PhotonView>().viewID))
		{
			ShowBulletBoom();
		}
	}

	public override void ShowBulletBoom()
	{
		DamageInRadius();
		WeaponsPoolManager.instance.ShowBulletHitFX(base.transform.position, Vector3.up, type);
		base.ShowBulletBoom();
	}

	protected virtual void DamageInRadius()
	{
		if (parentView != null && !parentView.isMine)
		{
			return;
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position, damageRadius);
		int i = 0;
		PhotonView photonView = null;
		for (; i < array.Length; i++)
		{
			if (array[i].GetComponent<IDamageReciver>() == null)
			{
				continue;
			}
			photonView = array[i].GetComponent<PhotonView>();
			if (photonView != null)
			{
				if (photonView.viewID != parentViewID)
				{
					array[i].GetComponent<IDamageReciver>().Damage(damage, parentViewID);
				}
			}
			else
			{
				array[i].GetComponent<IDamageReciver>().Damage(damage, parentViewID);
			}
		}
	}
}
