using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public class ReleaseDebug
	{
		public static void LogError(string s)
		{
			Debug.LogError(s);
		}

		public static void LogError(Exception e)
		{
			Debug.LogError(e);
		}
	}
}
