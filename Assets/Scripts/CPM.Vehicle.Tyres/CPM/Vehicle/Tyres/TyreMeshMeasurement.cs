using UnityEngine;

namespace CPM.Vehicle.Tyres
{
	public static class TyreMeshMeasurement
	{
		public const float FlangeAllowance = 0.005f;

		private const float WidthBandFraction = 0.12f;

		public static BeadMeasurement MeasureBead(float[] radii, float[] halfWidths, float diameterAllowance, float widthAllowance = 0f)
		{
			if (radii == null || halfWidths == null || radii.Length == 0 || halfWidths.Length != radii.Length)
			{
				return default;
			}
			float num = float.MaxValue;
			float num2 = 0f;
			for (int i = 0; i < radii.Length; i++)
			{
				if (radii[i] < num)
				{
					num = radii[i];
				}
				if (radii[i] > num2)
				{
					num2 = radii[i];
				}
			}
			float num3 = num + (num2 - num) * 0.12f;
			float num4 = 0f;
			for (int j = 0; j < radii.Length; j++)
			{
				if (radii[j] <= num3 && halfWidths[j] > num4)
				{
					num4 = halfWidths[j];
				}
			}
			return new BeadMeasurement(num * 2f + diameterAllowance, num4 * 2f + widthAllowance);
		}

		public static BeadMeasurement MeasureBead(Vector3[] vertices, float diameterAllowance = 0.005f, float widthAllowance = 0f)
		{
			if (vertices == null || vertices.Length == 0)
			{
				return default;
			}
			float[] array = new float[vertices.Length];
			float[] array2 = new float[vertices.Length];
			for (int i = 0; i < vertices.Length; i++)
			{
				Vector3 vector = vertices[i];
				array[i] = Mathf.Sqrt(vector.y * vector.y + vector.z * vector.z);
				array2[i] = Mathf.Abs(vector.x);
			}
			return MeasureBead(array, array2, diameterAllowance, widthAllowance);
		}
	}
}
