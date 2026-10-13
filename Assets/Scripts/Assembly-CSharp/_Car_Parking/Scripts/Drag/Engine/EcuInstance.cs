using System;
using System.Collections.Generic;
using CPMEngine;
using _Car_Parking.Scripts.EngineTuning.Instances;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class EcuInstance : PartInstance<ECUPart>
	{
		private readonly float _idleRpm;

		private readonly float _idleMinRpm;

		private readonly float _idleMaxRpm;

		private readonly float _redlineRpm;

		private readonly float _limitMaxRpm;

		private readonly List<FuelMap> _fuelMaps;

		public float GetIdleRPM => _idleRpm;

		public float GetMinIdleRPM => _idleMinRpm;

		public float GetMaxIdleRPM => _idleMaxRpm;

		public float GetLimitRPM => _redlineRpm;

		public float GetMaxLimitRPM => _limitMaxRpm;

		public List<FuelMap> GetFuelMap => _fuelMaps;

		public EcuInstance(ECUPart part)
		{
			Part = part;
			ECUValues getEcuValues = Part.GetEcuValues;
			_idleRpm = getEcuValues.RpmIdle;
			_idleMinRpm = getEcuValues.RpmIdleMin;
			_idleMaxRpm = getEcuValues.RpmIdleMax;
			_redlineRpm = getEcuValues.RpmLimit;
			_limitMaxRpm = getEcuValues.RpmLimitMax;
			_fuelMaps = getEcuValues.FuelMaps;
		}

		public override void UpdateWear(Pv value)
		{
			Mileage += value.P;
		}
	}
}
