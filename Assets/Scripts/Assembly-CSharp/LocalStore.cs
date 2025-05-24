using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalStore : MonoBehaviour
{
	public static LocalStore instance;

	private static bool initialized;

	public static Action<int> CurrencyBalanceChanged;

	public static Action IAPCompleted;

	private static int _currencyBalance;

	public static int currencyBalance
	{
		get
		{
			return PlayerPrefs.GetInt("Currency", -1);
		}
		set
		{
			_currencyBalance = value;
			PlayerPrefs.SetInt("Currency", _currencyBalance);
			if (CurrencyBalanceChanged != null)
			{
				CurrencyBalanceChanged(_currencyBalance);
			}
		}
	}

	public static bool HasEverBoughtByRealMoney
	{
		get
		{
			return PlayerPrefs.GetInt("HasEverBoughtByRealMoney") == 9;
		}
		set
		{
			PlayerPrefs.SetInt("HasEverBoughtByRealMoney", value ? 9 : 0);
		}
	}

	public static bool isAdsDisabled => HasEverBoughtByRealMoney;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		if (!initialized)
		{
			if (currencyBalance == -1)
			{
				currencyBalance = 10;
				DataModel.AddExp(1);
			}
			initialized = true;
			ShopController.instance.UpdateAfterIAP();
		}
		AnaliticsManager.LogCurrencyCount(currencyBalance);
		if (StorageController.instance.isDevelopmentBuild)
		{
			GiveMoney(9999);
		}
	}

	public void OnInitialized()
	{
		initialized = true;
		if (PlayerPrefs.GetInt("test") < 3)
		{
			PlayerPrefs.SetInt("test", 9);
		}
	}

	public void UpdateAfterIAP()
	{
		if (SceneManager.GetActiveScene().name.Equals("MainMenu") && ShopController.instance != null)
		{
			ShopController.instance.UpdateAfterIAP();
		}
	}

	public static bool BuyItem(ShopItem item)
	{
		if (item.price <= currencyBalance)
		{
			item.IsBought = true;
			currencyBalance -= item.price;
			return true;
		}
		return false;
	}

	public static void GiveMoney(int val)
	{
		currencyBalance += val;
	}

	public static bool TakeMoney(int count)
	{
		if (currencyBalance >= count)
		{
			currencyBalance -= count;
			return true;
		}
		return false;
	}
}
