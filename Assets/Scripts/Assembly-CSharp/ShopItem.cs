using UnityEngine;

public class ShopItem : MonoBehaviour
{
	public Sprite icon;

	public string id;

	public int price;

	public bool isFreeGood;

	public int openingLevel;

	public int experience;

	public float scaleForShop = 1f;

	public bool IsBought
	{
		get
		{
			if (isFreeGood)
			{
				return true;
			}
			return PlayerPrefs.GetInt(id + "_") == 1;
		}
		set
		{
			PlayerPrefs.SetInt(id + "_", value ? 1 : 0);
			PlayerPrefs.Save();
		}
	}

	public bool IsLocked
	{
		get
		{
			if (IsBought)
			{
				return false;
			}
			return openingLevel > DataModel.instance.PlayerLevelIndex;
		}
	}

	public virtual void Show(bool show)
	{
		base.gameObject.SetActive(show);
	}
}
