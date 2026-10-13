using System;
using System.Threading;
using CPMEngine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Car.Physics;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.CPMEngine
{
	public class ForceFeedback : IStartable, ITickable, IDisposable
	{
		private Powertrain _chassis;

		private bool _setupComplete;

		private SteeringWheelValueEvent _steeringWheelValueEvent;

		private CancellationTokenSource _retryCts;

		private readonly CancellationTokenSource _lifetimeCts = new CancellationTokenSource();

		private bool HasFFBDevice => _steeringWheelValueEvent.StoredValue;

		[Inject]
		public void Construct(SteeringWheelValueEvent steeringWheelValueEvent)
		{
			_steeringWheelValueEvent = steeringWheelValueEvent;
		}

		public void Start()
		{
			ScheduleStart(_steeringWheelValueEvent.StoredValue);
			_steeringWheelValueEvent.Subscribe(ScheduleStart);
		}

		public void Dispose()
		{
			CancelPendingRetry();
			_lifetimeCts.Cancel();
			_lifetimeCts.Dispose();
			_steeringWheelValueEvent?.Unsubscribe(ScheduleStart);
			ShutDownForceFeedback();
			_chassis = null;
		}

		public void SetCar(Powertrain powertrain)
		{
			_chassis = powertrain;
		}

		public void ClearCar()
		{
			_chassis = null;
		}

		private void ScheduleStart(bool hasSteeringWheel)
		{
			CancelPendingRetry();
			if (!hasSteeringWheel)
			{
				ShutDownForceFeedback();
				return;
			}
			if ((bool)V.Car && V.Car.TryGetComponent<Powertrain>(out _chassis))
			{
				InitForceFeedback();
				return;
			}
			_retryCts = CancellationTokenSource.CreateLinkedTokenSource(_lifetimeCts.Token);
			RetryScheduleStartAsync(_retryCts.Token).Forget();
		}

		private async UniTaskVoid RetryScheduleStartAsync(CancellationToken ct)
		{
			try
			{
				await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			}
			catch (OperationCanceledException)
			{
				return;
			}
			ScheduleStart(_steeringWheelValueEvent.StoredValue);
		}

		private void CancelPendingRetry()
		{
			if (_retryCts != null)
			{
				_retryCts.Cancel();
				_retryCts.Dispose();
				_retryCts = null;
			}
		}

		public void Tick()
		{
			if ((bool)_chassis && _setupComplete && HasFFBDevice)
			{
				float value = (0f - _chassis.SteerAlignMom) * _steeringWheelValueEvent.MomentCoefficient * _steeringWheelValueEvent.Multiplier;
				value = Mathf.Clamp(value, -1f, 1f);
				_steeringWheelValueEvent.Force = (int)(value * 10000f);
				ForceFeedbackMediator.SetDeviceForcesXY(_steeringWheelValueEvent.Force, 0);
			}
		}

		private void InitForceFeedback()
		{
			if (!_setupComplete)
			{
				ForceFeedbackMediator.InitDirectInput(ForceFeedbackMediator.GetForegroundWindow());
				ForceFeedbackMediator.Aquire();
				ForceFeedbackMediator.StartEffect();
				ForceFeedbackMediator.SetAutoCenter(autoCenter: false);
				_setupComplete = true;
			}
		}

		private void ShutDownForceFeedback()
		{
			if (_setupComplete)
			{
				ForceFeedbackMediator.SetDeviceForcesXY(0, 0);
				ForceFeedbackMediator.StopEffect();
				ForceFeedbackMediator.FreeDirectInput();
				_setupComplete = false;
			}
		}
	}
}
