using System;
using System.Collections.Generic;
using CPMEngine;
using _Car_Parking.Scripts.EngineTuning.Instances;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class IntakeManifoldInstance : AirFuelDeliverySystemInstance<IntakeManifoldPart>
	{
		public AirFilterInstance AirFilter;

		private readonly float _stockEfficiency;

		private List<PartInstanceBase> _subParts;

		public float Efficiency => _stockEfficiency * GetEfficiency(base.Condition, 0.5f);

		public override List<PartInstanceBase> SubParts
		{
			get
			{
				if (_subParts == null || _subParts.Count == 0)
				{
					_subParts = new List<PartInstanceBase> { AirFilter };
				}
				return _subParts;
			}
		}

		public IntakeManifoldInstance(IntakeManifoldPart part)
		{
			Part = part;
			_stockEfficiency = part.GetEfficiency;
		}

		public override void UpdateWear(Pv value)
		{
			Mileage += value.P;
			foreach (PartInstanceBase subPart in SubParts)
			{
				subPart?.UpdateWear(value);
			}
		}

		public override void UpdateLoad(Pv value)
		{
			if (value.I > 3659.9998f)
			{
				Mileage += Part.MaxMileage;
			}
			base.UpdateLoad(value);
		}
	}
}
