using Photon;
using UnityEngine;

public class LootBox : Photon.MonoBehaviour, IDestroyable
{
	[SerializeField]
	private GameObject crate;

	[SerializeField]
	private ParticleSystem explodeFX;

	private int hp = 1;

	public void ApplyDamage(float val, int fromWhom)
	{
		if (hp != 0)
		{
			hp = 0;
			crate.SetActive(value: false);
			explodeFX.Play();
			AmmoGenerator.instance.OnBoomLootBox(base.transform.position);
			Object.Destroy(base.gameObject, 1f);
		}
	}

	public void ApplyHeal(float val)
	{
	}
}
