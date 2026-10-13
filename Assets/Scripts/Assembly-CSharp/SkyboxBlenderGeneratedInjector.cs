using System;
using System.Collections.Generic;
using VContainer;

[Preserve]
internal class SkyboxBlenderGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:SkyboxBlender cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		SkyboxBlender obj = (SkyboxBlender)instance;
		SceneControl sceneControl = (SceneControl)resolver.ResolveOrParameter(typeof(SceneControl), "sceneControl", parameters);
		obj.Construct(sceneControl);
	}
}
