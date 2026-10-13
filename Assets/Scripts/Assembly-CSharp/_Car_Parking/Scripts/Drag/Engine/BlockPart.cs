using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "Block", menuName = "CarPartsTuning/EngineParts/Block", order = 5)]
	public class BlockPart : PartItem
	{
		[Tooltip("Number of cylinders")]
		[Header("Params")]
		[SerializeField]
		[Range(1f, 16f)]
		internal int Cylinders = 1;

		[SerializeField]
		[Range(100f, 300f)]
		internal float CrankCenter2CylinderTop;

		[Range(3000f, 19000f)]
		[SerializeField]
		[Tooltip("Maximum allowable RPM")]
		internal float maxRPM;

		[Range(1f, 46f)]
		[SerializeField]
		internal float OilCapacity = 4f;

		public override string GetMaxReliability => base.GetMaxReliability + " " + Utils.GetTorquePostfix;
	}
}
