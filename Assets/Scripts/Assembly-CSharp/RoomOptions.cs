using System;
using ExitGames.Client.Photon;

public class RoomOptions
{
	private bool isVisibleField = true;

	private bool isOpenField = true;

	public byte MaxPlayers;

	public int PlayerTtl;

	public int EmptyRoomTtl;

	private bool cleanupCacheOnLeaveField = PhotonNetwork.autoCleanUpPlayerObjects;

	public Hashtable CustomRoomProperties;

	public string[] CustomRoomPropertiesForLobby = new string[0];

	public string[] Plugins;

	private bool suppressRoomEventsField;

	private bool publishUserIdField;

	private bool deleteNullPropertiesField;

	public bool IsVisible
	{
		get
		{
			return isVisibleField;
		}
		set
		{
			isVisibleField = value;
		}
	}

	public bool IsOpen
	{
		get
		{
			return isOpenField;
		}
		set
		{
			isOpenField = value;
		}
	}

	public bool CleanupCacheOnLeave
	{
		get
		{
			return cleanupCacheOnLeaveField;
		}
		set
		{
			cleanupCacheOnLeaveField = value;
		}
	}

	public bool SuppressRoomEvents => suppressRoomEventsField;

	public bool PublishUserId
	{
		get
		{
			return publishUserIdField;
		}
		set
		{
			publishUserIdField = value;
		}
	}

	public bool DeleteNullProperties
	{
		get
		{
			return deleteNullPropertiesField;
		}
		set
		{
			deleteNullPropertiesField = value;
		}
	}

	[Obsolete("Use property with uppercase naming instead.")]
	public bool isVisible
	{
		get
		{
			return isVisibleField;
		}
		set
		{
			isVisibleField = value;
		}
	}

	[Obsolete("Use property with uppercase naming instead.")]
	public bool isOpen
	{
		get
		{
			return isOpenField;
		}
		set
		{
			isOpenField = value;
		}
	}

	[Obsolete("Use property with uppercase naming instead.")]
	public byte maxPlayers
	{
		get
		{
			return MaxPlayers;
		}
		set
		{
			MaxPlayers = value;
		}
	}

	[Obsolete("Use property with uppercase naming instead.")]
	public bool cleanupCacheOnLeave
	{
		get
		{
			return cleanupCacheOnLeaveField;
		}
		set
		{
			cleanupCacheOnLeaveField = value;
		}
	}

	[Obsolete("Use property with uppercase naming instead.")]
	public Hashtable customRoomProperties
	{
		get
		{
			return CustomRoomProperties;
		}
		set
		{
			CustomRoomProperties = value;
		}
	}

	[Obsolete("Use property with uppercase naming instead.")]
	public string[] customRoomPropertiesForLobby
	{
		get
		{
			return CustomRoomPropertiesForLobby;
		}
		set
		{
			CustomRoomPropertiesForLobby = value;
		}
	}

	[Obsolete("Use property with uppercase naming instead.")]
	public string[] plugins
	{
		get
		{
			return Plugins;
		}
		set
		{
			Plugins = value;
		}
	}

	[Obsolete("Use property with uppercase naming instead.")]
	public bool suppressRoomEvents => suppressRoomEventsField;

	[Obsolete("Use property with uppercase naming instead.")]
	public bool publishUserId
	{
		get
		{
			return publishUserIdField;
		}
		set
		{
			publishUserIdField = value;
		}
	}
}
