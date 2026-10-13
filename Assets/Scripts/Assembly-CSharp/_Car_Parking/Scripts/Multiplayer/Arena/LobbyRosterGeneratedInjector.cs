using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	[Preserve]
	internal class LobbyRosterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			LobbyRoster lobbyRoster = new LobbyRoster((IArenaNetwork)resolver.ResolveOrParameter(typeof(IArenaNetwork), "net", parameters));
			Inject(lobbyRoster, resolver, parameters);
			return lobbyRoster;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
