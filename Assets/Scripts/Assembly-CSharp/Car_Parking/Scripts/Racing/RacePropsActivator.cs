using UnityEngine;
using _Car_Parking;
using _Car_Parking.Scripts.Infrastructure;

namespace Car_Parking.Scripts.Racing
{
	public class RacePropsActivator : MonoBehaviour
	{
		[SerializeField]
		private GameObject[] PropsForActivate;

		[SerializeField]
		private GameObject[] PropsForDeactivate;

		private void Start()
		{
			GlobalEventSystem.OnActivateInSceneProps += RaceStartedHandler;
		}

		private void RaceStartedHandler(bool state)
		{
			GameObject[] propsForActivate;
			if (FlagsStorage.IsFreeDriveMode)
			{
				propsForActivate = PropsForActivate;
				foreach (GameObject gameObject in propsForActivate)
				{
					if ((bool)gameObject)
					{
						gameObject.SetActive(state);
					}
				}
			}
			propsForActivate = PropsForDeactivate;
			foreach (GameObject gameObject2 in propsForActivate)
			{
				if ((bool)gameObject2)
				{
					gameObject2.SetActive(!state);
				}
			}
		}

		private void OnDestroy()
		{
			GlobalEventSystem.OnActivateInSceneProps -= RaceStartedHandler;
		}
	}
}
