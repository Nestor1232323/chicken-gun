using UnityEngine;

public class MeleeAttackCollider : MonoBehaviour
{
	private BaseMeleeWeapon melee;

	private void Awake()
	{
		melee = GetComponentInParent<BaseMeleeWeapon>();
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (melee.parentPhotonView.isMine && collider.GetComponent<DamageReciver2>() != null && collider.GetComponent<PhotonView>().viewID != melee.parentViewID)
		{
			collider.GetComponent<DamageReciver2>().Damage(melee.damagePerHit, melee.parentViewID);
		}
	}
}
