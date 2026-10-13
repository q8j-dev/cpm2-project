using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

[Preserve]
internal class FinishFlashControllerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:FinishFlashController cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		FinishFlashController obj = (FinishFlashController)instance;
		Camera camera = (Camera)resolver.ResolveOrParameter(typeof(Camera), "camera", parameters);
		obj.Construct(camera);
	}
}
