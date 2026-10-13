using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Notification;
using _Car_Parking.Scripts.Offers.Reward;

namespace _Car_Parking.Scripts.GrantService
{
	[Preserve]
	internal class GrantHandlerSuitpackGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GrantHandlerSuitpack grantHandlerSuitpack = new GrantHandlerSuitpack((ISuitInventory)resolver.ResolveOrParameter(typeof(ISuitInventory), "suits", parameters), (ISuitPacksConfig)resolver.ResolveOrParameter(typeof(ISuitPacksConfig), "packs", parameters), (INotificationService)resolver.ResolveOrParameter(typeof(INotificationService), "notification", parameters));
			Inject(grantHandlerSuitpack, resolver, parameters);
			return grantHandlerSuitpack;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
