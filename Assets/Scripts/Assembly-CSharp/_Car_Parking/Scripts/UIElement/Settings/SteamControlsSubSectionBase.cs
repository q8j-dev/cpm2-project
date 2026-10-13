using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public abstract class SteamControlsSubSectionBase : SteamControlsBindingBase
	{
		private const string ResetTitleGamepadKey = "SteamControls.ResetTitleGamepad";

		private const string ResetTitleMnkKey = "SteamControls.ResetTitleMnk";

		[SerializeField]
		protected Button KeyBindsButton;

		[Header("Camera Multipliers")]
		[SerializeField]
		protected SliderValueHandler CameraMultiplierX;

		[SerializeField]
		protected Toggle InvertCameraXToggle;

		[SerializeField]
		protected SliderValueHandler CameraMultiplierY;

		[SerializeField]
		protected Toggle InvertCameraYToggle;

		[SerializeField]
		protected SliderValueHandler ZoomSpeed;

		[SerializeField]
		protected Toggle InvertZoomToggle;

		[Header("Car Controls")]
		[SerializeField]
		protected SliderValueHandler SteeringSensitivity;

		[SerializeField]
		protected Toggle InvertCarSteering;

		[SerializeField]
		protected SliderValueHandler ShifterKnobSensitivity;

		[SerializeField]
		protected Toggle CasualModeToggle;

		protected abstract ControlDevice Device { get; }

		protected override LocalizedString ResetTitle => new LocalizedString((Device == ControlDevice.Gamepad) ? "SteamControls.ResetTitleGamepad" : "SteamControls.ResetTitleMnk");

		protected override void ResetToDefaults()
		{
			Settings?.ResetDevice(Device);
		}

		protected override void BindRows()
		{
			BindSlider(CameraMultiplierX, () => Settings.GetDeviceMultiplierAxis(Device, x: true), (float v) =>
			{
				Settings.SetDeviceMultiplier(Device, x: true, v);
			});
			BindToggle(InvertCameraXToggle, () => Settings.GetDeviceMultiplierInvert(Device, x: true), (bool v) =>
			{
				Settings.SetDeviceMultiplierInvert(Device, x: true, v);
			});
			BindSlider(CameraMultiplierY, () => Settings.GetDeviceMultiplierAxis(Device, x: false), (float v) =>
			{
				Settings.SetDeviceMultiplier(Device, x: false, v);
			});
			BindToggle(InvertCameraYToggle, () => Settings.GetDeviceMultiplierInvert(Device, x: false), (bool v) =>
			{
				Settings.SetDeviceMultiplierInvert(Device, x: false, v);
			});
			BindSlider(ZoomSpeed, () => Settings.GetCarSlider(Device, Sliders.ZoomSpeed), (float v) =>
			{
				Settings.SetSlider(Device, Sliders.ZoomSpeed, v);
			});
			BindToggle(InvertZoomToggle, () => Settings.GetCarToggle(Device, Toggles.ZoomInvert), (bool v) =>
			{
				Settings.SetToggle(Device, Toggles.ZoomInvert, v);
			});
			BindSlider(SteeringSensitivity, () => Settings.GetCarSlider(Device, Sliders.Steering), (float v) =>
			{
				Settings.SetSlider(Device, Sliders.Steering, v);
			});
			BindToggle(InvertCarSteering, () => Settings.GetCarToggle(Device, Toggles.InvertSteering), (bool v) =>
			{
				Settings.SetToggle(Device, Toggles.InvertSteering, v);
			});
			BindSlider(ShifterKnobSensitivity, () => Settings.GetCarSlider(Device, Sliders.ShifterKnob), (float v) =>
			{
				Settings.SetSlider(Device, Sliders.ShifterKnob, v);
			});
			BindToggle(CasualModeToggle, () => Settings.GetCasualToggle(), (bool v) =>
			{
				Settings.SetCasual(v);
			});
		}
	}
}
