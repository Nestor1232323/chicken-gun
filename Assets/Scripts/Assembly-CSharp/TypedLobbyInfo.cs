public class TypedLobbyInfo : TypedLobby
{
	public int PlayerCount;

	public int RoomCount;

	public override string ToString()
	{
		return $"TypedLobbyInfo '{Name}'[{Type}] rooms: {RoomCount} players: {PlayerCount}";
	}
}
