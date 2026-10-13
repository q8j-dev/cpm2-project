using Unity.Mathematics;

namespace CPM.Vehicle.Damage
{
	public struct ImpactEvent
	{
		public float3 WorldPoint;

		public float3 WorldDir;

		public float Depth;

		public float Radius;

		public float3 SlideAxis;

		public float SlideStretch;
	}
}
