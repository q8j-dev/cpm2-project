#define SUPPORTED_UNITY
using System;
using System.Collections.Generic;
using System.Diagnostics;
using ExitGames.Client.Photon;

namespace Photon.Chat
{
	public class ChatPeer : PhotonPeer
	{
		public string NameServerHost = "ns.photonengine.io";

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

		public ushort NameServerPortOverride;

		public string NameServerAddress => GetNameServerAddress();

		internal virtual bool IsProtocolSecure => base.UsedProtocol == ConnectionProtocol.WebSocketSecure;

		public ChatPeer(IPhotonPeerListener listener, ConnectionProtocol protocol)
			: base(listener, protocol)
		{
			ConfigUnitySockets();
		}

		[Conditional("SUPPORTED_UNITY")]
		private void ConfigUnitySockets()
		{
			Type type = null;
			type = Type.GetType("ExitGames.Client.Photon.SocketWebTcp, PhotonWebSocket", throwOnError: false);
			if (type == null)
			{
				type = Type.GetType("ExitGames.Client.Photon.SocketWebTcp, Assembly-CSharp-firstpass", throwOnError: false);
			}
			if (type == null)
			{
				type = Type.GetType("ExitGames.Client.Photon.SocketWebTcp, Assembly-CSharp", throwOnError: false);
			}
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
			if (NameServerPortOverride != 0)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, $"Using NameServerPortInAppSettings as port for Name Server: {NameServerPortOverride}");
				value = NameServerPortOverride;
			}
			switch (base.TransportProtocol)
			{
			case ConnectionProtocol.Udp:
			case ConnectionProtocol.Tcp:
				return $"{NameServerHost}:{value}";
			case ConnectionProtocol.WebSocket:
				return $"ws://{NameServerHost}:{value}";
			case ConnectionProtocol.WebSocketSecure:
				return $"wss://{NameServerHost}:{value}";
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
				if (authValues.AuthType != CustomAuthenticationType.None)
				{
					dictionary[217] = (byte)authValues.AuthType;
					if (authValues.Token != null)
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
			return SendOperation(230, dictionary, new SendOptions
			{
				Reliability = true,
				Encrypt = base.IsEncryptionAvailable
			});
		}
	}
}
