using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UiScrollRectOcclusiion : MonoBehaviour
{
	public bool InitByUser;

	private ScrollRect _scrollRect;

	private ContentSizeFitter _contentSizeFitter;

	private VerticalLayoutGroup _verticalLayoutGroup;

	private HorizontalLayoutGroup _horizontalLayoutGroup;

	private GridLayoutGroup _gridLayoutGroup;

	private bool _isVertical;

	private bool _isHorizontal;

	private float _disableMarginX;

	private float _disableMarginY;

	private bool _hasDisabledGridComponents;

	private readonly List<RectTransform> _items = new List<RectTransform>();

	private readonly HashSet<RectTransform> _occlusionHiddenItems = new HashSet<RectTransform>();

	private bool _isDestroyed;

	private void Awake()
	{
		if (!InitByUser)
		{
			Init();
		}
	}

	private void OnDestroy()
	{
		_isDestroyed = true;
	}

	public async UniTaskVoid RebuildOcclusion()
	{
		_items.Clear();
		_occlusionHiddenItems.Clear();
		if (!_scrollRect)
		{
			_scrollRect = GetComponent<ScrollRect>();
		}
		RectTransform currentRect;
		if ((bool)_gridLayoutGroup)
		{
			currentRect = _gridLayoutGroup.GetComponent<RectTransform>();
			_gridLayoutGroup.enabled = true;
		}
		else if ((bool)_verticalLayoutGroup)
		{
			currentRect = _verticalLayoutGroup.GetComponent<RectTransform>();
			_verticalLayoutGroup.enabled = true;
		}
		else if ((bool)_horizontalLayoutGroup)
		{
			currentRect = _horizontalLayoutGroup.GetComponent<RectTransform>();
			_horizontalLayoutGroup.enabled = true;
		}
		else
		{
			if (!_scrollRect.content)
			{
				return;
			}
			currentRect = _scrollRect.content.GetComponent<RectTransform>();
		}
		if (!_scrollRect.content)
		{
			return;
		}
		if (!_contentSizeFitter)
		{
			_contentSizeFitter = _scrollRect.content.GetComponent<ContentSizeFitter>();
		}
		_contentSizeFitter.enabled = true;
		_scrollRect.onValueChanged.RemoveAllListeners();
		_hasDisabledGridComponents = false;
		for (int i = 0; i < _scrollRect.content.childCount; i++)
		{
			RectTransform component = _scrollRect.content.GetChild(i).GetComponent<RectTransform>();
			if ((bool)component)
			{
				component.gameObject.SetActive(value: true);
			}
		}
		await UniTask.Yield();
		if (!_isDestroyed)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(currentRect);
			await UniTask.Yield();
			Init();
		}
	}

	public void Init()
	{
		if (_isDestroyed)
		{
			return;
		}
		if (TryGetComponent<ScrollRect>(out var component))
		{
			_scrollRect = component;
			_scrollRect.onValueChanged.AddListener(OnScroll);
			_isHorizontal = _scrollRect.horizontal;
			_isVertical = _scrollRect.vertical;
			if (!_scrollRect.content)
			{
				return;
			}
			for (int i = 0; i < _scrollRect.content.childCount; i++)
			{
				RectTransform component2 = _scrollRect.content.GetChild(i).GetComponent<RectTransform>();
				if ((bool)component2)
				{
					_items.Add(component2);
				}
			}
			if (_scrollRect.content.TryGetComponent<VerticalLayoutGroup>(out var component3))
			{
				_verticalLayoutGroup = component3;
			}
			if (_scrollRect.content.TryGetComponent<HorizontalLayoutGroup>(out var component4))
			{
				_horizontalLayoutGroup = component4;
			}
			if (_scrollRect.content.TryGetComponent<GridLayoutGroup>(out var component5))
			{
				_gridLayoutGroup = component5;
			}
			if (_scrollRect.content.TryGetComponent<ContentSizeFitter>(out var component6))
			{
				_contentSizeFitter = component6;
			}
		}
		else
		{
			Debug.LogError("UI_ScrollRectOcclusion => No ScrollRect component found");
		}
	}

	private void DisableGridComponents()
	{
		if (_items.Count < 1 || !_scrollRect)
		{
			return;
		}
		RectTransform component = _scrollRect.GetComponent<RectTransform>();
		if (!component)
		{
			Debug.LogWarning("Rect transform was destroyed");
			return;
		}
		int num = _items.FindIndex((RectTransform r) => r);
		if (num == -1)
		{
			Debug.LogWarning("All items are destroyed");
			return;
		}
		if (_isVertical)
		{
			_disableMarginY = component.rect.height + _items[num].sizeDelta.y;
		}
		if (_isHorizontal)
		{
			_disableMarginX = component.rect.width + _items[num].sizeDelta.x * 2f;
		}
		if ((bool)_verticalLayoutGroup)
		{
			_verticalLayoutGroup.enabled = false;
		}
		if ((bool)_horizontalLayoutGroup)
		{
			_horizontalLayoutGroup.enabled = false;
		}
		if ((bool)_contentSizeFitter)
		{
			_contentSizeFitter.enabled = false;
		}
		if ((bool)_gridLayoutGroup)
		{
			_gridLayoutGroup.enabled = false;
		}
		_hasDisabledGridComponents = true;
	}

	private void SetOcclusionActive(RectTransform t, bool active)
	{
		if (active)
		{
			if (_occlusionHiddenItems.Remove(t))
			{
				t.gameObject.SetActive(value: true);
			}
		}
		else if (t.gameObject.activeSelf)
		{
			_occlusionHiddenItems.Add(t);
			t.gameObject.SetActive(value: false);
		}
	}

	public void OnScroll(Vector2 pos)
	{
		if (!_hasDisabledGridComponents)
		{
			DisableGridComponents();
		}
		foreach (RectTransform item in _items)
		{
			if (!item)
			{
				continue;
			}
			Vector3 position = item.TransformPoint(item.rect.min);
			Vector3 position2 = item.TransformPoint(item.rect.max);
			Vector3 vector = _scrollRect.transform.InverseTransformPoint(position);
			Vector3 vector2 = _scrollRect.transform.InverseTransformPoint(position2);
			bool flag = vector2.y < 0f - _disableMarginY || vector.y > _disableMarginY;
			bool flag2 = vector2.x < 0f - _disableMarginX || vector.x > _disableMarginX;
			if (_isVertical && _isHorizontal)
			{
				bool flag3 = flag | flag2;
				SetOcclusionActive(item, !flag3);
				continue;
			}
			if (_isVertical)
			{
				SetOcclusionActive(item, !flag);
			}
			if (_isHorizontal)
			{
				SetOcclusionActive(item, !flag2);
			}
		}
	}
}
