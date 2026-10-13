using System;
using Cysharp.Threading.Tasks;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Steam;

public class SteamKeyboardFlow
{
	private const bool _debug = false;

	private readonly EventRegistry _eventRegistry;

	private readonly UiNavigationValueEvent _uiNavigationValueEvent;

	private readonly Func<Selectable, bool> _isSelectableInteractable;

	private TMP_InputField _currentInputField;

	private bool _isKeyboardOpen;

	private const int MaxChars = 256;

	private bool _confirmPressWhileKeyboardOpen;

	private int _lastKeyboardCloseFrame = -100;

	private const int KeyboardReopenCooldownFrames = 3;

	private TMP_InputField _floatingShownForField;

	private int _lastFloatingOpenFrame = -100;

	private TMP_InputField _typingTrackedField;

	private string _typingBaseline;

	private bool _typedSinceFocus;

	private RegistryHandle<BoolEvent> _setActionsActiveHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.SetActionsActive);

	private readonly Vector3[] _fieldCorners = new Vector3[4];

	public bool IsOpen => _isKeyboardOpen;

	public TMP_InputField CurrentInputField => _currentInputField;

	public SteamKeyboardFlow(EventRegistry eventRegistry, UiNavigationValueEvent uiNavigationValueEvent, Func<Selectable, bool> isSelectableInteractable)
	{
		_eventRegistry = eventRegistry;
		_uiNavigationValueEvent = uiNavigationValueEvent;
		_isSelectableInteractable = isSelectableInteractable;
	}

	public void ClearDialogFieldRef()
	{
		if ((bool)_currentInputField)
		{
			_currentInputField = null;
		}
	}

	public void OpenKeyboard(TMP_InputField inputField, string placeHolderText = "Enter Text", bool fromPointerClick = false)
	{
		if (SteamClient.IsValid && !_isKeyboardOpen && (bool)inputField && Time.frameCount - _lastKeyboardCloseFrame >= 3 && (!ShouldUseFloatingKeyboard() || !TryOpenFloatingKeyboard(inputField, fromPointerClick)))
		{
			_currentInputField = inputField;
			_isKeyboardOpen = true;
			bool inputMode = inputField.contentType == TMP_InputField.ContentType.Password;
			GamepadTextInputLineMode lineInputMode = ((inputField.lineType == TMP_InputField.LineType.MultiLineNewline) ? GamepadTextInputLineMode.MultipleLines : GamepadTextInputLineMode.SingleLine);
			int maxChars = ((inputField.characterLimit > 0) ? inputField.characterLimit : 256);
			string description = ((inputField.placeholder is TMP_Text tMP_Text) ? tMP_Text.text : placeHolderText);
			if (SteamUtils.ShowGamepadTextInput(inputMode ? GamepadTextInputMode.Password : GamepadTextInputMode.Normal, lineInputMode, description, maxChars, _currentInputField.text))
			{
				_setActionsActiveHandle.Get(_eventRegistry).Raise(value: false);
				return;
			}
			_isKeyboardOpen = false;
			_currentInputField = null;
		}
	}

	private bool ShouldUseFloatingKeyboard()
	{
		if (SteamClient.IsValid)
		{
			if (!SteamUtils.IsRunningOnSteamDeck)
			{
				return !SteamUtils.IsSteamInBigPictureMode;
			}
			return true;
		}
		return false;
	}

	private bool TryOpenFloatingKeyboard(TMP_InputField inputField, bool fromPointerClick)
	{
		if (Time.frameCount - _lastFloatingOpenFrame < 3)
		{
			return true;
		}
		if (!fromPointerClick)
		{
			if (_floatingShownForField == inputField && inputField.isFocused)
			{
				return true;
			}
			if (_typedSinceFocus && _typingTrackedField == inputField)
			{
				return true;
			}
		}
		TextInputMode floatingKeyboardMode = GetFloatingKeyboardMode(inputField);
		GetFieldScreenRect(inputField, out var x, out var y, out var w, out var h);
		if (!SteamFloatingKeyboard.TryShow(floatingKeyboardMode, x, y, w, h))
		{
			return false;
		}
		_lastFloatingOpenFrame = Time.frameCount;
		_floatingShownForField = inputField;
		return true;
	}

	private static TextInputMode GetFloatingKeyboardMode(TMP_InputField inputField)
	{
		if (inputField.lineType == TMP_InputField.LineType.MultiLineNewline)
		{
			return TextInputMode.MultipleLines;
		}
		return inputField.contentType switch
		{
			TMP_InputField.ContentType.IntegerNumber => TextInputMode.Numeric, 
			TMP_InputField.ContentType.DecimalNumber => TextInputMode.Numeric, 
			TMP_InputField.ContentType.Pin => TextInputMode.Numeric, 
			TMP_InputField.ContentType.EmailAddress => TextInputMode.Email, 
			_ => TextInputMode.SingleLine, 
		};
	}

	private void GetFieldScreenRect(TMP_InputField inputField, out int x, out int y, out int w, out int h)
	{
		(inputField.textViewport ? inputField.textViewport : ((RectTransform)inputField.transform)).GetWorldCorners(_fieldCorners);
		Canvas componentInParent = inputField.GetComponentInParent<Canvas>();
		Camera cam = null;
		if ((bool)componentInParent && (bool)componentInParent.rootCanvas && componentInParent.rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
		{
			cam = componentInParent.rootCanvas.worldCamera;
		}
		Vector2 lhs = new Vector2(float.MaxValue, float.MaxValue);
		Vector2 lhs2 = new Vector2(float.MinValue, float.MinValue);
		for (int i = 0; i < 4; i++)
		{
			Vector2 rhs = RectTransformUtility.WorldToScreenPoint(cam, _fieldCorners[i]);
			lhs = Vector2.Min(lhs, rhs);
			lhs2 = Vector2.Max(lhs2, rhs);
		}
		x = (int)lhs.x;
		y = (int)((float)Screen.height - lhs2.y);
		w = (int)(lhs2.x - lhs.x);
		h = (int)(lhs2.y - lhs.y);
	}

	public void OnConfirmClick(bool isStarted)
	{
		if (isStarted)
		{
			_confirmPressWhileKeyboardOpen = _isKeyboardOpen;
		}
		else
		{
			if (_isKeyboardOpen)
			{
				return;
			}
			if (_confirmPressWhileKeyboardOpen)
			{
				_confirmPressWhileKeyboardOpen = false;
				return;
			}
			UiNavigationEnum storedValue = _uiNavigationValueEvent.StoredValue;
			if (storedValue == UiNavigationEnum.Mouse || storedValue == UiNavigationEnum.Stick)
			{
				TryOpenKeyboardFromClickAsync().Forget();
			}
		}
	}

	private async UniTask TryOpenKeyboardFromClickAsync()
	{
		await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
		if (!_isKeyboardOpen && (bool)EventSystem.current)
		{
			GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
			if ((bool)currentSelectedGameObject && currentSelectedGameObject.TryGetComponent<TMP_InputField>(out var component) && component.isFocused && _isSelectableInteractable(component))
			{
				OpenKeyboard(component, "Enter Text", fromPointerClick: true);
			}
		}
	}

	public void UpdateTypingTracker()
	{
		EventSystem current = EventSystem.current;
		GameObject gameObject = (current ? current.currentSelectedGameObject : null);
		TMP_InputField component = null;
		if ((bool)gameObject)
		{
			gameObject.TryGetComponent<TMP_InputField>(out component);
		}
		if ((bool)component && !component.isFocused)
		{
			component = null;
		}
		if (component == _typingTrackedField)
		{
			if ((bool)component && !_typedSinceFocus && !string.Equals(component.text, _typingBaseline))
			{
				_typedSinceFocus = true;
			}
			return;
		}
		if ((bool)_floatingShownForField && _floatingShownForField == _typingTrackedField)
		{
			_floatingShownForField = null;
			SteamFloatingKeyboard.TryDismiss();
		}
		_typingTrackedField = component;
		_typingBaseline = (component ? component.text : null);
		_typedSinceFocus = false;
	}

	public void ForceCloseKeyboard()
	{
		if (_isKeyboardOpen)
		{
			SimulateTextInputDismissed(submitted: false);
		}
	}

	public void SimulateTextInputDismissed(bool submitted)
	{
		if (_isKeyboardOpen)
		{
			_isKeyboardOpen = false;
			_lastKeyboardCloseFrame = Time.frameCount;
			if (submitted && (bool)_currentInputField)
			{
				_currentInputField.onSubmit.Invoke(_currentInputField.text);
			}
			_currentInputField = null;
			_setActionsActiveHandle.Get(_eventRegistry).Raise(value: true);
			EventSystem.current.SetSelectedGameObject(null);
		}
	}

	public void OnGamepadTextInputDismissed(bool submitted)
	{
		if (_isKeyboardOpen)
		{
			_isKeyboardOpen = false;
			_lastKeyboardCloseFrame = Time.frameCount;
			if (submitted && _currentInputField != null)
			{
				string enteredGamepadText = SteamUtils.GetEnteredGamepadText();
				_currentInputField.text = enteredGamepadText;
				_currentInputField.onSubmit.Invoke(enteredGamepadText);
			}
			_currentInputField = null;
			_setActionsActiveHandle.Get(_eventRegistry).Raise(value: true);
			EventSystem.current.SetSelectedGameObject(null);
		}
	}
}
