using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Steam.Purchase
{
	[Preserve]
	internal class SteamPurchaseGateGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SteamPurchaseGate steamPurchaseGate = new SteamPurchaseGate();
			Inject(steamPurchaseGate, resolver, parameters);
			return steamPurchaseGate;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
