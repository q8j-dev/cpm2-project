using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace _Car_Parking.Scripts.UIElement
{
	public class HornButtonHandler : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		private QuickCarSettings _quickCarSettings;

		private bool _pressed;

		[Inject]
		public void Construct(QuickCarSettings quickCarSettings)
		{
			_quickCarSettings = quickCarSettings;
		}

		private void OnDisable()
		{
			if (_pressed)
			{
				OnHornReleased();
			}
		}

		private void OnHornPressed()
		{
			if ((bool)_quickCarSettings)
			{
				_quickCarSettings.FlashHeadLights();
			}
		}

		private void OnHornReleased()
		{
			if ((bool)_quickCarSettings && (bool)V.Car)
			{
				_quickCarSettings.onFlashReleased();
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			_pressed = true;
			OnHornPressed();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			_pressed = false;
			OnHornReleased();
		}
	}
}
