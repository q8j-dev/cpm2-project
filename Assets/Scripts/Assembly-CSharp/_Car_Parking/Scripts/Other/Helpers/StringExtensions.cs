using System;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public static class StringExtensions
	{
		public static string[] Split(this string s, string separator, bool a)
		{
			return s.Split(new string[1] { separator }, StringSplitOptions.None);
		}
	}
}
