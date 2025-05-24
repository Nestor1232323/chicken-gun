using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadSavedGameScreen : BaseScreen
{
	public GameObject rowPrefab;

	public RectTransform container;

	public GameModeSelectionMenu gameModeSelectionMenu;

	private List<GameObject> rows = new List<GameObject>();

	private void Start()
	{
		gameModeSelectionMenu.OnGameModeSelected = GameModeSelected;
		CreateListOfSavedGames();
	}

	private void CreateListOfSavedGames()
	{
		foreach (GameObject row in rows)
		{
			Array.ForEach(row.GetComponentsInChildren<Button>(), delegate(Button bt)
			{
				bt.onClick.RemoveAllListeners();
			});
		}
		GeneralUtils.RemoveAllChilds(container.transform);
		rows.Clear();
		string[] savedGamesList = GameSavingManager.GetSavedGamesList();
		foreach (string text in savedGamesList)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(rowPrefab);
			gameObject.transform.SetParent(container);
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.GetChild(0).GetComponent<Text>().text = text;
			string buf = text;
			gameObject.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate
			{
				MonoBehaviour.print(buf);
				GameSavingManager.RemoveGame(buf);
				CreateListOfSavedGames();
			});
			gameObject.GetComponent<Button>().onClick.AddListener(delegate
			{
				ScreenManager.instance.ShowLoading(show: true);
				SavedGameInfo savedGame = GameSavingManager.GetSavedGame(buf);
				MonoBehaviour.print("sceneName = " + savedGame.sceneName);
				MultiplayerController.instance.selectedMapID = (MapID)Enum.Parse(typeof(MapID), savedGame.sceneName, ignoreCase: true);
				GameController.gameConfigData.savedGame = savedGame;
				MultiplayerController.instance.CreateRoom(buf, 10, isSavedGame: true);
			});
			rows.Add(gameObject);
		}
	}

	private void GameModeSelected(GameMode mode)
	{
		MultiplayerController.gameType = mode;
	}
}
