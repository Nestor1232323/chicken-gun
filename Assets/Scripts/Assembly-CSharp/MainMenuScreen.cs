using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : BaseScreen
{
	public Transform CameraPosForThisScreen;

	public Text playerNamePlaceHolder;

	public InputField playerNameInput;

	public Transform playerPosForThisScreen;

	public IDamageReciver fuck;

	public Text gameVersionLabel;

	public GameObject vkButton;

	public GameObject rateButton;

	[SerializeField]
	private GameObject nameSuggestionWidget;

	[SerializeField]
	private Dropdown namesDropDown;

	[SerializeField]
	private GameObject threeDTitle;

	[SerializeField]
	private GameObject noInternetAlert;

	private const int playerNameLength = 13;

	private bool autoJoinGameAfterConnection;

	public override void Awake()
	{
		base.Awake();
	}

	private void Start()
	{
		gameVersionLabel.text = Application.version;
		Camera.main.transform.position = CameraPosForThisScreen.position;
		Camera.main.transform.rotation = CameraPosForThisScreen.rotation;
		if (StorageController.instance.PlayerName != "Player")
		{
			if (StorageController.instance.PlayerName.Length > 13)
			{
				StorageController.instance.PlayerName = StorageController.instance.PlayerName.Substring(0, 13);
			}
			playerNameInput.text = StorageController.instance.PlayerName;
			playerNamePlaceHolder.gameObject.SetActive(value: false);
		}
		playerNameInput.text = StorageController.instance.PlayerName;
		OnShow();
		MultiplayerController.PhotonConnected = (Action)Delegate.Combine(MultiplayerController.PhotonConnected, new Action(OnConnectedToPhoton));
		MultiplayerController.JoinedToLobby = (Action)Delegate.Combine(MultiplayerController.JoinedToLobby, new Action(OnJoinedToPhotonLobby));
		MultiplayerController instance = MultiplayerController.instance;
		instance.ConnectionFailed = (Action)Delegate.Combine(instance.ConnectionFailed, new Action(OnConnectionFail));
	}

	private void OnDestroy()
	{
		MultiplayerController.PhotonConnected = (Action)Delegate.Remove(MultiplayerController.PhotonConnected, new Action(OnConnectedToPhoton));
		MultiplayerController.JoinedToLobby = (Action)Delegate.Remove(MultiplayerController.JoinedToLobby, new Action(OnJoinedToPhotonLobby));
		MultiplayerController instance = MultiplayerController.instance;
		instance.ConnectionFailed = (Action)Delegate.Remove(instance.ConnectionFailed, new Action(OnConnectionFail));
	}

	protected override void OnShow()
	{
		ShopController.instance.CreateMenuPlayer();
		Camera.main.transform.position = CameraPosForThisScreen.position;
		Camera.main.transform.rotation = CameraPosForThisScreen.rotation;
		if ((bool)threeDTitle)
		{
			threeDTitle.SetActive(value: true);
		}
		MultiplayerController.instance.DisconnectToPhoton();
	}

	protected override void OnHide()
	{
		base.OnHide();
		Camera.main.transform.SetParent(null);
		if ((bool)threeDTitle)
		{
			threeDTitle.SetActive(value: false);
		}
	}

	public void OnShopBtnClick()
	{
		ShowAdsOnMenubtnsClick();
		ScreenManager.instance.ShowScreen(ScreenManager.instance.shopScreen);
	}

	public void OnMultiplayerBtnClick()
	{
		ShowAdsOnMenubtnsClick();
		MultiplayerController.gameType = GameMode.TeamFight;
		MultiplayerController.instance.selectedMapID = DataModel.instance.GetRandomMap(GameMode.TeamFight);
		ScreenManager.instance.ShowScreen(ScreenManager.instance.gameConnectionScreen);
	}

	public void OnTeamFightBtnClick()
	{
		ShowAdsOnMenubtnsClick();
		autoJoinGameAfterConnection = true;
		ScreenManager.instance.ShowLoading(show: true);
		MultiplayerController.gameType = GameMode.TeamFight;
		MultiplayerController.instance.selectedMapID = DataModel.instance.GetRandomMap(GameMode.TeamFight);
		MultiplayerController.instance.ConnectToPhoton();
	}

	public void OnBattleRoyaleBtnClick()
	{
		ShowAdsOnMenubtnsClick();
		autoJoinGameAfterConnection = true;
		ScreenManager.instance.ShowLoading(show: true);
		MultiplayerController.gameType = GameMode.BattleRoyalePvP;
		MultiplayerController.instance.selectedMapID = DataModel.instance.GetRandomMap(GameMode.BattleRoyalePvP);
		MultiplayerController.instance.ConnectToPhoton();
	}

	public void OnOfflineBtnClick()
	{
	}

	private void ShowAdsOnMenubtnsClick()
	{
	}

	private void OnConnectedToPhoton()
	{
	}

	private void OnConnectionFail()
	{
		ScreenManager.instance.ShowLoading(show: false);
	}

	private void OnJoinedToPhotonLobby()
	{
		if (base.isActiveAndEnabled && autoJoinGameAfterConnection)
		{
			MultiplayerController.instance.JoinRandomGame();
			autoJoinGameAfterConnection = true;
		}
	}

	public void handlePlayerNameInput()
	{
	}

	public void OnEndNameInputEdit()
	{
		if (!string.IsNullOrEmpty(playerNameInput.text))
		{
			string text = playerNameInput.text;
			if (text.Length > 13)
			{
				text = text.Substring(0, 13);
			}
			StorageController.instance.PlayerName = text;
		}
	}

	public void OnFacebookBtn()
	{
		Application.OpenURL("https://www.facebook.com/chaloapps.chaloapps");
	}

	public void OnVKBtn()
	{
		Application.OpenURL("https://vk.com/club143386452");
	}

	public void OnRateAppBtn()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			Application.OpenURL("https://play.google.com/store/apps/details?id=com.chaloapps.shootersandbox");
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Application.OpenURL("https://itunes.apple.com/ru/app/strikebox-team-fight-shooter/id1435785527");
		}
		else
		{
			Application.OpenURL("https://play.google.com/store/apps/details?id=com.chaloapps.shootersandbox");
		}
	}

	public void OpenGamblingScreen()
	{
		ScreenManager.instance.ShowScreen(ScreenManager.instance.gamblingScreen);
	}
}
