using System;
using ExitGames.Client.Photon;

public class RoomInfo
{
	private Hashtable customPropertiesField = new Hashtable();

	protected byte maxPlayersField;

	protected int emptyRoomTtlField;

	protected int playerTtlField;

	protected string[] expectedUsersField;

	protected bool openField = true;

	protected bool visibleField = true;

	protected bool autoCleanUpField = PhotonNetwork.autoCleanUpPlayerObjects;

	protected string nameField;

	protected internal int masterClientIdField;

	public bool removedFromList { get; internal set; }

	protected internal bool serverSideMasterClient { get; private set; }

	public Hashtable CustomProperties
	{
		get
		{
			return customPropertiesField;
		}
	}

	public string Name
	{
		get
		{
			return nameField;
		}
	}

	public int PlayerCount { get; private set; }

	public bool IsLocalClientInside { get; set; }

	public byte MaxPlayers
	{
		get
		{
			return maxPlayersField;
		}
	}

	public bool IsOpen
	{
		get
		{
			return openField;
		}
	}

	public bool IsVisible
	{
		get
		{
			return visibleField;
		}
	}

	[Obsolete("Please use CustomProperties (updated case for naming).")]
	public Hashtable customProperties
	{
		get
		{
			return CustomProperties;
		}
	}

	[Obsolete("Please use Name (updated case for naming).")]
	public string name
	{
		get
		{
			return Name;
		}
	}

	[Obsolete("Please use PlayerCount (updated case for naming).")]
	public int playerCount
	{
		get
		{
			return PlayerCount;
		}
		set
		{
			PlayerCount = value;
		}
	}

	[Obsolete("Please use IsLocalClientInside (updated case for naming).")]
	public bool isLocalClientInside
	{
		get
		{
			return IsLocalClientInside;
		}
		set
		{
			IsLocalClientInside = value;
		}
	}

	[Obsolete("Please use MaxPlayers (updated case for naming).")]
	public byte maxPlayers
	{
		get
		{
			return MaxPlayers;
		}
	}

	[Obsolete("Please use IsOpen (updated case for naming).")]
	public bool open
	{
		get
		{
			return IsOpen;
		}
	}

	[Obsolete("Please use IsVisible (updated case for naming).")]
	public bool visible
	{
		get
		{
			return IsVisible;
		}
	}

	protected internal RoomInfo(string roomName, Hashtable properties)
	{
		InternalCacheProperties(properties);
		nameField = roomName;
	}

	public override bool Equals(object other)
	{
		RoomInfo roomInfo = other as RoomInfo;
		return roomInfo != null && Name.Equals(roomInfo.nameField);
	}

	public override int GetHashCode()
	{
		return nameField.GetHashCode();
	}

	public override string ToString()
	{
		return string.Format("Room: '{0}' {1},{2} {4}/{3} players.", nameField, (!visibleField) ? "hidden" : "visible", (!openField) ? "closed" : "open", maxPlayersField, PlayerCount);
	}

	public string ToStringFull()
	{
		return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", nameField, (!visibleField) ? "hidden" : "visible", (!openField) ? "closed" : "open", maxPlayersField, PlayerCount, customPropertiesField.ToStringFull());
	}

	protected internal void InternalCacheProperties(Hashtable propertiesToCache)
	{
		if (propertiesToCache == null || propertiesToCache.Count == 0 || customPropertiesField.Equals(propertiesToCache))
		{
			return;
		}
		if (propertiesToCache.ContainsKey((byte)251))
		{
			removedFromList = (bool)propertiesToCache[(byte)251];
			if (removedFromList)
			{
				return;
			}
		}
		if (propertiesToCache.ContainsKey(byte.MaxValue))
		{
			maxPlayersField = (byte)propertiesToCache[byte.MaxValue];
		}
		if (propertiesToCache.ContainsKey((byte)253))
		{
			openField = (bool)propertiesToCache[(byte)253];
		}
		if (propertiesToCache.ContainsKey((byte)254))
		{
			visibleField = (bool)propertiesToCache[(byte)254];
		}
		if (propertiesToCache.ContainsKey((byte)252))
		{
			PlayerCount = (byte)propertiesToCache[(byte)252];
		}
		if (propertiesToCache.ContainsKey((byte)249))
		{
			autoCleanUpField = (bool)propertiesToCache[(byte)249];
		}
		if (propertiesToCache.ContainsKey((byte)248))
		{
			serverSideMasterClient = true;
			bool flag = masterClientIdField != 0;
			masterClientIdField = (int)propertiesToCache[(byte)248];
			if (flag)
			{
				PhotonNetwork.networkingPeer.UpdateMasterClient();
			}
		}
		if (propertiesToCache.ContainsKey((byte)247))
		{
			expectedUsersField = (string[])propertiesToCache[(byte)247];
		}
		if (propertiesToCache.ContainsKey((byte)245))
		{
			emptyRoomTtlField = (int)propertiesToCache[(byte)245];
		}
		if (propertiesToCache.ContainsKey((byte)246))
		{
			playerTtlField = (int)propertiesToCache[(byte)246];
		}
		customPropertiesField.MergeStringKeys(propertiesToCache);
		customPropertiesField.StripKeysWithNullValues();
	}
}
