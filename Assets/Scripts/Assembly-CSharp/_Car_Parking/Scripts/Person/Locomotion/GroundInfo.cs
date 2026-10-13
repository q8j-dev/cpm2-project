using UnityEngine;

namespace _Car_Parking.Scripts.Person.Locomotion
{
	public struct GroundInfo
	{
		public bool Grounded;

		public Vector3 Normal;

		public Vector3 Point;

		public float SlopeAngle;

		public Collider Collider;

		public bool IsSteepSlope;
	}
}
