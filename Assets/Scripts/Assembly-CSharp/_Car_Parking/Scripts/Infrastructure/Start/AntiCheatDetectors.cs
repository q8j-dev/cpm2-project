using System;
using System.Globalization;
using System.Text;
using CodeStage.AntiCheat.Detectors;
using CodeStage.AntiCheat.Storage;
using UnityEngine;

namespace _Car_Parking.Scripts.Infrastructure.Start
{
	public static class AntiCheatDetectors
	{
		private const int MarkGeneration = 2;

		public static bool DetectedThisSession { get; private set; }

		public static bool MarkedPreviously => ObscuredPrefs.GetBool(CheatingMarkKey(2));

		public static event Action CheatingDetected;

		private static string CheatingMarkKey(int generation)
		{
			byte[] array = new byte[8] { 97, 99, 100, 95, 109, 97, 114, 107 };
			StringBuilder stringBuilder = new StringBuilder(array.Length + 2);
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append((char)array[i]);
			}
			if (generation > 1)
			{
				stringBuilder.Append(generation.ToString(CultureInfo.InvariantCulture));
			}
			return stringBuilder.ToString();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		public static void Start()
		{
			ForgetOlderGenerations(2);
			ObscuredCheatingDetector.StartDetection(OnCheatingDetected);
		}

		private static void ForgetOlderGenerations(int currentGeneration)
		{
			bool flag = false;
			for (int i = 1; i < currentGeneration; i++)
			{
				string key = CheatingMarkKey(i);
				if (ObscuredPrefs.HasKey(key))
				{
					ObscuredPrefs.DeleteKey(key);
					flag = true;
				}
			}
			if (flag)
			{
				ObscuredPrefs.Save();
			}
		}

		private static void OnCheatingDetected()
		{
			if (!DetectedThisSession)
			{
				DetectedThisSession = true;
				ObscuredPrefs.SetBool(CheatingMarkKey(2), value: true);
				Debug.LogError("[AntiCheat] Obscured value mismatch — memory tampering detected.");
				CheatingDetected?.Invoke();
			}
		}
	}
}
