using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	[Preserve]
	internal class ArenaPointsContainerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ArenaPointsContainer arenaPointsContainer = new ArenaPointsContainer((IEnumerable<ArenaPointsConfig>)resolver.ResolveOrParameter(typeof(IEnumerable<ArenaPointsConfig>), "configs", parameters));
			Inject(arenaPointsContainer, resolver, parameters);
			return arenaPointsContainer;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
