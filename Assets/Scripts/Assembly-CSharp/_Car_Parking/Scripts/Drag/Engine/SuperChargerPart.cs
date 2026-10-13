using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "SuperCharger", menuName = "CarPartsTuning/EngineParts/Chargers/SuperCharger", order = 22)]
	public class SuperChargerPart : ChargerPart
	{
		internal float PulleyDiameter = 1f;

		internal float PulleyWidth = 1f;

		internal float DefaultRpmTurboMul = 1f;

		public SuperChargerPart()
		{
			TypeId = PartType.SuperCharger;
			Mass = 14f;
		}
	}
}
