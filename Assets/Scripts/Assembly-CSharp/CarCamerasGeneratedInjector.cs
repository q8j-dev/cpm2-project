using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

[Preserve]
internal class CarCamerasGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:CarCameras cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		CarCameras obj = (CarCameras)instance;
		MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		OptionalDependency<LevelScript> levelScript = (OptionalDependency<LevelScript>)resolver.ResolveOrParameter(typeof(OptionalDependency<LevelScript>), "levelScript", parameters);
		obj.Construct(menuLevelParent, freeDriveProvider, levelScript);
	}
}
