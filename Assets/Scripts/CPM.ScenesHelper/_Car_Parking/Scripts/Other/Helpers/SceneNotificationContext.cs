namespace _Car_Parking.Scripts.Other.Helpers
{
	public static class SceneNotificationContext
	{
		public static SceneNotificationReason Reason { get; set; }

		public static void Reset()
		{
			Reason = SceneNotificationReason.AppUpdate;
		}
	}
}
