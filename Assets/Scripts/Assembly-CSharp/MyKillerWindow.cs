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
		GameWindow.instance.ShowMainUI(show: false);
		myWindow.SetActive(value: true);
		if ((player as CharacterMotor).LastKiller != null)
		{
			killerName.text = (player as CharacterMotor).LastKiller.playerInfo.name;
			killerName.gameObject.SetActive(value: true);
		}
		else
		{
			killerName.gameObject.SetActive(value: false);
		}
		if (MultiplayerController.gameType != GameMode.BattleRoyalePvP && MultiplayerController.gameType != GameMode.BattleRoyaleTeams)
		{
			StartCoroutine(CounterCRT());
		}
	}

	public void OnContinueClick()
	{
		GameWindow.instance.ShowMainUI(show: true);
		myWindow.SetActive(value: false);
		CarOrPlayerSwitcher.instance.DisableKillerCamera();
	}

	public void OnMatchFinished()
	{
		if (!(myWindow == null) && myWindow.activeSelf)
		{
			CarOrPlayerSwitcher.instance.DisableKillerCamera(withRespawn: false);
			if (myWindow != null)
			{
				myWindow.SetActive(value: false);
			}
		}
	}

	private IEnumerator CounterCRT()
	{
		counterText.gameObject.SetActive(value: true);
		float num = 3f;
		float t = num;
		counterText.text = t.ToString();
		while (t > 0f)
		{
			counterText.text = t.ToString("F1");
			t -= Time.deltaTime;
			yield return null;
		}
		continueBtn.interactable = true;
		counterText.gameObject.SetActive(value: false);
	}
}
