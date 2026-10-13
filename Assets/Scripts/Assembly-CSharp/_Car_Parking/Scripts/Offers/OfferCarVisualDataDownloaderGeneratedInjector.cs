using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Offers
{
	[Preserve]
	internal class OfferCarVisualDataDownloaderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			OfferCarVisualDataDownloader offerCarVisualDataDownloader = new OfferCarVisualDataDownloader();
			Inject(offerCarVisualDataDownloader, resolver, parameters);
			return offerCarVisualDataDownloader;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
