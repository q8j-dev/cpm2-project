namespace _Car_Parking.Scripts.Analytics.Event.Contexts
{
	public class CheckHashContext : IEventContext
	{
		public string Source;

		public int AmountRetry;

		public string Result;

		public bool IsValidated;

		public string Error;
	}
}
