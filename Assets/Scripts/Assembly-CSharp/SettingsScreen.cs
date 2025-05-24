using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : BaseScreen
{
	public Toggle vibroToggle;

	public InputField nameInput;

	public GameObject[] controlTypeButtons;

	public Sprite ActiveControlButtonSprite;

	public Sprite[] controlButtonsImages;

	private void Start()
	{
	}

	public void SetCarControlType(int i)
	{
	}

	public void handleVibroChange()
	{
		StorageController.instance.IsVibroAllowed = (vibroToggle.isOn ? 1 : 0);
	}

	public void handlePlayerNameInput()
	{
		if (!string.IsNullOrEmpty(nameInput.text))
		{
			MonoBehaviour.print("handlePlayerNameInput = " + nameInput.text);
			StorageController.instance.PlayerName = nameInput.text;
		}
	}

	public override void MoveToPreviousScreen()
	{
		base.MoveToPreviousScreen();
		StorageController.instance.PlayerName = nameInput.text;
	}
}
