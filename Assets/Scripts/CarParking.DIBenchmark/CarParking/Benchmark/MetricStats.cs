using System;

namespace CarParking.Benchmark
{
	[Serializable]
	public class MetricStats
	{
		public float mean;

		public float median;

		public float min;

		public float max;

		public float stddev;

		public int sampleCount;

		public static MetricStats Compute(float[] values)
		{
			if (values == null || values.Length == 0)
			{
				return new MetricStats();
			}
			int num = values.Length;
			float[] array = new float[num];
			Array.Copy(values, array, num);
			Array.Sort(array);
			double num2 = 0.0;
			for (int i = 0; i < num; i++)
			{
				num2 += (double)array[i];
			}
			float num3 = (float)(num2 / (double)num);
			float num4 = ((num % 2 == 0) ? ((array[num / 2 - 1] + array[num / 2]) * 0.5f) : array[num / 2]);
			double num5 = 0.0;
			for (int j = 0; j < num; j++)
			{
				double num6 = array[j] - num3;
				num5 += num6 * num6;
			}
			float num7 = (float)Math.Sqrt(num5 / (double)num);
			return new MetricStats
			{
				mean = num3,
				median = num4,
				min = array[0],
				max = array[num - 1],
				stddev = num7,
				sampleCount = num
			};
		}
	}
}
