using System;
using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.GameFacts.Base;

namespace _Car_Parking.Scripts.GameFacts.Conditions
{
	[Serializable]
	public sealed class AllCondition : ConditionNode
	{
		[SerializeReference]
		private List<ConditionNode> Items;

		public AllCondition()
		{
		}

		public AllCondition(params ConditionNode[] items)
		{
			Items = new List<ConditionNode>(items);
		}

		public override bool IsMet(IGameFactManager facts)
		{
			if (Items == null)
			{
				return true;
			}
			for (int i = 0; i < Items.Count; i++)
			{
				if (Items[i] != null && !Items[i].IsMet(facts))
				{
					return false;
				}
			}
			return true;
		}
	}
}
