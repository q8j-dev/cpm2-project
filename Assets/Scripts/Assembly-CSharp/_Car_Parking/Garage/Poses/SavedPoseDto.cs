using System;
using UnityEngine;

namespace _Car_Parking.Garage.Poses
{
	[Serializable]
	public struct SavedPoseDto
	{
		public int GarageId;

		public Vector3 Position;

		public float Yaw;

		public float SteerNorm;
	}
}
