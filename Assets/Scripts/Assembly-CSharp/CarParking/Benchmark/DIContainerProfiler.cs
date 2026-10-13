using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace CarParking.Benchmark
{
	public static class DIContainerProfiler
	{
		private class SampleData
		{
			public readonly Stopwatch Stopwatch = new Stopwatch();

			public long GcBytesAtStart;

			public float ElapsedMs;

			public long GcBytes;

			public long EndTimestamp;

			public bool HasResult;
		}

		public const string SceneConfigure = "SceneConfigure";

		public const string SceneTotalAwake = "SceneTotalAwake";

		public const string PersistentConfigure = "PersistentConfigure";

		public const string PersistentTotalAwake = "PersistentTotalAwake";

		private static readonly Dictionary<string, SampleData> Samples = new Dictionary<string, SampleData>();

		public static long AppStartTimestamp { get; private set; }

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void CaptureAppStart()
		{
			AppStartTimestamp = Stopwatch.GetTimestamp();
		}

		public static void BeginSample(string name)
		{
			if (!Samples.TryGetValue(name, out var value))
			{
				value = new SampleData();
				Samples[name] = value;
			}
			value.GcBytesAtStart = GC.GetTotalMemory(forceFullCollection: false);
			value.Stopwatch.Restart();
		}

		public static void EndSample(string name)
		{
			if (Samples.TryGetValue(name, out var value))
			{
				value.Stopwatch.Stop();
				value.ElapsedMs = (float)value.Stopwatch.Elapsed.TotalMilliseconds;
				value.EndTimestamp = Stopwatch.GetTimestamp();
				long totalMemory = GC.GetTotalMemory(forceFullCollection: false);
				value.GcBytes = Math.Max(0L, totalMemory - value.GcBytesAtStart);
				value.HasResult = true;
			}
		}

		public static float GetElapsedMs(string name)
		{
			if (!Samples.TryGetValue(name, out var value) || !value.HasResult)
			{
				return 0f;
			}
			return value.ElapsedMs;
		}

		public static long GetGcBytes(string name)
		{
			if (!Samples.TryGetValue(name, out var value) || !value.HasResult)
			{
				return 0L;
			}
			return value.GcBytes;
		}

		public static long GetEndTimestamp(string name)
		{
			if (!Samples.TryGetValue(name, out var value) || !value.HasResult)
			{
				return 0L;
			}
			return value.EndTimestamp;
		}

		public static void Reset()
		{
			Samples.Clear();
		}

		public static void ResetSample(string name)
		{
			Samples.Remove(name);
		}
	}
}
