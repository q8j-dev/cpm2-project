using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class DraggyUIElement : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI FirstKmhTime = default;

		[SerializeField]
		private TextMeshProUGUI FirstKmhSlope = default;

		[SerializeField]
		private TextMeshProUGUI SecondKmhTime = default;

		[SerializeField]
		private TextMeshProUGUI SecondKmhSlope = default;

		[SerializeField]
		private TextMeshProUGUI MphTime = default;

		[SerializeField]
		private TextMeshProUGUI MphSlope = default;

		[SerializeField]
		private TextMeshProUGUI MileTime = default;

		[SerializeField]
		private TextMeshProUGUI MileSlope = default;

		[SerializeField]
		private Image FirstKmhIcon = default;

		[SerializeField]
		private Image SecondKmhIcon = default;

		[SerializeField]
		private Image MphIcon = default;

		[SerializeField]
		private Image MileKmhIcon = default;

		[SerializeField]
		private Sprite ValidSprite = default;

		[SerializeField]
		private Sprite InvalidSprite = default;

		[SerializeField]
		private GameObject FirstKmhTimeLine = default;

		[SerializeField]
		private GameObject FirstKmhSlopeLine = default;

		[SerializeField]
		private GameObject SecondKmhTimeLine = default;

		[SerializeField]
		private GameObject SecondKmhSlopeLine = default;

		[SerializeField]
		private GameObject MphTimeLine = default;

		[SerializeField]
		private GameObject MphSlopeLine = default;

		[SerializeField]
		private GameObject MileTimeLine = default;

		[SerializeField]
		private GameObject MileSlopeLine = default;

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
