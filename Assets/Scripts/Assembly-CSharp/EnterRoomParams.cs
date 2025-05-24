using ExitGames.Client.Photon;

internal class EnterRoomParams
{
	public string RoomName;

	public RoomOptions RoomOptions;

	public TypedLobby Lobby;

	public Hashtable PlayerProperties;

	public bool OnGameServer = true;

	public bool CreateIfNotExists;

	public bool RejoinOnly;

	public string[] ExpectedUsers;
}
