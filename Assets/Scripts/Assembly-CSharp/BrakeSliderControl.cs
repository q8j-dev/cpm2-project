using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Providers;

public class BrakeSliderControl : MonoBehaviour, IPointerUpHandler, IEventSystemHandler, IPointerDownHandler
{
	public ReactiveProperty<bool> IsBrakePressed = new ReactiveProperty<bool>();

	public Slider BrakeSlider;

	private InputItemsProvider _controlItemsProvider;

	private CarInputManager _carInputManger;

	private CompositeDisposable _disposables = new CompositeDisposable();

	[Inject]
	public void Construct(InputItemsProvider controlItemsProvider, CarInputManager carInputManager)
	{
		_controlItemsProvider = controlItemsProvider;
		_carInputManger = carInputManager;
	}

	private void Awake()
	{
		_controlItemsProvider.BrakeSliderControl = this;
	}

	private void Start()
	{
		InitializeBrakeSliderEvents();
	}

	private void InitializeBrakeSliderEvents()
	{
		_disposables?.Dispose();
		_disposables = new CompositeDisposable();
		BrakeSlider.OnValueChangedAsObservable().Subscribe(UpdateInputAction).AddTo(_disposables);
		BrakeSlider.OnPointerDownAsObservable().Subscribe((PointerEventData _) =>
		{
			OnBrakeSliderPressed();
		}).AddTo(_disposables);
		BrakeSlider.OnPointerUpAsObservable().Subscribe((PointerEventData _) =>
		{
			OnBrakeSliderReleased();
		}).AddTo(_disposables);
		BrakeSlider.OnDragAsObservable().Subscribe((PointerEventData _) =>
		{
			if (IsBrakePressed.Value)
			{
				OnBrakeStateChanged(isPressed: true);
			}
		}).AddTo(_disposables);
	}

	private void OnBrakeStateChanged(bool isPressed)
	{
		if (isPressed)
		{
			HandleBrakePressed();
		}
		else
		{
			HandleBrakeReleased();
		}
	}

	private void HandleBrakePressed()
	{
		if (!_carInputManger.CarOptions.AnalogBrake)
		{
			BrakeSlider.value = 1f;
		}
	}

	private void HandleBrakeReleased()
	{
		if (BrakeSlider.value > 0.1f)
		{
			BrakeSlider.value = Mathf.Lerp(BrakeSlider.value, 0f, Time.deltaTime / CarInputManager.BrakesReleaseTime);
		}
		else
		{
			BrakeSlider.value = 0f;
		}
	}

	private void OnDisable()
	{
		ResetBrake();
	}

	private void OnDestroy()
	{
		_disposables?.Dispose();
		_controlItemsProvider.BrakeSliderControl = null;
	}

	private void UpdateInputAction(float value)
	{
		_carInputManger.SetBrakeFromSlider(value);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		IsBrakePressed.Value = false;
		HandleBrakeReleased();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		IsBrakePressed.Value = true;
	}

	public void OnBrakeSliderPressed()
	{
		IsBrakePressed.Value = true;
	}

	public void OnBrakeSliderReleased()
	{
		IsBrakePressed.Value = false;
	}

	public void OnBrakePressed()
	{
		IsBrakePressed.Value = true;
	}

	public void OnBrakeReleased()
	{
		IsBrakePressed.Value = false;
	}

	private void ResetBrake()
	{
		IsBrakePressed.Value = false;
		BrakeSlider.value = 0f;
	}

	private void Update()
	{
		OnBrakeStateChanged(IsBrakePressed.Value);
	}
}
