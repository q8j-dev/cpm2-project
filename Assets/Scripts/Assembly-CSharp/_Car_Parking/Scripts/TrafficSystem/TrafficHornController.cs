using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gley.TrafficSystem;
using UnityEngine;

namespace _Car_Parking.Scripts.TrafficSystem
{
	public class TrafficHornController : IDisposable
	{
		private const float FirstHortCarCloseDelayMin = 6f;

		private const float FirstHortCarCloseDelayMax = 10f;

		private const float NextHortCarCloseDelayMin = 8f;

		private const float NextHortCarCloseDelayMax = 15f;

		private const float SpeedToHorn = 5f;

		private const float MinCollisionImpulseToHorn = 625f;

		private readonly VehicleComponent _vehicleComponent;

		private readonly TrafficSoundComponent _trafficSoundComponent;

		private CancellationTokenSource _cancellationTokenSourceCarInFront;

		private CancellationTokenSource _cancellationTokenSourceMain;

		private List<int> _triggerCars = new List<int>();

		private bool _isHornPlaying;

		public TrafficHornController(VehicleComponent vehicleComponent, TrafficSoundComponent trafficSoundComponent)
		{
			_vehicleComponent = vehicleComponent;
			_trafficSoundComponent = trafficSoundComponent;
			_cancellationTokenSourceMain = new CancellationTokenSource();
		}

		public void Dispose()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceCarInFront);
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceMain);
		}

		public void OnEnable()
		{
		}

		public void OnDisable()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceCarInFront);
			_triggerCars.Clear();
		}

		public void OnTriggerEnter(Collider other)
		{
			if (!other.gameObject.CompareTag("Car") || other.isTrigger)
			{
				return;
			}
			if (_vehicleComponent.GetCurrentSpeed() > 5f)
			{
				PlayHornAsync(_cancellationTokenSourceMain.Token).Forget();
			}
			if (!_triggerCars.Contains(other.gameObject.GetInstanceID()))
			{
				_triggerCars.Add(other.gameObject.GetInstanceID());
				if (_triggerCars.Count == 1)
				{
					CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceCarInFront);
					_cancellationTokenSourceCarInFront = new CancellationTokenSource();
					CarInFrontProcessAsync(_cancellationTokenSourceCarInFront.Token).Forget();
				}
			}
		}

		public void OnTriggerExit(Collider other)
		{
			if (other.gameObject.CompareTag("Car") && !other.isTrigger)
			{
				_triggerCars.Remove(other.gameObject.GetInstanceID());
				if (_triggerCars.Count == 0)
				{
					CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceCarInFront);
				}
			}
		}

		public void OnCollisionEnter(Collision collInfo)
		{
			if (collInfo.contacts.Length != 0 && collInfo.contacts[0].otherCollider.gameObject.CompareTag("Car") && collInfo.contacts[0].impulse.sqrMagnitude > 625f)
			{
				PlayHornAsync(_cancellationTokenSourceMain.Token).Forget();
			}
		}

		private async UniTaskVoid PlayHornAsync(CancellationToken cancellationToken)
		{
			if (_isHornPlaying)
			{
				return;
			}
			_isHornPlaying = true;
			_trafficSoundComponent.PlayHorn();
			bool startState = _vehicleComponent.LightsOn;
			for (int i = 0; i < 6; i++)
			{
				_vehicleComponent.SetMainLights(i % 2 == 0);
				await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}
			}
			_vehicleComponent.SetMainLights(startState);
			_isHornPlaying = false;
		}

		private async UniTaskVoid CarInFrontProcessAsync(CancellationToken cancellationToken)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(UnityEngine.Random.Range(6f, 10f)), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
			while (_triggerCars.Count > 0)
			{
				PlayHornAsync(_cancellationTokenSourceMain.Token).Forget();
				await UniTask.Delay(TimeSpan.FromSeconds(UnityEngine.Random.Range(8f, 15f)), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
				if (cancellationToken.IsCancellationRequested)
				{
					break;
				}
			}
		}
	}
}
