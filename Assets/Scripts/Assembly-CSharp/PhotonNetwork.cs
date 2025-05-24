using System;
using System.Collections.Generic;
using System.Diagnostics;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PhotonNetwork
{
	public delegate void EventCallback(byte eventCode, object content, int senderId);

	public const string versionPUN = "1.92";

	internal static readonly PhotonHandler photonMono;

	internal static NetworkingPeer networkingPeer;

	public static readonly int MAX_VIEW_IDS;

	internal const string serverSettingsAssetFile = "PhotonServerSettings";

	public static ServerSettings PhotonServerSettings;

	public static bool InstantiateInRoomOnly;

	public static PhotonLogLevel logLevel;

	public static float precisionForVectorSynchronization;

	public static float precisionForQuaternionSynchronization;

	public static float precisionForFloatSynchronization;

	public static bool UseRpcMonoBehaviourCache;

	public static bool UsePrefabCache;

	public static Dictionary<string, GameObject> PrefabCache;

	public static HashSet<GameObject> SendMonoMessageTargets;

	public static Type SendMonoMessageTargetType;

	public static bool StartRpcsAsCoroutine;

	private static bool isOfflineMode;

	private static Room offlineModeRoom;

	[Obsolete("Used for compatibility with Unity networking only.")]
	public static int maxConnections;

	private static bool _mAutomaticallySyncScene;

	private static bool m_autoCleanUpPlayerObjects;

	private static int sendInterval;

	private static int sendIntervalOnSerialize;

	private static bool m_isMessageQueueRunning;

	private static Stopwatch startupStopwatch;

	public static float BackgroundTimeout;

	internal static int lastUsedViewSubId;

	internal static int lastUsedViewSubIdStatic;

	internal static List<int> manuallyAllocatedViewIds;

	public static string gameVersion { get; set; }

	public static string ServerAddress
	{
		get
		{
			if (networkingPeer != null)
			{
				return networkingPeer.ServerAddress;
			}
			return "<not connected>";
		}
	}

	public static CloudRegionCode CloudRegion
	{
		get
		{
			if (networkingPeer != null && connected && Server != ServerConnection.NameServer)
			{
				return networkingPeer.CloudRegion;
			}
			return CloudRegionCode.none;
		}
	}

	public static bool connected
	{
		get
		{
			if (offlineMode)
			{
				return true;
			}
			if (networkingPeer == null)
			{
				return false;
			}
			if (!networkingPeer.IsInitialConnect && networkingPeer.State != ClientState.PeerCreated && networkingPeer.State != ClientState.Disconnected && networkingPeer.State != ClientState.Disconnecting)
			{
				return networkingPeer.State != ClientState.ConnectingToNameServer;
			}
			return false;
		}
	}

	public static bool connecting
	{
		get
		{
			if (networkingPeer.IsInitialConnect)
			{
				return !offlineMode;
			}
			return false;
		}
	}

	public static bool connectedAndReady
	{
		get
		{
			if (!connected)
			{
				return false;
			}
			if (offlineMode)
			{
				return true;
			}
			switch (connectionStateDetailed)
			{
			case ClientState.PeerCreated:
			case ClientState.ConnectingToGameserver:
			case ClientState.Joining:
			case ClientState.ConnectingToMasterserver:
			case ClientState.Disconnecting:
			case ClientState.Disconnected:
			case ClientState.ConnectingToNameServer:
			case ClientState.Authenticating:
				return false;
			default:
				return true;
			}
		}
	}

	public static ConnectionState connectionState
	{
		get
		{
			if (offlineMode)
			{
				return ConnectionState.Connected;
			}
			if (networkingPeer == null)
			{
				return ConnectionState.Disconnected;
			}
			switch (networkingPeer.PeerState)
			{
			case PeerStateValue.Disconnected:
				return ConnectionState.Disconnected;
			case PeerStateValue.Connecting:
				return ConnectionState.Connecting;
			case PeerStateValue.Connected:
				return ConnectionState.Connected;
			case PeerStateValue.Disconnecting:
				return ConnectionState.Disconnecting;
			case PeerStateValue.InitializingApplication:
				return ConnectionState.InitializingApplication;
			default:
				return ConnectionState.Disconnected;
			}
		}
	}

	public static ClientState connectionStateDetailed
	{
		get
		{
			if (offlineMode)
			{
				if (offlineModeRoom != null)
				{
					return ClientState.Joined;
				}
				return ClientState.ConnectedToMaster;
			}
			if (networkingPeer == null)
			{
				return ClientState.Disconnected;
			}
			return networkingPeer.State;
		}
	}

	public static ServerConnection Server
	{
		get
		{
			if (networkingPeer != null)
			{
				return networkingPeer.Server;
			}
			return ServerConnection.NameServer;
		}
	}

	public static AuthenticationValues AuthValues
	{
		get
		{
			if (networkingPeer != null)
			{
				return networkingPeer.AuthValues;
			}
			return null;
		}
		set
		{
			if (networkingPeer != null)
			{
				networkingPeer.AuthValues = value;
			}
		}
	}

	public static Room room
	{
		get
		{
			if (isOfflineMode)
			{
				return offlineModeRoom;
			}
			return networkingPeer.CurrentRoom;
		}
	}

	public static PhotonPlayer player
	{
		get
		{
			if (networkingPeer == null)
			{
				return null;
			}
			return networkingPeer.LocalPlayer;
		}
	}

	public static PhotonPlayer masterClient
	{
		get
		{
			if (offlineMode)
			{
				return player;
			}
			if (networkingPeer == null)
			{
				return null;
			}
			return networkingPeer.GetPlayerWithId(networkingPeer.mMasterClientId);
		}
	}

	public static string playerName
	{
		get
		{
			return networkingPeer.PlayerName;
		}
		set
		{
			networkingPeer.PlayerName = value;
		}
	}

	public static PhotonPlayer[] playerList
	{
		get
		{
			if (networkingPeer == null)
			{
				return new PhotonPlayer[0];
			}
			return networkingPeer.mPlayerListCopy;
		}
	}

	public static PhotonPlayer[] otherPlayers
	{
		get
		{
			if (networkingPeer == null)
			{
				return new PhotonPlayer[0];
			}
			return networkingPeer.mOtherPlayerListCopy;
		}
	}

	public static List<FriendInfo> Friends { get; internal set; }

	public static int FriendsListAge
	{
		get
		{
			if (networkingPeer == null)
			{
				return 0;
			}
			return networkingPeer.FriendListAge;
		}
	}

	public static IPunPrefabPool PrefabPool
	{
		get
		{
			return networkingPeer.ObjectPool;
		}
		set
		{
			networkingPeer.ObjectPool = value;
		}
	}

	public static bool offlineMode
	{
		get
		{
			return isOfflineMode;
		}
		set
		{
			if (value == isOfflineMode)
			{
				return;
			}
			if (value && connected)
			{
				UnityEngine.Debug.LogError("Can't start OFFLINE mode while connected!");
				return;
			}
			if (networkingPeer.PeerState != PeerStateValue.Disconnected)
			{
				networkingPeer.Disconnect();
			}
			isOfflineMode = value;
			if (isOfflineMode)
			{
				networkingPeer.ChangeLocalID(-1);
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnConnectedToMaster);
			}
			else
			{
				offlineModeRoom = null;
				networkingPeer.ChangeLocalID(-1);
			}
		}
	}

	public static bool automaticallySyncScene
	{
		get
		{
			return _mAutomaticallySyncScene;
		}
		set
		{
			_mAutomaticallySyncScene = value;
			if (_mAutomaticallySyncScene && room != null)
			{
				networkingPeer.LoadLevelIfSynced();
			}
		}
	}

	public static bool autoCleanUpPlayerObjects
	{
		get
		{
			return m_autoCleanUpPlayerObjects;
		}
		set
		{
			if (room != null)
			{
				UnityEngine.Debug.LogError("Setting autoCleanUpPlayerObjects while in a room is not supported.");
			}
			else
			{
				m_autoCleanUpPlayerObjects = value;
			}
		}
	}

	public static bool autoJoinLobby
	{
		get
		{
			return PhotonServerSettings.JoinLobby;
		}
		set
		{
			PhotonServerSettings.JoinLobby = value;
		}
	}

	public static bool EnableLobbyStatistics
	{
		get
		{
			return PhotonServerSettings.EnableLobbyStatistics;
		}
		set
		{
			PhotonServerSettings.EnableLobbyStatistics = value;
		}
	}

	public static List<TypedLobbyInfo> LobbyStatistics
	{
		get
		{
			return networkingPeer.LobbyStatistics;
		}
		private set
		{
			networkingPeer.LobbyStatistics = value;
		}
	}

	public static bool insideLobby => networkingPeer.insideLobby;

	public static TypedLobby lobby
	{
		get
		{
			return networkingPeer.lobby;
		}
		set
		{
			networkingPeer.lobby = value;
		}
	}

	public static int sendRate
	{
		get
		{
			return 1000 / sendInterval;
		}
		set
		{
			sendInterval = 1000 / value;
			if (photonMono != null)
			{
				photonMono.updateInterval = sendInterval;
			}
			if (value < sendRateOnSerialize)
			{
				sendRateOnSerialize = value;
			}
		}
	}

	public static int sendRateOnSerialize
	{
		get
		{
			return 1000 / sendIntervalOnSerialize;
		}
		set
		{
			if (value > sendRate)
			{
				UnityEngine.Debug.LogError("Error: Can not set the OnSerialize rate higher than the overall SendRate.");
				value = sendRate;
			}
			sendIntervalOnSerialize = 1000 / value;
			if (photonMono != null)
			{
				photonMono.updateIntervalOnSerialize = sendIntervalOnSerialize;
			}
		}
	}

	public static bool isMessageQueueRunning
	{
		get
		{
			return m_isMessageQueueRunning;
		}
		set
		{
			if (value)
			{
				PhotonHandler.StartFallbackSendAckThread();
			}
			networkingPeer.IsSendingOnlyAcks = !value;
			m_isMessageQueueRunning = value;
		}
	}

	public static int unreliableCommandsLimit
	{
		get
		{
			return networkingPeer.LimitOfUnreliableCommands;
		}
		set
		{
			networkingPeer.LimitOfUnreliableCommands = value;
		}
	}

	public static double time => (double)(uint)ServerTimestamp / 1000.0;

	public static int ServerTimestamp
	{
		get
		{
			if (offlineMode)
			{
				return (int)startupStopwatch.ElapsedMilliseconds;
			}
			return networkingPeer.ServerTimeInMilliSeconds;
		}
	}

	public static bool isMasterClient
	{
		get
		{
			if (offlineMode)
			{
				return true;
			}
			return networkingPeer.mMasterClientId == player.ID;
		}
	}

	public static bool inRoom => connectionStateDetailed == ClientState.Joined;

	public static bool isNonMasterClientInRoom
	{
		get
		{
			if (!isMasterClient)
			{
				return room != null;
			}
			return false;
		}
	}

	public static int countOfPlayersOnMaster => networkingPeer.PlayersOnMasterCount;

	public static int countOfPlayersInRooms => networkingPeer.PlayersInRoomsCount;

	public static int countOfPlayers => networkingPeer.PlayersInRoomsCount + networkingPeer.PlayersOnMasterCount;

	public static int countOfRooms => networkingPeer.RoomsCount;

	public static bool NetworkStatisticsEnabled
	{
		get
		{
			return networkingPeer.TrafficStatsEnabled;
		}
		set
		{
			networkingPeer.TrafficStatsEnabled = value;
		}
	}

	public static int ResentReliableCommands => networkingPeer.ResentReliableCommands;

	public static bool CrcCheckEnabled
	{
		get
		{
			return networkingPeer.CrcEnabled;
		}
		set
		{
			if (!connected && !connecting)
			{
				networkingPeer.CrcEnabled = value;
			}
			else
			{
				UnityEngine.Debug.Log("Can't change CrcCheckEnabled while being connected. CrcCheckEnabled stays " + networkingPeer.CrcEnabled);
			}
		}
	}

	public static int PacketLossByCrcCheck => networkingPeer.PacketLossByCrc;

	public static int MaxResendsBeforeDisconnect
	{
		get
		{
			return networkingPeer.SentCountAllowance;
		}
		set
		{
			if (value < 3)
			{
				value = 3;
			}
			if (value > 10)
			{
				value = 10;
			}
			networkingPeer.SentCountAllowance = value;
		}
	}

	public static int QuickResends
	{
		get
		{
			return networkingPeer.QuickResendAttempts;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			if (value > 3)
			{
				value = 3;
			}
			networkingPeer.QuickResendAttempts = (byte)value;
		}
	}

	public static bool UseAlternativeUdpPorts { get; set; }

	public static event EventCallback OnEventCall;

	static PhotonNetwork()
	{
		MAX_VIEW_IDS = 1000;
		PhotonServerSettings = (ServerSettings)Resources.Load("PhotonServerSettings", typeof(ServerSettings));
		InstantiateInRoomOnly = true;
		logLevel = PhotonLogLevel.ErrorsOnly;
		precisionForVectorSynchronization = 9.9E-05f;
		precisionForQuaternionSynchronization = 1f;
		precisionForFloatSynchronization = 0.01f;
		UsePrefabCache = true;
		PrefabCache = new Dictionary<string, GameObject>();
		SendMonoMessageTargetType = typeof(MonoBehaviour);
		StartRpcsAsCoroutine = true;
		isOfflineMode = false;
		offlineModeRoom = null;
		_mAutomaticallySyncScene = false;
		m_autoCleanUpPlayerObjects = true;
		sendInterval = 50;
		sendIntervalOnSerialize = 100;
		m_isMessageQueueRunning = true;
		BackgroundTimeout = 60f;
		lastUsedViewSubId = 0;
		lastUsedViewSubIdStatic = 0;
		manuallyAllocatedViewIds = new List<int>();
		if (PhotonServerSettings != null)
		{
			Application.runInBackground = PhotonServerSettings.RunInBackground;
		}
		GameObject gameObject = new GameObject();
		photonMono = gameObject.AddComponent<PhotonHandler>();
		gameObject.name = "PhotonMono";
		gameObject.hideFlags = HideFlags.HideInHierarchy;
		ConnectionProtocol protocol = PhotonServerSettings.Protocol;
		networkingPeer = new NetworkingPeer(string.Empty, protocol);
		networkingPeer.QuickResendAttempts = 2;
		networkingPeer.SentCountAllowance = 7;
		startupStopwatch = new Stopwatch();
		startupStopwatch.Start();
		networkingPeer.LocalMsTimestampDelegate = () => (int)startupStopwatch.ElapsedMilliseconds;
		CustomTypes.Register();
	}

	public static void SwitchToProtocol(ConnectionProtocol cp)
	{
		networkingPeer.TransportProtocol = cp;
	}

	public static bool ConnectUsingSettings(string gameVersion)
	{
		if (networkingPeer.PeerState != PeerStateValue.Disconnected)
		{
			UnityEngine.Debug.LogWarning("ConnectUsingSettings() failed. Can only connect while in state 'Disconnected'. Current state: " + networkingPeer.PeerState);
			return false;
		}
		if (PhotonServerSettings == null)
		{
			UnityEngine.Debug.LogError("Can't connect: Loading settings failed. ServerSettings asset must be in any 'Resources' folder as: PhotonServerSettings");
			return false;
		}
		if (PhotonServerSettings.HostType == ServerSettings.HostingOption.NotSet)
		{
			UnityEngine.Debug.LogError("You did not select a Hosting Type in your PhotonServerSettings. Please set it up or don't use ConnectUsingSettings().");
			return false;
		}
		if (logLevel == PhotonLogLevel.ErrorsOnly)
		{
			logLevel = PhotonServerSettings.PunLogging;
		}
		if (networkingPeer.DebugOut == DebugLevel.ERROR)
		{
			networkingPeer.DebugOut = PhotonServerSettings.NetworkLogging;
		}
		SwitchToProtocol(PhotonServerSettings.Protocol);
		networkingPeer.SetApp(PhotonServerSettings.AppID, gameVersion);
		if (PhotonServerSettings.HostType == ServerSettings.HostingOption.OfflineMode)
		{
			offlineMode = true;
			return true;
		}
		if (offlineMode)
		{
			UnityEngine.Debug.LogWarning("ConnectUsingSettings() disabled the offline mode. No longer offline.");
		}
		offlineMode = false;
		isMessageQueueRunning = true;
		networkingPeer.IsInitialConnect = true;
		if (PhotonServerSettings.HostType == ServerSettings.HostingOption.SelfHosted)
		{
			networkingPeer.IsUsingNameServer = false;
			networkingPeer.MasterServerAddress = ((PhotonServerSettings.ServerPort != 0) ? (PhotonServerSettings.ServerAddress + ":" + PhotonServerSettings.ServerPort) : PhotonServerSettings.ServerAddress);
			return networkingPeer.Connect(networkingPeer.MasterServerAddress, ServerConnection.MasterServer);
		}
		if (PhotonServerSettings.HostType == ServerSettings.HostingOption.BestRegion)
		{
			return ConnectToBestCloudServer(gameVersion);
		}
		return networkingPeer.ConnectToRegionMaster(PhotonServerSettings.PreferredRegion);
	}

	public static bool ConnectToMaster(string masterServerAddress, int port, string appID, string gameVersion)
	{
		if (networkingPeer.PeerState != PeerStateValue.Disconnected)
		{
			UnityEngine.Debug.LogWarning("ConnectToMaster() failed. Can only connect while in state 'Disconnected'. Current state: " + networkingPeer.PeerState);
			return false;
		}
		if (offlineMode)
		{
			offlineMode = false;
			UnityEngine.Debug.LogWarning("ConnectToMaster() disabled the offline mode. No longer offline.");
		}
		if (!isMessageQueueRunning)
		{
			isMessageQueueRunning = true;
			UnityEngine.Debug.LogWarning("ConnectToMaster() enabled isMessageQueueRunning. Needs to be able to dispatch incoming messages.");
		}
		networkingPeer.SetApp(appID, gameVersion);
		networkingPeer.IsUsingNameServer = false;
		networkingPeer.IsInitialConnect = true;
		networkingPeer.MasterServerAddress = ((port != 0) ? (masterServerAddress + ":" + port) : masterServerAddress);
		return networkingPeer.Connect(networkingPeer.MasterServerAddress, ServerConnection.MasterServer);
	}

	public static bool Reconnect()
	{
		if (string.IsNullOrEmpty(networkingPeer.MasterServerAddress))
		{
			UnityEngine.Debug.LogWarning("Reconnect() failed. It seems the client wasn't connected before?! Current state: " + networkingPeer.PeerState);
			return false;
		}
		if (networkingPeer.PeerState != PeerStateValue.Disconnected)
		{
			UnityEngine.Debug.LogWarning("Reconnect() failed. Can only connect while in state 'Disconnected'. Current state: " + networkingPeer.PeerState);
			return false;
		}
		if (offlineMode)
		{
			offlineMode = false;
			UnityEngine.Debug.LogWarning("Reconnect() disabled the offline mode. No longer offline.");
		}
		if (!isMessageQueueRunning)
		{
			isMessageQueueRunning = true;
			UnityEngine.Debug.LogWarning("Reconnect() enabled isMessageQueueRunning. Needs to be able to dispatch incoming messages.");
		}
		networkingPeer.IsUsingNameServer = false;
		networkingPeer.IsInitialConnect = false;
		return networkingPeer.ReconnectToMaster();
	}

	public static bool ReconnectAndRejoin()
	{
		if (networkingPeer.PeerState != PeerStateValue.Disconnected)
		{
			UnityEngine.Debug.LogWarning("ReconnectAndRejoin() failed. Can only connect while in state 'Disconnected'. Current state: " + networkingPeer.PeerState);
			return false;
		}
		if (offlineMode)
		{
			offlineMode = false;
			UnityEngine.Debug.LogWarning("ReconnectAndRejoin() disabled the offline mode. No longer offline.");
		}
		if (string.IsNullOrEmpty(networkingPeer.GameServerAddress))
		{
			UnityEngine.Debug.LogWarning("ReconnectAndRejoin() failed. It seems the client wasn't connected to a game server before (no address).");
			return false;
		}
		if (networkingPeer.enterRoomParamsCache == null)
		{
			UnityEngine.Debug.LogWarning("ReconnectAndRejoin() failed. It seems the client doesn't have any previous room to re-join.");
			return false;
		}
		if (!isMessageQueueRunning)
		{
			isMessageQueueRunning = true;
			UnityEngine.Debug.LogWarning("ReconnectAndRejoin() enabled isMessageQueueRunning. Needs to be able to dispatch incoming messages.");
		}
		networkingPeer.IsUsingNameServer = false;
		networkingPeer.IsInitialConnect = false;
		return networkingPeer.ReconnectAndRejoin();
	}

	public static bool ConnectToBestCloudServer(string gameVersion)
	{
		if (networkingPeer.PeerState != PeerStateValue.Disconnected)
		{
			UnityEngine.Debug.LogWarning("ConnectToBestCloudServer() failed. Can only connect while in state 'Disconnected'. Current state: " + networkingPeer.PeerState);
			return false;
		}
		if (PhotonServerSettings == null)
		{
			UnityEngine.Debug.LogError("Can't connect: Loading settings failed. ServerSettings asset must be in any 'Resources' folder as: PhotonServerSettings");
			return false;
		}
		if (PhotonServerSettings.HostType == ServerSettings.HostingOption.OfflineMode)
		{
			return ConnectUsingSettings(gameVersion);
		}
		networkingPeer.IsInitialConnect = true;
		networkingPeer.SetApp(PhotonServerSettings.AppID, gameVersion);
		return networkingPeer.ConnectToNameServer();
	}

	public static bool ConnectToRegion(CloudRegionCode region, string gameVersion)
	{
		if (networkingPeer.PeerState != PeerStateValue.Disconnected)
		{
			UnityEngine.Debug.LogWarning("ConnectToRegion() failed. Can only connect while in state 'Disconnected'. Current state: " + networkingPeer.PeerState);
			return false;
		}
		if (PhotonServerSettings == null)
		{
			UnityEngine.Debug.LogError("Can't connect: ServerSettings asset must be in any 'Resources' folder as: PhotonServerSettings");
			return false;
		}
		if (PhotonServerSettings.HostType == ServerSettings.HostingOption.OfflineMode)
		{
			return ConnectUsingSettings(gameVersion);
		}
		networkingPeer.IsInitialConnect = true;
		networkingPeer.SetApp(PhotonServerSettings.AppID, gameVersion);
		if (region != CloudRegionCode.none)
		{
			UnityEngine.Debug.Log("ConnectToRegion: " + region);
			return networkingPeer.ConnectToRegionMaster(region);
		}
		return false;
	}

	public static void OverrideBestCloudServer(CloudRegionCode region)
	{
		PhotonHandler.BestRegionCodeInPreferences = region;
	}

	public static void RefreshCloudServerRating()
	{
		throw new NotImplementedException("not available at the moment");
	}

	public static void NetworkStatisticsReset()
	{
		networkingPeer.TrafficStatsReset();
	}

	public static string NetworkStatisticsToString()
	{
		if (networkingPeer == null || offlineMode)
		{
			return "Offline or in OfflineMode. No VitalStats available.";
		}
		return networkingPeer.VitalStatsToString(all: false);
	}

	[Obsolete("Used for compatibility with Unity networking only. Encryption is automatically initialized while connecting.")]
	public static void InitializeSecurity()
	{
	}

	private static bool VerifyCanUseNetwork()
	{
		if (connected)
		{
			return true;
		}
		UnityEngine.Debug.LogError("Cannot send messages when not connected. Either connect to Photon OR use offline mode!");
		return false;
	}

	public static void Disconnect()
	{
		if (offlineMode)
		{
			offlineMode = false;
			offlineModeRoom = null;
			networkingPeer.State = ClientState.Disconnecting;
			networkingPeer.OnStatusChanged(StatusCode.Disconnect);
		}
		else if (networkingPeer != null)
		{
			networkingPeer.Disconnect();
		}
	}

	public static bool FindFriends(string[] friendsToFind)
	{
		if (networkingPeer == null || isOfflineMode)
		{
			return false;
		}
		return networkingPeer.OpFindFriends(friendsToFind);
	}

	public static bool CreateRoom(string roomName)
	{
		return CreateRoom(roomName, null, null, null);
	}

	public static bool CreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby)
	{
		return CreateRoom(roomName, roomOptions, typedLobby, null);
	}

	public static bool CreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby, string[] expectedUsers)
	{
		if (offlineMode)
		{
			if (offlineModeRoom != null)
			{
				UnityEngine.Debug.LogError("CreateRoom failed. In offline mode you still have to leave a room to enter another.");
				return false;
			}
			EnterOfflineRoom(roomName, roomOptions, createdRoom: true);
			return true;
		}
		if (networkingPeer.Server != ServerConnection.MasterServer || !connectedAndReady)
		{
			UnityEngine.Debug.LogError("CreateRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
			return false;
		}
		typedLobby = typedLobby ?? ((!networkingPeer.insideLobby) ? null : networkingPeer.lobby);
		EnterRoomParams enterRoomParams = new EnterRoomParams();
		enterRoomParams.RoomName = roomName;
		enterRoomParams.RoomOptions = roomOptions;
		enterRoomParams.Lobby = typedLobby;
		enterRoomParams.ExpectedUsers = expectedUsers;
		return networkingPeer.OpCreateGame(enterRoomParams);
	}

	public static bool JoinRoom(string roomName)
	{
		return JoinRoom(roomName, null);
	}

	public static bool JoinRoom(string roomName, string[] expectedUsers)
	{
		if (offlineMode)
		{
			if (offlineModeRoom != null)
			{
				UnityEngine.Debug.LogError("JoinRoom failed. In offline mode you still have to leave a room to enter another.");
				return false;
			}
			EnterOfflineRoom(roomName, null, createdRoom: true);
			return true;
		}
		if (networkingPeer.Server != ServerConnection.MasterServer || !connectedAndReady)
		{
			UnityEngine.Debug.LogError("JoinRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
			return false;
		}
		if (string.IsNullOrEmpty(roomName))
		{
			UnityEngine.Debug.LogError("JoinRoom failed. A roomname is required. If you don't know one, how will you join?");
			return false;
		}
		EnterRoomParams enterRoomParams = new EnterRoomParams();
		enterRoomParams.RoomName = roomName;
		enterRoomParams.ExpectedUsers = expectedUsers;
		return networkingPeer.OpJoinRoom(enterRoomParams);
	}

	public static bool JoinOrCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby)
	{
		return JoinOrCreateRoom(roomName, roomOptions, typedLobby, null);
	}

	public static bool JoinOrCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby, string[] expectedUsers)
	{
		if (offlineMode)
		{
			if (offlineModeRoom != null)
			{
				UnityEngine.Debug.LogError("JoinOrCreateRoom failed. In offline mode you still have to leave a room to enter another.");
				return false;
			}
			EnterOfflineRoom(roomName, roomOptions, createdRoom: true);
			return true;
		}
		if (networkingPeer.Server != ServerConnection.MasterServer || !connectedAndReady)
		{
			UnityEngine.Debug.LogError("JoinOrCreateRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
			return false;
		}
		if (string.IsNullOrEmpty(roomName))
		{
			UnityEngine.Debug.LogError("JoinOrCreateRoom failed. A roomname is required. If you don't know one, how will you join?");
			return false;
		}
		typedLobby = typedLobby ?? ((!networkingPeer.insideLobby) ? null : networkingPeer.lobby);
		EnterRoomParams enterRoomParams = new EnterRoomParams();
		enterRoomParams.RoomName = roomName;
		enterRoomParams.RoomOptions = roomOptions;
		enterRoomParams.Lobby = typedLobby;
		enterRoomParams.CreateIfNotExists = true;
		enterRoomParams.PlayerProperties = player.CustomProperties;
		enterRoomParams.ExpectedUsers = expectedUsers;
		return networkingPeer.OpJoinRoom(enterRoomParams);
	}

	public static bool JoinRandomRoom()
	{
		return JoinRandomRoom(null, 0, MatchmakingMode.FillRoom, null, null);
	}

	public static bool JoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers)
	{
		return JoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, MatchmakingMode.FillRoom, null, null);
	}

	public static bool JoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, MatchmakingMode matchingType, TypedLobby typedLobby, string sqlLobbyFilter, string[] expectedUsers = null)
	{
		if (offlineMode)
		{
			if (offlineModeRoom != null)
			{
				UnityEngine.Debug.LogError("JoinRandomRoom failed. In offline mode you still have to leave a room to enter another.");
				return false;
			}
			EnterOfflineRoom("offline room", null, createdRoom: true);
			return true;
		}
		if (networkingPeer.Server != ServerConnection.MasterServer || !connectedAndReady)
		{
			UnityEngine.Debug.LogError("JoinRandomRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
			return false;
		}
		typedLobby = typedLobby ?? ((!networkingPeer.insideLobby) ? null : networkingPeer.lobby);
		OpJoinRandomRoomParams opJoinRandomRoomParams = new OpJoinRandomRoomParams();
		opJoinRandomRoomParams.ExpectedCustomRoomProperties = expectedCustomRoomProperties;
		opJoinRandomRoomParams.ExpectedMaxPlayers = expectedMaxPlayers;
		opJoinRandomRoomParams.MatchingType = matchingType;
		opJoinRandomRoomParams.TypedLobby = typedLobby;
		opJoinRandomRoomParams.SqlLobbyFilter = sqlLobbyFilter;
		opJoinRandomRoomParams.ExpectedUsers = expectedUsers;
		return networkingPeer.OpJoinRandomRoom(opJoinRandomRoomParams);
	}

	public static bool ReJoinRoom(string roomName)
	{
		if (offlineMode)
		{
			UnityEngine.Debug.LogError("ReJoinRoom failed due to offline mode.");
			return false;
		}
		if (networkingPeer.Server != ServerConnection.MasterServer || !connectedAndReady)
		{
			UnityEngine.Debug.LogError("ReJoinRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
			return false;
		}
		if (string.IsNullOrEmpty(roomName))
		{
			UnityEngine.Debug.LogError("ReJoinRoom failed. A roomname is required. If you don't know one, how will you join?");
			return false;
		}
		EnterRoomParams enterRoomParams = new EnterRoomParams();
		enterRoomParams.RoomName = roomName;
		enterRoomParams.RejoinOnly = true;
		enterRoomParams.PlayerProperties = player.CustomProperties;
		return networkingPeer.OpJoinRoom(enterRoomParams);
	}

	private static void EnterOfflineRoom(string roomName, RoomOptions roomOptions, bool createdRoom)
	{
		offlineModeRoom = new Room(roomName, roomOptions);
		networkingPeer.ChangeLocalID(1);
		networkingPeer.State = ClientState.ConnectingToGameserver;
		networkingPeer.OnStatusChanged(StatusCode.Connect);
		offlineModeRoom.MasterClientId = 1;
		if (createdRoom)
		{
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom);
		}
		NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom);
	}

	public static bool JoinLobby()
	{
		return JoinLobby(null);
	}

	public static bool JoinLobby(TypedLobby typedLobby)
	{
		if (connected && Server == ServerConnection.MasterServer)
		{
			if (typedLobby == null)
			{
				typedLobby = TypedLobby.Default;
			}
			bool num = networkingPeer.OpJoinLobby(typedLobby);
			if (num)
			{
				networkingPeer.lobby = typedLobby;
			}
			return num;
		}
		return false;
	}

	public static bool LeaveLobby()
	{
		if (connected && Server == ServerConnection.MasterServer)
		{
			return networkingPeer.OpLeaveLobby();
		}
		return false;
	}

	public static bool LeaveRoom(bool becomeInactive = true)
	{
		if (offlineMode)
		{
			offlineModeRoom = null;
			networkingPeer.State = ClientState.PeerCreated;
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnLeftRoom);
			return true;
		}
		if (room == null)
		{
			UnityEngine.Debug.LogWarning("PhotonNetwork.room is null. You don't have to call LeaveRoom() when you're not in one. State: " + connectionStateDetailed);
		}
		else
		{
			becomeInactive = becomeInactive && room.PlayerTtl != 0;
		}
		return networkingPeer.OpLeaveRoom(becomeInactive);
	}

	public static bool GetCustomRoomList(TypedLobby typedLobby, string sqlLobbyFilter)
	{
		return networkingPeer.OpGetGameList(typedLobby, sqlLobbyFilter);
	}

	public static RoomInfo[] GetRoomList()
	{
		if (offlineMode || networkingPeer == null)
		{
			return new RoomInfo[0];
		}
		return networkingPeer.mGameListCopy;
	}

	public static void SetPlayerCustomProperties(Hashtable customProperties)
	{
		if (customProperties == null)
		{
			customProperties = new Hashtable();
			foreach (object key in player.CustomProperties.Keys)
			{
				customProperties[(string)key] = null;
			}
		}
		if (room != null && room.IsLocalClientInside)
		{
			player.SetCustomProperties(customProperties);
		}
		else
		{
			player.InternalCacheProperties(customProperties);
		}
	}

	public static void RemovePlayerCustomProperties(string[] customPropertiesToDelete)
	{
		if (customPropertiesToDelete == null || customPropertiesToDelete.Length == 0 || player.CustomProperties == null)
		{
			player.CustomProperties = new Hashtable();
			return;
		}
		foreach (string key in customPropertiesToDelete)
		{
			if (player.CustomProperties.ContainsKey(key))
			{
				player.CustomProperties.Remove(key);
			}
		}
	}

	public static bool RaiseEvent(byte eventCode, object eventContent, bool sendReliable, RaiseEventOptions options)
	{
		if (!inRoom || eventCode >= 200)
		{
			UnityEngine.Debug.LogWarning("RaiseEvent() failed. Your event is not being sent! Check if your are in a Room and the eventCode must be less than 200 (0..199).");
			return false;
		}
		return networkingPeer.OpRaiseEvent(eventCode, eventContent, sendReliable, options);
	}

	public static int AllocateViewID()
	{
		int num = AllocateViewID(player.ID);
		manuallyAllocatedViewIds.Add(num);
		return num;
	}

	public static int AllocateSceneViewID()
	{
		if (!isMasterClient)
		{
			UnityEngine.Debug.LogError("Only the Master Client can AllocateSceneViewID(). Check PhotonNetwork.isMasterClient!");
			return -1;
		}
		int num = AllocateViewID(0);
		manuallyAllocatedViewIds.Add(num);
		return num;
	}

	private static int AllocateViewID(int ownerId)
	{
		if (ownerId == 0)
		{
			int num = lastUsedViewSubIdStatic;
			int num2 = ownerId * MAX_VIEW_IDS;
			for (int i = 1; i < MAX_VIEW_IDS; i++)
			{
				num = (num + 1) % MAX_VIEW_IDS;
				if (num != 0)
				{
					int num3 = num + num2;
					if (!networkingPeer.photonViewList.ContainsKey(num3))
					{
						lastUsedViewSubIdStatic = num;
						return num3;
					}
				}
			}
			throw new Exception($"AllocateViewID() failed. Room (user {ownerId}) is out of 'scene' viewIDs. It seems all available are in use.");
		}
		int num4 = lastUsedViewSubId;
		int num5 = ownerId * MAX_VIEW_IDS;
		for (int j = 1; j < MAX_VIEW_IDS; j++)
		{
			num4 = (num4 + 1) % MAX_VIEW_IDS;
			if (num4 != 0)
			{
				int num6 = num4 + num5;
				if (!networkingPeer.photonViewList.ContainsKey(num6) && !manuallyAllocatedViewIds.Contains(num6))
				{
					lastUsedViewSubId = num4;
					return num6;
				}
			}
		}
		throw new Exception($"AllocateViewID() failed. User {ownerId} is out of subIds, as all viewIDs are used.");
	}

	private static int[] AllocateSceneViewIDs(int countOfNewViews)
	{
		int[] array = new int[countOfNewViews];
		for (int i = 0; i < countOfNewViews; i++)
		{
			array[i] = AllocateViewID(0);
		}
		return array;
	}

	public static void UnAllocateViewID(int viewID)
	{
		manuallyAllocatedViewIds.Remove(viewID);
		if (networkingPeer.photonViewList.ContainsKey(viewID))
		{
			UnityEngine.Debug.LogWarning($"UnAllocateViewID() should be called after the PhotonView was destroyed (GameObject.Destroy()). ViewID: {viewID} still found in: {networkingPeer.photonViewList[viewID]}");
		}
	}

	public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, byte group)
	{
		return Instantiate(prefabName, position, rotation, group, null);
	}

	public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, byte group, object[] data)
	{
		if (!connected || (InstantiateInRoomOnly && !inRoom))
		{
			UnityEngine.Debug.LogError("Failed to Instantiate prefab: " + prefabName + ". Client should be in a room. Current connectionStateDetailed: " + connectionStateDetailed);
			return null;
		}
		if (!UsePrefabCache || !PrefabCache.TryGetValue(prefabName, out var value))
		{
			value = (GameObject)Resources.Load(prefabName, typeof(GameObject));
			if (UsePrefabCache)
			{
				PrefabCache.Add(prefabName, value);
			}
		}
		if (value == null)
		{
			UnityEngine.Debug.LogError("Failed to Instantiate prefab: " + prefabName + ". Verify the Prefab is in a Resources folder (and not in a subfolder)");
			return null;
		}
		if (value.GetComponent<PhotonView>() == null)
		{
			UnityEngine.Debug.LogError("Failed to Instantiate prefab:" + prefabName + ". Prefab must have a PhotonView component.");
			return null;
		}
		Component[] photonViewsInChildren = value.GetPhotonViewsInChildren();
		int[] array = new int[photonViewsInChildren.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = AllocateViewID(player.ID);
		}
		Hashtable evData = networkingPeer.SendInstantiate(prefabName, position, rotation, group, array, data, isGlobalObject: false);
		return networkingPeer.DoInstantiate(evData, networkingPeer.LocalPlayer, value);
	}

	public static GameObject InstantiateSceneObject(string prefabName, Vector3 position, Quaternion rotation, byte group, object[] data)
	{
		if (!connected || (InstantiateInRoomOnly && !inRoom))
		{
			UnityEngine.Debug.LogError("Failed to InstantiateSceneObject prefab: " + prefabName + ". Client should be in a room. Current connectionStateDetailed: " + connectionStateDetailed);
			return null;
		}
		if (!isMasterClient)
		{
			UnityEngine.Debug.LogError("Failed to InstantiateSceneObject prefab: " + prefabName + ". Client is not the MasterClient in this room.");
			return null;
		}
		if (!UsePrefabCache || !PrefabCache.TryGetValue(prefabName, out var value))
		{
			value = (GameObject)Resources.Load(prefabName, typeof(GameObject));
			if (UsePrefabCache)
			{
				PrefabCache.Add(prefabName, value);
			}
		}
		if (value == null)
		{
			UnityEngine.Debug.LogError("Failed to InstantiateSceneObject prefab: " + prefabName + ". Verify the Prefab is in a Resources folder (and not in a subfolder)");
			return null;
		}
		if (value.GetComponent<PhotonView>() == null)
		{
			UnityEngine.Debug.LogError("Failed to InstantiateSceneObject prefab:" + prefabName + ". Prefab must have a PhotonView component.");
			return null;
		}
		Component[] photonViewsInChildren = value.GetPhotonViewsInChildren();
		int[] array = AllocateSceneViewIDs(photonViewsInChildren.Length);
		if (array == null)
		{
			UnityEngine.Debug.LogError("Failed to InstantiateSceneObject prefab: " + prefabName + ". No ViewIDs are free to use. Max is: " + MAX_VIEW_IDS);
			return null;
		}
		Hashtable evData = networkingPeer.SendInstantiate(prefabName, position, rotation, group, array, data, isGlobalObject: true);
		return networkingPeer.DoInstantiate(evData, networkingPeer.LocalPlayer, value);
	}

	public static int GetPing()
	{
		return networkingPeer.RoundTripTime;
	}

	public static void FetchServerTimestamp()
	{
		if (networkingPeer != null)
		{
			networkingPeer.FetchServerTimestamp();
		}
	}

	public static void SendOutgoingCommands()
	{
		if (VerifyCanUseNetwork())
		{
			while (networkingPeer.SendOutgoingCommands())
			{
			}
		}
	}

	public static bool CloseConnection(PhotonPlayer kickPlayer)
	{
		if (!VerifyCanUseNetwork())
		{
			return false;
		}
		if (!player.IsMasterClient)
		{
			UnityEngine.Debug.LogError("CloseConnection: Only the masterclient can kick another player.");
			return false;
		}
		if (kickPlayer == null)
		{
			UnityEngine.Debug.LogError("CloseConnection: No such player connected!");
			return false;
		}
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		raiseEventOptions.TargetActors = new int[1] { kickPlayer.ID };
		RaiseEventOptions raiseEventOptions2 = raiseEventOptions;
		return networkingPeer.OpRaiseEvent(203, null, sendReliable: true, raiseEventOptions2);
	}

	public static bool SetMasterClient(PhotonPlayer masterClientPlayer)
	{
		if (!inRoom || !VerifyCanUseNetwork() || offlineMode)
		{
			if (logLevel == PhotonLogLevel.Informational)
			{
				UnityEngine.Debug.Log("Can not SetMasterClient(). Not in room or in offlineMode.");
			}
			return false;
		}
		if (room.serverSideMasterClient)
		{
			Hashtable gameProperties = new Hashtable { 
			{
				(byte)248,
				masterClientPlayer.ID
			} };
			Hashtable expectedProperties = new Hashtable { 
			{
				(byte)248,
				networkingPeer.mMasterClientId
			} };
			return networkingPeer.OpSetPropertiesOfRoom(gameProperties, expectedProperties);
		}
		if (!isMasterClient)
		{
			return false;
		}
		return networkingPeer.SetMasterClient(masterClientPlayer.ID, sync: true);
	}

	public static void Destroy(PhotonView targetView)
	{
		if (targetView != null)
		{
			networkingPeer.RemoveInstantiatedGO(targetView.gameObject, !inRoom);
		}
		else
		{
			UnityEngine.Debug.LogError("Destroy(targetPhotonView) failed, cause targetPhotonView is null.");
		}
	}

	public static void Destroy(GameObject targetGo)
	{
		networkingPeer.RemoveInstantiatedGO(targetGo, !inRoom);
	}

	public static void DestroyPlayerObjects(PhotonPlayer targetPlayer)
	{
		if (player == null)
		{
			UnityEngine.Debug.LogError("DestroyPlayerObjects() failed, cause parameter 'targetPlayer' was null.");
		}
		DestroyPlayerObjects(targetPlayer.ID);
	}

	public static void DestroyPlayerObjects(int targetPlayerId)
	{
		if (VerifyCanUseNetwork())
		{
			if (player.IsMasterClient || targetPlayerId == player.ID)
			{
				networkingPeer.DestroyPlayerObjects(targetPlayerId, localOnly: false);
			}
			else
			{
				UnityEngine.Debug.LogError("DestroyPlayerObjects() failed, cause players can only destroy their own GameObjects. A Master Client can destroy anyone's. This is master: " + isMasterClient);
			}
		}
	}

	public static void DestroyAll()
	{
		if (isMasterClient)
		{
			networkingPeer.DestroyAll(localOnly: false);
		}
		else
		{
			UnityEngine.Debug.LogError("Couldn't call DestroyAll() as only the master client is allowed to call this.");
		}
	}

	public static void RemoveRPCs(PhotonPlayer targetPlayer)
	{
		if (VerifyCanUseNetwork())
		{
			if (!targetPlayer.IsLocal && !isMasterClient)
			{
				UnityEngine.Debug.LogError("Error; Only the MasterClient can call RemoveRPCs for other players.");
			}
			else
			{
				networkingPeer.OpCleanRpcBuffer(targetPlayer.ID);
			}
		}
	}

	public static void RemoveRPCs(PhotonView targetPhotonView)
	{
		if (VerifyCanUseNetwork())
		{
			networkingPeer.CleanRpcBufferIfMine(targetPhotonView);
		}
	}

	public static void RemoveRPCsInGroup(int targetGroup)
	{
		if (VerifyCanUseNetwork())
		{
			networkingPeer.RemoveRPCsInGroup(targetGroup);
		}
	}

	internal static void RPC(PhotonView view, string methodName, PhotonTargets target, bool encrypt, params object[] parameters)
	{
		if (!VerifyCanUseNetwork())
		{
			return;
		}
		if (room == null)
		{
			UnityEngine.Debug.LogWarning("RPCs can only be sent in rooms. Call of \"" + methodName + "\" gets executed locally only, if at all.");
		}
		else if (networkingPeer != null)
		{
			if (room.serverSideMasterClient)
			{
				networkingPeer.RPC(view, methodName, target, null, encrypt, parameters);
			}
			else if (networkingPeer.hasSwitchedMC && target == PhotonTargets.MasterClient)
			{
				networkingPeer.RPC(view, methodName, PhotonTargets.Others, masterClient, encrypt, parameters);
			}
			else
			{
				networkingPeer.RPC(view, methodName, target, null, encrypt, parameters);
			}
		}
		else
		{
			UnityEngine.Debug.LogWarning("Could not execute RPC " + methodName + ". Possible scene loading in progress?");
		}
	}

	internal static void RPC(PhotonView view, string methodName, PhotonPlayer targetPlayer, bool encrpyt, params object[] parameters)
	{
		if (!VerifyCanUseNetwork())
		{
			return;
		}
		if (room == null)
		{
			UnityEngine.Debug.LogWarning("RPCs can only be sent in rooms. Call of \"" + methodName + "\" gets executed locally only, if at all.");
			return;
		}
		if (player == null)
		{
			UnityEngine.Debug.LogError("RPC can't be sent to target PhotonPlayer being null! Did not send \"" + methodName + "\" call.");
		}
		if (networkingPeer != null)
		{
			networkingPeer.RPC(view, methodName, PhotonTargets.Others, targetPlayer, encrpyt, parameters);
		}
		else
		{
			UnityEngine.Debug.LogWarning("Could not execute RPC " + methodName + ". Possible scene loading in progress?");
		}
	}

	public static void CacheSendMonoMessageTargets(Type type)
	{
		if (type == null)
		{
			type = SendMonoMessageTargetType;
		}
		SendMonoMessageTargets = FindGameObjectsWithComponent(type);
	}

	public static HashSet<GameObject> FindGameObjectsWithComponent(Type type)
	{
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
		Component[] array = (Component[])UnityEngine.Object.FindObjectsOfType(type);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				hashSet.Add(array[i].gameObject);
			}
		}
		return hashSet;
	}

	[Obsolete("Use SetInterestGroups(byte group, bool enabled) instead.")]
	public static void SetReceivingEnabled(int group, bool enabled)
	{
		if (VerifyCanUseNetwork())
		{
			SetInterestGroups((byte)group, enabled);
		}
	}

	public static void SetInterestGroups(byte group, bool enabled)
	{
		if (VerifyCanUseNetwork())
		{
			if (enabled)
			{
				byte[] enableGroups = new byte[1] { group };
				networkingPeer.SetInterestGroups(null, enableGroups);
			}
			else
			{
				byte[] disableGroups = new byte[1] { group };
				networkingPeer.SetInterestGroups(disableGroups, null);
			}
		}
	}

	[Obsolete("Use SetInterestGroups(byte[] disableGroups, byte[] enableGroups) instead. Mind the parameter order!")]
	public static void SetReceivingEnabled(int[] enableGroups, int[] disableGroups)
	{
		if (VerifyCanUseNetwork())
		{
			byte[] array = null;
			byte[] array2 = null;
			if (enableGroups != null)
			{
				array2 = new byte[enableGroups.Length];
				Array.Copy(enableGroups, array2, enableGroups.Length);
			}
			if (disableGroups != null)
			{
				array = new byte[disableGroups.Length];
				Array.Copy(disableGroups, array, disableGroups.Length);
			}
			networkingPeer.SetInterestGroups(array, array2);
		}
	}

	public static void SetInterestGroups(byte[] disableGroups, byte[] enableGroups)
	{
		if (VerifyCanUseNetwork())
		{
			networkingPeer.SetInterestGroups(disableGroups, enableGroups);
		}
	}

	[Obsolete("Use SetSendingEnabled(byte group, bool enabled). Interest Groups have a byte-typed ID. Mind the parameter order.")]
	public static void SetSendingEnabled(int group, bool enabled)
	{
		SetSendingEnabled((byte)group, enabled);
	}

	public static void SetSendingEnabled(byte group, bool enabled)
	{
		if (VerifyCanUseNetwork())
		{
			networkingPeer.SetSendingEnabled(group, enabled);
		}
	}

	[Obsolete("Use SetSendingEnabled(byte group, bool enabled). Interest Groups have a byte-typed ID. Mind the parameter order.")]
	public static void SetSendingEnabled(int[] enableGroups, int[] disableGroups)
	{
		byte[] array = null;
		byte[] array2 = null;
		if (enableGroups != null)
		{
			array2 = new byte[enableGroups.Length];
			Array.Copy(enableGroups, array2, enableGroups.Length);
		}
		if (disableGroups != null)
		{
			array = new byte[disableGroups.Length];
			Array.Copy(disableGroups, array, disableGroups.Length);
		}
		SetSendingEnabled(array, array2);
	}

	public static void SetSendingEnabled(byte[] disableGroups, byte[] enableGroups)
	{
		if (VerifyCanUseNetwork())
		{
			networkingPeer.SetSendingEnabled(disableGroups, enableGroups);
		}
	}

	public static void SetLevelPrefix(short prefix)
	{
		if (VerifyCanUseNetwork())
		{
			networkingPeer.SetLevelPrefix(prefix);
		}
	}

	public static void LoadLevel(int levelNumber)
	{
		networkingPeer.AsynchLevelLoadCall = false;
		if (automaticallySyncScene)
		{
			networkingPeer.SetLevelInPropsIfSynced(levelNumber, initiatingCall: true);
		}
		isMessageQueueRunning = false;
		networkingPeer.loadingLevelAndPausedNetwork = true;
		SceneManager.LoadScene(levelNumber);
	}

	public static AsyncOperation LoadLevelAsync(int levelNumber)
	{
		networkingPeer.AsynchLevelLoadCall = true;
		if (automaticallySyncScene)
		{
			networkingPeer.SetLevelInPropsIfSynced(levelNumber, initiatingCall: true);
		}
		isMessageQueueRunning = false;
		networkingPeer.loadingLevelAndPausedNetwork = true;
		return SceneManager.LoadSceneAsync(levelNumber, LoadSceneMode.Single);
	}

	public static void LoadLevel(string levelName)
	{
		networkingPeer.AsynchLevelLoadCall = false;
		if (automaticallySyncScene)
		{
			networkingPeer.SetLevelInPropsIfSynced(levelName, initiatingCall: true);
		}
		isMessageQueueRunning = false;
		networkingPeer.loadingLevelAndPausedNetwork = true;
		SceneManager.LoadScene(levelName);
	}

	public static AsyncOperation LoadLevelAsync(string levelName)
	{
		networkingPeer.AsynchLevelLoadCall = true;
		if (automaticallySyncScene)
		{
			networkingPeer.SetLevelInPropsIfSynced(levelName, initiatingCall: true);
		}
		isMessageQueueRunning = false;
		networkingPeer.loadingLevelAndPausedNetwork = true;
		return SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
	}

	public static bool WebRpc(string name, object parameters)
	{
		return networkingPeer.WebRpc(name, parameters);
	}

	public static bool CallEvent(byte eventCode, object content, int senderId)
	{
		if (PhotonNetwork.OnEventCall != null)
		{
			PhotonNetwork.OnEventCall(eventCode, content, senderId);
			return true;
		}
		return false;
	}
}
