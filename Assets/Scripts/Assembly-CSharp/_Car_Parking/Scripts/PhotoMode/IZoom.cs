namespace _Car_Parking.Scripts.PhotoMode
{
	public interface IZoom
	{
		float ZoomDistance { get; set; }

		void CalculateDistanceFree(float zoomValue);
	}
}
