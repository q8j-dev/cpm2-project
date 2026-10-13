using UnityEngine;
using _Car_Parking.Scripts.EventSystem.Data.Response;

namespace _Car_Parking.Scripts.EventSystem.View
{
	public class EventCollectableHideGameObjectIfCount : EventGameObjectViewBase
	{
		[SerializeField]
		private GameObject[] GameObjects;

		[SerializeField]
		private int CollectCount;

		protected override void UpdateView()
		{
			bool active = true;
			if (_eventService.GetEventByName(_eventType)?.Data is CollectablesEventData collectablesEventData)
			{
				active = collectablesEventData.Items.Count < CollectCount;
			}
			for (int i = 0; i < GameObjects.Length; i++)
			{
				if ((bool)GameObjects[i])
				{
					GameObjects[i].SetActive(active);
				}
			}
		}
	}
}
