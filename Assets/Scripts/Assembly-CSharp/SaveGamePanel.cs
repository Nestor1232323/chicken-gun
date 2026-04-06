using System;
using UnityEngine;
using UnityEngine.UI;

public class SaveGamePanel : MonoBehaviour
{
	public InputField gameNameInput;

	public void OnSaveBtnClick()
	{
		string text = DateTime.Now.ToString("hh:mm:ss");
		string gameTitle = gameNameInput.text + " game_" + text;
		GameSavingManager.SaveCurrentGameWithName(gameTitle);
		base.gameObject.SetActive(false);
	}
}
