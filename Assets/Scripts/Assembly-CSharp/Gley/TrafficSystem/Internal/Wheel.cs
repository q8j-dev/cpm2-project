using System;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
	[Serializable]
	public class Wheel
	{
		public enum WheelPosition
		{
			Front = 0,
			Back = 1,
			Other = 2
		}

		public Transform wheelTransform;

		public Transform wheelGraphics;

		public WheelPosition wheelPosition;

		public float wheelRadius;

		[HideInInspector]
		public float wheelCircumference;

		[HideInInspector]
		public float raycastLength;

		public float maxSuspension;
	}
}
