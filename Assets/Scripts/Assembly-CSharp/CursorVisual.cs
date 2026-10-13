using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using _Car_Parking.Scripts.Other.Helpers;

public class CursorVisual
{
	private const bool _debug = false;

	private const bool _debugNav = false;

	private readonly Canvas _cursorTempCanvas;

	private readonly GameObject _cursorImagePrefab;

	private readonly Vector2 _defaultSize;

	private readonly float _paddingPercent;

	private readonly Action<bool> _setForceShow;

	private RectTransform _cursorHighlight;

	private Image _cursorImage;

	private CanvasGroup _cursorCanvasGroup;

	private bool _isCursorVisible;

	private bool _isCursorSetupGoing;

	private bool _isCursorSetupComplete;

	private RectTransform _targetRect;

	private Canvas _currentTargetCanvas;

	private readonly Vector3[] _targetCorners = new Vector3[4];

	private Vector2 _expectedMousePos;

	public RectTransform CursorHighlight => _cursorHighlight;

	public Image CursorImage => _cursorImage;

	public bool IsSetupComplete => _isCursorSetupComplete;

	public Vector2 ExpectedMousePos
	{
		get
		{
			return _expectedMousePos;
		}
		set
		{
			_expectedMousePos = value;
		}
	}

	public Canvas CurrentTargetCanvas
	{
		get
		{
			return _currentTargetCanvas;
		}
		set
		{
			_currentTargetCanvas = value;
		}
	}

	public RectTransform TargetRect
	{
		get
		{
			return _targetRect;
		}
		set
		{
			_targetRect = value;
		}
	}

	public CursorVisual(Canvas cursorTempCanvas, GameObject cursorImagePrefab, Vector2 defaultSize, float paddingPercent, Action<bool> setForceShow)
	{
		_cursorTempCanvas = cursorTempCanvas;
		_cursorImagePrefab = cursorImagePrefab;
		_defaultSize = defaultSize;
		_paddingPercent = paddingPercent;
		_setForceShow = setForceShow;
	}

	public async UniTask<bool> SetupAsync()
	{
		if (_isCursorSetupComplete || _isCursorSetupGoing)
		{
			return false;
		}
		_isCursorSetupGoing = true;
		if ((bool)_cursorHighlight)
		{
			UnityEngine.Object.Destroy(_cursorHighlight.gameObject);
		}
		if (!_cursorImagePrefab)
		{
			return false;
		}
		GameObject newCursor = UnityEngine.Object.Instantiate(_cursorImagePrefab, _cursorTempCanvas.transform);
		await UniTask.DelayFrame(1);
		if (!newCursor)
		{
			return false;
		}
		_cursorImage = newCursor.GetComponent<Image>();
		_cursorImage.raycastTarget = false;
		_cursorHighlight = newCursor.GetOrAddComponent<RectTransform>();
		if (_cursorHighlight.TryGetComponent<CanvasGroup>(out _cursorCanvasGroup))
		{
			_cursorCanvasGroup.alpha = 0f;
		}
		UiInputSetup.DisabledWindows.Clear();
		newCursor.SetActive(value: true);
		_isCursorVisible = false;
		_isCursorSetupComplete = true;
		_isCursorSetupGoing = false;
		return true;
	}

	public void ResetForSceneLoad()
	{
		_currentTargetCanvas = null;
		if ((bool)_cursorHighlight)
		{
			UnityEngine.Object.Destroy(_cursorHighlight.gameObject);
		}
		_cursorHighlight = null;
		_isCursorSetupComplete = false;
	}

	public bool Hide()
	{
		if (!_isCursorVisible)
		{
			return false;
		}
		if ((bool)_cursorCanvasGroup)
		{
			_cursorCanvasGroup.alpha = 0f;
		}
		_isCursorVisible = false;
		_setForceShow(obj: false);
		return true;
	}

	public void Show(bool force = false)
	{
		if (!_isCursorVisible)
		{
			if ((bool)_cursorCanvasGroup)
			{
				_cursorCanvasGroup.alpha = 1f;
			}
			_isCursorVisible = true;
			if (force)
			{
				_setForceShow(obj: true);
			}
		}
	}

	public void ResetHighlight()
	{
		if ((bool)_cursorHighlight)
		{
			if (_cursorHighlight.parent != _cursorTempCanvas.transform)
			{
				_cursorHighlight.SetParent(_cursorTempCanvas.transform);
			}
			_cursorHighlight.localScale = Vector3.one;
			_cursorHighlight.sizeDelta = _defaultSize;
			_cursorHighlight.localRotation = Quaternion.identity;
		}
	}

	public void UpdateCursorTransform()
	{
		if ((bool)_targetRect)
		{
			_targetRect.GetWorldCorners(_targetCorners);
			Camera cam = null;
			if ((bool)_currentTargetCanvas && _currentTargetCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
			{
				cam = _currentTargetCanvas.worldCamera;
			}
			RectTransform rectTransform = _cursorTempCanvas.transform as RectTransform;
			Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, 0f);
			Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, 0f);
			for (int i = 0; i < 4; i++)
			{
				Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, _targetCorners[i]);
				screenPoint.x = Mathf.Clamp(screenPoint.x, 0f, Screen.width);
				screenPoint.y = Mathf.Clamp(screenPoint.y, 0f, Screen.height);
				RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, null, out var localPoint);
				vector = Vector3.Min(vector, new Vector3(localPoint.x, localPoint.y, 0f));
				vector2 = Vector3.Max(vector2, new Vector3(localPoint.x, localPoint.y, 0f));
			}
			float num = Mathf.Abs(vector2.x - vector.x);
			float num2 = Mathf.Abs(vector2.y - vector.y);
			Vector3 localPosition = (vector + vector2) * 0.5f;
			num *= _paddingPercent;
			num2 *= _paddingPercent;
			_cursorHighlight.sizeDelta = new Vector2(num, num2);
			_cursorHighlight.localPosition = localPosition;
			_cursorHighlight.localRotation = Quaternion.identity;
			Vector3 worldPoint = _targetRect.TransformPoint(_targetRect.rect.center);
			Camera cam2 = (_currentTargetCanvas ? _currentTargetCanvas.worldCamera : null);
			_expectedMousePos = RectTransformUtility.WorldToScreenPoint(cam2, worldPoint);
			ClampCursorToScreen(rectTransform);
		}
	}

	private void ClampCursorToScreen(RectTransform overlayRect)
	{
		Vector3[] array = new Vector3[4];
		overlayRect.GetLocalCorners(array);
		float x = array[0].x;
		float y = array[0].y;
		float x2 = array[2].x;
		float y2 = array[2].y;
		Vector2 sizeDelta = _cursorHighlight.sizeDelta;
		Vector3 localPosition = _cursorHighlight.localPosition;
		float num = sizeDelta.x * 0.5f;
		float num2 = sizeDelta.y * 0.5f;
		if (localPosition.x - num < x)
		{
			localPosition.x = x + num;
		}
		if (localPosition.x + num > x2)
		{
			localPosition.x = x2 - num;
		}
		if (localPosition.y - num2 < y)
		{
			localPosition.y = y + num2;
		}
		if (localPosition.y + num2 > y2)
		{
			localPosition.y = y2 - num2;
		}
		_cursorHighlight.localPosition = localPosition;
	}
}
