using UnityEngine;

namespace _Car_Parking.Scripts.PhotoMode.UI
{
	public class ApertureParameter : PhotoModeCameraParameterWithScroll
	{
		protected override ParameterInfo GetParameterInfo()
		{
			return Config.Categories[0].Parameters[1];
		}

		protected override void OnScrollValueChanged(float value)
		{
			int num = Mathf.RoundToInt(value);
			PhotoMode.Aperture = Config.Apertures[num];
		}
	}
}
