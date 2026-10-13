using System;
using UnityEngine;

namespace Gley.UrbanSystem.Internal
{
	[Serializable]
	public class Area
	{
		public Vector3 Center;

		public float Radius;

		[HideInInspector]
		public float SqrRadius;

		public Area(Vector3 center, float radius)
		{
			Center = center;
			Radius = radius;
			SqrRadius = radius * radius;
		}

		public Area(Area area)
		{
			Center = area.Center;
			Radius = area.Radius;
			SqrRadius = Radius * Radius;
		}
	}
}
