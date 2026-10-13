using System.Collections.Generic;
using CPM.PlayerData;
using VContainer;
using _Car_Parking.Scripts.CDN;

namespace _Car_Parking.Scripts.EventSystem.EventService
{
	[Preserve]
	internal class EventCdnPrefetcherGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EventCdnPrefetcher eventCdnPrefetcher = new EventCdnPrefetcher((IEventService)resolver.ResolveOrParameter(typeof(IEventService), "eventService", parameters), (ICDNDownloader)resolver.ResolveOrParameter(typeof(ICDNDownloader), "cdnDownloader", parameters), (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters));
			Inject(eventCdnPrefetcher, resolver, parameters);
			return eventCdnPrefetcher;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
