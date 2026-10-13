using _Car_Parking.Scripts.EventSystem.Data;

namespace _Car_Parking.Scripts.SummaryOfEvents.ItemViews
{
	public interface IItemEventView
	{
		void UpdateView(_Car_Parking.Scripts.EventSystem.Data.Event @event);
	}
}
