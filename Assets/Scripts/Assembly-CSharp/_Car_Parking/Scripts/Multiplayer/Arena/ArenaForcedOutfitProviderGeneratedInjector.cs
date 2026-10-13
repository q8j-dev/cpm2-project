using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	[Preserve]
	internal class ArenaForcedOutfitProviderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ArenaForcedOutfitProvider arenaForcedOutfitProvider = new ArenaForcedOutfitProvider((FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters), (ArenaModeRegistry)resolver.ResolveOrParameter(typeof(ArenaModeRegistry), "registry", parameters));
			Inject(arenaForcedOutfitProvider, resolver, parameters);
			return arenaForcedOutfitProvider;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
