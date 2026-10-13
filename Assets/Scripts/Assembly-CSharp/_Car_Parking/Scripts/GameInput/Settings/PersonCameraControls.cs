using System;

namespace _Car_Parking.Scripts.GameInput.Settings
{
	[Serializable]
	public class PersonCameraControls
	{
		public ControlSettingVector2 Look = new ControlSettingVector2();

		public ControlSetting TurnSpeed = new ControlSetting();

		public ControlToggleSetting InvertTurn = new ControlToggleSetting();
	}
}
