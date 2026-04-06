using UnityEngine;
using UnityEngine.UI;

public class MapItemButton : MonoBehaviour
{
	public Image icon;

	public Text nameLabel;

	public Image selectionImg;

	public void Select(bool select)
	{
		selectionImg.gameObject.SetActive(select);
	}

	public void SetName(string val)
	{
		nameLabel.text = val;
	}

	public void SetIcon(Sprite img)
	{
		icon.sprite = img;
	}
}
