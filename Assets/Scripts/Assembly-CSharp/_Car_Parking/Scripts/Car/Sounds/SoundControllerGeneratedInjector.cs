using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Car.Sounds
{
	[Preserve]
	internal class SoundControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:SoundController cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SoundController obj = (SoundController)instance;
			SoundBankLoadService soundBankService = (SoundBankLoadService)resolver.ResolveOrParameter(typeof(SoundBankLoadService), "soundBankService", parameters);
			IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
			obj.Construct(soundBankService, prefabFactory);
		}
	}
}
