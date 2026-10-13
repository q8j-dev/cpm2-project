using System;

namespace _Car_Parking.Scripts.GameInput.Settings
{
	[Serializable]
	public class ControlSetting
	{
		public float Min = 0.1f;

		public float Default = 1f;

		public float Max = 4f;

		public float Current;

		public bool DisplayRawValue;

		public float Step;

		public void ResetToDefault()
		{
			Current = Default;
		}
	}
}
