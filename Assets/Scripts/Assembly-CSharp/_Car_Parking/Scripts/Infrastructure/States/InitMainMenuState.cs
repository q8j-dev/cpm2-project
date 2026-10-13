using System;
using CPM.Database.Constructors;
using CodeBase.Infrastructure.States;
using Persistence.Systems;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.Infrastructure.States
{
	public class InitMainMenuState : IState, IExitableState
	{
		private readonly IGameStateMachine _gameStateMachine;

		private CoreGameManager _coreGameManager;

		private AnimationShop _animationShop;

		private CarCameras _carCameras;

		[Inject]
		public InitMainMenuState(IGameStateMachine gameStateMachine, CoreGameManager coreGameManager, CarCameras carCameras, OptionalDependency<AnimationShop> animationShop)
		{
			_carCameras = carCameras;
			_animationShop = animationShop.Value;
			_coreGameManager = coreGameManager;
			_gameStateMachine = gameStateMachine;
		}

		public void Enter()
		{
			ClearAllForMainMenu();
			try
			{
				_carCameras.InitStart();
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			try
			{
				_gameStateMachine.Enter<MainMenuState>();
			}
			catch (Exception message2)
			{
				Debug.LogError(message2);
			}
		}

		public void Exit()
		{
		}

		private static void ClearAllForMainMenu()
		{
			EasySaveDataHandler.ClearAllCache();
			FlagConstructor.Clear();
		}
	}
}
