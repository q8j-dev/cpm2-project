using System;
using System.Collections.Generic;
using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.EngineTuning.Instances
{
	[Serializable]
	public class PartInstanceBase : IWearable
	{
		public virtual List<PartInstanceBase> SubParts { get; }

		public virtual void UpdateVariables()
		{
		}

		public virtual void UpdateWear(Pv value)
		{
			if (SubParts == null || SubParts.Count == 0)
			{
				return;
			}
			foreach (PartInstanceBase subPart in SubParts)
			{
				subPart?.UpdateWear(value);
			}
		}

		public virtual void UpdateLoad(Pv value)
		{
			if (SubParts == null || SubParts.Count == 0)
			{
				return;
			}
			foreach (PartInstanceBase subPart in SubParts)
			{
				subPart?.UpdateLoad(value);
			}
		}

		[ContextMenu("Get Weight")]
		public virtual float GetWeight()
		{
			float num = 0f;
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
	}
}
