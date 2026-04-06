using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MyKillerWindow : MonoBehaviour
{
	[SerializeField]
	private GameObject myWindow;

	[SerializeField]
	private Text killerName;

	[SerializeField]
	private Button continueBtn;

	[SerializeField]
	private Text counterText;

	private IEnumerator Start()
	{
		GameController.MatchFinidhed += OnMatchFinished;
		while (GameController.instance.OurPlayer == null)
		{
			yield return null;
		}
		CharacterMotor ourPlayer = GameController.instance.OurPlayer;
		ourPlayer.PlayerCrashed = (Action<UnityEngine.Object>)Delegate.Combine(ourPlayer.PlayerCrashed, new Action<UnityEngine.Object>(OnPlayerKilled));
	}

	private void OnDestroy()
	{
		if (!(GameController.instance == null) && !(GameController.instance.OurPlayer == null))
		{
			CharacterMotor ourPlayer = GameController.instance.OurPlayer;
			ourPlayer.PlayerCrashed = (Action<UnityEngine.Object>)Delegate.Remove(ourPlayer.PlayerCrashed, new Action<UnityEngine.Object>(OnPlayerKilled));
			GameController.MatchFinidhed -= OnMatchFinished;
		}
	}

	private void OnPlayerKilled(object player)
	{
		continueBtn.interactable = false;
		GameWindow.instance.ShowMainUI(false);
		myWindow.SetActive(true);
		if ((player as CharacterMotor).LastKiller != null)
		{
			killerName.text = (player as CharacterMotor).LastKiller.playerInfo.name;
			killerName.gameObject.SetActive(true);
		}
		else
		{
			killerName.gameObject.SetActive(false);
		}
		if (MultiplayerController.gameType != GameMode.BattleRoyalePvP && MultiplayerController.gameType != GameMode.BattleRoyaleTeams)
		{
			StartCoroutine(CounterCRT());
		}
	}

	public void OnContinueClick()
	{
		GameWindow.instance.ShowMainUI(true);
		myWindow.SetActive(false);
		CarOrPlayerSwitcher.instance.DisableKillerCamera();
	}

	public void OnMatchFinished()
	{
		if (!(myWindow == null) && myWindow.activeSelf)
		{
			CarOrPlayerSwitcher.instance.DisableKillerCamera(false);
			if (myWindow != null)
			{
				myWindow.SetActive(false);
			}
		}
	}

	private IEnumerator CounterCRT()
	{
		counterText.gameObject.SetActive(true);
		float T = 3f;
		float t = T;
		counterText.text = t.ToString();
		while (t > 0f)
		{
			counterText.text = t.ToString("F1");
			t -= Time.deltaTime;
			yield return null;
		}
		continueBtn.interactable = true;
		counterText.gameObject.SetActive(false);
	}
}
