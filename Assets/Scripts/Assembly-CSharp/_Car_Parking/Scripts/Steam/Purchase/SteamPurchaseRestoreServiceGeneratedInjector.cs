using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Steam.Base;

namespace _Car_Parking.Scripts.Steam.Purchase
{
	[Preserve]
	internal class SteamPurchaseRestoreServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SteamPurchaseRestoreService steamPurchaseRestoreService = new SteamPurchaseRestoreService((ISteamAccountService)resolver.ResolveOrParameter(typeof(ISteamAccountService), "steamAccountService", parameters));
			Inject(steamPurchaseRestoreService, resolver, parameters);
			return steamPurchaseRestoreService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
