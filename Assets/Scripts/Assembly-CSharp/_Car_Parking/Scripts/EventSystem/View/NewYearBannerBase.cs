using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.EventSystem.Data;

namespace _Car_Parking.Scripts.EventSystem.View
{
	public class NewYearBannerBase : EventView
	{
		public override void Init(Event @event)
		{
			base.Init(@event);
			SetupBanner().Forget();
		}
	}
}
