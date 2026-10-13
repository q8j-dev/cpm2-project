using UnityEngine;
using UnityEngine.EventSystems;

public class RacingTouchInput : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IDragHandler, IInitializePotentialDragHandler
{
	private int _pointerId = int.MinValue;

	private int _secondPointerId = int.MinValue;

	private Vector2 _currentPos;

	private Vector2 _lastPos;

	private Vector2 _prevPos;

	private Vector2 _secondCurrentPos;

	private Vector2 _secondPrevPos;

	public Vector2 Delta { get; private set; }

	public bool IsActive { get; private set; }

	public float ZoomDelta { get; private set; }

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!IsOverInteractiveElement(eventData))
		{
			if (!IsActive)
			{
				_pointerId = eventData.pointerId;
				_currentPos = eventData.position;
				_lastPos = eventData.position;
				_prevPos = eventData.position;
				IsActive = true;
				Delta = Vector2.zero;
			}
			else if (_secondPointerId == int.MinValue)
			{
				_secondPointerId = eventData.pointerId;
				_secondCurrentPos = eventData.position;
				_secondPrevPos = eventData.position;
				Delta = Vector2.zero;
			}
		}
	}

	public void OnInitializePotentialDrag(PointerEventData eventData)
	{
		if (eventData.pointerId != _pointerId && eventData.pointerId != _secondPointerId)
		{
			eventData.pointerDrag = eventData.pointerPress;
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (eventData.pointerId == _pointerId)
		{
			_currentPos = eventData.position;
		}
		else if (eventData.pointerId == _secondPointerId)
		{
			_secondCurrentPos = eventData.position;
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (eventData.pointerId == _secondPointerId)
		{
			_secondPointerId = int.MinValue;
			ZoomDelta = 0f;
		}
		else if (eventData.pointerId == _pointerId)
		{
			if (_secondPointerId != int.MinValue)
			{
				_pointerId = _secondPointerId;
				_currentPos = _secondCurrentPos;
				_lastPos = _secondCurrentPos;
				_prevPos = _secondCurrentPos;
				_secondPointerId = int.MinValue;
			}
			else
			{
				_pointerId = int.MinValue;
				IsActive = false;
			}
			Delta = Vector2.zero;
			ZoomDelta = 0f;
		}
	}

	private bool IsOverInteractiveElement(PointerEventData eventData)
	{
		foreach (GameObject item in eventData.hovered)
		{
			if (item == base.gameObject)
			{
				continue;
			}
			Transform parent = item.transform;
			while (parent != null && parent.gameObject != base.gameObject)
			{
				if (parent.TryGetComponent<IPointerDownHandler>(out var component) && !(component is RacingTouchInput))
				{
					return true;
				}
				if (parent.TryGetComponent<IPointerClickHandler>(out var _))
				{
					return true;
				}
				parent = parent.parent;
			}
		}
		return false;
	}

	public void SetInputEnabled(bool value)
	{
		base.enabled = value;
		_pointerId = int.MinValue;
		_secondPointerId = int.MinValue;
		IsActive = false;
		Delta = Vector2.zero;
		ZoomDelta = 0f;
	}

	private void Update()
	{
		if (!IsActive)
		{
			Delta = Vector2.zero;
			ZoomDelta = 0f;
			return;
		}
		if (_secondPointerId != int.MinValue)
		{
			float magnitude = (_prevPos - _secondPrevPos).magnitude;
			float magnitude2 = (_currentPos - _secondCurrentPos).magnitude;
			ZoomDelta = magnitude - magnitude2;
			Delta = Vector2.zero;
		}
		else
		{
			Delta = _currentPos - _lastPos;
			ZoomDelta = 0f;
		}
		_lastPos = _currentPos;
		_prevPos = _currentPos;
		_secondPrevPos = _secondCurrentPos;
	}
}
