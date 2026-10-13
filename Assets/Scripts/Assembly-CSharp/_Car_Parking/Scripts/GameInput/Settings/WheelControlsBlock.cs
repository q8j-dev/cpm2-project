using System;

namespace _Car_Parking.Scripts.GameInput.Settings
{
	[Serializable]
	public class WheelControlsBlock
	{
		public ControlSetting RotationRange = new ControlSetting
		{
			Min = 40f,
			Max = 900f,
			Default = 900f,
			DisplayRawValue = true,
			Step = 10f
		};

		public ControlToggleSetting RangeOverrideEnabled = new ControlToggleSetting
		{
			Default = true,
			Current = true
		};

		public ControlSetting FfbMultiplier = new ControlSetting
		{
			Min = 0f,
			Max = 3f,
			Default = 1f,
			DisplayRawValue = true,
			Step = 0.01f
		};

		public float MozaBaseMoment = 0.01f;

		public float MozaBaseMultiplier = 0.1f;

		public float LogitechBaseMoment = 0.4f;

		public float LogitechBaseMultiplier = 0.07f;
	}
}
