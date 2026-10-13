using System;

namespace _Car_Parking.Scripts.GameInput.Settings
{
	[Serializable]
	public class ControlToggleSetting
	{
		public bool Default;

		public bool Current;

		public void ResetToDefault()
		{
			Current = Default;
		}
	}
}
