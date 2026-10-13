using UnityEngine;

namespace _Car_Parking.Scripts
{
	public static class ApplicationCpm
	{
		public static string Platform
		{
			get
			{
				if (Application.platform != RuntimePlatform.Android)
				{
					return "ios";
				}
				return "android";
			}
		}
	}
}
