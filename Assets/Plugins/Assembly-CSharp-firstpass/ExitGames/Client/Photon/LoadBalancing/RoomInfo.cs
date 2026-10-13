namespace ExitGames.Client.Photon.LoadBalancing
{
	public class RoomInfo
	{
		protected internal bool removedFromList;

		private Hashtable customProperties = new Hashtable();

		protected byte maxPlayers;

		protected string[] expectedUsers;

		protected bool isOpen = true;

		protected bool isVisible = true;

		protected bool autoCleanUp = true;

		protected string name;

		protected internal int masterClientId;

		protected string[] propertiesListedInLobby;

		public Hashtable CustomProperties => customProperties;

		public string Name => name;

		public int PlayerCount { get; private set; }

		public bool IsLocalClientInside { get; set; }

		public byte MaxPlayers => maxPlayers;

		public bool IsOpen => isOpen;

		public bool IsVisible => isVisible;

		protected internal RoomInfo(string roomName, Hashtable roomProperties)
		{
			InternalCacheProperties(roomProperties);
			name = roomName;
		}

		public override bool Equals(object other)
		{
			if (other is RoomInfo roomInfo)
			{
				return Name.Equals(roomInfo.name);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return name.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("Room: '{0}' {1},{2} {4}/{3} players.", name, isVisible ? "visible" : "hidden", isOpen ? "open" : "closed", maxPlayers, PlayerCount);
		}

		public string ToStringFull()
		{
			return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", name, isVisible ? "visible" : "hidden", isOpen ? "open" : "closed", maxPlayers, PlayerCount, customProperties.ToStringFull());
		}

		protected internal virtual void InternalCacheProperties(Hashtable propertiesToCache)
		{
			if (propertiesToCache == null || propertiesToCache.Count == 0 || customProperties.Equals(propertiesToCache))
			{
				return;
			}
			if (propertiesToCache.ContainsKey(251))
			{
				removedFromList = (bool)propertiesToCache[251];
				if (removedFromList)
				{
					return;
				}
			}
			if (propertiesToCache.ContainsKey(byte.MaxValue))
			{
				maxPlayers = (byte)propertiesToCache[byte.MaxValue];
			}
			if (propertiesToCache.ContainsKey(253))
			{
				isOpen = (bool)propertiesToCache[253];
			}
			if (propertiesToCache.ContainsKey(254))
			{
				isVisible = (bool)propertiesToCache[254];
			}
			if (propertiesToCache.ContainsKey(252))
			{
				PlayerCount = (byte)propertiesToCache[252];
			}
			if (propertiesToCache.ContainsKey(249))
			{
				autoCleanUp = (bool)propertiesToCache[249];
			}
			if (propertiesToCache.ContainsKey(248))
			{
				masterClientId = (int)propertiesToCache[248];
			}
			if (propertiesToCache.ContainsKey(250))
			{
				propertiesListedInLobby = propertiesToCache[250] as string[];
			}
			if (propertiesToCache.ContainsKey(247))
			{
				expectedUsers = (string[])propertiesToCache[247];
			}
			customProperties.MergeStringKeys(propertiesToCache);
			customProperties.StripKeysWithNullValues();
		}
	}
}
