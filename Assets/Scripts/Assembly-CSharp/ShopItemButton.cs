using UnityEngine;
using UnityEngine.UI;

public class ShopItemButton : MonoBehaviour
{
	public Image icon;

	public Image lockImg;

	public Text openeingLevelText;

	public void SetData(ShopItem item)
	{
		icon.sprite = item.icon;
		lockImg.gameObject.SetActive(item.openingLevel >= DataModel.instance.PlayerLevelIndex);
		openeingLevelText.text = item.openingLevel.ToString();
	}
}
