using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class DraggyUIElement : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI FirstKmhTime;

		[SerializeField]
		private TextMeshProUGUI FirstKmhSlope;

		[SerializeField]
		private TextMeshProUGUI SecondKmhTime;

		[SerializeField]
		private TextMeshProUGUI SecondKmhSlope;

		[SerializeField]
		private TextMeshProUGUI MphTime;

		[SerializeField]
		private TextMeshProUGUI MphSlope;

		[SerializeField]
		private TextMeshProUGUI MileTime;

		[SerializeField]
		private TextMeshProUGUI MileSlope;

		[SerializeField]
		private Image FirstKmhIcon;

		[SerializeField]
		private Image SecondKmhIcon;

		[SerializeField]
		private Image MphIcon;

		[SerializeField]
		private Image MileKmhIcon;

		[SerializeField]
		private Sprite ValidSprite;

		[SerializeField]
		private Sprite InvalidSprite;

		[SerializeField]
		private GameObject FirstKmhTimeLine;

		[SerializeField]
		private GameObject FirstKmhSlopeLine;

		[SerializeField]
		private GameObject SecondKmhTimeLine;

		[SerializeField]
		private GameObject SecondKmhSlopeLine;

		[SerializeField]
		private GameObject MphTimeLine;

		[SerializeField]
		private GameObject MphSlopeLine;

		[SerializeField]
		private GameObject MileTimeLine;

		[SerializeField]
		private GameObject MileSlopeLine;

		public void Init(Dragy dragy)
		{
			if (dragy.kmh100Result == 0f)
			{
				FirstKmhTimeLine.SetActive(value: true);
				FirstKmhTime.gameObject.SetActive(value: false);
			}
			else
			{
				FirstKmhTime.text = dragy.kmh100Result.ToString("F2") + "s";
			}
			if (dragy.kmh100slope == 0f)
			{
				FirstKmhSlopeLine.SetActive(value: true);
				FirstKmhSlope.gameObject.SetActive(value: false);
			}
			else
			{
				FirstKmhSlope.text = dragy.kmh100slope.ToString("F2") + "%";
				FirstKmhIcon.sprite = ((dragy.kmh100slope > -1f) ? ValidSprite : InvalidSprite);
			}
			if (dragy.kmh100200Result == 0f)
			{
				SecondKmhTimeLine.SetActive(value: true);
				SecondKmhTime.gameObject.SetActive(value: false);
			}
			else
			{
				SecondKmhTime.text = dragy.kmh100200Result.ToString("F2") + "s";
			}
			if (dragy.kmh100200Slope == 0f)
			{
				SecondKmhSlopeLine.SetActive(value: true);
				SecondKmhSlope.gameObject.SetActive(value: false);
			}
			else
			{
				SecondKmhSlope.text = dragy.kmh100200Slope.ToString("F2") + "%";
				SecondKmhIcon.sprite = ((dragy.kmh100200Slope > -1f) ? ValidSprite : InvalidSprite);
			}
			if (dragy.mph60Result == 0f)
			{
				MphTimeLine.SetActive(value: true);
				MphTime.gameObject.SetActive(value: false);
			}
			else
			{
				MphTime.text = dragy.mph60Result.ToString("F2") + "s";
			}
			if (dragy.mph60Slope == 0f)
			{
				MphSlopeLine.SetActive(value: true);
				MphSlope.gameObject.SetActive(value: false);
			}
			else
			{
				MphSlope.text = dragy.mph60Slope.ToString("F2") + "%";
				MphIcon.sprite = ((dragy.mph60Slope > -1f) ? ValidSprite : InvalidSprite);
			}
			if (dragy.m402Result == 0f)
			{
				MileTimeLine.SetActive(value: true);
				MileTime.gameObject.SetActive(value: false);
			}
			else
			{
				MileTime.text = dragy.m402Result.ToString("F2") + "s";
			}
			if (dragy.m402Slope == 0f)
			{
				MileSlopeLine.SetActive(value: true);
				MileSlope.gameObject.SetActive(value: false);
			}
			else
			{
				MileSlope.text = dragy.m402Slope.ToString("F2") + "%";
				MileKmhIcon.sprite = ((dragy.m402Slope > -1f) ? ValidSprite : InvalidSprite);
			}
		}
	}
}
