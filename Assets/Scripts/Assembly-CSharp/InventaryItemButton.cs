using UnityEngine;
using UnityEngine.UI;

public class InventaryItemButton : MonoBehaviour
{
	public Image icon;

	public Text title;

	public Image lockImg;

	public void SetIcon(Sprite img)
	{
		icon.sprite = img;
	}

	public void SetTitle(string val)
	{
		title.text = val;
	}

	public void SetLocked()
	{
		lockImg.gameObject.SetActive(value: true);
	}
}
