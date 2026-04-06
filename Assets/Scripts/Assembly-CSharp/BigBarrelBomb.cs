using UnityEngine;

public class BigBarrelBomb : ArmGrenade
{
	protected override void Start()
	{
		base.transform.rotation = Quaternion.identity;
		Invoke("Explode", 3.23f);
	}

	public override void Throw(float strength, float torque)
	{
		Rigidbody component = GetComponent<Rigidbody>();
		component.velocity = base.transform.forward * 15f;
	}

	protected override void Explode()
	{
		WeaponsPoolManager.instance.ShowBulletHitFX(base.transform.position, Vector3.up, BulletType.Grenade);
		DamageInRadius();
		DestroyInRadius();
		OnRecycle();
	}

	private void DestroyInRadius()
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position, damageRadius);
		int num = 0;
		while (num < array.Length)
		{
			if (array[num].GetComponent<InventoryObject>() != null && !array[num].GetComponent<InventoryObject>().isSelected)
			{
				if (array[num].GetComponent<Vehicle>() != null && array[num].GetComponent<Vehicle>().isBusyByPlayer)
				{
					continue;
				}
				PhotonNetwork.Destroy(array[num].gameObject);
			}
			num++;
		}
	}
}
