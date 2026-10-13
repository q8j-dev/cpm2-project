using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	[RequireComponent(typeof(Button))]
	public class PersonJumpButton : MonoBehaviour
	{
		private Button _button;

		private Image _image;

		private bool _pressed;

		private void Awake()
		{
			_button = GetComponent<Button>();
			_image = GetComponent<Image>();
		}

		private void OnEnable()
		{
			if ((bool)_button)
			{
				_button.onClick.RemoveListener(OnClick);
				_button.onClick.AddListener(OnClick);
			}
			V.OnOutOfCarChanged = (Action<bool>)Delegate.Remove(V.OnOutOfCarChanged, new Action<bool>(OutOfCarChangedCallback));
			V.OnOutOfCarChanged = (Action<bool>)Delegate.Combine(V.OnOutOfCarChanged, new Action<bool>(OutOfCarChangedCallback));
			ApplyState(FlagsStorage.OutOfCar);
		}

		private void OnDisable()
		{
			if ((bool)_button)
			{
				_button.onClick.RemoveListener(OnClick);
			}
			V.OnOutOfCarChanged = (Action<bool>)Delegate.Remove(V.OnOutOfCarChanged, new Action<bool>(OutOfCarChangedCallback));
		}

		public bool ConsumePressed()
		{
			if (!_pressed)
			{
				return false;
			}
			_pressed = false;
			return true;
		}

		private void OnClick()
		{
			_pressed = true;
		}

		private void OutOfCarChangedCallback(bool outOfCar)
		{
			ApplyState(outOfCar);
		}

		private void ApplyState(bool outOfCar)
		{
			if ((bool)_image)
			{
				_image.enabled = outOfCar;
			}
			if (!outOfCar)
			{
				_pressed = false;
			}
		}
	}
}
