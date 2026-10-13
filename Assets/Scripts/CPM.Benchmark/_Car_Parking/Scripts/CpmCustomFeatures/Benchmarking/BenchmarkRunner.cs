using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.CpmCustomFeatures.Benchmarking
{
	public sealed class BenchmarkRunner
	{
		private const string ManualBenchmarkName = "Manual";

		private BenchmarkReport _lastReport;

		private static MonoBenchmark _monoBenchmark;

		private readonly Dictionary<string, Func<Dictionary<string, object>, UniTask>> _runners = new Dictionary<string, Func<Dictionary<string, object>, UniTask>>();

		private static UniTaskCompletionSource _manualBenchmark;

		public static BenchmarkRunner Instance { get; private set; }

		public static BenchmarkReport LastReport => Instance._lastReport;

		public static event Action<string, bool> OnBenchmarkStateChanged;

		public static event Action<BenchmarkReport> OnReportReady;

		public static event Action OnUpdate
		{
			add
			{
				_monoBenchmark.OnUpdate += value;
			}
			remove
			{
				_monoBenchmark.OnUpdate -= value;
			}
		}

		[RuntimeInitializeOnLoadMethod]
		private static void Initialize()
		{
			if (Instance == null)
			{
				Instance = new BenchmarkRunner();
				_monoBenchmark = new GameObject("MonoBenchmark", typeof(MonoBenchmark)).GetComponent<MonoBenchmark>();
				UnityEngine.Object.DontDestroyOnLoad(_monoBenchmark.gameObject);
			}
		}

		public static UniTask Run(string benchmarkName)
		{
			return Instance.RunBenchmark(benchmarkName);
		}

		private async UniTask RunBenchmark(string benchmarkName)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(0.5));
			Dictionary<string, object> arg = new Dictionary<string, object>();
			DateTime start = DateTime.Now;
			_monoBenchmark.RecordFps();
			if (_runners.TryGetValue(benchmarkName, out var value))
			{
				await value(arg);
			}
			_monoBenchmark.StopRecordFps();
			BenchmarkReport lastReport = new BenchmarkReport(benchmarkName, start, DateTime.Now)
			{
				FpsData = _monoBenchmark.GetFpsData()
			};
			_lastReport = lastReport;
			OnReportReady?.Invoke(_lastReport);
		}

		public static void RegisterBenchmark(string benchmarkName, Func<Dictionary<string, object>, UniTask> runner)
		{
			Instance._runners[benchmarkName] = runner;
			OnBenchmarkStateChanged?.Invoke(benchmarkName, arg2: true);
		}

		public static void UnregisterBenchmark(string benchmarkName)
		{
			Instance._runners.Remove(benchmarkName);
			OnBenchmarkStateChanged?.Invoke(benchmarkName, arg2: false);
		}

		public static void StartManualBenchmark()
		{
			_manualBenchmark?.TrySetCanceled();
			Instance._runners.Remove("Manual");
			_manualBenchmark = new UniTaskCompletionSource();
			Instance._runners["Manual"] = (Dictionary<string, object> d) => _manualBenchmark.Task;
			Run("Manual");
		}

		public static void StopManualBenchmark()
		{
			_manualBenchmark?.TrySetResult();
		}
	}
}
