using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.EventSystem.Data.Response;

namespace _Car_Parking.Scripts.EventSystem.EventService.Processes
{
	public abstract class RewardProcessor
	{
		protected readonly EventsAnalytic EventsAnalytic;

		protected RewardProcessor(EventsAnalytic eventsAnalytic)
		{
			EventsAnalytic = eventsAnalytic;
		}

		public abstract UniTask Execute(RewardResponseBase responseBase, string eventId);
	}
}
