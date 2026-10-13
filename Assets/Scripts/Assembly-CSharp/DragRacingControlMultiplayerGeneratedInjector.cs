using System;
using System.Collections.Generic;
using CPM.PlayerData;
using Car_Parking.Scripts.Racing;
using VContainer;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;

[Preserve]
internal class DragRacingControlMultiplayerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:DragRacingControlMultiplayer cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		DragRacingControlMultiplayer obj = (DragRacingControlMultiplayer)instance;
		PlayerDataStorage playerDataStorage = (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters);
		CurrencyManagerController currencyManagerController = (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters);
		IPricesService pricesService = (IPricesService)resolver.ResolveOrParameter(typeof(IPricesService), "pricesService", parameters);
		IEarnMoneyService earnMoneyService = (IEarnMoneyService)resolver.ResolveOrParameter(typeof(IEarnMoneyService), "earnMoneyService", parameters);
		Sort2 sort = (Sort2)resolver.ResolveOrParameter(typeof(Sort2), "sort2", parameters);
		CoreGameManager coreGameManager = (CoreGameManager)resolver.ResolveOrParameter(typeof(CoreGameManager), "coreGameManager", parameters);
		OnlineCollectionsProvider onlineCollectionsProvider = (OnlineCollectionsProvider)resolver.ResolveOrParameter(typeof(OnlineCollectionsProvider), "onlineCollectionsProvider", parameters);
		FreeDriveControl freeDriveControl = (FreeDriveControl)resolver.ResolveOrParameter(typeof(FreeDriveControl), "freeDriveControl", parameters);
		InputMapSetEvent inputMapSetEvent = (InputMapSetEvent)resolver.ResolveOrParameter(typeof(InputMapSetEvent), "inputMapSetEvent", parameters);
		EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
		InputItemsProvider controlItemsProvider = (InputItemsProvider)resolver.ResolveOrParameter(typeof(InputItemsProvider), "controlItemsProvider", parameters);
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		LevelsProvider levelsProvider = (LevelsProvider)resolver.ResolveOrParameter(typeof(LevelsProvider), "levelsProvider", parameters);
		MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
		CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
		FreeTimeAttackController timeAttackController = (FreeTimeAttackController)resolver.ResolveOrParameter(typeof(FreeTimeAttackController), "timeAttackController", parameters);
		obj.Construct(playerDataStorage, currencyManagerController, pricesService, earnMoneyService, sort, coreGameManager, onlineCollectionsProvider, freeDriveControl, inputMapSetEvent, eventRegistry, controlItemsProvider, freeDriveProvider, levelsProvider, menuLevelParent, carInputManager, timeAttackController);
	}
}
