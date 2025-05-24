using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CaptureFlagFinalTable : MonoBehaviour
{
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

	private TeamID winnerTeam;

	private void Start()
	{
	}

	public void ShowTable()
	{
		CaptureFlagModeManager captureFlagModeManager = Object.FindObjectOfType<CaptureFlagModeManager>();
		base.gameObject.SetActive(value: true);
		lockPanel.SetActive(value: true);
		int num = 0;
		int num2 = 0;
		if (captureFlagModeManager.teamAScore > 0 || captureFlagModeManager.teamBScore > 0)
		{
			if (captureFlagModeManager.teamAScore > captureFlagModeManager.teamBScore)
			{
				winnerTeam = TeamID.TeamA;
			}
			else
			{
				winnerTeam = TeamID.TeamB;
			}
		}
		if (GameController.instance.OurPlayer.myTeam == TeamID.TeamA)
		{
			teamAScoreText.text = captureFlagModeManager.teamAScore.ToString();
			teamBScoreText.text = captureFlagModeManager.teamBScore.ToString();
		}
		else
		{
			teamAScoreText.text = captureFlagModeManager.teamBScore.ToString();
			teamBScoreText.text = captureFlagModeManager.teamAScore.ToString();
		}
		num2 = 3;
		if (GameController.instance.OurPlayer.myTeam == winnerTeam)
		{
			num2 = 7;
		}
		num = num2 * 2;
		totalExpLabel.text = num.ToString();
		totalCoinsLabel.text = num2.ToString();
		LocalStore.GiveMoney(num2);
		StatisticsManager.AddExp(num);
		Invoke("ShowAds", 0.35f);
		captureFlagModeManager.ResetScores();
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
		SceneManager.LoadScene("MainMenu");
	}

	public void OnPlayAgain()
	{
		GameController.instance.OnClickPlayAgain();
	}
}
