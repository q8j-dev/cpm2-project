using System;
using CPM.Shifter.Contracts;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.MenuUI
{
	public class ClutchSliderControl : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		private readonly ReactiveProperty<bool> _isClutchPressed = new ReactiveProperty<bool>();

		[SerializeField]
		private Slider ClutchSlider = default;

		private CarInputManager _carInputManger;

		private InputItemsProvider _inputItemsProvider;

		private Tweener _clutchTweener;

		private IDisposable _clutchSubscription;

		private CompositeDisposable _disposables = new CompositeDisposable();

		[Inject]
		public void Construct(FreeDriveProvider freeDriveProvider, CarInputManager carInputManager, InputItemsProvider inputItemsProvider)
		{
			_carInputManger = carInputManager;
			_inputItemsProvider = inputItemsProvider;
		}

		private void Start()
		{
			_clutchSubscription = _isClutchPressed.Subscribe(OnClutchStateChanged);
			InitializeClutchSliderEvents();
		}

		private void InitializeClutchSliderEvents()
		{
			_disposables?.Dispose();
			_disposables = new CompositeDisposable();
			ClutchSlider.OnValueChangedAsObservable().Subscribe(UpdateInputAction).AddTo(_disposables);
			ClutchSlider.OnPointerDownAsObservable().Subscribe((PointerEventData _) =>
			{
				OnClutchSliderPressed();
			}).AddTo(_disposables);
			ClutchSlider.OnPointerUpAsObservable().Subscribe((PointerEventData _) =>
			{
				OnClutchSliderReleased();
			}).AddTo(_disposables);
			ClutchSlider.OnDragAsObservable().Subscribe((PointerEventData _) =>
			{
				OnClutchStateChanged(isPressed: true);
			}).AddTo(_disposables);
		}

		private void OnClutchStateChanged(bool isPressed)
		{
			_clutchTweener?.Kill();
			if (isPressed)
			{
				HandleClutchPressed();
			}
			else
			{
				HandleClutchReleased();
			}
		}

		private void HandleClutchPressed()
		{
			if (!_carInputManger.CarOptions.AnalogClutch)
			{
				ClutchSlider.value = 1f;
			}
		}

		private void HandleClutchReleased()
		{
			if (ClutchSlider.value > 0.1f)
			{
				_clutchTweener = DOTween.To(() => ClutchSlider.value, (float x) =>
				{
					ClutchSlider.value = x;
				}, 0f, CarInputManager.ClutchReleaseTime * ClutchSlider.value).SetEase(Ease.OutQuad).OnComplete(NotifyShifterClutchReleased);
			}
			else
			{
				ClutchSlider.value = 0f;
				NotifyShifterClutchReleased();
			}
		}

		private void NotifyShifterClutchReleased()
		{
			IShifterControl shifterControl = _inputItemsProvider?.ShifterControl;
			if (shifterControl != null)
			{
				if (_carInputManger?.GetPowertrain != null)
				{
					shifterControl.SetPosByGearJoyStick(_carInputManger.GetPowertrain.Gear);
				}
				shifterControl.OnClutchReleased();
			}
		}

		private void OnDisable()
		{
			ResetClutch();
		}

		private void OnDestroy()
		{
			_disposables?.Dispose();
			_clutchTweener?.Kill();
			_clutchSubscription?.Dispose();
			_disposables?.Dispose();
		}

		private void UpdateInputAction(float value)
		{
			_carInputManger.SetClutchFromSlider(value);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			_isClutchPressed.Value = false;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			_isClutchPressed.Value = true;
		}

		public void OnClutchSliderPressed()
		{
			_isClutchPressed.Value = true;
		}

		public void OnClutchSliderReleased()
		{
			_isClutchPressed.Value = false;
			if (ClutchSlider.value > 0f)
			{
				ClutchSlider.value = 0f;
			}
		}

		public void ResetClutch()
		{
			_isClutchPressed.Value = false;
			ClutchSlider.value = 0f;
		}
	}
}
