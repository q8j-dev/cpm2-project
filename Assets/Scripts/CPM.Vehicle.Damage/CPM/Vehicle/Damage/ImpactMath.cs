using Unity.Collections;
using Unity.Mathematics;

namespace CPM.Vehicle.Damage
{
	public static class ImpactMath
	{
		public static bool TryBuild(NativeArray<float3> points, NativeArray<float3> inwardNormals, int count, float3 impulse, float3 relativeVelocity, float3 bodyUp, in DamageSettings s, out ImpactEvent e)
		{
			e = default;
			if (count <= 0)
			{
				return false;
			}
			float3 zero = float3.zero;
			float3 zero2 = float3.zero;
			for (int i = 0; i < count; i++)
			{
				zero += points[i];
				zero2 += inwardNormals[i];
			}
			zero /= (float)count;
			if (math.lengthsq(zero2) < 1E-08f)
			{
				return false;
			}
			zero2 = math.normalize(zero2);
			float x = 0f;
			for (int j = 0; j < count; j++)
			{
				x = math.max(x, math.distancesq(points[j], zero));
			}
			float num = math.length(impulse);
			float num2 = math.dot(bodyUp, zero2);
			if (num2 > 0f)
			{
				num *= 1f - num2 * s.YForceDamp;
			}
			if (num < s.MinImpulse)
			{
				return false;
			}
			float3 slideAxis = float3.zero;
			float slideStretch = 1f;
			if (s.SlideStretch > 1f && math.lengthsq(relativeVelocity) > 1E-06f)
			{
				float3 float5 = relativeVelocity - zero2 * math.dot(relativeVelocity, zero2);
				float num3 = math.length(float5);
				if (num3 > 0.0001f)
				{
					slideAxis = float5 / num3;
					float t = math.saturate(num3 / math.length(relativeVelocity));
					slideStretch = math.lerp(1f, s.SlideStretch, t);
				}
			}
			e.WorldPoint = zero;
			e.WorldDir = zero2;
			e.SlideAxis = slideAxis;
			e.SlideStretch = slideStretch;
			e.Radius = s.RadiusBase * 0.5f + math.min(math.sqrt(x) * 0.5f, 0.15f);
			e.Depth = s.MaxDepth * (1f - math.exp((0f - (num - s.MinImpulse)) / s.ImpulseScale));
			return true;
		}
	}
}
