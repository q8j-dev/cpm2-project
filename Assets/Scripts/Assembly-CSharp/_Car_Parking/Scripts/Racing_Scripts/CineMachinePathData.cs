using System;
using Cinemachine;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	[Serializable]
	public class CineMachinePathData
	{
		public int Resolution;

		public float PathLength;

		public bool Loopped;

		public CinemachinePath.Waypoint[] Waypoints;
	}
}
