using System.Collections.Generic;
using UnityEngine.Analytics;

public sealed class AnaliticsManager
{
	public static void LogGameStartConfigs(string weaponId, string skin, string hat, string shoes, string eyes, string smile)
	{
		Analytics.CustomEvent("GameStartConfigs", new Dictionary<string, object>
		{
			{ "weaponId", weaponId },
			{ "skin", skin },
			{ "hat", hat },
			{ "shoes", shoes },
			{ "eyes", eyes },
			{ "smile", smile }
		});
	}

	public static void LogBuyGood(string goodId)
	{
		Analytics.CustomEvent("BuyGood", new Dictionary<string, object> { { "goodId", goodId } });
	}

	public static void LogCurrencyCount(int Currency)
	{
		Analytics.CustomEvent("Currency", new Dictionary<string, object> { { "Currency", Currency } });
	}
}
