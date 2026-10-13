using System;
using System.Collections.Generic;
using CPM.DailyTasks;
using CPM.Rating;
using Flags;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.WindowSystem.Mediators;

[Preserve]
internal class DragRacingUIGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:DragRacingUI cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		DragRacingUI obj = (DragRacingUI)instance;
		RaceFinishUI raceFinishUI = (RaceFinishUI)resolver.ResolveOrParameter(typeof(RaceFinishUI), "raceFinishUI", parameters);
		IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		FlagsDataSO flagsDataSo = (FlagsDataSO)resolver.ResolveOrParameter(typeof(FlagsDataSO), "flagsDataSo", parameters);
		UIReferencesMediator uiReferencesMediator = (UIReferencesMediator)resolver.ResolveOrParameter(typeof(UIReferencesMediator), "uiReferencesMediator", parameters);
		InputItemsProvider inputItemsProvider = (InputItemsProvider)resolver.ResolveOrParameter(typeof(InputItemsProvider), "inputItemsProvider", parameters);
		CoreGameManager coreGameManager = (CoreGameManager)resolver.ResolveOrParameter(typeof(CoreGameManager), "coreGameManager", parameters);
		MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
		RacingProvider racingProvider = (RacingProvider)resolver.ResolveOrParameter(typeof(RacingProvider), "racingProvider", parameters);
		RacingAnalytic racingAnalytic = (RacingAnalytic)resolver.ResolveOrParameter(typeof(RacingAnalytic), "racingAnalytic", parameters);
		CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
		RatingSender ratingController = (RatingSender)resolver.ResolveOrParameter(typeof(RatingSender), "ratingController", parameters);
		DailyTasksManager dailyTasksManager = (DailyTasksManager)resolver.ResolveOrParameter(typeof(DailyTasksManager), "dailyTasksManager", parameters);
		EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
		InputMapSetEvent inputMapSetEvent = (InputMapSetEvent)resolver.ResolveOrParameter(typeof(InputMapSetEvent), "inputMapSetEvent", parameters);
		obj.Construct(raceFinishUI, prefabFactory, managerUI, flagsDataSo, uiReferencesMediator, inputItemsProvider, coreGameManager, menuLevelParent, racingProvider, racingAnalytic, carInputManager, ratingController, dailyTasksManager, eventRegistry, inputMapSetEvent);
	}
}
