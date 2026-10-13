using System;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event
{
	[Preserve]
	public class AnalyticEventFactory
	{
		private readonly CPMAnalytics _analytics;

		public AnalyticEventFactory(CPMAnalytics analytics)
		{
			_analytics = analytics;
		}

		public TEvent CreateEvent<TEvent, TContext>(TContext context) where TEvent : DefaultEvent<TContext> where TContext : IEventContext
		{
			return (TEvent)Activator.CreateInstance(typeof(TEvent), _analytics, context);
		}
	}
}
