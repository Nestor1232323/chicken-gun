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

	public override Sprite icon
	{
		get
		{
			return GetComponent<ShopItem>().icon;
		}
	}

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
			PhotonView photonView = PhotonView.Find(senderViewId);
			if (photonView != null)
			{
				MyCar.EnablePhysics(false);
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
			MyCar.EnablePhysics(true);
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
