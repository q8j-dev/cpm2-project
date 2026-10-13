using System;
using CodeBase.Infrastructure.States;
using UnityEngine;
using VContainer;

namespace _Car_Parking.Scripts.Infrastructure.States
{
	public class InitFreeDriveState : IState, IExitableState
	{
		private readonly IGameStateMachine _gameStateMachine;

		private FreeDriveControl _freeDriveControl;

		private CoreGameManager _coreGameManager;

		private MenuLevelParent _menuLevelParent;

		private QuickCarSettings _quickCarSettings;

		private MapControl _mapControl;

		private MapCamera _mapCamera;

		private CarCameras _carCameras;

		private SceneControl _sceneControl;

		private AnimationShop _animationShop;

		[Inject]
		public InitFreeDriveState(IGameStateMachine gameStateMachine, FreeDriveControl freeDriveControl, CoreGameManager coreGameManager, MenuLevelParent menuLevelParent, QuickCarSettings quickCarSettings, MapControl mapControl, MapCamera mapCamera, CarCameras carCameras, SceneControl sceneControl, AnimationShop animationShop)
		{
			_animationShop = animationShop;
			_sceneControl = sceneControl;
			_carCameras = carCameras;
			_mapCamera = mapCamera;
			_mapControl = mapControl;
			_quickCarSettings = quickCarSettings;
			_menuLevelParent = menuLevelParent;
			_coreGameManager = coreGameManager;
			_freeDriveControl = freeDriveControl;
			_gameStateMachine = gameStateMachine;
		}

		public void Enter()
		{
			try
			{
				_mapCamera.InitStart();
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			try
			{
				_quickCarSettings.InitStart();
			}
			catch (Exception message2)
			{
				Debug.LogError(message2);
			}
			try
			{
				_carCameras.InitStart();
			}
			catch (Exception message3)
			{
				Debug.LogError(message3);
			}
			try
			{
				_mapControl.InitStart();
			}
			catch (Exception message4)
			{
				Debug.LogError(message4);
			}
			try
			{
				_sceneControl.InitStart();
			}
			catch (Exception message5)
			{
				Debug.LogError(message5);
			}
			try
			{
				_coreGameManager.InitStart();
			}
			catch (Exception message6)
			{
				Debug.LogError(message6);
			}
			try
			{
				_freeDriveControl.InitStart();
			}
			catch (Exception message7)
			{
				Debug.LogError(message7);
			}
			try
			{
				_gameStateMachine.Enter<GameLoopState>();
			}
			catch (Exception message8)
			{
				Debug.LogError(message8);
			}
		}

		public void Exit()
		{
		}
	}
}
