using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
	public static ScreenManager instance;

	public BaseScreen currentScreen;

	public BaseScreen mainMenuScreen;

	public BaseScreen arenaSetupScreen;

	public BaseScreen multiplayerConnectScreen;

	public BaseScreen bankScreen;

	public BaseScreen gameTypeSelectScreen;

	public BaseScreen gameConnectionScreen;

	public BaseScreen loadSavedGameScreen;

	public BaseScreen highscoresScreen;

	public BaseScreen settingsScreen;

	public BaseScreen shopScreen;

	public BaseScreen freeMoneyScreen;

	public BaseScreen gamblingScreen;

	public GameObject loadingIndicator;

	public GameObject notEnoughMoneyPanel;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	public void ShowScreen(BaseScreen screen)
	{
		screen.previousScreen = currentScreen;
		currentScreen.Hide();
		screen.Show();
		currentScreen = screen;
	}

	public void HideScreen(BaseScreen screen)
	{
		screen.Hide();
		screen.previousScreen.Show();
		currentScreen = screen.previousScreen;
	}

	public void ShowLoading(bool show)
	{
		loadingIndicator.SetActive(show);
	}

	public void ShowNotEnoughMoneyPanel(bool show)
	{
		notEnoughMoneyPanel.SetActive(show);
	}

	public bool IsINMainMenu()
	{
		return currentScreen == mainMenuScreen;
	}

	public bool IsInMenuScene()
	{
		return SceneManager.GetActiveScene().name == "MainMenu";
	}
}
