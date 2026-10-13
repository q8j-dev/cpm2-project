using UnityEngine;

namespace _Car_Parking.Scripts.PhotoMode.UI
{
	public class CameraTiltParameter : PhotoModeCameraParameterWithScroll
	{
		protected override ParameterInfo GetParameterInfo()
		{
			return Config.Categories[0].Parameters[0];
		}

		protected override void OnScrollValueChanged(float value)
		{
			int num = Mathf.RoundToInt(value);
			PhotoMode.TiltCamera(num);
		}
	}
}
