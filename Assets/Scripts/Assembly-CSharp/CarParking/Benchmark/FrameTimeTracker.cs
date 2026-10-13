using UnityEngine;

namespace CarParking.Benchmark
{
	public class FrameTimeTracker : MonoBehaviour
	{
		public bool IsTracking { get; set; }

		public float PeakFrametimeMs { get; private set; }

		public void ResetPeak()
		{
			PeakFrametimeMs = 0f;
		}

		private void Update()
		{
			if (IsTracking)
			{
				float num = Time.unscaledDeltaTime * 1000f;
				if (num > PeakFrametimeMs)
				{
					PeakFrametimeMs = num;
				}
			}
		}
	}
}
