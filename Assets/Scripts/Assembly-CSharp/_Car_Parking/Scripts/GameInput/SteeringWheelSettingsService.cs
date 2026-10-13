using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.Layouts;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Car.Physics;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.GameInput
{
	public class SteeringWheelSettingsService : IStartable, IDisposable
	{
		private const int VendorLogitech = 1133;

		private static readonly int[] SupportedProducts = new int[4] { 49817, 49819, 49743, 49818 };

		private const int CompatModeProduct = 49812;

		private DesktopControlsSettingsSO _settings;

		private SteeringWheelValueEvent _steeringWheelValueEvent;

		private readonly CancellationTokenSource _lifetimeCts = new CancellationTokenSource();

		[Inject]
		public void Construct(DesktopControlsSettingsSO settings, SteeringWheelValueEvent steeringWheelValueEvent)
		{
			_settings = settings;
			_steeringWheelValueEvent = steeringWheelValueEvent;
		}

		public void Start()
		{
			try
			{
				InputSystem.RegisterLayout("\r\n                    {\r\n                        \"name\": \"LogitechCompatModeWheel\",\r\n                        \"extend\": \"HID\",\r\n                        \"format\": \"HID\",\r\n                        \"controls\": [\r\n                            { \"name\": \"unused\", \"layout\": \"Button\", \"offset\": 0, \"bit\": 0 }\r\n                        ]\r\n                    }", "LogitechCompatModeWheel", default(InputDeviceMatcher).WithInterface("HID").WithCapability("vendorId", 1133).WithCapability("productId", 49812));
			}
			catch (Exception)
			{
			}
			foreach (InputDevice device in InputSystem.devices)
			{
				TryApplyRangeAsync(device, _lifetimeCts.Token).Forget();
			}
			InputSystem.onDeviceChange += OnDeviceChange;
			_settings.WheelRangeChanged += OnRangeSettingChanged;
			_settings.WheelRangeOverrideChanged += OnRangeOverrideChanged;
			_settings.WheelFfbMultiplierChanged += OnFfbMultiplierChanged;
			MozaWrapper.BaseConnectionChanged += OnMozaBaseConnectionChanged;
			if (MozaWrapper.IsBaseConnected())
			{
				ApplyMozaRange();
			}
			ApplyFfbCoefficients();
		}

		public void Dispose()
		{
			InputSystem.onDeviceChange -= OnDeviceChange;
			if (_settings != null)
			{
				_settings.WheelRangeChanged -= OnRangeSettingChanged;
				_settings.WheelRangeOverrideChanged -= OnRangeOverrideChanged;
				_settings.WheelFfbMultiplierChanged -= OnFfbMultiplierChanged;
			}
			MozaWrapper.BaseConnectionChanged -= OnMozaBaseConnectionChanged;
			_lifetimeCts.Cancel();
			_lifetimeCts.Dispose();
		}

		private void OnDeviceChange(InputDevice device, InputDeviceChange change)
		{
			LogDeviceChange(device, change);
			switch (change)
			{
			case InputDeviceChange.Added:
			case InputDeviceChange.Reconnected:
				TryApplyRangeAsync(device, _lifetimeCts.Token).Forget();
				ApplyFfbCoefficients();
				break;
			case InputDeviceChange.Removed:
			case InputDeviceChange.Disconnected:
				ApplyFfbCoefficients();
				break;
			}
		}

		private void OnMozaBaseConnectionChanged(bool connected)
		{
			if (connected)
			{
				ApplyMozaRange();
			}
			ApplyFfbCoefficients();
		}

		private void OnRangeSettingChanged(float degrees)
		{
			ApplyRangeToAll();
		}

		private void OnRangeOverrideChanged(bool enabled)
		{
			if (enabled)
			{
				ApplyRangeToAll();
			}
		}

		private void ApplyRangeToAll()
		{
			if (!_settings.WheelRangeOverrideEnabled)
			{
				return;
			}
			foreach (InputDevice device in InputSystem.devices)
			{
				if (IsSupportedLogitechWheel(device, out var _))
				{
					ApplyLogitechRange(device);
				}
			}
			ApplyMozaRange();
		}

		private async UniTaskVoid TryApplyRangeAsync(InputDevice device, CancellationToken ct)
		{
			if (!IsSupportedLogitechWheel(device, out var compatMode))
			{
				if (compatMode)
				{
					TrySwitchToNativeModeAsync(device, ct).Forget();
				}
			}
			else if (_settings.WheelRangeOverrideEnabled)
			{
				await UniTask.Delay(TimeSpan.FromMilliseconds(500.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				if (!ApplyLogitechRange(device))
				{
					await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
					ApplyLogitechRange(device);
				}
			}
		}

		private bool ApplyLogitechRange(InputDevice device)
		{
			if (!device.added)
			{
				return true;
			}
			if (!_settings.WheelRangeOverrideEnabled)
			{
				return true;
			}
			LogitechWheelOutputCommand command = LogitechWheelOutputCommand.Range(Mathf.RoundToInt(_settings.WheelRotationRange));
			if (device.ExecuteCommand(ref command) >= 0)
			{
				return true;
			}
			return false;
		}

		private async UniTaskVoid TrySwitchToNativeModeAsync(InputDevice device, CancellationToken ct)
		{
			await UniTask.Delay(TimeSpan.FromMilliseconds(500.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			if (!SendNativeModeSwitch(device))
			{
				await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				SendNativeModeSwitch(device);
			}
		}

		private static bool SendNativeModeSwitch(InputDevice device)
		{
			if (!device.added)
			{
				return true;
			}
			LogitechWheelOutputCommand command = LogitechWheelOutputCommand.NativeModeG25();
			if (device.ExecuteCommand(ref command) >= 0)
			{
				return true;
			}
			return false;
		}

		private static void LogDeviceChange(InputDevice device, InputDeviceChange change)
		{
			if (change != InputDeviceChange.Added && change != InputDeviceChange.Removed && change != InputDeviceChange.Reconnected && change != InputDeviceChange.Disconnected)
			{
				return;
			}
			InputDeviceDescription description = device.description;
			if (description.interfaceName == "HID" && !string.IsNullOrEmpty(description.capabilities))
			{
				try
				{
					HID.HIDDeviceDescriptor.FromJson(description.capabilities);
				}
				catch (Exception)
				{
				}
			}
		}

		private void ApplyMozaRange()
		{
			if (_settings.WheelRangeOverrideEnabled)
			{
				MozaWrapper.TrySetWheelRange(Mathf.RoundToInt(_settings.WheelRotationRange));
			}
		}

		private void OnFfbMultiplierChanged(float value)
		{
			ApplyFfbCoefficients();
		}

		private void ApplyFfbCoefficients()
		{
			bool flag = false;
			flag = MozaWrapper.IsBaseConnected();
			bool flag2 = false;
			foreach (InputDevice device in InputSystem.devices)
			{
				if (IsSupportedLogitechWheel(device, out var _))
				{
					flag2 = true;
					break;
				}
			}
			if (flag || flag2)
			{
				var (momentCoefficient, num) = _settings.GetWheelFfbBase(flag);
				_steeringWheelValueEvent.MomentCoefficient = momentCoefficient;
				_steeringWheelValueEvent.Multiplier = num * _settings.WheelFfbMultiplier;
			}
		}

		private static bool IsSupportedLogitechWheel(InputDevice device, out bool compatMode)
		{
			compatMode = false;
			if (device.description.interfaceName != "HID")
			{
				return false;
			}
			string capabilities = device.description.capabilities;
			if (string.IsNullOrEmpty(capabilities))
			{
				return false;
			}
			try
			{
				HID.HIDDeviceDescriptor hIDDeviceDescriptor = HID.HIDDeviceDescriptor.FromJson(capabilities);
				if (hIDDeviceDescriptor.vendorId != 1133)
				{
					return false;
				}
				if (hIDDeviceDescriptor.productId == 49812)
				{
					compatMode = true;
					return false;
				}
				int[] supportedProducts = SupportedProducts;
				foreach (int num in supportedProducts)
				{
					if (hIDDeviceDescriptor.productId == num)
					{
						return true;
					}
				}
			}
			catch (Exception)
			{
			}
			return false;
		}
	}
}
