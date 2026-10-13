using System.Collections.Generic;
using CPM.Connection;
using CPM.Database;
using CPM.RemoteConfig;
using Persistence.Systems;
using VContainer;
using _Car_Parking.Scripts.Persistence.Migration;
using _Car_Parking.Scripts.VinylsAi;

namespace _Car_Parking.Scripts.Scopes
{
	[Preserve]
	internal class PersistentStartupGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PersistentStartup persistentStartup = new PersistentStartup((ConnectionHandler)resolver.ResolveOrParameter(typeof(ConnectionHandler), "connectionHandler", parameters), (RideableMeshesGraph)resolver.ResolveOrParameter(typeof(RideableMeshesGraph), "rideableMeshesGraph", parameters), (CoroutineHelper)resolver.ResolveOrParameter(typeof(CoroutineHelper), "coroutineHelper", parameters), (IMigrationManager)resolver.ResolveOrParameter(typeof(IMigrationManager), "migrationManager", parameters), (PlayerDatas)resolver.ResolveOrParameter(typeof(PlayerDatas), "playerDatas", parameters), (PlayerDatas.FriendsUtility)resolver.ResolveOrParameter(typeof(PlayerDatas.FriendsUtility), "friendsUtility", parameters), (IConnectionDataHandler)resolver.ResolveOrParameter(typeof(IConnectionDataHandler), "connectionDataHandler", parameters), (RuntimePlayerDataHandler)resolver.ResolveOrParameter(typeof(RuntimePlayerDataHandler), "runtimePlayerDataHandler", parameters), (IDatabaseSaveLoadService)resolver.ResolveOrParameter(typeof(IDatabaseSaveLoadService), "databaseSaveLoadService", parameters), (PartsInventorySaveLoadService)resolver.ResolveOrParameter(typeof(PartsInventorySaveLoadService), "partsInventory", parameters), (EngineSaveLoadService)resolver.ResolveOrParameter(typeof(EngineSaveLoadService), "engineSaveLoad", parameters), (PlatesSaveLoadService)resolver.ResolveOrParameter(typeof(PlatesSaveLoadService), "platesSaveLoad", parameters), (PlayerStorageSaveLoadService)resolver.ResolveOrParameter(typeof(PlayerStorageSaveLoadService), "playerStorageSaveLoad", parameters), (PlayerAnonStorageSaveLoadService)resolver.ResolveOrParameter(typeof(PlayerAnonStorageSaveLoadService), "playerAnonStorageSaveLoad", parameters), (WalletSaveLoadService)resolver.ResolveOrParameter(typeof(WalletSaveLoadService), "walletSaveLoad", parameters), (FriendsSaveLoadService)resolver.ResolveOrParameter(typeof(FriendsSaveLoadService), "friendsSaveLoad", parameters), (SlotsOnDatabaseSaveLoadService)resolver.ResolveOrParameter(typeof(SlotsOnDatabaseSaveLoadService), "slotsSaveLoad", parameters), (VinylsCheckApi)resolver.ResolveOrParameter(typeof(VinylsCheckApi), "vinylsCheckApi", parameters), (IRemoteConfigManager)resolver.ResolveOrParameter(typeof(IRemoteConfigManager), "remoteConfigManager", parameters));
			Inject(persistentStartup, resolver, parameters);
			return persistentStartup;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
