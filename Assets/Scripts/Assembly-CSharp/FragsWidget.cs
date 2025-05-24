using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FragsWidget : MonoBehaviour
{
	public GameObject DeathMatchPanel;

	public GameObject TeamFightPanel;

	public GameObject ZombieSurvPanel;

	public GameObject BattleRoyalePvPPanel;

	public GameObject BattleRoyaleTeamsPanel;

	public Text fragsCount;

	public Text TeamA_fragsCount;

	public Text TeamB_fragsCount;

	public Image teamA_icon;

	public Image teamB_icon;

	[SerializeField]
	private GameObject OtherPlayersListBtn;

	[SerializeField]
	private GameObject teamA_indc;

	[SerializeField]
	private GameObject teamB_indc;

	public Text curZWave;

	[SerializeField]
	private Text battleRoyalePvPalivesText;

	private void Awake()
	{
		fragsCount.text = 0.ToString();
		GameController.OurPlayerCreated = (Action<CharacterMotor>)Delegate.Combine(GameController.OurPlayerCreated, new Action<CharacterMotor>(OnOurPlayerCreated));
		GameController.PlayerKilled = (Action<CharacterMotor>)Delegate.Combine(GameController.PlayerKilled, new Action<CharacterMotor>(OnPlayerKilled));
		GameController.PlayerKilled = (Action<CharacterMotor>)Delegate.Combine(GameController.PlayerKilled, new Action<CharacterMotor>(OnPlayerJoined));
	}

	private void Start()
	{
		teamA_indc.SetActive(value: false);
		teamB_indc.SetActive(value: false);
		if (MultiplayerController.gameType == GameMode.PvP)
		{
			DeathMatchPanel.SetActive(value: true);
			TeamFightPanel.SetActive(value: false);
			ZombieSurvPanel.SetActive(value: false);
		}
		else if (MultiplayerController.gameType == GameMode.TeamFight)
		{
			DeathMatchPanel.SetActive(value: false);
			TeamFightPanel.SetActive(value: true);
			ZombieSurvPanel.SetActive(value: false);
		}
		else if (MultiplayerController.gameType == GameMode.BattleRoyalePvP)
		{
			BattleRoyalePvPPanel.SetActive(value: true);
		}
		else if (MultiplayerController.gameType == GameMode.BattleRoyaleTeams)
		{
			BattleRoyaleTeamsPanel.SetActive(value: true);
		}
	}

	private void OnOurPlayerCreated(CharacterMotor player)
	{
		StartCoroutine(OnOurPlayerCreateCrt(player));
		battleRoyalePvPalivesText.text = GameController.instance.AllPlayers().FindAll((CharacterMotor p) => p.IsAlive()).Count.ToString();
	}

	private IEnumerator OnOurPlayerCreateCrt(CharacterMotor player)
	{
		yield return null;
		yield return null;
		if (player.myTeam == TeamID.TeamA)
		{
			teamA_indc.SetActive(value: true);
		}
		else
		{
			teamB_indc.SetActive(value: true);
		}
	}

	public void UpdateFragsCount()
	{
		if (MultiplayerController.gameType == GameMode.TeamFight)
		{
			TeamA_fragsCount.text = GameController.instance.teamScores.TeamAScore.ToString();
			TeamB_fragsCount.text = GameController.instance.teamScores.TeamBScore.ToString();
		}
		else if (GameController.instance.OurPlayer != null)
		{
			fragsCount.text = GameController.instance.OurPlayer.fragsCount.ToString();
		}
	}

	public void UpdateCaptuedFlagsCount(int score1, int score2)
	{
		TeamA_fragsCount.text = score1.ToString();
		TeamB_fragsCount.text = score2.ToString();
	}

	private void OnDestroy()
	{
		GameController.OurPlayerCreated = (Action<CharacterMotor>)Delegate.Remove(GameController.OurPlayerCreated, new Action<CharacterMotor>(OnOurPlayerCreated));
		GameController.PlayerKilled = (Action<CharacterMotor>)Delegate.Remove(GameController.PlayerKilled, new Action<CharacterMotor>(OnPlayerKilled));
		GameController.PlayerKilled = (Action<CharacterMotor>)Delegate.Remove(GameController.PlayerKilled, new Action<CharacterMotor>(OnPlayerJoined));
	}

	private void OnPlayerKilled(CharacterMotor player)
	{
		battleRoyalePvPalivesText.text = GameController.instance.AllPlayers().FindAll((CharacterMotor p) => p.IsAlive()).Count.ToString();
	}

	private void OnPlayerJoined(CharacterMotor player)
	{
		battleRoyalePvPalivesText.text = GameController.instance.AllPlayers().FindAll((CharacterMotor p) => p.IsAlive()).Count.ToString();
	}
}
