using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : Photon.MonoBehaviour
{
	public delegate void MatchTimeEnded();

	public static GameController instance;

	public static GameConfigData gameConfigData = new GameConfigData();

	public static PlayerConfigData playerConfigData = new PlayerConfigData();

	public GameObject PlayerPrefab;

	public List<CharacterMotor> Players = new List<CharacterMotor>();

	public CharacterMotor OurPlayer;

	public double GameStartTime = -1.0;

	public double TimeUntilTheEndOfLevel;

	public bool IsLevelFinished;

	public static float globalScaleKoeff = 1f;

	public CameraMode cameraMode;

	public TeamsScores teamScores;

	private bool isJoinedRoom;

	private double gameStartTime;

	public static bool isOffline;

	public static bool isMobile;

	public static Action<CharacterMotor> PlayerJoined;

	public static Action<CharacterMotor> PlayerDisconnected;

	public const int RevengeScoreReward = 12;

	public const int KillAssistScoreReward = 3;

	public const int KillScoreReward = 10;

	public static Action<CharacterMotor> OurPlayerCreated;

	public static Action PlayerMakeKill;

	public static Action PlayerMakeKillAssist;

	public static Action PlayerMakeRevenge;

	public static Action OurPlayerKilled;

	public static Action<CharacterMotor> PlayerKilled;

	public static Action ContinueMatch;

	private int prevMatchIndex = -1;

	private int matchIndex = -1;

	public static Action<string> BattleRoyaleFinished;

	public static bool isInMainMenu => SceneManager.GetActiveScene().name == "MainMenu";

	public int MyTeamScore
	{
		get
		{
			if (OurPlayer == null)
			{
				return -1;
			}
			int num = 0;
			foreach (CharacterMotor item in AllPlayers())
			{
				if (item.myTeam == OurPlayer.myTeam)
				{
					num += item.playerInfo.score;
				}
			}
			return num;
		}
	}

	public int OtherTeamScore
	{
		get
		{
			if (OurPlayer == null)
			{
				return -1;
			}
			int num = 0;
			foreach (CharacterMotor item in AllPlayers())
			{
				if (item.myTeam != OurPlayer.myTeam)
				{
					num += item.playerInfo.score;
				}
			}
			return num;
		}
	}

	public static event MatchTimeEnded MatchFinidhed;

	private void Awake()
	{
		instance = this;
		isMobile = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
		if (gameConfigData.savedGame.items != null)
		{
			InitializeSavedGame();
		}
	}

	private IEnumerator Start()
	{
		InitVars();
		yield return null;
		GameWindow.instance.ShowTeamJoiningPanel();
	}

	private void InitVars()
	{
		teamScores = default(TeamsScores);
		SetUpPlayerConfigData();
	}

	private void SetUpPlayerConfigData()
	{
		playerConfigData.SelectedSkinIndex = StorageController.instance.SelectedPlayer;
	}

	private void Update()
	{
		CheckMatchEndTime();
	}

	public void OnCompletlyConnected()
	{
		StartCoroutine(OnCompletlyConnectedCRT());
	}

	private IEnumerator OnCompletlyConnectedCRT()
	{
		while (PhotonNetwork.time < 1.0)
		{
			yield return null;
		}
		if (OurPlayer == null)
		{
			CreateOurPlayer();
			GameWindow.instance.HidePhotonConnectionPanel();
		}
		yield return null;
		StartCoroutine(WaitForStartTime());
	}

	private void RemoveMyDublicates()
	{
		foreach (CharacterMotor player in Players)
		{
			if (!(player == null) && !(player.photonView == null) && player.photonView.owner != null && player.photonView.owner.ID == PhotonNetwork.player.ID)
			{
				Debug.LogError("SIT!!!");
				if (player.photonView.isMine)
				{
					PhotonNetwork.Destroy(player.gameObject);
				}
			}
		}
	}

	private void CreateOurPlayer()
	{
		RemoveMyDublicates();
		Transform transform = ArenaScript.instance.GetRandomRespawnPoint();
		if (gameConfigData.gameMode == GameMode.TeamFight || MultiplayerController.gameType == GameMode.CaptureFlag)
		{
			transform = ArenaScript.instance.GetTeamRespawnPoint(MultiplayerController.instance.playerSelectedTeamID);
		}
		else if (gameConfigData.gameMode == GameMode.BattleRoyalePvP)
		{
			transform = ArenaScript.instance.GetBattleRoyalePvPLobbyPoint();
		}
		else if (gameConfigData.gameMode == GameMode.BattleRoyaleTeams)
		{
			transform = ArenaScript.instance.GetBattleRoyaleTeamsLobbyPoint(MultiplayerController.instance.playerSelectedTeamID);
		}
		GameObject gameObject = PhotonNetwork.Instantiate(StorageController.instance.SelectedPlayer, transform.position, transform.rotation, 0, new object[6]
		{
			StorageController.instance.PlayerName,
			(int)MultiplayerController.instance.playerSelectedTeamID,
			StorageController.SelectedHat,
			StorageController.SelectedEyes,
			StorageController.SelectedSmile,
			StorageController.SelectedShoes
		});
		OurPlayer = gameObject.GetComponent<CharacterMotor>();
		OurPlayer.name = "------OUR_PLAYER------";
		if (OurPlayerCreated != null)
		{
			OurPlayerCreated(OurPlayer);
		}
	}

	private IEnumerator WaitForStartTime()
	{
		while (PhotonNetwork.time < 9.999999747378752E-05)
		{
			yield return null;
		}
		if (PhotonNetwork.isMasterClient)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add("starttime", PhotonNetwork.time.ToString());
			PhotonNetwork.room.SetCustomProperties(hashtable);
		}
		gameStartTime = double.Parse((string)PhotonNetwork.room.CustomProperties["starttime"]);
		isJoinedRoom = true;
	}

	public void OnPlayerConnected(GameObject otherPlayer)
	{
		if (PlayerJoined != null)
		{
			PlayerJoined(otherPlayer.GetComponent<CharacterMotor>());
		}
		if (!(OurPlayer == otherPlayer))
		{
			Players.Add(otherPlayer.GetComponent<CharacterMotor>());
			if (OurPlayer != null)
			{
				OurPlayer.OnNewPlayerJoined();
			}
			if (PhotonNetwork.isMasterClient)
			{
				SyncTeamScore();
			}
		}
	}

	public void OnOtherPlayerDisconnected(CharacterMotor otherPlayer)
	{
		if (PlayerDisconnected != null)
		{
			PlayerDisconnected(otherPlayer);
		}
		Players.Remove(otherPlayer);
	}

	public void OnPlayerKilled(CharacterMotor player)
	{
		if (PlayerKilled != null)
		{
			PlayerKilled(player);
		}
		CheckBattleRoyaleAlives();
		if (OurPlayer == null)
		{
			return;
		}
		if (player.ViewId == OurPlayer.ViewId)
		{
			_ = OurPlayer.lastKillerId;
			_ = player.ViewId;
			if (OurPlayerKilled != null)
			{
				OurPlayerKilled();
			}
			OurPlayer.HandleMyDeath();
		}
		else if (player.lastKillerId == OurPlayer.ViewId)
		{
			OnPlayerMakeKill();
		}
		else if (player.IsThisIdKillerAssist(OurPlayer.ViewId))
		{
			OnPlayerMakeKillAssist();
		}
	}

	private void OnPlayerMakeKill()
	{
		Debug.Log("PlayerMakeKill");
		if (PlayerMakeKill != null)
		{
			PlayerMakeKill();
		}
		OurPlayer.AddScore(10);
		OurPlayer.HandleMakeKill();
	}

	private void OnPlayerMakeKillAssist()
	{
		Debug.Log("OnPlayerMakeKillAssist");
		if (PlayerMakeKillAssist != null)
		{
			PlayerMakeKillAssist();
		}
		OurPlayer.AddScore(3);
		OurPlayer.HandleMakeKillAssist();
	}

	public void AddTeamScore(int score, TeamID team)
	{
		if (gameConfigData.gameMode == GameMode.TeamFight)
		{
			switch (team)
			{
			case TeamID.TeamA:
				teamScores.TeamAScore += score;
				break;
			case TeamID.TeamB:
				teamScores.TeamBScore += score;
				break;
			}
			SyncTeamScore();
		}
	}

	public void ResetTeamScore()
	{
		if (PhotonNetwork.isMasterClient)
		{
			teamScores.TeamAScore = 0;
			teamScores.TeamBScore = 0;
			SyncTeamScore();
		}
	}

	private void OnDestroy()
	{
		gameConfigData.savedGame.items = null;
		OnGameSceneLeave();
	}

	public void OnGameSceneLeave()
	{
		MultiplayerController.instance.OnGameSceneLeave();
	}

	private void CheckMatchEndTime()
	{
		if (gameConfigData.gameMode == GameMode.ZombieSurvival || gameConfigData.gameMode == GameMode.BattleRoyalePvP || gameConfigData.gameMode == GameMode.BattleRoyaleTeams || !isJoinedRoom)
		{
			return;
		}
		double num = PhotonNetwork.time - gameStartTime;
		matchIndex = (int)Mathf.Floor((int)(num / gameConfigData.levelDuration));
		if (prevMatchIndex < 0)
		{
			prevMatchIndex = matchIndex;
		}
		TimeUntilTheEndOfLevel = gameConfigData.levelDuration - ((double)(float)num - (double)matchIndex * gameConfigData.levelDuration);
		if (matchIndex == prevMatchIndex + 1)
		{
			prevMatchIndex = matchIndex;
			if (gameConfigData.gameMode == GameMode.Sandbox)
			{
				return;
			}
			if (GameController.MatchFinidhed != null)
			{
				GameController.MatchFinidhed();
			}
			StartCoroutine(MatchFinishedCRT());
			OnLevelTimeFinished();
			OurPlayer.EnterPauseMode();
		}
		if (TimeUntilTheEndOfLevel < 0.0)
		{
			TimeUntilTheEndOfLevel = 0.0;
		}
	}

	private IEnumerator MatchFinishedCRT()
	{
		yield return new WaitForSeconds(0.82f);
		if (gameConfigData.gameMode == GameMode.TeamFight && PhotonNetwork.isMasterClient)
		{
			teamScores.TeamAScore = 0;
			teamScores.TeamBScore = 0;
			SyncTeamScore();
		}
		OurPlayer.ResetOnPlayAgain();
	}

	private void OnLevelTimeFinished()
	{
		GameInputController.instance.StopAll();
		GameWindow.instance.OnMatchFinished();
	}

	private void CheckBattleRoyaleAlives()
	{
		if (MultiplayerController.gameType != GameMode.BattleRoyalePvP || !PhotonNetwork.isMasterClient)
		{
			return;
		}
		int count = AllPlayers().FindAll((CharacterMotor p) => p.IsAlive()).Count;
		if (count != 0 && count != 1)
		{
			return;
		}
		string empty = string.Empty;
		if (count == 1)
		{
			empty = AllPlayers().FindAll((CharacterMotor p) => p.IsAlive())[0].playerInfo.name;
		}
		PhotonNetwork.RPC(base.photonView, "BattleRoyaleFinishedR", PhotonTargets.All, false, empty);
	}

	[PunRPC]
	private void BattleRoyaleFinishedR(string winnerName)
	{
		if (GameController.MatchFinidhed != null)
		{
			GameController.MatchFinidhed();
		}
		OnLevelTimeFinished();
		OurPlayer.EnterPauseMode();
		OurPlayer.ResetOnPlayAgain();
		if (BattleRoyaleFinished != null)
		{
			BattleRoyaleFinished(winnerName);
		}
	}

	public void OnClickPlayAgain()
	{
		GameWindow.instance.OnContinurGame();
		OurPlayer.OutFromPauseMode();
		OurPlayer.ResetOnPlayAgain();
		if (ContinueMatch != null)
		{
			ContinueMatch();
		}
	}

	private void SyncTeamScore()
	{
		PhotonNetwork.RPC(base.photonView, "SyncTeamScoreR", PhotonTargets.All, false, teamScores.TeamAScore, teamScores.TeamBScore);
	}

	[PunRPC]
	private void SyncTeamScoreR(int teamAScore, int teamBscore)
	{
		teamScores.TeamAScore = teamAScore;
		teamScores.TeamBScore = teamBscore;
		GameWindow.instance.UpdateFragsCount();
	}

	private void InitializeSavedGame()
	{
		InventoryItem[] items = gameConfigData.savedGame.items;
		foreach (InventoryItem inventoryItem in items)
		{
			if (!string.IsNullOrEmpty(inventoryItem.prefabName) && Resources.Load(inventoryItem.prefabName) != null)
			{
				PhotonNetwork.Instantiate(inventoryItem.prefabName, inventoryItem.pos, Quaternion.Euler(inventoryItem.rot), 0);
			}
		}
	}

	public void GoToMainMenu()
	{
		Debug.Log("------GoToMainMenu---------");
		StartCoroutine(GoToMainMenuCrt());
	}

	private IEnumerator GoToMainMenuCrt()
	{
		FPSCamera.instance.OnGoToMainMenu();
		PhotonNetwork.Destroy(OurPlayer.gameObject);
		yield return null;
		MultiplayerController.instance.DisconnectToPhoton();
		yield return null;
		SceneManager.LoadScene("MainMenu");
		yield return null;
	}

	public List<CharacterMotor> AllPlayers()
	{
		return new List<CharacterMotor>(Players) { OurPlayer };
	}

	public List<CharacterMotor> AllPlayersNotDublicated()
	{
		List<CharacterMotor> list = instance.AllPlayers();
		List<CharacterMotor> list2 = new List<CharacterMotor>();
		foreach (CharacterMotor item in list)
		{
			if (item == null || item.photonView == null || item.photonView.owner == null)
			{
				continue;
			}
			bool flag = false;
			foreach (CharacterMotor item2 in list2)
			{
				if (item2.photonView.owner.ID == item.photonView.owner.ID)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				list2.Add(item);
			}
		}
		return list2;
	}

	public int GetTeamScore(TeamID teamId)
	{
		switch (teamId)
		{
		case TeamID.TeamA:
			return teamScores.TeamAScore;
		case TeamID.TeamB:
			return teamScores.TeamBScore;
		default:
			return -1;
		}
	}

	public Color GetTeamColor(TeamID teamId)
	{
		if (gameConfigData.gameMode == GameMode.Sandbox)
		{
			return Color.white;
		}
		if (teamId == TeamID.None)
		{
			return DataModel.instance.teamB_Color;
		}
		if (teamId == MultiplayerController.instance.playerSelectedTeamID)
		{
			return DataModel.instance.teamA_Color;
		}
		return DataModel.instance.teamB_Color;
	}

	public bool IsTeamMate(TeamID teamId)
	{
		if (OurPlayer == null)
		{
			return false;
		}
		if (teamId == OurPlayer.myTeam)
		{
			return teamId != TeamID.None;
		}
		return false;
	}

	public Color GetTemRelColor(TeamID team)
	{
		if (IsTeamMate(team))
		{
			return DataModel.instance.teamA_Color;
		}
		return DataModel.instance.teamB_Color;
	}

	private void OnMasterClientSwitched()
	{
		if (PhotonNetwork.isMasterClient)
		{
			SyncTeamScore();
		}
	}

	public void StartBattleRoyaleGame()
	{
		if (PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.RPC(base.photonView, "StartBattleRoyaleR", PhotonTargets.All, false);
		}
	}

	[PunRPC]
	public void StartBattleRoyaleR()
	{
		Debug.Log("StartBattleRoyaleR");
		Transform transform = ArenaScript.instance.GetRandomRespawnPoint();
		if (MultiplayerController.gameType == GameMode.BattleRoyaleTeams)
		{
			transform = ArenaScript.instance.GetTeamRespawnPoint(OurPlayer.myTeam);
		}
		OurPlayer.transform.position = transform.position + Vector3.up * 40f;
	}
}
