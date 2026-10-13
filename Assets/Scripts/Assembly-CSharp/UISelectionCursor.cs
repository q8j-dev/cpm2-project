using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DebugPagesUi;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.GameInput;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;

public class UISelectionCursor : MonoBehaviour
{
	private const bool _debug = false;

	[Tooltip("The Image/RectTransform that will move to highlight the selection.")]
	[SerializeField]
	[Header("References")]
	private RectTransform cursorHighlight;

	[SerializeField]
	private GameObject defaultTarget;

	[SerializeField]
	private GameObject cursorImagePrefab;

	[SerializeField]
	private Canvas cursorTempCanvas;

	[SerializeField]
	[Header("Settings")]
	private float moveSpeed = 450f;

	[SerializeField]
	private float paddingPercent = 1.05f;

	[SerializeField]
	private Vector2 defaultSize = new Vector2(64f, 64f);

	[SerializeField]
	private float cursorSizeMultiplier = 1f;

	[SerializeField]
	[Header("ImagesForCursor")]
	[Tooltip("The selection image for key navigation.")]
	private Sprite square;

	[SerializeField]
	private Sprite idleStill;

	[SerializeField]
	private Sprite idleStillHover;

	[SerializeField]
	private Sprite idleMove;

	[SerializeField]
	private Sprite idleMoveHover;

	[SerializeField]
	private Sprite pressedStill;

	[SerializeField]
	private Sprite pressedStillHover;

	[SerializeField]
	private Sprite pressedMove;

	[SerializeField]
	private Sprite pressedMoveHover;

	[SerializeField]
	[Header("Auto Scroll")]
	private float autoScrollSpeed = 10f;

	[SerializeField]
	private float scrollMarginVertical = 50f;

	[SerializeField]
	private float scrollMarginHorizontal = 50f;

	[SerializeField]
	private bool _forceShowCursor;

	private static UISelectionCursor _instance;

	private CursorVisual _visual;

	private KeysNavigationController _keysNav;

	private GamepadHapticService _gamepadHaptics;

	private SteamKeyboardFlow _keyboard;

	private StickCursorController _stick;

	private readonly ScrollCenteringService _scroll = new ScrollCenteringService();

	private readonly SelectionHighlightOverride _selectionHighlight = new SelectionHighlightOverride();

	private int _lastGroupSwitchFrame = -1;

	private EventRegistry _eventRegistry;

	private InputMapSetEvent _inputMapSetEvent;

	private UiNavigationValueEvent _uiNavigationValueEvent;

	private RegistryHandle<BoolEvent> _onConfirmClickHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnConfirmClick);

	private RegistryHandle<ScriptableEvent> _confirmPerformedHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.Accept);

	private Transform _debugSheetRoot;

	private Transform _debugConsoleRoot;

	[Inject]
	public void Construct(EventRegistry eventRegistry, InputMapSetEvent inputMapSetEvent, GamepadHapticService gamepadHaptics, UiNavigationValueEvent uiNavigationValueEvent)
	{
		_eventRegistry = eventRegistry;
		_inputMapSetEvent = inputMapSetEvent;
		_uiNavigationValueEvent = uiNavigationValueEvent;
		_gamepadHaptics = gamepadHaptics;
	}

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		_instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
		Initialize();
	}

	private void Initialize()
	{
		_visual = new CursorVisual(cursorTempCanvas, cursorImagePrefab, defaultSize, paddingPercent, (bool v) =>
		{
			_forceShowCursor = v;
		});
		_keysNav = new KeysNavigationController(_inputMapSetEvent, _uiNavigationValueEvent, _visual, _scroll, () => defaultTarget, (GameObject v) =>
		{
			defaultTarget = v;
		}, IsDebugSheetChild);
		_keysNav.OnSelectionChangedPulse = () =>
		{
			_gamepadHaptics?.PulseUiNavigate();
		};
		_keyboard = new SteamKeyboardFlow(_eventRegistry, _uiNavigationValueEvent, _keysNav.IsSelectableInteractable);
		_stick = new StickCursorController(_eventRegistry, _inputMapSetEvent, _uiNavigationValueEvent, _visual, _keyboard, _keysNav, IsDebugSheetChild, () => moveSpeed, new StickCursorSprites
		{
			IdleStill = idleStill,
			IdleStillHover = idleStillHover,
			IdleMove = idleMove,
			IdleMoveHover = idleMoveHover,
			PressedStill = pressedStill,
			PressedStillHover = pressedStillHover,
			PressedMove = pressedMove,
			PressedMoveHover = pressedMoveHover
		});
		if (!FlagsStorage.IsSteamDeck && Mouse.current != null)
		{
			_visual.ExpectedMousePos = Mouse.current.position.ReadValue();
		}
		cursorTempCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
		cursorTempCanvas.sortingOrder = 30000;
		SceneManager.sceneLoaded += OnSceneLoaded;
		_uiNavigationValueEvent.Subscribe(SetCursorMoveMethod);
		_inputMapSetEvent.Subscribe(OnInputGroupChanged);
		_stick.RefreshPointerMapActive();
		_confirmPerformedHandle.Get(_eventRegistry).Subscribe(OnConfirmPerformed);
		_onConfirmClickHandle.Get(_eventRegistry).Subscribe(TryPressCursor);
		_onConfirmClickHandle.Get(_eventRegistry).Subscribe(OnConfirmClickForKeyboard);
		if (SteamClient.IsValid)
		{
			SteamUtils.OnGamepadTextInputDismissed += OnGamepadTextInputDismissed;
		}
		TryDisableDebugSheetNavigation().Forget();
		_keysNav.ResetForSceneLoad();
		_stick.ResetForSceneLoad();
		if ((bool)cursorHighlight)
		{
			Object.Destroy(cursorHighlight.gameObject);
		}
		cursorHighlight = null;
		_visual.ResetForSceneLoad();
		SetupCursorFlow().Forget();
	}

	private void OnDestroy()
	{
		_selectionHighlight.Restore();
		SceneManager.sceneLoaded -= OnSceneLoaded;
		_uiNavigationValueEvent?.Unsubscribe(SetCursorMoveMethod);
		_inputMapSetEvent?.Unsubscribe(OnInputGroupChanged);
		_confirmPerformedHandle.Get(_eventRegistry)?.Unsubscribe(OnConfirmPerformed);
		_onConfirmClickHandle.Get(_eventRegistry)?.Unsubscribe(TryPressCursor);
		_onConfirmClickHandle.Get(_eventRegistry)?.Unsubscribe(OnConfirmClickForKeyboard);
		if (SteamClient.IsValid)
		{
			SteamUtils.OnGamepadTextInputDismissed -= OnGamepadTextInputDismissed;
		}
	}

	private void OnInputGroupChanged(InputGroupsEnum group)
	{
		_stick.RefreshPointerMapActive();
		if (!_stick.IsPointerMapActive && _uiNavigationValueEvent.StoredValue == UiNavigationEnum.Stick)
		{
			HideCursor();
			_uiNavigationValueEvent.Raise(UiNavigationEnum.None);
		}
	}

	private void TryPressCursor(bool isStarted)
	{
		_stick.SetPressed(isStarted);
	}

	private void OnConfirmPerformed()
	{
		if (_lastGroupSwitchFrame == Time.frameCount)
		{
			return;
		}
		_lastGroupSwitchFrame = Time.frameCount;
		GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
		if (!(currentSelectedGameObject == null))
		{
			IEnumerable<InputMapEnum> maps;
			if (currentSelectedGameObject.TryGetComponent<TMP_InputField>(out var component))
			{
				OpenKeyboard(component);
			}
			else if (_uiNavigationValueEvent.StoredValue != UiNavigationEnum.Stick && (_uiNavigationValueEvent.StoredValue != UiNavigationEnum.Keys || Gamepad.current == null || !Gamepad.current.buttonSouth.wasPressedThisFrame || !_inputMapSetEvent.TryGetCurrentMaps(out maps) || !maps.Contains(InputMapEnum.UiPointer)))
			{
				ExecuteEvents.Execute(currentSelectedGameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
			}
		}
	}

	public void OpenKeyboard(TMP_InputField inputField, string placeHolderText = "Enter Text", bool fromPointerClick = false)
	{
		_keyboard.OpenKeyboard(inputField, placeHolderText, fromPointerClick);
	}

	private void OnConfirmClickForKeyboard(bool isStarted)
	{
		_keyboard.OnConfirmClick(isStarted);
	}

	private void OnGamepadTextInputDismissed(bool submitted)
	{
		_keyboard.OnGamepadTextInputDismissed(submitted);
	}

	public static void SimulateCloseKeyboard(bool submit)
	{
		if ((bool)_instance)
		{
			_instance._keyboard.SimulateTextInputDismissed(submit);
		}
	}

	public static void OpenKeyboardStatic(TMP_InputField inputField, string placeHolderText = "Enter Text")
	{
		if (_instance != null)
		{
			_instance.OpenKeyboard(inputField, placeHolderText);
		}
	}

	private void SetCursorMoveMethod(UiNavigationEnum moveMethod)
	{
		switch (moveMethod)
		{
		case UiNavigationEnum.None:
			_visual.ResetHighlight();
			if (!FlagsStorage.IsSteamDeck)
			{
				if (Mouse.current != null)
				{
					_visual.ExpectedMousePos = Mouse.current.position.ReadValue();
				}
				Cursor.visible = false;
			}
			break;
		case UiNavigationEnum.Mouse:
			_visual.ResetHighlight();
			_stick.ClearHoverState();
			if (!FlagsStorage.IsSteamDeck)
			{
				Cursor.visible = true;
				if (Mouse.current != null)
				{
					Mouse.current.WarpCursorPosition(_visual.ExpectedMousePos);
				}
			}
			if (!_forceShowCursor)
			{
				HideCursor();
			}
			break;
		case UiNavigationEnum.Keys:
			if (_keysNav.DisabledKeyNavigation)
			{
				_uiNavigationValueEvent.Raise(UiNavigationEnum.Keys);
			}
			else
			{
				if (!_visual.CursorHighlight)
				{
					break;
				}
				if (!FlagsStorage.IsSteamDeck)
				{
					Cursor.visible = false;
					if (Mouse.current != null)
					{
						_visual.ExpectedMousePos = Mouse.current.position.ReadValue();
					}
				}
				_visual.ResetHighlight();
				_stick.ClearHoverState();
				if ((bool)square)
				{
					_visual.CursorImage.sprite = square;
				}
				if (!_keyboard.CurrentInputField && !_keysNav.DisabledKeyNavigation)
				{
					ShowCursor();
				}
			}
			break;
		case UiNavigationEnum.Stick:
			if (!_visual.CursorHighlight)
			{
				break;
			}
			if (!FlagsStorage.IsSteamDeck)
			{
				Cursor.visible = false;
				if (Mouse.current != null)
				{
					_visual.ExpectedMousePos = Mouse.current.position.ReadValue();
				}
			}
			_visual.ResetHighlight();
			if (_visual.IsSetupComplete && _stick.IsPointerMapActive && !_stick.IsGroupExcluded(_inputMapSetEvent.CurrentInputGroup))
			{
				_visual.CurrentTargetCanvas = null;
				_visual.CursorHighlight.localScale = Vector3.one * cursorSizeMultiplier;
				Vector2 expectedMousePos = _visual.ExpectedMousePos;
				expectedMousePos.x = Mathf.Clamp(expectedMousePos.x, 0f, Screen.width);
				expectedMousePos.y = Mathf.Clamp(expectedMousePos.y, 0f, Screen.height);
				RectTransformUtility.ScreenPointToWorldPointInRectangle(cursorTempCanvas.transform as RectTransform, expectedMousePos, null, out var worldPoint);
				_visual.CursorHighlight.position = worldPoint;
				_stick.UpdateCursorSprite();
				ShowCursor();
			}
			break;
		}
	}

	private void OnSceneLoaded(Scene current, LoadSceneMode loadSceneMode)
	{
		if (loadSceneMode != LoadSceneMode.Additive)
		{
			_selectionHighlight.Restore();
			_keysNav.ResetForSceneLoad();
			_stick.ResetForSceneLoad();
			_visual.ResetForSceneLoad();
			SetupCursorFlow().Forget();
		}
	}

	private async UniTask TryDisableDebugSheetNavigation()
	{
	}

	private async UniTask SetupCursorFlow()
	{
		if (await _visual.SetupAsync())
		{
			_keysNav.ResolveDefaultTargetIfNull();
		}
	}

	public static void HideCursor(bool force = false)
	{
		if ((bool)_instance && _instance._visual.Hide() && force)
		{
			_instance._uiNavigationValueEvent?.Raise(UiNavigationEnum.None);
		}
	}

	public static void ShowCursor(GameObject target = null, bool force = false)
	{
		if ((bool)_instance)
		{
			_instance._visual.Show(force);
			if ((bool)target && !_instance._keysNav.DisabledKeyNavigation && (bool)EventSystem.current && target.TryGetComponent<Button>(out var component))
			{
				EventSystem.current.SetSelectedGameObject(null);
				EventSystem.current.SetSelectedGameObject(component.gameObject);
				_instance._keysNav.NavigateWithKeys(target);
			}
		}
	}

	public static void SetPreferredFallbackSelectable(GameObject selectable)
	{
		if ((bool)_instance)
		{
			_instance._keysNav.PreferredFallbackSelectable = selectable;
		}
	}

	private bool IsDebugSheetChild(Transform t)
	{
		if (!_debugSheetRoot || !t.IsChildOf(_debugSheetRoot))
		{
			if ((bool)_debugConsoleRoot)
			{
				return t.IsChildOf(_debugConsoleRoot);
			}
			return false;
		}
		return true;
	}

	[DebugButton("Steam/")]
	public static void ToggleEnableSquareCursor()
	{
		_instance._keysNav.DisabledKeyNavigation = !_instance._keysNav.DisabledKeyNavigation;
		_instance._uiNavigationValueEvent.Raise(UiNavigationEnum.Stick);
	}

	private void LateUpdate()
	{
		if (_instance != this)
		{
			return;
		}
		_keyboard.UpdateTypingTracker();
		if (_keyboard.IsOpen)
		{
			if (!_keyboard.CurrentInputField || !_keyboard.CurrentInputField.isActiveAndEnabled)
			{
				_keyboard.ForceCloseKeyboard();
			}
			return;
		}
		if (!EventSystem.current)
		{
			_selectionHighlight.Restore();
			HideCursor();
			return;
		}
		switch (_uiNavigationValueEvent.StoredValue)
		{
		case UiNavigationEnum.Keys:
			_keysNav.UpdateNavFieldEngagement();
			if (!_keysNav.TryNavigateOffField())
			{
				_keysNav.NavigateWithKeys();
			}
			break;
		case UiNavigationEnum.Stick:
			if (!_stick.IsPointerMapActive || _stick.IsGroupExcluded(_inputMapSetEvent.CurrentInputGroup))
			{
				_selectionHighlight.Restore();
				HideCursor();
				return;
			}
			_stick.MoveCursorViaStick();
			_stick.UpdateHover();
			break;
		}
		UpdateSelectionHighlightOverride();
	}

	private void UpdateSelectionHighlightOverride()
	{
		UiNavigationEnum storedValue = _uiNavigationValueEvent.StoredValue;
		if (storedValue == UiNavigationEnum.Keys || storedValue == UiNavigationEnum.Stick)
		{
			_selectionHighlight.Apply(EventSystem.current.currentSelectedGameObject);
		}
		else
		{
			_selectionHighlight.Restore();
		}
	}
}
