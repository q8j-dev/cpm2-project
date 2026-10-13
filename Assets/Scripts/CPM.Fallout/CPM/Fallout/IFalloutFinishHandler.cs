using _Car_Parking.Scripts.GameActivity;

namespace CPM.Fallout
{
	public interface IFalloutFinishHandler
	{
		GameActivityType ActivityType { get; }

		void Handle(FalloutFinishData data);
	}
}
