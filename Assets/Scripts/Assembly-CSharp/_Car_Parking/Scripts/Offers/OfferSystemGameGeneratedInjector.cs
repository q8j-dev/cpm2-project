using System.Collections.Generic;
using CPM.PlayerData;
using VContainer;
using _Car_Parking.Scripts.CDN;

namespace _Car_Parking.Scripts.Offers
{
	[Preserve]
	internal class OfferSystemGameGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			OfferSystemGame offerSystemGame = new OfferSystemGame((ICDNDownloader)resolver.ResolveOrParameter(typeof(ICDNDownloader), "cdnDownloader", parameters), (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters));
			Inject(offerSystemGame, resolver, parameters);
			return offerSystemGame;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
