using System;
using System.Collections.Generic;
using VContainer;

[Preserve]
internal class BirdPointGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:BirdPoint cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		BirdPoint obj = (BirdPoint)instance;
		LevelScript levelScript = (LevelScript)resolver.ResolveOrParameter(typeof(LevelScript), "levelScript", parameters);
		obj.Construct(levelScript);
	}
}
