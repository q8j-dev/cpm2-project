using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Other.Helpers;

[Preserve]
internal class CircuitRacingUIControlGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:CircuitRacingUIControl cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		CircuitRacingUIControl obj = (CircuitRacingUIControl)instance;
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		CircuitRacingConfig circuitRacingConfig = (CircuitRacingConfig)resolver.ResolveOrParameter(typeof(CircuitRacingConfig), "circuitRacingConfig", parameters);
		IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
		IPlayerStorageService playerStorageService = (IPlayerStorageService)resolver.ResolveOrParameter(typeof(IPlayerStorageService), "playerStorageService", parameters);
		MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
		IPlayerStatusSystem playerStatusSystem = (IPlayerStatusSystem)resolver.ResolveOrParameter(typeof(IPlayerStatusSystem), "playerStatusSystem", parameters);
		LapRaceTrafficLightUI lapRaceTrafficLightUI = (LapRaceTrafficLightUI)resolver.ResolveOrParameter(typeof(LapRaceTrafficLightUI), "lapRaceTrafficLightUI", parameters);
		RacingCameraController racingCameraController = (RacingCameraController)resolver.ResolveOrParameter(typeof(RacingCameraController), "racingCameraController", parameters);
		obj.Construct(managerUI, circuitRacingConfig, prefabFactory, playerStorageService, menuLevelParent, playerStatusSystem, lapRaceTrafficLightUI, racingCameraController);
	}
}
