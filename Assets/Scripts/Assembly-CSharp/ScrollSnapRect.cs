using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Mask))]
[RequireComponent(typeof(ScrollRect))]
[RequireComponent(typeof(Image))]
public class ScrollSnapRect : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IEndDragHandler, IDragHandler
{
	[Tooltip("Set starting page index - starting from 0")]
	public int startingPage;

	[Tooltip("Threshold time for fast swipe in seconds")]
	public float fastSwipeThresholdTime = 0.3f;

	[Tooltip("Threshold time for fast swipe in (unscaled) pixels")]
	public int fastSwipeThresholdDistance = 100;

	[Tooltip("How fast will page lerp to target position")]
	public float decelerationRate = 10f;

	[Tooltip("Button to go to the previous page (optional)")]
	public GameObject prevButton;

	[Tooltip("Button to go to the next page (optional)")]
	public GameObject nextButton;

	[Tooltip("Sprite for unselected page (optional)")]
	public Sprite unselectedPage;

	[Tooltip("Sprite for selected page (optional)")]
	public Sprite selectedPage;

	[Tooltip("Container with page images (optional)")]
	public Transform pageSelectionIcons;

	private int _fastSwipeThresholdMaxLimit;

	private ScrollRect _scrollRectComponent;

	private RectTransform _scrollRectRect;

	private RectTransform _container;

	private bool _horizontal;

	private int _pageCount;

	private int _currentPage;

	private bool _lerp;

	private Vector2 _lerpTo;

	private List<Vector2> _pagePositions = new List<Vector2>();

	private bool _dragging;

	private float _timeStamp;

	private Vector2 _startPosition;

	private bool _showPageSelection;

	private int _previousPageSelectionIndex;

	private List<Image> _pageSelectionImages;

	private void Start()
	{
		_scrollRectComponent = GetComponent<ScrollRect>();
		_scrollRectRect = GetComponent<RectTransform>();
		_container = _scrollRectComponent.content;
		_pageCount = _container.childCount;
		if (_scrollRectComponent.horizontal && !_scrollRectComponent.vertical)
		{
			_horizontal = true;
		}
		else if (!_scrollRectComponent.horizontal && _scrollRectComponent.vertical)
		{
			_horizontal = false;
		}
		else
		{
			Debug.LogWarning("Confusing setting of horizontal/vertical direction. Default set to horizontal.");
			_horizontal = true;
		}
		_lerp = false;
		SetPagePositions();
		SetPage(startingPage);
		InitPageSelection();
		SetPageSelection(startingPage);
		if ((bool)nextButton)
		{
			nextButton.GetComponent<Button>().onClick.AddListener(() =>
			{
				NextScreen();
			});
		}
		if ((bool)prevButton)
		{
			prevButton.GetComponent<Button>().onClick.AddListener(() =>
			{
				PreviousScreen();
			});
		}
	}

	private void Update()
	{
		if (_lerp)
		{
			float t = Mathf.Min(decelerationRate * Time.deltaTime, 1f);
			_container.anchoredPosition = Vector2.Lerp(_container.anchoredPosition, _lerpTo, t);
			if (Vector2.SqrMagnitude(_container.anchoredPosition - _lerpTo) < 0.25f)
			{
				_container.anchoredPosition = _lerpTo;
				_lerp = false;
				_scrollRectComponent.velocity = Vector2.zero;
			}
			if (_showPageSelection)
			{
				SetPageSelection(GetNearestPage());
			}
		}
	}

	private void SetPagePositions()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		if (_horizontal)
		{
			num = (int)_scrollRectRect.rect.width;
			num3 = num / 2;
			num5 = num * _pageCount;
			_fastSwipeThresholdMaxLimit = num;
		}
		else
		{
			num2 = (int)_scrollRectRect.rect.height;
			num4 = num2 / 2;
			num6 = num2 * _pageCount;
			_fastSwipeThresholdMaxLimit = num2;
		}
		Vector2 sizeDelta = new Vector2(num5, num6);
		_container.sizeDelta = sizeDelta;
		Vector2 anchoredPosition = new Vector2(num5 / 2, num6 / 2);
		_container.anchoredPosition = anchoredPosition;
		_pagePositions.Clear();
		for (int i = 0; i < _pageCount; i++)
		{
			Vector2 vector = (_container.GetChild(i).GetComponent<RectTransform>().anchoredPosition = ((!_horizontal) ? new Vector2(0f, -(i * num2 - num6 / 2 + num4)) : new Vector2(i * num - num5 / 2 + num3, 0f)));
			_pagePositions.Add(-vector);
		}
	}

	private void SetPage(int aPageIndex)
	{
		aPageIndex = Mathf.Clamp(aPageIndex, 0, _pageCount - 1);
		_container.anchoredPosition = _pagePositions[aPageIndex];
		_currentPage = aPageIndex;
	}

	private void LerpToPage(int aPageIndex)
	{
		aPageIndex = Mathf.Clamp(aPageIndex, 0, _pageCount - 1);
		_lerpTo = _pagePositions[aPageIndex];
		_lerp = true;
		_currentPage = aPageIndex;
	}

	private void InitPageSelection()
	{
		_showPageSelection = unselectedPage != null && selectedPage != null;
		if (!_showPageSelection)
		{
			return;
		}
		if (pageSelectionIcons == null || pageSelectionIcons.childCount != _pageCount)
		{
			Debug.LogWarning("Different count of pages and selection icons - will not show page selection");
			_showPageSelection = false;
			return;
		}
		_previousPageSelectionIndex = -1;
		_pageSelectionImages = new List<Image>();
		for (int i = 0; i < pageSelectionIcons.childCount; i++)
		{
			Image component = pageSelectionIcons.GetChild(i).GetComponent<Image>();
			if (component == null)
			{
				Debug.LogWarning("Page selection icon at position " + i + " is missing Image component");
			}
			_pageSelectionImages.Add(component);
		}
	}

	private void SetPageSelection(int aPageIndex)
	{
		if (_previousPageSelectionIndex != aPageIndex)
		{
			if (_previousPageSelectionIndex >= 0)
			{
				_pageSelectionImages[_previousPageSelectionIndex].sprite = unselectedPage;
				_pageSelectionImages[_previousPageSelectionIndex].SetNativeSize();
			}
			_pageSelectionImages[aPageIndex].sprite = selectedPage;
			_pageSelectionImages[aPageIndex].SetNativeSize();
			_previousPageSelectionIndex = aPageIndex;
		}
	}

	private void NextScreen()
	{
		LerpToPage(_currentPage + 1);
	}

	private void PreviousScreen()
	{
		LerpToPage(_currentPage - 1);
	}

	private int GetNearestPage()
	{
		Vector2 anchoredPosition = _container.anchoredPosition;
		float num = float.MaxValue;
		int result = _currentPage;
		for (int i = 0; i < _pagePositions.Count; i++)
		{
			float num2 = Vector2.SqrMagnitude(anchoredPosition - _pagePositions[i]);
			if (num2 < num)
			{
				num = num2;
				result = i;
			}
		}
		return result;
	}

	public void OnBeginDrag(PointerEventData aEventData)
	{
		_lerp = false;
		_dragging = false;
	}

	public void OnEndDrag(PointerEventData aEventData)
	{
		float num = ((!_horizontal) ? (0f - (_startPosition.y - _container.anchoredPosition.y)) : (_startPosition.x - _container.anchoredPosition.x));
		if (Time.unscaledTime - _timeStamp < fastSwipeThresholdTime && Mathf.Abs(num) > (float)fastSwipeThresholdDistance && Mathf.Abs(num) < (float)_fastSwipeThresholdMaxLimit)
		{
			if (num > 0f)
			{
				NextScreen();
			}
			else
			{
				PreviousScreen();
			}
		}
		else
		{
			LerpToPage(GetNearestPage());
		}
		_dragging = false;
	}

	public void OnDrag(PointerEventData aEventData)
	{
		if (!_dragging)
		{
			_dragging = true;
			_timeStamp = Time.unscaledTime;
			_startPosition = _container.anchoredPosition;
		}
		else if (_showPageSelection)
		{
			SetPageSelection(GetNearestPage());
		}
	}
}
