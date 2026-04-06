using Photon;
using UnityEngine;

public class ExplodableObject : Photon.MonoBehaviour, IDestroyable
{
	[SerializeField]
	private float _hp = 40f;

	public virtual void ApplyDamage(float val, int fromWhom)
	{
		if (!(_hp <= 0f))
		{
			_hp -= val;
			if (_hp <= 0f)
			{
				PhotonNetwork.RPC(base.photonView, "Explode", PhotonTargets.All, false);
			}
		}
	}

	public virtual void ApplyHeal(float val)
	{
		_hp += val;
	}

	[PunRPC]
	private void Explode()
	{
		Object.Instantiate(WeaponsPoolManager.instance.carExplosion, base.transform.position, base.transform.rotation);
		Invoke("Destroy", 2f);
	}

	private void Destroy()
	{
		if ((bool)base.gameObject && base.photonView.isMine)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
	}
}
