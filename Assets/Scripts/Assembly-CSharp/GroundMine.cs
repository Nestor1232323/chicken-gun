using UnityEngine;

public class GroundMine : ArmGrenade
{
	protected override void Start()
	{
		if (GetComponent<PhotonView>().isMine)
		{
			parentView = GameController.instance.OurPlayer.photonView;
		}
	}

	protected override void OnTriggerEnter(Collider collider)
	{
		if (collider.GetComponent<DamageReciver2>() != null)
		{
			Explode();
		}
	}

	protected override void Explode()
	{
		WeaponsPoolManager.instance.ShowBulletHitFX(base.transform.position, Vector3.up, BulletType.Grenade);
		if (GetComponent<PhotonView>().isMine)
		{
			DamageInRadius();
			PhotonNetwork.Destroy(base.gameObject);
		}
	}
}
