using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.PlayerProfile
{
	[Preserve]
	internal class PlayerProfileCacheGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PlayerProfileCache playerProfileCache = new PlayerProfileCache();
			Inject(playerProfileCache, resolver, parameters);
			return playerProfileCache;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
