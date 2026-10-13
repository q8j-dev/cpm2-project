using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.UIElement;

public class KeysNavigationController
{
	private const bool _debug = false;

	private const bool _debugNav = false;

	private readonly InputMapSetEvent _inputMapSetEvent;

	private readonly UiNavigationValueEvent _uiNavigationValueEvent;

	private readonly CursorVisual _visual;

	private readonly ScrollCenteringService _scroll;

	private readonly Func<GameObject> _getDefaultTarget;

	private readonly Action<GameObject> _setDefaultTarget;

	private readonly Func<Transform, bool> _isDebugSheetChild;

	private GameObject _previousTarget;

	public Action OnSelectionChangedPulse;

	private TMP_InputField _navFocusedField;

	private string _navFieldBaseline;

	private bool _navFieldEngaged;

	private bool _navFieldJustFocused;

	private Vector2 _prevNavInput;

	private const float NavPressThreshold = 0.5f;

	private readonly HashSet<InputMapEnum> _includedMapsInKeysNavigation = new HashSet<InputMapEnum>
	{
		InputMapEnum.UI,
		InputMapEnum.PaintSelectNew,
		InputMapEnum.PaintSelectSet,
		InputMapEnum.MapUi
	};

	private int _occludedGraceFrames;

	private const int MaxOccludedGraceFrames = 3;

	private bool _disabledKeyNavigation;

	private GameObject _preferredFallbackSelectable;

	public GameObject PreviousTarget
	{
		get
		{
			return _previousTarget;
		}
		set
		{
			_previousTarget = value;
		}
	}

	public bool DisabledKeyNavigation
	{
		get
		{
			return _disabledKeyNavigation;
		}
		set
		{
			_disabledKeyNavigation = value;
		}
	}

	public GameObject PreferredFallbackSelectable
	{
		set
		{
			_preferredFallbackSelectable = value;
		}
	}

	public KeysNavigationController(InputMapSetEvent inputMapSetEvent, UiNavigationValueEvent uiNavigationValueEvent, CursorVisual visual, ScrollCenteringService scroll, Func<GameObject> getDefaultTarget, Action<GameObject> setDefaultTarget, Func<Transform, bool> isDebugSheetChild)
	{
		_inputMapSetEvent = inputMapSetEvent;
		_uiNavigationValueEvent = uiNavigationValueEvent;
		_visual = visual;
		_scroll = scroll;
		_getDefaultTarget = getDefaultTarget;
		_setDefaultTarget = setDefaultTarget;
		_isDebugSheetChild = isDebugSheetChild;
	}

	public void ResetForSceneLoad()
	{
		_previousTarget = null;
		_occludedGraceFrames = 0;
	}

	public void ResolveDefaultTargetIfNull()
	{
		if (!(_getDefaultTarget() != null))
		{
			Canvas canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
			GameObject gameObject = (UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None).FirstOrDefault((Canvas c) => UiInputSetup.DefaultCanvases.Contains(c.name)) ?? canvas)?.gameObject;
			_setDefaultTarget(gameObject);
			_ = (bool)gameObject;
		}
	}

	private GameObject GetNewSelectedObject(GameObject selectedObject = null)
	{
		if (!EventSystem.current)
		{
			return null;
		}
		bool flag = selectedObject;
		if (!selectedObject)
		{
			selectedObject = EventSystem.current.currentSelectedGameObject;
		}
		if (!selectedObject)
		{
			return null;
		}
		if (!_navFocusedField || !(selectedObject == _navFocusedField.gameObject) || !(selectedObject == _previousTarget))
		{
			_scroll.ApplyScroll(selectedObject);
		}
		if (!selectedObject.activeInHierarchy)
		{
			return null;
		}
		if (!selectedObject.TryGetComponent<Selectable>(out var component))
		{
			return null;
		}
		if (!IsSelectableInteractable(component))
		{
			return null;
		}
		if (!flag && component.navigation.mode == Navigation.Mode.None && !selectedObject.TryGetComponent<TMP_InputField>(out var _) && !selectedObject.TryGetComponent<VynilStickerUI>(out var _) && !selectedObject.TryGetComponent<StickerItem>(out var _))
		{
			return null;
		}
		if (_isDebugSheetChild(selectedObject.transform))
		{
			return null;
		}
		return selectedObject;
	}

	public bool IsSelectableInteractable(Selectable selectable)
	{
		if (!selectable || !selectable.interactable || !selectable.isActiveAndEnabled)
		{
			return false;
		}
		return IsInteractable(selectable.gameObject);
	}

	private bool IsInteractable(GameObject go)
	{
		CanvasGroup[] componentsInParent = go.GetComponentsInParent<CanvasGroup>();
		foreach (CanvasGroup canvasGroup in componentsInParent)
		{
			if (!canvasGroup.interactable)
			{
				return false;
			}
			if (canvasGroup.ignoreParentGroups)
			{
				return true;
			}
		}
		return true;
	}

	private bool IsKeyNavigationAvailable()
	{
		if (_visual.IsSetupComplete && !_disabledKeyNavigation && _inputMapSetEvent.TryGetCurrentMaps(out var maps) && maps.Any((InputMapEnum map) => _includedMapsInKeysNavigation.Contains(map)))
		{
			return _uiNavigationValueEvent.StoredValue == UiNavigationEnum.Keys;
		}
		return false;
	}

	public void NavigateWithKeys(GameObject selectedObject = null)
	{
		if (!IsKeyNavigationAvailable())
		{
			_visual.Hide();
			return;
		}
		selectedObject = GetNewSelectedObject(selectedObject);
		if ((bool)selectedObject)
		{
			_occludedGraceFrames = 0;
			if ((bool)_previousTarget && selectedObject == _previousTarget)
			{
				_visual.UpdateCursorTransform();
				_visual.Show();
				return;
			}
			OnSelectionChanged(selectedObject);
		}
		else
		{
			if (TryHoldOccludedSelection())
			{
				return;
			}
			_occludedGraceFrames = 0;
			_visual.TargetRect = null;
			_previousTarget = null;
			_scroll.Clear();
			EventSystem.current.SetSelectedGameObject(null);
		}
		_visual.UpdateCursorTransform();
		SelectOrHide(selectedObject).Forget();
	}

	private bool TryHoldOccludedSelection()
	{
		GameObject gameObject = (EventSystem.current ? EventSystem.current.currentSelectedGameObject : null);
		if (!gameObject || gameObject.activeInHierarchy || !gameObject.GetComponentInParent<ScrollRect>(includeInactive: true))
		{
			return false;
		}
		if (_occludedGraceFrames >= 3)
		{
			return false;
		}
		_occludedGraceFrames++;
		_visual.TargetRect = gameObject.GetComponent<RectTransform>();
		Canvas componentInParent = gameObject.GetComponentInParent<Canvas>(includeInactive: true);
		if ((bool)componentInParent && (bool)componentInParent.rootCanvas)
		{
			_visual.CurrentTargetCanvas = componentInParent.rootCanvas;
		}
		_visual.UpdateCursorTransform();
		_visual.Show();
		return true;
	}

	private void OnSelectionChanged(GameObject selectedObj)
	{
		_visual.TargetRect = selectedObj.GetComponent<RectTransform>();
		_previousTarget = selectedObj;
		OnSelectionChangedPulse?.Invoke();
		Canvas componentInParent = selectedObj.GetComponentInParent<Canvas>();
		if ((bool)componentInParent && (bool)componentInParent.rootCanvas)
		{
			_visual.CurrentTargetCanvas = componentInParent.rootCanvas;
			_visual.ResetHighlight();
			_visual.UpdateCursorTransform();
			if (Mouse.current != null && _uiNavigationValueEvent.StoredValue == UiNavigationEnum.Keys)
			{
				Vector2 expectedMousePos = _visual.ExpectedMousePos;
				expectedMousePos.x = Mathf.Clamp(expectedMousePos.x, 0f, Screen.width);
				expectedMousePos.y = Mathf.Clamp(expectedMousePos.y, 0f, Screen.height);
				Mouse.current.WarpCursorPosition(expectedMousePos);
			}
		}
	}

	public void UpdateNavFieldEngagement()
	{
		EventSystem current = EventSystem.current;
		GameObject gameObject = (current ? current.currentSelectedGameObject : null);
		TMP_InputField component = null;
		if ((bool)gameObject)
		{
			gameObject.TryGetComponent<TMP_InputField>(out component);
		}
		if (!component || !component.isFocused)
		{
			_navFocusedField = null;
			_navFieldEngaged = false;
		}
		else if (component != _navFocusedField)
		{
			_navFocusedField = component;
			_navFieldBaseline = component.text;
			_navFieldEngaged = false;
			_navFieldJustFocused = true;
		}
		else if (!_navFieldEngaged && !string.Equals(component.text, _navFieldBaseline))
		{
			_navFieldEngaged = true;
		}
	}

	public bool TryNavigateOffField()
	{
		if (!_navFocusedField || _navFieldEngaged)
		{
			_prevNavInput = Vector2.zero;
			return false;
		}
		InputSystemUIInputModule inputSystemUIInputModule = (EventSystem.current ? (EventSystem.current.currentInputModule as InputSystemUIInputModule) : null);
		Vector2 prevNavInput = (((bool)inputSystemUIInputModule && (bool)inputSystemUIInputModule.move && inputSystemUIInputModule.move.action != null) ? inputSystemUIInputModule.move.action.ReadValue<Vector2>() : Vector2.zero);
		if (_navFieldJustFocused)
		{
			_navFieldJustFocused = false;
			_prevNavInput = prevNavInput;
			return false;
		}
		bool num = _prevNavInput.sqrMagnitude > 0.25f;
		bool flag = prevNavInput.sqrMagnitude > 0.25f;
		_prevNavInput = prevNavInput;
		if (num || !flag)
		{
			return false;
		}
		Selectable selectable = ((!(Mathf.Abs(prevNavInput.x) > Mathf.Abs(prevNavInput.y))) ? ((prevNavInput.y > 0f) ? _navFocusedField.FindSelectableOnUp() : _navFocusedField.FindSelectableOnDown()) : ((prevNavInput.x > 0f) ? _navFocusedField.FindSelectableOnRight() : _navFocusedField.FindSelectableOnLeft()));
		if (!selectable || !IsSelectableInteractable(selectable))
		{
			return false;
		}
		_navFocusedField.DeactivateInputField();
		EventSystem.current.SetSelectedGameObject(selectable.gameObject);
		_navFocusedField = null;
		_navFieldEngaged = false;
		return true;
	}

	private async UniTask SelectOrHide(GameObject selectedObject = null)
	{
		if (!(await TrySelectAnything(selectedObject)))
		{
			_visual.Hide();
		}
		else if (_uiNavigationValueEvent.StoredValue == UiNavigationEnum.Keys)
		{
			_visual.Show();
		}
	}

	private bool IsPreviousTargetViable()
	{
		if ((bool)_previousTarget && _previousTarget.activeInHierarchy && _previousTarget.TryGetComponent<Selectable>(out var component))
		{
			return IsSelectableInteractable(component);
		}
		return false;
	}

	private async UniTask<bool> TrySelectAnything(GameObject selectedObject = null)
	{
		EventSystem current = EventSystem.current;
		if (!current)
		{
			return false;
		}
		if (!selectedObject)
		{
			selectedObject = current.currentSelectedGameObject;
		}
		if (IsPreviousTargetViable() && selectedObject != _previousTarget)
		{
			current.SetSelectedGameObject(_previousTarget);
			return true;
		}
		if ((bool)selectedObject && selectedObject.gameObject.activeInHierarchy)
		{
			if (!IsInteractable(selectedObject) || (selectedObject.TryGetComponent<Selectable>(out var component) && !IsSelectableInteractable(component)) || (!selectedObject.TryGetComponent<TMP_InputField>(out var _) && !selectedObject.TryGetComponent<VynilStickerUI>(out var _) && !selectedObject.TryGetComponent<StickerItem>(out var _) && selectedObject.TryGetComponent<Selectable>(out var component5) && component5.navigation.mode == Navigation.Mode.None))
			{
				current.SetSelectedGameObject(null);
				return false;
			}
			return true;
		}
		if ((bool)_preferredFallbackSelectable && _preferredFallbackSelectable.activeInHierarchy && _preferredFallbackSelectable.TryGetComponent<Selectable>(out var component6) && IsSelectableInteractable(component6))
		{
			current.SetSelectedGameObject(_preferredFallbackSelectable);
			return true;
		}
		GameObject gameObject = _getDefaultTarget();
		if ((bool)gameObject)
		{
			return await UiInputSetup.AutoSelectFirstInteractable(gameObject, immediately: true);
		}
		Canvas canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
		gameObject = (UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None).FirstOrDefault((Canvas c) => UiInputSetup.DefaultCanvases.Contains(c.name)) ?? canvas)?.gameObject;
		_setDefaultTarget(gameObject);
		_ = (bool)gameObject;
		return await UiInputSetup.AutoSelectFirstInteractable(gameObject, immediately: true);
	}
}
