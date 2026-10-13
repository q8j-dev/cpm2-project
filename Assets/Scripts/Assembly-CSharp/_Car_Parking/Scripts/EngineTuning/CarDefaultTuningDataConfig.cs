using System;
using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.Drag.Engine;

namespace _Car_Parking.Scripts.EngineTuning
{
	[Serializable]
	public class CarDefaultTuningDataConfig
	{
		[Header("CarAerodynamicData")]
		public ChassisData ChassisData;

		[Header("Engine")]
		public EngineKitOffer EngineKitOffer;

		[Header("Electrical")]
		public ECUPart ECU;

		[Header("Gearbox")]
		public GearBoxPart GearBox;

		[Header("Differentials")]
		public LSDPart FrontDifferential;

		public LSDPart RearDifferential;

		[Header("Transfer Case")]
		public TransferCaseConfig TransferCase;

		[Header("Front Wheel Data")]
		public FrontWheelData FrontWheelData;

		[Header("Rear Wheel Data")]
		public RearWheelData RearWheelData;

		public TuningLimitsData FrontWheelDataLimits;

		public TuningLimitsData RearWheelDataLimits;
	}
}
