using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XInput;
using VContainer.Unity;

namespace _Car_Parking.Scripts.GameInput
{
	public sealed class GamepadMirrorTracker : IInitializable, ITickable, IDisposable
	{
		private const float TriggerPressThreshold = 0.5f;

		private readonly Dictionary<int, GamepadSignatureHistory> _historyByDeviceId = new Dictionary<int, GamepadSignatureHistory>();

		private readonly Dictionary<int, GamepadMirrorEvidence> _evidenceByDeviceId = new Dictionary<int, GamepadMirrorEvidence>();

		public event Action MirrorsChanged;

		public void Initialize()
		{
			InputSystem.onDeviceChange += OnDeviceChange;
		}

		public void Dispose()
		{
			InputSystem.onDeviceChange -= OnDeviceChange;
		}

		public void Tick()
		{
			if (!HasNativePad() || !HasXInputPad())
			{
				return;
			}
			ReadOnlyArray<Gamepad> all = Gamepad.all;
			float unscaledTime = Time.unscaledTime;
			for (int i = 0; i < all.Count; i++)
			{
				if (all[i] is XInputController || IsNativePad(all[i]))
				{
					GetHistory(all[i]).Push(Capture(all[i], unscaledTime));
				}
			}
			for (int j = 0; j < all.Count; j++)
			{
				if (all[j] is XInputController xinput)
				{
					RecordEcho(xinput, all);
				}
			}
		}

		public bool IsMirrorPad(InputDevice device)
		{
			if (!(device is XInputController) || !HasNativePad())
			{
				return false;
			}
			if (_evidenceByDeviceId.TryGetValue(device.deviceId, out var value))
			{
				return value.IsMirror;
			}
			return true;
		}

		public Gamepad GetPreferred()
		{
			Gamepad current = Gamepad.current;
			if (current != null && !IsMirrorPad(current))
			{
				return current;
			}
			ReadOnlyArray<Gamepad> all = Gamepad.all;
			for (int i = 0; i < all.Count; i++)
			{
				if (IsNativePad(all[i]))
				{
					return all[i];
				}
			}
			for (int j = 0; j < all.Count; j++)
			{
				if (!IsMirrorPad(all[j]))
				{
					return all[j];
				}
			}
			return null;
		}

		public Gamepad GetMirrorOf(Gamepad pad)
		{
			if (!IsNativePad(pad))
			{
				return null;
			}
			ReadOnlyArray<Gamepad> all = Gamepad.all;
			for (int i = 0; i < all.Count; i++)
			{
				if (IsMirrorPad(all[i]))
				{
					return all[i];
				}
			}
			return null;
		}

		private void RecordEcho(XInputController xinput, ReadOnlyArray<Gamepad> pads)
		{
			GamepadSignatureHistory history = GetHistory(xinput);
			bool? flag = null;
			for (int i = 0; i < pads.Count; i++)
			{
				if (IsNativePad(pads[i]))
				{
					bool? flag2 = GamepadEcho.Compare(history, GetHistory(pads[i]));
					if (flag2 == true)
					{
						flag = true;
						break;
					}
					if (flag2 == false)
					{
						flag = false;
					}
				}
			}
			if (flag.HasValue && GetEvidence(xinput).Record(flag.Value))
			{
				MirrorsChanged?.Invoke();
			}
		}

		private static GamepadSignature Capture(Gamepad pad, float time)
		{
			int num = 0;
			if (pad.buttonSouth.isPressed)
			{
				num |= 1;
			}
			if (pad.buttonEast.isPressed)
			{
				num |= 2;
			}
			if (pad.buttonWest.isPressed)
			{
				num |= 4;
			}
			if (pad.buttonNorth.isPressed)
			{
				num |= 8;
			}
			if (pad.leftShoulder.isPressed)
			{
				num |= 0x10;
			}
			if (pad.rightShoulder.isPressed)
			{
				num |= 0x20;
			}
			if (pad.leftTrigger.ReadValue() > 0.5f)
			{
				num |= 0x40;
			}
			if (pad.rightTrigger.ReadValue() > 0.5f)
			{
				num |= 0x80;
			}
			if (pad.dpad.up.isPressed)
			{
				num |= 0x100;
			}
			if (pad.dpad.down.isPressed)
			{
				num |= 0x200;
			}
			if (pad.dpad.left.isPressed)
			{
				num |= 0x400;
			}
			if (pad.dpad.right.isPressed)
			{
				num |= 0x800;
			}
			if (pad.startButton.isPressed)
			{
				num |= 0x1000;
			}
			if (pad.selectButton.isPressed)
			{
				num |= 0x2000;
			}
			if (pad.leftStickButton.isPressed)
			{
				num |= 0x4000;
			}
			if (pad.rightStickButton.isPressed)
			{
				num |= 0x8000;
			}
			return new GamepadSignature((ushort)num, pad.leftStick.ReadValue(), pad.rightStick.ReadValue(), time);
		}

		private GamepadSignatureHistory GetHistory(InputDevice pad)
		{
			if (!_historyByDeviceId.TryGetValue(pad.deviceId, out var value))
			{
				value = new GamepadSignatureHistory();
				_historyByDeviceId[pad.deviceId] = value;
			}
			return value;
		}

		private GamepadMirrorEvidence GetEvidence(InputDevice pad)
		{
			if (!_evidenceByDeviceId.TryGetValue(pad.deviceId, out var value))
			{
				value = new GamepadMirrorEvidence();
				_evidenceByDeviceId[pad.deviceId] = value;
			}
			return value;
		}

		private void OnDeviceChange(InputDevice device, InputDeviceChange change)
		{
			if (change == InputDeviceChange.Removed)
			{
				_historyByDeviceId.Remove(device.deviceId);
				if (_evidenceByDeviceId.Remove(device.deviceId))
				{
					MirrorsChanged?.Invoke();
				}
			}
		}

		private static bool IsNativePad(InputDevice pad)
		{
			if (!(pad is DualShockGamepad))
			{
				return SwitchProSupport.IsSwitchPro(pad);
			}
			return true;
		}

		private static bool HasNativePad()
		{
			ReadOnlyArray<Gamepad> all = Gamepad.all;
			for (int i = 0; i < all.Count; i++)
			{
				if (IsNativePad(all[i]))
				{
					return true;
				}
			}
			return false;
		}

		private static bool HasXInputPad()
		{
			ReadOnlyArray<Gamepad> all = Gamepad.all;
			for (int i = 0; i < all.Count; i++)
			{
				if (all[i] is XInputController)
				{
					return true;
				}
			}
			return false;
		}
	}
}
