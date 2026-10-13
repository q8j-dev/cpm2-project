namespace CPM.Connection
{
	public static class PhotonAppVersion
	{
		public const string ProjectSuffix = "_d";

		public const string ArenaToken = "arena";

		public const string DevVersion = "999_d_DEV";

		private const string QuarantineToken = "AA";

		private const string BannedToken = "BB";

		public static string Build(in PhotonAppVersionInput input)
		{
			if (input.DevServer)
			{
				return "999_d_DEV";
			}
			int photonGameVersion = input.PhotonGameVersion;
			string text = photonGameVersion + "_d";
			if (input.Banned)
			{
				return text + "BB";
			}
			object obj;
			if (input.Profile != PhotonConnectionProfile.Arena)
			{
				photonGameVersion = input.LanguageIndex;
				obj = photonGameVersion.ToString();
			}
			else
			{
				obj = "arena";
			}
			string text2 = (string)obj;
			if (input.CorrectHash && input.HasPlayerId)
			{
				return text + "_" + text2;
			}
			return text + "AA_" + text2;
		}
	}
}
