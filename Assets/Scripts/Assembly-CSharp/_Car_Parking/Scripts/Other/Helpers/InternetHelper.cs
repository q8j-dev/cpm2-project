using UnityEngine;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public class InternetHelper
	{
		public static bool HasInternetConnection()
		{
			return Application.internetReachability != NetworkReachability.NotReachable;
		}
	}
}
