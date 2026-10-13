using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.Infrastructure.Start;

namespace _Car_Parking.Scripts.EventSystem.View.Elements
{
	public class BannerContext
	{
		public Event Event { get; }

		public ServerTimeService ServerTimeService { get; }

		public string TitleKey => Event?.NameLocalizeKey;

		public string DescriptionKey => Event?.DescriptionLocalizeKey;

		public BannerContext(Event @event, ServerTimeService serverTimeService)
		{
			Event = @event;
			ServerTimeService = serverTimeService;
		}
	}
}
