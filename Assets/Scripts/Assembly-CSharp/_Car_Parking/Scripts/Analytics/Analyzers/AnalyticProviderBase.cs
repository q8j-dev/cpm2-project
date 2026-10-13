using _Car_Parking.Scripts.Analytics.Event;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	public abstract class AnalyticProviderBase
	{
		protected AnalyticEventFactory AnalyticEventFactory { get; }

		public AnalyticProviderBase()
		{
		}

		protected AnalyticProviderBase(AnalyticEventFactory analyticEventFactory)
		{
			AnalyticEventFactory = analyticEventFactory;
		}
	}
}
