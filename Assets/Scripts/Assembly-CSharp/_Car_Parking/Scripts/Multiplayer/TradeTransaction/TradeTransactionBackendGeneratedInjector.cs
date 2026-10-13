using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Backend;

namespace _Car_Parking.Scripts.Multiplayer.TradeTransaction
{
	[Preserve]
	internal class TradeTransactionBackendGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			TradeTransactionBackend tradeTransactionBackend = new TradeTransactionBackend((BackendClient)resolver.ResolveOrParameter(typeof(BackendClient), "backendClient", parameters));
			Inject(tradeTransactionBackend, resolver, parameters);
			return tradeTransactionBackend;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
