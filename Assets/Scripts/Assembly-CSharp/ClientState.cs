public enum ClientState
{
	Uninitialized = 0,
	PeerCreated = 1,
	Queued = 2,
	Authenticated = 3,
	JoinedLobby = 4,
	DisconnectingFromMasterserver = 5,
	ConnectingToGameserver = 6,
	ConnectedToGameserver = 7,
	Joining = 8,
	Joined = 9,
	Leaving = 10,
	DisconnectingFromGameserver = 11,
	ConnectingToMasterserver = 12,
	QueuedComingFromGameserver = 13,
	Disconnecting = 14,
	Disconnected = 15,
	ConnectedToMaster = 16,
	ConnectingToNameServer = 17,
	ConnectedToNameServer = 18,
	DisconnectingFromNameServer = 19,
	Authenticating = 20
}
