using System;
using System.Diagnostics;
using UnityEngine;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public static class WDebug
	{
		public const bool ReleaseLogsOnly = false;

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEVELOPMENT_BUILD")]
		public static void Log(string s)
		{
			UnityEngine.Debug.Log(s.Colored(Color.gray));
		}

		[Conditional("UNITY_EDITOR")]
		public static void LogEditorOnly(string s)
		{
			UnityEngine.Debug.Log(("EDITOR ONLY LOG: " + s).Colored(Color.gray));
		}

		[Conditional("UNITY_EDITOR")]
		public static void LogEditorOnly(string s, GameObject gameObject)
		{
			UnityEngine.Debug.Log(("EDITOR ONLY LOG: " + s).Colored(Color.gray));
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEVELOPMENT_BUILD")]
		public static void Log(string s, Color color, UnityEngine.Object context = null)
		{
			UnityEngine.Debug.Log(s.Colored(color) ?? "", context);
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEVELOPMENT_BUILD")]
		public static void LogError(string s, bool dev = false)
		{
			UnityEngine.Debug.LogError((dev ? "[DEV] " : "") + s);
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEVELOPMENT_BUILD")]
		public static void LogWarning(string s)
		{
			UnityEngine.Debug.LogWarning(s.Colored(Color.gray));
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		public static void LogWarning(Exception e)
		{
			UnityEngine.Debug.LogWarning(e.ToString().Colored(Color.gray));
		}

		[Conditional("UNITY_EDITOR")]
		public static void LogWarningEditorOnly(string s, GameObject gameObject = null)
		{
			UnityEngine.Debug.LogWarning(("EDITOR ONLY LOG: " + s).Colored(Color.gray), gameObject);
		}

		private static string Colored(this string s, Color color)
		{
			return s;
		}
	}
}
