using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Scopes;

[Preserve]
internal class LoadControlGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:LoadControl cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		LoadControl obj = (LoadControl)instance;
		OptionalDependency<LevelScript> levelScript = (OptionalDependency<LevelScript>)resolver.ResolveOrParameter(typeof(OptionalDependency<LevelScript>), "levelScript", parameters);
		obj.Construct(levelScript);
	}
}
