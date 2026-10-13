using System.Collections.Generic;
using MVPFree.MapIcons.ScrObjs.Scripts;
using VContainer;
using _Car_Parking.Scripts.Multiplayer.Arena;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.GameActivity.Providers
{
	[Preserve]
	internal class ArenaActivityProviderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ArenaActivityProvider arenaActivityProvider = new ArenaActivityProvider((ArenaModeRegistry)resolver.ResolveOrParameter(typeof(ArenaModeRegistry), "registry", parameters), (OptionalDependency<MapIconPositionsDataSo>)resolver.ResolveOrParameter(typeof(OptionalDependency<MapIconPositionsDataSo>), "positions", parameters));
			Inject(arenaActivityProvider, resolver, parameters);
			return arenaActivityProvider;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
