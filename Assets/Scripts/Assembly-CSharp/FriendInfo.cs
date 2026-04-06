using System;

public class FriendInfo
{
	[Obsolete("Use UserId.")]
	public string Name
	{
		get
		{
			return UserId;
		}
	}

	public string UserId { get; protected internal set; }

	public bool IsOnline { get; protected internal set; }

	public string Room { get; protected internal set; }

	public bool IsInRoom
	{
		get
		{
			return IsOnline && !string.IsNullOrEmpty(Room);
		}
	}

	public override string ToString()
	{
		return string.Format("{0}\t is: {1}", UserId, (!IsOnline) ? "offline" : ((!IsInRoom) ? "on master" : "playing"));
	}
}
