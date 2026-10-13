using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.HID;

namespace _Car_Parking.Scripts.GameInput
{
	public class PlayStationBluetoothRumble
	{
		private const int DualShock4UsbReportLength = 32;

		private const int DualSenseUsbReportLength = 48;

		private readonly byte[] _report = new byte[80];

		private readonly Dictionary<int, int> _outputReportSizeByDeviceId = new Dictionary<int, int>();

		private byte _sequence;

		public bool Handles(Gamepad pad)
		{
			if (!(pad is DualSenseGamepadHID))
			{
				if (pad is DualShock4GamepadHID)
				{
					return GetOutputReportSize(pad) > 32;
				}
				return false;
			}
			return GetOutputReportSize(pad) > 48;
		}

		public bool Send(Gamepad pad, float lowFrequency, float highFrequency)
		{
			byte lowFrequency2 = PlayStationBluetoothReport.ToMotorByte(lowFrequency);
			byte highFrequency2 = PlayStationBluetoothReport.ToMotorByte(highFrequency);
			if (pad is DualSenseGamepadHID)
			{
				_sequence = (byte)((_sequence + 1) & 0xF);
				PlayStationBluetoothReport.WriteDualSense(_report, _sequence, lowFrequency2, highFrequency2);
			}
			else
			{
				PlayStationBluetoothReport.WriteDualShock4(_report, lowFrequency2, highFrequency2);
			}
			PlayStationBluetoothOutputCommand command = PlayStationBluetoothOutputCommand.Create(_report, GetOutputReportSize(pad));
			return pad.ExecuteCommand(ref command) >= 0;
		}

		private int GetOutputReportSize(InputDevice pad)
		{
			if (_outputReportSizeByDeviceId.TryGetValue(pad.deviceId, out var value))
			{
				return value;
			}
			value = 0;
			string capabilities = pad.description.capabilities;
			if (!string.IsNullOrEmpty(capabilities))
			{
				try
				{
					value = HID.HIDDeviceDescriptor.FromJson(capabilities).outputReportSize;
				}
				catch (Exception)
				{
				}
			}
			_outputReportSizeByDeviceId[pad.deviceId] = value;
			return value;
		}
	}
}
