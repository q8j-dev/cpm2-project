using System;
using UnityEngine;

namespace Gley.TrafficSystem
{
	[Serializable]
	public struct Area
	{
		public Vector3 center;

		public float radius;

		[HideInInspector]
		public float sqrRadius;

		public Area(Vector3 center, float radius)
		{
			this.center = center;
			this.radius = radius;
			sqrRadius = radius * radius;
		}

		public Area(Area area)
		{
			center = area.center;
			radius = area.radius;
			sqrRadius = radius * radius;
		}
	}
}
