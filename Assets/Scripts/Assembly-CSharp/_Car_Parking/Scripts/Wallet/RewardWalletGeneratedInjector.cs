using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Wallet
{
	[Preserve]
	internal class RewardWalletGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RewardWallet rewardWallet = new RewardWallet((CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters));
			Inject(rewardWallet, resolver, parameters);
			return rewardWallet;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
