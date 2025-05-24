#define UNITY
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ExitGames.Client.Photon.Chat
{
	public class ChatPeer : PhotonPeer
	{
		public const string NameServerHost = "ns.exitgames.com";

		public const string NameServerHttp = "http://ns.exitgamescloud.com:80/photon/n";

		private static readonly Dictionary<ConnectionProtocol, int> ProtocolToNameServerPort = new Dictionary<ConnectionProtocol, int>
		{
			{
				ConnectionProtocol.Udp,
				5058
			},
			{
				ConnectionProtocol.Tcp,
				4533
			},
			{
				ConnectionProtocol.WebSocket,
				9093
			},
			{
				ConnectionProtocol.WebSocketSecure,
				19093
			}
		};

		public string NameServerAddress => GetNameServerAddress();

		internal virtual bool IsProtocolSecure => base.UsedProtocol == ConnectionProtocol.WebSocketSecure;

		public ChatPeer(IPhotonPeerListener listener, ConnectionProtocol protocol)
			: base(listener, protocol)
		{
			ConfigUnitySockets();
		}

		[Conditional("UNITY")]
		private void ConfigUnitySockets()
		{
			Type type = Type.GetType("ExitGames.Client.Photon.SocketWebTcpThread, Assembly-CSharp", throwOnError: false);
			type = type ?? Type.GetType("ExitGames.Client.Photon.SocketWebTcpThread, Assembly-CSharp-firstpass", throwOnError: false);
			type = type ?? Type.GetType("ExitGames.Client.Photon.SocketWebTcpCoroutine, Assembly-CSharp", throwOnError: false);
			type = type ?? Type.GetType("ExitGames.Client.Photon.SocketWebTcpCoroutine, Assembly-CSharp-firstpass", throwOnError: false);
			if (type != null)
			{
				SocketImplementationConfig[ConnectionProtocol.WebSocket] = type;
				SocketImplementationConfig[ConnectionProtocol.WebSocketSecure] = type;
			}
		}

		private string GetNameServerAddress()
		{
			int value = 0;
			ProtocolToNameServerPort.TryGetValue(base.TransportProtocol, out value);
			switch (base.TransportProtocol)
			{
			case ConnectionProtocol.Udp:
			case ConnectionProtocol.Tcp:
				return string.Format("{0}:{1}", "ns.exitgames.com", value);
			case ConnectionProtocol.WebSocket:
				return string.Format("ws://{0}:{1}", "ns.exitgames.com", value);
			case ConnectionProtocol.WebSocketSecure:
				return string.Format("wss://{0}:{1}", "ns.exitgames.com", value);
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public bool Connect()
		{
			if ((int)DebugOut >= 3)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "Connecting to nameserver " + NameServerAddress);
			}
			return Connect(NameServerAddress, "NameServer");
		}

		public bool AuthenticateOnNameServer(string appId, string appVersion, string region, AuthenticationValues authValues)
		{
			if ((int)DebugOut >= 3)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpAuthenticate()");
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[220] = appVersion;
			dictionary[224] = appId;
			dictionary[210] = region;
			if (authValues != null)
			{
				if (!string.IsNullOrEmpty(authValues.UserId))
				{
					dictionary[225] = authValues.UserId;
				}
				if (authValues != null && authValues.AuthType != CustomAuthenticationType.None)
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
			return OpCustom(230, dictionary, sendReliable: true, 0, base.IsEncryptionAvailable);
		}
	}
}
