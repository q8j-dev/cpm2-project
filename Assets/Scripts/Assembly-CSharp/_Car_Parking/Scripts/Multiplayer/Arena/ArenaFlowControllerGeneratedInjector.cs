using System.Collections.Generic;
using CPM.Fallout;
using CPM.PlayerData;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	[Preserve]
	internal class ArenaFlowControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ArenaFlowController arenaFlowController = new ArenaFlowController((IArenaNetwork)resolver.ResolveOrParameter(typeof(IArenaNetwork), "net", parameters), (ArenaModeRegistry)resolver.ResolveOrParameter(typeof(ArenaModeRegistry), "registry", parameters), (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters), (ArenaSceneNavigator)resolver.ResolveOrParameter(typeof(ArenaSceneNavigator), "navigator", parameters), (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerData", parameters), (FalloutRaceSession)resolver.ResolveOrParameter(typeof(FalloutRaceSession), "raceSession", parameters), (ArenaForcedOutfitProvider)resolver.ResolveOrParameter(typeof(ArenaForcedOutfitProvider), "forcedOutfit", parameters));
			Inject(arenaFlowController, resolver, parameters);
			return arenaFlowController;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
