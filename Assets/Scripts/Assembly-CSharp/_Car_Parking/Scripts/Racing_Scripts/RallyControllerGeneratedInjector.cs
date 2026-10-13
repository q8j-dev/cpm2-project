using System;
using System.Collections.Generic;
using CPM.Database;
using RacingScripts.RaceControllers;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.GrantService;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Racing_Scripts.Rally.Analytics;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	[Preserve]
	internal class RallyControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:RallyController cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RallyController obj = (RallyController)instance;
			RacingCameraController racingCameraController = (RacingCameraController)resolver.ResolveOrParameter(typeof(RacingCameraController), "racingCameraController", parameters);
			PlayerDatas.CarLoadingUtility carLoadingUtility = (PlayerDatas.CarLoadingUtility)resolver.ResolveOrParameter(typeof(PlayerDatas.CarLoadingUtility), "carLoadingUtility", parameters);
			CircuitRacingWaitingWindow circuitRacingWaitingWindow = (CircuitRacingWaitingWindow)resolver.ResolveOrParameter(typeof(CircuitRacingWaitingWindow), "circuitRacingWaitingWindow", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			LapRaceTrafficLightUI lapRaceTrafficLightUI = (LapRaceTrafficLightUI)resolver.ResolveOrParameter(typeof(LapRaceTrafficLightUI), "lapRaceTrafficLightUI", parameters);
			MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
			RaceModeController raceModeController = (RaceModeController)resolver.ResolveOrParameter(typeof(RaceModeController), "raceModeController", parameters);
			IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
			RallyAnalytic rallyAnalytic = (RallyAnalytic)resolver.ResolveOrParameter(typeof(RallyAnalytic), "rallyAnalytic", parameters);
			RallySessionTracker rallySessionTracker = (RallySessionTracker)resolver.ResolveOrParameter(typeof(RallySessionTracker), "rallySessionTracker", parameters);
			CurrencyManagerController currencyManagerController = (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters);
			RallyData rallyData = (RallyData)resolver.ResolveOrParameter(typeof(RallyData), "rallyData", parameters);
			ISteamAchievementService steamAchievementService = (ISteamAchievementService)resolver.ResolveOrParameter(typeof(ISteamAchievementService), "steamAchievementService", parameters);
			RewardGranter rewardGranter = (RewardGranter)resolver.ResolveOrParameter(typeof(RewardGranter), "rewardGranter", parameters);
			AllCarsInfo allCarsInfo = (AllCarsInfo)resolver.ResolveOrParameter(typeof(AllCarsInfo), "allCarsInfo", parameters);
			SoundBankLoadService soundBankLoadService = (SoundBankLoadService)resolver.ResolveOrParameter(typeof(SoundBankLoadService), "soundBankLoadService", parameters);
			RacingProvider racingProvider = (RacingProvider)resolver.ResolveOrParameter(typeof(RacingProvider), "racingProvider", parameters);
			RainSound rainSound = (RainSound)resolver.ResolveOrParameter(typeof(RainSound), "rainSound", parameters);
			CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
			obj.Construct(racingCameraController, carLoadingUtility, circuitRacingWaitingWindow, managerUI, lapRaceTrafficLightUI, menuLevelParent, raceModeController, prefabFactory, rallyAnalytic, rallySessionTracker, currencyManagerController, rallyData, steamAchievementService, rewardGranter, allCarsInfo, soundBankLoadService, racingProvider, rainSound, carInputManager);
		}
	}
}
