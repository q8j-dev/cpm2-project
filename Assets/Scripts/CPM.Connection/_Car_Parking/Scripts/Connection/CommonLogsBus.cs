using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace _Car_Parking.Scripts.Connection
{
	public static class CommonLogsBus
	{
		private static List<string> _logs;

		private static List<string> _logsError;

		private static IDisposable _disposable;

		static CommonLogsBus()
		{
			_logs = new List<string>();
			_logsError = new List<string>();
			_disposable?.Dispose();
			_disposable = Observable.EveryUpdate().Subscribe((long _) =>
			{
				TryPrintLogNextFrame();
			});
		}

		public static void Print(string log)
		{
			_logs.Add(log);
		}

		public static void PrintError(string log)
		{
			_logsError.Add(log);
		}

		private static void TryPrintLogNextFrame()
		{
			if (_logs.Count > 0)
			{
				string message = _logs[0];
				_logs.RemoveAt(0);
				Debug.Log(message);
			}
			if (_logsError.Count > 0)
			{
				string message2 = _logsError[0];
				_logsError.RemoveAt(0);
				Debug.LogError(message2);
			}
		}
	}
}
