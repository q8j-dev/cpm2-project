using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Audio.SoundBanks
{
	[Preserve]
	internal class SoundBankLoadServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SoundBankLoadService soundBankLoadService = new SoundBankLoadService();
			Inject(soundBankLoadService, resolver, parameters);
			return soundBankLoadService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
