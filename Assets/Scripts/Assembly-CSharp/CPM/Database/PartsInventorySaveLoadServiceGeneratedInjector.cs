using System.Collections.Generic;
using CPM.Account;
using CPM.Connection;
using CPM.PlayerData;
using VContainer;
using _Car_Parking.Scripts.Characters;

namespace CPM.Database
{
	[Preserve]
	internal class PartsInventorySaveLoadServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PartsInventorySaveLoadService partsInventorySaveLoadService = new PartsInventorySaveLoadService();
			Inject(partsInventorySaveLoadService, resolver, parameters);
			return partsInventorySaveLoadService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PartsInventorySaveLoadService obj = (PartsInventorySaveLoadService)instance;
			IAccountController accountController = (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters);
			IConnectionManager connectionManager = (IConnectionManager)resolver.ResolveOrParameter(typeof(IConnectionManager), "connectionManager", parameters);
			PlayerDataStorage playerDataStorage = (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters);
			PresetGrantSession presetGrantSession = (PresetGrantSession)resolver.ResolveOrParameter(typeof(PresetGrantSession), "presetGrantSession", parameters);
			obj.Construct(accountController, connectionManager, playerDataStorage, presetGrantSession);
		}
	}
}
