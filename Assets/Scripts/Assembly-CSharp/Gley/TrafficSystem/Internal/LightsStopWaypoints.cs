using System;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
	[Serializable]
	public struct LightsStopWaypoints
	{
		public int[] roadWaypoints;

		public GameObject[] redLightObjects;

		public GameObject[] yellowLightObjects;

		public GameObject[] greenLightObjects;

		public float greenLightTime;

		public LightsStopWaypoints(int[] roadWaypoints, GameObject[] redLightObjects, GameObject[] yellowLightObjects, GameObject[] greenLightObjects, float greenLightTime)
		{
			this.roadWaypoints = roadWaypoints;
			this.redLightObjects = redLightObjects;
			this.yellowLightObjects = yellowLightObjects;
			this.greenLightObjects = greenLightObjects;
			this.greenLightTime = greenLightTime;
		}
	}
}
