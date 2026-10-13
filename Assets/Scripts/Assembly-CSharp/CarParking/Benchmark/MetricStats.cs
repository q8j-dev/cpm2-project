using System;

namespace CarParking.Benchmark
{
	[Serializable]
	public struct MetricStats
	{
		public float mean;

		public float median;

		public float stddev;

		public float min;

		public float max;

		public static MetricStats Compute(float[] values)
		{
			if (values == null || values.Length == 0)
			{
				return default;
			}
			if (values.Length == 1)
			{
				return new MetricStats
				{
					mean = values[0],
					median = values[0],
					stddev = 0f,
					min = values[0],
					max = values[0]
				};
			}
			float[] array = new float[values.Length];
			Array.Copy(values, array, values.Length);
			Array.Sort(array);
			float num = 0f;
			for (int i = 0; i < array.Length; i++)
			{
				num += array[i];
			}
			float num2 = num / (float)array.Length;
			float num3 = 0f;
			for (int j = 0; j < array.Length; j++)
			{
				float num4 = array[j] - num2;
				num3 += num4 * num4;
			}
			int num5 = array.Length / 2;
			float num6 = ((array.Length % 2 == 0) ? ((array[num5 - 1] + array[num5]) / 2f) : array[num5]);
			return new MetricStats
			{
				mean = num2,
				median = num6,
				stddev = (float)Math.Sqrt(num3 / (float)array.Length),
				min = array[0],
				max = array[array.Length - 1]
			};
		}
	}
}
