using System;

namespace _Car_Parking.Scripts.GameInput.Settings
{
	[Serializable]
	public class DroneCameraControls : CameraControls
	{
		public ControlSetting MoveSpeed = new ControlSetting
		{
			Min = 1f,
			Max = 30f,
			Default = 10f
		};

		public ControlSetting VerticalSpeed = new ControlSetting
		{
			Min = 1f,
			Max = 40f,
			Default = 20f
		};

		public ControlSetting TiltSpeed = new ControlSetting
		{
			Min = 1f,
			Max = 30f,
			Default = 10f
		};
	}
}
