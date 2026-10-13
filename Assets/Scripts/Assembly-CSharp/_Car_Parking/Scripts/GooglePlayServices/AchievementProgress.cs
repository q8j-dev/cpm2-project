namespace _Car_Parking.Scripts.GooglePlayServices
{
	public static class AchievementProgress
	{
		public const string FreeDriveKmKey = "gpg_ach_free_drive_km";

		public const float FreeDriveKmTarget = 25f;

		public const string SpeedBannersKey = "gpg_ach_speed_banners";

		public const int SpeedBannersTarget = 8;

		public const string OffroadKmKey = "gpg_ach_offroad_km";

		public const float OffroadKmTarget = 5f;

		public const string TaxiCompletedKey = "gpg_ach_taxi_completed";

		public const int TaxiCompletedTarget = 10;

		public const string DynoPullsKey = "gpg_ach_dyno_pulls";

		public const int DynoPullsTarget = 10;

		public const string CarsAcquiredKey = "gpg_ach_cars_acquired";

		public const int CarsAcquiredTarget = 2;

		public const string PassengerKmKey = "gpg_ach_passenger_km";

		public const float PassengerKmTarget = 10f;

		public const string VisitedEnvironmentsMaskKey = "gpg_postcard_env_mask";

		public const int VisitedEnvironmentsTarget = 5;

		public const string AcquiredClassesMaskKey = "gpg_garage_classes_mask";

		public const int AcquiredClassesTarget = 5;

		public const int PaintMaterialsTarget = 3;

		public static int EffectiveTaxiCompletedTarget => 10;

		public static float EffectiveFreeDriveKmTarget => 25f;

		public static float EffectiveOffroadKmTarget => 5f;

		public static float EffectivePassengerKmTarget => 10f;

		public static int EffectiveVisitedEnvironmentsTarget => 5;
	}
}
