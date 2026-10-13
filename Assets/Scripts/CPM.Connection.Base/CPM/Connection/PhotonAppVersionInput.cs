namespace CPM.Connection
{
	public struct PhotonAppVersionInput
	{
		public int PhotonGameVersion;

		public PhotonConnectionProfile Profile;

		public int LanguageIndex;

		public bool CorrectHash;

		public bool HasPlayerId;

		public bool Banned;

		public bool DevServer;
	}
}
