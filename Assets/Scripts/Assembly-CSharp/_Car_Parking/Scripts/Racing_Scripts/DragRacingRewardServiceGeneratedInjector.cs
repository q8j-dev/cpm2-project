using System.Collections.Generic;
using CPM.Account;
using CPM.Connection;
using VContainer;
using _Car_Parking.Scripts.Wallet;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	[Preserve]
	internal class DragRacingRewardServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DragRacingRewardService dragRacingRewardService = new DragRacingRewardService((IWalletService)resolver.ResolveOrParameter(typeof(IWalletService), "walletService", parameters), (IConnectionManager)resolver.ResolveOrParameter(typeof(IConnectionManager), "connectionManager", parameters), (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters));
			Inject(dragRacingRewardService, resolver, parameters);
			return dragRacingRewardService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
