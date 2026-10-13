using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace _Car_Parking.Scripts.Steam.Perf
{
	public static class SteamPerfNotes
	{
		private static readonly List<(int Frame, string Text)> Pending = new List<(int, string)>();

		public static bool Enabled { get; set; }

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		public static void Add(string text)
		{
			if (Enabled)
			{
				Pending.Add((Time.frameCount, text));
			}
		}

		public static void Drain(int lastFrame, List<string> into)
		{
			for (int i = 0; i < Pending.Count; i++)
			{
				if (Pending[i].Frame <= lastFrame)
				{
					into.Add(Pending[i].Text);
					Pending.RemoveAt(i--);
				}
			}
		}

		public static void Clear()
		{
			Pending.Clear();
		}
	}
}
