using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamFightFinalTable : MonoBehaviour
{
	[SerializeField]
	private GameObject teamPlayerRowFinal;

	[SerializeField]
	private RectTransform teamAPlayersScrollContent;

	[SerializeField]
	private RectTransform teamBPlayersScrollContent;

	[SerializeField]
	private Text totalExpLabel;

	[SerializeField]
	private Text totalCoinsLabel;

	[SerializeField]
	private Text teamAScoreText;

	[SerializeField]
	private Text teamBScoreText;

	[SerializeField]
	private GameObject lockPanel;

	private List<GameObject> rows = new List<GameObject>();

	private TeamID winnerTeam;

	[SerializeField]
	private GameObject winLabel;

	[SerializeField]
	private GameObject defeatLabel;

	[SerializeField]
	private List<int> winRewards;

	[SerializeField]
	private List<int> defeatRewards;

	public void ShowTable()
	{
		base.gameObject.SetActive(value: true);
		lockPanel.SetActive(value: true);
		int num = 0;
		if (GameController.instance.teamScores.TeamAScore > 0 || GameController.instance.teamScores.TeamBScore > 0)
		{
			if (GameController.instance.teamScores.TeamAScore > GameController.instance.teamScores.TeamBScore)
			{
				winnerTeam = TeamID.TeamA;
			}
			else
			{
				winnerTeam = TeamID.TeamB;
			}
		}
		winLabel.SetActive(winnerTeam == GameController.instance.OurPlayer.myTeam);
		defeatLabel.SetActive(winnerTeam != GameController.instance.OurPlayer.myTeam);
		for (int num2 = rows.Count - 1; num2 >= 0; num2--)
		{
			Object.Destroy(rows[num2]);
		}
		rows.Clear();
		List<CharacterMotor> list = GameController.instance.AllPlayersNotDublicated();
		list.Sort((CharacterMotor x, CharacterMotor y) => y.fragsCount.CompareTo(x.fragsCount));
		float fragsSum = 0f;
		list.ForEach(delegate(CharacterMotor p)
		{
			fragsSum += p.fragsCount;
		});
		if (fragsSum == 0f)
		{
			fragsSum = 1f;
		}
		float num3 = list.Count * 5;
		int num4 = 0;
		int num5 = 0;
		foreach (CharacterMotor item in list)
		{
			TeamPlayerRowFinal component = Object.Instantiate(teamPlayerRowFinal).GetComponent<TeamPlayerRowFinal>();
			if (item.myTeam == TeamID.TeamA)
			{
				component.transform.SetParent(teamAPlayersScrollContent);
				num4 += item.playerInfo.score;
			}
			else
			{
				component.transform.SetParent(teamBPlayersScrollContent);
				num5 += item.playerInfo.score;
			}
			component.transform.localScale = Vector3.one;
			component.SetName(item.playerInfo.name);
			component.SetKills(item.fragsCount);
			component.SetKillAssists(item.playerInfo.killAssistsCount);
			component.SetDeaths(item.playerInfo.deathCount);
			component.SetScore(item.playerInfo.score);
			component.SetIcon(item.playerInfo.playerIcon);
			rows.Add(component.gameObject);
			if (item == GameController.instance.OurPlayer)
			{
				component.HighlightAsSelf();
			}
		}
		teamAScoreText.text = GameController.instance.teamScores.TeamAScore.ToString();
		teamBScoreText.text = GameController.instance.teamScores.TeamBScore.ToString();
		int num6 = (int)Mathf.Ceil((float)GameController.instance.OurPlayer.fragsCount / fragsSum * num3);
		if (num6 == 0)
		{
			num6 = 1;
		}
		if (GameController.instance.OurPlayer.myTeam == winnerTeam)
		{
			num6++;
		}
		num = num6;
		totalExpLabel.text = num.ToString();
		totalCoinsLabel.text = num6.ToString();
		LocalStore.GiveMoney(num6);
		StatisticsManager.AddExp(num);
		Invoke("ShowAds", 0.1f);
		GameController.instance.ResetTeamScore();
	}

	private void ShowAds()
	{
		lockPanel.SetActive(value: false);
	}

	public void HideTable()
	{
		lockPanel.SetActive(value: false);
		base.gameObject.SetActive(value: false);
	}

	public void OnLoadMenu()
	{
		GameController.instance.GoToMainMenu();
	}

	public void OnPlayAgain()
	{
		GameController.instance.OnClickPlayAgain();
	}
}
