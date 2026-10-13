using System.Collections.Generic;
using CPM.Account;
using VContainer;

namespace _Car_Parking.Scripts.Other.Helpers
{
	[Preserve]
	internal class PlayerStatusSystemGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PlayerStatusSystem playerStatusSystem = new PlayerStatusSystem((IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters));
			Inject(playerStatusSystem, resolver, parameters);
			return playerStatusSystem;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
