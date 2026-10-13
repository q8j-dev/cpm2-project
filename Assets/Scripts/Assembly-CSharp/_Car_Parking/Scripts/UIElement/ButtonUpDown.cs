using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class ButtonUpDown : Button, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerClickHandler
	{
		private const double DelayEnableButton = 0.5;

		[SerializeField]
		[FormerlySerializedAs("IsStartCkickEventsPointerUp")]
		private bool IsStartClickEventsPointerUp;

		[SerializeField]
		private CanvasGroup CanvasGroup;

		[SerializeField]
		private bool UseVibration;

		private Action _buttonAction;

		private Action _pointerUpAction;

		private Action _pointerDownAction;

		private Action _holdAction;

		private Action _clickAction;

		public float longPressThreshold = 0.3f;

		public bool IsVibrated;

		private IDisposable _startTimerDisposable;

		private bool _isHold;

		private bool _enableButton;

		private bool _isLongPress;

		public float PressTime { get; private set; }

		public new void OnPointerDown(PointerEventData eventData)
		{
			_isHold = true;
			_isLongPress = false;
			if (_enableButton)
			{
				_startTimerDisposable?.Dispose();
				PressTime = 0.1f;
				_startTimerDisposable = StartPressTimer();
				ExecuteActionsOnPointerDown();
				if (!IsStartClickEventsPointerUp)
				{
					ExecuteActionsClick();
				}
			}
		}

		public new void OnPointerUp(PointerEventData eventData)
		{
			_isHold = false;
			IsVibrated = false;
			if (_enableButton)
			{
				if (PressTime >= longPressThreshold)
				{
					_isLongPress = true;
				}
				ExecuteActionsOnPointerUp();
				if (IsStartClickEventsPointerUp)
				{
					ExecuteActionsClick();
				}
				_startTimerDisposable?.Dispose();
				PressTime = 0f;
			}
		}

		public void SetInteractable(bool interactable)
		{
			if ((bool)CanvasGroup)
			{
				CanvasGroup.alpha = (interactable ? 1f : 0.2f);
				CanvasGroup.interactable = interactable;
				CanvasGroup.blocksRaycasts = interactable;
			}
		}

		public void AddClickListener(Action clickAction)
		{
			_clickAction = clickAction;
		}

		public void AddListenerOnPointerUp(Action action)
		{
			_pointerUpAction = (Action)Delegate.Combine(_pointerUpAction, action);
		}

		public void AddHoldButtonListener(Action action)
		{
			_holdAction = action;
		}

		public void AddListenerOnPointerDown(Action action)
		{
			_pointerDownAction = (Action)Delegate.Combine(_pointerDownAction, action);
		}

		public void AddListener(Action action)
		{
			_buttonAction = (Action)Delegate.Combine(_buttonAction, action);
		}

		public void RemoveListenerHold()
		{
			_holdAction = null;
		}

		public void RemoveClickListener()
		{
			_clickAction = null;
		}

		public void RemoveListenerOnPointerUp(Action action)
		{
			_pointerUpAction = (Action)Delegate.Remove(_pointerUpAction, action);
		}

		public void RemoveListenerOnPointerDown(Action action)
		{
			_pointerDownAction = (Action)Delegate.Remove(_pointerDownAction, action);
		}

		public void RemoveListener(Action action)
		{
			_buttonAction = (Action)Delegate.Remove(_buttonAction, action);
		}

		public void RemoveAllListeners()
		{
			_clickAction = null;
			_buttonAction = null;
			_pointerUpAction = null;
			_pointerDownAction = null;
			_holdAction = null;
		}

		private void ExecuteActionsClick()
		{
			_buttonAction?.Invoke();
		}

		private void ExecuteActionsOnPointerUp()
		{
			_pointerUpAction?.Invoke();
		}

		private void ExecuteActionsOnPointerDown()
		{
			_pointerDownAction?.Invoke();
		}

		private IDisposable StartPressTimer()
		{
			return Observable.EveryUpdate().Subscribe((long _) =>
			{
				PressTime += Time.deltaTime;
			});
		}

		private void Update()
		{
			if (_isHold)
			{
				_holdAction?.Invoke();
			}
		}

		private new void OnEnable()
		{
			Observable.Timer(TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
			{
				_enableButton = true;
			}).AddTo(this);
		}

		private new void OnDisable()
		{
			_enableButton = false;
			ExecuteActionsOnPointerUp();
			if (IsStartClickEventsPointerUp)
			{
				ExecuteActionsClick();
			}
			_startTimerDisposable?.Dispose();
			PressTime = 0f;
		}

		public new void OnPointerClick(PointerEventData eventData)
		{
			if (!_isLongPress)
			{
				_clickAction?.Invoke();
			}
		}

		public override void OnSubmit(BaseEventData eventData)
		{
			base.OnSubmit(eventData);
			if (_enableButton)
			{
				_clickAction?.Invoke();
			}
		}
	}
}
