using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Audio.SoundBanks;

[Preserve]
internal class EventPositionConfinerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:EventPositionConfiner cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		EventPositionConfiner obj = (EventPositionConfiner)instance;
		SoundBankLoadService soundBankService = (SoundBankLoadService)resolver.ResolveOrParameter(typeof(SoundBankLoadService), "soundBankService", parameters);
		obj.Construct(soundBankService);
	}
}
