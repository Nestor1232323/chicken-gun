using System;

public class FriendInfo
{
	[Obsolete("Use UserId.")]
	public string Name => UserId;

	public string UserId { get; protected internal set; }

	public bool IsOnline { get; protected internal set; }

	public string Room { get; protected internal set; }

	public bool IsInRoom
	{
		get
		{
			if (IsOnline)
			{
				return !string.IsNullOrEmpty(Room);
			}
			return false;
		}
	}

	public override string ToString()
	{
		return string.Format("{0}\t is: {1}", UserId, (!IsOnline) ? "offline" : ((!IsInRoom) ? "on master" : "playing"));
	}
}
