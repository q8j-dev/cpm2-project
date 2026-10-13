using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using _Car_Parking.Scripts.GameInput.Rebinding;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public static class RebindConfirmStep
	{
		private sealed class StepButtons
		{
			private readonly List<ButtonControl> _buttons = new List<ButtonControl>();

			private readonly List<ButtonControl> _waitingForRelease = new List<ButtonControl>();

			public StepButtons(IReadOnlyList<string> paths)
			{
				foreach (string path in paths)
				{
					using InputControlList<InputControl> inputControlList = InputSystem.FindControls(path);
					foreach (InputControl item in inputControlList)
					{
						if (item is ButtonControl buttonControl && !_buttons.Contains(buttonControl))
						{
							_buttons.Add(buttonControl);
							if (buttonControl.isPressed)
							{
								_waitingForRelease.Add(buttonControl);
							}
						}
					}
				}
			}

			public bool Pressed()
			{
				_waitingForRelease.RemoveAll((ButtonControl button) => !button.isPressed);
				foreach (ButtonControl button in _buttons)
				{
					if (button.device.added && button.wasPressedThisFrame && !_waitingForRelease.Contains(button))
					{
						return true;
					}
				}
				return false;
			}
		}

		private const float TimeoutSeconds = 15f;

		public static async UniTask<bool> RunAsync(RebindDeviceProfile profile, Action onSwitch, CancellationToken ct)
		{
			StepButtons confirm = new StepButtons(profile.ConfirmPaths);
			StepButtons cancel = new StepButtons(profile.CancelPaths);
			StepButtons toggle = ((onSwitch == null) ? null : new StepButtons(profile.SwitchPaths));
			float deadline = Time.realtimeSinceStartup + 15f;
			while (Time.realtimeSinceStartup < deadline)
			{
				await UniTask.Yield(PlayerLoopTiming.Update, ct);
				if (cancel.Pressed())
				{
					return false;
				}
				if (confirm.Pressed())
				{
					return true;
				}
				if (toggle != null && toggle.Pressed())
				{
					onSwitch();
					deadline = Time.realtimeSinceStartup + 15f;
				}
			}
			return false;
		}
	}
}
