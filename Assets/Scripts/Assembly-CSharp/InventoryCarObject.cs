using UnityEngine;

public class InventoryCarObject : InventoryObject
{
	public Vehicle myCar;

	private Vehicle MyCar
	{
		get
		{
			if (myCar == null)
			{
				myCar = GetComponent<Vehicle>();
			}
			return myCar;
		}
	}

	public override bool isSelected
	{
		get
		{
			if (MyCar.isBusyByPlayer)
			{
				return true;
			}
			return base.isSelected;
		}
		set
		{
			base.isSelected = value;
		}
	}

	public override Sprite icon => GetComponent<ShopItem>().icon;

	protected override void Start()
	{
		base.Start();
		myCar = GetComponent<CarController>();
	}

	public override void Select()
	{
		if (isSelectable)
		{
			isSelected = true;
		}
	}

	public override void Select(int senderViewId)
	{
		if (isSelectable)
		{
			isSelected = true;
			if (PhotonView.Find(senderViewId) != null)
			{
				MyCar.EnablePhysics(enable: false);
				rb.isKinematic = true;
				rb.useGravity = false;
			}
		}
	}

	public override void Deselect()
	{
		isSelected = false;
	}

	public override void Deselect(int senderViewId)
	{
		isSelected = false;
		PhotonView photonView = PhotonView.Find(senderViewId);
		if (!(photonView != null))
		{
			return;
		}
		if (photonView.isMine)
		{
			MyCar.EnablePhysics(enable: true);
			if (rb != null)
			{
				rb.isKinematic = false;
				rb.useGravity = true;
			}
		}
		else if (rb != null)
		{
			rb.isKinematic = false;
			rb.useGravity = true;
		}
	}
}
