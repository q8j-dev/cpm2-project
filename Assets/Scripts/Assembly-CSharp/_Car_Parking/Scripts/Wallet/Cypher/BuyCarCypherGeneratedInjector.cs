using System.Collections.Generic;
using CPM.PlayerData;
using VContainer;

namespace _Car_Parking.Scripts.Wallet.Cypher
{
	[Preserve]
	internal class BuyCarCypherGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			BuyCarCypher buyCarCypher = new BuyCarCypher((PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters));
			Inject(buyCarCypher, resolver, parameters);
			return buyCarCypher;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
