using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Offers.Reward
{
	[Preserve]
	internal class EmoteOfferBannersConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EmoteOfferBannersConfig emoteOfferBannersConfig = new EmoteOfferBannersConfig();
			Inject(emoteOfferBannersConfig, resolver, parameters);
			return emoteOfferBannersConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
