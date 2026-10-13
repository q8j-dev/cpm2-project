using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class PersonSprintLockIndicator : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		private Image _image;

		private void Awake()
		{
			_image = GetComponent<Image>();
			if ((bool)_image)
			{
				_image.enabled = false;
			}
		}

		private void OnDisable()
		{
			if ((bool)_image)
			{
				_image.enabled = false;
			}
		}

		private void Update()
		{
			bool flag = FlagsStorage.OutOfCar && InputStorage.SprintLocked;
			if ((bool)_image && _image.enabled != flag)
			{
				_image.enabled = flag;
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (InputStorage.SprintLocked)
			{
				InputStorage.SprintLockCancelRequested = true;
			}
		}
	}
}
