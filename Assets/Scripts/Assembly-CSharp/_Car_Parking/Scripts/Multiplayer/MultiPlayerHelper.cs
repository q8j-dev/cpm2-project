using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer
{
	public static class MultiPlayerHelper
	{
		public static bool IgnoreCarSellRequest
		{
			get
			{
				return PlayerPrefs.GetInt("ignoreCarSellRequest", 0) == 1;
			}
			set
			{
				PlayerPrefs.SetInt("ignoreCarSellRequest", value ? 1 : 0);
			}
		}
	}
}
