using System;
using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.GameFacts.Base;

namespace _Car_Parking.Scripts.GameFacts.Conditions
{
	[Serializable]
	public sealed class AnyCondition : ConditionNode
	{
		[SerializeReference]
		private List<ConditionNode> Items;

		public AnyCondition()
		{
		}

		public AnyCondition(params ConditionNode[] items)
		{
			Items = new List<ConditionNode>(items);
		}

		public override bool IsMet(IGameFactManager facts)
		{
			if (Items == null)
			{
				return false;
			}
			for (int i = 0; i < Items.Count; i++)
			{
				if (Items[i] != null && Items[i].IsMet(facts))
				{
					return true;
				}
			}
			return false;
		}
	}
}
