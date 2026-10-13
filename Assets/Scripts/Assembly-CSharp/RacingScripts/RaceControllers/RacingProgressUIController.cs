using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RacingScripts.RaceControllers
{
	public class RacingProgressUIController : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI ProgressText;

		[SerializeField]
		private Slider ProgressSlider;

		[SerializeField]
		private Slider GhostSlider;

		[SerializeField]
		private TextMeshProUGUI DistanceText;

		public void SetProgress(float progress)
		{
			ProgressSlider.value = progress;
		}

		public void SetGhostProgress(float progress)
		{
			if ((bool)GhostSlider)
			{
				GhostSlider.value = progress;
			}
		}

		public void SetGhostVisible(bool visible)
		{
			if ((bool)GhostSlider)
			{
				GhostSlider.gameObject.SetActive(visible);
			}
		}

		public void SetDistance(float distanceKm)
		{
			if (DistanceText != null)
			{
				DistanceText.text = $"{distanceKm:F2} km";
			}
		}

		public void SetProgressText(string text)
		{
			ProgressText.text = text;
		}
	}
}
