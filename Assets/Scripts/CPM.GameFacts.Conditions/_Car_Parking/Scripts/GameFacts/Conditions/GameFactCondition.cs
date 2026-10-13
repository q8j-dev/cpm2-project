using UnityEngine;
using _Car_Parking.Scripts.GameFacts.Base;

namespace _Car_Parking.Scripts.GameFacts.Conditions
{
	[CreateAssetMenu(fileName = "GameFactCondition", menuName = "GameFacts/Condition")]
	public sealed class GameFactCondition : ScriptableObject
	{
		[SerializeReference]
		private ConditionNode Root;

		public bool IsMet(IGameFactManager facts)
		{
			if (Root != null)
			{
				return Root.IsMet(facts);
			}
			return true;
		}
	}
}
