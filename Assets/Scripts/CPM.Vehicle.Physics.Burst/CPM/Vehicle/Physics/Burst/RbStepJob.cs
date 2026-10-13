using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace CPM.Vehicle.Physics.Burst
{
	[BurstCompile]
	public struct RbStepJob : IJobParallelFor
	{
		public NativeArray<RbState> States;

		public float Dt;

		public float3 Gravity;

		public void Execute(int i)
		{
			RbState s = States[i];
			RbMathCore.StoreVelocity(ref s);
			RbMathCore.AddExtForce(ref s, Gravity * (s.Mass * Dt));
			RbMathCore.ProcessExternalForces(ref s);
			RbMathCore.UpdatePosition(ref s, Dt);
			RbMathCore.CalcAccel(ref s, 1f / Dt);
			States[i] = s;
		}
	}
}
