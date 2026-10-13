using System;

namespace _Car_Parking.Scripts.GameInput.Settings
{
	[Serializable]
	public class DeviceControls
	{
		public ControlSettingVector2 CameraMultiplier = new ControlSettingVector2();

		public ControlSetting ZoomSpeed = new ControlSetting
		{
			Min = 1f,
			Max = 100f,
			Default = 25f
		};

		public ControlToggleSetting ZoomInvert = new ControlToggleSetting();

		public CarControlsBlock CarControls = new CarControlsBlock();
	}
}
