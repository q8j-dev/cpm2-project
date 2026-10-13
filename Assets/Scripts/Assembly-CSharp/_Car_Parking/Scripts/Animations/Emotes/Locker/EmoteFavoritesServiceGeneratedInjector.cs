using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.PlayerProfile;

namespace _Car_Parking.Scripts.Animations.Emotes.Locker
{
	[Preserve]
	internal class EmoteFavoritesServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EmoteFavoritesService emoteFavoritesService = new EmoteFavoritesService((IPlayerProfileService)resolver.ResolveOrParameter(typeof(IPlayerProfileService), "profile", parameters));
			Inject(emoteFavoritesService, resolver, parameters);
			return emoteFavoritesService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
