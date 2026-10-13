using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarTuningUISlider : MonoBehaviour
{
	[SerializeField]
	private Slider Slider;

	[SerializeField]
	private Image[] SliderImages;

	[SerializeField]
	private TextMeshProUGUI ValueText;

	public void Init(Color color, float value)
	{
		ValueText.text = $"{value:p0}";
		Slider.value = value * 100f;
		for (int i = 0; i < SliderImages.Length; i++)
		{
			SliderImages[i].color = ColorHelperUtility.SetColorWithAlpha(color, SliderImages[i].color.a);
		}
	}

	public void Init(Color color, string text, float value)
	{
		ValueText.text = text;
		Slider.value = value;
		for (int i = 0; i < SliderImages.Length; i++)
		{
			SliderImages[i].color = ColorHelperUtility.SetColorWithAlpha(color, SliderImages[i].color.a);
		}
	}
}
