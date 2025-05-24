using System;
using UnityEngine;
using UnityEngine.UI;

public class SaveGamePanel : MonoBehaviour
{
	public InputField gameNameInput;

	public void OnSaveBtnClick()
	{
		string text = DateTime.Now.ToString("hh:mm:ss");
		GameSavingManager.SaveCurrentGameWithName(gameNameInput.text + " game_" + text);
		base.gameObject.SetActive(value: false);
	}
}
