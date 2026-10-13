using System;
using System.Collections.Generic;
using CPMEngine;
using VContainer;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

[Preserve]
internal class TrafficCarControlGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:TrafficCarControl cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		TrafficCarControl obj = (TrafficCarControl)instance;
		PlayerDatas.CarLoadingUtility carLoadingUtility = (PlayerDatas.CarLoadingUtility)resolver.ResolveOrParameter(typeof(PlayerDatas.CarLoadingUtility), "carLoadingUtility", parameters);
		LevelScript levelScript = (LevelScript)resolver.ResolveOrParameter(typeof(LevelScript), "levelScript", parameters);
		LevelsProvider levelsProvider = (LevelsProvider)resolver.ResolveOrParameter(typeof(LevelsProvider), "levelsProvider", parameters);
		CPMVehicleCore cpmVehicleCore = (CPMVehicleCore)resolver.ResolveOrParameter(typeof(CPMVehicleCore), "cpmVehicleCore", parameters);
		OptionalDependency<Sort2> sort = (OptionalDependency<Sort2>)resolver.ResolveOrParameter(typeof(OptionalDependency<Sort2>), "sort2", parameters);
		obj.Construct(carLoadingUtility, levelScript, levelsProvider, cpmVehicleCore, sort);
	}
}
