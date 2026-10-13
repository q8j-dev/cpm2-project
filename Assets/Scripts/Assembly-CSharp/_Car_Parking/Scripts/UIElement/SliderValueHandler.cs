using System;
using System.Threading;
using CPMEngine;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.UIElement
{
	public class SliderValueHandler : MonoBehaviour, IPointerUpHandler, IEventSystemHandler, IPointerDownHandler, IDeselectHandler
	{
		[SerializeField]
		private TextMeshProUGUI HeaderText;

		public Slider CurrentSlider;

		[SerializeField]
		private TextMeshProUGUI CurrentTmPro;

		[SerializeField]
		private TextMeshProUGUI CurrentTmProDouble;

		[SerializeField]
		private Button Plus;

		[SerializeField]
		private Button Minus;

		[SerializeField]
		private SliderStepRepeater PlusRepeater;

		[SerializeField]
		private SliderStepRepeater MinusRepeater;

		[SerializeField]
		private float MaxValue;

		[SerializeField]
		private float MinValue;

		private float _centerSliderValue;

		[SerializeField]
		private float SlideStep;

		[SerializeField]
		private bool IgnoreStart;

		[SerializeField]
		private bool RoundValue;

		[SerializeField]
		private bool SetPercentSign;

		[SerializeField]
		private int ValueRoundValue = 2;

		private float _cachedMaxAncorPos;

		private float _cachedMinAncorPos;

		private float _currentValueToView;

		private float _step;

		private bool _stepConfigured;

		public Action OnPointerUpAction;

		public Action OnPointerDownAction;

		[CanBeNull]
		private Func<float, string> _toStringFunc;

		[SerializeField]
		private float HoldInitialDelay = 0.5f;

		[SerializeField]
		private float HoldRepeatInterval = 0.15f;

		[SerializeField]
		private float HoldStepAcceleration = 0.5f;

		[SerializeField]
		private float HoldMaxStepMultiplier = 10f;

		private CancellationTokenSource _holdCts;

		private UiNavigationValueEvent _uiNavigationValueEvent;

		private EventRegistry _eventRegistry;

		private RegistryHandle<BoolEvent> _acceptHoldHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.AcceptHold);

		public event Action<float> OnReleased;

		[Inject]
		public void Construct(UiNavigationValueEvent uiNavigationValueEvent, EventRegistry eventRegistry)
		{
			_uiNavigationValueEvent = uiNavigationValueEvent;
			_eventRegistry = eventRegistry;
		}

		private void Start()
		{
			OnStart();
			Navigation navigation = CurrentSlider.navigation;
			navigation.mode = Navigation.Mode.None;
			CurrentSlider.navigation = navigation;
		}

		public void OnDeselect(BaseEventData eventData)
		{
			if (_uiNavigationValueEvent.StoredValue == UiNavigationEnum.Keys)
			{
				UISelectionCursor.ShowCursor();
			}
		}

		public void SetCurrentValueTextActive(bool state)
		{
			CurrentTmPro.gameObject.SetActive(state);
		}

		public virtual void OnStart()
		{
			if ((bool)Plus)
			{
				Plus.onClick.AddListener(PlusStep);
			}
			if ((bool)Minus)
			{
				Minus.onClick.AddListener(MinusStep);
			}
			if ((bool)Plus)
			{
				WireHoldRepeat(Plus, 1);
			}
			if ((bool)Minus)
			{
				WireHoldRepeat(Minus, -1);
			}
			_acceptHoldHandle.Get(_eventRegistry)?.Subscribe(OnAcceptHold);
			if (!IgnoreStart)
			{
				if (!_stepConfigured)
				{
					_step = SlideStep / MaxValue;
				}
				CurrentSlider.onValueChanged.AddListener(SetText);
				SetText(CurrentSlider.value);
			}
		}

		public void ShowHeaderText(bool state)
		{
			if ((bool)HeaderText)
			{
				HeaderText.SetActive(state);
			}
		}

		public void SetHeaderText(string text)
		{
			if ((bool)HeaderText)
			{
				HeaderText.SetText(text);
			}
		}

		public void SetMinMaxValues(float min, float max, float value)
		{
			MinValue = min;
			MaxValue = max;
			CurrentSlider.minValue = MinValue;
			CurrentSlider.maxValue = MaxValue;
			CurrentSlider.SetValueWithoutNotify(value);
		}

		public virtual void Init(float minValue, float maxValue, float step, bool isRoundValue = false, bool removeSpecialListener = false, int numberAfterFloat = 2, bool ignoreSetDefaultValue = false)
		{
			SetCurrentValueTextActive(state: true);
			_ = CurrentSlider.value;
			MinValue = minValue;
			MaxValue = maxValue;
			_centerSliderValue = (minValue + maxValue) * 0.5f;
			CurrentSlider.minValue = minValue;
			CurrentSlider.maxValue = maxValue;
			CurrentSlider.wholeNumbers = isRoundValue;
			ValueRoundValue = numberAfterFloat;
			_step = step;
			_stepConfigured = true;
			string sourceText = Utils.RoundTo(minValue, ValueRoundValue).ToString();
			if ((bool)CurrentTmPro)
			{
				CurrentTmPro.SetText(sourceText);
			}
			if ((bool)CurrentTmProDouble)
			{
				CurrentTmProDouble.SetText(sourceText);
			}
			if (!removeSpecialListener)
			{
				CurrentSlider.onValueChanged.RemoveAllListeners();
			}
			else
			{
				CurrentSlider.onValueChanged.RemoveListener(isRoundValue ? new UnityAction<float>(SetTextRounded) : new UnityAction<float>(SetTextWithoutNotify));
			}
			if (isRoundValue)
			{
				CurrentSlider.onValueChanged.AddListener(SetTextRounded);
			}
			else
			{
				CurrentSlider.onValueChanged.AddListener(SetTextWithoutNotify);
			}
			if (!ignoreSetDefaultValue)
			{
				CurrentSlider.value = minValue;
			}
		}

		public void Init(Func<float, string> toString)
		{
			_toStringFunc = toString;
		}

		public void SetText(float value)
		{
			_currentValueToView = (int)(value * MaxValue);
			if ((bool)CurrentTmPro)
			{
				if (_toStringFunc != null)
				{
					CurrentTmPro.text = _toStringFunc(value);
				}
				else if (SetPercentSign)
				{
					CurrentTmPro.text = _currentValueToView + "%";
				}
				else
				{
					CurrentTmPro.text = _currentValueToView.ToString();
				}
			}
		}

		public void SetTextWithoutNotify(float value)
		{
			if ((bool)CurrentTmPro)
			{
				CurrentTmPro.text = Utils.RoundTo(value, ValueRoundValue).ToString();
			}
		}

		public void SetTextRounded(float value)
		{
			if ((bool)CurrentTmPro)
			{
				int num = Mathf.RoundToInt(value);
				if (SetPercentSign)
				{
					CurrentTmPro.text = num + "%";
				}
				else if ((bool)CurrentTmProDouble)
				{
					float num2 = (float)num - _centerSliderValue;
					float num3 = _centerSliderValue - num2;
					CurrentTmProDouble.text = $"{num}";
					CurrentTmPro.text = $"{num3}";
				}
				else
				{
					CurrentTmPro.text = num.ToString();
				}
			}
		}

		private void PlusStep()
		{
			if (!(PlusRepeater != null) || !PlusRepeater.ConsumeClickSuppression())
			{
				ApplySteps(1);
				NotifyReleased();
			}
		}

		private void MinusStep()
		{
			if (!(MinusRepeater != null) || !MinusRepeater.ConsumeClickSuppression())
			{
				ApplySteps(-1);
				NotifyReleased();
			}
		}

		private void SetupRepeater(SliderStepRepeater repeater, Button button, int direction)
		{
			if (!(repeater == null))
			{
				repeater.Setup(direction, button, RangeInSteps, ApplySteps, NotifyReleased);
			}
		}

		private float RangeInSteps()
		{
			if (_step <= 0f)
			{
				return 0f;
			}
			return (CurrentSlider.maxValue - CurrentSlider.minValue) / _step;
		}

		private void ApplySteps(int signedSteps)
		{
			CurrentSlider.value += (float)signedSteps * _step;
		}

		private void NotifyReleased()
		{
			float num = Utils.RoundTo(CurrentSlider.value, ValueRoundValue);
			OnReleased?.Invoke(RoundValue ? num : CurrentSlider.value);
		}

		private void WireHoldRepeat(Button button, int sign)
		{
			EventTrigger eventTrigger = button.gameObject.GetComponent<EventTrigger>();
			if (!eventTrigger)
			{
				eventTrigger = button.gameObject.AddComponent<EventTrigger>();
			}
			EventTrigger.Entry entry = new EventTrigger.Entry
			{
				eventID = EventTriggerType.PointerDown
			};
			entry.callback.AddListener((BaseEventData _) =>
			{
				StartHold(sign);
			});
			eventTrigger.triggers.Add(entry);
			EventTrigger.Entry entry2 = new EventTrigger.Entry
			{
				eventID = EventTriggerType.PointerUp
			};
			entry2.callback.AddListener((BaseEventData _) =>
			{
				StopHold();
			});
			eventTrigger.triggers.Add(entry2);
		}

		private void OnAcceptHold(bool isPressed)
		{
			if (!isPressed)
			{
				StopHold();
			}
			else if (base.isActiveAndEnabled && !(UnityEngine.EventSystems.EventSystem.current == null))
			{
				GameObject currentSelectedGameObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
				if ((bool)Plus && currentSelectedGameObject == Plus.gameObject)
				{
					StartHold(1);
				}
				else if ((bool)Minus && currentSelectedGameObject == Minus.gameObject)
				{
					StartHold(-1);
				}
			}
		}

		private void StartHold(int sign)
		{
			StopHold();
			_holdCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
			HoldLoop(sign, _holdCts.Token).Forget();
		}

		private void StopHold()
		{
			if (_holdCts != null)
			{
				_holdCts.Cancel();
				_holdCts.Dispose();
				_holdCts = null;
			}
		}

		private void StepScaled(int sign, float multiplier)
		{
			CurrentSlider.value += (float)sign * _step * multiplier;
			float num = Utils.RoundTo(CurrentSlider.value, ValueRoundValue);
			OnReleased?.Invoke(RoundValue ? num : CurrentSlider.value);
		}

		private async UniTaskVoid HoldLoop(int sign, CancellationToken ct)
		{
			_ = 1;
			try
			{
				await UniTask.Delay(TimeSpan.FromSeconds(HoldInitialDelay), DelayType.Realtime, PlayerLoopTiming.Update, ct);
				float multiplier = 1f;
				while (!ct.IsCancellationRequested && base.isActiveAndEnabled)
				{
					StepScaled(sign, multiplier);
					multiplier = Mathf.Min(multiplier + HoldStepAcceleration, HoldMaxStepMultiplier);
					await UniTask.Delay(TimeSpan.FromSeconds(HoldRepeatInterval), DelayType.Realtime, PlayerLoopTiming.Update, ct);
				}
			}
			catch (OperationCanceledException)
			{
			}
		}

		private void OnDestroy()
		{
			_acceptHoldHandle.Get(_eventRegistry)?.Unsubscribe(OnAcceptHold);
			StopHold();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			NotifyReleased();
			OnPointerUpAction?.Invoke();
		}

		public void SetValueWithoutNotify(float value)
		{
			CurrentSlider.SetValueWithoutNotify(value);
		}

		public void SetValue(float value)
		{
			CurrentSlider.value = value;
		}

		public float GetValue()
		{
			return CurrentSlider.value;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			OnPointerDownAction?.Invoke();
		}

		public void RemoveAllListeners()
		{
			CurrentSlider.onValueChanged.RemoveAllListeners();
		}

		public void AddListener(UnityAction<float> action)
		{
			CurrentSlider.onValueChanged.AddListener(action);
		}

		public void AddListenerClamped(Action<float> action)
		{
			CurrentSlider.onValueChanged.AddListener((float value) =>
			{
				action?.Invoke(value * MaxValue);
			});
		}
	}
}
