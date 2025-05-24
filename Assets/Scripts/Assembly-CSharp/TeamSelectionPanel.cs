using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TeamSelectionPanel : MonoBehaviour
{
	public GameObject TeamJoiningPlayerRowPrefab;

	public RectTransform TeamAPlayersScrollContent;

	public RectTransform TeamBPlayersScrollContent;

	[SerializeField]
	private GameObject SelectTeamA_Btn;

	[SerializeField]
	private GameObject SelectTeamB_Btn;

	[SerializeField]
	private GameObject FullTeamA_Btn;

	[SerializeField]
	private GameObject FullTeamB_Btn;

	[SerializeField]
	private Text timer;

	[SerializeField]
	private float timeCounter = 12f;

	[SerializeField]
	private GameObject loading;

	[SerializeField]
	private GameObject lobbyWaitingPanel;

	private TimeSpan ts;

	private int teamA_count;

	private int teamB_count;

	public void Show()
	{
		if (MultiplayerController.gameType == GameMode.TeamFight)
		{
			ShowTeamAPlayers();
		}
		else if (MultiplayerController.gameType == GameMode.PvP)
		{
			JoinToTeam(TeamID.None);
		}
		else if (MultiplayerController.gameType == GameMode.BattleRoyalePvP)
		{
			lobbyWaitingPanel.SetActive(value: true);
			JoinToTeam(TeamID.None);
		}
		else if (MultiplayerController.gameType == GameMode.BattleRoyaleTeams)
		{
			lobbyWaitingPanel.SetActive(value: true);
			ShowTeamAPlayers();
		}
	}

	private void ShowTeamAPlayers()
	{
		base.gameObject.SetActive(value: true);
		FullTeamA_Btn.SetActive(value: false);
		FullTeamB_Btn.SetActive(value: false);
		loading.SetActive(value: false);
		StartCoroutine(SelectTeamCRT());
	}

	private void Update()
	{
		timeCounter -= Time.deltaTime;
		if (timeCounter > 0f)
		{
			ts = TimeSpan.FromSeconds(timeCounter);
			timer.text = $"{ts.Seconds:D2}";
			return;
		}
		timer.text = "Joining random...";
		if (timeCounter < -1.3f)
		{
			if (teamB_count > teamA_count)
			{
				JoinToTeam_A();
			}
			else if (teamA_count > teamB_count)
			{
				JoinToTeam_B();
			}
			else if (UnityEngine.Random.value > 0.5f)
			{
				JoinToTeam_A();
			}
			else
			{
				JoinToTeam_B();
			}
		}
		else if (timeCounter < -0.7f)
		{
			loading.SetActive(value: true);
		}
	}

	private IEnumerator SelectTeamCRT(bool forceJoinTo = false)
	{
		GameWindow.instance.ShowMainUI(show: false);
		yield return new WaitForSeconds(0.9f);
		foreach (CharacterMotor player in GameController.instance.Players)
		{
			TeamID myTeam = player.myTeam;
			TeamJoiningPlayerRow component = UnityEngine.Object.Instantiate(TeamJoiningPlayerRowPrefab).GetComponent<TeamJoiningPlayerRow>();
			switch (myTeam)
			{
			case TeamID.TeamA:
				component.gameObject.transform.SetParent(TeamAPlayersScrollContent);
				teamA_count++;
				break;
			case TeamID.TeamB:
				component.gameObject.transform.SetParent(TeamBPlayersScrollContent);
				teamB_count++;
				break;
			}
			component.transform.localScale = Vector3.one;
			component.SetPlayerName(player.playerInfo.name);
		}
		int num = Mathf.FloorToInt((float)PhotonNetwork.room.MaxPlayers * 0.5f);
		FullTeamA_Btn.SetActive(teamA_count == num);
		FullTeamB_Btn.SetActive(teamB_count == num);
		SelectTeamA_Btn.SetActive(teamA_count < num);
		SelectTeamB_Btn.SetActive(teamB_count < num);
		if (MultiplayerController.gameType == GameMode.BattleRoyaleTeams)
		{
			if (teamB_count > teamA_count)
			{
				JoinToTeam_A();
			}
			else if (teamA_count > teamB_count)
			{
				JoinToTeam_B();
			}
		}
		else if (PhotonNetwork.room.MaxPlayers == 1)
		{
			JoinToTeam_A();
		}
	}

	public void JoinToTeam_A()
	{
		JoinToTeam(TeamID.TeamA);
	}

	public void JoinToTeam_B()
	{
		JoinToTeam(TeamID.TeamB);
	}

	private void JoinToTeam(TeamID team)
	{
		GameWindow.instance.HideTeamJoiningPanel();
		MultiplayerController.instance.JoinToTeam(team);
		Camera.main.orthographic = false;
		GameWindow.instance.ShowMainUI(show: true);
	}
}
