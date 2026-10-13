using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts
{
	public class WeightedProgress
	{
		private class Stage
		{
			public readonly float Weight;

			public float Progress;

			public Stage(float weight)
			{
				Weight = weight;
				Progress = 0f;
			}
		}

		private readonly Dictionary<string, Stage> _stages = new Dictionary<string, Stage>();

		public float Total { get; private set; }

		public void CreateStage(string name, float weight)
		{
			if (!_stages.ContainsKey(name))
			{
				_stages[name] = new Stage(weight);
				Recalculate();
			}
		}

		public void SetProgress(string name, float value)
		{
			if (_stages.TryGetValue(name, out var value2))
			{
				value2.Progress = Mathf.Max(value2.Progress, Mathf.Clamp01(value));
				Recalculate();
			}
		}

		public void Reset()
		{
			foreach (Stage value in _stages.Values)
			{
				value.Progress = 0f;
			}
			Total = 0f;
		}

		public void ResetStage(string name)
		{
			if (_stages.TryGetValue(name, out var value))
			{
				value.Progress = 0f;
				Recalculate();
			}
		}

		private void Recalculate()
		{
			float num = 0f;
			float num2 = 0f;
			foreach (Stage value in _stages.Values)
			{
				num += value.Weight;
				num2 += value.Weight * value.Progress;
			}
			Total = ((num > 0f) ? (num2 / num) : 0f);
		}
	}
}
