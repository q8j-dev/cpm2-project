using System.Collections.Generic;
using CPM.PlayerData;
using VContainer;

namespace _Car_Parking.Scripts.Wallet
{
	[Preserve]
	internal class DatabaseWalletServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DatabaseWalletService databaseWalletService = new DatabaseWalletService((PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters));
			Inject(databaseWalletService, resolver, parameters);
			return databaseWalletService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
