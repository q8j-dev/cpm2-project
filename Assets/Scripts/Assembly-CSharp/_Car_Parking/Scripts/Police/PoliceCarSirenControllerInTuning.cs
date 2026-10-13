using System;
using System.Threading;
using CPM.Database;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace _Car_Parking.Scripts.Police
{
	public class PoliceCarSirenControllerInTuning : MonoBehaviour
	{
		[SerializeField]
		private PoliceCarBodyKitsUpdater PoliceCarBodyKitsUpdater;

		[SerializeField]
		private PoliceCarSirenController PoliceCarSirenController;

		private IPoliceSystem _policeSystem;

		private PoliceSystemConfig _policeSystemConfig;

		private VehicleData _vehicleData;

		private float _timeAnimationDisable;

		private float _timeSoundDisable;

		private CancellationTokenSource _cancellationTokenSourceAnimation;

		private CancellationTokenSource _cancellationTokenStopSoundAnimation;

		private bool IsAnimation => PoliceCarSirenController.SirenActive;

		[Inject]
		public void Construct(IPoliceSystem policeSystem, PoliceSystemConfig policeSystemConfig)
		{
			_policeSystem = policeSystem;
			_policeSystemConfig = policeSystemConfig;
		}

		public void Init(VehicleData vehicleData)
		{
			_vehicleData = vehicleData;
		}

		public void StartAnimation(bool playSound)
		{
			_timeAnimationDisable = Time.time + _policeSystemConfig.UI.UISirenAnimationDuration;
			if (!IsAnimation)
			{
				_cancellationTokenSourceAnimation = new CancellationTokenSource();
				AnimationAsync(playSound, _cancellationTokenSourceAnimation.Token).Forget();
			}
			else
			{
				PoliceCarSirenController.StartSirens(playSound);
			}
			if (playSound)
			{
				_timeSoundDisable = Time.time + _policeSystemConfig.UI.UISirenAnimationDuration;
			}
			CancelStopSoundAsync();
			_cancellationTokenStopSoundAnimation = new CancellationTokenSource();
			StopSoundAsync(_cancellationTokenStopSoundAnimation.Token).Forget();
		}

		public void SetTempSoundIndex(int index)
		{
			PoliceCarSirenController.SetTempSirenSoundIndex(index);
		}

		public void StartSirenSound()
		{
			PoliceCarSirenController.StartSirenSound();
		}

		public void RemoveTempSirenSound()
		{
			PoliceCarSirenController.RemoveTempSirenSound();
		}

		private void OnEnable()
		{
			PoliceCarBodyKitsUpdater.OnNewBodyKit += OnNewBodyKit;
			CarInfoPoliceExtensionInstall.OnInstallUpdated += OnPoliceSystemUpdated;
		}

		private void OnDisable()
		{
			PoliceCarBodyKitsUpdater.OnNewBodyKit -= OnNewBodyKit;
			CarInfoPoliceExtensionInstall.OnInstallUpdated -= OnPoliceSystemUpdated;
			CancelAnimationAsync();
		}

		private void OnNewBodyKit(PoliceCarBodyKitsUpdater policeCarBodyKitsUpdater, PoliceCarBodyKit policeCarBodyKit)
		{
			if (IsAnimation)
			{
				policeCarBodyKit.StartLampAnimation();
			}
			else
			{
				policeCarBodyKit.StopLampAnimation();
			}
		}

		private void OnPoliceSystemUpdated(PoliceData policeData, bool isInstalled)
		{
			if ((bool)_vehicleData && _vehicleData.CarInfoProvider.CarInfo.AdditionalData.Police == policeData)
			{
				if (isInstalled)
				{
					StartAnimation(playSound: true);
				}
				else
				{
					StopAnimation();
				}
			}
		}

		private void StopAnimation()
		{
			CancelAnimationAsync();
			PoliceCarSirenController.StopSirens();
		}

		private async UniTaskVoid AnimationAsync(bool playSound, CancellationToken cancellationToken)
		{
			PoliceCarSirenController.StartSirens(playSound);
			while (Time.time < _timeAnimationDisable && !cancellationToken.IsCancellationRequested)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(0.25), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			}
			if (!cancellationToken.IsCancellationRequested)
			{
				PoliceCarSirenController.StopSirens();
			}
		}

		private async UniTaskVoid StopSoundAsync(CancellationToken cancellationToken)
		{
			while (Time.time < _timeSoundDisable && !cancellationToken.IsCancellationRequested)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(0.25), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			}
			if (!cancellationToken.IsCancellationRequested)
			{
				PoliceCarSirenController.StopSirenSound();
			}
		}

		private void CancelAnimationAsync()
		{
			_cancellationTokenSourceAnimation?.Cancel();
			_cancellationTokenSourceAnimation?.Dispose();
			_cancellationTokenSourceAnimation = null;
			CancelStopSoundAsync();
		}

		private void CancelStopSoundAsync()
		{
			_cancellationTokenStopSoundAnimation?.Cancel();
			_cancellationTokenStopSoundAnimation?.Dispose();
			_cancellationTokenStopSoundAnimation = null;
		}
	}
}
