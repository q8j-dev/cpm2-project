using System;

namespace _Car_Parking.Scripts.GameInput.Settings
{
	[Serializable]
	public class CarControlsBlock
	{
		public ControlSetting SteeringSensitivity = new ControlSetting();

		public ControlToggleSetting InvertSteering = new ControlToggleSetting();

		public ControlSetting ShifterKnobScale = new ControlSetting();
	}
}
