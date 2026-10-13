using System;
using UnityEngine;
using _Car_Parking.Scripts.GameFacts.Base;

namespace _Car_Parking.Scripts.GameFacts.Conditions
{
	[Serializable]
	public sealed class NotCondition : ConditionNode
	{
		[SerializeReference]
		private ConditionNode Inner;

		public NotCondition()
		{
		}

		public NotCondition(ConditionNode inner)
		{
			Inner = inner;
		}

		public override bool IsMet(IGameFactManager facts)
		{
			if (Inner != null)
			{
				return !Inner.IsMet(facts);
			}
			return true;
		}
	}
}
