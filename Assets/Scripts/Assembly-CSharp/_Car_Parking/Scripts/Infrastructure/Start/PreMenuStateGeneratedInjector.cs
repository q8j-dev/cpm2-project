using System.Collections.Generic;
using CPM.Account;
using CPM.Database;
using VContainer;

namespace _Car_Parking.Scripts.Infrastructure.Start
{
	[Preserve]
	internal class PreMenuStateGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PreMenuState preMenuState = new PreMenuState((IEnumerable<IReInitPreMainMenu>)resolver.ResolveOrParameter(typeof(IEnumerable<IReInitPreMainMenu>), "reInits", parameters), (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters));
			Inject(preMenuState, resolver, parameters);
			return preMenuState;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
