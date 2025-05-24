using System.Collections.Generic;
using Photon;
using UnityEngine;

public class ChatController : Photon.MonoBehaviour
{
	public class ChatMessage
	{
		public string text;

		public float time;

		public ChatMessage(string text, float t)
		{
			this.text = text;
			time = t;
		}
	}

	public static ChatController Instance;

	private TouchScreenKeyboard touchScreenKeyboard;

	private const int MAX_MESSAGE_LENGTH = 45;

	private const int MAX_LINES_COUNT = 5;

	private const int DEL_FIRST_LINE_PERIOD = 30;

	public Dictionary<int, string> playerChatColors = new Dictionary<int, string>();

	private List<ChatMessage> ChatMessages = new List<ChatMessage>();

	private string _playerName => GameController.instance.OurPlayer.playerInfo.name;

	private void Start()
	{
		if (GameWindow.instance == null)
		{
			Object.Destroy(this);
		}
		else if (base.photonView.isMine)
		{
			Instance = this;
		}
	}

	public string[] GetChatStrings()
	{
		string[] array = new string[5];
		for (int i = 0; i < Instance.ChatMessages.Count; i++)
		{
			array[i] = Instance.ChatMessages[i].text;
		}
		return array;
	}

	private void Update()
	{
		if (base.photonView.isMine && touchScreenKeyboard != null)
		{
			if (touchScreenKeyboard.status == TouchScreenKeyboard.Status.Done)
			{
				AddMessage(touchScreenKeyboard.text);
				touchScreenKeyboard = null;
			}
			else if (touchScreenKeyboard.text.Length > 45 - _playerName.Length - 1)
			{
				touchScreenKeyboard.text = touchScreenKeyboard.text.Substring(0, 45 - _playerName.Length - 1);
			}
		}
		for (int num = ChatMessages.Count - 1; num >= 0; num--)
		{
			ChatMessages[num].time -= Time.deltaTime;
			if (ChatMessages[num].time < 0f)
			{
				ChatMessages.RemoveAt(num);
				GameWindow.instance.SetChatText(GetChatStrings());
			}
		}
	}

	public void OpenKeyboard()
	{
		touchScreenKeyboard = TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.Default, autocorrection: false, multiline: false, secure: false, alert: false, "Enter your message");
	}

	public void AddMessage(string message)
	{
		if (!string.IsNullOrEmpty(message))
		{
			base.photonView.RPC("AddMessage2RPC", PhotonTargets.All, message, GameController.instance.OurPlayer.playerInfo.name, GameController.instance.OurPlayer.myTeam);
		}
	}

	public void AddSimpleMessage(string message, bool sync = true)
	{
		if (!string.IsNullOrEmpty(message))
		{
			if (sync)
			{
				base.photonView.RPC("AddMessageRPC", PhotonTargets.All, message);
			}
			else
			{
				AddMessageRPC(message);
			}
		}
	}

	[PunRPC]
	private void AddMessageRPC(string message)
	{
		if (Instance != null)
		{
			Instance.ChatMessages.Add(new ChatMessage(message, 30f));
			if (Instance.ChatMessages.Count > 5)
			{
				DeleteFirstLine();
			}
			if (GameWindow.instance != null)
			{
				GameWindow.instance.SetChatText(GetChatStrings());
			}
		}
	}

	private void DeleteFirstLine()
	{
		if (Instance.ChatMessages.Count > 0)
		{
			Instance.ChatMessages.RemoveAt(0);
			if (GameWindow.instance != null)
			{
				GameWindow.instance.SetChatText(GetChatStrings());
			}
		}
	}

	[PunRPC]
	private void AddMessage2RPC(string message, string playerName, TeamID team)
	{
		if (Instance != null)
		{
			string text = string.Empty;
			if (team != TeamID.None)
			{
				text = ((!GameController.instance.IsTeamMate(team)) ? GeneralUtils.ColorToHex(DataModel.instance.teamB_Color) : GeneralUtils.ColorToHex(DataModel.instance.teamA_Color));
			}
			string text2 = "<b><color=#" + text + ">" + playerName + "</color></b> : " + message;
			Instance.ChatMessages.Add(new ChatMessage(text2, 30f));
			if (Instance.ChatMessages.Count > 5)
			{
				DeleteFirstLine();
			}
			GameWindow.instance.SetChatText(GetChatStrings());
		}
	}
}
