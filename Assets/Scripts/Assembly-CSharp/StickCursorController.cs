using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;

public class StickCursorController
{
	private const bool _debug = false;

	private readonly EventRegistry _eventRegistry;

	private readonly InputMapSetEvent _inputMapSetEvent;

	private readonly UiNavigationValueEvent _uiNavigationValueEvent;

	private readonly CursorVisual _visual;

	private readonly SteamKeyboardFlow _keyboard;

	private readonly KeysNavigationController _keysNav;

	private readonly Func<Transform, bool> _isDebugSheetChild;

	private readonly Func<float> _getMoveSpeed;

	private readonly StickCursorSprites _sprites;

	private PointerEventData _pointerEventData;

	private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>(16);

	private GameObject _hoveredObject;

	private Selectable _hoveredSelectable;

	private bool _isCursorMoving;

	private bool _isCursorPressed;

	private bool _isPointerMapActive;

	private RegistryHandle<Vector2Variable> _cursorMoveHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.StickCursorMove);

	private readonly HashSet<InputGroupsEnum> _excludedGroupsFromStickCursor = new HashSet<InputGroupsEnum>
	{
		InputGroupsEnum.PaintBase,
		InputGroupsEnum.PaintColor,
		InputGroupsEnum.PaintMultiDone,
		InputGroupsEnum.PaintSelectedOne,
		InputGroupsEnum.PaintSize,
		InputGroupsEnum.PaintText,
		InputGroupsEnum.PaintPlates,
		InputGroupsEnum.Drone,
		InputGroupsEnum.PhotoMod
	};

	public bool IsPointerMapActive => _isPointerMapActive;

	public StickCursorController(EventRegistry eventRegistry, InputMapSetEvent inputMapSetEvent, UiNavigationValueEvent uiNavigationValueEvent, CursorVisual visual, SteamKeyboardFlow keyboard, KeysNavigationController keysNav, Func<Transform, bool> isDebugSheetChild, Func<float> getMoveSpeed, StickCursorSprites sprites)
	{
		_eventRegistry = eventRegistry;
		_inputMapSetEvent = inputMapSetEvent;
		_uiNavigationValueEvent = uiNavigationValueEvent;
		_visual = visual;
		_keyboard = keyboard;
		_keysNav = keysNav;
		_isDebugSheetChild = isDebugSheetChild;
		_getMoveSpeed = getMoveSpeed;
		_sprites = sprites;
		_pointerEventData = new PointerEventData(EventSystem.current);
	}

	public void ResetForSceneLoad()
	{
		_hoveredObject = null;
		_hoveredSelectable = null;
		if (EventSystem.current != null)
		{
			_pointerEventData = new PointerEventData(EventSystem.current);
		}
	}

	public bool IsGroupExcluded(InputGroupsEnum group)
	{
		return _excludedGroupsFromStickCursor.Contains(group);
	}

	public void RefreshPointerMapActive()
	{
		_isPointerMapActive = _inputMapSetEvent.TryGetCurrentMaps(out var maps) && maps.Contains(InputMapEnum.UiPointer);
	}

	public void SetPressed(bool isStarted)
	{
		if (_uiNavigationValueEvent.StoredValue == UiNavigationEnum.Stick)
		{
			_isCursorPressed = isStarted;
		}
	}

	public void MoveCursorViaStick()
	{
		if (_visual.IsSetupComplete)
		{
			Vector2 value = _cursorMoveHandle.Get(_eventRegistry).Value;
			_isCursorMoving = value != Vector2.zero;
			if (!(value == Vector2.zero))
			{
				Vector3 vector = new Vector3(value.x, value.y, 0f) * (_getMoveSpeed() * Time.unscaledDeltaTime);
				Vector3 position = _visual.CursorHighlight.position + vector;
				position.x = Mathf.Clamp(position.x, 0f, Screen.width);
				position.y = Mathf.Clamp(position.y, 0f, Screen.height);
				_visual.CursorHighlight.position = position;
				float z = Mathf.Atan2(value.y, value.x) * 57.29578f;
				_visual.CursorHighlight.localRotation = Quaternion.Euler(0f, 0f, z);
				UpdateMousePosition();
				_visual.Show();
			}
		}
	}

	private void UpdateMousePosition()
	{
		if (Mouse.current != null)
		{
			Vector2 vector = RectTransformUtility.WorldToScreenPoint(_visual.CurrentTargetCanvas ? _visual.CurrentTargetCanvas.worldCamera : null, _visual.CursorHighlight.position);
			Mouse.current.WarpCursorPosition(vector);
			_visual.ExpectedMousePos = vector;
		}
	}

	public void UpdateHover()
	{
		EventSystem current = EventSystem.current;
		if (!_visual.CursorHighlight || !current || _pointerEventData == null)
		{
			return;
		}
		_pointerEventData.position = _visual.CursorHighlight.position;
		_raycastResults.Clear();
		current.RaycastAll(_pointerEventData, _raycastResults);
		GameObject gameObject = _raycastResults.Where((RaycastResult t) =>
		{
			Func<Transform, bool> isDebugSheetChild = _isDebugSheetChild;
			RaycastResult raycastResult = t;
			return !isDebugSheetChild(raycastResult.gameObject.transform);
		}).Select((RaycastResult t) =>
		{
			RaycastResult raycastResult = t;
			return raycastResult.gameObject;
		}).FirstOrDefault();
		Selectable hoveredSelectable = null;
		if ((bool)gameObject)
		{
			hoveredSelectable = gameObject.GetComponentInParent<Selectable>();
		}
		if ((bool)_hoveredObject && _hoveredObject != gameObject)
		{
			ExecuteEvents.Execute(_hoveredObject, _pointerEventData, ExecuteEvents.pointerExitHandler);
		}
		if ((bool)gameObject && gameObject != _hoveredObject)
		{
			ExecuteEvents.Execute(gameObject, _pointerEventData, ExecuteEvents.pointerEnterHandler);
		}
		_hoveredObject = gameObject;
		_hoveredSelectable = hoveredSelectable;
		_keysNav.PreviousTarget = _hoveredObject;
		if ((bool)_hoveredSelectable)
		{
			if (current.currentSelectedGameObject != _hoveredSelectable.gameObject)
			{
				current.SetSelectedGameObject(_hoveredSelectable.gameObject);
			}
		}
		else
		{
			current.SetSelectedGameObject(null);
			_keyboard.ClearDialogFieldRef();
		}
		UpdateCursorSprite();
	}

	public void UpdateCursorSprite()
	{
		if ((bool)_visual.CursorImage && _uiNavigationValueEvent.StoredValue == UiNavigationEnum.Stick)
		{
			bool flag = _hoveredSelectable;
			bool isCursorPressed = _isCursorPressed;
			bool isCursorMoving = _isCursorMoving;
			Sprite sprite = ((!isCursorPressed) ? ((!isCursorMoving) ? (flag ? _sprites.IdleStillHover : _sprites.IdleStill) : (flag ? _sprites.IdleMoveHover : _sprites.IdleMove)) : ((!isCursorMoving) ? (flag ? _sprites.PressedStillHover : _sprites.PressedStill) : (flag ? _sprites.PressedMoveHover : _sprites.PressedMove)));
			Sprite sprite2 = sprite;
			if ((bool)sprite2)
			{
				_visual.CursorImage.sprite = sprite2;
			}
		}
	}

	public void ClearHoverState()
	{
		_hoveredObject = null;
		_hoveredSelectable = null;
		_isCursorPressed = false;
		_isCursorMoving = false;
		EventSystem.current.SetSelectedGameObject(null);
	}
}
