using System.Collections.Generic;
using CPM.Account;
using CPM.Balance;
using CPM.Connection;
using VContainer;

namespace _Car_Parking.Scripts.AccountSystemNew
{
	[Preserve]
	internal class GuestAccountCreatorGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GuestAccountCreator guestAccountCreator = new GuestAccountCreator((IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters), (IGameDataRepository)resolver.ResolveOrParameter(typeof(IGameDataRepository), "gameDataRepository", parameters), (IConnectionDataHandler)resolver.ResolveOrParameter(typeof(IConnectionDataHandler), "connectionDataHandler", parameters), (IBalanceDocumentSynchronizer)resolver.ResolveOrParameter(typeof(IBalanceDocumentSynchronizer), "balanceDocumentSynchronizer", parameters));
			Inject(guestAccountCreator, resolver, parameters);
			return guestAccountCreator;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
