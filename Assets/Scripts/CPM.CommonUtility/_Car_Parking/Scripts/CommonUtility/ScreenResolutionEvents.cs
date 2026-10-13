using System;

namespace _Car_Parking.Scripts.CommonUtility
{
	public static class ScreenResolutionEvents
	{
		public static event Action OnResolutionApplied;

		public static void RaiseResolutionApplied()
		{
			OnResolutionApplied?.Invoke();
		}
	}
}
