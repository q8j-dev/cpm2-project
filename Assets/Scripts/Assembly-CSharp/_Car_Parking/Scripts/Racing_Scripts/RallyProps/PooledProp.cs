using UnityEngine;

namespace _Car_Parking.Scripts.Racing_Scripts.RallyProps
{
	public class PooledProp : MonoBehaviour
	{
		public int PrefabIndex { get; private set; }

		public void SetIndex(int index)
		{
			PrefabIndex = index;
		}
	}
}
