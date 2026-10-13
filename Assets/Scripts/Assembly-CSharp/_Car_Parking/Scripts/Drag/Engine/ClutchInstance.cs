using System;
using CPMEngine;
using UnityEngine;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class ClutchInstance : ReciprocatingEnginePartInstance<ClutchPart>
	{
		private readonly float _maxInputTorque;

		private readonly float _newGearshiftDelay;

		private readonly float _partReliability;

		private float _outputTorque;

		private float _outputGearShiftDelay;

		private float _inertia;

		private int _hiddenInternalCode;

		private bool _isTestingMode;

		private string _reservedIdentifier;

		private double _errorAccumulator;

		private Vector3 _controlPoint;

		public float GetMaxTorque => _outputTorque;

		public float GetGearShiftDelay => _outputGearShiftDelay;

		public float GetInertia => _inertia;

		public ClutchInstance(ClutchPart part)
		{
			Part = part;
			_maxInputTorque = Part.GetMaxF;
			_newGearshiftDelay = Part.GetShiftDelay;
			_partReliability = Part.GetReliability;
			_outputTorque = _maxInputTorque;
			_outputGearShiftDelay = _newGearshiftDelay;
			_inertia = Part.GetInertia();
		}

		public override void UpdateVariables()
		{
			float num = 1f + GetInvEfficiency(base.Condition, 0.3f);
			_outputTorque = Utils.RoundTo(_maxInputTorque * GetEfficiency(base.Condition, 0.4f), 0);
			_outputGearShiftDelay = Utils.RoundTo(num * _newGearshiftDelay, 3);
		}

		public override void UpdateWear(Pv value)
		{
			UpdateVariables();
			float num = ((value.I > _partReliability) ? (value.I / _partReliability) : 0f);
			Mileage += value.P + 0.5f * num;
		}

		public override void UpdateLoad(Pv value)
		{
			if (value.I > 3659.9998f || _partReliability < 0f)
			{
				Mileage += Part.MaxMileage;
			}
		}
	}
}
