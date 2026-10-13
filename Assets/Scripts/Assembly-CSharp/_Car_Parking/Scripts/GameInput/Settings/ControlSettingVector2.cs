using System;
using UnityEngine;

namespace _Car_Parking.Scripts.GameInput.Settings
{
	[Serializable]
	public class ControlSettingVector2
	{
		public Vector2 Min;

		public Vector2 Default = Vector2.one;

		public Vector2 Max = Vector2.one;

		public Vector2 Current;

		public bool DisplayRawValue;

		public float Step;

		public ControlToggleSetting InvertX = new ControlToggleSetting();

		public ControlToggleSetting InvertY = new ControlToggleSetting();

		public void ResetToDefault()
		{
			Current = Default;
		}

		public ControlSetting Axis(bool x)
		{
			return new ControlSetting
			{
				Min = (x ? Min.x : Min.y),
				Max = (x ? Max.x : Max.y),
				Default = (x ? Default.x : Default.y),
				Current = (x ? Current.x : Current.y),
				DisplayRawValue = DisplayRawValue,
				Step = Step
			};
		}
	}
}
