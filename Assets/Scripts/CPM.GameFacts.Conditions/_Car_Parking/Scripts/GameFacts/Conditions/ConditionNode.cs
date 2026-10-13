using System;
using _Car_Parking.Scripts.GameFacts.Base;

namespace _Car_Parking.Scripts.GameFacts.Conditions
{
	[Serializable]
	public abstract class ConditionNode
	{
		public abstract bool IsMet(IGameFactManager facts);
	}
}
