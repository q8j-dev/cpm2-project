using System;
using System.Text;
using UnityEngine;

namespace Persistence.Systems
{
	public static class NativeSalt
	{
		private static string _save;

		private static string _timer;

		private static bool _unavailable;

		public static string ForSaveFiles => _save ?? (_save = Fetch(saveFiles: true));

		public static string ForTuningTimers => _timer ?? (_timer = Fetch(saveFiles: false));

		private static string Fetch(bool saveFiles)
		{
			if (_unavailable)
			{
				return null;
			}
			try
			{
				return Managed(saveFiles);
			}
			catch (Exception ex)
			{
				_unavailable = true;
				Debug.LogWarning("[NativeSalt] unavailable, integrity checks are off: " + ex.Message);
				return null;
			}
		}

		private static string Managed(bool saveFiles)
		{
			byte[] array = ((!saveFiles) ? new byte[11]
			{
				116, 109, 114, 46, 118, 49, 46, 57, 102, 51,
				99
			} : new byte[11]
			{
				115, 97, 118, 46, 118, 49, 46, 52, 98, 55,
				101
			});
			StringBuilder stringBuilder = new StringBuilder(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append((char)array[i]);
			}
			return stringBuilder.ToString();
		}
	}
}
