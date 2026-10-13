using System;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Person.Locomotion;

namespace _Car_Parking.Scripts.UIElement
{
	[RequireComponent(typeof(Button))]
	public class PersonCrouchButton : MonoBehaviour
	{
		[SerializeField]
		private Image _crouchIcon = default;

		[SerializeField]
		private Image _slideIcon = default;

		[SerializeField]
		private float _fadeDuration = 0.15f;

		private Button _button;

		private Image _image;

		private GameObject _cachedPerson;

		private PersonLocomotion _locomotion;

		private float _crouchAlpha;

		private float _slideAlpha;

		private bool _wasShown;

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
			_wasShown = false;
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

		private void OnClick()
		{
			if (FlagsStorage.OutOfCar)
			{
				InputStorage.CrouchRequested = true;
			}
		}

		private void OutOfCarChangedCallback(bool outOfCar)
		{
			ApplyState(outOfCar);
			if (!outOfCar)
			{
				InputStorage.CrouchRequested = false;
			}
		}

		private void Update()
		{
			ApplyState(FlagsStorage.OutOfCar);
		}

		private void ApplyState(bool outOfCar)
		{
			bool flag = outOfCar;
			if ((bool)_image && _image.enabled != flag)
			{
				_image.enabled = flag;
			}
			PersonLocomotion personLocomotion = ResolveLocomotion();
			bool flag2 = flag && (bool)personLocomotion && personLocomotion.SlideOnCrouch;
			bool fade = flag && _wasShown && _fadeDuration > 0f;
			_wasShown = flag;
			FadeIcon(_crouchIcon, ref _crouchAlpha, flag && !flag2, fade);
			FadeIcon(_slideIcon, ref _slideAlpha, flag2, fade);
		}

		private void FadeIcon(Image icon, ref float alpha, bool visible, bool fade)
		{
			float num = (visible ? 1f : 0f);
			alpha = (fade ? Mathf.MoveTowards(alpha, num, Time.unscaledDeltaTime / _fadeDuration) : num);
			if ((bool)icon)
			{
				Color color = icon.color;
				if (!Mathf.Approximately(color.a, alpha))
				{
					color.a = alpha;
					icon.color = color;
				}
				bool flag = alpha > 0f;
				if (icon.gameObject.activeSelf != flag)
				{
					icon.gameObject.SetActive(flag);
				}
			}
		}

		private PersonLocomotion ResolveLocomotion()
		{
			GameObject driverPerson = V.DriverPerson;
			if (driverPerson != _cachedPerson)
			{
				_cachedPerson = driverPerson;
				_locomotion = (driverPerson ? driverPerson.GetComponent<PersonLocomotion>() : null);
			}
			return _locomotion;
		}
	}
}
