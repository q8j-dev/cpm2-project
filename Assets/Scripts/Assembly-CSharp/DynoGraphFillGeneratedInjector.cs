using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Steam.Base;

[Preserve]
internal class DynoGraphFillGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:DynoGraphFill cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		DynoGraphFill obj = (DynoGraphFill)instance;
		DynoRunController dynoRunController = (DynoRunController)resolver.ResolveOrParameter(typeof(DynoRunController), "dynoRunController", parameters);
		CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
		ISteamAchievementService steamAchievementService = (ISteamAchievementService)resolver.ResolveOrParameter(typeof(ISteamAchievementService), "steamAchievementService", parameters);
		obj.Construct(dynoRunController, carInputManager, steamAchievementService);
	}
}
