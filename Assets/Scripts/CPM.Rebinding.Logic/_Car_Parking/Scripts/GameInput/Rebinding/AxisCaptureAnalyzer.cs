using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public sealed class AxisCaptureAnalyzer
	{
		private const float PedalThreshold = 0.5f;

		private const float BipolarThreshold = 0.2f;

		private const float DeltaThreshold = 150f;

		private const float RestAtEndThreshold = 0.5f;

		private const double SustainSeconds = 0.15;

		private readonly AxisSemantic _semantic;

		private readonly float _threshold;

		private float[] _baseline = Array.Empty<float>();

		private float[] _accumulated = Array.Empty<float>();

		private int _candidate = -1;

		private double _candidateSince;

		public AxisCaptureAnalyzer(AxisSemantic semantic)
		{
			_semantic = semantic;
			_threshold = semantic switch
			{
				AxisSemantic.Pedal => 0.5f, 
				AxisSemantic.Delta => 150f, 
				_ => 0.2f, 
			};
		}

		public void Begin(IReadOnlyList<float> baseline, double time)
		{
			_baseline = new float[baseline.Count];
			for (int i = 0; i < baseline.Count; i++)
			{
				_baseline[i] = baseline[i];
			}
			_accumulated = new float[baseline.Count];
			_candidate = -1;
			_candidateSince = time;
		}

		public bool Feed(IReadOnlyList<float> values, double time, out AxisCaptureResult result)
		{
			result = default;
			int num = -1;
			float num2 = 0f;
			int num3 = Math.Min(values.Count, _baseline.Length);
			for (int i = 0; i < num3; i++)
			{
				float num4 = Math.Abs(ChangeOf(i, values[i]));
				if (num4 >= _threshold && num4 > num2)
				{
					num = i;
					num2 = num4;
				}
			}
			if (num != _candidate)
			{
				_candidate = num;
				_candidateSince = time;
				return false;
			}
			if (num < 0 || time - _candidateSince < 0.15)
			{
				return false;
			}
			float num5 = ((_semantic == AxisSemantic.Delta) ? _accumulated[num] : (values[num] - _baseline[num]));
			bool needsRemap = _semantic == AxisSemantic.Pedal && Math.Abs(_baseline[num]) > 0.5f;
			result = new AxisCaptureResult(num, num5 < 0f, needsRemap);
			return true;
		}

		private float ChangeOf(int index, float value)
		{
			if (_semantic != AxisSemantic.Delta)
			{
				return value - _baseline[index];
			}
			_accumulated[index] += value;
			return _accumulated[index];
		}
	}
}
