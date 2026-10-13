using System;
using UnityEngine;

namespace _Car_Parking.Scripts.DataStorageService
{
	public static class PlayerStorageServiceExtension
	{
		private const string GuestPrefix = "Guest#";

		public static string GetRandomPlayerName()
		{
			return $"Player#{UnityEngine.Random.Range(0, 1000)}";
		}

		public static string GetRandomGuestName()
		{
			return string.Format("{0}{1}", "Guest#", UnityEngine.Random.Range(0, 1000));
		}

		public static bool IsRandomGuestName(string name)
		{
			if (!string.IsNullOrEmpty(name))
			{
				return name.IndexOf("Guest#", StringComparison.Ordinal) == 0;
			}
			return false;
		}
	}
}
