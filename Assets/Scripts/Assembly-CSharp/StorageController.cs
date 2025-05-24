using System;
using UnityEngine;

public class StorageController : MonoBehaviour
{
	[Serializable]
	public class GameConfigData
	{
		public bool isOnlineModeSelected;

		public ArenaID arenaID;
	}

	public static GameConfigData gameConfigData;

	public static StorageController instance;

	public static float CarSpeedCoeff = 4.1f;

	public bool isDevelopmentBuild;

	private string[] RandomNamesEN = new string[5] { "KoKoKo", "General KoKoKo", "Angry Rooster", "Boss", "Rudy" };

	private string[] RandomNamesRU = new string[8] { "Петушок", "Генерал КоКоКо", "Сержант Кукареку", "Злой Петух", "Апасный тип", "Перелетный Петух", "Босс", "Директор курятника" };

	public bool IsFirstLaunch
	{
		get
		{
			return PlayerPrefs.GetInt("IsFirstLaunch", 0) == 0;
		}
		set
		{
			PlayerPrefs.SetInt("IsFirstLaunch", (!value) ? 1 : 0);
		}
	}

	public string SelectedArenaId
	{
		get
		{
			return PlayerPrefs.GetString("SelectedArenaId");
		}
		set
		{
			PlayerPrefs.SetString("SelectedArenaId", value);
		}
	}

	public int IsVibroAllowed
	{
		get
		{
			return PlayerPrefs.GetInt("IsVibroAllowed");
		}
		set
		{
			PlayerPrefs.SetInt("IsVibroAllowed", value);
		}
	}

	public string PlayerName
	{
		get
		{
			if (string.IsNullOrEmpty(PlayerPrefs.GetString("PlayerName")))
			{
				PlayerName = GetRandoName();
				return PlayerName;
			}
			return PlayerPrefs.GetString("PlayerName");
		}
		set
		{
			PlayerPrefs.SetString("PlayerName", value);
		}
	}

	public string SelectedPlayer
	{
		get
		{
			return PlayerPrefs.GetString("SelectedPlayer");
		}
		set
		{
			PlayerPrefs.SetString("SelectedPlayer", value);
		}
	}

	public static string SelectedHat
	{
		get
		{
			return PlayerPrefs.GetString("Hat");
		}
		set
		{
			PlayerPrefs.SetString("Hat", value);
		}
	}

	public static string SelectedEyes
	{
		get
		{
			return PlayerPrefs.GetString("Eyes");
		}
		set
		{
			PlayerPrefs.SetString("Eyes", value);
		}
	}

	public static string SelectedSmile
	{
		get
		{
			return PlayerPrefs.GetString("Smile");
		}
		set
		{
			PlayerPrefs.SetString("Smile", value);
		}
	}

	public static string SelectedShoes
	{
		get
		{
			return PlayerPrefs.GetString("Shoes");
		}
		set
		{
			PlayerPrefs.SetString("Shoes", value);
		}
	}

	public string SelectedGun
	{
		get
		{
			return PlayerPrefs.GetString("SelectedGun");
		}
		set
		{
			PlayerPrefs.SetString("SelectedGun", value);
		}
	}

	public int survivalScore
	{
		get
		{
			return PlayerPrefs.GetInt("survival_score");
		}
		set
		{
			if (value > PlayerPrefs.GetInt("survival_score"))
			{
				PlayerPrefs.SetInt("survival_score", value);
			}
		}
	}

	public static bool AutoshootEnabled
	{
		get
		{
			return PlayerPrefs.GetInt("Autoshoot") == 1;
		}
		set
		{
			PlayerPrefs.SetInt("Autoshoot", value ? 1 : 0);
		}
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			UnityEngine.Object.DontDestroyOnLoad(this);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		if (!PlayerPrefs.HasKey("IsVibroAllowed"))
		{
			IsVibroAllowed = 1;
		}
		SelectedArenaId = ArenaID.Location1.ToString();
	}

	private string GetRandoName()
	{
		_ = string.Empty;
		if (Application.systemLanguage == SystemLanguage.Russian)
		{
			return RandomNamesRU[UnityEngine.Random.Range(0, RandomNamesRU.Length)];
		}
		return RandomNamesEN[UnityEngine.Random.Range(0, RandomNamesEN.Length)];
	}

	public void SetTempWeaponAmmo(string weaponid, int value)
	{
		PlayerPrefs.SetInt(weaponid, value);
	}

	public int GetTempWeaponAmmo(string weaponid)
	{
		return PlayerPrefs.GetInt(weaponid);
	}

	public void IncreaseWinsCount()
	{
		PlayerPrefs.SetInt("mult_score", PlayerPrefs.GetInt("mult_score") + 1);
	}

	public string GetPlayerSelectedGun(string playerId)
	{
		return PlayerPrefs.GetString(playerId + "_gun");
	}

	public void SetPlayerSelectedGun(string playerId, string gunId)
	{
		PlayerPrefs.SetString(playerId + "_gun", gunId);
	}

	public static void SetGunUpgradeLevel(string gunId, int level)
	{
		PlayerPrefs.SetInt(gunId + "_level", level);
	}

	public static int GetGunUpgradeLevel(string gunId)
	{
		return PlayerPrefs.GetInt(gunId + "_level", 0);
	}

	public static int GetPlayerTextureIndex(string playerId)
	{
		return PlayerPrefs.GetInt(playerId + "TextureIndex", 0);
	}

	public static void SetPlayerTextureIndex(GameObject player, int val)
	{
		PlayerPrefs.SetInt(player.GetComponent<ShopItem>().id + "TextureIndex", val);
	}
}
