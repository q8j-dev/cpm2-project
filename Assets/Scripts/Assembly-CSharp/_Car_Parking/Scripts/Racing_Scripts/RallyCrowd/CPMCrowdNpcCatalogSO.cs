using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Racing_Scripts.RallyCrowd
{
	[CreateAssetMenu(menuName = "Rally/Rally Crowd NPC Catalog")]
	public class CPMCrowdNpcCatalogSO : ScriptableObject
	{
		public List<CPMCrowdNpc> Prefabs = new List<CPMCrowdNpc>();

		public CPMCrowdNpc GetPrefab(int index)
		{
			if (index < 0 || index >= Prefabs.Count)
			{
				return null;
			}
			return Prefabs[index];
		}
	}
}
