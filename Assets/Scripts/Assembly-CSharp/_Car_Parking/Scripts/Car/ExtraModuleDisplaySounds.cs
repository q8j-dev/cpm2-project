using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Audio.SoundBanks;

namespace _Car_Parking.Scripts.Car
{
	public abstract class ExtraModuleDisplaySounds : VehicleDataExtraModule
	{
		private readonly uint _playSlideOpenDashboardID = AudioWrapper.GetId("Play_Slide_Open_Dashboard");

		private readonly uint _playFoldOpenDashboardID = AudioWrapper.GetId("Play_Fold_Open_Dashboard");

		private readonly uint _playSlideCloseDashboardID = AudioWrapper.GetId("Play_Slide_Close_Dashboard");

		private readonly uint _playFoldCloseDashboardID = AudioWrapper.GetId("Play_Fold_Close_Dashboard");

		private CancellationTokenSource _cancellationTokenSourceSlideOpenDashboard;

		private CancellationTokenSource _cancellationTokenSourceFoldOpenDashboard;

		private CancellationTokenSource _cancellationTokenSourceSlideCloseDashboard;

		private CancellationTokenSource _cancellationTokenSourceFoldCloseDashboard;

		private GameObject _soundObject;

		protected abstract float DelaySlideOpenDashboard { get; }

		protected abstract float DelayFoldOpenDashboard { get; }

		protected abstract float DelaySlideCloseDashboard { get; }

		protected abstract float DelayFoldCloseDashboard { get; }

		private GameObject SoundObject
		{
			get
			{
				if ((bool)_soundObject)
				{
					return _soundObject;
				}
				if (!vehicleData)
				{
					return null;
				}
				if ((bool)vehicleData.BodyLoadingComposition.CarRetractableDisplay)
				{
					_soundObject = new GameObject("Sound");
					_soundObject.transform.SetParent(vehicleData.BodyLoadingComposition.CarRetractableDisplay.transform);
					_soundObject.transform.localPosition = Vector3.zero;
					_soundObject.transform.localRotation = Quaternion.identity;
					_soundObject.transform.localScale = Vector3.one;
					_soundObject.layer = LayerMask.NameToLayer("Water");
					_soundObject.AddComponent<SphereCollider>().isTrigger = true;
					_soundObject.AddComponent<Rigidbody>().isKinematic = true;
					return _soundObject;
				}
				return vehicleData.gameObject;
			}
		}

		protected ExtraModuleDisplaySounds(VehicleData vehicleData)
			: base(vehicleData)
		{
		}

		public override void Dispose()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceSlideOpenDashboard);
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceFoldOpenDashboard);
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceSlideCloseDashboard);
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceFoldCloseDashboard);
		}

		public override void EnableCarDisplay(bool isEngineOn)
		{
			base.EnableCarDisplay(isEngineOn);
			if ((bool)vehicleData && (bool)vehicleData.BodyLoadingComposition.CarRetractableDisplay)
			{
				if (isEngineOn)
				{
					CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceSlideOpenDashboard);
					CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceFoldOpenDashboard);
					_cancellationTokenSourceSlideOpenDashboard = new CancellationTokenSource();
					_cancellationTokenSourceFoldOpenDashboard = new CancellationTokenSource();
					PlaySound(DelaySlideOpenDashboard, _playSlideOpenDashboardID, _cancellationTokenSourceSlideOpenDashboard.Token).Forget();
					PlaySound(DelayFoldOpenDashboard, _playFoldOpenDashboardID, _cancellationTokenSourceFoldOpenDashboard.Token).Forget();
				}
				else
				{
					CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceSlideCloseDashboard);
					CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceFoldCloseDashboard);
					_cancellationTokenSourceSlideCloseDashboard = new CancellationTokenSource();
					_cancellationTokenSourceFoldCloseDashboard = new CancellationTokenSource();
					PlaySound(DelaySlideCloseDashboard, _playSlideCloseDashboardID, _cancellationTokenSourceSlideCloseDashboard.Token).Forget();
					PlaySound(DelayFoldCloseDashboard, _playFoldCloseDashboardID, _cancellationTokenSourceFoldCloseDashboard.Token).Forget();
				}
			}
		}

		private async UniTaskVoid PlaySound(float delay, uint nameSound, CancellationToken cancellationToken)
		{
			if (delay > 0f)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			}
			if (!cancellationToken.IsCancellationRequested)
			{
				vehicleData.SoundBankService?.PostEvent(nameSound, SoundBankType.CarDashboard, SoundObject).Forget();
			}
		}
	}
}
