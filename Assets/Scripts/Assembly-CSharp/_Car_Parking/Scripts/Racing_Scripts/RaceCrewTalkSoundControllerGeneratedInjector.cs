using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Audio.SoundBanks;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	[Preserve]
	internal class RaceCrewTalkSoundControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RaceCrewTalkSoundController raceCrewTalkSoundController = new RaceCrewTalkSoundController();
			Inject(raceCrewTalkSoundController, resolver, parameters);
			return raceCrewTalkSoundController;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RaceCrewTalkSoundController obj = (RaceCrewTalkSoundController)instance;
			SoundBankLoadService soundBankLoadService = (SoundBankLoadService)resolver.ResolveOrParameter(typeof(SoundBankLoadService), "soundBankLoadService", parameters);
			obj.Construct(soundBankLoadService);
		}
	}
}
