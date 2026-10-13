using DanielLochner.Assets.SimpleScrollSnap;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.UIElement;

public class ScrollCenteringService
{
	private const bool _debug = false;

	private ScrollRect _currentScrollRect;

	private SimpleScrollSnap _currentSimpleScrollSnap;

	private RectTransform _currentViewPort;

	public void Clear()
	{
		_currentScrollRect = null;
		_currentSimpleScrollSnap = null;
	}

	public void ApplyScroll(GameObject selectedObj)
	{
		if (!selectedObj)
		{
			return;
		}
		_currentScrollRect = selectedObj.GetComponentInParent<ScrollRect>(includeInactive: true);
		if ((bool)_currentScrollRect)
		{
			_currentViewPort = (_currentScrollRect.viewport ? _currentScrollRect.viewport : _currentScrollRect.GetComponent<RectTransform>());
		}
		_currentSimpleScrollSnap = null;
		if ((bool)_currentScrollRect && ((bool)_currentScrollRect.GetComponent<GamepadScrollCenter>() || IsInsideDropdownList(_currentScrollRect)))
		{
			if (_currentScrollRect.TryGetComponent<SimpleScrollSnap>(out _currentSimpleScrollSnap))
			{
				CenterViaSimpleScrollSnap(selectedObj.transform);
			}
			else
			{
				CenterViaPlainScrollRect(selectedObj.GetComponent<RectTransform>());
			}
		}
	}

	private static bool IsInsideDropdownList(ScrollRect scrollRect)
	{
		if (!scrollRect.GetComponentInParent<Dropdown>())
		{
			return scrollRect.GetComponentInParent<TMP_Dropdown>();
		}
		return true;
	}

	private void CenterViaSimpleScrollSnap(Transform selectedTransform)
	{
		if (_currentSimpleScrollSnap.Panels == null || _currentSimpleScrollSnap.NumberOfPanels == 0)
		{
			return;
		}
		Transform transform = selectedTransform;
		while ((bool)transform && transform.parent != _currentSimpleScrollSnap.Content)
		{
			transform = transform.parent;
		}
		if (!transform)
		{
			return;
		}
		for (int i = 0; i < _currentSimpleScrollSnap.Panels.Length; i++)
		{
			if ((bool)_currentSimpleScrollSnap.Panels[i] && _currentSimpleScrollSnap.Panels[i].transform == transform)
			{
				_currentSimpleScrollSnap.GoToPanel(i);
				break;
			}
		}
	}

	private void CenterViaPlainScrollRect(RectTransform targetItem)
	{
		if (!_currentScrollRect || !targetItem || !_currentScrollRect.content)
		{
			return;
		}
		RectTransform content = _currentScrollRect.content;
		RectTransform currentViewPort = _currentViewPort;
		Vector3 position = targetItem.TransformPoint(targetItem.rect.center);
		Vector3 vector = content.InverseTransformPoint(position);
		Rect rect = content.rect;
		Rect rect2 = currentViewPort.rect;
		Vector2 normalizedPosition = _currentScrollRect.normalizedPosition;
		if (_currentScrollRect.vertical)
		{
			float num = rect.height - rect2.height;
			if (num > 0f)
			{
				float num2 = 0f - vector.y + rect.yMax - rect2.height * 0.5f;
				normalizedPosition.y = 1f - Mathf.Clamp01(num2 / num);
			}
		}
		if (_currentScrollRect.horizontal)
		{
			float num3 = rect.width - rect2.width;
			if (num3 > 0f)
			{
				float num4 = vector.x - rect.xMin - rect2.width * 0.5f;
				normalizedPosition.x = Mathf.Clamp01(num4 / num3);
			}
		}
		_currentScrollRect.normalizedPosition = normalizedPosition;
	}
}
