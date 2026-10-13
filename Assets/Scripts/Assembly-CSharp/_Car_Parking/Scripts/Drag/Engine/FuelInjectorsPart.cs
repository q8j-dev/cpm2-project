using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "FuelInjectors", menuName = "CarPartsTuning/EngineParts/AirFuel/FuelInjectors", order = 24)]
	public class FuelInjectorsPart : AirFuelDeliverySystem
	{
		[SerializeField]
		private float flowRate = 240f;

		public float GetPerformance => Mathf.Clamp(flowRate, 0f, 1000f) * 0.74f / 60f;

		public FuelInjectorsPart()
		{
			TypeId = PartType.FuelInjectors;
			Mass = 1f;
		}
	}
}
