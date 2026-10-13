using Unity.Burst;
using Unity.Mathematics;

namespace CPM.Vehicle.Physics.Burst
{
	[BurstCompile]
	public static class RbMathBurst
	{
		public static void Normalize(ref float3 v)
		{
			float num = math.sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
			if (num > 1E-05f)
			{
				float num2 = 1f / num;
				v.x *= num2;
				v.y *= num2;
				v.z *= num2;
			}
			else
			{
				v = float3.zero;
			}
		}

		public static float3x3 Skew(float3 v)
		{
			return new float3x3(0f, 0f - v.z, v.y, v.z, 0f, 0f - v.x, 0f - v.y, v.x, 0f);
		}

		public static float3x3 CrossProductMatrix(float3 v)
		{
			return new float3x3(0f, v.z, 0f - v.y, 0f - v.z, 0f, v.x, v.y, 0f - v.x, 0f);
		}

		public static void Orthonormalize(ref float3x3 m)
		{
			float3 v = m.c0;
			float3 v2 = m.c1;
			Normalize(ref v);
			v2 -= v * math.dot(v, v2);
			Normalize(ref v2);
			float3 c = math.cross(v, v2);
			m = new float3x3(v, v2, c);
		}

		public static float3x3 Diagonal(float3 d)
		{
			return new float3x3(d.x, 0f, 0f, 0f, d.y, 0f, 0f, 0f, d.z);
		}
	}
}
