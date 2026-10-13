using TMPro;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement
{
	public class CustomSliderValueHandler : SliderValueHandler
	{
		private const float DefaultMinAncorValue = 0f;

		private const float DefaultMaxAncorValue = 1f;

		[SerializeField]
		private TextMeshProUGUI LeftText;

		[SerializeField]
		private TextMeshProUGUI ButtonPlusTopText;

		[SerializeField]
		private TextMeshProUGUI ButtonMinusTopText;

		[SerializeField]
		private RectTransform LeftLineLimitsLine;

		[SerializeField]
		private RectTransform RightLineLimitsLine;

		[SerializeField]
		private GameObject RedBckgLine;

		[SerializeField]
		private RectTransform RectTransform;

		[SerializeField]
		private float LeftMinAncor;

		[SerializeField]
		private float LeftMaxAncor;

		[SerializeField]
		private float RightMinAncor;

		[SerializeField]
		private float RightMaxAncor;

		[SerializeField]
		private GameObject TitlePanel;

		public AlphaHidableObject AlphaHidableObject;

		public GameObject Parent;

		private float _minValueLimit;

		private float _maxValueLimit;

		private void OnDisable()
		{
			ShowLimitLines(state: false);
			RectTransform.anchorMin = new Vector2(0f, 0f);
			RectTransform.anchorMax = new Vector2(1f, 1f);
		}

		private void Start()
		{
			AlphaHidableObject = Parent.GetComponent<AlphaHidableObject>();
			OnStart();
		}

		public override void Init(float minValue, float maxValue, float step, bool isRoundValue = false, bool removeSpecialListener = false, int numberAfterFloat = 2, bool ignoreSetDefaultValue = false)
		{
			float value = CurrentSlider.value;
			base.Init(minValue, maxValue, step, isRoundValue, removeSpecialListener, numberAfterFloat);
			SetNewLimitedValue(value);
		}

		public override void OnStart()
		{
			base.OnStart();
			RectTransform = CurrentSlider.GetComponent<RectTransform>();
		}

		public void SetSliderTexts(string title)
		{
			LeftText.text = title;
		}

		public void ShowLimitLines(bool state)
		{
			LeftLineLimitsLine.gameObject.SetActive(state);
			RightLineLimitsLine.gameObject.SetActive(state);
			RedBckgLine.gameObject.SetActive(state);
		}

		public void ShowSliderTitlTexts(bool state)
		{
			ButtonPlusTopText.gameObject.SetActive(state);
			ButtonMinusTopText.gameObject.SetActive(state);
			TitlePanel.SetActive(state);
		}

		public void SetButtonsTitleText(string plusButton, string minusButton)
		{
			ShowSliderTitlTexts(!string.IsNullOrEmpty(plusButton) && !string.IsNullOrEmpty(minusButton));
			ButtonPlusTopText.text = plusButton;
			ButtonMinusTopText.text = minusButton;
		}

		private void SetNewLimitedValue(float value)
		{
			float value2 = value;
			if (value > CurrentSlider.maxValue)
			{
				value2 = CurrentSlider.maxValue;
			}
			if (value < CurrentSlider.minValue)
			{
				value2 = CurrentSlider.minValue;
			}
			CurrentSlider.value = value2;
		}

		public void SetMinMaxLineLimits(float minDefaultSliderValue, float maxDefaultSliderValue, float limitMinValue, float limitMaxValue)
		{
			RedBckgLine.SetActive(value: true);
			_minValueLimit = limitMinValue;
			_maxValueLimit = limitMaxValue;
			float value = (limitMinValue - minDefaultSliderValue) / (maxDefaultSliderValue - minDefaultSliderValue);
			float value2 = (limitMaxValue - minDefaultSliderValue) / (maxDefaultSliderValue - minDefaultSliderValue);
			value = Mathf.Clamp(value, LeftMinAncor, LeftMaxAncor);
			value2 = Mathf.Clamp(value2, RightMinAncor, RightMaxAncor);
			RectTransform.anchorMin = new Vector2(value, 0f);
			RectTransform.anchorMax = new Vector2(value2, 1f);
			bool active = value2 != RightMaxAncor;
			bool active2 = value != LeftMinAncor;
			LeftLineLimitsLine.gameObject.SetActive(active2);
			RightLineLimitsLine.gameObject.SetActive(active);
		}
	}
}
