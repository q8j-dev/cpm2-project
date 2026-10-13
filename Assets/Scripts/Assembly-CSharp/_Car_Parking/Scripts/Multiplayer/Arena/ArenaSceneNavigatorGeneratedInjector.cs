using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	[Preserve]
	internal class ArenaSceneNavigatorGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ArenaSceneNavigator arenaSceneNavigator = new ArenaSceneNavigator((IArenaNetwork)resolver.ResolveOrParameter(typeof(IArenaNetwork), "net", parameters), (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters), (CommonProvider)resolver.ResolveOrParameter(typeof(CommonProvider), "commonProvider", parameters));
			Inject(arenaSceneNavigator, resolver, parameters);
			return arenaSceneNavigator;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
