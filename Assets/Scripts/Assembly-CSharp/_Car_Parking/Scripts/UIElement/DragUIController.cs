using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Car_Parking.Scripts.UIElement
{
	public class DragUIController : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
	{
		private const float CloseDistanceSquare = 0.0001f;

		[SerializeField]
		private float DragLerpPower = 12f;

		public Action<DragUIController> OnBeginDragAction;

		public Action<DragUIController> OnDragAction;

		public Action<DragUIController> OnEndDragAction;

		private Vector2 _offsetDrag;

		private bool _isDragging;

		private bool _isTargetClose = true;

		private Vector3 _dragTarget;

		private void LateUpdate()
		{
			if (!_isTargetClose)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, _dragTarget, DragLerpPower * Time.unscaledDeltaTime);
				if (!_isDragging && Extensions.DistanceSquare(base.transform.position, _dragTarget) < 0.0001f)
				{
					_isTargetClose = true;
					base.transform.position = _dragTarget;
					OnEndDragAction?.Invoke(this);
				}
			}
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			_isDragging = true;
			_isTargetClose = false;
			_offsetDrag = new Vector2(base.transform.position.x, base.transform.position.y) - eventData.position;
			OnBeginDragAction?.Invoke(this);
		}

		public void OnDrag(PointerEventData eventData)
		{
			_dragTarget = eventData.position + _offsetDrag;
			OnDragAction?.Invoke(this);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			OnEndDragAction?.Invoke(this);
			_isDragging = false;
		}
	}
}
