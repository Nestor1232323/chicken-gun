using System;
using ExitGames.Client.Photon;
using Photon;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerController : Photon.MonoBehaviour
{
	public delegate void PhotonRoomPropertiesChanged();

	public static MultiplayerController instance;

	public Action ConnectionFailed;

	public Action RoomListUpdated;

	public Action PhotonJoinRoomFailed;

	public Action PhotonCreateRoomFailed;

	public static Action PhotonConnected;

	public static Action MasterClientChanged;

	public static Action JoinedToLobby;

	public int photonGameVersion = 1;

	public static GameMode gameType;

	public MapID selectedMapID;

	public string newGameRoomName;

	public string password = string.Empty;

	public TeamID playerSelectedTeamID;

	public byte MAX_PLAYER_COUNT = 10;

	public static TypedLobby lobby = new TypedLobby("BallsWars", LobbyType.SqlLobby);

	public int PhotonGameVersion => photonGameVersion;

	public static event PhotonRoomPropertiesChanged RoomPropertiesChanged;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		PhotonNetwork.BackgroundTimeout = 38f;
	}

	private bool IsKitayPlayer()
	{
		if (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified || Application.systemLanguage == SystemLanguage.ChineseTraditional)
		{
			return true;
		}
		return false;
	}

	private void Start()
	{
		playerSelectedTeamID = TeamID.None;
		PhotonNetwork.autoCleanUpPlayerObjects = true;
	}

	public void ConnectToPhoton()
	{
		if (PhotonNetwork.connectionState == ConnectionState.Connected)
		{
			PhotonNetwork.JoinLobby();
			return;
		}
		GameConnectionScreen.instance.PrintLog("Connecting...");
		if (!PhotonNetwork.ConnectUsingSettings("v" + photonGameVersion))
		{
			if (ConnectionFailed != null)
			{
				ConnectionFailed();
			}
			Debug.LogError("Fail");
		}
	}

	public void DisconnectToPhoton()
	{
		if (PhotonNetwork.connectionState == ConnectionState.Connected || PhotonNetwork.connectionState == ConnectionState.Connecting)
		{
			PhotonNetwork.Disconnect();
		}
	}

	public void JoinRandomGame()
	{
		PhotonNetwork.JoinRandomRoom(new Hashtable
		{
			{
				"mode",
				(int)gameType
			},
			{
				"pass",
				string.Empty
			}
		}, MaxPlayersCountPerMode(gameType));
	}

	public void JoinRandomRoomWithThisName(string roomName)
	{
		PhotonNetwork.JoinRoom(roomName);
	}

	public void JoinRandomRoom(GameMode mode)
	{
		PhotonNetwork.JoinRandomRoom(new Hashtable
		{
			{
				"mode",
				(int)mode
			},
			{
				"pass",
				string.Empty
			}
		}, MaxPlayersCountPerMode(mode));
	}

	private void OnPhotonJoinRoomFailed()
	{
		Debug.Log("OnPhotonJoinRoomFailed");
		if (PhotonJoinRoomFailed != null)
		{
			PhotonJoinRoomFailed();
		}
		GameConnectionScreen.instance.PrintLog("OnPhotonJoinRoomFailed");
	}

	private void OnPhotonCreateRoomFailed()
	{
		Debug.Log("OnPhotonCreateRoomFailed");
		if (PhotonCreateRoomFailed != null)
		{
			PhotonCreateRoomFailed();
		}
	}

	private void OnPhotonRandomJoinFailed()
	{
		Debug.Log("OnPhotonRandomJoinFailed");
		GameConnectionScreen.instance.PrintLog("OnPhotonRandomJoinFailed");
		CreateRoomWithSelectedParams(MaxPlayersCountPerMode(gameType));
	}

	private void OnJoinedLobby()
	{
		Debug.Log("OnJoinedLobby");
		GameConnectionScreen.instance.PrintLog("JoinedLobby");
		GameConnectionScreen.instance.OnJoinedLobby();
		if (JoinedToLobby != null)
		{
			JoinedToLobby();
		}
	}

	private void OnConnectedToPhoton()
	{
		Debug.Log("OnConnectedToPhoton");
		GameConnectionScreen.instance.PrintLog("OnConnectedToPhoton");
		if (PhotonConnected != null)
		{
			PhotonConnected();
		}
	}

	private void OnConnectionFail()
	{
		Debug.LogError("OnConnectionFail");
		if (ConnectionFailed != null)
		{
			ConnectionFailed();
		}
		GameConnectionScreen.instance.PrintLog("Connection fail, try again");
	}

	private void OnJoinedRoom()
	{
		MapID mapID = (MapID)int.Parse(PhotonNetwork.room.CustomProperties["map"].ToString());
		Debug.Log("OnJoinedRoom map = " + mapID);
		GameConnectionScreen.instance.PrintLog("JoinedRoom ");
		Debug.Log("OnJoinedRoom " + PhotonNetwork.room.CustomProperties.ToString());
		LoadLocation(mapID);
	}

	private void OnConnectedToMaster()
	{
		Debug.Log("OnConnectedToMaster");
		PhotonNetwork.JoinLobby();
	}

	private void OnCreatedRoom()
	{
		Debug.Log("OnCreatedRoom");
	}

	private void OnLeftRoom()
	{
		Debug.Log("=========OnLeftRoom==========");
		SceneManager.LoadScene("MainMenu");
	}

	private void OnReceivedRoomListUpdate()
	{
		if (RoomListUpdated != null)
		{
			RoomListUpdated();
		}
	}

	private void OnMasterClientSwitched()
	{
		Debug.Log("OnMasterClientSwitched");
		if (MasterClientChanged != null)
		{
			MasterClientChanged();
		}
	}

	private void OnPhotonMaxCccuReached()
	{
		Debug.Log("OnPhotonMaxCccuReached");
	}

	private void OnPhotonCustomRoomPropertiesChanged()
	{
		if (MultiplayerController.RoomPropertiesChanged != null)
		{
			MultiplayerController.RoomPropertiesChanged();
		}
	}

	public void CreateRoom(string roomname, int maxPlayers, string password = "")
	{
		this.password = password;
		MAX_PLAYER_COUNT = (byte)maxPlayers;
		newGameRoomName = roomname;
		CreateRoomWithSelectedParams((byte)maxPlayers);
	}

	public void CreateRoom(string roomname, int maxPlayers, bool isSavedGame)
	{
		MAX_PLAYER_COUNT = (byte)maxPlayers;
		newGameRoomName = roomname;
		CreateRoomWithSelectedParams((byte)maxPlayers);
	}

	public void CreateRoomWithSelectedParams(byte maxPlayers)
	{
		GameConnectionScreen.instance.PrintLog("CreateRoomWithSelectedParams");
		string[] customRoomPropertiesForLobby = new string[4] { "starttime", "mode", "map", "pass" };
		Hashtable hashtable = new Hashtable();
		if (newGameRoomName == string.Empty || newGameRoomName == string.Empty)
		{
			newGameRoomName = StorageController.instance.PlayerName + "'game";
		}
		hashtable.Add("mode", (int)gameType);
		hashtable.Add("map", (int)selectedMapID);
		hashtable.Add("pass", password);
		password = string.Empty;
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.IsVisible = true;
		roomOptions.IsOpen = true;
		roomOptions.MaxPlayers = maxPlayers;
		roomOptions.CustomRoomProperties = hashtable;
		roomOptions.CustomRoomPropertiesForLobby = customRoomPropertiesForLobby;
		PhotonNetwork.CreateRoom(newGameRoomName, roomOptions, TypedLobby.Default);
	}

	public void LoadLocation(MapID mapID)
	{
		Debug.Log("LoadLocation " + mapID);
		PhotonNetwork.LoadLevel(mapID.ToString());
	}

	public void OnGameSceneLeave()
	{
		playerSelectedTeamID = TeamID.None;
		if (PhotonNetwork.connectionState != ConnectionState.Disconnected && PhotonNetwork.connectionState != ConnectionState.Disconnecting)
		{
			PhotonNetwork.LeaveRoom();
		}
	}

	public void JoinToTeam(TeamID teamID)
	{
		playerSelectedTeamID = teamID;
		GameController.instance.OnCompletlyConnected();
	}

	public string GetTeamCustomName(TeamID teamID)
	{
		if (teamID == TeamID.None)
		{
			return "none";
		}
		return teamID.ToString();
	}

	public int GetTeamIconIndex(TeamID teamID)
	{
		object obj;
		switch (teamID)
		{
		case TeamID.None:
			return 0;
		case TeamID.TeamA:
			obj = "taicon";
			break;
		default:
			obj = "tbicon";
			break;
		}
		string key = (string)obj;
		string text = PhotonNetwork.room.CustomProperties[key].ToString();
		int result = 0;
		if (text != string.Empty && text != string.Empty)
		{
			result = int.Parse(text);
		}
		return result;
	}

	public void SetTeamCustomName(string name)
	{
	}

	public void SetTeamCustomIcon(int iconIndex)
	{
	}

	public bool IsTeamNameCustomized(TeamID team)
	{
		return true;
	}

	public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		Debug.Log("Player Connected " + player.NickName);
	}

	public void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		Debug.Log("Player Disconnected " + player.NickName);
	}

	public byte MaxPlayersCountPerMode(GameMode mode)
	{
		switch (mode)
		{
		case GameMode.PvP:
			return 10;
		case GameMode.TeamFight:
			return 10;
		case GameMode.BattleRoyalePvP:
			return 1;
		case GameMode.BattleRoyaleTeams:
			return 14;
		default:
			return 10;
		}
	}
}
