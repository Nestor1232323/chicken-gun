using System;
using System.Collections.Generic;
using UnityEngine;

public class DamageZome : MonoBehaviour
{
	[Serializable]
	private class TrgTarget
	{
		public float lastDmgTime;

		public DamageReciver2 damageReciver;
	}

	[SerializeField]
	protected float damagePeriod;

	[SerializeField]
	protected float damageVal;

	private List<TrgTarget> targets = new List<TrgTarget>();

	private void OnTriggerEnter(Collider collider)
	{
		targets.RemoveAll((TrgTarget t) => t.damageReciver == null);
		DamageReciver2 component = collider.GetComponent<DamageReciver2>();
		if (component != null && collider.GetComponent<PhotonView>() != null && collider.GetComponent<PhotonView>().isMine)
		{
			TrgTarget trgTarget = new TrgTarget();
			trgTarget.lastDmgTime = -10f;
			trgTarget.damageReciver = component;
			targets.Add(trgTarget);
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		targets.RemoveAll((TrgTarget t) => t.damageReciver == null);
		DamageReciver2 dm = collider.GetComponent<DamageReciver2>();
		if (dm != null && collider.GetComponent<PhotonView>() != null && collider.GetComponent<PhotonView>().isMine)
		{
			targets.RemoveAll((TrgTarget item) => item.damageReciver == dm);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		targets.ForEach(delegate(TrgTarget t)
		{
			if (t.damageReciver != null && Time.timeSinceLevelLoad - t.lastDmgTime > damagePeriod)
			{
				t.damageReciver.Damage(damageVal);
				t.lastDmgTime = Time.timeSinceLevelLoad;
			}
		});
	}
}
