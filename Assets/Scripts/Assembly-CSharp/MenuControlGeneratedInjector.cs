using System;
using System.Collections.Generic;
using CPM.Account;
using CPM.Database;
using CPM.PlayerData;
using CPM.Scripts.Garage;
using CPM.Scripts.Garage.GarageBuyer;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.WindowSystem.ProgressWindows;

[Preserve]
internal class MenuControlGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:MenuControl cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		MenuControl obj = (MenuControl)instance;
		PlayerDataStorage playerDataStorage = (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters);
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		LevelsProvider levelsProvider = (LevelsProvider)resolver.ResolveOrParameter(typeof(LevelsProvider), "levelsProvider", parameters);
		DataBase dataBase = (DataBase)resolver.ResolveOrParameter(typeof(DataBase), "dataBase", parameters);
		MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
		ICharacterEquipmentDataService characterEquipmentDataService = (ICharacterEquipmentDataService)resolver.ResolveOrParameter(typeof(ICharacterEquipmentDataService), "characterEquipmentDataService", parameters);
		IAccountController accountController = (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters);
		MultiPlayerMenu multiPlayerMenu = (MultiPlayerMenu)resolver.ResolveOrParameter(typeof(MultiPlayerMenu), "multiPlayerMenu", parameters);
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		IGarageSelectorService garageSelectorService = (IGarageSelectorService)resolver.ResolveOrParameter(typeof(IGarageSelectorService), "garageSelectorService", parameters);
		ProgressContainer progressContainer = (ProgressContainer)resolver.ResolveOrParameter(typeof(ProgressContainer), "progressContainer", parameters);
		IGarageBuyService garageBuyService = (IGarageBuyService)resolver.ResolveOrParameter(typeof(IGarageBuyService), "garageBuyService", parameters);
		InputMapSetEvent inputMapSetEvent = (InputMapSetEvent)resolver.ResolveOrParameter(typeof(InputMapSetEvent), "inputMapSetEvent", parameters);
		OpenWorldAnalytic openWorldAnalytic = (OpenWorldAnalytic)resolver.ResolveOrParameter(typeof(OpenWorldAnalytic), "openWorldAnalytic", parameters);
		OptionalDependency<GarageController> garageController = (OptionalDependency<GarageController>)resolver.ResolveOrParameter(typeof(OptionalDependency<GarageController>), "garageController", parameters);
		obj.Construct(playerDataStorage, freeDriveProvider, levelsProvider, dataBase, menuLevelParent, characterEquipmentDataService, accountController, multiPlayerMenu, managerUI, garageSelectorService, progressContainer, garageBuyService, inputMapSetEvent, openWorldAnalytic, garageController);
	}
}
