using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using _Car_Parking.Scripts.Car.Physics;
using _Car_Parking.Scripts.GameInput.Rebinding;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public class WheelControlsSubSection : SteamControlsBindingBase
	{
		private const string ResetTitleWheelKey = "SteamControls.ResetTitleWheel";

		private const string NoDevicesKey = "-";

		[SerializeField]
		private Button KeyBindsButton;

		[SerializeField]
		[Header("Camera Multipliers")]
		private SliderValueHandler CameraMultiplierX;

		[SerializeField]
		private Toggle InvertCameraXToggle;

		[SerializeField]
		private SliderValueHandler CameraMultiplierY;

		[SerializeField]
		private Toggle InvertCameraYToggle;

		[Header("Steering Wheel")]
		[SerializeField]
		private TextMeshProUGUI ConnectedDevicesText;

		[SerializeField]
		private SliderValueHandler RotationRange;

		[SerializeField]
		private Toggle RangeOverrideToggle;

		[SerializeField]
		private SliderValueHandler FfbMultiplier;

		protected override LocalizedString ResetTitle => new LocalizedString("SteamControls.ResetTitleWheel");

		protected override void ResetToDefaults()
		{
			Settings?.ResetDevice(ControlDevice.Wheel);
		}

		protected override void BindRows()
		{
			BindSlider(CameraMultiplierX, () => Settings.GetDeviceMultiplierAxis(ControlDevice.Wheel, x: true), (float v) =>
			{
				Settings.SetDeviceMultiplier(ControlDevice.Wheel, x: true, v);
			});
			BindToggle(InvertCameraXToggle, () => Settings.GetDeviceMultiplierInvert(ControlDevice.Wheel, x: true), (bool v) =>
			{
				Settings.SetDeviceMultiplierInvert(ControlDevice.Wheel, x: true, v);
			});
			BindSlider(CameraMultiplierY, () => Settings.GetDeviceMultiplierAxis(ControlDevice.Wheel, x: false), (float v) =>
			{
				Settings.SetDeviceMultiplier(ControlDevice.Wheel, x: false, v);
			});
			BindToggle(InvertCameraYToggle, () => Settings.GetDeviceMultiplierInvert(ControlDevice.Wheel, x: false), (bool v) =>
			{
				Settings.SetDeviceMultiplierInvert(ControlDevice.Wheel, x: false, v);
			});
			BindSlider(RotationRange, () => Settings.GetUniqueSlider(UniqueSliders.WheelRange), (float v) =>
			{
				Settings.SetUniqueSlider(UniqueSliders.WheelRange, v);
			});
			BindToggle(RangeOverrideToggle, () => Settings.GetUniqueToggle(UniqueToggles.WheelRangeOverride), (bool v) =>
			{
				Settings.SetUniqueToggle(UniqueToggles.WheelRangeOverride, v);
			});
			BindSlider(FfbMultiplier, () => Settings.GetUniqueSlider(UniqueSliders.WheelFfbMultiplier), (float v) =>
			{
				Settings.SetUniqueSlider(UniqueSliders.WheelFfbMultiplier, v);
			});
		}

		private void OnEnable()
		{
			RefreshDevicesPanel();
			InputSystem.onDeviceChange += OnDeviceChange;
			MozaWrapper.BaseConnectionChanged += OnMozaBaseConnectionChanged;
		}

		private void OnDisable()
		{
			InputSystem.onDeviceChange -= OnDeviceChange;
			MozaWrapper.BaseConnectionChanged -= OnMozaBaseConnectionChanged;
		}

		private void OnMozaBaseConnectionChanged(bool connected)
		{
			RefreshDevicesPanel();
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
			foreach (InputDevice device in InputSystem.devices)
			{
				if (LooksLikeSteeringWheel(device))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append('\n');
					}
					stringBuilder.Append(device.displayName ?? device.name);
				}
			}
			ConnectedDevicesText.SetText((stringBuilder.Length > 0) ? stringBuilder.ToString() : "-");
		}

		private static bool LooksLikeSteeringWheel(InputDevice device)
		{
			WheelKind wheelKind = WheelClassifier.Classify(WheelDeviceInfo.From(device));
			if (wheelKind == WheelKind.Moza)
			{
				return MozaWrapper.IsBaseConnected();
			}
			return WheelClassifier.IsWheel(wheelKind);
		}
	}
}
