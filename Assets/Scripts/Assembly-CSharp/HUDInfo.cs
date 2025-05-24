using UnityEngine;
using UnityEngine.UI;

public class HUDInfo : MonoBehaviour
{
	public Text item_name;

	public RawImage armorCountImage;

	public void UpdateView(float curHP, float maxHP)
	{
		float x = armorCountImage.rectTransform.sizeDelta.x;
		float num = curHP / maxHP;
		float x2 = 0f - x + x * num;
		armorCountImage.rectTransform.anchoredPosition = new Vector2(x2, armorCountImage.rectTransform.anchoredPosition.y);
		if (num < 0.25f)
		{
			armorCountImage.color = new Color(0.9f, 0f, 0f);
		}
		else if (num < 0.5f)
		{
			armorCountImage.color = new Color(0.9f, 0.6f, 0f);
		}
		else
		{
			armorCountImage.color = new Color(0.1f, 0.9f, 0f);
		}
	}

	public void SetName(string str)
	{
		item_name.text = str;
	}

	private void Update()
	{
	}

	public void Show(bool show)
	{
		base.gameObject.SetActive(show);
	}
}
