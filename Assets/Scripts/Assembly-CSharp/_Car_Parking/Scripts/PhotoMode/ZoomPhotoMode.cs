namespace _Car_Parking.Scripts.PhotoMode
{
	public class ZoomPhotoMode : IZoom
	{
		public const float MinZoomDist = 6f;

		public const float MaxZoomDist = 100f;

		public const float DefaultDist = 60f;

		public float ZoomDistance { get; set; }

		public void CalculateDistanceFree(float zoomValue)
		{
			float num = 94f / 100f * (100f * (1f - zoomValue));
			ZoomDistance = 6f + num;
		}
	}
}
