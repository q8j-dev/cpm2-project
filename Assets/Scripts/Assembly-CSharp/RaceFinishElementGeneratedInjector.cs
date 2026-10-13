using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Scopes;

[Preserve]
internal class RaceFinishElementGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:RaceFinishElement cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		RaceFinishElement obj = (RaceFinishElement)instance;
		OptionalDependency<DragRacingUI> dragRacingUI = (OptionalDependency<DragRacingUI>)resolver.ResolveOrParameter(typeof(OptionalDependency<DragRacingUI>), "dragRacingUI", parameters);
		IPlayerStorageService playerStorageService = (IPlayerStorageService)resolver.ResolveOrParameter(typeof(IPlayerStorageService), "playerStorageService", parameters);
		obj.Construct(dragRacingUI, playerStorageService);
	}
}
