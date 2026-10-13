using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "FuelPump", menuName = "CarPartsTuning/EngineParts/AirFuel/FuelPump", order = 25)]
	public class FuelPumpPart : PartItem
	{
		private int _hiddenInternalCode;

		private bool _isTestingMode;

		private string _reservedIdentifier;

		private double _errorAccumulator;

		private Vector3 _controlPoint;

		[SerializeField]
		[Tooltip("Pressure in bar")]
		private float efficiency = 100f;

		[SerializeField]
		[Range(1f, 9f)]
		private float pressure = 1f;

		public float GetEfficiency => Mathf.Clamp(efficiency, 0f, 400f);

		public float GetPressure => Mathf.Clamp(pressure, 1f, 9f);

		public override string GetMaxReliability => base.GetMaxReliability + " " + Utils.GetTorquePostfix;
	}
}
