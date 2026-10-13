using UnityEngine;

namespace CPMEngine.Protected
{
	public class RB
	{
		private float _mass;

		private float _invMass;

		private Matrix4x4 _matrix = Matrix4x4.identity;

		private Matrix4x4 _invMatrix = Matrix4x4.identity;

		private Matrix4x4 _invInertia = Matrix4x4.identity;

		private Matrix4x4 _invInertiaWorld;

		private Vector3 _w = Vector3.zero;

		private Vector3 _a = Vector3.zero;

		private Vector3 _v = Vector3.zero;

		private Vector3 _oldV = Vector3.zero;

		private Vector3 _oldW = Vector3.zero;

		private Vector3 _localCm = Vector3.zero;

		private Vector3 _worldCm = Vector3.zero;

		private Vector3 _inertia;

		private Vector3 _extFdt;

		private Vector3 _extMFdt;

		private Vector3 _totalFdt;

		private Vector3 _totalMFdt;

		public Vector3 Acceleration => _a;

		public Vector3 AngularVelocity => _w;

		public Matrix4x4 InvMatrix => _invMatrix;

		public Vector3 LocalCenterMass => _localCm;

		public float Mass
		{
			get
			{
				return Mathf.Max(_mass, 400f);
			}
			private set
			{
				_mass = value;
				float num = Mathf.Max(Mathf.Abs(_mass), 1f);
				_invMass = 1f / num;
			}
		}

		public Matrix4x4 Matrix => _matrix;

		public Vector3 Velocity => _v;

		public Vector3 WorldCenterMass => _worldCm;

		public void AddExtForce(Vector3 fdt)
		{
			_extFdt += fdt;
		}

		public void AddExtForce(Vector3 pos, Vector3 fdt)
		{
			_extFdt += fdt;
			_extMFdt += Vector3.Cross(pos - _worldCm, fdt);
		}

		public void AddExtTorque(Vector3 tdt)
		{
			_extMFdt += tdt;
		}

		public void AlignForces(Plane rightPlane)
		{
			_totalFdt -= PhysMath.Vector3Mul(rightPlane.normal, Vector3.Dot(rightPlane.normal, _totalFdt));
		}

		public void AlignMatrix(Plane rightPlane)
		{
			Vector3 up = PhysMath.GetUp(_matrix);
			Vector3 forward = PhysMath.GetForward(_matrix);
			Matrix4x4 m = default;
			Matrix4x4 res = default;
			PhysMath.MatrixTransform(ref m, Vector3.zero, Vector3.right, Vector3.up, Vector3.forward);
			PhysMath.MatrixGetInverse(m, ref res);
			forward -= PhysMath.Vector3Mul(rightPlane.normal, Vector3.Dot(rightPlane.normal, forward));
			forward.Normalize();
			up -= PhysMath.Vector3Mul(forward, Vector3.Dot(forward, up));
			up.Normalize();
			Vector3 ox = Vector3.Cross(up, forward);
			Matrix4x4 m2 = default;
			PhysMath.MatrixTransform(ref m2, Vector3.zero, ox, up, forward);
			Matrix4x4 res2 = Matrix4x4.identity;
			PhysMath.MatrixMultiply3x3(m2, res, ref res2);
			Vector3 origin = PhysMath.GetOrigin(_matrix);
			Vector3 v = origin - PhysMath.Vector3Mul(rightPlane.normal, Vector3.Dot(rightPlane.normal, origin) + rightPlane.distance);
			PhysMath.MatrixSetTranslate(ref res2, v);
			SetMatrix(res2);
		}

		public void AlignVelocities(Plane rightPlane)
		{
			_v -= PhysMath.Vector3Mul(rightPlane.normal, Vector3.Dot(rightPlane.normal, _v));
			_oldV -= PhysMath.Vector3Mul(rightPlane.normal, Vector3.Dot(rightPlane.normal, _oldV));
		}

		public void CalcAccel(float invDT)
		{
			_a = PhysMath.Vector3Mul(_v - _oldV, invDT);
			_oldV = _v;
		}

		private void ClearExternals()
		{
			_totalFdt += _extFdt;
			_totalMFdt += _extMFdt;
			_extFdt = Vector3.zero;
			_extMFdt = Vector3.zero;
		}

		public void ClearTotals()
		{
			_totalFdt = Vector3.zero;
			_totalMFdt = Vector3.zero;
			_extFdt = Vector3.zero;
			_extMFdt = Vector3.zero;
		}

		private static float Determinant(Vector3 c0, Vector3 c1, Vector3 c2)
		{
			return c0.x * (c1.y * c2.z - c1.z * c2.y) + c0.y * (c1.z * c2.x - c1.x * c2.z) + c0.z * (c1.x * c2.y - c1.y * c2.x);
		}

		public Vector3 GetBrakingImpulse(Vector3 c, Vector3 n)
		{
			Vector3 vector = c - _worldCm;
			Vector3 rhs = _v + Vector3.Cross(_w, vector);
			Vector3 lhs = _invInertiaWorld.MultiplyVector(Vector3.Cross(vector, n));
			Vector3 lhs2 = PhysMath.Vector3Mul(n, _invMass) + Vector3.Cross(lhs, vector);
			if (!(Mathf.Abs(Vector3.Dot(lhs2, n)) < 1E-05f))
			{
				return PhysMath.Vector3Mul(n, (0f - Vector3.Dot(n, rhs)) / Vector3.Dot(lhs2, n));
			}
			return Vector3.zero;
		}

		public Vector3 GetDeltaImpulse(Vector3 c, Vector3 n, Vector3 dv)
		{
			Vector3 vector = c - _worldCm;
			Vector3 lhs = _invInertiaWorld.MultiplyVector(Vector3.Cross(vector, n));
			Vector3 lhs2 = PhysMath.Vector3Mul(n, _invMass) + Vector3.Cross(lhs, vector);
			if (!(Mathf.Abs(Vector3.Dot(lhs2, n)) < 1E-05f))
			{
				return PhysMath.Vector3Mul(n, (0f - Vector3.Dot(n, -dv)) / Vector3.Dot(lhs2, n));
			}
			return Vector3.zero;
		}

		public void GetExtForce(out Vector3 fdt)
		{
			fdt = _extFdt;
		}

		public float GetInertiaDir(Vector3 dir)
		{
			Vector3 vector = _invMatrix.MultiplyVector(dir);
			return _inertia.x * vector.x * vector.x + _inertia.y * vector.y * vector.y + _inertia.z * vector.z * vector.z;
		}

		public void GetParams(ref RbEntity desc)
		{
			desc.Matrix = _matrix;
			desc.Mass = _mass;
			desc.Inertia = _inertia;
			desc.CenterOfMass = _localCm;
			desc.Velocity = _v;
			desc.AngularVelocity = _w;
		}

		public void ResetVelocites()
		{
			_v = Vector3.zero;
			_w = Vector3.zero;
		}

		public void GetTotals(out Vector3 fdt, out Vector3 mFdt)
		{
			fdt = _totalFdt;
			mFdt = _totalMFdt;
		}

		public Vector3 GetVelocity(Vector3 pos)
		{
			return _v + Vector3.Cross(_w, pos - _worldCm);
		}

		private void SetInertia(Vector3 inertia)
		{
			inertia.x = Mathf.Max(Mathf.Abs(inertia.x), 0.0001f);
			inertia.y = Mathf.Max(Mathf.Abs(inertia.y), 0.0001f);
			inertia.z = Mathf.Max(Mathf.Abs(inertia.z), 0.0001f);
			_inertia = inertia;
			_invInertia.m00 = 1f / inertia.x;
			_invInertia.m11 = 1f / inertia.y;
			_invInertia.m22 = 1f / inertia.z;
			_invInertiaWorld = _invInertia;
		}

		private void SetMatrix(Matrix4x4 m)
		{
			_matrix = m;
			_invMatrix = Matrix4x4.Inverse(_matrix);
			_worldCm = _matrix.MultiplyPoint3x4(_localCm);
			Matrix4x4 res = default;
			PhysMath.MatrixMultiply3x3(_matrix, _invInertia, ref res);
			PhysMath.MatrixMultiply3x3(res, _invMatrix, ref _invInertiaWorld);
		}

		public void SetParams(ref RbEntity desc)
		{
			Mass = desc.Mass;
			SetInertia(desc.Inertia);
			_localCm = desc.CenterOfMass;
			_v = desc.Velocity;
			_w = desc.AngularVelocity;
			SetMatrix(desc.Matrix);
		}

		private static int SolveCramer(Vector3 c0, Vector3 c1, Vector3 c2, Vector3 b, out Vector3 x)
		{
			x = default;
			float num = Determinant(c0, c1, c2);
			num = 1f / num;
			x.x = Determinant(b, c1, c2) * num;
			x.y = Determinant(c0, b, c2) * num;
			x.z = Determinant(c0, c1, b) * num;
			return 0;
		}

		public void UpdatePosition(float dt)
		{
			_worldCm += PhysMath.Vector3Mul(_oldV + _v, 0.5f * dt);
			Vector3 vector = PhysMath.Vector3Mul(_oldW + _w, 0.5f * dt);
			Matrix4x4 lhs = default;
			Matrix4x4 res = default;
			lhs.m00 = 0f;
			lhs.m01 = 0f - vector.z;
			lhs.m02 = vector.y;
			lhs.m10 = vector.z;
			lhs.m11 = 0f;
			lhs.m12 = 0f - vector.x;
			lhs.m20 = 0f - vector.y;
			lhs.m21 = vector.x;
			lhs.m22 = 0f;
			PhysMath.MatrixMultiply3x3(lhs, _matrix, ref res);
			PhysMath.MatrixAdd3x3(ref _matrix, res);
			PhysMath.MatrixOrthonormalize(ref _matrix);
			Vector3 vector2 = _matrix.MultiplyVector(_localCm);
			_matrix.m03 = _worldCm.x - vector2.x;
			_matrix.m13 = _worldCm.y - vector2.y;
			_matrix.m23 = _worldCm.z - vector2.z;
			SetMatrix(_matrix);
		}

		private void UpdateVelocity(Vector3 fdt, Vector3 mFdt)
		{
			_v += PhysMath.Vector3Mul(fdt, _invMass);
			_w += _invInertiaWorld.MultiplyVector(mFdt);
		}

		public void ProcessExternalForces()
		{
			UpdateVelocity(_extFdt, _extMFdt);
			ClearExternals();
		}

		public int SolveImpulseDynamic(RB a, RB b, Vector3 r1, Vector3 r2, Vector3 deltaV, out Vector3 fdt)
		{
			Matrix4x4 res = default;
			Matrix4x4 res2 = default;
			Matrix4x4 res3 = default;
			Matrix4x4 rhs = PhysMath.CrossProductMatrix(r1);
			PhysMath.MatrixMultiply3x3(rhs.transpose, a._invInertiaWorld, ref res3);
			PhysMath.MatrixMultiply3x3(res3, rhs, ref res);
			rhs = PhysMath.CrossProductMatrix(r2);
			PhysMath.MatrixMultiply3x3(rhs.transpose, b._invInertiaWorld, ref res3);
			PhysMath.MatrixMultiply3x3(res3, rhs, ref res2);
			Vector3 c = new Vector3(a._invMass + b._invMass + res.m00 + res2.m00, res.m10 + res2.m10, res.m20 + res2.m20);
			Vector3 c2 = new Vector3(res.m01 + res2.m01, a._invMass + b._invMass + res.m11 + res2.m11, res.m21 + res2.m21);
			Vector3 c3 = new Vector3(res.m02 + res2.m02, res.m12 + res2.m12, a._invMass + b._invMass + res.m22 + res2.m22);
			return SolveCramer(c, c2, c3, deltaV, out fdt);
		}

		public void StoreVelocity()
		{
			_oldV = _v;
			_oldW = _w;
		}
	}
}
