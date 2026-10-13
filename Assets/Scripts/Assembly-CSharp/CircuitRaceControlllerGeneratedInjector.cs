using System;
using System.Collections.Generic;
using CPM.Account;
using CPM.Connection;
using CPM.DailyTasks;
using CPM.Rating;
using CPM.RemoteConfig;
using RacingScripts.RaceControllers;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Connection;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.PlayerProfile;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.WindowSystem;

[Preserve]
internal class CircuitRaceControlllerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:CircuitRaceControlller cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		CircuitRaceControlller obj = (CircuitRaceControlller)instance;
		IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
		PlayerDatas.CarLoadingUtility carLoadingUtility = (PlayerDatas.CarLoadingUtility)resolver.ResolveOrParameter(typeof(PlayerDatas.CarLoadingUtility), "carLoadingUtility", parameters);
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
		CurrencyManagerController currencyManagerController = (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters);
		IPlayerStorageService playerStorageService = (IPlayerStorageService)resolver.ResolveOrParameter(typeof(IPlayerStorageService), "playerStorageService", parameters);
		Camera camera = (Camera)resolver.ResolveOrParameter(typeof(Camera), "camera", parameters);
		IConnectionManager connectionManager = (IConnectionManager)resolver.ResolveOrParameter(typeof(IConnectionManager), "connectionManager", parameters);
		IAccountController accountController = (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters);
		ICircuitRacingRewardService circuitRacingRewardService = (ICircuitRacingRewardService)resolver.ResolveOrParameter(typeof(ICircuitRacingRewardService), "circuitRacingRewardService", parameters);
		ITokenService tokenService = (ITokenService)resolver.ResolveOrParameter(typeof(ITokenService), "tokenService", parameters);
		CircuitRacingConfig circuitRacingConfig = (CircuitRacingConfig)resolver.ResolveOrParameter(typeof(CircuitRacingConfig), "circuitRacingConfig", parameters);
		IRemoteConfigManager remoteConfigManager = (IRemoteConfigManager)resolver.ResolveOrParameter(typeof(IRemoteConfigManager), "remoteConfigManager", parameters);
		CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
		RatingSender ratingController = (RatingSender)resolver.ResolveOrParameter(typeof(RatingSender), "ratingController", parameters);
		DailyTasksManager dailyTasksManager = (DailyTasksManager)resolver.ResolveOrParameter(typeof(DailyTasksManager), "dailyTasksManager", parameters);
		InputItemsProvider controlItemsProvider = (InputItemsProvider)resolver.ResolveOrParameter(typeof(InputItemsProvider), "controlItemsProvider", parameters);
		CoreGameManager coreGameManager = (CoreGameManager)resolver.ResolveOrParameter(typeof(CoreGameManager), "coreGameManager", parameters);
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		RacingAnalytic racingAnalytic = (RacingAnalytic)resolver.ResolveOrParameter(typeof(RacingAnalytic), "racingAnalytic", parameters);
		RacingCameraController racingCameraController = (RacingCameraController)resolver.ResolveOrParameter(typeof(RacingCameraController), "racingCameraController", parameters);
		CircuitRacingWaitingWindow circuitRacingWaitingWindow = (CircuitRacingWaitingWindow)resolver.ResolveOrParameter(typeof(CircuitRacingWaitingWindow), "circuitRacingWaitingWindow", parameters);
		LapRaceTrafficLightUI lapRaceTrafficLightUI = (LapRaceTrafficLightUI)resolver.ResolveOrParameter(typeof(LapRaceTrafficLightUI), "lapRaceTrafficLightUI", parameters);
		IGuestAccountCreator guestAccountCreator = (IGuestAccountCreator)resolver.ResolveOrParameter(typeof(IGuestAccountCreator), "guestAccountCreator", parameters);
		RaceModeController raceModeController = (RaceModeController)resolver.ResolveOrParameter(typeof(RaceModeController), "raceModeController", parameters);
		IPlayerProfileService playerProfileService = (IPlayerProfileService)resolver.ResolveOrParameter(typeof(IPlayerProfileService), "playerProfileService", parameters);
		ISteamAchievementService steamAchievementService = (ISteamAchievementService)resolver.ResolveOrParameter(typeof(ISteamAchievementService), "steamAchievementService", parameters);
		obj.Construct(prefabFactory, carLoadingUtility, managerUI, menuLevelParent, currencyManagerController, playerStorageService, camera, connectionManager, accountController, circuitRacingRewardService, tokenService, circuitRacingConfig, remoteConfigManager, carInputManager, ratingController, dailyTasksManager, controlItemsProvider, coreGameManager, freeDriveProvider, racingAnalytic, racingCameraController, circuitRacingWaitingWindow, lapRaceTrafficLightUI, guestAccountCreator, raceModeController, playerProfileService, steamAchievementService);
	}
}
