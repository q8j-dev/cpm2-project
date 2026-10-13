using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

[Preserve]
internal class RaceFinishUIGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:RaceFinishUI cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		RaceFinishUI obj = (RaceFinishUI)instance;
		OptionalDependency<DragRacingUI> dragRacingUI = (OptionalDependency<DragRacingUI>)resolver.ResolveOrParameter(typeof(OptionalDependency<DragRacingUI>), "dragRacingUI", parameters);
		SoundEventChannel soundEventChannel = (SoundEventChannel)resolver.ResolveOrParameter(typeof(SoundEventChannel), "soundEventChannel", parameters);
		RacingProvider racingProvider = (RacingProvider)resolver.ResolveOrParameter(typeof(RacingProvider), "racingProvider", parameters);
		obj.Construct(dragRacingUI, soundEventChannel, racingProvider);
	}
}
