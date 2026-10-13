using System.Collections.Generic;
using CPM.Account;
using CPM.Connection;
using VContainer;
using _Car_Parking.Scripts.Wallet;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	[Preserve]
	internal class CircuitRacingRewardServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CircuitRacingRewardService circuitRacingRewardService = new CircuitRacingRewardService((IWalletService)resolver.ResolveOrParameter(typeof(IWalletService), "walletService", parameters), (IConnectionManager)resolver.ResolveOrParameter(typeof(IConnectionManager), "connectionManager", parameters), (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters));
			Inject(circuitRacingRewardService, resolver, parameters);
			return circuitRacingRewardService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
