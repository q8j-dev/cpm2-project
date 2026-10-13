using System.Collections.Generic;
using CPM.Connection;
using CPM.PlayerData;
using CPM.RemoteConfig;
using CommonUtility.Contracts;
using VContainer;
using _Car_Parking.Scripts.Backend.Session;

namespace CPM.Account
{
	[Preserve]
	internal class FirebaseAccountControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FirebaseAccountController firebaseAccountController = new FirebaseAccountController((IEnumerable<IAuthorizationDependant>)resolver.ResolveOrParameter(typeof(IEnumerable<IAuthorizationDependant>), "authorizationDependants", parameters), (IEnumerable<IAuthorizationFailureDependant>)resolver.ResolveOrParameter(typeof(IEnumerable<IAuthorizationFailureDependant>), "authorizationFailureDependants", parameters), (IEnumerable<IFirstSignInDependant>)resolver.ResolveOrParameter(typeof(IEnumerable<IFirstSignInDependant>), "firstSignInDependants", parameters), (CoroutineHelper)resolver.ResolveOrParameter(typeof(CoroutineHelper), "coroutineHelper", parameters), (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters), (IRemoteConfigManager)resolver.ResolveOrParameter(typeof(IRemoteConfigManager), "remoteConfigManager", parameters), (IPrefsSaversGetter)resolver.ResolveOrParameter(typeof(IPrefsSaversGetter), "prefsSaversGetter", parameters), (IDeleterUserDataGetter)resolver.ResolveOrParameter(typeof(IDeleterUserDataGetter), "deleterUserDataGetter", parameters), (IConnectionManagerBase)resolver.ResolveOrParameter(typeof(IConnectionManagerBase), "connectionManagerBase", parameters), (SessionBackend)resolver.ResolveOrParameter(typeof(SessionBackend), "sessionBackend", parameters));
			Inject(firebaseAccountController, resolver, parameters);
			return firebaseAccountController;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
