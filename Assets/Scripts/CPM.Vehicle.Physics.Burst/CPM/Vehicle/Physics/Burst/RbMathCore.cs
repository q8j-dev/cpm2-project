using Unity.Burst;
using Unity.Mathematics;

namespace CPM.Vehicle.Physics.Burst
{
	[BurstCompile]
	public static class RbMathCore
	{
		public static void SetInertia(ref RbState s, float3 inertia)
		{
			inertia.x = math.max(math.abs(inertia.x), 0.0001f);
			inertia.y = math.max(math.abs(inertia.y), 0.0001f);
			inertia.z = math.max(math.abs(inertia.z), 0.0001f);
			s.Inertia = inertia;
			s.InvInertiaDiag = new float3(1f / inertia.x, 1f / inertia.y, 1f / inertia.z);
			s.InvInertiaWorld = RbMathBurst.Diagonal(s.InvInertiaDiag);
		}

		public static void SetMatrix(ref RbState s, float3x3 basis, float3 origin)
		{
			s.M = basis;
			s.Origin = origin;
			s.WorldCm = math.mul(s.M, s.LocalCm) + s.Origin;
			float3x3 b = RbMathBurst.Diagonal(s.InvInertiaDiag);
			s.InvInertiaWorld = math.mul(math.mul(s.M, b), math.transpose(s.M));
		}

		public static void SetParams(ref RbState s, in RbEntityData d)
		{
			s.Mass = d.Mass;
			s.InvMass = 1f / math.max(math.abs(d.Mass), 1f);
			SetInertia(ref s, d.Inertia);
			s.LocalCm = d.CenterOfMass;
			s.V = d.Velocity;
			s.W = d.AngularVelocity;
			SetMatrix(ref s, d.Basis, d.Origin);
		}

		public static void GetParams(in RbState s, out RbEntityData d)
		{
			d.Mass = s.Mass;
			d.Inertia = s.Inertia;
			d.CenterOfMass = s.LocalCm;
			d.Velocity = s.V;
			d.AngularVelocity = s.W;
			d.Basis = s.M;
			d.Origin = s.Origin;
		}

		public static void AddExtForce(ref RbState s, float3 fdt)
		{
			s.ExtFdt += fdt;
		}

		public static void AddExtForce(ref RbState s, float3 pos, float3 fdt)
		{
			s.ExtFdt += fdt;
			s.ExtMFdt += math.cross(pos - s.WorldCm, fdt);
		}

		public static void AddExtTorque(ref RbState s, float3 tdt)
		{
			s.ExtMFdt += tdt;
		}

		public static void GetExtForce(in RbState s, out float3 fdt)
		{
			fdt = s.ExtFdt;
		}

		public static void GetTotals(in RbState s, out float3 fdt, out float3 mFdt)
		{
			fdt = s.TotalFdt;
			mFdt = s.TotalMFdt;
		}

		public static void ClearTotals(ref RbState s)
		{
			s.TotalFdt = float3.zero;
			s.TotalMFdt = float3.zero;
			s.ExtFdt = float3.zero;
			s.ExtMFdt = float3.zero;
		}

		private static void ClearExternals(ref RbState s)
		{
			s.TotalFdt += s.ExtFdt;
			s.TotalMFdt += s.ExtMFdt;
			s.ExtFdt = float3.zero;
			s.ExtMFdt = float3.zero;
		}

		private static void UpdateVelocity(ref RbState s, float3 fdt, float3 mFdt)
		{
			s.V += fdt * s.InvMass;
			s.W += math.mul(s.InvInertiaWorld, mFdt);
		}

		public static void ProcessExternalForces(ref RbState s)
		{
			UpdateVelocity(ref s, s.ExtFdt, s.ExtMFdt);
			ClearExternals(ref s);
		}

		public static void StoreVelocity(ref RbState s)
		{
			s.OldV = s.V;
			s.OldW = s.W;
		}

		public static void CalcAccel(ref RbState s, float invDT)
		{
			s.A = (s.V - s.OldV) * invDT;
			s.OldV = s.V;
		}

		public static void UpdatePosition(ref RbState s, float dt)
		{
			s.WorldCm += (s.OldV + s.V) * (0.5f * dt);
			float3x3 float3x5 = math.mul(RbMathBurst.Skew((s.OldW + s.W) * (0.5f * dt)), s.M);
			float3x3 m = s.M + float3x5;
			RbMathBurst.Orthonormalize(ref m);
			float3 float5 = math.mul(m, s.LocalCm);
			float3 origin = s.WorldCm - float5;
			SetMatrix(ref s, m, origin);
		}

		public static float3 GetVelocity(in RbState s, float3 pos)
		{
			return s.V + math.cross(s.W, pos - s.WorldCm);
		}

		public static float GetInertiaDir(in RbState s, float3 dir)
		{
			float3 float5 = math.mul(math.transpose(s.M), dir);
			return s.Inertia.x * float5.x * float5.x + s.Inertia.y * float5.y * float5.y + s.Inertia.z * float5.z * float5.z;
		}

		public static float3 GetBrakingImpulse(in RbState s, float3 c, float3 n)
		{
			float3 float5 = c - s.WorldCm;
			float3 y = s.V + math.cross(s.W, float5);
			float3 x = math.mul(s.InvInertiaWorld, math.cross(float5, n));
			float num = math.dot(n * s.InvMass + math.cross(x, float5), n);
			if (!(math.abs(num) < 1E-05f))
			{
				return n * ((0f - math.dot(n, y)) / num);
			}
			return float3.zero;
		}

		public static float3 GetDeltaImpulse(in RbState s, float3 c, float3 n, float3 dv)
		{
			float3 float5 = c - s.WorldCm;
			float3 x = math.mul(s.InvInertiaWorld, math.cross(float5, n));
			float num = math.dot(n * s.InvMass + math.cross(x, float5), n);
			if (!(math.abs(num) < 1E-05f))
			{
				return n * ((0f - math.dot(n, -dv)) / num);
			}
			return float3.zero;
		}

		private static float Det(float3 c0, float3 c1, float3 c2)
		{
			return c0.x * (c1.y * c2.z - c1.z * c2.y) + c0.y * (c1.z * c2.x - c1.x * c2.z) + c0.z * (c1.x * c2.y - c1.y * c2.x);
		}

		public static void SolveImpulseDynamic(in RbState a, in RbState b, float3 r1, float3 r2, float3 deltaV, out float3 fdt)
		{
			float3x3 float3x5 = RbMathBurst.CrossProductMatrix(r1);
			float3x3 obj = math.mul(math.mul(math.transpose(float3x5), a.InvInertiaWorld), float3x5);
			float3x3 float3x6 = RbMathBurst.CrossProductMatrix(r2);
			float3x3 float3x7 = math.mul(math.mul(math.transpose(float3x6), b.InvInertiaWorld), float3x6);
			float3x3 obj2 = obj + float3x7;
			float num = a.InvMass + b.InvMass;
			float3 c = obj2.c0 + new float3(num, 0f, 0f);
			float3 c2 = obj2.c1 + new float3(0f, num, 0f);
			float3 c3 = obj2.c2 + new float3(0f, 0f, num);
			float num2 = 1f / Det(c, c2, c3);
			fdt = new float3(Det(deltaV, c2, c3) * num2, Det(c, deltaV, c3) * num2, Det(c, c2, deltaV) * num2);
		}

		public static void AlignForces(ref RbState s, float3 n)
		{
			s.TotalFdt -= n * math.dot(n, s.TotalFdt);
		}

		public static void AlignVelocities(ref RbState s, float3 n)
		{
			s.V -= n * math.dot(n, s.V);
			s.OldV -= n * math.dot(n, s.OldV);
		}

		public static void AlignMatrix(ref RbState s, float3 n, float planeDistance)
		{
			float3 v = s.M.c1;
			float3 v2 = s.M.c2;
			v2 -= n * math.dot(n, v2);
			RbMathBurst.Normalize(ref v2);
			v -= v2 * math.dot(v2, v);
			RbMathBurst.Normalize(ref v);
			float3 c = math.cross(v, v2);
			float3x3 basis = new float3x3(c, v, v2);
			float3 origin = s.Origin;
			float3 origin2 = origin - n * (math.dot(n, origin) + planeDistance);
			SetMatrix(ref s, basis, origin2);
		}
	}
}
