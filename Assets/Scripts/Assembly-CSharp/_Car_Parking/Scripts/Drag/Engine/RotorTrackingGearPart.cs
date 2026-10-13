using UnityEngine;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "RotorTrackingGear", menuName = "CarPartsTuning/EngineParts/RotorParts/RotorTrackingGear", order = 21)]
	public class RotorTrackingGearPart : ReciprocatingEnginePart
	{
		[Tooltip("Number of intake ports per rotor")]
		[Range(1f, 4f)]
		[SerializeField]
		internal int IntakePortsPerRotor = 2;

		[Tooltip("Number of exhaust ports per rotor")]
		[SerializeField]
		[Range(1f, 3f)]
		internal int ExhaustPortsPerRotor = 1;

		public bool GetValue(float condition)
		{
			return condition > 0.5f;
		}
	}
}
