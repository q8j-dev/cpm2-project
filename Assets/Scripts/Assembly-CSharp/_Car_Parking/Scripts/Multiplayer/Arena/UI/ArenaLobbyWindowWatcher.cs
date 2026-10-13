using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.GameFacts.Base;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.Multiplayer.Arena.UI
{
	public sealed class ArenaLobbyWindowWatcher : IStartable, IDisposable
	{
		private const float CameraWaitTimeoutSeconds = 10f;

		private readonly IGameFactManager _gameFactManager;

		private readonly ManagerUI _managerUI;

		private readonly Camera _mainCamera;

		private readonly InputMapSetEvent _inputMapSetEvent;

		private CancellationTokenSource _cts;

		private WindowScript _window;

		private bool _opening;

		public ArenaLobbyWindowWatcher(IGameFactManager gameFactManager, ManagerUI managerUI, Camera mainCamera, InputMapSetEvent inputMapSetEvent)
		{
			_gameFactManager = gameFactManager;
			_managerUI = managerUI;
			_mainCamera = mainCamera;
			_inputMapSetEvent = inputMapSetEvent;
		}

		public void Start()
		{
			_cts = new CancellationTokenSource();
			_gameFactManager.OnChanged += OnFactChanged;
			Evaluate();
		}

		public void Dispose()
		{
			_gameFactManager.OnChanged -= OnFactChanged;
			CommonExtensions.CancelAndDisposeToken(ref _cts);
			Hide();
		}

		private void OnFactChanged(GameFact gameFact)
		{
			if (gameFact == GameFact.ArenaGatheringPlayers)
			{
				Evaluate();
			}
		}

		private void Evaluate()
		{
			if (_gameFactManager.IsTrue(GameFact.ArenaGatheringPlayers))
			{
				ApplyArenaInputGroup();
				Show().Forget();
			}
			else
			{
				Hide();
			}
		}

		private async UniTaskVoid Show()
		{
			if ((bool)_window || _opening)
			{
				return;
			}
			_opening = true;
			try
			{
				await UniTask.NextFrame(_cts.Token);
				float deadline = Time.realtimeSinceStartup + 10f;
				await UniTask.WaitUntil(() => HasRenderingCamera() || Time.realtimeSinceStartup >= deadline, PlayerLoopTiming.Update, _cts.Token).SuppressCancellationThrow();
				if (_cts.IsCancellationRequested || !_gameFactManager.IsTrue(GameFact.ArenaGatheringPlayers))
				{
					return;
				}
				_window = await _managerUI.ShowWindow("ArenaLobbyWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false, null, withAnimate: false, updateBackButton: true, trackForCleanup: false);
			}
			catch (OperationCanceledException)
			{
			}
			finally
			{
				_opening = false;
			}
			if (!_gameFactManager.IsTrue(GameFact.ArenaGatheringPlayers))
			{
				Hide();
			}
		}

		private void Hide()
		{
			if ((bool)_window)
			{
				_window.Close();
				_window = null;
			}
		}

		private void ApplyArenaInputGroup()
		{
			_inputMapSetEvent?.TrySetGroup(InputGroupsEnum.Person, setRoot: true);
		}

		private bool HasRenderingCamera()
		{
			if ((bool)_mainCamera && _mainCamera.isActiveAndEnabled)
			{
				return true;
			}
			Camera main = Camera.main;
			if ((bool)main)
			{
				return main.isActiveAndEnabled;
			}
			return false;
		}
	}
}
