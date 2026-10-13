using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using VContainer.Unity;
using _Car_Parking.Scripts.Car.Physics;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public sealed class ActiveWheelService : IStartable, IDisposable
	{
		private readonly List<(InputDevice Device, WheelKind Kind)> _wheels = new List<(InputDevice, WheelKind)>();

		private readonly CancellationTokenSource _disposeCts = new CancellationTokenSource();

		private bool _rescanQueued;

		public InputDevice ActiveWheel => ActiveEntry().Device;

		public WheelKind ActiveKind => ActiveEntry().Kind;

		public event Action ActiveChanged;

		void IStartable.Start()
		{
			Rescan();
			InputSystem.onDeviceChange += OnDeviceChange;
		}

		private void OnDeviceChange(InputDevice device, InputDeviceChange change)
		{
			if ((change == InputDeviceChange.Added || change == InputDeviceChange.Removed || change == InputDeviceChange.Reconnected || change == InputDeviceChange.Disconnected) && !_rescanQueued)
			{
				_rescanQueued = true;
				RescanNextFrameAsync(_disposeCts.Token).Forget();
			}
		}

		private async UniTaskVoid RescanNextFrameAsync(CancellationToken ct)
		{
			bool num = await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
			_rescanQueued = false;
			if (!num)
			{
				InputDevice activeWheel = ActiveWheel;
				Rescan();
				if (ActiveWheel != activeWheel)
				{
					ActiveChanged?.Invoke();
				}
			}
		}

		private void Rescan()
		{
			_wheels.Clear();
			foreach (InputDevice device in InputSystem.devices)
			{
				if (device.added)
				{
					WheelKind wheelKind = WheelClassifier.Classify(WheelDeviceInfo.From(device));
					if (WheelClassifier.IsWheel(wheelKind) && IsRealDevice(wheelKind))
					{
						_wheels.Add((device, wheelKind));
					}
				}
			}
		}

		private static bool IsRealDevice(WheelKind kind)
		{
			if (kind == WheelKind.Moza)
			{
				return MozaWrapper.IsBaseConnected();
			}
			return true;
		}

		private (InputDevice Device, WheelKind Kind) ActiveEntry()
		{
			(InputDevice, WheelKind) result = default;
			double num = double.MinValue;
			foreach (var wheel in _wheels)
			{
				if (wheel.Device != null && wheel.Device.added && wheel.Device.lastUpdateTime > num)
				{
					result = wheel;
					num = wheel.Device.lastUpdateTime;
				}
			}
			return result;
		}

		public void Dispose()
		{
			InputSystem.onDeviceChange -= OnDeviceChange;
			_disposeCts.Cancel();
			_disposeCts.Dispose();
		}
	}
}
