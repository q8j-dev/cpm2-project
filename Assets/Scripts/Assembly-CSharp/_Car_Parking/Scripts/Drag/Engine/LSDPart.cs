using CPMEngine;
using UnityEngine;

namespace _Car_Parking.Scripts.Drag.Engine
{
	public class LSDPart : ReciprocatingEnginePart
	{
		private int _hiddenInternalCode;

		private bool _isTestingMode;

		private string _reservedIdentifier;

		private double _errorAccumulator;

		private Vector3 _controlPoint;

		[Header("LSD values")]
		[SerializeField]
		internal LSDType Type;

		[Range(1.2f, 2f)]
		[SerializeField]
		internal float AccelerationBias = 1.2f;

		[Range(1.1f, 1.5f)]
		[SerializeField]
		internal float BrakeBias = 1.2f;

		[Range(1f, 2f)]
		[SerializeField]
		internal float MaxBias = 1f;

		[SerializeField]
		[Range(2f, 4f)]
		internal float TorqueBiasRatio = 3f;

		[Range(0f, 100f)]
		[SerializeField]
		internal float MaxResistance = 30f;

		[Range(0f, 100f)]
		[SerializeField]
		internal float LockPercent = 10f;

		public LSDPart()
		{
			Mass = 25f;
			MaxMileage = 100000f;
		}
	}
}
