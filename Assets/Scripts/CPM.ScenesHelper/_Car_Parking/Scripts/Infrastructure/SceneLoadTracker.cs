namespace _Car_Parking.Scripts.Infrastructure
{
	public static class SceneLoadTracker
	{
		private static int _pending;

		public static bool HasPendingLoads => _pending > 0;

		public static void Begin()
		{
			_pending++;
		}

		public static void End()
		{
			if (_pending > 0)
			{
				_pending--;
			}
		}
	}
}
