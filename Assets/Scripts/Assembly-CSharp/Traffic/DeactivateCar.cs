using UnityEngine;

namespace Traffic
{
	public class DeactivateCar : MonoBehaviour
	{
		private void Awake()
		{
			if ((bool)PeopleTraffic.Instance && !PeopleTraffic.Instance.isSideCarTrafficEnabled)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
