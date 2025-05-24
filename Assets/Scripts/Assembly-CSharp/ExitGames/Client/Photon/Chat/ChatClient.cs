using System;
using System.Collections.Generic;

namespace ExitGames.Client.Photon.Chat
{
	public class ChatClient : IPhotonPeerListener
	{
		private const int FriendRequestListMax = 1024;

		private string chatRegion = "EU";

		public int MessageLimit;

		public readonly Dictionary<string, ChatChannel> PublicChannels;

		public readonly Dictionary<string, ChatChannel> PrivateChannels;

		private readonly HashSet<string> PublicChannelsUnsubscribing;

		private readonly IChatClientListener listener;

		public ChatPeer chatPeer;

		private const string ChatAppName = "chat";

		private bool didAuthenticate;

		private int msDeltaForServiceCalls = 50;

		private int msTimestampOfLastServiceCall;

		public string NameServerAddress { get; private set; }

		public string FrontendAddress { get; private set; }

		public string ChatRegion
		{
			get
			{
				return chatRegion;
			}
			set
			{
				chatRegion = value;
			}
		}

		public ChatState State { get; private set; }

		public ChatDisconnectCause DisconnectedCause { get; private set; }

		public bool CanChat
		{
			get
			{
				if (State == ChatState.ConnectedToFrontEnd)
				{
					return HasPeer;
				}
				return false;
			}
		}

		private bool HasPeer => chatPeer != null;

		public string AppVersion { get; private set; }

		public string AppId { get; private set; }

		public AuthenticationValues AuthValues { get; set; }

		public string UserId
		{
			get
			{
				if (AuthValues != null)
				{
					return AuthValues.UserId;
				}
				return null;
			}
			private set
			{
				if (AuthValues == null)
				{
					AuthValues = new AuthenticationValues();
				}
				AuthValues.UserId = value;
			}
		}

		public bool UseBackgroundWorkerForSending { get; set; }

		public ConnectionProtocol TransportProtocol
		{
			get
			{
				return chatPeer.TransportProtocol;
			}
			set
			{
				if (chatPeer == null || chatPeer.PeerState != PeerStateValue.Disconnected)
				{
					listener.DebugReturn(DebugLevel.WARNING, "Can't set TransportProtocol. Disconnect first! " + ((chatPeer == null) ? "The chatPeer is null." : ("PeerState: " + chatPeer.PeerState)));
				}
				else
				{
					chatPeer.TransportProtocol = value;
				}
			}
		}

		public Dictionary<ConnectionProtocol, Type> SocketImplementationConfig => chatPeer.SocketImplementationConfig;

		public DebugLevel DebugOut
		{
			get
			{
				return chatPeer.DebugOut;
			}
			set
			{
				chatPeer.DebugOut = value;
			}
		}

		public ChatClient(IChatClientListener listener, ConnectionProtocol protocol = ConnectionProtocol.Udp)
		{
			this.listener = listener;
			State = ChatState.Uninitialized;
			chatPeer = new ChatPeer(this, protocol);
			PublicChannels = new Dictionary<string, ChatChannel>();
			PrivateChannels = new Dictionary<string, ChatChannel>();
			PublicChannelsUnsubscribing = new HashSet<string>();
		}

		public bool CanChatInChannel(string channelName)
		{
			if (CanChat && PublicChannels.ContainsKey(channelName))
			{
				return !PublicChannelsUnsubscribing.Contains(channelName);
			}
			return false;
		}

		public bool Connect(string appId, string appVersion, AuthenticationValues authValues)
		{
			chatPeer.TimePingInterval = 3000;
			DisconnectedCause = ChatDisconnectCause.None;
			if (authValues != null)
			{
				AuthValues = authValues;
				if (string.IsNullOrEmpty(AuthValues.UserId))
				{
					if ((int)DebugOut >= 1)
					{
						listener.DebugReturn(DebugLevel.ERROR, "Connect failed: no UserId specified in authentication values.");
					}
					return false;
				}
				AppId = appId;
				AppVersion = appVersion;
				didAuthenticate = false;
				chatPeer.QuickResendAttempts = 2;
				chatPeer.SentCountAllowance = 7;
				PublicChannels.Clear();
				PrivateChannels.Clear();
				PublicChannelsUnsubscribing.Clear();
				NameServerAddress = chatPeer.NameServerAddress;
				bool num = chatPeer.Connect();
				if (num)
				{
					State = ChatState.ConnectingToNameServer;
				}
				if (UseBackgroundWorkerForSending)
				{
					SupportClass.StartBackgroundCalls(SendOutgoingInBackground, msDeltaForServiceCalls, "ChatClient Service Thread");
				}
				return num;
			}
			if ((int)DebugOut >= 1)
			{
				listener.DebugReturn(DebugLevel.ERROR, "Connect failed: no authentication values specified");
			}
			return false;
		}

		public void Service()
		{
			while (HasPeer && chatPeer.DispatchIncomingCommands())
			{
			}
			if (!UseBackgroundWorkerForSending && (Environment.TickCount - msTimestampOfLastServiceCall > msDeltaForServiceCalls || msTimestampOfLastServiceCall == 0))
			{
				msTimestampOfLastServiceCall = Environment.TickCount;
				while (HasPeer && chatPeer.SendOutgoingCommands())
				{
				}
			}
		}

		private bool SendOutgoingInBackground()
		{
			while (HasPeer && chatPeer.SendOutgoingCommands())
			{
			}
			return State != ChatState.Disconnected;
		}

		[Obsolete("Better use UseBackgroundWorkerForSending and Service().")]
		public void SendAcksOnly()
		{
			if (HasPeer)
			{
				chatPeer.SendAcksOnly();
			}
		}

		public void Disconnect()
		{
			if (HasPeer && chatPeer.PeerState != PeerStateValue.Disconnected)
			{
				chatPeer.Disconnect();
			}
		}

		public void StopThread()
		{
			if (HasPeer)
			{
				chatPeer.StopThread();
			}
		}

		public bool Subscribe(string[] channels)
		{
			return Subscribe(channels, 0);
		}

		public bool Subscribe(string[] channels, int messagesFromHistory)
		{
			if (!CanChat)
			{
				if ((int)DebugOut >= 1)
				{
					listener.DebugReturn(DebugLevel.ERROR, "Subscribe called while not connected to front end server.");
				}
				return false;
			}
			if (channels == null || channels.Length == 0)
			{
				if ((int)DebugOut >= 2)
				{
					listener.DebugReturn(DebugLevel.WARNING, "Subscribe can't be called for empty or null channels-list.");
				}
				return false;
			}
			return SendChannelOperation(channels, 0, messagesFromHistory);
		}

		public bool Unsubscribe(string[] channels)
		{
			if (!CanChat)
			{
				if ((int)DebugOut >= 1)
				{
					listener.DebugReturn(DebugLevel.ERROR, "Unsubscribe called while not connected to front end server.");
				}
				return false;
			}
			if (channels == null || channels.Length == 0)
			{
				if ((int)DebugOut >= 2)
				{
					listener.DebugReturn(DebugLevel.WARNING, "Unsubscribe can't be called for empty or null channels-list.");
				}
				return false;
			}
			foreach (string item in channels)
			{
				PublicChannelsUnsubscribing.Add(item);
			}
			return SendChannelOperation(channels, 1, 0);
		}

		public bool PublishMessage(string channelName, object message, bool forwardAsWebhook = false)
		{
			return publishMessage(channelName, message, reliable: true, forwardAsWebhook);
		}

		internal bool PublishMessageUnreliable(string channelName, object message, bool forwardAsWebhook = false)
		{
			return publishMessage(channelName, message, reliable: false, forwardAsWebhook);
		}

		private bool publishMessage(string channelName, object message, bool reliable, bool forwardAsWebhook = false)
		{
			if (!CanChat)
			{
				if ((int)DebugOut >= 1)
				{
					listener.DebugReturn(DebugLevel.ERROR, "PublishMessage called while not connected to front end server.");
				}
				return false;
			}
			if (string.IsNullOrEmpty(channelName) || message == null)
			{
				if ((int)DebugOut >= 2)
				{
					listener.DebugReturn(DebugLevel.WARNING, "PublishMessage parameters must be non-null and not empty.");
				}
				return false;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>
			{
				{ 1, channelName },
				{ 3, message }
			};
			if (forwardAsWebhook)
			{
				dictionary.Add(21, (byte)1);
			}
			return chatPeer.OpCustom(2, dictionary, reliable);
		}

		public bool SendPrivateMessage(string target, object message, bool forwardAsWebhook = false)
		{
			return SendPrivateMessage(target, message, encrypt: false, forwardAsWebhook);
		}

		public bool SendPrivateMessage(string target, object message, bool encrypt, bool forwardAsWebhook)
		{
			return sendPrivateMessage(target, message, encrypt, reliable: true, forwardAsWebhook);
		}

		internal bool SendPrivateMessageUnreliable(string target, object message, bool encrypt, bool forwardAsWebhook = false)
		{
			return sendPrivateMessage(target, message, encrypt, reliable: false, forwardAsWebhook);
		}

		private bool sendPrivateMessage(string target, object message, bool encrypt, bool reliable, bool forwardAsWebhook = false)
		{
			if (!CanChat)
			{
				if ((int)DebugOut >= 1)
				{
					listener.DebugReturn(DebugLevel.ERROR, "SendPrivateMessage called while not connected to front end server.");
				}
				return false;
			}
			if (string.IsNullOrEmpty(target) || message == null)
			{
				if ((int)DebugOut >= 2)
				{
					listener.DebugReturn(DebugLevel.WARNING, "SendPrivateMessage parameters must be non-null and not empty.");
				}
				return false;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>
			{
				{ 225, target },
				{ 3, message }
			};
			if (forwardAsWebhook)
			{
				dictionary.Add(21, (byte)1);
			}
			return chatPeer.OpCustom(3, dictionary, reliable, 0, encrypt);
		}

		private bool SetOnlineStatus(int status, object message, bool skipMessage)
		{
			if (!CanChat)
			{
				if ((int)DebugOut >= 1)
				{
					listener.DebugReturn(DebugLevel.ERROR, "SetOnlineStatus called while not connected to front end server.");
				}
				return false;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object> { { 10, status } };
			if (skipMessage)
			{
				dictionary[12] = true;
			}
			else
			{
				dictionary[3] = message;
			}
			return chatPeer.OpCustom(5, dictionary, sendReliable: true);
		}

		public bool SetOnlineStatus(int status)
		{
			return SetOnlineStatus(status, null, skipMessage: true);
		}

		public bool SetOnlineStatus(int status, object message)
		{
			return SetOnlineStatus(status, message, skipMessage: false);
		}

		public bool AddFriends(string[] friends)
		{
			if (!CanChat)
			{
				if ((int)DebugOut >= 1)
				{
					listener.DebugReturn(DebugLevel.ERROR, "AddFriends called while not connected to front end server.");
				}
				return false;
			}
			if (friends == null || friends.Length == 0)
			{
				if ((int)DebugOut >= 2)
				{
					listener.DebugReturn(DebugLevel.WARNING, "AddFriends can't be called for empty or null list.");
				}
				return false;
			}
			if (friends.Length > 1024)
			{
				if ((int)DebugOut >= 2)
				{
					listener.DebugReturn(DebugLevel.WARNING, "AddFriends max list size exceeded: " + friends.Length + " > " + 1024);
				}
				return false;
			}
			Dictionary<byte, object> customOpParameters = new Dictionary<byte, object> { { 11, friends } };
			return chatPeer.OpCustom(6, customOpParameters, sendReliable: true);
		}

		public bool RemoveFriends(string[] friends)
		{
			if (!CanChat)
			{
				if ((int)DebugOut >= 1)
				{
					listener.DebugReturn(DebugLevel.ERROR, "RemoveFriends called while not connected to front end server.");
				}
				return false;
			}
			if (friends == null || friends.Length == 0)
			{
				if ((int)DebugOut >= 2)
				{
					listener.DebugReturn(DebugLevel.WARNING, "RemoveFriends can't be called for empty or null list.");
				}
				return false;
			}
			if (friends.Length > 1024)
			{
				if ((int)DebugOut >= 2)
				{
					listener.DebugReturn(DebugLevel.WARNING, "RemoveFriends max list size exceeded: " + friends.Length + " > " + 1024);
				}
				return false;
			}
			Dictionary<byte, object> customOpParameters = new Dictionary<byte, object> { { 11, friends } };
			return chatPeer.OpCustom(7, customOpParameters, sendReliable: true);
		}

		public string GetPrivateChannelNameByUser(string userName)
		{
			return $"{UserId}:{userName}";
		}

		public bool TryGetChannel(string channelName, bool isPrivate, out ChatChannel channel)
		{
			if (!isPrivate)
			{
				return PublicChannels.TryGetValue(channelName, out channel);
			}
			return PrivateChannels.TryGetValue(channelName, out channel);
		}

		public bool TryGetChannel(string channelName, out ChatChannel channel)
		{
			if (PublicChannels.TryGetValue(channelName, out channel))
			{
				return true;
			}
			return PrivateChannels.TryGetValue(channelName, out channel);
		}

		void IPhotonPeerListener.DebugReturn(DebugLevel level, string message)
		{
			listener.DebugReturn(level, message);
		}

		void IPhotonPeerListener.OnEvent(EventData eventData)
		{
			switch (eventData.Code)
			{
			case 0:
				HandleChatMessagesEvent(eventData);
				break;
			case 2:
				HandlePrivateMessageEvent(eventData);
				break;
			case 4:
				HandleStatusUpdate(eventData);
				break;
			case 5:
				HandleSubscribeEvent(eventData);
				break;
			case 6:
				HandleUnsubscribeEvent(eventData);
				break;
			case 1:
			case 3:
				break;
			}
		}

		void IPhotonPeerListener.OnOperationResponse(OperationResponse operationResponse)
		{
			byte operationCode = operationResponse.OperationCode;
			if (operationCode == 230)
			{
				HandleAuthResponse(operationResponse);
			}
			else if (operationResponse.ReturnCode != 0 && (int)DebugOut >= 1)
			{
				if (operationResponse.ReturnCode == -2)
				{
					listener.DebugReturn(DebugLevel.ERROR, $"Chat Operation {operationResponse.OperationCode} unknown on server. Check your AppId and make sure it's for a Chat application.");
				}
				else
				{
					listener.DebugReturn(DebugLevel.ERROR, $"Chat Operation {operationResponse.OperationCode} failed (Code: {operationResponse.ReturnCode}). Debug Message: {operationResponse.DebugMessage}");
				}
			}
		}

		void IPhotonPeerListener.OnStatusChanged(StatusCode statusCode)
		{
			switch (statusCode)
			{
			case StatusCode.Connect:
				if (!chatPeer.IsProtocolSecure)
				{
					chatPeer.EstablishEncryption();
				}
				else if (!didAuthenticate)
				{
					didAuthenticate = chatPeer.AuthenticateOnNameServer(AppId, AppVersion, chatRegion, AuthValues);
					if (!didAuthenticate && (int)DebugOut >= 1)
					{
						((IPhotonPeerListener)this).DebugReturn(DebugLevel.ERROR, "Error calling OpAuthenticate! Did not work. Check log output, AuthValues and if you're connected. State: " + State);
					}
				}
				if (State == ChatState.ConnectingToNameServer)
				{
					State = ChatState.ConnectedToNameServer;
					listener.OnChatStateChange(State);
				}
				else if (State == ChatState.ConnectingToFrontEnd)
				{
					AuthenticateOnFrontEnd();
				}
				break;
			case StatusCode.EncryptionEstablished:
				if (!didAuthenticate)
				{
					didAuthenticate = chatPeer.AuthenticateOnNameServer(AppId, AppVersion, chatRegion, AuthValues);
					if (!didAuthenticate && (int)DebugOut >= 1)
					{
						((IPhotonPeerListener)this).DebugReturn(DebugLevel.ERROR, "Error calling OpAuthenticate! Did not work. Check log output, AuthValues and if you're connected. State: " + State);
					}
				}
				break;
			case StatusCode.EncryptionFailedToEstablish:
				State = ChatState.Disconnecting;
				chatPeer.Disconnect();
				break;
			case StatusCode.Disconnect:
				if (State == ChatState.Authenticated)
				{
					ConnectToFrontEnd();
					break;
				}
				State = ChatState.Disconnected;
				listener.OnChatStateChange(ChatState.Disconnected);
				listener.OnDisconnected();
				break;
			}
		}

		private bool SendChannelOperation(string[] channels, byte operation, int historyLength)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object> { { 0, channels } };
			if (historyLength != 0)
			{
				dictionary.Add(14, historyLength);
			}
			return chatPeer.OpCustom(operation, dictionary, sendReliable: true);
		}

		private void HandlePrivateMessageEvent(EventData eventData)
		{
			object message = eventData.Parameters[3];
			string text = (string)eventData.Parameters[5];
			string privateChannelNameByUser;
			if (UserId != null && UserId.Equals(text))
			{
				string userName = (string)eventData.Parameters[225];
				privateChannelNameByUser = GetPrivateChannelNameByUser(userName);
			}
			else
			{
				privateChannelNameByUser = GetPrivateChannelNameByUser(text);
			}
			if (!PrivateChannels.TryGetValue(privateChannelNameByUser, out var value))
			{
				value = new ChatChannel(privateChannelNameByUser);
				value.IsPrivate = true;
				value.MessageLimit = MessageLimit;
				PrivateChannels.Add(value.Name, value);
			}
			value.Add(text, message);
			listener.OnPrivateMessage(text, message, privateChannelNameByUser);
		}

		private void HandleChatMessagesEvent(EventData eventData)
		{
			object[] messages = (object[])eventData.Parameters[2];
			string[] senders = (string[])eventData.Parameters[4];
			string text = (string)eventData.Parameters[1];
			if (!PublicChannels.TryGetValue(text, out var value))
			{
				if ((int)DebugOut >= 2)
				{
					listener.DebugReturn(DebugLevel.WARNING, "Channel " + text + " for incoming message event not found.");
				}
			}
			else
			{
				value.Add(senders, messages);
				listener.OnGetMessages(text, senders, messages);
			}
		}

		private void HandleSubscribeEvent(EventData eventData)
		{
			string[] array = (string[])eventData.Parameters[0];
			bool[] array2 = (bool[])eventData.Parameters[15];
			for (int i = 0; i < array.Length; i++)
			{
				if (array2[i])
				{
					string text = array[i];
					if (!PublicChannels.ContainsKey(text))
					{
						ChatChannel chatChannel = new ChatChannel(text);
						chatChannel.MessageLimit = MessageLimit;
						PublicChannels.Add(chatChannel.Name, chatChannel);
					}
				}
			}
			listener.OnSubscribed(array, array2);
		}

		private void HandleUnsubscribeEvent(EventData eventData)
		{
			string[] array = (string[])eventData[0];
			string[] array2 = array;
			foreach (string text in array2)
			{
				PublicChannels.Remove(text);
				PublicChannelsUnsubscribing.Remove(text);
			}
			listener.OnUnsubscribed(array);
		}

		private void HandleAuthResponse(OperationResponse operationResponse)
		{
			if ((int)DebugOut >= 3)
			{
				listener.DebugReturn(DebugLevel.INFO, operationResponse.ToStringFull() + " on: " + chatPeer.NameServerAddress);
			}
			if (operationResponse.ReturnCode == 0)
			{
				if (State == ChatState.ConnectedToNameServer)
				{
					State = ChatState.Authenticated;
					listener.OnChatStateChange(State);
					if (operationResponse.Parameters.ContainsKey(221))
					{
						if (AuthValues == null)
						{
							AuthValues = new AuthenticationValues();
						}
						AuthValues.Token = operationResponse[221] as string;
						FrontendAddress = (string)operationResponse[230];
						chatPeer.Disconnect();
					}
					else if ((int)DebugOut >= 1)
					{
						listener.DebugReturn(DebugLevel.ERROR, "No secret in authentication response.");
					}
				}
				else if (State == ChatState.ConnectingToFrontEnd)
				{
					State = ChatState.ConnectedToFrontEnd;
					listener.OnChatStateChange(State);
					listener.OnConnected();
				}
				return;
			}
			switch (operationResponse.ReturnCode)
			{
			case short.MaxValue:
				DisconnectedCause = ChatDisconnectCause.InvalidAuthentication;
				break;
			case 32755:
				DisconnectedCause = ChatDisconnectCause.CustomAuthenticationFailed;
				break;
			case 32756:
				DisconnectedCause = ChatDisconnectCause.InvalidRegion;
				break;
			case 32757:
				DisconnectedCause = ChatDisconnectCause.MaxCcuReached;
				break;
			case -3:
				DisconnectedCause = ChatDisconnectCause.OperationNotAllowedInCurrentState;
				break;
			}
			if ((int)DebugOut >= 1)
			{
				listener.DebugReturn(DebugLevel.ERROR, "Authentication request error: " + operationResponse.ReturnCode + ". Disconnecting.");
			}
			State = ChatState.Disconnecting;
			chatPeer.Disconnect();
		}

		private void HandleStatusUpdate(EventData eventData)
		{
			string user = (string)eventData.Parameters[5];
			int status = (int)eventData.Parameters[10];
			object message = null;
			bool flag = eventData.Parameters.ContainsKey(3);
			if (flag)
			{
				message = eventData.Parameters[3];
			}
			listener.OnStatusUpdate(user, status, flag, message);
		}

		private void ConnectToFrontEnd()
		{
			State = ChatState.ConnectingToFrontEnd;
			if ((int)DebugOut >= 3)
			{
				listener.DebugReturn(DebugLevel.INFO, "Connecting to frontend " + FrontendAddress);
			}
			chatPeer.Connect(FrontendAddress, "chat");
		}

		private bool AuthenticateOnFrontEnd()
		{
			if (AuthValues != null)
			{
				if (AuthValues.Token == null || AuthValues.Token == string.Empty)
				{
					if ((int)DebugOut >= 1)
					{
						listener.DebugReturn(DebugLevel.ERROR, "Can't authenticate on front end server. Secret is not set");
					}
					return false;
				}
				Dictionary<byte, object> customOpParameters = new Dictionary<byte, object> { { 221, AuthValues.Token } };
				return chatPeer.OpCustom(230, customOpParameters, sendReliable: true);
			}
			if ((int)DebugOut >= 1)
			{
				listener.DebugReturn(DebugLevel.ERROR, "Can't authenticate on front end server. Authentication Values are not set");
			}
			return false;
		}
	}
}
