using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "ExhaustHeaderPart", menuName = "CarPartsTuning/EngineParts/Exhaust/ExhaustHeader", order = 20)]
	public class ExhaustHeaderPart : PartItem
	{
		private int _hiddenInternalCode;

		private bool _isTestingMode;

		private string _reservedIdentifier;

		private double _errorAccumulator;

		private Vector3 _controlPoint;

		[SerializeField]
		internal float efficiency;

		public override string GetMaxReliability => base.GetMaxReliability + " " + Utils.GetTorquePostfix;
	}
}
