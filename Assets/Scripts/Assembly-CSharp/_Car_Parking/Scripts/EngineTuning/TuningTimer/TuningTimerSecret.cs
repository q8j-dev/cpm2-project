using Persistence.Systems;
using UnityEngine;
using _Car_Parking.Scripts.PrefsKeys;

namespace _Car_Parking.Scripts.EngineTuning.TuningTimer
{
	public static class TuningTimerSecret
	{
		private static string _cached;

		private static string _cachedForAccount;

		public static string Current
		{
			get
			{
				string accountId = ManagerKeysForPrefs.AccountId;
				if (_cached != null && _cachedForAccount == accountId)
				{
					return _cached;
				}
				string forTuningTimers = NativeSalt.ForTuningTimers;
				if (forTuningTimers == null)
				{
					return null;
				}
				_cachedForAccount = accountId;
				_cached = SystemInfo.deviceUniqueIdentifier + ":" + accountId + ":" + forTuningTimers;
				return _cached;
			}
		}

		public static bool IsAvailable => Current != null;

		public static void Invalidate()
		{
			_cached = null;
			_cachedForAccount = null;
		}
	}
}
