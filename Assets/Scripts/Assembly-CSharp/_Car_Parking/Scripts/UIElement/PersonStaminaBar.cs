using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Person.Locomotion;

namespace _Car_Parking.Scripts.UIElement
{
	public class PersonStaminaBar : MonoBehaviour
	{
		private const float FullEpsilon = 0.0001f;

		[SerializeField]
		private Slider _slider;

		[SerializeField]
		private CanvasGroup _canvasGroup;

		[SerializeField]
		private float _hideDelay = 10f;

		[SerializeField]
		private float _fadeDuration = 0.5f;

		private GameObject _cachedPerson;

		private PersonLocomotion _locomotion;

		private float _lastValue = 1f;

		private float _idleTimer;

		private float _alpha;

		private float _target;

		private void Awake()
		{
			if (!_slider)
			{
				_slider = GetComponentInChildren<Slider>(includeInactive: true);
			}
			if (!_canvasGroup)
			{
				TryGetComponent<CanvasGroup>(out _canvasGroup);
			}
		}

		private void OnEnable()
		{
			_alpha = 0f;
			_target = 0f;
			_idleTimer = 0f;
			ApplyAlpha();
		}

		private void Update()
		{
			PersonLocomotion personLocomotion = ResolveLocomotion();
			bool num = personLocomotion;
			float num2 = (_lastValue = (num ? personLocomotion.StaminaNormalized : _lastValue));
			if ((bool)_slider)
			{
				_slider.value = num2;
			}
			if (num && num2 < 0.9999f)
			{
				_idleTimer = 0f;
				_target = 1f;
			}
			else
			{
				_idleTimer += Time.deltaTime;
				if (_idleTimer >= _hideDelay)
				{
					_target = 0f;
				}
			}
			_alpha = Mathf.MoveTowards(_alpha, _target, Time.deltaTime / Mathf.Max(0.0001f, _fadeDuration));
			ApplyAlpha();
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

		private void ApplyAlpha()
		{
			if ((bool)_canvasGroup)
			{
				_canvasGroup.alpha = _alpha;
				_canvasGroup.blocksRaycasts = false;
				_canvasGroup.interactable = false;
			}
		}
	}
}
