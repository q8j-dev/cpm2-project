using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Steam.Base;

namespace _Car_Parking.Scripts.Steam.Purchase
{
	[Preserve]
	internal class SteamPurchaseServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SteamPurchaseService steamPurchaseService = new SteamPurchaseService((ISteamInventoryService)resolver.ResolveOrParameter(typeof(ISteamInventoryService), "steamInventoryService", parameters), (ISteamPurchaseGate)resolver.ResolveOrParameter(typeof(ISteamPurchaseGate), "steamPurchaseGate", parameters));
			Inject(steamPurchaseService, resolver, parameters);
			return steamPurchaseService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
