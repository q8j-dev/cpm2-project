using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Notification;
using _Car_Parking.Scripts.Offers.Reward;

namespace _Car_Parking.Scripts.GrantService
{
	[Preserve]
	internal class GrantHandlerSuitGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GrantHandlerSuit grantHandlerSuit = new GrantHandlerSuit((ISuitInventory)resolver.ResolveOrParameter(typeof(ISuitInventory), "suits", parameters), (INotificationService)resolver.ResolveOrParameter(typeof(INotificationService), "notification", parameters));
			Inject(grantHandlerSuit, resolver, parameters);
			return grantHandlerSuit;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
