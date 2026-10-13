using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "Intercooler", menuName = "CarPartsTuning/EngineParts/Cooling/Intercooler", order = 31)]
	public class IntercoolerPart : PartItem
	{
		[SerializeField]
		private float MinCooling = 10f;

		[SerializeField]
		private float MaxCooling = 50f;

		internal float AirFlowSpeed = 0.1f;

		public float IntercoolingPressure = 100000f;

		public override string GetMaxReliability => base.GetMaxReliability + " " + Utils.GetTorquePostfix;

		public IntercoolerPart()
		{
			TypeId = PartType.Intercooler;
			Mass = 5f;
		}
	}
}
