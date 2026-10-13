using CPM.PlayerData;
using CPMEngine;
using CommonUtility.Contracts;
using Persistence.Systems;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Connection;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Inventory;
using _Car_Parking.Scripts.Other.Cypher;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.Wallet;
using _Car_Parking.Scripts.Wallet.Cars;
using _Car_Parking.Scripts.Wallet.Cypher;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace _Car_Parking.Scripts.Scopes
{
	public static class PlayerDataScope
	{
		public static void Initialize(IContainerBuilder builder)
		{
			builder.RegisterComponentOnNewGameObject<PlayerDatas>(Lifetime.Singleton, "PlayerDatas");
			builder.RegisterComponentInHierarchy<CarInputManager>();
			builder.RegisterComponentInHierarchy<CPMVehicleCore>();
			builder.Register<PlayerDatas.FriendsUtility>(Lifetime.Singleton).As<IFriendsUtility>().AsSelf();
			builder.Register<PlayerDataStorage>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				c.Resolve<PlayerDataStorage>();
			});
			builder.Register<PurchaseItemService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
			builder.Register<StorageService>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.Register<EasySaveAuthorizationHandler>(Lifetime.Singleton).As<IFirstSignInDependant>();
			builder.Register<CurrencyManagerController>(Lifetime.Singleton);
			builder.Register<RewardWallet>(Lifetime.Singleton).As<IRewardWallet>();
			builder.Register<DatabaseWalletService>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.Register<DragRacingRewardService>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.Register<CircuitRacingRewardService>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.Register<TokenService>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.Register<WalletCypher>(Lifetime.Singleton);
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				c.Resolve<WalletCypher>();
			});
			builder.Register<BuyCarCypher>(Lifetime.Singleton);
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				c.Resolve<BuyCarCypher>();
			});
			builder.RegisterEntryPoint<InventoryService>().As<IInventoryPartsService>();
			builder.RegisterEntryPoint<EngineKitsInventoryService>().As<IEngineKitsInventoryService>();
			builder.Register<BuyCarBackendService>(Lifetime.Singleton).As<IBuyCarBackendService>();
			builder.Register<CarBuyRequestFactory>(Lifetime.Singleton);
			builder.Register<CommonProvider>(Lifetime.Singleton);
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				c.Resolve<CommonProvider>();
			});
			builder.Register<CarTuningFactory>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.Register<AllPricesConfigConst>(Lifetime.Singleton).As<IPricesService>().As<IEarnMoneyService>();
			builder.Register<RallyData>(Lifetime.Singleton);
		}
	}
}
