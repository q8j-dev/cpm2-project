namespace _Car_Parking.Scripts.Backend
{
	public static class BackendEndpoints
	{
		public static class Progress
		{
			public const string Set = "progress-service/v1/rating/update";

			public const string Get = "progress-service/v1/rating";

			public const string Daily = "progress-service/v1/rating/daily";
		}

		public static class Player
		{
			public const string Profile = "progress-service/v1/player/profile";

			public const string Favourites = "progress-service/v1/player/favourites";
		}
	}
}
