using UnityEngine;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public class CameraControlsSubSection : SteamControlsBindingBase
	{
		[SerializeField]
		[Header("Car Camera")]
		protected SliderValueHandler CarCameraSensitivityX;

		[SerializeField]
		protected SliderValueHandler CarCameraSensitivityY;

		[Header("Car FPV")]
		[SerializeField]
		protected SliderValueHandler CarFPVCameraSensitivityX;

		[SerializeField]
		protected SliderValueHandler CarFPVCameraSensitivityY;

		[SerializeField]
		[Header("Person Camera")]
		protected SliderValueHandler PersonCameraSensitivityX;

		[SerializeField]
		protected SliderValueHandler PersonCameraSensitivityY;

		[SerializeField]
		protected SliderValueHandler PersonCameraTurnSensitivity;

		[Header("Garage Camera")]
		[SerializeField]
		protected SliderValueHandler GarageCameraSensitivityX;

		[SerializeField]
		protected SliderValueHandler GarageCameraSensitivityY;

		[Header("Drone Camera")]
		[SerializeField]
		protected SliderValueHandler DroneCameraSensitivityX;

		[SerializeField]
		protected SliderValueHandler DroneCameraSensitivityY;

		[SerializeField]
		protected SliderValueHandler DroneSpeed;

		[SerializeField]
		protected SliderValueHandler DroneVerticalSpeed;

		[SerializeField]
		protected SliderValueHandler DroneTiltSpeed;

		[SerializeField]
		[Header("Map Camera")]
		protected SliderValueHandler MapCameraSensitivityX;

		[SerializeField]
		protected SliderValueHandler MapCameraSensitivityY;

		protected override LocalizedString ResetTitle => new LocalizedString("SteamControls.ResetTitle");

		protected override void ResetToDefaults()
		{
			Settings?.ResetCameras();
		}

		protected override void BindRows()
		{
			BindCamera(CameraSettingsContext.Car, CarCameraSensitivityX, CarCameraSensitivityY);
			BindCamera(CameraSettingsContext.CarFpv, CarFPVCameraSensitivityX, CarFPVCameraSensitivityY);
			BindCamera(CameraSettingsContext.Garage, GarageCameraSensitivityX, GarageCameraSensitivityY);
			BindCamera(CameraSettingsContext.Drone, DroneCameraSensitivityX, DroneCameraSensitivityY);
			BindCamera(CameraSettingsContext.Map, MapCameraSensitivityX, MapCameraSensitivityY);
			BindCamera(CameraSettingsContext.Person, PersonCameraSensitivityX, PersonCameraSensitivityY);
			BindSlider(PersonCameraTurnSensitivity, Settings.GetPersonTurnSlider, (float v) =>
			{
				Settings.SetPersonTurnSpeed(v);
			});
			BindSlider(DroneSpeed, () => Settings.GetUniqueSlider(UniqueSliders.DroneMove), (float v) =>
			{
				Settings.SetUniqueSlider(UniqueSliders.DroneMove, v);
			});
			BindSlider(DroneVerticalSpeed, () => Settings.GetUniqueSlider(UniqueSliders.DroneVertical), (float v) =>
			{
				Settings.SetUniqueSlider(UniqueSliders.DroneVertical, v);
			});
			BindSlider(DroneTiltSpeed, () => Settings.GetUniqueSlider(UniqueSliders.DroneTilt), (float v) =>
			{
				Settings.SetUniqueSlider(UniqueSliders.DroneTilt, v);
			});
		}

		private void BindCamera(CameraSettingsContext ctx, SliderValueHandler sx, SliderValueHandler sy)
		{
			BindSlider(sx, () => Settings.GetCameraSlider(ctx, CameraAxis.SensX), (float v) =>
			{
				Settings.SetCameraSlider(ctx, CameraAxis.SensX, v);
			});
			BindSlider(sy, () => Settings.GetCameraSlider(ctx, CameraAxis.SensY), (float v) =>
			{
				Settings.SetCameraSlider(ctx, CameraAxis.SensY, v);
			});
		}
	}
}
