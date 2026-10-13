using System;
using System.Collections.Generic;
using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning.Instances;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class RotorHousingInstance : PartInstance<RotorHousingPart>
	{
		[Header("Parts")]
		public EccentricShaftInstance EccentricShaft;

		public RotorTrackingGearInstance RotorTrackingGears;

		public RotorInstance Rotors;

		public FlywheelInstance Flywheel;

		private List<PartInstanceBase> _subParts;

		private readonly RotorHousingResult _values;

		private float _wear = 1f;

		public RotorHousingResult GetParams => _values;

		internal float InMax
		{
			get
			{
				float effectiveIntakePortArea = EffectiveIntakePortArea;
				if (_values.IntakePortType != RotorHousingPart.PortType.Peripheral)
				{
					return effectiveIntakePortArea;
				}
				return effectiveIntakePortArea * 1.3f;
			}
		}

		internal float OutMax => EffectiveExhaustPortArea * Rotors.ThermalExpansion;

		internal float EffectiveIntakePortArea => CalculatedIntakePortArea;

		internal float EffectiveExhaustPortArea => CalculatedExhaustPortArea;

		private float CalculatedIntakePortArea
		{
			get
			{
				float num = (float)Math.PI / 4f * Mathf.Pow(_values.IntakePortDiameter, 2f) * (float)RotorTrackingGears.IntakePortsCount * (float)_values.Rotors;
				return Utils.RoundTo(_wear * Rotors.PortEfficiency * num / 1000000f, 7);
			}
		}

		private float CalculatedExhaustPortArea
		{
			get
			{
				float num = (float)Math.PI / 4f * Mathf.Pow(_values.ExhaustPortDiameter, 2f) * (float)RotorTrackingGears.ExhaustPortsCount * (float)_values.Rotors;
				return Utils.RoundTo(_wear * Rotors.PortEfficiency * num / 1000000f, 7);
			}
		}

		public override List<PartInstanceBase> SubParts
		{
			get
			{
				if (_subParts == null || _subParts.Count == 0)
				{
					_subParts = new List<PartInstanceBase> { EccentricShaft, Rotors, RotorTrackingGears, Flywheel };
				}
				return _subParts;
			}
		}

		public RotorHousingInstance(RotorHousingPart part)
		{
			Part = part;
			_values = new RotorHousingResult(Part);
		}

		public override void UpdateWear(Pv value)
		{
			float num = ((value.R / _values.MaxRPM > 1f) ? (value.R / _values.MaxRPM) : 0f);
			float num2 = ((value.I > Part.GetReliability) ? (value.I / Part.GetReliability) : 0f);
			Mileage += value.P + (0.01f * num + 0.5f * num2);
			if (SubParts == null || SubParts.Count == 0)
			{
				return;
			}
			foreach (PartInstanceBase subPart in SubParts)
			{
				subPart?.UpdateWear(value);
			}
		}

		public override void UpdateLoad(Pv value)
		{
			if (value.I > 4012.0247f)
			{
				Mileage += Part.MaxMileage;
			}
			base.UpdateLoad(value);
		}
	}
}
