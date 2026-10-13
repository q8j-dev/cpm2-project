using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

namespace _Car_Parking.Scripts.GameInput
{
	public class SwitchProRumble
	{
		private const int NintendoVendorId = 1406;

		private const int ProControllerProductId = 8201;

		private const float MinWriteIntervalSeconds = 0.025f;

		private const float RefreshIntervalSeconds = 0.05f;

		private const float EnableIntervalSeconds = 1f;

		private readonly byte[] _report = new byte[64];

		private readonly Dictionary<int, bool> _hasMotorsByDeviceId = new Dictionary<int, bool>();

		private readonly Dictionary<int, int> _reportSizeByDeviceId = new Dictionary<int, int>();

		private Gamepad _pad;

		private float _low;

		private float _high;

		private bool _dirty;

		private float _lastWrite = float.NegativeInfinity;

		private float _lastEnable = float.NegativeInfinity;

		private byte _counter;

		private bool _loggedWriteFailure;

		public bool Handles(Gamepad pad)
		{
			if (SwitchProSupport.IsSwitchPro(pad))
			{
				return HasMotors(pad);
			}
			return false;
		}

		public void Set(Gamepad pad, float lowFrequency, float highFrequency, float now)
		{
			if (pad != _pad)
			{
				if (_pad != null && _pad.added && (_low > 0f || _high > 0f))
				{
					Write(_pad, 0f, 0f, enableVibration: false);
				}
				_pad = pad;
				_lastWrite = float.NegativeInfinity;
				_lastEnable = float.NegativeInfinity;
			}
			_low = lowFrequency;
			_high = highFrequency;
			_dirty = true;
			Update(now);
		}

		public void Update(float now)
		{
			if (_pad == null || !_pad.added)
			{
				return;
			}
			bool flag = _low > 0f || _high > 0f;
			float num = now - _lastWrite;
			if ((!flag || !(num < 0.025f)) && (_dirty || (flag && num >= 0.05f)))
			{
				bool flag2 = flag && now - _lastEnable >= 1f;
				if (Write(_pad, _low, _high, flag2) & flag2)
				{
					_lastEnable = now;
				}
				_lastWrite = now;
				_dirty = false;
			}
		}

		private bool Write(Gamepad pad, float low, float high, bool enableVibration)
		{
			_counter = (byte)((_counter + 1) & 0xF);
			if (enableVibration)
			{
				SwitchProRumbleReport.WriteRumbleWithEnable(_report, _counter, low, high);
			}
			else
			{
				SwitchProRumbleReport.WriteRumble(_report, _counter, low, high);
			}
			bool num = Send(pad);
			if (!num && !_loggedWriteFailure)
			{
				_loggedWriteFailure = true;
			}
			return num;
		}

		private bool Send(Gamepad pad)
		{
			if (_reportSizeByDeviceId.TryGetValue(pad.deviceId, out var value))
			{
				SwitchProOutputCommand command = SwitchProOutputCommand.Create(_report, value);
				return pad.ExecuteCommand(ref command) >= 0;
			}
			SwitchProOutputCommand command2 = SwitchProOutputCommand.Create(_report, 49);
			if (pad.ExecuteCommand(ref command2) > 0)
			{
				_reportSizeByDeviceId[pad.deviceId] = 49;
				return true;
			}
			SwitchProOutputCommand command3 = SwitchProOutputCommand.Create(_report, 64);
			if (pad.ExecuteCommand(ref command3) < 0)
			{
				return false;
			}
			_reportSizeByDeviceId[pad.deviceId] = 64;
			return true;
		}

		private bool HasMotors(InputDevice pad)
		{
			if (_hasMotorsByDeviceId.TryGetValue(pad.deviceId, out var value))
			{
				return value;
			}
			value = false;
			string capabilities = pad.description.capabilities;
			if (!string.IsNullOrEmpty(capabilities))
			{
				try
				{
					HID.HIDDeviceDescriptor hIDDeviceDescriptor = HID.HIDDeviceDescriptor.FromJson(capabilities);
					value = hIDDeviceDescriptor.vendorId == 1406 && hIDDeviceDescriptor.productId == 8201;
				}
				catch (Exception)
				{
				}
			}
			_hasMotorsByDeviceId[pad.deviceId] = value;
			return value;
		}
	}
}
