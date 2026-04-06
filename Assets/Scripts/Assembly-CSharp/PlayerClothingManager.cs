using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClothingManager : MonoBehaviour
{
	[SerializeField]
	private Transform hatsContainer;

	[SerializeField]
	private Transform shoesContainer;

	[SerializeField]
	private Transform gunsContainer;

	[SerializeField]
	private Transform beaksContainer;

	[SerializeField]
	private Transform eyesContainer;

	[SerializeField]
	private ShopItem lastHat;

	[SerializeField]
	private ShopItem lastShoes;

	[SerializeField]
	private ShopItem lastEyes;

	[SerializeField]
	private ShopItem lastBeak;

	[SerializeField]
	private Transform HeadContainer;

	public List<ShopItem> AllHats
	{
		get
		{
			return new List<ShopItem>(hatsContainer.GetComponentsInChildren<ShopItem>(true));
		}
	}

	public List<ShopItem> AllShoes
	{
		get
		{
			return new List<ShopItem>(shoesContainer.GetComponentsInChildren<ShopItem>(true));
		}
	}

	public List<ShopItem> AllBeaks
	{
		get
		{
			return new List<ShopItem>(beaksContainer.GetComponentsInChildren<ShopItem>(true));
		}
	}

	public List<ShopItem> AllEyes
	{
		get
		{
			return new List<ShopItem>(eyesContainer.GetComponentsInChildren<ShopItem>(true));
		}
	}

	public List<ShopItem> AllGuns
	{
		get
		{
			return new List<ShopItem>(gunsContainer.GetComponentsInChildren<ShopItem>(true));
		}
	}

	public void WearHat(string hatId)
	{
		if (hatsContainer == null)
		{
			return;
		}
		ShopItem[] componentsInChildren = hatsContainer.GetComponentsInChildren<ShopItem>(true);
		ShopItem shopItem = Array.Find(componentsInChildren, (ShopItem i) => i.id == hatId);
		if (shopItem != null)
		{
			if (lastHat != null)
			{
				lastHat.Show(false);
			}
			lastHat = shopItem;
			shopItem.Show(true);
		}
	}

	public void WearShoes(string shoesId)
	{
		if (shoesContainer == null)
		{
			return;
		}
		ShopItem[] componentsInChildren = shoesContainer.GetComponentsInChildren<ShopItem>(true);
		ShopItem shopItem = Array.Find(componentsInChildren, (ShopItem i) => i.id == shoesId);
		if (shopItem != null)
		{
			if (lastShoes != null)
			{
				lastShoes.Show(false);
			}
			lastShoes = shopItem;
			shopItem.Show(true);
		}
	}

	public void WearEyes(string id)
	{
		if (eyesContainer == null)
		{
			return;
		}
		ShopItem[] componentsInChildren = eyesContainer.GetComponentsInChildren<ShopItem>(true);
		ShopItem shopItem = Array.Find(componentsInChildren, (ShopItem i) => i.id == id);
		if (shopItem != null)
		{
			if (lastEyes != null)
			{
				lastEyes.Show(false);
			}
			lastEyes = shopItem;
			shopItem.Show(true);
		}
	}

	public void WearSmile(string shoesId)
	{
		if (beaksContainer == null)
		{
			return;
		}
		ShopItem[] componentsInChildren = beaksContainer.GetComponentsInChildren<ShopItem>(true);
		ShopItem shopItem = Array.Find(componentsInChildren, (ShopItem i) => i.id == shoesId);
		if (shopItem != null)
		{
			if (lastBeak != null)
			{
				lastBeak.Show(false);
			}
			lastBeak = shopItem;
			shopItem.Show(true);
		}
	}

	public void SetUpBody(bool isTerr, int viewId)
	{
	}

	public void HideHead()
	{
		HeadContainer.gameObject.SetActive(false);
	}

	public void HideAll(bool hide)
	{
		HeadContainer.parent.parent.GetChild(0).gameObject.SetActive(!hide);
		HeadContainer.parent.parent.GetChild(1).gameObject.SetActive(!hide);
	}
}
