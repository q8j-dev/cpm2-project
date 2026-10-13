using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Audio.SoundBanks;

namespace _Car_Parking.WwisePrefabs.WiseSoundScripts
{
	[Preserve]
	internal class AkAmbientCustomGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:AkAmbientCustom cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			AkAmbientCustom obj = (AkAmbientCustom)instance;
			SoundBankLoadService soundBankLoadService = (SoundBankLoadService)resolver.ResolveOrParameter(typeof(SoundBankLoadService), "soundBankLoadService", parameters);
			obj.Construct(soundBankLoadService);
		}
	}
}
