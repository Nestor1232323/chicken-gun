using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

[Serializable]
public class ServerSettings : ScriptableObject
{
	public enum HostingOption
	{
		NotSet = 0,
		PhotonCloud = 1,
		SelfHosted = 2,
		OfflineMode = 3,
		BestRegion = 4
	}

	public string AppID = string.Empty;

	public string VoiceAppID = string.Empty;

	public string ChatAppID = string.Empty;

	public HostingOption HostType;

	public CloudRegionCode PreferredRegion;

	public CloudRegionFlag EnabledRegions = (CloudRegionFlag)(-1);

	public ConnectionProtocol Protocol;

	public string ServerAddress = string.Empty;

	public int ServerPort = 5055;

	public int VoiceServerPort = 5055;

	public bool JoinLobby;

	public bool EnableLobbyStatistics;

	public PhotonLogLevel PunLogging;

	public DebugLevel NetworkLogging = DebugLevel.ERROR;

	public bool RunInBackground = true;

	public List<string> RpcList = new List<string>();

	[HideInInspector]
	public bool DisableAutoOpenWizard;

	public static CloudRegionCode BestRegionCodeInPreferences => PhotonHandler.BestRegionCodeInPreferences;

	public void UseCloudBestRegion(string cloudAppid)
	{
		HostType = HostingOption.BestRegion;
		AppID = cloudAppid;
	}

	public void UseCloud(string cloudAppid)
	{
		HostType = HostingOption.PhotonCloud;
		AppID = cloudAppid;
	}

	public void UseCloud(string cloudAppid, CloudRegionCode code)
	{
		HostType = HostingOption.PhotonCloud;
		AppID = cloudAppid;
		PreferredRegion = code;
	}

	public void UseMyServer(string serverAddress, int serverPort, string application)
	{
		HostType = HostingOption.SelfHosted;
		AppID = ((application == null) ? "master" : application);
		ServerAddress = serverAddress;
		ServerPort = serverPort;
	}

	public static bool IsAppId(string val)
	{
		try
		{
			new Guid(val);
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static void ResetBestRegionCodeInPreferences()
	{
		PhotonHandler.BestRegionCodeInPreferences = CloudRegionCode.none;
	}

	public override string ToString()
	{
		return string.Concat("ServerSettings: ", HostType, " ", ServerAddress);
	}
}
