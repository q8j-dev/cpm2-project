using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Audio;

[Preserve]
internal class RainStateZoneGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:RainStateZone cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		RainStateZone obj = (RainStateZone)instance;
		RainSoundState rainSoundState = (RainSoundState)resolver.ResolveOrParameter(typeof(RainSoundState), "rainSoundState", parameters);
		obj.Construct(rainSoundState);
	}
}
