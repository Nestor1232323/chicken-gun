using System;
using System.Collections;
using System.Collections.Generic;
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
			lobbyWaitingPanel.SetActive(true);
			JoinToTeam(TeamID.None);
		}
		else if (MultiplayerController.gameType == GameMode.BattleRoyaleTeams)
		{
			lobbyWaitingPanel.SetActive(true);
			ShowTeamAPlayers();
		}
	}

	private void ShowTeamAPlayers()
	{
		base.gameObject.SetActive(true);
		FullTeamA_Btn.SetActive(false);
		FullTeamB_Btn.SetActive(false);
		loading.SetActive(false);
		StartCoroutine(SelectTeamCRT());
	}

	private void Update()
	{
		timeCounter -= Time.deltaTime;
		if (timeCounter > 0f)
		{
			ts = TimeSpan.FromSeconds(timeCounter);
			timer.text = string.Format("{0:D2}", ts.Seconds);
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
			loading.SetActive(true);
		}
	}

	private IEnumerator SelectTeamCRT(bool forceJoinTo = false)
	{
		GameWindow.instance.ShowMainUI(false);
		yield return new WaitForSeconds(0.9f);
		List<CharacterMotor> OtherPlayers = GameController.instance.Players;
		foreach (CharacterMotor item in OtherPlayers)
		{
			TeamID myTeam = item.myTeam;
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
			component.SetPlayerName(item.playerInfo.name);
		}
		int maxPlayers = Mathf.FloorToInt((float)PhotonNetwork.room.MaxPlayers * 0.5f);
		FullTeamA_Btn.SetActive(teamA_count == maxPlayers);
		FullTeamB_Btn.SetActive(teamB_count == maxPlayers);
		SelectTeamA_Btn.SetActive(teamA_count < maxPlayers);
		SelectTeamB_Btn.SetActive(teamB_count < maxPlayers);
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
		GameWindow.instance.ShowMainUI(true);
	}
}
