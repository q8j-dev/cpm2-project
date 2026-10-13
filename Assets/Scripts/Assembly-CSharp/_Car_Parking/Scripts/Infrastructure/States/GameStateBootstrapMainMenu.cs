using CodeBase.Infrastructure.States;
using Photon.Pun;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Multiplayer.PhotonCustom;

namespace _Car_Parking.Scripts.Infrastructure.States
{
	public class GameStateBootstrapMainMenu : MonoBehaviour
	{
		private IGameStateMachine _gameStateMachine;

		private IObjectResolver _container;

		[Inject]
		public void Construct(IGameStateMachine gameStateMachine, IObjectResolver container)
		{
			_container = container;
			_gameStateMachine = gameStateMachine;
		}

		private void Start()
		{
			_gameStateMachine.Enter<InitMainMenuState>();
		}

		private void Awake()
		{
			PhotonNetwork.PrefabPool = new VContainerPrefabPool(_container);
		}
	}
}
