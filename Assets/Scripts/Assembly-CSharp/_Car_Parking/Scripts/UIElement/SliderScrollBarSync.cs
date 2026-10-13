using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	[RequireComponent(typeof(Scrollbar))]
	public class SliderScrollBarSync : MonoBehaviour
	{
		[SerializeField]
		private Scrollbar Scrollbar = default;

		[SerializeField]
		private Slider Slider = default;

		private void Start()
		{
			Scrollbar.onValueChanged.AddListener(OnScrollbarChanging);
			Slider.onValueChanged.AddListener(OnSliderChanging);
			Slider.value = Scrollbar.value;
		}

		private void OnEnable()
		{
			Slider.SetActive(active: true);
		}

		private void OnDisable()
		{
			Slider.SetActive(active: false);
		}

		private void OnScrollbarChanging(float value)
		{
			Slider.value = value;
		}

		private void OnSliderChanging(float value)
		{
			Scrollbar.value = value;
		}
	}
}
