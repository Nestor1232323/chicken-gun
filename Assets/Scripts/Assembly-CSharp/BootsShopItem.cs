using UnityEngine;

public class BootsShopItem : ShopItem
{
	[SerializeField]
	private GameObject rightShoe;

	public override void Show(bool show)
	{
		base.Show(show);
		if ((bool)rightShoe)
		{
			rightShoe.SetActive(show);
		}
	}
}
