using System;
using UnityEngine;

namespace _Car_Parking.Scripts
{
	public static class SpawnLayout
	{
		public static PositionsAndRotation GetPoint(int index, int count, in SpawnLayoutSettings settings)
		{
			return GetPoint(index, count, in settings, Vector3.zero, Quaternion.identity);
		}

		public static PositionsAndRotation GetPoint(int index, int count, in SpawnLayoutSettings settings, Vector3 origin, Quaternion orientation)
		{
			count = Mathf.Max(1, count);
			index = Mathf.Clamp(index, 0, count - 1);
			PositionsAndRotation positionsAndRotation = ((settings.Shape == SpawnLayoutShape.Circle) ? Circle(index, count, in settings) : Grid(index, count, in settings));
			return new PositionsAndRotation
			{
				Position = origin + orientation * positionsAndRotation.Position,
				Rotation = orientation * positionsAndRotation.Rotation
			};
		}

		private static PositionsAndRotation Grid(int index, int count, in SpawnLayoutSettings s)
		{
			int num = Mathf.Max(1, s.ColumnsPerRow);
			int num2 = index / num;
			int num3 = index % num;
			float num4 = (float)(Mathf.Min(num, count - num2 * num) - 1) * 0.5f;
			float x = ((float)num3 - num4) * s.GridSpacing.x;
			float z = (float)(-num2) * s.GridSpacing.y;
			return new PositionsAndRotation
			{
				Position = new Vector3(x, 0f, z),
				Rotation = Quaternion.identity
			};
		}

		private static PositionsAndRotation Circle(int index, int count, in SpawnLayoutSettings s)
		{
			float f = (float)Math.PI * 2f * (float)index / (float)count;
			float num = Mathf.Sin(f);
			float num2 = Mathf.Cos(f);
			Vector3 position = new Vector3(num, 0f, num2) * s.Radius;
			Vector3 forward = (s.FaceOutward ? new Vector3(num, 0f, num2) : new Vector3(0f - num, 0f, 0f - num2));
			return new PositionsAndRotation
			{
				Position = position,
				Rotation = Quaternion.LookRotation(forward, Vector3.up)
			};
		}
	}
}
