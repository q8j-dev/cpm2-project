namespace _Car_Parking.Scripts.PhotoMode.UI
{
	public class DistortionParameter : PhotoModeCameraParameterWithScroll
	{
		protected override ParameterInfo GetParameterInfo()
		{
			return Config.Categories[0].Parameters[3];
		}

		protected override void OnScrollValueChanged(float value)
		{
			PhotoMode.CameraDistortion = value;
		}
	}
}
