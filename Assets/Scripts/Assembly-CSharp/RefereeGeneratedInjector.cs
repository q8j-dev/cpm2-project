using System;
using System.Collections.Generic;
using VContainer;

[Preserve]
internal class RefereeGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:Referee cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		Referee obj = (Referee)instance;
		DragRacingTree dragRacingTree = (DragRacingTree)resolver.ResolveOrParameter(typeof(DragRacingTree), "dragRacingTree", parameters);
		obj.Construct(dragRacingTree);
	}
}
