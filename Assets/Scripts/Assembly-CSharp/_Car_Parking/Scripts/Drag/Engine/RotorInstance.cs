using System;
using CPMEngine;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class RotorInstance : ReciprocatingEnginePartInstance<RotorPart>
	{
		public readonly float CompressionRatioFactor;

		public readonly float PortEfficiency;

		public readonly float ThermalExpansion;

		public RotorInstance(RotorPart part)
		{
			Part = part;
			CompressionRatioFactor = Part.GetCompressionRatioFactor;
			PortEfficiency = Part.PortEfficiency;
			ThermalExpansion = Part.ThermalExpansion;
		}

		public float GetInertia()
		{
			return Utils.KgToInertia(Part.Mass);
		}

		public override void UpdateWear(Pv value)
		{
			float num = ((value.R / Part.maxRPM > 1f) ? (value.R / Part.maxRPM) : 0f);
			float num2 = ((value.I > Part.GetReliability) ? (value.I / Part.GetReliability) : 0f);
			float wearMultiplier = OilStarvation.GetWearMultiplier(value.O);
			Mileage += (value.P + (0.01f * num + 0.4f * num2)) * wearMultiplier;
		}

		public override void UpdateLoad(Pv value)
		{
			if (value.I > 3659.9998f)
			{
				Mileage += Part.MaxMileage;
			}
		}
	}
}
