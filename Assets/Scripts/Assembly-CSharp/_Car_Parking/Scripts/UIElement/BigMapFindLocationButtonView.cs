using UnityEngine;

namespace _Car_Parking.Scripts.UIElement
{
	public class BigMapFindLocationButtonView : MonoBehaviour
	{
		[SerializeField]
		private GameObject Deselect = default;

		private void OnEnable()
		{
			GPSTracker.OnIsDrawingChangedEvent += GPSTrackerIsDrawingChangedCallback;
			UpdateDeselectState();
		}

		private void OnDisable()
		{
			GPSTracker.OnIsDrawingChangedEvent -= GPSTrackerIsDrawingChangedCallback;
		}

		private void GPSTrackerIsDrawingChangedCallback(bool isDrawing)
		{
			UpdateDeselectState();
		}

		private void UpdateDeselectState()
		{
			if ((bool)Deselect)
			{
				Deselect.SetActive(GPSTracker.IsDrawing);
			}
		}
	}
}
