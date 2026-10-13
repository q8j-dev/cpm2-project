using System;
using System.Collections.Generic;
using CPM.Account;
using CPM.PlayerData;
using CPM.RemoteConfig;
using VContainer;

[Preserve]
internal class RootCheckerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:RootChecker cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		RootChecker obj = (RootChecker)instance;
		PlayerDataStorage playerDataStorage = (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters);
		IAccountController accountController = (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters);
		IRemoteConfigManager remoteConfigManager = (IRemoteConfigManager)resolver.ResolveOrParameter(typeof(IRemoteConfigManager), "remoteConfigManager", parameters);
		obj.Construct(playerDataStorage, accountController, remoteConfigManager);
	}
}
