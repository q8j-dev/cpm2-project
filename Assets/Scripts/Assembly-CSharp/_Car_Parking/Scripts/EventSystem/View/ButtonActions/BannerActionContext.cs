using System;
using _Car_Parking.Scripts.EventSystem.Data;

namespace _Car_Parking.Scripts.EventSystem.View.ButtonActions
{
	public class BannerActionContext
	{
		public Event Event { get; }

		public Action CloseBanner { get; }

		public BannerActionContext(Event @event, Action closeBanner)
		{
			Event = @event;
			CloseBanner = closeBanner;
		}
	}
}
