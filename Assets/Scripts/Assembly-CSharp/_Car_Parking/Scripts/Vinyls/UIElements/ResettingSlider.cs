using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Vinyls.UIElements
{
	public class ResettingSlider : MonoBehaviour, IPointerUpHandler, IEventSystemHandler, IPointerDownHandler
	{
		public float target = 0.5f;

		public float speed = 1f;

		public Slider slider;

		public bool touchingSlider;

		private void Update()
		{
			if ((bool)slider && !touchingSlider)
			{
				slider.value = Mathf.MoveTowards(slider.value, target, Time.deltaTime * speed);
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			touchingSlider = false;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			touchingSlider = true;
		}
	}
}
