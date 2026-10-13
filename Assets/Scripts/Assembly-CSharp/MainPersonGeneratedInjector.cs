using System;
using System.Collections.Generic;
using Racing.CameraManagement;
using VContainer;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Characters;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

[Preserve]
internal class MainPersonGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:MainPerson cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		MainPerson obj = (MainPerson)instance;
		CharactersService charactersService = (CharactersService)resolver.ResolveOrParameter(typeof(CharactersService), "charactersService", parameters);
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		PersonKitConfigs personKitConfigs = (PersonKitConfigs)resolver.ResolveOrParameter(typeof(PersonKitConfigs), "personKitConfigs", parameters);
		OptionalDependency<AnimationShop> animationShop = (OptionalDependency<AnimationShop>)resolver.ResolveOrParameter(typeof(OptionalDependency<AnimationShop>), "animationShop", parameters);
		OptionalDependency<CameraManager> cameraManager = (OptionalDependency<CameraManager>)resolver.ResolveOrParameter(typeof(OptionalDependency<CameraManager>), "cameraManager", parameters);
		IForcedOutfitProvider forcedOutfit = (IForcedOutfitProvider)resolver.ResolveOrParameter(typeof(IForcedOutfitProvider), "forcedOutfit", parameters);
		obj.Construct(charactersService, freeDriveProvider, personKitConfigs, animationShop, cameraManager, forcedOutfit);
	}
}
