using UnityEngine;

namespace Gley.TrafficSystem
{
	[CreateAssetMenu(fileName = "VehiclePool", menuName = "TrafficSystem/Vehicle Pool", order = 1)]
	public class VehiclePool : ScriptableObject
	{
		public CarType[] trafficCars;

		public VehiclePool()
		{
			CarType carType = new CarType();
			trafficCars = new CarType[1] { carType };
		}
	}
}
