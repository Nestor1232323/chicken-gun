using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataModel : MonoBehaviour
{
	public static DataModel instance;

	public InventoryCategory[] InventoryCategories;

	public string gameUrlAndroid;

	public string gameUrlIOS;

	public GameObject[] Players;

	public GameObject[] Weapons;

	public List<GameObject> Vehicles;

	public WheelItem[] Wheels;

	public Map[] Maps;

	public Map[] ZombieMaps;

	public GameObject[] Bodyguards;

	[SerializeField]
	private List<GunInfoMB> gunInfos;

	private Dictionary<string, GameObject> WeaponsDic = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> SkinsDic = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> HatsDic = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> ShoesDic = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> EyesDic = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> SmilesDic = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> VehiclesDic = new Dictionary<string, GameObject>();

	public Color teamA_Color;

	public Color teamB_Color;

	public Sprite teamA_Icon;

	public Sprite teamB_Icon;

	public static bool isIOS;

	public static bool isAndroid;

	private List<int> playerExpLevels = new List<int>();

	private static bool isPrefabsLoaded;

	public static Action PrefabsLoaded;

	public static Action<int> LoagingProgress;

	public static int PlayerExp => PlayerPrefs.GetInt("PlayerExp", 0);

	public int PlayerLevelIndex
	{
		get
		{
			int playerExp = PlayerExp;
			int num = -1;
			for (int i = 0; i < playerExpLevels.Count; i++)
			{
				if (playerExp <= playerExpLevels[i])
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				num = playerExpLevels.Count - 1;
			}
			return num + 1;
		}
	}

	private void Awake()
	{
		isAndroid = true;
		isIOS = false;
		playerExpLevels.Add(10);
		playerExpLevels.Add(40);
		for (int i = 2; i < 199; i++)
		{
			playerExpLevels.Add(playerExpLevels[i - 1] + (playerExpLevels[i - 1] - playerExpLevels[i - 2]));
		}
		Screen.sleepTimeout = -1;
	}

	private IEnumerator Start()
	{
		if (instance == null)
		{
			instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			yield return new WaitForSeconds(0f);
			UpdateInventoryItemsList();
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void MakeOtherTasksAfterLoading()
	{
		SetUpFirstLaunch();
		Array.Sort(Maps, (Map x, Map y) => y.order.CompareTo(x.order));
		InventoryCategory[] inventoryCategories = InventoryCategories;
		foreach (InventoryCategory inventoryCategory in inventoryCategories)
		{
			byte b = 0;
			foreach (InventoryObject item in inventoryCategory.items)
			{
				item.id = b;
				item.categoryId = inventoryCategory.id;
				item.category = inventoryCategory.categoryType;
				b++;
				item.prefabName = item.gameObject.name;
			}
		}
		GameObject[] weapons = Weapons;
		foreach (GameObject gameObject in weapons)
		{
			WeaponsDic.Add(gameObject.GetComponent<ShopItem>().id, gameObject);
		}
		foreach (GameObject vehicle in Vehicles)
		{
			VehiclesDic.Add(vehicle.GetComponent<ShopItem>().id, vehicle);
		}
	}

	private void SetUpFirstLaunch()
	{
		if (StorageController.instance.IsFirstLaunch)
		{
			ShopItem component = Players[0].GetComponent<ShopItem>();
			StorageController.instance.SelectedPlayer = component.id;
			StorageController.instance.IsFirstLaunch = false;
		}
	}

	public MapID GetRandomMap(GameMode mode)
	{
		switch (mode)
		{
		case GameMode.BattleRoyaleTeams:
			return Array.Find(Maps, (Map m) => m.BattleRoyale_TeamsCompatible).mapID;
		case GameMode.TeamFight:
			return Array.Find(Maps, (Map m) => m.teamFightCompatible).mapID;
		case GameMode.BattleRoyalePvP:
			return Array.Find(Maps, (Map m) => m.BattleRoyale_PvPCompatible).mapID;
		default:
			return Array.Find(Maps, (Map m) => m.PvPCompatible).mapID;
		}
	}

	public BaseWeaponScript.GunInfo GetGunConfig(string id)
	{
		return gunInfos.Find((GunInfoMB item) => item.gunInfo.gunId == id).gunInfo;
	}

	public List<BaseWeaponScript> GetBoughtWeapons()
	{
		List<BaseWeaponScript> list = new List<BaseWeaponScript>();
		GameObject[] weapons = Weapons;
		foreach (GameObject gameObject in weapons)
		{
			if (gameObject.GetComponent<ShopItem>().IsBought)
			{
				list.Add(gameObject.GetComponent<BaseWeaponScript>());
			}
		}
		return list;
	}

	public GameObject GetWeaponById(string id)
	{
		return WeaponsDic[id];
	}

	public GameObject GetSkinById(string id)
	{
		return SkinsDic[id];
	}

	public GameObject GetHatById(string id)
	{
		return HatsDic[id];
	}

	public GameObject GetShoesById(string id)
	{
		return ShoesDic[id];
	}

	public GameObject GetEyeById(string id)
	{
		return EyesDic[id];
	}

	public GameObject GetSmileById(string id)
	{
		return SmilesDic[id];
	}

	public GameObject GetVehicleId(string id)
	{
		return VehiclesDic[id];
	}

	public GameObject GetPlayerId(string id)
	{
		if (id == string.Empty)
		{
			return Players[0];
		}
		return Array.Find(Players, (GameObject p) => p.GetComponent<ShopItem>().id == id);
	}

	public GameObject GetWheelByType(WheelType type)
	{
		return Wheels[0].prefab;
	}

	public Map GetMap(MapID id)
	{
		Map map = Array.Find(Maps, (Map map2) => map2.mapID == id);
		if (map == null)
		{
			map = Array.Find(ZombieMaps, (Map map2) => map2.mapID == id);
		}
		return map;
	}

	public int WeaponIndexById(string weaponId)
	{
		int num = 0;
		foreach (KeyValuePair<string, GameObject> item in WeaponsDic)
		{
			if (item.Key == weaponId)
			{
				return num;
			}
			num++;
		}
		return num;
	}

	public Color TeamColor(TeamID team)
	{
		switch (team)
		{
		case TeamID.TeamA:
			return teamA_Color;
		case TeamID.TeamB:
			return teamB_Color;
		default:
			return Color.white;
		}
	}

	public Sprite TeamIcon(TeamID team)
	{
		switch (team)
		{
		case TeamID.TeamA:
			return teamA_Icon;
		case TeamID.TeamB:
			return teamB_Icon;
		default:
			return null;
		}
	}

	public static void AddExp(int val)
	{
		PlayerPrefs.SetInt("PlayerExp", PlayerExp + val);
		GPSController.PostScoreToLeaderboard(PlayerExp);
	}

	public int CurrentLevelMaxExp()
	{
		return playerExpLevels[PlayerLevelIndex - 1];
	}

	public void UpdateInventoryItemsList()
	{
		if (!isPrefabsLoaded)
		{
			StartCoroutine(LoadPrefabs());
		}
	}

	private IEnumerator LoadPrefabs()
	{
		isPrefabsLoaded = true;
		MakeOtherTasksAfterLoading();
		if (PrefabsLoaded != null)
		{
			PrefabsLoaded();
		}
		yield return null;
	}

	public InventoryCategory GetInvCategory(InventoryCategoryType type)
	{
		return Array.Find(InventoryCategories, (InventoryCategory cat) => cat.categoryType == type);
	}

	public GameObject GetPetByShopId(string shopID, InventoryCategoryType category = InventoryCategoryType.Pets)
	{
		foreach (InventoryObject item in Array.Find(InventoryCategories, (InventoryCategory ctg) => ctg.categoryType == category).items)
		{
			if (item.GetComponent<ShopItem>() != null && item.GetComponent<ShopItem>().id == shopID)
			{
				return item.gameObject;
			}
		}
		return null;
	}

	public GameObject GetGuardByShopId(string shopID)
	{
		return Array.Find(Bodyguards, (GameObject ctg) => ctg.GetComponent<ShopItem>().id == shopID);
	}

	public GameObject GetInvObjectByShopId(string shopID)
	{
		InventoryCategory[] inventoryCategories = InventoryCategories;
		for (int i = 0; i < inventoryCategories.Length; i++)
		{
			foreach (InventoryObject item in inventoryCategories[i].items)
			{
				ShopItem component = item.GetComponent<ShopItem>();
				if (component != null && component.id == shopID)
				{
					return component.gameObject;
				}
			}
		}
		return null;
	}

	public string BodyguardPrefabName(string guardId)
	{
		return Array.Find(Bodyguards, (GameObject bg) => bg.GetComponent<ShopItem>().id == guardId).name;
	}

	public MapID GetMapForMode(GameMode mode)
	{
		return Array.Find(Maps, (Map map) => map.compModes.Contains(mode)).mapID;
	}
}
