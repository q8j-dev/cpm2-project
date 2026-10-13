using System;

namespace _Car_Parking.Scripts.GameInput.Settings
{
	[Serializable]
	public class MouseKeyboardControlsBlock
	{
		public ControlSetting MouseSteerMaxDisplacement = new ControlSetting
		{
			Min = 50f,
			Max = 2000f,
			Default = 400f,
			DisplayRawValue = true,
			Step = 10f
		};

		public float MouseSteerReturnRate = 6f;

		public float UiScrollDeltaPerTick = 2f;
	}
}
