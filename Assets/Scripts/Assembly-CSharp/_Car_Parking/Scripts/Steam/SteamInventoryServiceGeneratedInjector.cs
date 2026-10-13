using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Steam
{
	[Preserve]
	internal class SteamInventoryServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SteamInventoryService steamInventoryService = new SteamInventoryService();
			Inject(steamInventoryService, resolver, parameters);
			return steamInventoryService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
