using UnityEngine;

namespace _Car_Parking.Scripts
{
	public class GameObjectStateUpdaterContainer : MonoBehaviour
	{
		[SerializeField]
		private GameObjectStateUpdater[] GameObjectStateUpdaters = default;

		public void UpdateState(bool isOn)
		{
			for (int i = 0; i < GameObjectStateUpdaters.Length; i++)
			{
				if ((bool)GameObjectStateUpdaters[i])
				{
					GameObjectStateUpdaters[i].UpdateState(isOn);
				}
			}
		}
	}
}
