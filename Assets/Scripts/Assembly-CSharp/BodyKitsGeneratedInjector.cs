using System;
using System.Collections.Generic;
using CPM.Scripts.VinylDrawer;
using VContainer;
using _Car_Parking.Scripts.Car.Configs;
using _Car_Parking.Scripts.Providers;

[Preserve]
internal class BodyKitsGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:BodyKits cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		BodyKits obj = (BodyKits)instance;
		VinylDrawer vinylDrawer = (VinylDrawer)resolver.ResolveOrParameter(typeof(VinylDrawer), "vinylDrawer", parameters);
		DefaultMaterialsConfig defaultMaterialsConfig = (DefaultMaterialsConfig)resolver.ResolveOrParameter(typeof(DefaultMaterialsConfig), "defaultMaterialsConfig", parameters);
		LevelsProvider levelsProvider = (LevelsProvider)resolver.ResolveOrParameter(typeof(LevelsProvider), "levelsProvider", parameters);
		obj.Construct(vinylDrawer, defaultMaterialsConfig, levelsProvider);
	}
}
