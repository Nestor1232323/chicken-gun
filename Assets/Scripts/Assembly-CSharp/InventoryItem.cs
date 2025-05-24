using System;
using Photon;
using UnityEngine;

public class InventoryItem : Photon.MonoBehaviour, IDestroyable
{
	public byte id;

	[HideInInspector]
	public byte categoryId;

	[HideInInspector]
	public InventoryCategoryType category;

	[SerializeField]
	protected float hp = 50f;

	[HideInInspector]
	public Vector3 pos;

	[HideInInspector]
	public Vector3 rot;

	public int order;

	public string _prefabName;

	public string prefabName
	{
		get
		{
			if (string.IsNullOrEmpty(_prefabName))
			{
				string text = base.gameObject.name;
				text.Replace("(Clone)", string.Empty);
				text.Replace(" ", string.Empty);
				return text;
			}
			return _prefabName;
		}
		set
		{
			_prefabName = value;
		}
	}

	public string CategoryFolderName => Array.Find(DataModel.instance.InventoryCategories, (InventoryCategory ctg) => ctg.id == categoryId).name;

	public string FullPathToPrefab => "Prefabs/Inventory/" + CategoryFolderName + "/" + prefabName;

	public virtual void ApplyDamage(float val, int fromWhom)
	{
		hp -= val;
		if (hp <= 0f)
		{
			PhotonNetwork.RPC(base.photonView, "DestroySelfR", PhotonTargets.All, false);
		}
	}

	public void ApplyHeal(float val)
	{
	}

	[PunRPC]
	protected virtual void DestroySelfR()
	{
		if (base.photonView.isMine)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
	}
}
