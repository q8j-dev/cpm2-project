using System;
using System.Collections.Generic;
using CPM.Ads.Policy;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Database.Account;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Free_drive.Scenes;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.PlayerProperty.Business;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement;

[Preserve]
internal class MenuLevelParentGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:MenuLevelParent cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		MenuLevelParent obj = (MenuLevelParent)instance;
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		OnlineCollectionsProvider onlineCollectionsProvider = (OnlineCollectionsProvider)resolver.ResolveOrParameter(typeof(OnlineCollectionsProvider), "onlineCollectionsProvider", parameters);
		LevelsProvider levelsProvider = (LevelsProvider)resolver.ResolveOrParameter(typeof(LevelsProvider), "levelsProvider", parameters);
		InputItemsProvider inputItemsProvider = (InputItemsProvider)resolver.ResolveOrParameter(typeof(InputItemsProvider), "inputItemsProvider", parameters);
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
		LevelRatingSender ratingSender = (LevelRatingSender)resolver.ResolveOrParameter(typeof(LevelRatingSender), "ratingSender", parameters);
		GlobalCollectionsProvider globalCollectionsProvider = (GlobalCollectionsProvider)resolver.ResolveOrParameter(typeof(GlobalCollectionsProvider), "globalCollectionsProvider", parameters);
		OpenWorldAnalytic openWorldAnalytic = (OpenWorldAnalytic)resolver.ResolveOrParameter(typeof(OpenWorldAnalytic), "openWorldAnalytic", parameters);
		CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
		RacingProvider racingProvider = (RacingProvider)resolver.ResolveOrParameter(typeof(RacingProvider), "racingProvider", parameters);
		EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
		OptionalDependency<MultiPlayer> multiPlayer = (OptionalDependency<MultiPlayer>)resolver.ResolveOrParameter(typeof(OptionalDependency<MultiPlayer>), "multiPlayer", parameters);
		OptionalDependency<SkyboxBlender> skyboxBlender = (OptionalDependency<SkyboxBlender>)resolver.ResolveOrParameter(typeof(OptionalDependency<SkyboxBlender>), "skyboxBlender", parameters);
		OptionalDependency<SceneControl> sceneControl = (OptionalDependency<SceneControl>)resolver.ResolveOrParameter(typeof(OptionalDependency<SceneControl>), "sceneControl", parameters);
		OptionalDependency<LevelScript> levelScript = (OptionalDependency<LevelScript>)resolver.ResolveOrParameter(typeof(OptionalDependency<LevelScript>), "levelScript", parameters);
		IObjectResolver resolver2 = (IObjectResolver)resolver.ResolveOrParameter(typeof(IObjectResolver), "resolver", parameters);
		OptionalDependency<ParkingMenuUI> parkingMenuUI = (OptionalDependency<ParkingMenuUI>)resolver.ResolveOrParameter(typeof(OptionalDependency<ParkingMenuUI>), "parkingMenuUI", parameters);
		OptionalDependency<QuickCarSettings> quickCarSettings = (OptionalDependency<QuickCarSettings>)resolver.ResolveOrParameter(typeof(OptionalDependency<QuickCarSettings>), "quickCarSettings", parameters);
		OptionalDependency<CarSellTriggersCollection> carSellTriggersCollection = (OptionalDependency<CarSellTriggersCollection>)resolver.ResolveOrParameter(typeof(OptionalDependency<CarSellTriggersCollection>), "carSellTriggersCollection", parameters);
		OptionalDependency<LevelsAnalytic> levelsAnalytic = (OptionalDependency<LevelsAnalytic>)resolver.ResolveOrParameter(typeof(OptionalDependency<LevelsAnalytic>), "levelsAnalytic", parameters);
		OptionalDependency<ChatControl> chatControl = (OptionalDependency<ChatControl>)resolver.ResolveOrParameter(typeof(OptionalDependency<ChatControl>), "chatControl", parameters);
		OptionalDependency<IBusinessProfitControl> businessProfitControlDep = (OptionalDependency<IBusinessProfitControl>)resolver.ResolveOrParameter(typeof(OptionalDependency<IBusinessProfitControl>), "businessProfitControlDep", parameters);
		IAdOfferService adOffers = (IAdOfferService)resolver.ResolveOrParameter(typeof(IAdOfferService), "adOffers", parameters);
		AdsAnalytic adsAnalytic = (AdsAnalytic)resolver.ResolveOrParameter(typeof(AdsAnalytic), "adsAnalytic", parameters);
		IClientAdRewardConsumeService adRewardConsume = (IClientAdRewardConsumeService)resolver.ResolveOrParameter(typeof(IClientAdRewardConsumeService), "adRewardConsume", parameters);
		obj.Construct(freeDriveProvider, onlineCollectionsProvider, levelsProvider, inputItemsProvider, managerUI, prefabFactory, ratingSender, globalCollectionsProvider, openWorldAnalytic, carInputManager, racingProvider, eventRegistry, multiPlayer, skyboxBlender, sceneControl, levelScript, resolver2, parkingMenuUI, quickCarSettings, carSellTriggersCollection, levelsAnalytic, chatControl, businessProfitControlDep, adOffers, adsAnalytic, adRewardConsume);
	}
}
