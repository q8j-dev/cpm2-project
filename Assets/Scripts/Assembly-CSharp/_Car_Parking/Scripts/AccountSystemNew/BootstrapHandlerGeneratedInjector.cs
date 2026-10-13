using System.Collections.Generic;
using CPM.Account;
using CPM.Balance;
using CPM.Connection;
using CPM.Database;
using CPM.PlayerData;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.OfflineMode;
using _Car_Parking.Scripts.PlayerProfile;
using _Car_Parking.Scripts.Steam.Base;

namespace _Car_Parking.Scripts.AccountSystemNew
{
	[Preserve]
	internal class BootstrapHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			BootstrapHandler bootstrapHandler = new BootstrapHandler((IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters), (IOfflineModeSystem)resolver.ResolveOrParameter(typeof(IOfflineModeSystem), "offlineModeSystem", parameters), (DataBase)resolver.ResolveOrParameter(typeof(DataBase), "dataBase", parameters), (IDatabaseSaveLoadService)resolver.ResolveOrParameter(typeof(IDatabaseSaveLoadService), "databaseSaveLoadService", parameters), (IConnectionDataHandler)resolver.ResolveOrParameter(typeof(IConnectionDataHandler), "connectionDataHandler", parameters), (BootStartData)resolver.ResolveOrParameter(typeof(BootStartData), "bootStartData", parameters), (IGameDataRepository)resolver.ResolveOrParameter(typeof(IGameDataRepository), "gameDataRepository", parameters), (ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters), (MigrationAnalytic)resolver.ResolveOrParameter(typeof(MigrationAnalytic), "migrationAnalytic", parameters), (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters), (ISteamAccountService)resolver.ResolveOrParameter(typeof(ISteamAccountService), "steamAccountService", parameters), (ISteamPurchaseRestoreService)resolver.ResolveOrParameter(typeof(ISteamPurchaseRestoreService), "steamPurchaseRestoreService", parameters), (ISteamPurchaseGate)resolver.ResolveOrParameter(typeof(ISteamPurchaseGate), "steamPurchaseGate", parameters), (IBalanceDocumentSynchronizer)resolver.ResolveOrParameter(typeof(IBalanceDocumentSynchronizer), "balanceDocumentSynchronizer", parameters), (IPlayerProfileService)resolver.ResolveOrParameter(typeof(IPlayerProfileService), "playerProfileService", parameters));
			Inject(bootstrapHandler, resolver, parameters);
			return bootstrapHandler;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
