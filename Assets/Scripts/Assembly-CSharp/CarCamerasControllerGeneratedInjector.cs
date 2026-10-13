using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.WindowSystem.Mediators;

[Preserve]
internal class CarCamerasControllerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:CarCamerasController cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		CarCamerasController obj = (CarCamerasController)instance;
		LevelsProvider levelsProvider = (LevelsProvider)resolver.ResolveOrParameter(typeof(LevelsProvider), "levelsProvider", parameters);
		CoreGameManager coreGameManager = (CoreGameManager)resolver.ResolveOrParameter(typeof(CoreGameManager), "coreGameManager", parameters);
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
		CarCamerasParametersConfig camerasParametersConfig = (CarCamerasParametersConfig)resolver.ResolveOrParameter(typeof(CarCamerasParametersConfig), "camerasParametersConfig", parameters);
		OptionalDependency<UIReferencesMediator> freeUIMediator = (OptionalDependency<UIReferencesMediator>)resolver.ResolveOrParameter(typeof(OptionalDependency<UIReferencesMediator>), "freeUIMediator", parameters);
		OptionalDependency<ParkingMenuUI> parkingMenuUI = (OptionalDependency<ParkingMenuUI>)resolver.ResolveOrParameter(typeof(OptionalDependency<ParkingMenuUI>), "parkingMenuUI", parameters);
		SoundControllerExtension carInOutSound = (SoundControllerExtension)resolver.ResolveOrParameter(typeof(SoundControllerExtension), "carInOutSound", parameters);
		obj.Construct(levelsProvider, coreGameManager, freeDriveProvider, menuLevelParent, camerasParametersConfig, freeUIMediator, parkingMenuUI, carInOutSound);
	}
}
