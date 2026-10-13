using System;
using System.Collections.Generic;
using VContainer;

[Preserve]
internal class dynoSceneParentGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:dynoSceneParent cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		dynoSceneParent obj = (dynoSceneParent)instance;
		DynoRunController dynoRunController = (DynoRunController)resolver.ResolveOrParameter(typeof(DynoRunController), "dynoRunController", parameters);
		obj.Construct(dynoRunController);
	}
}
