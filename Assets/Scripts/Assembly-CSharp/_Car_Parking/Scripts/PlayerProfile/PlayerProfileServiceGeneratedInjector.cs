using System.Collections.Generic;
using CPM.Account;
using VContainer;

namespace _Car_Parking.Scripts.PlayerProfile
{
	[Preserve]
	internal class PlayerProfileServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PlayerProfileService playerProfileService = new PlayerProfileService((IPlayerProfileBackend)resolver.ResolveOrParameter(typeof(IPlayerProfileBackend), "backend", parameters), (IPlayerProfileCache)resolver.ResolveOrParameter(typeof(IPlayerProfileCache), "cache", parameters), (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters));
			Inject(playerProfileService, resolver, parameters);
			return playerProfileService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
