using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class SliderStepRepeater : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public const float HoldDelay = 0.35f;

		public const float StartStepsPerSecond = 4f;

		public const float RateGrowthPerSecond = 3f;

		public const float MinSecondsForFullRange = 2.5f;

		private Button _button;

		private int _direction;

		private Func<float> _rangeInSteps;

		private Action<int> _onSteps;

		private Action _onHoldReleased;

		private bool _pressed;

		private bool _repeated;

		private int _suppressClickUntilFrame = -1;

		private float _heldTime;

		private float _accumulatedSteps;

		private CancellationTokenSource _holdCts;

		public void Setup(int direction, Button button, Func<float> rangeInSteps, Action<int> onSteps, Action onHoldReleased)
		{
			_direction = direction;
			_button = button;
			_rangeInSteps = rangeInSteps;
			_onSteps = onSteps;
			_onHoldReleased = onHoldReleased;
		}

		public bool ConsumeClickSuppression()
		{
			return ConsumeClickSuppression(Time.frameCount);
		}

		public bool ConsumeClickSuppression(int frame)
		{
			bool result = frame <= _suppressClickUntilFrame;
			_suppressClickUntilFrame = -1;
			return result;
		}

		public static float StepsPerSecond(float heldSeconds, float rangeInSteps)
		{
			if (heldSeconds < 0.35f)
			{
				return 0f;
			}
			float p = heldSeconds - 0.35f;
			float a = 4f * Mathf.Pow(3f, p);
			float b = Mathf.Max(4f, rangeInSteps / 2.5f);
			return Mathf.Min(a, b);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			EndHold();
			_pressed = true;
			_repeated = false;
			_heldTime = 0f;
			_accumulatedSteps = 1f;
			_holdCts = CancellationTokenSource.CreateLinkedTokenSource(base.destroyCancellationToken);
			RunHold(_holdCts.Token).Forget();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			EndHold();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (_pressed)
			{
				EndHold();
			}
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
		}

		public void OnDrag(PointerEventData eventData)
		{
		}

		public void OnEndDrag(PointerEventData eventData)
		{
		}

		public void Tick(float deltaTime)
		{
			if (!_pressed)
			{
				return;
			}
			if (_button != null && !_button.IsInteractable())
			{
				EndHold();
				return;
			}
			_heldTime += deltaTime;
			if (!(_heldTime < 0.35f))
			{
				float rangeInSteps = _rangeInSteps?.Invoke() ?? 0f;
				_accumulatedSteps += StepsPerSecond(_heldTime, rangeInSteps) * deltaTime;
				int num = Mathf.FloorToInt(_accumulatedSteps);
				if (num > 0)
				{
					_accumulatedSteps -= num;
					_repeated = true;
					_onSteps?.Invoke(_direction * num);
				}
			}
		}

		private void EndHold()
		{
			bool num = _pressed && _repeated;
			_pressed = false;
			_holdCts?.Cancel();
			_holdCts?.Dispose();
			_holdCts = null;
			if (num)
			{
				_suppressClickUntilFrame = Time.frameCount + 1;
				_onHoldReleased?.Invoke();
			}
		}

		private async UniTaskVoid RunHold(CancellationToken token)
		{
			while (_pressed && !(await UniTask.Yield(PlayerLoopTiming.Update, token).SuppressCancellationThrow()))
			{
				Tick(Time.unscaledDeltaTime);
			}
		}

		private void OnDisable()
		{
			EndHold();
			_repeated = false;
			_suppressClickUntilFrame = -1;
		}
	}
}
