using System;
using CPMEngine;
using UnityEngine;

namespace _Car_Parking.Scripts.EngineTuning.Instances
{
	[Serializable]
	public class PartInstance<TPart> : PartInstanceBase where TPart : PartItem
	{
		public int InstanceId;

		public TPart Part;

		public float Mileage;

		public float Condition
		{
			get
			{
				if (Part == null || Part.MaxMileage <= 0f)
				{
					return 0f;
				}
				float num = Mileage / Part.MaxMileage;
				if (float.IsNaN(num) || float.IsInfinity(num))
				{
					return 0f;
				}
				return Mathf.Clamp(1f - num, 0f, 1f);
			}
		}

		public float Wear => 1f - Condition;

		public float GetEfficiency(float condition, float efficiencyWhenBroken)
		{
			return efficiencyWhenBroken + (1f - efficiencyWhenBroken) * condition;
		}

		public float GetInvEfficiency(float condition, float maxEfficiencyWhenBroken)
		{
			return maxEfficiencyWhenBroken * (1f - condition);
		}

		[ContextMenu("Get Weight")]
		public override float GetWeight()
		{
			float num = 0f;
			if ((bool)Part)
			{
				num = Part.Mass;
			}
			if (SubParts == null || SubParts.Count == 0)
			{
				return num;
			}
			foreach (PartInstanceBase subPart in SubParts)
			{
				if (subPart != null)
				{
					num += subPart.GetWeight();
				}
			}
			return num;
		}

		public override void UpdateWear(Pv value)
		{
			float num = ((value.I > Part.GetReliability) ? (value.I / Part.GetReliability) : 0f);
			if (Part.GetReliability <= 0f)
			{
				num = 0f;
			}
			float num2 = value.P + 0.05f * num;
			if (!float.IsFinite(num2) || Mathf.Abs(num2) > 5f)
			{
				num2 = 0f;
			}
			Mileage += num2;
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
	}
}
