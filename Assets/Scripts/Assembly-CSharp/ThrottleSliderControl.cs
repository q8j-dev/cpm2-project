using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;

public class ThrottleSliderControl : MonoBehaviour, IPointerUpHandler, IEventSystemHandler, IPointerDownHandler
{
	public ReactiveProperty<bool> IsThrottlePressed = new ReactiveProperty<bool>();

	public Slider ThrottleSlider;

	private CarInputManager _carInputManger;

	private InputItemsProvider _controlItemsProvider;

	private FreeDriveProvider _freeDriveProvider;

	private CompositeDisposable _disposables = new CompositeDisposable();

	private EventRegistry _eventRegistry;

	private RegistryHandle<BoolEvent> _onThrottleHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnThrottle);

	[Inject]
	public void Construct(InputItemsProvider controlItemsProvider, FreeDriveProvider freeDriveProvider, CarInputManager carInputManager)
	{
		_freeDriveProvider = freeDriveProvider;
		_controlItemsProvider = controlItemsProvider;
		_carInputManger = carInputManager;
	}

	private void Awake()
	{
		_controlItemsProvider.ThrottleSliderControl = this;
	}

	private void Start()
	{
		InitializeThrottleSliderEvents();
		_onThrottleHandle.Get(_eventRegistry)?.Subscribe(OnThrottle);
	}

	private void InitializeThrottleSliderEvents()
	{
		_disposables?.Dispose();
		_disposables = new CompositeDisposable();
		ThrottleSlider.onValueChanged.AddListener(UpdateInputAction);
		ThrottleSlider.OnPointerDownAsObservable().Subscribe((PointerEventData _) =>
		{
			OnThrottleSliderPressed();
		}).AddTo(_disposables);
		ThrottleSlider.OnPointerUpAsObservable().Subscribe((PointerEventData _) =>
		{
			OnThrottleSliderReleased();
		}).AddTo(_disposables);
		ThrottleSlider.OnDragAsObservable().Subscribe((PointerEventData _) =>
		{
			if (IsThrottlePressed.Value)
			{
				OnThrottleStateChanged(isPressed: true);
			}
		}).AddTo(_disposables);
	}

	private void OnThrottleStateChanged(bool isPressed)
	{
		if (isPressed)
		{
			HandleThrottlePressed();
		}
		else
		{
			HandleThrottleReleased();
		}
	}

	private void HandleThrottlePressed()
	{
		if (!_carInputManger.CarOptions.AnalogThrottle)
		{
			ThrottleSlider.value = 1f;
		}
	}

	private void HandleThrottleReleased()
	{
		if (ThrottleSlider.value > 0.1f)
		{
			ThrottleSlider.value = Mathf.Lerp(ThrottleSlider.value, 0f, Time.deltaTime / CarInputManager.ThrottleReleaseTime);
		}
		else
		{
			ThrottleSlider.value = 0f;
		}
	}

	private void OnDisable()
	{
		ResetThrottle();
		_onThrottleHandle.Get(_eventRegistry)?.Unsubscribe(OnThrottle);
	}

	private void OnThrottle(bool isPressed)
	{
		if (isPressed)
		{
			OnThrottleDownMessages();
		}
	}

	private void OnDestroy()
	{
		_disposables?.Dispose();
		_controlItemsProvider.ThrottleSliderControl = null;
	}

	private void UpdateInputAction(float value)
	{
		_carInputManger.SetThrottleFromSlider(value);
	}

	private void OnThrottleDownMessages()
	{
		if ((bool)_carInputManger.GetPowertrain && !_carInputManger.GetPowertrain.IsEngineWorks && !(_carInputManger.GetPowertrain.Starter > 0f))
		{
			bool num = _carInputManger.GetPowertrain.CurrentFuel < 0.01f;
			string message = (num ? new LocalizedString("ThrottleSliderControll.NoFuel") : new LocalizedString("ThrottleSliderControll.TurnOnEngine"));
			if (num && (bool)_freeDriveProvider.FreeDriveControl)
			{
				_freeDriveProvider.FreeDriveControl.ShowGetFuelMenu();
			}
			V.CoreGameManager.SetMessage(message, ScenesHelper.IsParkingLevelActive() ? 2 : 3);
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		IsThrottlePressed.Value = false;
		HandleThrottleReleased();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		IsThrottlePressed.Value = true;
	}

	public void OnThrottleSliderPressed()
	{
		IsThrottlePressed.Value = true;
		OnThrottleDownMessages();
	}

	public void OnThrottleSliderReleased()
	{
		IsThrottlePressed.Value = false;
	}

	public void OnThrottleReleased()
	{
		IsThrottlePressed.Value = false;
	}

	public void ResetThrottle()
	{
		IsThrottlePressed.Value = false;
		ThrottleSlider.value = 0f;
	}

	private void Update()
	{
		OnThrottleStateChanged(IsThrottlePressed.Value);
	}
}
