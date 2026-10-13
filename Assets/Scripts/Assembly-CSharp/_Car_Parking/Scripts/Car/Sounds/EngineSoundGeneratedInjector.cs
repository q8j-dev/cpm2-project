using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Audio.SoundBanks;

namespace _Car_Parking.Scripts.Car.Sounds
{
	[Preserve]
	internal class EngineSoundGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:EngineSound cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EngineSound obj = (EngineSound)instance;
			SoundBankLoadService soundBankService = (SoundBankLoadService)resolver.ResolveOrParameter(typeof(SoundBankLoadService), "soundBankService", parameters);
			obj.Construct(soundBankService);
		}
	}
}
