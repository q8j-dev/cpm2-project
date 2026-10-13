using System;

namespace CarParking.Benchmark
{
	[Serializable]
	public class BenchmarkReportData
	{
		public string framework;

		public string deviceModel;

		public string operatingSystem;

		public string unityVersion;

		public string timestamp;

		public int totalIterations;

		public int warmupIterations;

		public string testSceneName;

		public DIIterationResult persistentScopeCost;

		public MetricStats configureStats;

		public MetricStats buildStats;

		public MetricStats totalAwakeStats;

		public MetricStats sceneLoadStats;

		public MetricStats peakFrametimeStats;

		public MetricStats gcAllocStats;

		public DIIterationResultList iterationLog;
	}
}
