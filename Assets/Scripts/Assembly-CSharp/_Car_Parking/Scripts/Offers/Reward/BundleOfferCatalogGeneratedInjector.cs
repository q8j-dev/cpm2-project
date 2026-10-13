using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Offers.Reward
{
	[Preserve]
	internal class BundleOfferCatalogGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			BundleOfferCatalog bundleOfferCatalog = new BundleOfferCatalog((IOfferSystem)resolver.ResolveOrParameter(typeof(IOfferSystem), "offerSystem", parameters), (ISuitPacksConfig)resolver.ResolveOrParameter(typeof(ISuitPacksConfig), "packs", parameters));
			Inject(bundleOfferCatalog, resolver, parameters);
			return bundleOfferCatalog;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
