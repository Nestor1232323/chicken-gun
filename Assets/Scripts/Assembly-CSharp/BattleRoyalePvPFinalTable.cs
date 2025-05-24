using System;
using UnityEngine;
using UnityEngine.UI;

public class BattleRoyalePvPFinalTable : MonoBehaviour
{
	[SerializeField]
	private Text winnerNameText;

	private void Awake()
	{
		GameController.BattleRoyaleFinished = (Action<string>)Delegate.Combine(GameController.BattleRoyaleFinished, new Action<string>(OnBattleRoyaleFinished));
	}

	private void OnDestroy()
	{
		GameController.BattleRoyaleFinished = (Action<string>)Delegate.Remove(GameController.BattleRoyaleFinished, new Action<string>(OnBattleRoyaleFinished));
	}

	public void ShowTable()
	{
		base.gameObject.SetActive(value: true);
	}

	private void OnBattleRoyaleFinished(string winnerName)
	{
		if (!string.IsNullOrEmpty(winnerName))
		{
			winnerNameText.text = winnerName;
		}
		else
		{
			winnerNameText.text = "Nobody O_o";
		}
		Invoke("GoToMainMenu", 2.5f);
	}

	private void GoToMainMenu()
	{
		GameController.instance.GoToMainMenu();
	}
}
