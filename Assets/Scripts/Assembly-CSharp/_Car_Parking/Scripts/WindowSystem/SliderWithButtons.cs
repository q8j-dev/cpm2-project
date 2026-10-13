using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class SliderWithButtons : MonoBehaviour
	{
		[SerializeField]
		private float Step = 1f;

		[SerializeField]
		private float MinValue;

		[SerializeField]
		private float MaxValue = 10f;

		[SerializeField]
		private Slider Slider;

		[SerializeField]
		private Button Plus;

		[SerializeField]
		private Button Minus;

		[SerializeField]
		private TMP_InputField TextValueInput;

		public event Action<float> ValueChanged;

		private void Start()
		{
			Slider.onValueChanged.AddListener(ChangedSlider);
			Plus.onClick.AddListener(PlusPressed);
			Minus.onClick.AddListener(MinusPressed);
			TextValueInput.onEndEdit.AddListener(TextChanged);
			Slider.minValue = MinValue;
			Slider.maxValue = MaxValue;
			UpdateTextInInputField();
		}

		public void SetMaxValue(float val)
		{
			Slider.maxValue = val;
		}

		public void SetValueWithoutNotify(float data)
		{
			Slider.SetValueWithoutNotify(data);
		}

		private void TextChanged(string arg0)
		{
			if (float.TryParse(arg0, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
			{
				ChangeValue(result);
			}
			else
			{
				Debug.LogError("Can't parse value " + arg0);
			}
		}

		private void MinusPressed()
		{
			ChangeValue(Slider.value - Step);
		}

		private void PlusPressed()
		{
			ChangeValue(Slider.value + Step);
		}

		public void ChangeValue(float result)
		{
			Slider.value = result;
		}

		private void ChangedSlider(float arg0)
		{
			ValueChanged?.Invoke(arg0);
			UpdateTextInInputField();
			Debug.Log($"New value: {arg0}");
		}

		private void UpdateTextInInputField()
		{
			float value = Slider.value;
			string text = "";
			text = ((value != (float)Mathf.RoundToInt(value)) ? value.ToString("F2") : value.ToString("F0"));
			TextValueInput.SetTextWithoutNotify(text);
		}
	}
}
