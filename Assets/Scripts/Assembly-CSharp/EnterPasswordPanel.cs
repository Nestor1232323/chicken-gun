using UnityEngine;
using UnityEngine.UI;

public class EnterPasswordPanel : MonoBehaviour
{
	[SerializeField]
	private InputField passInput;

	private RoomInfo room;

	[SerializeField]
	private GameObject passwordIsIncorrect;

	[SerializeField]
	private GameConnectionScreen gameConnectionScreen;

	public void Show(RoomInfo room)
	{
		this.room = room;
		base.gameObject.SetActive(true);
	}

	public void CloseWindow()
	{
		room = null;
		base.gameObject.SetActive(false);
	}

	public void OnJoinBtnClick()
	{
		passwordIsIncorrect.SetActive(false);
		string text = passInput.text;
		string text2 = (string)room.CustomProperties["pass"];
		if (string.IsNullOrEmpty(text2))
		{
			base.gameObject.SetActive(false);
			return;
		}
		base.gameObject.SetActive(true);
		if (text == text2)
		{
			base.gameObject.SetActive(false);
			gameConnectionScreen.LoadingPanel.SetActive(true);
			MultiplayerController.instance.JoinRandomRoomWithThisName(room.Name);
		}
		else
		{
			passwordIsIncorrect.SetActive(true);
		}
	}
}
