using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.PhotoMode.UI
{
	public class TouchDetector : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		private const float TouchRadiusSq = 250f;

		[SerializeField]
		private Button DetectButton;

		[SerializeField]
		private float DistanceThreshold = 0.01f;

		private Vector2 _downPosition;

		private int _touchCount;

		public int TouchId { get; private set; }

		public bool IsPressed { get; private set; }

		public bool IsZoom => _touchCount > 1;

		public event Action<Vector2> OnFocusPoint;

		public event Action<bool> OnZoomStateChanged;

		public void OnPointerDown(PointerEventData eventData)
		{
			if (eventData != null)
			{
				IsPressed = true;
				_touchCount++;
				_downPosition = eventData.position;
				if (_touchCount == 1)
				{
					TouchId = FindMatchingTouchId(eventData.position);
				}
				else if (_touchCount == 2)
				{
					OnZoomStateChanged?.Invoke(obj: true);
				}
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			IsPressed = false;
			_touchCount--;
			if (_touchCount == 0)
			{
				TouchId = -1;
				OnZoomStateChanged?.Invoke(obj: false);
			}
			if (_touchCount < 0)
			{
				_touchCount = 0;
			}
		}

		public void ResetTouchCount()
		{
			_touchCount = 0;
		}

		private void Start()
		{
			DetectButton.onClick.AddListener(GetPoint);
		}

		private void GetPoint()
		{
			Vector2 zero = Vector2.zero;
			zero = Input.mousePosition;
			if (!(Extensions.DistanceSquare(zero, _downPosition) > DistanceThreshold))
			{
				OnFocusPoint?.Invoke(zero);
			}
		}

		private int FindMatchingTouchId(Vector2 screenPosition)
		{
			Touch[] touches = Input.touches;
			for (int i = 0; i < touches.Length; i++)
			{
				Touch touch = touches[i];
				if ((touch.position - screenPosition).sqrMagnitude < 250f)
				{
					return touch.fingerId;
				}
			}
			return -1;
		}
	}
}
