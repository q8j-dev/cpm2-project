using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Scopes;

[Preserve]
internal class tournirmentProgressBarGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:tournirmentProgressBar cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		tournirmentProgressBar obj = (tournirmentProgressBar)instance;
		OptionalDependency<DragRacingController> dragRacingController = (OptionalDependency<DragRacingController>)resolver.ResolveOrParameter(typeof(OptionalDependency<DragRacingController>), "dragRacingController", parameters);
		obj.Construct(dragRacingController);
	}
}
