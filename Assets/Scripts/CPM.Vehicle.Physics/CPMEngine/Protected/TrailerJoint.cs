using UnityEngine;

namespace CPMEngine.Protected
{
	public struct TrailerJoint
	{
		public PhysEngine Trailer;

		public Vector3 Loc1;

		public Vector3 Loc2;

		public float Spring;

		public float Damper;

		public float HorAngLimit;

		public float ScrewAngLimit;

		public float HillAngLimit;
	}
}
