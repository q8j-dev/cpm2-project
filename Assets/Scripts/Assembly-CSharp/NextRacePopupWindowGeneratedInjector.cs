using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Scopes;

[Preserve]
internal class NextRacePopupWindowGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:NextRacePopupWindow cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		NextRacePopupWindow obj = (NextRacePopupWindow)instance;
		OptionalDependency<DragRacingController> dragRacingController = (OptionalDependency<DragRacingController>)resolver.ResolveOrParameter(typeof(OptionalDependency<DragRacingController>), "dragRacingController", parameters);
		obj.Construct(dragRacingController);
	}
}
