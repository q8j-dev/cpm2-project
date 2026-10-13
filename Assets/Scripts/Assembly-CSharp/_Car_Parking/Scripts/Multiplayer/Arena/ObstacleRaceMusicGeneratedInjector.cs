using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Audio.SoundBanks;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	[Preserve]
	internal class ObstacleRaceMusicGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ObstacleRaceMusic obstacleRaceMusic = new ObstacleRaceMusic((SoundBankLoadService)resolver.ResolveOrParameter(typeof(SoundBankLoadService), "bankService", parameters));
			Inject(obstacleRaceMusic, resolver, parameters);
			return obstacleRaceMusic;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
