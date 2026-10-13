using System;
using UnityEngine;

namespace Service
{
	[Serializable]
	public struct Point
	{
		public Vector3 position;

		public string destinationName;

		public Point(Vector3 position, string name)
		{
			this.position = position;
			destinationName = name;
		}
	}
}
