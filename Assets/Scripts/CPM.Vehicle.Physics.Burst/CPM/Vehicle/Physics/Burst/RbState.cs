using Unity.Mathematics;

namespace CPM.Vehicle.Physics.Burst
{
	public struct RbState
	{
		public float Mass;

		public float InvMass;

		public float3x3 M;

		public float3 Origin;

		public float3 Inertia;

		public float3 InvInertiaDiag;

		public float3x3 InvInertiaWorld;

		public float3 W;

		public float3 A;

		public float3 V;

		public float3 OldV;

		public float3 OldW;

		public float3 LocalCm;

		public float3 WorldCm;

		public float3 ExtFdt;

		public float3 ExtMFdt;

		public float3 TotalFdt;

		public float3 TotalMFdt;
	}
}
