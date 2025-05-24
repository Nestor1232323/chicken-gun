using System;
using ExitGames.Client.Photon;
using UnityEngine;

public class Room : RoomInfo
{
	public new string Name
	{
		get
		{
			return nameField;
		}
		internal set
		{
			nameField = value;
		}
	}

	public new bool IsOpen
	{
		get
		{
			return openField;
		}
		set
		{
			if (!Equals(PhotonNetwork.room))
			{
				Debug.LogWarning("Can't set open when not in that room.");
			}
			if (value != openField && !PhotonNetwork.offlineMode)
			{
				PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(new Hashtable { 
				{
					(byte)253,
					value
				} });
			}
			openField = value;
		}
	}

	public new bool IsVisible
	{
		get
		{
			return visibleField;
		}
		set
		{
			if (!Equals(PhotonNetwork.room))
			{
				Debug.LogWarning("Can't set visible when not in that room.");
			}
			if (value != visibleField && !PhotonNetwork.offlineMode)
			{
				PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(new Hashtable { 
				{
					(byte)254,
					value
				} });
			}
			visibleField = value;
		}
	}

	public string[] PropertiesListedInLobby { get; private set; }

	public bool AutoCleanUp => autoCleanUpField;

	public new int MaxPlayers
	{
		get
		{
			return maxPlayersField;
		}
		set
		{
			if (!Equals(PhotonNetwork.room))
			{
				Debug.LogWarning("Can't set MaxPlayers when not in that room.");
			}
			if (value > 255)
			{
				Debug.LogWarning("Can't set Room.MaxPlayers to: " + value + ". Using max value: 255.");
				value = 255;
			}
			if (value != maxPlayersField && !PhotonNetwork.offlineMode)
			{
				PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(new Hashtable { 
				{
					byte.MaxValue,
					(byte)value
				} });
			}
			maxPlayersField = (byte)value;
		}
	}

	public new int PlayerCount
	{
		get
		{
			if (PhotonNetwork.playerList != null)
			{
				return PhotonNetwork.playerList.Length;
			}
			return 0;
		}
	}

	public string[] ExpectedUsers => expectedUsersField;

	public int PlayerTtl
	{
		get
		{
			return playerTtlField;
		}
		set
		{
			if (!Equals(PhotonNetwork.room))
			{
				Debug.LogWarning("Can't set PlayerTtl when not in a room.");
			}
			if (value != playerTtlField && !PhotonNetwork.offlineMode)
			{
				PhotonNetwork.networkingPeer.OpSetPropertyOfRoom(246, value);
			}
			playerTtlField = value;
		}
	}

	public int EmptyRoomTtl
	{
		get
		{
			return emptyRoomTtlField;
		}
		set
		{
			if (!Equals(PhotonNetwork.room))
			{
				Debug.LogWarning("Can't set EmptyRoomTtl when not in a room.");
			}
			if (value != emptyRoomTtlField && !PhotonNetwork.offlineMode)
			{
				PhotonNetwork.networkingPeer.OpSetPropertyOfRoom(245, value);
			}
			emptyRoomTtlField = value;
		}
	}

	protected internal int MasterClientId
	{
		get
		{
			return masterClientIdField;
		}
		set
		{
			masterClientIdField = value;
		}
	}

	[Obsolete("Please use Name (updated case for naming).")]
	public new string name
	{
		get
		{
			return Name;
		}
		internal set
		{
			Name = value;
		}
	}

	[Obsolete("Please use IsOpen (updated case for naming).")]
	public new bool open
	{
		get
		{
			return IsOpen;
		}
		set
		{
			IsOpen = value;
		}
	}

	[Obsolete("Please use IsVisible (updated case for naming).")]
	public new bool visible
	{
		get
		{
			return IsVisible;
		}
		set
		{
			IsVisible = value;
		}
	}

	[Obsolete("Please use PropertiesListedInLobby (updated case for naming).")]
	public string[] propertiesListedInLobby
	{
		get
		{
			return PropertiesListedInLobby;
		}
		private set
		{
			PropertiesListedInLobby = value;
		}
	}

	[Obsolete("Please use AutoCleanUp (updated case for naming).")]
	public bool autoCleanUp => AutoCleanUp;

	[Obsolete("Please use MaxPlayers (updated case for naming).")]
	public new int maxPlayers
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

	[Obsolete("Please use PlayerCount (updated case for naming).")]
	public new int playerCount => PlayerCount;

	[Obsolete("Please use ExpectedUsers (updated case for naming).")]
	public string[] expectedUsers => ExpectedUsers;

	[Obsolete("Please use MasterClientId (updated case for naming).")]
	protected internal int masterClientId
	{
		get
		{
			return MasterClientId;
		}
		set
		{
			MasterClientId = value;
		}
	}

	internal Room(string roomName, RoomOptions options)
		: base(roomName, null)
	{
		if (options == null)
		{
			options = new RoomOptions();
		}
		visibleField = options.IsVisible;
		openField = options.IsOpen;
		maxPlayersField = options.MaxPlayers;
		autoCleanUpField = options.CleanupCacheOnLeave;
		InternalCacheProperties(options.CustomRoomProperties);
		PropertiesListedInLobby = options.CustomRoomPropertiesForLobby;
	}

	public void SetCustomProperties(Hashtable propertiesToSet, Hashtable expectedValues = null, bool webForward = false)
	{
		if (propertiesToSet != null)
		{
			Hashtable hashtable = propertiesToSet.StripToStringKeys();
			Hashtable hashtable2 = expectedValues.StripToStringKeys();
			bool flag = hashtable2 == null || hashtable2.Count == 0;
			if (PhotonNetwork.offlineMode || flag)
			{
				base.CustomProperties.Merge(hashtable);
				base.CustomProperties.StripKeysWithNullValues();
			}
			if (!PhotonNetwork.offlineMode)
			{
				PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(hashtable, hashtable2, webForward);
			}
			if (PhotonNetwork.offlineMode || flag)
			{
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonCustomRoomPropertiesChanged, hashtable);
			}
		}
	}

	public void SetPropertiesListedInLobby(string[] propsListedInLobby)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[(byte)250] = propsListedInLobby;
		PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(hashtable);
		PropertiesListedInLobby = propsListedInLobby;
	}

	public void ClearExpectedUsers()
	{
		Hashtable hashtable = new Hashtable();
		hashtable[(byte)247] = new string[0];
		Hashtable hashtable2 = new Hashtable();
		hashtable2[(byte)247] = ExpectedUsers;
		PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(hashtable, hashtable2);
	}

	public void SetExpectedUsers(string[] expectedUsers)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[(byte)247] = expectedUsers;
		Hashtable hashtable2 = new Hashtable();
		hashtable2[(byte)247] = ExpectedUsers;
		PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(hashtable, hashtable2);
	}

	public override string ToString()
	{
		return string.Format("Room: '{0}' {1},{2} {4}/{3} players.", nameField, (!visibleField) ? "hidden" : "visible", (!openField) ? "closed" : "open", maxPlayersField, PlayerCount);
	}

	public new string ToStringFull()
	{
		return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", nameField, (!visibleField) ? "hidden" : "visible", (!openField) ? "closed" : "open", maxPlayersField, PlayerCount, base.CustomProperties.ToStringFull());
	}
}
