using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class ButtonHoldDetector : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		[SerializeField]
		private Button TargetButton;

		[SerializeField]
		private float HoldDelay = 1f;

		private bool _isPressed;

		private IDisposable _delayedDisposable;

		private void OnValidate()
		{
			TargetButton = GetComponent<Button>();
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			_isPressed = true;
			_delayedDisposable = Observable.Timer(TimeSpan.FromSeconds(HoldDelay)).Subscribe((long _) =>
			{
				DelayedInvoke().Forget();
			}).AddTo(this);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			_isPressed = false;
			_delayedDisposable?.Dispose();
		}

		private async UniTask DelayedInvoke()
		{
			while (_isPressed)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612));
				TargetButton.onClick.Invoke();
			}
		}

		private void OnDisable()
		{
			_isPressed = false;
			_delayedDisposable?.Dispose();
		}
	}
}
