using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "ThrottleValve", menuName = "CarPartsTuning/EngineParts/AirFuel/ThrottleValve", order = 23)]
	public class ThrottleValvePart : PartItem
	{
		[SerializeField]
		private float diameter = 54f;

		public float GetDiameter => diameter;

		public override string GetMaxReliability => base.GetMaxReliability + " " + Utils.GetTorquePostfix;
	}
}
