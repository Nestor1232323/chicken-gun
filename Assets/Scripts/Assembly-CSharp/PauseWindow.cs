using UnityEngine;

public class PauseWindow : MonoBehaviour
{
	public static PauseWindow instance;

	public GameObject saveGamePanel;

	[HideInInspector]
	public bool isOpened;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
	}

	public void Open()
	{
	}

	private void Update()
	{
	}

	public void Exit()
	{
	}

	public void OnLoadMenu()
	{
		GameController.instance.GoToMainMenu();
	}

	public void OnResume()
	{
		base.gameObject.SetActive(value: false);
	}

	public void ShowSaveGamePanel(bool show)
	{
		saveGamePanel.SetActive(show);
	}
}
