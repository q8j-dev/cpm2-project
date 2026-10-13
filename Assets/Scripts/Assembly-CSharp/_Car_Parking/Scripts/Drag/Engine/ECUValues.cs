using System.Collections.Generic;
using CPMEngine;

namespace _Car_Parking.Scripts.Drag.Engine
{
	public struct ECUValues
	{
		public float RpmIdle;

		public float RpmIdleMin;

		public float RpmIdleMax;

		public float RpmLimit;

		public float RpmLimitMax;

		public List<FuelMap> FuelMaps;
	}
}
