using UnityEngine;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.Data.Response;
using _Car_Parking.Scripts.EventSystem.EventService;

namespace _Car_Parking.Scripts.EventSystem.View
{
	public class EventCollectablesParent : MonoBehaviour, IEventGameObjectInit
	{
		[SerializeField]
		private EventCollectableObject[] CollectableObjects;

		[SerializeField]
		private bool showIfClaim;

		private IEventService _eventService;

		[UnityEngine.Scripting.Preserve]
		[Inject]
		public void Construct(IEventService eventService)
		{
			_eventService = eventService;
		}

		public void Init(EventType eventType)
		{
			_Car_Parking.Scripts.EventSystem.Data.Event eventByName = _eventService.GetEventByName(eventType);
			if (eventByName == null || !(eventByName.Data is CollectablesEventData))
			{
				return;
			}
			CollectablesEventData collectablesEventData = eventByName.Data as CollectablesEventData;
			for (int i = 0; i < CollectableObjects.Length; i++)
			{
				if ((bool)CollectableObjects[i])
				{
					bool flag = collectablesEventData.HaveClaimed(CollectableObjects[i].CollectableID);
					CollectableObjects[i].gameObject.SetActive(flag == showIfClaim);
					if (CollectableObjects[i].gameObject.activeSelf)
					{
						CollectableObjects[i].Init(eventType);
					}
				}
			}
		}
	}
}
