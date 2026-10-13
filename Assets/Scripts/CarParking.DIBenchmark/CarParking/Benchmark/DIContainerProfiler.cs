using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CarParking.Benchmark
{
	public static class DIContainerProfiler
	{
		private struct ActiveSample
		{
			public Stopwatch Sw;

			public long GcBefore;
		}

		public const string PersistentConfigure = "PersistentScope.Configure";

		public const string PersistentTotalAwake = "PersistentScope.TotalAwake";

		public const string SceneConfigure = "SceneScope.Configure";

		public const string SceneTotalAwake = "SceneScope.TotalAwake";

		private static readonly Dictionary<string, ActiveSample> _active = new Dictionary<string, ActiveSample>();

		private static readonly Dictionary<string, (float ElapsedMs, long GcBytes)> _completed = new Dictionary<string, (float, long)>();

		private static bool _frozen;

		public static void BeginSample(string label)
		{
			ActiveSample value = new ActiveSample
			{
				GcBefore = GC.GetTotalMemory(forceFullCollection: false),
				Sw = Stopwatch.StartNew()
			};
			_active[label] = value;
		}

		public static float EndSample(string label)
		{
			if (!_active.TryGetValue(label, out var value))
			{
				return 0f;
			}
			value.Sw.Stop();
			long item = GC.GetTotalMemory(forceFullCollection: false) - value.GcBefore;
			float num = (float)value.Sw.Elapsed.TotalMilliseconds;
			_completed[label] = (num, item);
			_active.Remove(label);
			return num;
		}

		public static float GetElapsedMs(string label)
		{
			if (!_completed.TryGetValue(label, out (float, long) value))
			{
				return 0f;
			}
			return value.Item1;
		}

		public static long GetGcBytes(string label)
		{
			if (!_completed.TryGetValue(label, out (float, long) value))
			{
				return 0L;
			}
			return value.Item2;
		}

		public static void Reset()
		{
			_active.Clear();
			if (!_frozen)
			{
				_completed.Clear();
				return;
			}
			List<string> list = new List<string>(_completed.Count);
			foreach (string key in _completed.Keys)
			{
				if (key != "PersistentScope.Configure" && key != "PersistentScope.TotalAwake")
				{
					list.Add(key);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				_completed.Remove(list[i]);
			}
		}

		public static void FreezePersistentSamples()
		{
			_frozen = true;
		}
	}
}
