using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Scopes
{
	internal static class PersistentConfigPreloader
	{
		private static Dictionary<string, ResourceRequest> _requests;

		private static int _ready;

		private static int _stalled;

		private static int _missed;

		public static T Get<T>(string path) where T : Object
		{
			if (_requests != null && _requests.TryGetValue(path, out var value))
			{
				_requests.Remove(path);
				bool isDone = value.isDone;
				if (value.asset is T result)
				{
					if (isDone)
					{
						_ready++;
					}
					else
					{
						_stalled++;
					}
					return result;
				}
			}
			_missed++;
			return Resources.Load<T>(path);
		}

		public static T GetComponent<T>(string path) where T : Component
		{
			if (_requests != null && _requests.TryGetValue(path, out var value))
			{
				_requests.Remove(path);
				bool isDone = value.isDone;
				if (value.asset is GameObject gameObject && gameObject.TryGetComponent<T>(out var component))
				{
					if (isDone)
					{
						_ready++;
					}
					else
					{
						_stalled++;
					}
					return component;
				}
			}
			_missed++;
			return Resources.Load<T>(path);
		}

		public static void ReportAndClear()
		{
			_ = _requests?.Count;
			_requests = null;
		}
	}
}
