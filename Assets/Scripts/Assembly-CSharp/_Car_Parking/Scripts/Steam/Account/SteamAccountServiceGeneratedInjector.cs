using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Infrastructure.Start;

namespace _Car_Parking.Scripts.Steam.Account
{
	[Preserve]
	internal class SteamAccountServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SteamAccountService steamAccountService = new SteamAccountService((ServerTimeService)resolver.ResolveOrParameter(typeof(ServerTimeService), "serverTimeService", parameters), (ProfileAnalytic)resolver.ResolveOrParameter(typeof(ProfileAnalytic), "profileAnalytic", parameters), (AccountLinkAnalytic)resolver.ResolveOrParameter(typeof(AccountLinkAnalytic), "accountLinkAnalytic", parameters));
			Inject(steamAccountService, resolver, parameters);
			return steamAccountService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
