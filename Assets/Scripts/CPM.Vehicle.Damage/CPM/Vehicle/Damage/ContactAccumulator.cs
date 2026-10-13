using Unity.Mathematics;

namespace CPM.Vehicle.Damage
{
	public struct ContactAccumulator
	{
		public int ColliderId;

		public float Normal;

		public float Elapsed;

		public void Reset()
		{
			ColliderId = 0;
			Normal = 0f;
			Elapsed = 0f;
		}

		public bool Add(int colliderId, float3 frameImpulse, float3 inward, float dt, float minImpulse, out float impulse)
		{
			impulse = 0f;
			if (colliderId != ColliderId)
			{
				ColliderId = colliderId;
				Normal = 0f;
				Elapsed = 0f;
			}
			float num = math.abs(math.dot(frameImpulse, inward));
			if (num > 0f)
			{
				Normal += num;
			}
			Elapsed += dt;
			if (Normal < minImpulse)
			{
				return false;
			}
			impulse = Normal;
			Normal = 0f;
			return true;
		}
	}
}
