using System;
using System.Threading;
using CPM.Scripts.Garage;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Racing.CameraManagement
{
	public class CustomMoveCameraToTarget
	{
		private const float DelayForBlendingGarageCamera = 1f;

		private GaragePositionsConfig _garagePositionsConfig;

		private CameraManager _cameraManager;

		private IGarageSelectorService _garageSelectorService;

		private CinemachineBrain _cinemachineBrain;

		private TouchMovePerson _touchMovePerson;

		private Transform _target;

		private CinemachineVirtualCamera _temporaryCamera;

		public CancellationTokenSource CancellationTokenSource { get; private set; }

		public bool Moving { get; private set; }

		[Inject]
		public void Construct(GaragePositionsConfig garagePositionsConfig, CameraManager cameraManager, IGarageSelectorService garageSelectorService, CinemachineBrain cinemachineBrain)
		{
			_garagePositionsConfig = garagePositionsConfig;
			_cameraManager = cameraManager;
			_garageSelectorService = garageSelectorService;
			_cinemachineBrain = cinemachineBrain;
			_target = new GameObject("Target").transform;
		}

		public async UniTask PlayCustomBlendToSellCar()
		{
			StopCustomMoveCam();
			await UniTask.Yield();
			if (!_cameraManager)
			{
				return;
			}
			Moving = true;
			CancellationTokenSource = new CancellationTokenSource();
			_target.position = _garagePositionsConfig.GaragePositionsDatas[_garageSelectorService.CurrentGarageIndex].SellCarPointForCustomMoveCam.Position;
			_cameraManager.ActivateCamera(RacingCameraType.BuyCar, _target, null, null, useFade: true, stopCustomMove: false);
			if (!_touchMovePerson)
			{
				_touchMovePerson = (V.DriverPerson ? V.DriverPerson.GetComponent<TouchMovePerson>() : null);
			}
			if ((bool)_touchMovePerson)
			{
				_touchMovePerson.enabled = false;
			}
			while (!V.CoreGameManager.SellCarModel)
			{
				await UniTask.Yield(CancellationTokenSource.Token);
				if (CancellationTokenSource.IsCancellationRequested || !_cameraManager)
				{
					return;
				}
			}
			_cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0f);
			_temporaryCamera = new GameObject("TempBlendCam").AddComponent<CinemachineVirtualCamera>();
			_temporaryCamera.enabled = true;
			_temporaryCamera.Priority = 100;
			_temporaryCamera.transform.position = V.CoreGameManager.transform.position;
			_temporaryCamera.transform.rotation = V.CoreGameManager.transform.rotation;
			_temporaryCamera.PreviousStateIsValid = true;
			_cinemachineBrain.ManualUpdate();
			Moving = false;
			if ((bool)_touchMovePerson)
			{
				_touchMovePerson.enabled = true;
			}
			_temporaryCamera.Priority = 0;
			_cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseOut, 2f);
			_cinemachineBrain.ManualUpdate();
			_cameraManager.garageCam.transform.position = _target.position;
			_cameraManager.ActivateCamera(RacingCameraType.GarageLookAround, V.CoreGameManager.SellCarModel.transform, new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Linear, 1f), null, useFade: true, stopCustomMove: false);
			_cameraManager.garageCam.ResetCameraForSellCar();
			await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, CancellationTokenSource.Token);
			if (!CancellationTokenSource.IsCancellationRequested && (bool)_cameraManager && (bool)_temporaryCamera)
			{
				UnityEngine.Object.Destroy(_temporaryCamera.gameObject);
			}
		}

		public void StopCustomMoveCam()
		{
			Moving = false;
			CancellationTokenSource?.Cancel();
			CancellationTokenSource?.Dispose();
			CancellationTokenSource = null;
			if ((bool)_touchMovePerson)
			{
				_touchMovePerson.enabled = true;
			}
			if ((bool)_temporaryCamera)
			{
				UnityEngine.Object.Destroy(_temporaryCamera.gameObject);
			}
		}
	}
}
