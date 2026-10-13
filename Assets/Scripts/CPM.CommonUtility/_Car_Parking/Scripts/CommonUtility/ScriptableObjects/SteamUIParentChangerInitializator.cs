using UnityEngine;

namespace _Car_Parking.Scripts.CommonUtility.ScriptableObjects
{
	public class SteamUIParentChangerInitializator : MonoBehaviour
	{
		[SerializeField]
		private SteamUIParentChanger steamUIParentChanger;

		private void Start()
		{
			if ((bool)steamUIParentChanger)
			{
				steamUIParentChanger.ApplyAllChanges();
			}
		}
	}
}
