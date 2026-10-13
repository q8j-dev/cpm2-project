using Unity.Mathematics;

namespace Gley.TrafficSystem
{
	public struct TargetSpeedPoint
	{
		public float3 StopPosition { get; set; }

		public string Name { get; set; }

		public float TargetSpeed { get; set; }

		public float DistanceToStop { get; set; }

		public TargetSpeedPoint(float3 stopPosition, float targetSpeed, float distanceToStop, string name)
		{
			StopPosition = stopPosition;
			TargetSpeed = targetSpeed;
			DistanceToStop = distanceToStop;
			Name = name;
		}
	}
}
