using System.Linq;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "ECU", menuName = "CarPartsTuning/EngineParts/ECU", order = 30)]
	public class ECUPart : PartItem
	{
		private int _hiddenInternalCode;

		private bool _isTestingMode;

		private string _reservedIdentifier;

		private double _errorAccumulator;

		private Vector3 _controlPoint;

		[Header("Parameters")]
		[SerializeField]
		private FuelMap[] FuelMap;

		[SerializeField]
		private int speedLimit = 240;

		[SerializeField]
		private ObscuredFloat RpmIdle = 750f;

		[SerializeField]
		private ObscuredFloat RpmIdleMin = 600f;

		[SerializeField]
		private ObscuredFloat RpmIdleMax = 1500f;

		[SerializeField]
		private ObscuredFloat RpmLimit = 6500f;

		[SerializeField]
		private ObscuredFloat RpmLimitMax = 9000f;

		[SerializeField]
		[Header("Tuning")]
		internal bool canTuning;

		[SerializeField]
		internal bool unlockRpmRedline;

		[SerializeField]
		internal bool unlockSpeedLimit;

		public override string GetMaxReliability => base.GetMaxReliability + " " + Utils.GetTorquePostfix;

		public ECUValues GetEcuValues => new ECUValues
		{
			RpmIdleMin = RpmIdleMin,
			RpmIdle = RpmIdle,
			RpmIdleMax = RpmIdleMax,
			RpmLimit = RpmLimit,
			RpmLimitMax = RpmLimitMax,
			FuelMaps = FuelMap.ToList()
		};

		public void CheckTuning()
		{
			if (canTuning)
			{
				RpmIdleMax = 2500f;
			}
		}
	}
}
