using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.EventSystem
{
	[Preserve]
	internal class SteamStorePageGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SteamStorePage steamStorePage = new SteamStorePage();
			Inject(steamStorePage, resolver, parameters);
			return steamStorePage;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
