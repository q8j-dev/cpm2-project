using System;
using CPMEngine;
using UnityEngine.Serialization;
using _Car_Parking.Scripts.EngineTuning.Instances;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class FrontLSDInstance : PartInstance<FrontLSDPart>, IReturnablePart<LSDValues>
	{
		[FormerlySerializedAs("DynamicTuningValues")]
		public DynamicTuningSettings DynamicTuningSettings;

		public LSDValues GetValue()
		{
			LSDValues lSDValues = new LSDValues
			{
				Type = Part.Type,
				AccelBias = LSDValues.ClampAccelBias(Part.AccelerationBias, Part.Type),
				BrakeBias = LSDValues.ClampBrakeBias(Part.BrakeBias, Part.Type),
				MaxBias = LSDValues.ClampMaxBias(Part.MaxBias, Part.Type),
				TorqueBiasRatio = LSDValues.ClampTorqueBiasRatio(Part.TorqueBiasRatio, Part.Type),
				MaxResistance = LSDValues.ClampMaxResistance(Part.MaxResistance, Part.Type),
				LockPercent = LSDValues.ClampLockPercent(Part.LockPercent, Part.Type)
			};
			if (DynamicTuningSettings.FrontAccelBias >= 1f)
			{
				lSDValues.AccelBias = LSDValues.ClampAccelBias(DynamicTuningSettings.FrontAccelBias, Part.Type);
			}
			if (DynamicTuningSettings.FrontBrakeBias >= 1f)
			{
				lSDValues.BrakeBias = LSDValues.ClampBrakeBias(DynamicTuningSettings.FrontBrakeBias, Part.Type);
			}
			if (DynamicTuningSettings.FrontMaxBias >= 1f)
			{
				lSDValues.MaxBias = LSDValues.ClampMaxBias(DynamicTuningSettings.FrontMaxBias, Part.Type);
			}
			if (DynamicTuningSettings.FrontTorqueBiasRatio >= 1f)
			{
				lSDValues.TorqueBiasRatio = LSDValues.ClampTorqueBiasRatio(DynamicTuningSettings.FrontTorqueBiasRatio, Part.Type);
			}
			if (DynamicTuningSettings.FrontLsdLock >= 0f)
			{
				lSDValues.LockPercent = LSDValues.ClampLockPercent(DynamicTuningSettings.FrontLsdLock, Part.Type);
			}
			return lSDValues;
		}

		public override void UpdateWear(Pv value)
		{
			float num = ((value.R / Part.maxRPM > 1f) ? (value.R / Part.maxRPM) : 0f);
			if (Part.maxRPM < 0f)
			{
				num = 0f;
			}
			float num2 = ((Part.GetReliability < 0f) ? 0f : (value.I / Part.GetReliability));
			Mileage += value.P + (0.002f * num + 0.035f * num2);
		}
	}
}
