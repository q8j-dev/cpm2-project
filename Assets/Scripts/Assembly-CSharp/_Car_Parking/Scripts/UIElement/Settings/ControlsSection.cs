using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public class ControlsSection : SettingsSectionBase
	{
		[SerializeField]
		private Toggle ArrowsControlToggle;

		[SerializeField]
		private Toggle WheelControlToggle;

		[SerializeField]
		private Toggle TiltControlToggle;

		[SerializeField]
		private Toggle TiltSmoothingToggle;

		[SerializeField]
		private Toggle GamepadOnToggle;

		[SerializeField]
		private Toggle GamepadOffToggle;

		[SerializeField]
		private SliderValueHandler SteeringSensitivitySlider;

		[SerializeField]
		private Button EditHudButton;

		[SerializeField]
		private Toggle AnalogAcceleratorToggle;

		[SerializeField]
		private Toggle AnalogBrakeToggle;

		[SerializeField]
		private Toggle AnalogClutchToggle;

		private CoreGameManager _coreGameManager;

		private OptionalDependency<CoreGameManager> _coreGameManagerDep;

		private InputItemsProvider _inputItemsProvider;

		private CarInputManager _carInputManager;

		private void OnEnable()
		{
			base.gameObject.SetActive(value: false);
		}

		public override void Init()
		{
		}
	}
}
