using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "RotorHousing", menuName = "CarPartsTuning/EngineParts/RotorHousingPart", order = 5)]
	public class RotorHousingPart : PartItem
	{
		public enum PortType
		{
			Side = 0,
			Peripheral = 1
		}

		public enum PortShape
		{
			Rectangular = 0,
			Round = 1
		}

		[Tooltip("Number of rotors")]
		[SerializeField]
		[Header("Rotor Housing Params")]
		[Range(1f, 4f)]
		internal int Rotors = 1;

		[Tooltip("Generating radius (R) - distance from center to rotor tip")]
		[Range(50f, 200f)]
		[SerializeField]
		internal float GeneratingRadius = 100f;

		[SerializeField]
		[Tooltip("Housing width - chamber depth")]
		[Range(60f, 200f)]
		internal float HousingWidth = 80f;

		[Tooltip("Maximum allowable RPM")]
		[SerializeField]
		[Range(3000f, 25000f)]
		internal float maxRPM = 9000f;

		[Range(1f, 10f)]
		[SerializeField]
		[Tooltip("Oil capacity per rotor chamber")]
		internal float OilCapacity = 2f;

		[SerializeField]
		[Tooltip("Cooling jacket capacity")]
		[Range(2f, 20f)]
		internal float CoolantCapacity = 5f;

		[Header("Port Configuration")]
		[SerializeField]
		internal PortType IntakePortType;

		[SerializeField]
		internal float IntakePortDiameter = 10f;

		[SerializeField]
		internal PortType ExhaustPortType;

		[SerializeField]
		internal float ExhaustPortDiameter = 10f;

		[SerializeField]
		[Tooltip("Port timing - degrees of eccentric shaft rotation")]
		[Range(0f, 360f)]
		internal float IntakeOpenDuration = 180f;

		[SerializeField]
		[Range(0f, 360f)]
		internal float IntakeCloseDuration = 180f;

		[Range(0f, 360f)]
		[SerializeField]
		internal float ExhaustOpenDuration = 180f;

		[SerializeField]
		[Range(0f, 360f)]
		internal float ExhaustCloseDuration = 180f;

		public override string GetMaxReliability => base.GetMaxReliability + " " + Utils.GetTorquePostfix;

		public float GetIntakeDuration()
		{
			return (IntakeCloseDuration - IntakeOpenDuration) / 360f;
		}

		public float GetExhaustDuration()
		{
			return (ExhaustCloseDuration - ExhaustOpenDuration) / 360f;
		}
	}
}
