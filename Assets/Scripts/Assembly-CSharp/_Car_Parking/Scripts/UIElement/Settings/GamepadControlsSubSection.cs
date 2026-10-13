using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.GameInput;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public class GamepadControlsSubSection : SteamControlsSubSectionBase
	{
		private const string NoDevicesText = "-";

		[Header("Gamepad Specific")]
		[SerializeField]
		private SliderValueHandler StickCursorSensitivity;

		[SerializeField]
		private SliderValueHandler StickDeadzoneLeft;

		[SerializeField]
		private SliderValueHandler StickDeadzoneRight;

		[SerializeField]
		private SliderValueHandler VibrationIntensity;

		[SerializeField]
		private Toggle VibrationEnableToggle;

		[SerializeField]
		private TextMeshProUGUI ConnectedDevicesText;

		private GamepadMirrorTracker _mirrors;

		protected override ControlDevice Device => ControlDevice.Gamepad;

		[Inject]
		public void ConstructMirrors(OptionalDependency<GamepadMirrorTracker> mirrors)
		{
			_mirrors = mirrors?.Value;
		}

		protected override void BindRows()
		{
			base.BindRows();
			BindSlider(StickCursorSensitivity, () => Settings.GetUniqueSlider(UniqueSliders.Cursor), (float v) =>
			{
				Settings.SetUniqueSlider(UniqueSliders.Cursor, v);
			});
			BindSlider(StickDeadzoneLeft, () => Settings.GetUniqueSlider(UniqueSliders.DeadzoneLeft), (float v) =>
			{
				Settings.SetUniqueSlider(UniqueSliders.DeadzoneLeft, v);
			});
			BindSlider(StickDeadzoneRight, () => Settings.GetUniqueSlider(UniqueSliders.DeadzoneRight), (float v) =>
			{
				Settings.SetUniqueSlider(UniqueSliders.DeadzoneRight, v);
			});
			BindSlider(VibrationIntensity, () => Settings.GetUniqueSlider(UniqueSliders.VibrationIntensity), (float v) =>
			{
				Settings.SetUniqueSlider(UniqueSliders.VibrationIntensity, v);
			});
			BindToggle(VibrationEnableToggle, () => Settings.GetUniqueToggle(UniqueToggles.VibrationEnabled), (bool v) =>
			{
				Settings.SetUniqueToggle(UniqueToggles.VibrationEnabled, v);
			});
		}

		private void OnEnable()
		{
			RefreshDevicesPanel();
			InputSystem.onDeviceChange += OnDeviceChange;
			if (_mirrors != null)
			{
				_mirrors.MirrorsChanged += RefreshDevicesPanel;
			}
		}

		private void OnDisable()
		{
			InputSystem.onDeviceChange -= OnDeviceChange;
			if (_mirrors != null)
			{
				_mirrors.MirrorsChanged -= RefreshDevicesPanel;
			}
		}

		private void OnDeviceChange(InputDevice device, InputDeviceChange change)
		{
			if (change == InputDeviceChange.Added || change == InputDeviceChange.Removed || change == InputDeviceChange.Reconnected || change == InputDeviceChange.Disconnected)
			{
				RefreshDevicesPanel();
			}
		}

		private void RefreshDevicesPanel()
		{
			if (!ConnectedDevicesText)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Gamepad item in Gamepad.all)
			{
				if (_mirrors == null || !_mirrors.IsMirrorPad(item))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append('\n');
					}
					stringBuilder.Append(item.displayName ?? item.name);
				}
			}
			ConnectedDevicesText.SetText((stringBuilder.Length > 0) ? stringBuilder.ToString() : "-");
		}
	}
}
