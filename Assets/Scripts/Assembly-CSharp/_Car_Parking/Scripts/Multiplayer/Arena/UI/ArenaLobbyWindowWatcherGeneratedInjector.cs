using System.Collections.Generic;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.GameFacts.Base;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.Multiplayer.Arena.UI
{
	[Preserve]
	internal class ArenaLobbyWindowWatcherGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ArenaLobbyWindowWatcher arenaLobbyWindowWatcher = new ArenaLobbyWindowWatcher((IGameFactManager)resolver.ResolveOrParameter(typeof(IGameFactManager), "gameFactManager", parameters), (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters), (Camera)resolver.ResolveOrParameter(typeof(Camera), "mainCamera", parameters), (InputMapSetEvent)resolver.ResolveOrParameter(typeof(InputMapSetEvent), "inputMapSetEvent", parameters));
			Inject(arenaLobbyWindowWatcher, resolver, parameters);
			return arenaLobbyWindowWatcher;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
