namespace CPM.RemoteConfig
{
	public static class RemoteConfigSnapshot
	{
		public static int MaxPlayers { get; private set; }

		public static void Apply(ConfigValuesStorage storage)
		{
			if (storage != null)
			{
				MaxPlayers = storage.maxPlayers;
			}
		}
	}
}
