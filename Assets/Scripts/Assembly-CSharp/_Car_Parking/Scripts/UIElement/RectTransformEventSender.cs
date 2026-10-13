using System;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement
{
	public class RectTransformEventSender : MonoBehaviour
	{
		[SerializeField]
		private RectTransform RectTransform;

		private Vector2 _lastAnchoredPosition;

		private Vector2 _lastSize;

		public Vector2 Size => _lastSize;

		public Vector2 AnchoredPosition
		{
			get
			{
				return _lastAnchoredPosition;
			}
			set
			{
				if ((bool)RectTransform)
				{
					RectTransform.anchoredPosition = value;
				}
			}
		}

		public event Action<Vector2> OnSizeChanged;

		public event Action<Vector2> OnAnchoredPositionChanged;

		private void OnEnable()
		{
			if (!RectTransform)
			{
				RectTransform = GetComponent<RectTransform>();
			}
			Update();
		}

		private void Update()
		{
			if ((bool)RectTransform)
			{
				if (!Mathf.Approximately(_lastAnchoredPosition.x, RectTransform.anchoredPosition.x) || !Mathf.Approximately(_lastAnchoredPosition.y, RectTransform.anchoredPosition.y))
				{
					_lastAnchoredPosition = RectTransform.anchoredPosition;
					OnAnchoredPositionChanged?.Invoke(RectTransform.anchoredPosition);
				}
				if (!Mathf.Approximately(_lastSize.x, RectTransform.rect.size.x) || !Mathf.Approximately(_lastSize.y, RectTransform.rect.size.y))
				{
					_lastSize = RectTransform.rect.size;
					OnSizeChanged?.Invoke(RectTransform.rect.size);
				}
			}
		}
	}
}
