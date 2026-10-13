using UnityEngine;

namespace _Car_Parking.Scripts
{
	public static class LayerViaName
	{
		public static readonly int CarLayer = LayerMask.NameToLayer("car");

		public static readonly int PersonLayer = LayerMask.NameToLayer("DriverPerson");

		public static readonly int WheelLayer = LayerMask.NameToLayer("Wheel");

		public static readonly int RoadLayer = LayerMask.NameToLayer("Road");

		public static readonly int LoadLayer = LayerMask.NameToLayer("Load");

		public static readonly int OnlineLoadLayer = LayerMask.NameToLayer("OnlineLoad");

		public static readonly int PedestrainLayer = LayerMask.NameToLayer("pedestrain");

		public static readonly int IgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");

		public static readonly int TransparentFX = LayerMask.NameToLayer("TransparentFX");

		public static readonly int CarBody = LayerMask.NameToLayer("CarBody");

		public static readonly int ShortDistance = LayerMask.NameToLayer("short distance");

		public static readonly int Map = LayerMask.NameToLayer("map");

		public static readonly int LayerWater = LayerMask.NameToLayer("Water");

		public static readonly int DefaultLayer = LayerMask.NameToLayer("Default");

		public static readonly int TrailerLayer = LayerMask.NameToLayer("Trailer");

		public static readonly int TunnelLayer = LayerMask.NameToLayer("Tunnel");

		public static readonly int TerrainLayer = LayerMask.NameToLayer("Terrain");

		public static readonly int GarageLayer = LayerMask.NameToLayer("Garage");

		public static readonly int TrafficLayer = LayerMask.NameToLayer("traffic car");

		public static readonly int ParktronicLayer = LayerMask.NameToLayer("parktronic");

		public static readonly int DriverPersonLayer = LayerMask.NameToLayer("DriverPerson");
	}
}
