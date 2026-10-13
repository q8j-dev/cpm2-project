using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Animations.Emotes
{
	public readonly struct EmoteWheelContext
	{
		public readonly GameObject Driver;

		public readonly int Gender;

		public readonly Func<int> GetOldSelected;

		public readonly Action<int> SetOldSelected;

		public readonly Action CancelReset;

		public readonly Action<float> ScheduleReset;

		public EmoteWheelContext(GameObject driver, int gender, Func<int> getOldSelected, Action<int> setOldSelected, Action cancelReset, Action<float> scheduleReset)
		{
			Driver = driver;
			Gender = gender;
			GetOldSelected = getOldSelected;
			SetOldSelected = setOldSelected;
			CancelReset = cancelReset;
			ScheduleReset = scheduleReset;
		}
	}
}
