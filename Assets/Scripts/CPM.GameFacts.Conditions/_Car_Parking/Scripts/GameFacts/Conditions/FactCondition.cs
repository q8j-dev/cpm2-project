using System;
using UnityEngine;
using _Car_Parking.Scripts.GameFacts.Base;

namespace _Car_Parking.Scripts.GameFacts.Conditions
{
	[Serializable]
	public sealed class FactCondition : ConditionNode
	{
		[SerializeField]
		private GameFact Fact;

		public FactCondition()
		{
		}

		public FactCondition(GameFact fact)
		{
			Fact = fact;
		}

		public override bool IsMet(IGameFactManager facts)
		{
			return facts.IsTrue(Fact);
		}
	}
}
