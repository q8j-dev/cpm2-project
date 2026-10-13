namespace CPM.Connection
{
	public class MultiplayerSettings
	{
		public const int MaxPlayersLimit = 32;

		public const int serializationRateMin = 1;

		public const int serializationRateRare = 3;

		public const int serializationRateNormal = 5;

		public const int serializationRateOften = 6;

		public const int serializationRateMax = 8;

		public static bool UsePersonSnapshotInterpolation;
	}
}
