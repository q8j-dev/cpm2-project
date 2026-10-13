using System;
using System.Collections.Generic;
using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning.Instances;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class BlockInstance : PartInstance<BlockPart>
	{
		[Header("Parts")]
		public CrankshaftInstance Crankshaft;

		public ConnectionRodInstance ConnectionRod;

		public PistonInstance Pistons;

		public FlywheelInstance Flywheel;

		private List<PartInstanceBase> _subParts;

		private readonly BlockResult _values;

		public float CrankCenter => _values.CrankCenter;

		public float maxRPM => _values.maxRPM;

		public float OilCapacity => _values.OilCapacity;

		public override List<PartInstanceBase> SubParts
		{
			get
			{
				if (_subParts == null || _subParts.Count == 0)
				{
					_subParts = new List<PartInstanceBase> { Crankshaft, Pistons, ConnectionRod, Flywheel };
				}
				return _subParts;
			}
		}

		public BlockInstance(BlockPart part)
		{
			Part = part;
			_values = new BlockResult(Part);
		}

		public override void UpdateWear(Pv value)
		{
			float num = ((value.R / _values.maxRPM > 1f) ? (value.R / _values.maxRPM) : 0f);
			float num2 = ((value.I > Part.GetReliability) ? (value.I / Part.GetReliability) : 0f);
			float wearMultiplier = OilStarvation.GetWearMultiplier(value.O);
			float num3 = (value.P + (0.01f * num + 0.5f * num2)) * wearMultiplier;
			if (!float.IsFinite(num3) || Mathf.Abs(num3) > 5f)
			{
				num3 = 0f;
			}
			Mileage += num3;
			if (!float.IsFinite(Mileage) && Part != null && Part.MaxMileage > 0f)
			{
				Mileage = Part.MaxMileage;
			}
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
			if (float.IsFinite(value.I) && value.I < 50000f && value.I > 4012.0247f)
			{
				Mileage += Part.MaxMileage;
			}
			base.UpdateLoad(value);
		}
	}
}
