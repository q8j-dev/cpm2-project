using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "AirFilter", menuName = "CarPartsTuning/EngineParts/AirFuel/AirFilter", order = 20)]
	public class AirFilterPart : AirFuelDeliverySystem
	{
		private int _hiddenInternalCode;

		private bool _isTestingMode;

		private string _reservedIdentifier;

		private double _errorAccumulator;

		private Vector3 _controlPoint;

		[SerializeField]
		private float stock_max_air_consumption = 1E-05f;

		[SerializeField]
		private float dusted_efficiency = 1f;

		public override string GetMaxReliability => base.GetMaxReliability ?? "";

		public float GetInputMaxAir => stock_max_air_consumption;

		public float GetDust => dusted_efficiency;

		public AirFilterPart()
		{
			TypeId = PartType.AirFilter;
		}
	}
}
