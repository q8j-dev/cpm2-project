using VContainer;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.Data.Response;
using _Car_Parking.Scripts.EventSystem.EventService;

namespace _Car_Parking.Scripts.SummaryOfEvents.ItemViews.Event
{
	public class ItemEventObstacleRace : ItemEvent
	{
		private IEventService _eventService;

		[Inject]
		public void Construct(IEventService eventService)
		{
			_eventService = eventService;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (_eventService != null)
			{
				_eventService.OnEventUpdated += EventUpdated;
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (_eventService != null)
			{
				_eventService.OnEventUpdated -= EventUpdated;
			}
		}

		private void EventUpdated(EventType eventType)
		{
			if (eventType == base.EventType && base.Event != null)
			{
				ApplyClaimState(base.Event);
			}
		}

		protected override void ApplyClaimState(_Car_Parking.Scripts.EventSystem.Data.Event @event)
		{
			ObstacleRaceEventData obstacleRaceEventData = @event.Data as ObstacleRaceEventData;
			bool flag = @event.Status == EventStatus.Claimed;
			bool claimVisible = !flag && obstacleRaceEventData != null && obstacleRaceEventData.Claimable > 0;
			SetClaimVisible(claimVisible);
			SetClaimedVisible(flag);
			RefreshProgress();
		}

		protected override (int Current, int Max) GetProgressValues(_Car_Parking.Scripts.EventSystem.Data.Event @event)
		{
			ObstacleRaceEventData obstacleRaceEventData = @event.Data as ObstacleRaceEventData;
			int[] array = obstacleRaceEventData?.Steps;
			if (array == null || array.Length == 0)
			{
				return base.GetProgressValues(@event);
			}
			bool claimable = @event.Status != EventStatus.Claimed && obstacleRaceEventData.Claimable > 0;
			return ObstacleRaceStepProgress.Calculate(array, @event.Progress, claimable);
		}
	}
}
