using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "ExhaustPipe", menuName = "CarPartsTuning/EngineParts/Exhaust/ExhaustPipe", order = 21)]
	public class ExhaustPipePart : PartItem
	{
		private int _hiddenInternalCode;

		private bool _isTestingMode;

		private string _reservedIdentifier;

		private double _errorAccumulator;

		private Vector3 _controlPoint;

		[SerializeField]
		internal float diameter = 56f;

		public override string GetMaxReliability => base.GetMaxReliability + " " + Utils.GetTorquePostfix;
	}
}
