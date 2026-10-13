using System.Collections.Generic;
using System.Linq;

namespace _Car_Parking.Scripts.CpmCustomFeatures.Benchmarking
{
	public sealed class FpsData
	{
		public readonly float Min;

		public readonly float Max;

		public readonly float Average;

		public readonly float Median;

		public readonly List<float> AllFps;

		public readonly List<float> AllDt;

		public FpsData(List<float> allFps, List<float> allDt)
		{
			AllFps = allFps.ToList();
			AllDt = allDt.ToList();
			Min = AllFps.Min();
			Max = AllFps.Max();
			Average = AllFps.Average();
			Median = AllFps.OrderBy((float f) => f).ToList()[AllFps.Count / 2];
		}

		public override string ToString()
		{
			return $"Frames: {AllFps.Count}\n" + $"Min: {Min}\n" + $"Max: {Max}\n" + $"Average: {Average}\n" + $"Median: {Median}";
		}
	}
}
