namespace _Car_Parking.Scripts
{
	public static class LayerBitmask
	{
		public static int CarLayer = 1 << LayerViaName.CarLayer;

		public static int PedestrainLayer = 1 << LayerViaName.PedestrainLayer;

		public static int RoadLayer = 1 << LayerViaName.RoadLayer;

		public static int DefaultLayer = 1 << LayerViaName.DefaultLayer;
	}
}
