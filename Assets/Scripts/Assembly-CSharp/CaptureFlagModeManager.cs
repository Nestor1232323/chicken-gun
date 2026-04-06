using System;
using Photon;
using UnityEngine;

public class CaptureFlagModeManager : Photon.MonoBehaviour
{
	public int teamAScore;

	public int teamBScore;

	private void Start()
	{
		if (GameController.gameConfigData.gameMode != GameMode.CaptureFlag)
		{
			UnityEngine.Object.Destroy(this);
		}
		GameController.PlayerJoined = (Action<CharacterMotor>)Delegate.Combine(GameController.PlayerJoined, new Action<CharacterMotor>(OnNewPlayerJoined));
	}

	private void OnDestroy()
	{
		GameController.PlayerJoined = (Action<CharacterMotor>)Delegate.Remove(GameController.PlayerJoined, new Action<CharacterMotor>(OnNewPlayerJoined));
	}

	private void OnNewPlayerJoined(CharacterMotor player)
	{
		if (base.photonView.isMine)
		{
			PhotonNetwork.RPC(base.photonView, "SyncCFScore", PhotonTargets.All, false, teamAScore, teamBScore);
		}
	}

	public void OnFlagCaptured(TeamID team, CharacterMotor owner)
	{
		switch (team)
		{
		case TeamID.TeamA:
			teamAScore++;
			break;
		case TeamID.TeamB:
			teamBScore++;
			break;
		}
		if (owner.photonView.isMine)
		{
			GameMessageLogger.instance.LogMessage("Flag captured! +1 score");
			PhotonNetwork.RPC(base.photonView, "SyncCFScore", PhotonTargets.All, false, teamAScore, teamBScore);
		}
	}

	[PunRPC]
	private void SyncCFScore(int teamAScore, int teamBScore)
	{
		if (GameWindow.instance != null && GameController.instance.OurPlayer != null)
		{
			int score = teamAScore;
			int score2 = teamBScore;
			if (GameController.instance.OurPlayer.myTeam == TeamID.TeamB)
			{
				score = teamBScore;
				score2 = teamAScore;
			}
			GameWindow.instance.fragsWidget.UpdateCaptuedFlagsCount(score, score2);
		}
	}

	public int MyTeamScore(TeamID team)
	{
		switch (team)
		{
		case TeamID.TeamA:
			return teamAScore;
		case TeamID.TeamB:
			return teamBScore;
		default:
			return -1;
		}
	}

	public void ResetScores()
	{
		teamAScore = 0;
		teamBScore = 0;
		GameWindow.instance.fragsWidget.UpdateCaptuedFlagsCount(teamAScore, teamBScore);
	}
}
