using System;
using CPM.Database;
using Cysharp.Threading.Tasks;
using Persistence.Systems;
using Photon.Pun;
using Rating.DailyTasks.Scripts;
using VContainer.Unity;
using _Car_Parking.Scripts.ArtistTools;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Notification;
using _Car_Parking.Scripts.OfflineMode;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Scopes
{
	public class MainMenuStartup : IStartable, IDisposable
	{
		private readonly DataBase _dataBase;

		private readonly IDailyServicesDependant _dailyServicesDependant;

		private readonly ISlotService _slotService;

		private readonly CharactersService _charactersService;

		private readonly PrefabFactoryLinkage _prefabFactoryLinkage;

		private readonly CommonProvider _commonProvider;

		private readonly OnlineCollectionsProvider _onlineCollectionsProvider;

		private readonly GlobalCollectionsProvider _globalCollectionsProvider;

		private readonly IOfflineModeSystem _offlineModeSystem;

		private readonly ICheckCarDataBase _checkCarDataBase;

		private readonly VfxPoolProvider _vfxPoolProvider;

		private readonly INotificationService _notificationService;

		public MainMenuStartup(DataBase dataBase, IDailyServicesDependant dailyServicesDependant, ISlotService slotService, CharactersService charactersService, PrefabFactoryLinkage prefabFactoryLinkage, CommonProvider commonProvider, OnlineCollectionsProvider onlineCollectionsProvider, GlobalCollectionsProvider globalCollectionsProvider, IOfflineModeSystem offlineModeSystem, ICheckCarDataBase checkCarDataBase, VfxPoolProvider vfxPoolProvider, INotificationService notificationService, ShadersVaultConfig shadersVaultConfig)
		{
			_dataBase = dataBase;
			_dailyServicesDependant = dailyServicesDependant;
			_slotService = slotService;
			_charactersService = charactersService;
			_prefabFactoryLinkage = prefabFactoryLinkage;
			_commonProvider = commonProvider;
			_onlineCollectionsProvider = onlineCollectionsProvider;
			_globalCollectionsProvider = globalCollectionsProvider;
			_offlineModeSystem = offlineModeSystem;
			_checkCarDataBase = checkCarDataBase;
			_vfxPoolProvider = vfxPoolProvider;
			_notificationService = notificationService;
			shadersVaultConfig.Clear();
		}

		public void Start()
		{
			_notificationService.Initialize();
			_commonProvider.SlotService = _slotService;
			_commonProvider.CharactersService = _charactersService;
			_commonProvider.DataBase = _dataBase;
			CommonExtensions.ClearGOCollection(Skidmarks.allSkidmarks);
			_onlineCollectionsProvider.ResetAllCollections();
			_globalCollectionsProvider.ResetAllCollections();
			_offlineModeSystem.CheckDisableStart();
			_checkCarDataBase.CheckDataBaseAndRestartIfNeedsAsync().Forget();
			_vfxPoolProvider.Initialize();
		}

		public void Dispose()
		{
			_vfxPoolProvider.Dispose();
			_offlineModeSystem.CheckDisableFinish();
			PhotonNetwork.PrefabPool?.ClearResources();
			_commonProvider.SlotService = null;
			_commonProvider.CharactersService = null;
			_commonProvider.DataBase = null;
		}
	}
}
