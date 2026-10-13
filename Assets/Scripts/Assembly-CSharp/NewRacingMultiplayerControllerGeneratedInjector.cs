using System;
using System.Collections.Generic;
using Car_Parking.Scripts.Racing;
using VContainer;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Steam.Base;

[Preserve]
internal class NewRacingMultiplayerControllerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:NewRacingMultiplayerController cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		NewRacingMultiplayerController obj = (NewRacingMultiplayerController)instance;
		FreeDriveControl freeDriveControl = (FreeDriveControl)resolver.ResolveOrParameter(typeof(FreeDriveControl), "freeDriveControl", parameters);
		InputItemsProvider controlItemsProvider = (InputItemsProvider)resolver.ResolveOrParameter(typeof(InputItemsProvider), "controlItemsProvider", parameters);
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
		FreeTimeAttackController timeAttackController = (FreeTimeAttackController)resolver.ResolveOrParameter(typeof(FreeTimeAttackController), "timeAttackController", parameters);
		ISteamAchievementService steamAchievementService = (ISteamAchievementService)resolver.ResolveOrParameter(typeof(ISteamAchievementService), "steamAchievementService", parameters);
		obj.Construct(freeDriveControl, controlItemsProvider, freeDriveProvider, carInputManager, timeAttackController, steamAchievementService);
	}
}
