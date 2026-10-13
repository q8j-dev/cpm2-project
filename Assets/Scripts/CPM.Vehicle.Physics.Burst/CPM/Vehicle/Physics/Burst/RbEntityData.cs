using Unity.Mathematics;

namespace CPM.Vehicle.Physics.Burst
{
	public struct RbEntityData
	{
		public float Mass;

		public float3 Inertia;

		public float3 CenterOfMass;

		public float3 AngularVelocity;

		public float3 Velocity;

		public float3x3 Basis;

		public float3 Origin;
	}
}
