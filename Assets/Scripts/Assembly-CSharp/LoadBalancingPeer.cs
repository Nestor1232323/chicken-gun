using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

internal class LoadBalancingPeer : PhotonPeer
{
	private enum RoomOptionBit
	{
		CheckUserOnJoin = 1,
		DeleteCacheOnLeave = 2,
		SuppressRoomEvents = 4,
		PublishUserId = 8,
		DeleteNullProps = 0x10,
		BroadcastPropsChangeToAll = 0x20
	}

	private readonly Dictionary<byte, object> opParameters = new Dictionary<byte, object>();

	internal bool IsProtocolSecure => base.UsedProtocol == ConnectionProtocol.WebSocketSecure;

	public LoadBalancingPeer(ConnectionProtocol protocolType)
		: base(protocolType)
	{
	}

	public LoadBalancingPeer(IPhotonPeerListener listener, ConnectionProtocol protocolType)
		: this(protocolType)
	{
		base.Listener = listener;
	}

	public virtual bool OpGetRegions(string appId)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary[224] = appId;
		return OpCustom(220, dictionary, sendReliable: true, 0, encrypt: true);
	}

	public virtual bool OpJoinLobby(TypedLobby lobby = null)
	{
		if ((int)DebugOut >= 3)
		{
			base.Listener.DebugReturn(DebugLevel.INFO, "OpJoinLobby()");
		}
		Dictionary<byte, object> dictionary = null;
		if (lobby != null && !lobby.IsDefault)
		{
			dictionary = new Dictionary<byte, object>();
			dictionary[213] = lobby.Name;
			dictionary[212] = (byte)lobby.Type;
		}
		return OpCustom(229, dictionary, sendReliable: true);
	}

	public virtual bool OpLeaveLobby()
	{
		if ((int)DebugOut >= 3)
		{
			base.Listener.DebugReturn(DebugLevel.INFO, "OpLeaveLobby()");
		}
		return OpCustom(228, null, sendReliable: true);
	}

	private void RoomOptionsToOpParameters(Dictionary<byte, object> op, RoomOptions roomOptions)
	{
		if (roomOptions == null)
		{
			roomOptions = new RoomOptions();
		}
		Hashtable hashtable = new Hashtable();
		hashtable[(byte)253] = roomOptions.IsOpen;
		hashtable[(byte)254] = roomOptions.IsVisible;
		hashtable[(byte)250] = ((roomOptions.CustomRoomPropertiesForLobby != null) ? roomOptions.CustomRoomPropertiesForLobby : new string[0]);
		hashtable.MergeStringKeys(roomOptions.CustomRoomProperties);
		if (roomOptions.MaxPlayers > 0)
		{
			hashtable[byte.MaxValue] = roomOptions.MaxPlayers;
		}
		op[248] = hashtable;
		int num = 0;
		op[241] = roomOptions.CleanupCacheOnLeave;
		if (roomOptions.CleanupCacheOnLeave)
		{
			num |= 2;
			hashtable[(byte)249] = true;
		}
		num |= 1;
		op[232] = true;
		if (roomOptions.PlayerTtl > 0 || roomOptions.PlayerTtl == -1)
		{
			op[235] = roomOptions.PlayerTtl;
		}
		if (roomOptions.EmptyRoomTtl > 0)
		{
			op[236] = roomOptions.EmptyRoomTtl;
		}
		if (roomOptions.SuppressRoomEvents)
		{
			num |= 4;
			op[237] = true;
		}
		if (roomOptions.Plugins != null)
		{
			op[204] = roomOptions.Plugins;
		}
		if (roomOptions.PublishUserId)
		{
			num |= 8;
			op[239] = true;
		}
		if (roomOptions.DeleteNullProperties)
		{
			num |= 0x10;
		}
		op[191] = num;
	}

	public virtual bool OpCreateRoom(EnterRoomParams opParams)
	{
		if ((int)DebugOut >= 3)
		{
			base.Listener.DebugReturn(DebugLevel.INFO, "OpCreateRoom()");
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		if (!string.IsNullOrEmpty(opParams.RoomName))
		{
			dictionary[byte.MaxValue] = opParams.RoomName;
		}
		if (opParams.Lobby != null && !string.IsNullOrEmpty(opParams.Lobby.Name))
		{
			dictionary[213] = opParams.Lobby.Name;
			dictionary[212] = (byte)opParams.Lobby.Type;
		}
		if (opParams.ExpectedUsers != null && opParams.ExpectedUsers.Length != 0)
		{
			dictionary[238] = opParams.ExpectedUsers;
		}
		if (opParams.OnGameServer)
		{
			if (opParams.PlayerProperties != null && opParams.PlayerProperties.Count > 0)
			{
				dictionary[249] = opParams.PlayerProperties;
				dictionary[250] = true;
			}
			RoomOptionsToOpParameters(dictionary, opParams.RoomOptions);
		}
		return OpCustom(227, dictionary, sendReliable: true);
	}

	public virtual bool OpJoinRoom(EnterRoomParams opParams)
	{
		if ((int)DebugOut >= 3)
		{
			base.Listener.DebugReturn(DebugLevel.INFO, "OpJoinRoom()");
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		if (!string.IsNullOrEmpty(opParams.RoomName))
		{
			dictionary[byte.MaxValue] = opParams.RoomName;
		}
		if (opParams.CreateIfNotExists)
		{
			dictionary[215] = (byte)1;
			if (opParams.Lobby != null)
			{
				dictionary[213] = opParams.Lobby.Name;
				dictionary[212] = (byte)opParams.Lobby.Type;
			}
		}
		if (opParams.RejoinOnly)
		{
			dictionary[215] = (byte)3;
		}
		if (opParams.ExpectedUsers != null && opParams.ExpectedUsers.Length != 0)
		{
			dictionary[238] = opParams.ExpectedUsers;
		}
		if (opParams.OnGameServer)
		{
			if (opParams.PlayerProperties != null && opParams.PlayerProperties.Count > 0)
			{
				dictionary[249] = opParams.PlayerProperties;
				dictionary[250] = true;
			}
			if (opParams.CreateIfNotExists)
			{
				RoomOptionsToOpParameters(dictionary, opParams.RoomOptions);
			}
		}
		return OpCustom(226, dictionary, sendReliable: true);
	}

	public virtual bool OpJoinRandomRoom(OpJoinRandomRoomParams opJoinRandomRoomParams)
	{
		if ((int)DebugOut >= 3)
		{
			base.Listener.DebugReturn(DebugLevel.INFO, "OpJoinRandomRoom()");
		}
		Hashtable hashtable = new Hashtable();
		hashtable.MergeStringKeys(opJoinRandomRoomParams.ExpectedCustomRoomProperties);
		if (opJoinRandomRoomParams.ExpectedMaxPlayers > 0)
		{
			hashtable[byte.MaxValue] = opJoinRandomRoomParams.ExpectedMaxPlayers;
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		if (hashtable.Count > 0)
		{
			dictionary[248] = hashtable;
		}
		if (opJoinRandomRoomParams.MatchingType != MatchmakingMode.FillRoom)
		{
			dictionary[223] = (byte)opJoinRandomRoomParams.MatchingType;
		}
		if (opJoinRandomRoomParams.TypedLobby != null && !string.IsNullOrEmpty(opJoinRandomRoomParams.TypedLobby.Name))
		{
			dictionary[213] = opJoinRandomRoomParams.TypedLobby.Name;
			dictionary[212] = (byte)opJoinRandomRoomParams.TypedLobby.Type;
		}
		if (!string.IsNullOrEmpty(opJoinRandomRoomParams.SqlLobbyFilter))
		{
			dictionary[245] = opJoinRandomRoomParams.SqlLobbyFilter;
		}
		if (opJoinRandomRoomParams.ExpectedUsers != null && opJoinRandomRoomParams.ExpectedUsers.Length != 0)
		{
			dictionary[238] = opJoinRandomRoomParams.ExpectedUsers;
		}
		return OpCustom(225, dictionary, sendReliable: true);
	}

	public virtual bool OpLeaveRoom(bool becomeInactive)
	{
		Dictionary<byte, object> dictionary = null;
		if (becomeInactive)
		{
			dictionary = new Dictionary<byte, object>();
			dictionary[233] = becomeInactive;
		}
		return OpCustom(254, dictionary, sendReliable: true);
	}

	public virtual bool OpGetGameList(TypedLobby lobby, string queryData)
	{
		if ((int)DebugOut >= 3)
		{
			base.Listener.DebugReturn(DebugLevel.INFO, "OpGetGameList()");
		}
		if (lobby == null)
		{
			if ((int)DebugOut >= 3)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpGetGameList not sent. Lobby cannot be null.");
			}
			return false;
		}
		if (lobby.Type != LobbyType.SqlLobby)
		{
			if ((int)DebugOut >= 3)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpGetGameList not sent. LobbyType must be SqlLobby.");
			}
			return false;
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary[213] = lobby.Name;
		dictionary[212] = (byte)lobby.Type;
		dictionary[245] = queryData;
		return OpCustom(217, dictionary, sendReliable: true);
	}

	public virtual bool OpFindFriends(string[] friendsToFind)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		if (friendsToFind != null && friendsToFind.Length != 0)
		{
			dictionary[1] = friendsToFind;
		}
		return OpCustom(222, dictionary, sendReliable: true);
	}

	public bool OpSetCustomPropertiesOfActor(int actorNr, Hashtable actorProperties)
	{
		return OpSetPropertiesOfActor(actorNr, actorProperties.StripToStringKeys());
	}

	protected internal bool OpSetPropertiesOfActor(int actorNr, Hashtable actorProperties, Hashtable expectedProperties = null, bool webForward = false)
	{
		if ((int)DebugOut >= 3)
		{
			base.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfActor()");
		}
		if (actorNr <= 0 || actorProperties == null)
		{
			if ((int)DebugOut >= 3)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfActor not sent. ActorNr must be > 0 and actorProperties != null.");
			}
			return false;
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(251, actorProperties);
		dictionary.Add(254, actorNr);
		dictionary.Add(250, true);
		if (expectedProperties != null && expectedProperties.Count != 0)
		{
			dictionary.Add(231, expectedProperties);
		}
		if (webForward)
		{
			dictionary[234] = true;
		}
		return OpCustom(252, dictionary, sendReliable: true, 0, encrypt: false);
	}

	protected internal void OpSetPropertyOfRoom(byte propCode, object value)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[propCode] = value;
		OpSetPropertiesOfRoom(hashtable);
	}

	public bool OpSetCustomPropertiesOfRoom(Hashtable gameProperties, bool broadcast, byte channelId)
	{
		return OpSetPropertiesOfRoom(gameProperties.StripToStringKeys());
	}

	protected internal bool OpSetPropertiesOfRoom(Hashtable gameProperties, Hashtable expectedProperties = null, bool webForward = false)
	{
		if ((int)DebugOut >= 3)
		{
			base.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfRoom()");
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(251, gameProperties);
		dictionary.Add(250, true);
		if (expectedProperties != null && expectedProperties.Count != 0)
		{
			dictionary.Add(231, expectedProperties);
		}
		if (webForward)
		{
			dictionary[234] = true;
		}
		return OpCustom(252, dictionary, sendReliable: true, 0, encrypt: false);
	}

	public virtual bool OpAuthenticate(string appId, string appVersion, AuthenticationValues authValues, string regionCode, bool getLobbyStatistics)
	{
		if ((int)DebugOut >= 3)
		{
			base.Listener.DebugReturn(DebugLevel.INFO, "OpAuthenticate()");
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		if (getLobbyStatistics)
		{
			dictionary[211] = true;
		}
		if (authValues != null && authValues.Token != null)
		{
			dictionary[221] = authValues.Token;
			return OpCustom(230, dictionary, sendReliable: true, 0, encrypt: false);
		}
		dictionary[220] = appVersion;
		dictionary[224] = appId;
		if (!string.IsNullOrEmpty(regionCode))
		{
			dictionary[210] = regionCode;
		}
		if (authValues != null)
		{
			if (!string.IsNullOrEmpty(authValues.UserId))
			{
				dictionary[225] = authValues.UserId;
			}
			if (authValues.AuthType != CustomAuthenticationType.None)
			{
				if (!IsProtocolSecure && !base.IsEncryptionAvailable)
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, "OpAuthenticate() failed. When you want Custom Authentication encryption is mandatory.");
					return false;
				}
				dictionary[217] = (byte)authValues.AuthType;
				if (!string.IsNullOrEmpty(authValues.Token))
				{
					dictionary[221] = authValues.Token;
				}
				else
				{
					if (!string.IsNullOrEmpty(authValues.AuthGetParameters))
					{
						dictionary[216] = authValues.AuthGetParameters;
					}
					if (authValues.AuthPostData != null)
					{
						dictionary[214] = authValues.AuthPostData;
					}
				}
			}
		}
		bool num = OpCustom(230, dictionary, sendReliable: true, 0, base.IsEncryptionAvailable);
		if (!num)
		{
			base.Listener.DebugReturn(DebugLevel.ERROR, "Error calling OpAuthenticate! Did not work. Check log output, AuthValues and if you're connected.");
		}
		return num;
	}

	public virtual bool OpAuthenticateOnce(string appId, string appVersion, AuthenticationValues authValues, string regionCode, EncryptionMode encryptionMode, ConnectionProtocol expectedProtocol)
	{
		if ((int)DebugOut >= 3)
		{
			base.Listener.DebugReturn(DebugLevel.INFO, "OpAuthenticate()");
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		if (authValues != null && authValues.Token != null)
		{
			dictionary[221] = authValues.Token;
			return OpCustom(231, dictionary, sendReliable: true, 0, encrypt: false);
		}
		if (encryptionMode == EncryptionMode.DatagramEncryption && expectedProtocol != ConnectionProtocol.Udp)
		{
			Debug.LogWarning("Expected protocol set to UDP, due to encryption mode DatagramEncryption. Changing protocol in PhotonServerSettings from: " + PhotonNetwork.PhotonServerSettings.Protocol);
			PhotonNetwork.PhotonServerSettings.Protocol = ConnectionProtocol.Udp;
			expectedProtocol = ConnectionProtocol.Udp;
		}
		dictionary[195] = (byte)expectedProtocol;
		dictionary[193] = (byte)encryptionMode;
		dictionary[220] = appVersion;
		dictionary[224] = appId;
		if (!string.IsNullOrEmpty(regionCode))
		{
			dictionary[210] = regionCode;
		}
		if (authValues != null)
		{
			if (!string.IsNullOrEmpty(authValues.UserId))
			{
				dictionary[225] = authValues.UserId;
			}
			if (authValues.AuthType != CustomAuthenticationType.None)
			{
				dictionary[217] = (byte)authValues.AuthType;
				if (!string.IsNullOrEmpty(authValues.Token))
				{
					dictionary[221] = authValues.Token;
				}
				else
				{
					if (!string.IsNullOrEmpty(authValues.AuthGetParameters))
					{
						dictionary[216] = authValues.AuthGetParameters;
					}
					if (authValues.AuthPostData != null)
					{
						dictionary[214] = authValues.AuthPostData;
					}
				}
			}
		}
		return OpCustom(231, dictionary, sendReliable: true, 0, base.IsEncryptionAvailable);
	}

	public virtual bool OpChangeGroups(byte[] groupsToRemove, byte[] groupsToAdd)
	{
		if ((int)DebugOut >= 5)
		{
			base.Listener.DebugReturn(DebugLevel.ALL, "OpChangeGroups()");
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		if (groupsToRemove != null)
		{
			dictionary[239] = groupsToRemove;
		}
		if (groupsToAdd != null)
		{
			dictionary[238] = groupsToAdd;
		}
		return OpCustom(248, dictionary, sendReliable: true, 0);
	}

	public virtual bool OpRaiseEvent(byte eventCode, object customEventContent, bool sendReliable, RaiseEventOptions raiseEventOptions)
	{
		opParameters.Clear();
		opParameters[244] = eventCode;
		if (customEventContent != null)
		{
			opParameters[245] = customEventContent;
		}
		if (raiseEventOptions == null)
		{
			raiseEventOptions = RaiseEventOptions.Default;
		}
		else
		{
			if (raiseEventOptions.CachingOption != EventCaching.DoNotCache)
			{
				opParameters[247] = (byte)raiseEventOptions.CachingOption;
			}
			if (raiseEventOptions.Receivers != ReceiverGroup.Others)
			{
				opParameters[246] = (byte)raiseEventOptions.Receivers;
			}
			if (raiseEventOptions.InterestGroup != 0)
			{
				opParameters[240] = raiseEventOptions.InterestGroup;
			}
			if (raiseEventOptions.TargetActors != null)
			{
				opParameters[252] = raiseEventOptions.TargetActors;
			}
			if (raiseEventOptions.ForwardToWebhook)
			{
				opParameters[234] = true;
			}
		}
		return OpCustom(253, opParameters, sendReliable, raiseEventOptions.SequenceChannel, raiseEventOptions.Encrypt);
	}

	public virtual bool OpSettings(bool receiveLobbyStats)
	{
		if ((int)DebugOut >= 5)
		{
			base.Listener.DebugReturn(DebugLevel.ALL, "OpSettings()");
		}
		opParameters.Clear();
		if (receiveLobbyStats)
		{
			opParameters[0] = receiveLobbyStats;
		}
		if (opParameters.Count == 0)
		{
			return true;
		}
		return OpCustom(218, opParameters, sendReliable: true);
	}
}
