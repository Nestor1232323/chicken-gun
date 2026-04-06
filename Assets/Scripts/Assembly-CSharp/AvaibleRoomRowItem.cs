using UnityEngine;
using UnityEngine.UI;

public class AvaibleRoomRowItem : MonoBehaviour
{
	public Text roomName;

	public Text maxPlayersCount;

	public Text currentPlayersCount;

	public Text mode;

	public Text mapTitle;

	public Image mapIcon;

	[SerializeField]
	private GameObject passswordLockImg;

	public void SetData(RoomInfo roomInfo)
	{
		GameMode gameMode = (GameMode)(int)roomInfo.CustomProperties["mode"];
		if (gameMode == GameMode.TeamFight)
		{
			roomName.text = roomInfo.Name;
		}
		else
		{
			roomName.text = roomInfo.Name;
		}
		maxPlayersCount.text = roomInfo.MaxPlayers.ToString();
		currentPlayersCount.text = roomInfo.PlayerCount.ToString();
		mode.text = gameMode.ToString();
		MapID id = (MapID)(int)roomInfo.CustomProperties["map"];
		mapTitle.text = id.ToString();
		mapIcon.sprite = DataModel.instance.GetMap(id).icon;
		string value = (string)roomInfo.CustomProperties["pass"];
		passswordLockImg.SetActive(!string.IsNullOrEmpty(value));
	}
}
