using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTypeSelectScreen : BaseScreen
{
	public Text playersCountLabel;

	public InputField gameTitleInput;

	public InputField passwordInput;

	public GameObject MapItemButtonPrefab;

	public Transform mapsContainer;

	private int playersCount = 10;

	private int maxPlayersCount;

	public GameModeSelectionMenu gameModeSelectionMenu;

	private List<MapItemButton> mapItems = new List<MapItemButton>();

	private void Start()
	{
		playersCountLabel.text = playersCount.ToString();
		MultiplayerController instance = MultiplayerController.instance;
		instance.PhotonCreateRoomFailed = (Action)Delegate.Combine(instance.PhotonCreateRoomFailed, new Action(OnPhotonCreateRoomFailed));
	}

	private void OnDestroy()
	{
		MultiplayerController instance = MultiplayerController.instance;
		instance.PhotonCreateRoomFailed = (Action)Delegate.Remove(instance.PhotonCreateRoomFailed, new Action(OnPhotonCreateRoomFailed));
	}

	protected override void OnShow()
	{
		base.OnShow();
		gameModeSelectionMenu.OnGameModeSelected = GameModeSelected;
		CreateMapsList();
		maxPlayersCount = ((MultiplayerController.gameType == GameMode.ZombieSurvival) ? 5 : 10);
		if (MultiplayerController.gameType == GameMode.ZombieSurvival)
		{
			playersCountLabel.text = maxPlayersCount.ToString();
		}
	}

	private void OnPhotonCreateRoomFailed()
	{
		ScreenManager.instance.ShowLoading(show: false);
	}

	private void GameModeSelected(GameMode mode)
	{
		MultiplayerController.gameType = mode;
		CreateMapsList();
	}

	public void PlusUpPlayersCount()
	{
		playersCount++;
		if (playersCount > maxPlayersCount)
		{
			playersCount = maxPlayersCount;
		}
		playersCountLabel.text = playersCount.ToString();
	}

	public void MinusUpPlayersCount()
	{
		playersCount--;
		if (playersCount < 1)
		{
			playersCount = 1;
		}
		playersCountLabel.text = playersCount.ToString();
	}

	public void CreateGameBtnClick()
	{
		ScreenManager.instance.ShowLoading(show: true);
		string text = gameTitleInput.text;
		if (text == string.Empty)
		{
			text = "Game" + UnityEngine.Random.Range(1, 9999);
		}
		else if (text.Length > 22)
		{
			text = text.Substring(0, 22);
		}
		string text2 = passwordInput.text;
		MultiplayerController.instance.CreateRoom(text, playersCount, text2);
	}

	private void CreateMapsList()
	{
		for (int num = mapItems.Count - 1; num >= 0; num--)
		{
			mapItems[num].gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
			UnityEngine.Object.Destroy(mapItems[num].gameObject);
		}
		mapItems.Clear();
		Map[] array = DataModel.instance.Maps;
		if (MultiplayerController.gameType == GameMode.PvP)
		{
			array = Array.FindAll(DataModel.instance.Maps, (Map m) => m.PvPCompatible);
		}
		else if (MultiplayerController.gameType == GameMode.TeamFight)
		{
			array = Array.FindAll(DataModel.instance.Maps, (Map m) => m.teamFightCompatible);
		}
		else if (MultiplayerController.gameType == GameMode.BattleRoyalePvP)
		{
			array = Array.FindAll(DataModel.instance.Maps, (Map m) => m.BattleRoyale_PvPCompatible);
		}
		else if (MultiplayerController.gameType == GameMode.BattleRoyaleTeams)
		{
			array = Array.FindAll(DataModel.instance.Maps, (Map m) => m.BattleRoyale_TeamsCompatible);
		}
		bool flag = true;
		Map[] array2 = array;
		foreach (Map map in array2)
		{
			MapItemButton mapItem = UnityEngine.Object.Instantiate(MapItemButtonPrefab).GetComponent<MapItemButton>();
			mapItem.SetIcon(map.icon);
			mapItem.SetName(map.name);
			mapItem.transform.SetParent(mapsContainer);
			mapItem.transform.localScale = Vector3.one;
			mapItems.Add(mapItem);
			MapID mapId = map.mapID;
			mapItem.GetComponent<Button>().onClick.AddListener(delegate
			{
				mapItems.ForEach(delegate(MapItemButton item)
				{
					item.Select(select: false);
				});
				mapItem.Select(select: true);
				MultiplayerController.instance.selectedMapID = mapId;
			});
			if (map.mapID == MultiplayerController.instance.selectedMapID)
			{
				mapItem.Select(select: true);
				flag = false;
			}
		}
		if (flag)
		{
			mapItems[0].Select(select: true);
			MultiplayerController.instance.selectedMapID = array[0].mapID;
		}
	}
}
