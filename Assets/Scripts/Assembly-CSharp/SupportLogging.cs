using System.Text;
using UnityEngine;

public class SupportLogging : MonoBehaviour
{
	public bool LogTrafficStats;

	public void Start()
	{
		if (LogTrafficStats)
		{
			InvokeRepeating("LogStats", 10f, 10f);
		}
	}

	protected void OnApplicationPause(bool pause)
	{
		Debug.Log("SupportLogger OnApplicationPause: " + pause + " connected: " + PhotonNetwork.connected);
	}

	public void OnApplicationQuit()
	{
		CancelInvoke();
	}

	public void LogStats()
	{
		if (LogTrafficStats)
		{
			Debug.Log("SupportLogger " + PhotonNetwork.NetworkStatisticsToString());
		}
	}

	private void LogBasics()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("SupportLogger Info: PUN {0}: ", "1.92");
		stringBuilder.AppendFormat("AppID: {0}*** GameVersion: {1} PeerId: {2} ", PhotonNetwork.networkingPeer.AppId.Substring(0, 8), PhotonNetwork.networkingPeer.AppVersion, PhotonNetwork.networkingPeer.PeerID);
		stringBuilder.AppendFormat("Server: {0}. Region: {1} ", PhotonNetwork.ServerAddress, PhotonNetwork.networkingPeer.CloudRegion);
		stringBuilder.AppendFormat("HostType: {0} ", PhotonNetwork.PhotonServerSettings.HostType);
		Debug.Log(stringBuilder.ToString());
	}

	public void OnConnectedToPhoton()
	{
		Debug.Log("SupportLogger OnConnectedToPhoton().");
		LogBasics();
		if (LogTrafficStats)
		{
			PhotonNetwork.NetworkStatisticsEnabled = true;
		}
	}

	public void OnFailedToConnectToPhoton(DisconnectCause cause)
	{
		Debug.Log(string.Concat("SupportLogger OnFailedToConnectToPhoton(", cause, ")."));
		LogBasics();
	}

	public void OnJoinedLobby()
	{
		Debug.Log(string.Concat("SupportLogger OnJoinedLobby(", PhotonNetwork.lobby, ")."));
	}

	public void OnJoinedRoom()
	{
		Debug.Log(string.Concat("SupportLogger OnJoinedRoom(", PhotonNetwork.room, "). ", PhotonNetwork.lobby, " GameServer:", PhotonNetwork.ServerAddress));
	}

	public void OnCreatedRoom()
	{
		Debug.Log(string.Concat("SupportLogger OnCreatedRoom(", PhotonNetwork.room, "). ", PhotonNetwork.lobby, " GameServer:", PhotonNetwork.ServerAddress));
	}

	public void OnLeftRoom()
	{
		Debug.Log("SupportLogger OnLeftRoom().");
	}

	public void OnDisconnectedFromPhoton()
	{
		Debug.Log("SupportLogger OnDisconnectedFromPhoton().");
	}
}
