using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.SOAP;

namespace NewUI.Scripts1
{
	public class UGUIHandBrakeControl : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		[SerializeField]
		private Slider BrakeSlider;

		[SerializeField]
		private Image HandleImage = default;

		[SerializeField]
		private GameObject BrakeActiveImage = default;

		[SerializeField]
		private GameObject BrakeInactiveImage = default;

		[SerializeField]
		private Image ProgressOnImage = default;

		private Tween _progressOnTween;

		private CarControlValuesSO _carControlValuesSO;

		private CarInputManager _carInputManager;

		private bool _sliderPrevState;

		private const string HandBrakePull = "HandBrakePull";

		private const string HandBrakeRelease = "HandBrakeRelease";

		private const string UISwitch = "UiSwitch";

		private const string UIEvent = "Ui_Event";

		private EventRegistry _eventRegistry;

		private RegistryHandle<BoolEvent> _onHandbrakeHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnHandbrake);

		private RegistryHandle<BoolEvent> _onHandbrakeHoldHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnHandbrakeHold);

		private RegistryHandle<FloatVariable> _handbrakeHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.Handbrake);

		[Inject]
		public void Construct(CarControlValuesSO carControlValuesSO, EventRegistry eventRegistry, CarInputManager carInputManager)
		{
			_carControlValuesSO = carControlValuesSO;
			_eventRegistry = eventRegistry;
			_carInputManager = carInputManager;
		}

		private void Start()
		{
			if (!BrakeSlider)
			{
				BrakeSlider = GetComponent<Slider>();
			}
			if ((bool)BrakeSlider)
			{
				BrakeSlider.value = _handbrakeHandle.Get(_eventRegistry).Value;
				SetBrakeState();
			}
		}

		private void OnEnable()
		{
			if ((bool)BrakeSlider)
			{
				_handbrakeHandle.Get(_eventRegistry).Value = BrakeSlider.value;
			}
			_sliderPrevState = _handbrakeHandle.Get(_eventRegistry).Value > 0f;
			ProgressOnImage.enabled = false;
			_carInputManager.OnInputsCleared += OnInputsCleared;
			V.OnCarEntered += OnCarEntered;
			_onHandbrakeHoldHandle.Get(_eventRegistry).Subscribe(OnHandBrakeHoldPress);
			_onHandbrakeHandle.Get(_eventRegistry).Subscribe(OnHandBrakePress);
		}

		private void OnDisable()
		{
			_progressOnTween?.Kill();
			_carInputManager.OnInputsCleared -= OnInputsCleared;
			V.OnCarEntered -= OnCarEntered;
			_onHandbrakeHoldHandle.Get(_eventRegistry)?.Unsubscribe(OnHandBrakeHoldPress);
			_onHandbrakeHandle.Get(_eventRegistry)?.Unsubscribe(OnHandBrakePress);
		}

		private void OnCarEntered(GameObject person, VehicleData vehicleData)
		{
			bool flag = (((bool)vehicleData && (bool)vehicleData.GetPowertrain) ? vehicleData.GetPowertrain.Handbrake : 0f) > 0f;
			_progressOnTween?.Kill();
			ProgressOnImage.fillAmount = 0f;
			ProgressOnImage.enabled = false;
			HandleImage.enabled = false;
			_sliderPrevState = flag;
			BrakeSlider.SetValueWithoutNotify(flag ? 1 : 0);
			SetBrakeState();
			_carInputManager.PressHandbrake(flag ? 1 : 0);
			_handbrakeHandle.Get(_eventRegistry).Value = (flag ? 1f : 0f);
		}

		private void OnInputsCleared()
		{
			ProgressOnImage.fillAmount = 0f;
			if (BrakeSlider.value > 0f)
			{
				_progressOnTween?.Kill();
				ProgressOnImage.enabled = false;
				HandleImage.enabled = false;
				_carInputManager.PressHandbrake(BrakeSlider.value);
			}
			else
			{
				ProcessPointerUp(playReleaseSound: false);
			}
		}

		private void OnHandBrakeHoldPress(bool isPressed)
		{
			if (isPressed)
			{
				_handbrakeHandle.Get(_eventRegistry).Value = 1f;
				BrakeActiveImage.SetActive(value: true);
				OnBrakeSliderUpdated(1f);
				if (!_sliderPrevState)
				{
					PlayHandBrakeSound(1f);
				}
				return;
			}
			_progressOnTween?.Kill();
			if (Mathf.Approximately(ProgressOnImage.fillAmount, 1f))
			{
				BrakeSlider.value = 1f;
				_handbrakeHandle.Get(_eventRegistry).Value = 1f;
				_sliderPrevState = true;
			}
			else
			{
				_handbrakeHandle.Get(_eventRegistry).Reset();
				BrakeSlider.value = 0f;
				PlayHandBrakeSound(0f);
				_sliderPrevState = false;
			}
			ProgressOnImage.enabled = false;
			HandleImage.enabled = false;
			SetBrakeState();
		}

		private void OnHandBrakePress(bool isPressed)
		{
			BrakeSlider.value = 0f;
			_handbrakeHandle.Get(_eventRegistry).Value = (isPressed ? 1f : 0f);
			_progressOnTween?.Kill();
			if ((bool)BrakeActiveImage)
			{
				BrakeActiveImage.SetActive(isPressed);
			}
			if ((bool)BrakeInactiveImage)
			{
				BrakeInactiveImage.SetActive(!isPressed);
			}
			PlayHandBrakeSound(isPressed && !_sliderPrevState);
			_sliderPrevState = false;
		}

		private void PlayHandBrakeSound(bool isOn)
		{
			if ((bool)base.gameObject)
			{
				string state = (isOn ? "HandBrakePull" : "HandBrakeRelease");
				AudioWrapper.SetSwitch("UiSwitch", state, base.gameObject);
				AudioWrapper.PostEvent("Ui_Event", base.gameObject);
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			ProcessPointerDown(playPullSound: true);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			ProcessPointerUp(playReleaseSound: true);
		}

		private void ProcessPointerDown(bool playPullSound)
		{
			_carInputManager.PressHandbrake();
			ShowAllImages();
			if (playPullSound && !_sliderPrevState)
			{
				PlayHandBrakeSound(1f);
			}
		}

		private void ProcessPointerUp(bool playReleaseSound)
		{
			_progressOnTween?.Kill();
			if (Mathf.Approximately(ProgressOnImage.fillAmount, 1f))
			{
				BrakeSlider.value = 1f;
				_sliderPrevState = true;
			}
			else
			{
				BrakeSlider.value = 0f;
				if (playReleaseSound)
				{
					PlayHandBrakeSound(0f);
				}
				_sliderPrevState = false;
			}
			ProgressOnImage.enabled = false;
			HandleImage.enabled = false;
			SetBrakeState();
			_carInputManager.PressHandbrake(BrakeSlider.value);
		}

		private void ShowAllImages()
		{
			HandleImage.enabled = true;
			BrakeActiveImage.SetActive(value: true);
			BrakeInactiveImage.SetActive(value: true);
		}

		private void SetBrakeState()
		{
			bool flag = BrakeSlider.value != 0f;
			BrakeActiveImage.SetActive(flag);
			BrakeInactiveImage.SetActive(!flag);
		}

		private void OnBrakeSliderUpdated(float value)
		{
			_progressOnTween?.Kill();
			ProgressOnImage.fillAmount = 0f;
			ProgressOnImage.enabled = true;
			if (!(value <= 0f))
			{
				_progressOnTween = ProgressOnImage.DOFillAmount(1f, _carControlValuesSO.TimeToOnHandBrake);
				_progressOnTween.SetEase(Ease.Linear);
				_progressOnTween.OnComplete(() =>
				{
					ProgressOnImage.enabled = false;
				});
			}
		}

		private void PlayHandBrakeSound(float value)
		{
			if ((bool)base.gameObject)
			{
				string state = ((value > 0f) ? "HandBrakePull" : "HandBrakeRelease");
				AudioWrapper.SetSwitch("UiSwitch", state, base.gameObject);
				AudioWrapper.PostEvent("Ui_Event", base.gameObject);
			}
		}
	}
}
