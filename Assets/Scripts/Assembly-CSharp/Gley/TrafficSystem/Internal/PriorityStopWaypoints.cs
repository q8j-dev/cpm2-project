using System;

namespace Gley.TrafficSystem.Internal
{
	[Serializable]
	public class PriorityStopWaypoints
	{
		public int[] roadWaypoints;

		public float greenLightTime;

		public int[] pedestrianWaypoints;

		public int[] directionWaypoints;

		public PriorityStopWaypoints(int[] roadWaypoints, float greenLightTime)
		{
			this.roadWaypoints = roadWaypoints;
			this.greenLightTime = greenLightTime;
		}
	}
}
