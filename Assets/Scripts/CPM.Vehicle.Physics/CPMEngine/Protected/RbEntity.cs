using UnityEngine;

namespace CPMEngine.Protected
{
	public struct RbEntity
	{
		public float Mass;

		public Vector3 Inertia;

		public Vector3 CenterOfMass;

		public Vector3 AngularVelocity;

		public Vector3 Velocity;

		public Matrix4x4 Matrix;
	}
}
