using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class PersonSprintButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		private Image _image;

		private bool _held;

		public bool Held => _held;

		private void Awake()
		{
			_image = GetComponent<Image>();
		}

		private void OnEnable()
		{
			V.OnOutOfCarChanged = (Action<bool>)Delegate.Remove(V.OnOutOfCarChanged, new Action<bool>(OutOfCarChangedCallback));
			V.OnOutOfCarChanged = (Action<bool>)Delegate.Combine(V.OnOutOfCarChanged, new Action<bool>(OutOfCarChangedCallback));
			ApplyState(FlagsStorage.OutOfCar);
		}

		private void OnDisable()
		{
			V.OnOutOfCarChanged = (Action<bool>)Delegate.Remove(V.OnOutOfCarChanged, new Action<bool>(OutOfCarChangedCallback));
			_held = false;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			_held = true;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			_held = false;
		}

		private void Update()
		{
			ApplyState(FlagsStorage.OutOfCar);
		}

		private void OutOfCarChangedCallback(bool outOfCar)
		{
			ApplyState(outOfCar);
		}

		private void ApplyState(bool outOfCar)
		{
			bool flag = outOfCar && !InputStorage.DeflectionSprintMode;
			if ((bool)_image && _image.enabled != flag)
			{
				_image.enabled = flag;
			}
			if (!flag)
			{
				_held = false;
			}
		}
	}
}
