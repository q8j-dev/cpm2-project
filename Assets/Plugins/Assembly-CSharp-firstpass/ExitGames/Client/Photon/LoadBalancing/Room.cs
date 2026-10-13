using System.Collections.Generic;

namespace ExitGames.Client.Photon.LoadBalancing
{
	public class Room : RoomInfo
	{
		protected internal int PlayerTTL;

		protected internal int RoomTTL;

		private Dictionary<int, Player> players = new Dictionary<int, Player>();

		protected internal LoadBalancingClient LoadBalancingClient { get; set; }

		public new string Name
		{
			get
			{
				return name;
			}
			internal set
			{
				name = value;
			}
		}

		public new bool IsOpen
		{
			get
			{
				return isOpen;
			}
			set
			{
				if (!base.IsLocalClientInside)
				{
					LoadBalancingClient.DebugReturn(DebugLevel.WARNING, "Can't set room properties when not in that room.");
				}
				if (value != isOpen)
				{
					LoadBalancingClient.OpSetPropertiesOfRoom(new Hashtable { { 253, value } });
				}
				isOpen = value;
			}
		}

		public new bool IsVisible
		{
			get
			{
				return isVisible;
			}
			set
			{
				if (!base.IsLocalClientInside)
				{
					LoadBalancingClient.DebugReturn(DebugLevel.WARNING, "Can't set room properties when not in that room.");
				}
				if (value != isVisible)
				{
					LoadBalancingClient.OpSetPropertiesOfRoom(new Hashtable { { 254, value } });
				}
				isVisible = value;
			}
		}

		public new byte MaxPlayers
		{
			get
			{
				return maxPlayers;
			}
			set
			{
				if (!base.IsLocalClientInside)
				{
					LoadBalancingClient.DebugReturn(DebugLevel.WARNING, "Can't set room properties when not in that room.");
				}
				if (value != maxPlayers)
				{
					LoadBalancingClient.OpSetPropertiesOfRoom(new Hashtable { 
					{
						byte.MaxValue,
						value
					} });
				}
				maxPlayers = value;
			}
		}

		public new byte PlayerCount
		{
			get
			{
				if (Players == null)
				{
					return 0;
				}
				return (byte)Players.Count;
			}
		}

		public Dictionary<int, Player> Players
		{
			get
			{
				return players;
			}
			private set
			{
				players = value;
			}
		}

		public string[] ExpectedUsers => expectedUsers;

		public int MasterClientId => masterClientId;

		public string[] PropertiesListedInLobby
		{
			get
			{
				return propertiesListedInLobby;
			}
			private set
			{
				propertiesListedInLobby = value;
			}
		}

		public bool AutoCleanUp => autoCleanUp;

		protected internal Room(string roomName, RoomOptions options)
			: base(roomName, options?.CustomRoomProperties)
		{
			if (options != null)
			{
				isVisible = options.IsVisible;
				isOpen = options.IsOpen;
				maxPlayers = options.MaxPlayers;
				propertiesListedInLobby = options.CustomRoomPropertiesForLobby;
				PlayerTTL = options.PlayerTtl;
				RoomTTL = options.EmptyRoomTtl;
			}
		}

		public virtual void SetCustomProperties(Hashtable propertiesToSet, Hashtable expectedProperties = null, WebFlags webFlags = null)
		{
			Hashtable hashtable = propertiesToSet.StripToStringKeys();
			if (expectedProperties == null || expectedProperties.Count == 0)
			{
				base.CustomProperties.Merge(hashtable);
				base.CustomProperties.StripKeysWithNullValues();
			}
			if (base.IsLocalClientInside)
			{
				LoadBalancingClient.loadBalancingPeer.OpSetPropertiesOfRoom(hashtable, expectedProperties, webFlags);
			}
		}

		public void SetPropertiesListedInLobby(string[] propertiesListedInLobby)
		{
			Hashtable hashtable = new Hashtable();
			hashtable[250] = propertiesListedInLobby;
			if (LoadBalancingClient.OpSetPropertiesOfRoom(hashtable))
			{
				base.propertiesListedInLobby = propertiesListedInLobby;
			}
		}

		protected internal virtual void RemovePlayer(Player player)
		{
			Players.Remove(player.ID);
			player.RoomReference = null;
		}

		protected internal virtual void RemovePlayer(int id)
		{
			RemovePlayer(GetPlayer(id));
		}

		public bool SetMasterClient(Player masterClientPlayer)
		{
			if (!base.IsLocalClientInside)
			{
				LoadBalancingClient.DebugReturn(DebugLevel.WARNING, "SetMasterClient can only be called for the current room (being in one).");
				return false;
			}
			Hashtable gameProperties = new Hashtable { { 248, masterClientPlayer.ID } };
			Hashtable expectedProperties = new Hashtable { { 248, MasterClientId } };
			return LoadBalancingClient.OpSetPropertiesOfRoom(gameProperties, expectedProperties);
		}

		public virtual bool AddPlayer(Player player)
		{
			if (!Players.ContainsKey(player.ID))
			{
				StorePlayer(player);
				return true;
			}
			return false;
		}

		public virtual Player StorePlayer(Player player)
		{
			Players[player.ID] = player;
			player.RoomReference = this;
			if (MasterClientId == 0 || player.ID < MasterClientId)
			{
				masterClientId = player.ID;
			}
			return player;
		}

		public virtual Player GetPlayer(int id)
		{
			Player value = null;
			Players.TryGetValue(id, out value);
			return value;
		}

		public void ClearExpectedUsers()
		{
			Hashtable hashtable = new Hashtable();
			hashtable[247] = new string[0];
			Hashtable hashtable2 = new Hashtable();
			hashtable2[247] = ExpectedUsers;
			LoadBalancingClient.OpSetPropertiesOfRoom(hashtable, hashtable2);
		}

		public override string ToString()
		{
			return string.Format("Room: '{0}' {1},{2} {4}/{3} players.", name, isVisible ? "visible" : "hidden", isOpen ? "open" : "closed", maxPlayers, PlayerCount);
		}

		public new string ToStringFull()
		{
			return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", name, isVisible ? "visible" : "hidden", isOpen ? "open" : "closed", maxPlayers, PlayerCount, base.CustomProperties.ToStringFull());
		}
	}
}
