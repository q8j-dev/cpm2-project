using UnityEngine;

namespace _Car_Parking.Scripts
{
	public abstract class GameObjectStateUpdater : MonoBehaviour
	{
		public abstract bool IsOn { get; }

		public abstract void UpdateState(bool isOn);

		public void RevertState()
		{
			UpdateState(!IsOn);
		}
	}
}
