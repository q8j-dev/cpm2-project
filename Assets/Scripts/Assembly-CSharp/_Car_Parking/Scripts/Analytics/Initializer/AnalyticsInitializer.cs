using System;
using CPM.Account;
using CPM.PlayerData;
using CPM.RemoteConfig;
using CPM.Scripts.Garage;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.Infrastructure.Start;

namespace _Car_Parking.Scripts.Analytics.Initializer
{
	[Preserve]
	public class AnalyticsInitializer : IInitializable, IDisposable
	{
		private readonly PlayerDataStorage _playerDataStorage;

		private readonly IAccountController _accountController;

		private readonly GeoGetterService _geoGetterService;

		private readonly ServerTimeService _serverTimeService;

		private readonly IGarageSelectorService _garageSelectorService;

		private readonly IRemoteConfigManager _remoteConfigManager;

		public AnalyticsInitializer(PlayerDataStorage playerDataStorage, IAccountController accountController, GeoGetterService geoGetterService, ServerTimeService serverTimeService, IGarageSelectorService garageSelectorService, IRemoteConfigManager remoteConfigManager)
		{
			_playerDataStorage = playerDataStorage;
			_accountController = accountController;
			_geoGetterService = geoGetterService;
			_serverTimeService = serverTimeService;
			_garageSelectorService = garageSelectorService;
			_remoteConfigManager = remoteConfigManager;
		}

		public void Initialize()
		{
			AnalyticsDependencies.RegisterDependence(_playerDataStorage);
			AnalyticsDependencies.RegisterDependence(_accountController);
			AnalyticsDependencies.RegisterDependence(_geoGetterService);
			AnalyticsDependencies.RegisterDependence(_serverTimeService);
			AnalyticsDependencies.RegisterDependence(_garageSelectorService);
			AnalyticsDependencies.RegisterDependence(_remoteConfigManager);
		}

		public void Dispose()
		{
			AnalyticsDependencies.Unregister(_playerDataStorage);
			AnalyticsDependencies.Unregister(_accountController);
			AnalyticsDependencies.Unregister(_geoGetterService);
			AnalyticsDependencies.Unregister(_serverTimeService);
			AnalyticsDependencies.Unregister(_garageSelectorService);
			AnalyticsDependencies.Unregister(_remoteConfigManager);
		}
	}
}
