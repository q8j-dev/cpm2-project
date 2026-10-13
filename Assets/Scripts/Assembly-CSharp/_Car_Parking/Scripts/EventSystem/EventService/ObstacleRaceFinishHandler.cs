using CPM.Fallout;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.GameActivity;

namespace _Car_Parking.Scripts.EventSystem.EventService
{
	[Preserve]
	public sealed class ObstacleRaceFinishHandler : IFalloutFinishHandler
	{
		private readonly IEventService _eventService;

		public GameActivityType ActivityType => GameActivityType.ObstacleRace;

		public ObstacleRaceFinishHandler(IEventService eventService)
		{
			_eventService = eventService;
		}

		public void Handle(FalloutFinishData data)
		{
			FalloutFinishObstacleRaceData obstacleData = data as FalloutFinishObstacleRaceData;
			if (obstacleData == null)
			{
				return;
			}
			_eventService.TryUpdateEvent(EventType.ObstacleRace, (Event currentEvent) =>
			{
				currentEvent.Progress = obstacleData.Progress;
				if (obstacleData.ObstacleRaceEventData != null)
				{
					currentEvent.Data = obstacleData.ObstacleRaceEventData;
				}
				if (currentEvent.Status == EventStatus.InProgress && currentEvent.MaxProgress > 0 && currentEvent.Progress >= currentEvent.MaxProgress)
				{
					currentEvent.Status = EventStatus.Finished;
				}
			});
		}
	}
}
