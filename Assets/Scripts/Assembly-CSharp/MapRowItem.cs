using UnityEngine;
using UnityEngine.UI;

public class MapRowItem : MonoBehaviour
{
	public Image icon;

	public Text nameText;

	public void SetName(string name)
	{
		nameText.text = name;
	}

	public void SetIcon(Sprite icon)
	{
		this.icon.sprite = icon;
	}
}
