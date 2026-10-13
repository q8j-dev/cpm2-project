using System;

namespace _Car_Parking.Scripts.GameInput.Settings
{
	[Serializable]
	public class GamepadControlsBlock
	{
		public ControlSetting CursorSensitivity = new ControlSetting();

		public ControlSetting StickDeadzoneLeft = new ControlSetting
		{
			Min = 0f,
			Max = 0.5f,
			Default = 0.1f
		};

		public ControlSetting StickDeadzoneRight = new ControlSetting
		{
			Min = 0f,
			Max = 0.5f,
			Default = 0.1f
		};

		public ControlSetting VibrationIntensity = new ControlSetting();

		public ControlToggleSetting VibrationEnabled = new ControlToggleSetting
		{
			Default = true
		};
	}
}
