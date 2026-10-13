using UnityEngine;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Scripts.EventSystem.EventService;

namespace _Car_Parking.Scripts.EventSystem.View
{
	public abstract class EventGameObjectViewBase : MonoBehaviour, IEventGameObjectInit
	{
		protected IEventService _eventService;

		[SerializeField]
		protected EventType _eventType = EventType.None;

		[Inject]
		[UnityEngine.Scripting.Preserve]
		public void Construct(IEventService eventService)
		{
			_eventService = eventService;
		}

		public void Init(EventType eventType)
		{
			_eventType = eventType;
			UpdateView();
		}

		protected abstract void UpdateView();

		private void OnEnable()
		{
			_eventService.OnEventsRetrieved += EventsRetrievedCallback;
			UpdateView();
		}

		private void OnDisable()
		{
			_eventService.OnEventsRetrieved -= EventsRetrievedCallback;
		}

		private void EventsRetrievedCallback()
		{
			UpdateView();
		}
	}
}
