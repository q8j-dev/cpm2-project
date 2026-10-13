using System;
using System.IO;
using UnityEngine;

namespace _Car_Parking.Scripts.CommonUtility
{
	public static class JsonLogger
	{
		[Serializable]
		public class LogEntry<T>
		{
			public string Timestamp;

			public T Data;
		}

		public static void Send<T>(T data, string logFileName)
		{
			string path = Path.Combine(Application.persistentDataPath, logFileName);
			string text = JsonUtility.ToJson(new LogEntry<T>
			{
				Timestamp = DateTime.Now.ToString("MM-dd HH:mm:ss"),
				Data = data
			});
			File.AppendAllText(path, text + Environment.NewLine);
		}
	}
}
