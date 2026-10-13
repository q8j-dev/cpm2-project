using System.Collections.Generic;
using CPM.Account;
using VContainer;
using _Car_Parking.Scripts.Backend.Session;

namespace _Car_Parking.Scripts.Steam.Session
{
	[Preserve]
	internal class DesktopSessionWatchdogGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DesktopSessionWatchdog desktopSessionWatchdog = new DesktopSessionWatchdog((IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "account", parameters), (SessionBackend)resolver.ResolveOrParameter(typeof(SessionBackend), "session", parameters));
			Inject(desktopSessionWatchdog, resolver, parameters);
			return desktopSessionWatchdog;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
