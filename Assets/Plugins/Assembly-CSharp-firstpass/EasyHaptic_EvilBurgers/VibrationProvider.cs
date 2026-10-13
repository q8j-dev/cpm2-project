using System;
using UniRx;

namespace EasyHaptic_EvilBurgers
{
	public class VibrationProvider
	{
		private const float CanVibrateDelay = 0.3f;

		private const float CanVibrateDelayUnscaled = 0.1f;

		private static bool BlockVibrate;

		public static void TryPlay(CustomVibrationData customData)
		{
			if (!BlockVibrate)
			{
				EasyHaptic.PlayCustom(customData);
				BlockVibrate = true;
				Observable.Timer(TimeSpan.FromSeconds(0.30000001192092896)).Subscribe((long _) =>
				{
					BlockVibrate = false;
				});
			}
		}

		public static void TryPlayUnscaled(CustomVibrationData customData)
		{
			if (!BlockVibrate)
			{
				EasyHaptic.PlayCustom(customData);
				BlockVibrate = true;
				Observable.Timer(TimeSpan.FromSeconds(0.10000000149011612), Scheduler.MainThreadIgnoreTimeScale).Subscribe((long _) =>
				{
					BlockVibrate = false;
				});
			}
		}
	}
}
