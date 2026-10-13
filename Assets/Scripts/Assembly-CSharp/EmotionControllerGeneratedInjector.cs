using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Animations.Emotes;
using _Car_Parking.Scripts.Providers;

[Preserve]
internal class EmotionControllerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:EmotionController cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		EmotionController obj = (EmotionController)instance;
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		EmoteCatalog emoteCatalog = (EmoteCatalog)resolver.ResolveOrParameter(typeof(EmoteCatalog), "emoteCatalog", parameters);
		obj.Construct(freeDriveProvider, emoteCatalog);
	}
}
