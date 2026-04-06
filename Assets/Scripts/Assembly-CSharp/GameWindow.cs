using System;
using System.Collections;
using Photon;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameWindow : Photon.MonoBehaviour
{
	public static GameWindow instance;

	public RectTransform TopInfoPanel;

	public RectTransform CarControlsPanel;

	public RectTransform PlayerControlsPanel;

	public GameObject HeliControlsPanel;

	public RectTransform PlaneControlsPanel;

	public RectTransform HorseControlsPanel;

	public RectTransform PausePanel;

	public RectTransform RaceResultsPanel;

	[SerializeField]
	private Text ammoCount;

	[SerializeField]
	private Text grenadesCount;

	public Image AimSprite;

	public Text PhotonConnectionLog;

	public GameObject PhotonConnectionPanel;

	public GameObject MainUI;

	public TeamFightFinalTable teamFightFinalTable;

	public CaptureFlagFinalTable captureFlagFinalTable;

	public FinalTable dmFinalTable;

	public BattleRoyalePvPFinalTable battleRoyalePvPFinalTable;

	public Text timelLabel;

	public FragsWidget fragsWidget;

	public Text[] chatLines;

	public GameObject ChatWidget;

	public Canvas gameWindowCanvas;

	public CanvasGroup redDamagePanel;

	public TeamSelectionPanel teamSelectionPanel;

	public GameObject getInCarButton;

	public GameObject leaveCarButton;

	public InventaryWidget inventaryWidget;

	public GameInputController gameInputController;

	public InventoryInputController inventoryInputController;

	public GameObject sniperAimPanel;

	public GameObject cameraModeBtn;

	public KickOutWindow kickOutWindow;

	[SerializeField]
	private Image damageFXPanel;

	public Image jetpackFuelIndicator;

	[SerializeField]
	private GameObject sniperModeBtn;

	[SerializeField]
	private Image crosshair;

	private TimeSpan ts;

	public InputField testChat;

	public RespawnFadePanel respawnPanel;

	public GameObject WaitingOtherPlayersPanel;

	public GameObject survivalEndPanel;

	[SerializeField]
	private Text smokeGrenadesCount;

	[SerializeField]
	private Text molotovGrenadesCount;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		if (GameController.gameConfigData.gameMode == GameMode.ZombieSurvival)
		{
			timelLabel.gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		UpdateLevelTime();
	}

	public void OnMatchFinished()
	{
		MainUI.SetActive(false);
		if (MultiplayerController.gameType == GameMode.TeamFight)
		{
			teamFightFinalTable.ShowTable();
		}
		else if (MultiplayerController.gameType == GameMode.PvP)
		{
			dmFinalTable.Show();
		}
		else if (MultiplayerController.gameType == GameMode.CaptureFlag)
		{
			captureFlagFinalTable.ShowTable();
		}
		else if (MultiplayerController.gameType == GameMode.BattleRoyalePvP)
		{
			battleRoyalePvPFinalTable.ShowTable();
		}
		else if (MultiplayerController.gameType != GameMode.BattleRoyaleTeams)
		{
		}
	}

	public void OnContinurGame()
	{
		MainUI.SetActive(true);
		if (MultiplayerController.gameType == GameMode.TeamFight)
		{
			teamFightFinalTable.HideTable();
		}
		else if (MultiplayerController.gameType == GameMode.PvP)
		{
			dmFinalTable.Hide();
		}
		else if (MultiplayerController.gameType == GameMode.CaptureFlag)
		{
			captureFlagFinalTable.HideTable();
		}
	}

	public void ShowMainUI(bool show)
	{
		MainUI.SetActive(show);
	}

	public void OnEnterTPSCameraMode()
	{
	}

	public void OnEnterFPSCameraMode()
	{
	}

	public void ShowDamageFX()
	{
	}

	public void OnMenuButtonClick()
	{
		SceneManager.LoadScene("MainMenu");
	}

	public void OnRestartButtonClick()
	{
		SceneManager.LoadScene("GameScene");
	}

	public IEnumerator AnimatePanel(RectTransform panel, Vector3 startPos, Vector3 endPos, float duration)
	{
		float t = 0f;
		while (t < duration)
		{
			t += Time.deltaTime;
			panel.anchoredPosition3D = Vector3.Lerp(startPos, endPos, t / duration);
			yield return null;
		}
		panel.anchoredPosition3D = endPos;
	}

	public void ShowPanel(RectTransform panel, float duration, float startAlpha, float endAlpha)
	{
		StartCoroutine(ShowPanelCrt(panel, duration, startAlpha, endAlpha));
	}

	private IEnumerator ShowPanelCrt(RectTransform panel, float duration, float startAlpha, float endAlpha)
	{
		if (startAlpha == -1f)
		{
			startAlpha = panel.GetComponent<CanvasGroup>().alpha;
		}
		CanvasGroup cg = panel.GetComponent<CanvasGroup>();
		float t = 0f;
		while (t < duration)
		{
			t += Time.deltaTime;
			cg.alpha = Mathf.Lerp(startAlpha, endAlpha, t / duration);
			yield return null;
		}
		cg.alpha = endAlpha;
	}

	public void OnPause()
	{
		PausePanel.gameObject.SetActive(true);
	}

	public void OnPauseExit()
	{
		PausePanel.gameObject.SetActive(false);
	}

	public void OnNextWeapon(int sign = 0)
	{
		GameController.instance.OurPlayer.SelectNextWeapon(sign);
	}

	public void PrintPhotonLog(string log)
	{
		Text photonConnectionLog = PhotonConnectionLog;
		photonConnectionLog.text = photonConnectionLog.text + "\n" + log;
	}

	public void HidePhotonConnectionPanel()
	{
		PhotonConnectionPanel.gameObject.SetActive(false);
	}

	private void UpdateLevelTime()
	{
		ts = TimeSpan.FromSeconds((MultiplayerController.gameType != GameMode.Sandbox) ? GameController.instance.TimeUntilTheEndOfLevel : ((double)Time.timeSinceLevelLoad));
		timelLabel.text = string.Format("{0:D2}:{1:D2}", ts.Minutes, ts.Seconds);
	}

	public void UpdateFragsCount()
	{
		fragsWidget.UpdateFragsCount();
	}

	public void SetChatText(string[] lines)
	{
		for (int i = 0; i < chatLines.Length; i++)
		{
			if (chatLines[i] != null)
			{
				chatLines[i].transform.parent.gameObject.SetActive(false);
			}
		}
		for (int j = 0; j < chatLines.Length; j++)
		{
			if (chatLines[j] == null)
			{
				continue;
			}
			if (j < lines.Length)
			{
				chatLines[j].transform.parent.gameObject.SetActive(true);
				chatLines[j].text = lines[j];
				if (chatLines[j].text == string.Empty)
				{
					chatLines[j].transform.parent.gameObject.SetActive(false);
				}
			}
			else
			{
				chatLines[j].text = string.Empty;
				chatLines[j].transform.parent.gameObject.SetActive(false);
			}
		}
	}

	public void TestChatText()
	{
		ChatController.Instance.AddMessage(testChat.text);
	}

	public void OnWriteChatMeesgeBtn()
	{
		ChatController.Instance.OpenKeyboard();
	}

	public void OnOpenChatBtn()
	{
		ChatWidget.SetActive(!ChatWidget.activeSelf);
	}

	public void ShowRespawnWhiteScreen(bool show)
	{
		respawnPanel.Show();
	}

	public void HighlightAim()
	{
		AimSprite.color = new Color(0.8f, 0f, 0f, 0.67f);
	}

	public void DeHighlightAim()
	{
		AimSprite.color = new Color(0.8f, 0.8f, 0.8f, 0.67f);
	}

	public void HideWaitingOtherPlayersPanel(int roomPlayersCount = 0)
	{
		WaitingOtherPlayersPanel.SetActive(false);
	}

	public void ShowSurvivalPnel()
	{
		survivalEndPanel.SetActive(true);
	}

	public void ShowTeamJoiningPanel()
	{
		teamSelectionPanel.Show();
	}

	public void HideTeamJoiningPanel()
	{
		MainUI.SetActive(true);
		teamSelectionPanel.gameObject.SetActive(false);
	}

	public void ShowGetInCarBtn(bool show)
	{
		getInCarButton.SetActive(show);
	}

	public void OnEnterPlayerMode()
	{
		CarControlsPanel.gameObject.SetActive(false);
		PlayerControlsPanel.gameObject.SetActive(true);
		getInCarButton.SetActive(false);
		inventaryWidget.ShawInventoryCallBtn(true);
		HeliControlsPanel.SetActive(false);
		PlaneControlsPanel.gameObject.SetActive(false);
		HorseControlsPanel.gameObject.SetActive(false);
		cameraModeBtn.SetActive(true);
		ShowCrosshair(true);
	}

	public void OnEnterVehicleMode(VehicleType type)
	{
		switch (type)
		{
		case VehicleType.Car:
		case VehicleType.Moto:
			CarControlsPanel.gameObject.SetActive(true);
			break;
		case VehicleType.Heli:
			HeliControlsPanel.SetActive(true);
			break;
		case VehicleType.Plane:
			PlaneControlsPanel.gameObject.SetActive(true);
			break;
		case VehicleType.Horse:
			HorseControlsPanel.gameObject.SetActive(true);
			break;
		}
		PlayerControlsPanel.gameObject.SetActive(false);
		inventaryWidget.ShawAllInventoryWidgetUI(false);
		cameraModeBtn.SetActive(false);
	}

	public void ShowInventary(bool show = true)
	{
		if (show)
		{
			inventaryWidget.Show();
		}
		else
		{
			inventaryWidget.CloseWidget();
		}
	}

	public void OnEnterSniperMode(bool enter)
	{
		sniperAimPanel.SetActive(enter);
	}

	public void ShowKickOutWindow()
	{
		kickOutWindow.Show(true);
	}

	private void OnDestroy()
	{
		instance = null;
	}

	public void ShowCrosshair(bool val)
	{
		crosshair.gameObject.SetActive(val);
	}

	public void HighlightCrosshair(bool val)
	{
		if (val)
		{
			crosshair.color = Color.red;
		}
		else
		{
			crosshair.color = Color.white;
		}
	}

	public void SetAmmo(int ammo)
	{
		ammoCount.text = ammo.ToString();
	}

	public void SetGrenades(int val, int smokes, int molotov)
	{
		grenadesCount.text = val.ToString();
		grenadesCount.transform.parent.gameObject.SetActive(val != 0);
		smokeGrenadesCount.text = smokes.ToString();
		smokeGrenadesCount.transform.parent.gameObject.SetActive(smokes != 0);
		molotovGrenadesCount.text = molotov.ToString();
		molotovGrenadesCount.transform.parent.gameObject.SetActive(molotov != 0);
	}
}
