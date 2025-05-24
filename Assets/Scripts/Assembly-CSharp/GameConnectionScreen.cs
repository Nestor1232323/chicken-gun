using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameConnectionScreen : BaseScreen
{
	public static GameConnectionScreen instance;

	public Text photonDebugLog;

	public GameObject RoomRowPrefab;

	public RectTransform mapsScrollContent;

	public RectTransform avaiableRoomsScrollContent;

	public InputField newDeathMatchGameNameInput;

	public InputField newTFGameNameInput;

	public GameObject NewDeathmatchGamePanel;

	public GameObject NewTeamFightGamePanel;

	public GameObject LoadingPanel;

	public GameObject NewGameCreationFailedPanel;

	public Transform CameraPosForThisScreen;

	public GameObject FailedToJoinRoomWindow;

	[SerializeField]
	private Text roomsPageIndexLbl;

	[SerializeField]
	private Text maxRoomsCountLbl;

	private bool roomListInitialized;

	private float prevUpdTime = -1f;

	private int listLength = 17;

	private List<AvaibleRoomRowItem> avaiableRoomsRowItems = new List<AvaibleRoomRowItem>();

	[SerializeField]
	private EnterPasswordPanel roomPasswordWindow;

	private int pageIndex;

	public override void Awake()
	{
		base.Awake();
		instance = this;
	}

	private void Start()
	{
		MultiplayerController multiplayerController = MultiplayerController.instance;
		multiplayerController.RoomListUpdated = (Action)Delegate.Combine(multiplayerController.RoomListUpdated, new Action(OnRoomListUpdate));
		MultiplayerController multiplayerController2 = MultiplayerController.instance;
		multiplayerController2.PhotonJoinRoomFailed = (Action)Delegate.Combine(multiplayerController2.PhotonJoinRoomFailed, new Action(OnPhotonJoinRoomFailed));
		MultiplayerController multiplayerController3 = MultiplayerController.instance;
		multiplayerController3.ConnectionFailed = (Action)Delegate.Combine(multiplayerController3.ConnectionFailed, new Action(OnConnectionToPhotonFailed));
	}

	protected override void OnShow()
	{
		base.OnShow();
		LoadingPanel.SetActive(value: true);
		MultiplayerController.instance.ConnectToPhoton();
		Camera.main.transform.position = CameraPosForThisScreen.position;
		Camera.main.transform.rotation = CameraPosForThisScreen.rotation;
		for (int num = avaiableRoomsRowItems.Count - 1; num >= 0; num--)
		{
			avaiableRoomsRowItems[num].gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
			UnityEngine.Object.Destroy(avaiableRoomsRowItems[num].gameObject);
		}
		avaiableRoomsRowItems.Clear();
		ShowAvaiableRooms();
	}

	protected override void OnHide()
	{
		base.OnHide();
		LoadingPanel.SetActive(value: false);
	}

	private void OnDestroy()
	{
		MultiplayerController multiplayerController = MultiplayerController.instance;
		multiplayerController.RoomListUpdated = (Action)Delegate.Remove(multiplayerController.RoomListUpdated, new Action(OnRoomListUpdate));
		MultiplayerController multiplayerController2 = MultiplayerController.instance;
		multiplayerController2.PhotonJoinRoomFailed = (Action)Delegate.Remove(multiplayerController2.PhotonJoinRoomFailed, new Action(OnPhotonJoinRoomFailed));
		MultiplayerController multiplayerController3 = MultiplayerController.instance;
		multiplayerController3.ConnectionFailed = (Action)Delegate.Remove(multiplayerController3.ConnectionFailed, new Action(OnConnectionToPhotonFailed));
	}

	private void OnConnectionToPhotonFailed()
	{
		ScreenManager.instance.ShowLoading(show: false);
		ScreenManager.instance.ShowScreen(ScreenManager.instance.mainMenuScreen);
	}

	public override void OnBackButtonClick()
	{
		base.OnBackButtonClick();
		LoadingPanel.SetActive(value: false);
	}

	public void PrintLog(string log)
	{
		if (photonDebugLog != null)
		{
			photonDebugLog.text = log;
		}
	}

	public void OnRoomListUpdate()
	{
		if (!roomListInitialized)
		{
			ShowAvaiableRooms();
			roomListInitialized = true;
		}
		else if (Time.time - prevUpdTime > 7.3f)
		{
			ShowAvaiableRooms();
			prevUpdTime = Time.time;
		}
	}

	public void OnPhotonJoinRoomFailed()
	{
		StartCoroutine(OnPhotonJoinRoomFailedCRT());
	}

	private IEnumerator OnPhotonJoinRoomFailedCRT()
	{
		ScreenManager.instance.ShowLoading(show: false);
		LoadingPanel.SetActive(value: false);
		FailedToJoinRoomWindow.SetActive(value: true);
		yield return new WaitForSeconds(1f);
		FailedToJoinRoomWindow.SetActive(value: false);
	}

	public void OnJoinedLobby()
	{
		if (isActive)
		{
			LoadingPanel.SetActive(value: false);
		}
	}

	private void ShowAvaiableRooms()
	{
		for (int num = avaiableRoomsRowItems.Count - 1; num >= 0; num--)
		{
			avaiableRoomsRowItems[num].gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
			UnityEngine.Object.Destroy(avaiableRoomsRowItems[num].gameObject);
		}
		avaiableRoomsRowItems.Clear();
		RoomInfo[] array = PhotonNetwork.GetRoomList();
		Debug.Log("rooms = " + array.Length);
		if (!StorageController.instance.isDevelopmentBuild)
		{
			array = Array.FindAll(array, (RoomInfo r) => r.PlayerCount != r.MaxPlayers);
		}
		if (MultiplayerController.gameType == GameMode.ZombieSurvival)
		{
			array = Array.FindAll(array, (RoomInfo r) => int.Parse(r.CustomProperties["mode"].ToString()) == 3);
		}
		int num2 = array.Length / listLength;
		if (array.Length % listLength != 0)
		{
			num2++;
		}
		if (pageIndex + 1 > num2)
		{
			pageIndex = 0;
		}
		for (int num3 = pageIndex * listLength; num3 < pageIndex * listLength + listLength; num3++)
		{
			if (num3 > array.Length - 1)
			{
				break;
			}
			RoomInfo room = array[num3];
			AvaibleRoomRowItem component = UnityEngine.Object.Instantiate(RoomRowPrefab).GetComponent<AvaibleRoomRowItem>();
			component.gameObject.transform.SetParent(avaiableRoomsScrollContent);
			component.transform.localScale = new Vector3(1f, 1f, 1f);
			component.SetData(room);
			_ = room.Name;
			component.gameObject.GetComponent<Button>().onClick.AddListener(delegate
			{
				OnAvaiableRoomRowClick(room);
			});
			avaiableRoomsRowItems.Add(component);
			if (room.PlayerCount == room.MaxPlayers)
			{
				component.gameObject.GetComponent<Button>().interactable = false;
			}
		}
		roomsPageIndexLbl.text = (pageIndex + 1).ToString();
		maxRoomsCountLbl.text = num2.ToString();
	}

	private void OnAvaiableRoomRowClick(RoomInfo room)
	{
		if (room.PlayerCount != room.MaxPlayers)
		{
			MultiplayerController.gameType = (GameMode)int.Parse(room.CustomProperties["mode"].ToString());
			if (!string.IsNullOrEmpty((string)room.CustomProperties["pass"]))
			{
				roomPasswordWindow.Show(room);
				return;
			}
			LoadingPanel.SetActive(value: true);
			MultiplayerController.instance.JoinRandomRoomWithThisName(room.Name);
		}
	}

	public void PagingNextBtn(int sign)
	{
		pageIndex += sign;
		if (pageIndex < 0)
		{
			pageIndex = 0;
		}
		ShowAvaiableRooms();
	}

	public void OnLoadSavedGameBtn()
	{
		ScreenManager.instance.ShowScreen(ScreenManager.instance.loadSavedGameScreen);
	}

	public void OnCreateNewGameBtn()
	{
		ScreenManager.instance.ShowScreen(ScreenManager.instance.gameTypeSelectScreen);
	}

	public void CreateNewGame()
	{
		if (MultiplayerController.gameType == GameMode.PvP)
		{
			MultiplayerController.instance.newGameRoomName = newDeathMatchGameNameInput.text;
		}
		else if (MultiplayerController.gameType == GameMode.TeamFight || MultiplayerController.gameType == GameMode.CaptureFlag)
		{
			MultiplayerController.instance.newGameRoomName = newTFGameNameInput.text;
		}
		MultiplayerController.instance.CreateRoomWithSelectedParams(MultiplayerController.instance.MaxPlayersCountPerMode(MultiplayerController.gameType));
		NewDeathmatchGamePanel.SetActive(value: false);
		NewTeamFightGamePanel.SetActive(value: false);
		LoadingPanel.SetActive(value: true);
		Debug.LogWarning(MultiplayerController.instance.playerSelectedTeamID);
	}

	public void CancelNewGameCreation()
	{
		NewDeathmatchGamePanel.SetActive(value: false);
		NewTeamFightGamePanel.SetActive(value: false);
	}

	public void SetNewGamePlayerTeamID_A()
	{
		MultiplayerController.instance.playerSelectedTeamID = TeamID.TeamA;
	}

	public void SetNewGamePlayerTeamID_B()
	{
		MultiplayerController.instance.playerSelectedTeamID = TeamID.TeamB;
	}

	public void OnNewGameCreationFailed()
	{
		LoadingPanel.SetActive(value: false);
		NewGameCreationFailedPanel.SetActive(value: true);
		Invoke("HideNewGameCreationFailedPanel", 1f);
	}

	private void HideNewGameCreationFailedPanel()
	{
		NewGameCreationFailedPanel.SetActive(value: false);
	}

	public void OnJoinRandomBtn()
	{
		LoadingPanel.SetActive(value: true);
		MultiplayerController.instance.JoinRandomRoom(GameMode.TeamFight);
	}

	public void OnCancelConnectingBtn()
	{
		MultiplayerController.instance.DisconnectToPhoton();
		LoadingPanel.SetActive(value: false);
		ScreenManager.instance.ShowLoading(show: false);
		ScreenManager.instance.ShowScreen(ScreenManager.instance.mainMenuScreen);
	}
}
