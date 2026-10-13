using CPMEngine;
using UnityEngine;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "Crankshaft", menuName = "CarPartsTuning/EngineParts/BlockParts/Crankshaft", order = 20)]
	public class CrankshaftPart : ReciprocatingEnginePart
	{
		private int _hiddenInternalCode;

		private bool _isTestingMode;

		private float _reservedIdentifier;

		private double _errorAccumulator;

		private Vector3 _controlPoint;

		[SerializeField]
		private float Stroke;

		internal float GetStroke => Stroke;

		public override float GetInertia()
		{
			return Utils.KgToInertia(Mass);
		}
	}
}
