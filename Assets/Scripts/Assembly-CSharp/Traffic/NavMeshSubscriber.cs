using Unity.AI.Navigation;
using UnityEngine;
using _Car_Parking.Scripts.Other.Helpers;

namespace Traffic
{
	public class NavMeshSubscriber : MonoBehaviour
	{
		private NavMeshSurface _surface;

		private void Awake()
		{
			_surface = GetComponent<NavMeshSurface>();
		}

		private void OnEnable()
		{
			if (ScenesHelper.IsFreeDriveSceneActive())
			{
				PeopleTraffic.SubscribeNavMeshSurface(_surface);
			}
		}

		private void OnDisable()
		{
			PeopleTraffic.UnsubscribeNavMeshSurface(_surface);
		}
	}
}
