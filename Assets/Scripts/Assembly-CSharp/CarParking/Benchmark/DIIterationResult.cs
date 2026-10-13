using System;

namespace CarParking.Benchmark
{
	[Serializable]
	public class DIIterationResult
	{
		public int iteration;

		public bool isWarmup;

		public float configureMs;

		public float buildMs;

		public float totalAwakeMs;

		public float sceneLoadMs;

		public float peakFrametimeMs;

		public long gcAllocBytes;
	}
}
