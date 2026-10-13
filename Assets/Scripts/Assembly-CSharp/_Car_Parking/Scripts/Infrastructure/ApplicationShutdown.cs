using UnityEngine;

namespace _Car_Parking.Scripts.Infrastructure
{
	public static class ApplicationShutdown
	{
		public static bool IsQuitting { get; private set; }

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Reset()
		{
			IsQuitting = false;
			Application.quitting -= OnQuitting;
			Application.quitting += OnQuitting;
		}

		private static void OnQuitting()
		{
			IsQuitting = true;
		}
	}
}
