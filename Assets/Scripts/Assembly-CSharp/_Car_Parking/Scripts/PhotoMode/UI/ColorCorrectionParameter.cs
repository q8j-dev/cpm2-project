using UnityEngine;

namespace _Car_Parking.Scripts.PhotoMode.UI
{
	public class ColorCorrectionParameter : PhotoModeCameraParameterWithScroll
	{
		protected override ParameterInfo GetParameterInfo()
		{
			return Config.Categories[1].Parameters[1];
		}

		protected override void OnScrollValueChanged(float value)
		{
			int num = Mathf.RoundToInt(value);
			PhotoMode.WhiteBallanceTint = num;
		}
	}
}
