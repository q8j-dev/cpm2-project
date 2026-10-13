using System.Collections.Generic;
using CPM.Connection;
using CPM.Database;
using CPM.PlayerData;
using VContainer;

namespace _Car_Parking.Scripts.Database
{
	[Preserve]
	internal class FirebaseSaveLoadServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FirebaseSaveLoadService firebaseSaveLoadService = new FirebaseSaveLoadService((IConnectionManager)resolver.ResolveOrParameter(typeof(IConnectionManager), "connectionManager", parameters), (EngineSaveLoadService)resolver.ResolveOrParameter(typeof(EngineSaveLoadService), "engineSaveLoadService", parameters), (PlatesSaveLoadService)resolver.ResolveOrParameter(typeof(PlatesSaveLoadService), "platesSaveLoadService", parameters), (PartsInventorySaveLoadService)resolver.ResolveOrParameter(typeof(PartsInventorySaveLoadService), "partsInventorySaveLoadService", parameters), (PlayerStorageSaveLoadService)resolver.ResolveOrParameter(typeof(PlayerStorageSaveLoadService), "playerStorageSaveLoadService", parameters), (PlayerAnonStorageSaveLoadService)resolver.ResolveOrParameter(typeof(PlayerAnonStorageSaveLoadService), "playerAnonStorageSaveLoadService", parameters), (WalletSaveLoadService)resolver.ResolveOrParameter(typeof(WalletSaveLoadService), "walletSaveLoadService", parameters), (SlotsOnDatabaseSaveLoadService)resolver.ResolveOrParameter(typeof(SlotsOnDatabaseSaveLoadService), "slotsOnDatabaseSaveLoadService", parameters), (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters));
			Inject(firebaseSaveLoadService, resolver, parameters);
			return firebaseSaveLoadService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
