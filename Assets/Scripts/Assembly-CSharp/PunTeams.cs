using System;
using System.Collections.Generic;
using UnityEngine;

public class PunTeams : MonoBehaviour
{
	public enum Team : byte
	{
		none = 0,
		red = 1,
		blue = 2
	}

	public static Dictionary<Team, List<PhotonPlayer>> PlayersPerTeam;

	public const string TeamPlayerProp = "team";

	public void Start()
	{
		PlayersPerTeam = new Dictionary<Team, List<PhotonPlayer>>();
		Array values = Enum.GetValues(typeof(Team));
		foreach (object item in values)
		{
			PlayersPerTeam[(Team)item] = new List<PhotonPlayer>();
		}
	}

	public void OnDisable()
	{
		PlayersPerTeam = new Dictionary<Team, List<PhotonPlayer>>();
	}

	public void OnJoinedRoom()
	{
		UpdateTeams();
	}

	public void OnLeftRoom()
	{
		Start();
	}

	public void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
	{
		UpdateTeams();
	}

	public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
	{
		UpdateTeams();
	}

	public void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		UpdateTeams();
	}

	public void UpdateTeams()
	{
		Array values = Enum.GetValues(typeof(Team));
		foreach (object item in values)
		{
			PlayersPerTeam[(Team)item].Clear();
		}
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
		{
			PhotonPlayer photonPlayer = PhotonNetwork.playerList[i];
			Team team = photonPlayer.GetTeam();
			PlayersPerTeam[team].Add(photonPlayer);
		}
	}
}
