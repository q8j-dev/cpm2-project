using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Steam.Achievements
{
	[Preserve]
	internal class SteamAchievementServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SteamAchievementService steamAchievementService = new SteamAchievementService();
			Inject(steamAchievementService, resolver, parameters);
			return steamAchievementService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
