using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Racing_Scripts.RallyProps
{
	[CreateAssetMenu(menuName = "Rally/Rally Prop Catalog")]
	public class RallyPropCatalogSO : ScriptableObject
	{
		public List<GameObject> Prefabs = new List<GameObject>();

		public GameObject GetPrefab(int index)
		{
			if (index < 0 || index >= Prefabs.Count)
			{
				return null;
			}
			return Prefabs[index];
		}
	}
}
