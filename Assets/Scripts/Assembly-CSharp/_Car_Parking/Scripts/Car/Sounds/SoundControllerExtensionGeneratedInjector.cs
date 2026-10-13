using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Audio;

namespace _Car_Parking.Scripts.Car.Sounds
{
	[Preserve]
	internal class SoundControllerExtensionGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SoundControllerExtension soundControllerExtension = new SoundControllerExtension((RainSoundState)resolver.ResolveOrParameter(typeof(RainSoundState), "rainSoundState", parameters));
			Inject(soundControllerExtension, resolver, parameters);
			return soundControllerExtension;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
