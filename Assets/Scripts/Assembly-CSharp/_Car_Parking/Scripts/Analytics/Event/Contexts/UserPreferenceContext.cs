using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Event.Contexts
{
	public class UserPreferenceContext : IEventContext
	{
		public UserPreferenceType UserPreferenceAction;

		public string OldValue;

		public string NewValue;
	}
}
