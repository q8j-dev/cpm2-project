using UnityEngine;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "IntakeManifold", menuName = "CarPartsTuning/EngineParts/AirFuel/IntakeManifold", order = 23)]
	public class IntakeManifoldPart : AirFuelDeliverySystem
	{
		[SerializeField]
		private float stock_efficiency = 0.8f;

		public float GetEfficiency => Mathf.Clamp01(stock_efficiency);

		public override string GetMaxReliability => base.GetMaxReliability ?? "";
	}
}
