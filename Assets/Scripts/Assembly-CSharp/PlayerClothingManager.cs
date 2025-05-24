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

	public List<ShopItem> AllHats => new List<ShopItem>(hatsContainer.GetComponentsInChildren<ShopItem>(includeInactive: true));

	public List<ShopItem> AllShoes => new List<ShopItem>(shoesContainer.GetComponentsInChildren<ShopItem>(includeInactive: true));

	public List<ShopItem> AllBeaks => new List<ShopItem>(beaksContainer.GetComponentsInChildren<ShopItem>(includeInactive: true));

	public List<ShopItem> AllEyes => new List<ShopItem>(eyesContainer.GetComponentsInChildren<ShopItem>(includeInactive: true));

	public List<ShopItem> AllGuns => new List<ShopItem>(gunsContainer.GetComponentsInChildren<ShopItem>(includeInactive: true));

	public void WearHat(string hatId)
	{
		if (hatsContainer == null)
		{
			return;
		}
		ShopItem shopItem = Array.Find(hatsContainer.GetComponentsInChildren<ShopItem>(includeInactive: true), (ShopItem i) => i.id == hatId);
		if (shopItem != null)
		{
			if (lastHat != null)
			{
				lastHat.Show(show: false);
			}
			lastHat = shopItem;
			shopItem.Show(show: true);
		}
	}

	public void WearShoes(string shoesId)
	{
		if (shoesContainer == null)
		{
			return;
		}
		ShopItem shopItem = Array.Find(shoesContainer.GetComponentsInChildren<ShopItem>(includeInactive: true), (ShopItem i) => i.id == shoesId);
		if (shopItem != null)
		{
			if (lastShoes != null)
			{
				lastShoes.Show(show: false);
			}
			lastShoes = shopItem;
			shopItem.Show(show: true);
		}
	}

	public void WearEyes(string id)
	{
		if (eyesContainer == null)
		{
			return;
		}
		ShopItem shopItem = Array.Find(eyesContainer.GetComponentsInChildren<ShopItem>(includeInactive: true), (ShopItem i) => i.id == id);
		if (shopItem != null)
		{
			if (lastEyes != null)
			{
				lastEyes.Show(show: false);
			}
			lastEyes = shopItem;
			shopItem.Show(show: true);
		}
	}

	public void WearSmile(string shoesId)
	{
		if (beaksContainer == null)
		{
			return;
		}
		ShopItem shopItem = Array.Find(beaksContainer.GetComponentsInChildren<ShopItem>(includeInactive: true), (ShopItem i) => i.id == shoesId);
		if (shopItem != null)
		{
			if (lastBeak != null)
			{
				lastBeak.Show(show: false);
			}
			lastBeak = shopItem;
			shopItem.Show(show: true);
		}
	}

	public void SetUpBody(bool isTerr, int viewId)
	{
	}

	public void HideHead()
	{
		HeadContainer.gameObject.SetActive(value: false);
	}

	public void HideAll(bool hide)
	{
		HeadContainer.parent.parent.GetChild(0).gameObject.SetActive(!hide);
		HeadContainer.parent.parent.GetChild(1).gameObject.SetActive(!hide);
	}
}
