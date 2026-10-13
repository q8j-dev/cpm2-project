using Unity.Mathematics;

namespace CPM.Vehicle.Physics.Burst
{
	public struct RbBurstBody
	{
		public RbState State;

		public float3 Velocity => State.V;

		public float3 WorldCenterMass => State.WorldCm;

		public void SetParams(in RbEntityData d)
		{
			RbMathCore.SetParams(ref State, in d);
		}

		public void AddExtForce(float3 fdt)
		{
			RbMathCore.AddExtForce(ref State, fdt);
		}

		public void AddExtForce(float3 pos, float3 fdt)
		{
			RbMathCore.AddExtForce(ref State, pos, fdt);
		}

		public void AddExtTorque(float3 tdt)
		{
			RbMathCore.AddExtTorque(ref State, tdt);
		}

		public void ProcessExternalForces()
		{
			RbMathCore.ProcessExternalForces(ref State);
		}

		public void StoreVelocity()
		{
			RbMathCore.StoreVelocity(ref State);
		}

		public void UpdatePosition(float dt)
		{
			RbMathCore.UpdatePosition(ref State, dt);
		}

		public void CalcAccel(float invDt)
		{
			RbMathCore.CalcAccel(ref State, invDt);
		}
	}
}
