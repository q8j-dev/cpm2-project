using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Audio.SoundBanks;

namespace _Car_Parking.Scripts.Audio
{
	[Preserve]
	internal class RainSoundGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RainSound rainSound = new RainSound((SoundBankLoadService)resolver.ResolveOrParameter(typeof(SoundBankLoadService), "bankService", parameters), (RainSoundState)resolver.ResolveOrParameter(typeof(RainSoundState), "rainSoundState", parameters));
			Inject(rainSound, resolver, parameters);
			return rainSound;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
