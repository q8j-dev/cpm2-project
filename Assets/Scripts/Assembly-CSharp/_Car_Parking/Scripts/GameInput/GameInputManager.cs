using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Car.Physics;
using _Car_Parking.Scripts.GameInput.Rebinding;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.GameInput
{
	public class GameInputManager : IStartable, ITickable, IFixedTickable, IDisposable
	{
		private enum PendingMapToggle
		{
			None = 0,
			Enable = 1,
			Disable = 2
		}

		private RegistryHandle<ScriptableEvent> _openPhoneHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OpenPhone);

		private RegistryHandle<ScriptableEvent> _onMenuHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnMenu);

		private RegistryHandle<BoolEvent> _onThrottleHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnThrottle);

		private RegistryHandle<BoolEvent> _onStartEngineHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnStartEngine);

		private RegistryHandle<BoolEvent> _onLookHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnLook);

		private RegistryHandle<ScriptableEvent> _cancelHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.Cancel);

		private RegistryHandle<ScriptableEvent> _acceptHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.Accept);

		private RegistryHandle<ScriptableEvent> _changeCameraHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ChangeCamera);

		private RegistryHandle<BoolEvent> _circleMenuHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.CircleMenu);

		private RegistryHandle<BoolEvent> _circleMenuScrollHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.CircleMenuScroll);

		private RegistryHandle<ScriptableEvent> _closeCircleMenuHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.CloseCircleMenu);

		private RegistryHandle<BoolEvent> _doorStartCancelHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.DoorStartCancel);

		private RegistryHandle<BoolEvent> _shifterUpDownHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.ShifterUpDown);

		private RegistryHandle<BoolEvent> _shifterMAHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.ShifterMA);

		private RegistryHandle<BoolEvent> _shifterMoveStartedHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.ShifterMoveStarted);

		private RegistryHandle<ScriptableEvent> _shiftToggleMAHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ShiftToggleMA);

		private RegistryHandle<BoolEvent> _onHornHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnHorn);

		private RegistryHandle<ScriptableEvent> _onHeadlightsHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnHeadlights);

		private RegistryHandle<BoolEvent> _onHandbrakeHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnHandbrake);

		private RegistryHandle<BoolEvent> _onHandbrakeHoldHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnHandbrakeHold);

		private RegistryHandle<GearVariable> _gearHandle = new RegistryHandle<GearVariable>(EventRegistryIDs.Gear);

		private RegistryHandle<BoolEvent> _onVoiceHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnVoice);

		private RegistryHandle<BoolEvent> _onCircleMenuCamHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnCircleMenuCam);

		private RegistryHandle<ScriptableEvent> _closeCircleMenuCamHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.CloseCircleMenuCam);

		private RegistryHandle<BoolEvent> _onCircleMenuAnimHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnCircleMenuAnim);

		private RegistryHandle<BoolEvent> _onLookFreeHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnLookFree);

		private RegistryHandle<BoolEvent> _onMapMoveHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnMapMove);

		private RegistryHandle<ScriptableEvent> _toggleFocusHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleFocus);

		private RegistryHandle<ScriptableEvent> _showPupUpOnMapHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ShowPupUpOnMap);

		private RegistryHandle<ScriptableEvent> _toggleSlowMoveHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleSlowMove);

		private RegistryHandle<ScriptableEvent> _interactHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.Interact);

		private RegistryHandle<FloatVariable> _throttleHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.Throttle);

		private RegistryHandle<FloatVariable> _brakeHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.Brake);

		private RegistryHandle<FloatVariable> _clutchHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.Clutch);

		private RegistryHandle<FloatVariable> _steerHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.Steer);

		private RegistryHandle<FloatVariable> _handbrakeHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.Handbrake);

		private RegistryHandle<FloatVariable> _zoomHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.Zoom);

		private RegistryHandle<Vector2Variable> _lookHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.Look);

		private RegistryHandle<Vector2Variable> _movePersonHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.MovePerson);

		private RegistryHandle<Vector2Variable> _circleMenuMoveHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.CircleMenuMove);

		private RegistryHandle<Vector2Variable> _shifterMoveHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.ShifterMove);

		private RegistryHandle<Vector2Variable> _lookRotateHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.LookRotate);

		private RegistryHandle<Vector2Variable> _lookFreeHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.LookFree);

		private RegistryHandle<Vector2ValueEvent> _mapDragMoveValueEventHandle = new RegistryHandle<Vector2ValueEvent>(EventRegistryIDs.MapDragMoveValueEvent);

		private RegistryHandle<BoolEvent> _setActionsActiveHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.SetActionsActive);

		private RegistryHandle<BoolEvent> _onConfirmClickHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnConfirmClick);

		private RegistryHandle<Vector2Variable> _stickCursorMoveHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.StickCursorMove);

		private RegistryHandle<ScriptableEvent> _bindingsChangedHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.BindingsChanged);

		private RegistryHandle<RebindCaptureStateEvent> _bindingCaptureStateHandle = new RegistryHandle<RebindCaptureStateEvent>(EventRegistryIDs.BindingCaptureState);

		private RegistryHandle<StringEvent> _bindingPresetChangedHandle = new RegistryHandle<StringEvent>(EventRegistryIDs.BindingPresetChanged);

		private RegistryHandle<ScrollValueEvent> _scrollValueEventHandle = new RegistryHandle<ScrollValueEvent>(EventRegistryIDs.ScrollValueEvent);

		private RegistryHandle<BoolEvent> _aimAtHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.AimAt);

		private RegistryHandle<BoolEvent> _acceptHoldHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.AcceptHold);

		private RegistryHandle<BoolEvent> _onNextPreviousHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnNextPrevious);

		private RegistryHandle<BoolEvent> _onLayerUpDownHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnLayerUpDown);

		private RegistryHandle<ScriptableEvent> _toggleToSizeHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleToSize);

		private RegistryHandle<ScriptableEvent> _toggleToColorHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleToColor);

		private RegistryHandle<ScriptableEvent> _addFavoriteHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.AddFavorite);

		private RegistryHandle<ScriptableEvent> _copyHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.Copy);

		private RegistryHandle<ScriptableEvent> _deleteAllHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.DeleteAll);

		private RegistryHandle<ScriptableEvent> _deleteSelectedHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.DeleteSelected);

		private RegistryHandle<ScriptableEvent> _mirrorHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.Mirror);

		private RegistryHandle<ScriptableEvent> _setColorHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.SetColor);

		private RegistryHandle<ScriptableEvent> _setSizeHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.SetSize);

		private RegistryHandle<ScriptableEvent> _blockCameraHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.BlockCamera);

		private RegistryHandle<ScriptableEvent> _toggleFavoritesHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleFavorites);

		private RegistryHandle<ScriptableEvent> _toggleMultiSelectHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleMultiSelect);

		private RegistryHandle<ScriptableEvent> _toggleUiHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleUi);

		private RegistryHandle<FloatVariable> _opacityChangeHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.OpacityChange);

		private RegistryHandle<FloatVariable> _turnLeftRightHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.TurnLeftRight);

		private RegistryHandle<Vector2Variable> _colorChangeHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.ColorChange);

		private RegistryHandle<Vector2Variable> _sizeChangeHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.SizeChange);

		private RegistryHandle<Vector2Variable> _positionChangeHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.PositionChange);

		private RegistryHandle<ScriptableEvent> _onCancelChangesHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnCancelChanges);

		private RegistryHandle<ScriptableEvent> _onStartColorMoveHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnStartColorMove);

		private RegistryHandle<ScriptableEvent> _onStartTintMoveHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnStartTintMove);

		private RegistryHandle<ScriptableEvent> _onUnselectOrBackHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnUnselectOrBack);

		private RegistryHandle<ScriptableEvent> _onSetTextHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnSetText);

		private RegistryHandle<ScriptableEvent> _toggleToTextHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleToText);

		private RegistryHandle<ScriptableEvent> _toggleBoldHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleBold);

		private RegistryHandle<ScriptableEvent> _toggleItalicsHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleItalics);

		private RegistryHandle<BoolEvent> _onFontUpDownHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnFontUpDown);

		private RegistryHandle<ScriptableEvent> _onToggleFrontPlateHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnToggleFrontPlate);

		private RegistryHandle<ScriptableEvent> _onToggleBackPlateHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnToggleBackPlate);

		private RegistryHandle<ScriptableEvent> _onEnterPlateTypesHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnEnterPlateTypes);

		private RegistryHandle<Vector2Variable> _lookVinylHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.LookVinyl);

		private RegistryHandle<FloatVariable> _zoomVinylHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.ZoomVinyl);

		private RegistryHandle<BoolEvent> _toggleMapHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.ToggleMap);

		private RegistryHandle<Vector3Variable> _mapMoveHandle = new RegistryHandle<Vector3Variable>(EventRegistryIDs.MapMove);

		private RegistryHandle<ScriptableEvent> _toggleFilterOnMapHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleFilterOnMap);

		private RegistryHandle<ScriptableEvent> _toggleShowHideMapFiltersHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleShowHideMapFilters);

		private RegistryHandle<ScriptableEvent> _markOnMapHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.MarkOnMap);

		private RegistryHandle<ScriptableEvent> _hideShowAllFiltersHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.HideShowAllFilters);

		private RegistryHandle<FloatVariable> _droneCameraHeightHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.DroneCameraHeight);

		private RegistryHandle<Vector2Variable> _droneCameraMoveHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.DroneCameraMove);

		private RegistryHandle<FloatVariable> _droneCameraTiltHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.DroneCameraTilt);

		private RegistryHandle<ScriptableEvent> _onSnapshotHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnSnapshot);

		private RegistryHandle<ScriptableEvent> _onResetDroneCameraHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnResetDroneCamera);

		private RegistryHandle<ScriptableEvent> _toggleDroneSpeedHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleDroneSpeed);

		private RegistryHandle<ScriptableEvent> _onCenterDroneCameraHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnCenterDroneCamera);

		private RegistryHandle<ScriptableEvent> _closeDroneHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.CloseDrone);

		private RegistryHandle<ScriptableEvent> _onPhotoHelpHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnPhotoHelp);

		private RegistryHandle<BoolEvent> _navigatePhotoSettingsHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.NavigatePhotoSettings);

		private RegistryHandle<BoolEvent> _setPhotoSettingsHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.SetPhotoSettings);

		private RegistryHandle<ScriptableEvent> _onPhotoColorHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnPhotoColor);

		private RegistryHandle<ScriptableEvent> _onPhotoOpticsHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnPhotoOptics);

		private RegistryHandle<ScriptableEvent> _onPhotoCameraHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnPhotoCamera);

		private RegistryHandle<BoolEvent> _onCameraMoveHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnCameraMove);

		private RegistryHandle<BoolEvent> _onCameraHeightHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnCameraHeight);

		private RegistryHandle<BoolEvent> _onCameraTiltHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnCameraTilt);

		private RegistryHandle<BoolEvent> _onOpenChatHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnOpenChat);

		private RegistryHandle<ScriptableEvent> _onResetFilterHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnResetFilter);

		private RegistryHandle<ScriptableEvent> _toggleLeftSignalHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleLeftSignal);

		private RegistryHandle<ScriptableEvent> _toggleRightSignalHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleRightSignal);

		private RegistryHandle<ScriptableEvent> _toggleEmergencySignalHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleEmergencySignal);

		private RegistryHandle<BoolEvent> _startStopLookLeftHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.StartStopLookLeft);

		private RegistryHandle<BoolEvent> _startStopLookBackHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.StartStopLookBack);

		private RegistryHandle<BoolEvent> _startStopLookRightHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.StartStopLookRight);

		private RegistryHandle<FloatVariable> _personRotateRightLeftHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.PersonRotateRightLeft);

		private RegistryHandle<ScriptableEvent> _toggleCameraSettingsHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleCameraSettings);

		private RegistryHandle<ScriptableEvent> _keepLaneHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.KeepLane);

		private RegistryHandle<BoolEvent> _startStopBurnoutHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.StartStopBurnout);

		private RegistryHandle<ScriptableEvent> _toggleTimerHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleTimer);

		private RegistryHandle<ScriptableEvent> _openPhotoModeHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OpenPhotoMode);

		private RegistryHandle<ScriptableEvent> _toRoadHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToRoad);

		private RegistryHandle<ScriptableEvent> _restartRaceHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.RestartRace);

		private RegistryHandle<ScriptableEvent> _cancelRaceHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.CancelRace);

		private RegistryHandle<ScriptableEvent> _backToCarHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.BackToCar);

		private RegistryHandle<Vector2Variable> _cameraSetupMoveHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.CameraSetupMove);

		private RegistryHandle<FloatVariable> _cameraSetupHeightHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.CameraSetupHeight);

		private RegistryHandle<FloatVariable> _personRunHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.PersonRun);

		private RegistryHandle<ScriptableEvent> _personCrouchHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.PersonCrouch);

		private RegistryHandle<ScriptableEvent> _personJumpHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.PersonJump);

		private RegistryHandle<ScriptableEvent> _personWalkRunToggleHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.PersonWalkRunToggle);

		private readonly HashSet<string> _cancelEventDefaults = new HashSet<string> { "_Car_Parking.Scripts.Drag.ManagerUI.Back", "FreeDriveControl.CancelInviteDelegate" };

		private EventRegistry _eventRegistry;

		private DesktopControlsSettingsSO _controlsSettings;

		private InputMapSetEvent _inputMapSetEvent;

		private UiNavigationValueEvent _uiNavigationValueEvent;

		private CurrentDeviceValueEvent _currentDeviceValueEvent;

		private SteeringWheelValueEvent _steeringWheelValueEvent;

		private GameInputController _controls;

		private UnityEngine.EventSystems.EventSystem _lastEventSystem;

		private InputSystemUIInputModule _lastInputModule;

		private bool _disposed;

		private readonly CancellationTokenSource _onDisposeCts = new CancellationTokenSource();

		private bool _isInputActive;

		private int _lastGroupSwitchFrame = -1;

		private int _lastActionFrame = -1000;

		private bool _doorPressAccepted;

		private int _doorPressFrame = -1;

		private bool _isLookRaised;

		private const float MouseMoveThreshold = 5f;

		private readonly bool _debug = true;

		private readonly Color _debugColor = new Color(0.69f, 0.84f, 0f, 1f);

		private InputField _chatInputField = default;

		private InputActionPhase _throttleActionPhase = InputActionPhase.Canceled;

		private InputActionPhase _breakActionPhase = InputActionPhase.Canceled;

		private InputActionPhase _clutchActionPhase = InputActionPhase.Canceled;

		private const float minPedalThreshold = 0.001f;

		private const float maxPedalThreshold = 0.999f;

		private bool _cursorOnlyActive;

		private bool _enabledPointerMap;

		private PendingMapToggle _pendingUiPointerToggle;

		private bool _insideInputUpdate;

		private readonly Dictionary<InputMapEnum, bool> _pendingMapToggles = new Dictionary<InputMapEnum, bool>();

		private bool _wasApplicationFocused = true;

		private const int CursorEngageWindowFrames = 12;

		private CancellationTokenSource _cursorEngageCts;

		private float _scrollRepeatTimer;

		private bool _scrollFirstFire;

		private const float SCROLL_INITIAL_DELAY = 0.4f;

		private const float SCROLL_REPEAT_INTERVAL = 0.1f;

		private const float CarStateTrackWindowSeconds = 15f;

		private const float CarStateSettleDelaySeconds = 0.5f;

		private CancellationTokenSource _carStateTrackingCts;

		private readonly HashSet<InputGroupsEnum> _paintInputGroups = new HashSet<InputGroupsEnum>
		{
			InputGroupsEnum.PaintSelection,
			InputGroupsEnum.PaintMultiSelection,
			InputGroupsEnum.PaintMultiDone,
			InputGroupsEnum.PaintSelectedOne,
			InputGroupsEnum.PaintColor,
			InputGroupsEnum.PaintSize,
			InputGroupsEnum.PaintText,
			InputGroupsEnum.PaintPlates,
			InputGroupsEnum.PaintBase
		};

		private const float MaxStickDeadzone = 0.95f;

		private InputActionReference _moveReference;

		private InputActionReference _cancelReference;

		private InputActionReference _pointReference;

		private InputActionReference _leftClickReference;

		private InputActionReference _scrollWheelReference;

		private InputActionReference _rightClickReference;

		private InputActionReference _middleClickReference;

		private bool _lmbDragOnScroll;

		private readonly List<RaycastResult> _lookScrollRaycast = new List<RaycastResult>();

		private float _mouseSteerPosition;

		private bool _mouseSteerHeld;

		private const float MouseSteerOverrideEpsilon = 0.0001f;

		private const uint MOUSEEVENTF_MOVE = 1u;

		public InputActionAsset InputAsset => _controls.asset;

		private void ShowCursorOnly(InputAction.CallbackContext ctx)
		{
			if (!IsApplicationFocused() || Time.frameCount == _lastGroupSwitchFrame || _inputMapSetEvent.CurrentInputGroup == InputGroupsEnum.None)
			{
				return;
			}
			if (Cursor.visible && Cursor.lockState != CursorLockMode.Locked)
			{
				_ = _debug;
				return;
			}
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.Confined;
			if (!_inputMapSetEvent.TryGetCurrentMaps(out var maps) || !maps.Contains(InputMapEnum.UiPointer))
			{
				RequestUiPointerMapToggle(enable: true);
				_enabledPointerMap = true;
			}
			_cursorOnlyActive = true;
			ResetAllLook();
			_ = _debug;
		}

		private void StopCursorOnly(InputAction.CallbackContext ctx)
		{
			ForceStopCursorOnly();
		}

		private void ForceStopCursorOnly()
		{
			if (!_cursorOnlyActive)
			{
				_ = _debug;
				return;
			}
			if (_enabledPointerMap)
			{
				RequestUiPointerMapToggle(enable: false);
				_enabledPointerMap = false;
			}
			_cursorOnlyActive = false;
			ApplyCursorStateForCurrentMaps();
			_ = _debug;
		}

		private void OnApplicationFocusChanged(bool focused)
		{
			if (!focused)
			{
				ForceStopCursorOnly();
			}
			else if (Keyboard.current != null)
			{
				InputSystem.TrySyncDevice(Keyboard.current);
			}
		}

		private void ReapplyCursorOnly()
		{
			if (!_inputMapSetEvent.TryGetCurrentMaps(out var maps) || !maps.Contains(InputMapEnum.UiPointer))
			{
				RequestUiPointerMapToggle(enable: true);
				_enabledPointerMap = true;
			}
			else
			{
				_enabledPointerMap = false;
			}
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.Confined;
		}

		private void RequestUiPointerMapToggle(bool enable)
		{
			_pendingUiPointerToggle = (enable ? PendingMapToggle.Enable : PendingMapToggle.Disable);
		}

		private bool IsSteeringWheel(InputAction.CallbackContext ctx)
		{
			if (!_steeringWheelValueEvent.StoredValue)
			{
				return false;
			}
			InputDevice device;
			try
			{
				device = ctx.control.device;
			}
			catch
			{
				return false;
			}
			return IsDeviceSteeringWheel(device);
		}

		private bool IsDeviceSteeringWheel(InputDevice device)
		{
			if (device == null)
			{
				return false;
			}
			device.name?.ToLower();
			WheelKind wheelKind = WheelClassifier.Classify(WheelDeviceInfo.From(device));
			bool flag = wheelKind == WheelKind.G25 || wheelKind == WheelKind.Generic;
			bool flag2 = wheelKind == WheelKind.Moza;
			if (flag2 && !flag)
			{
				flag2 = MozaWrapper.IsBaseConnected();
			}
			if (flag | flag2)
			{
				_ = _debug;
				return true;
			}
			_ = _debug;
			return false;
		}

		private bool CheckActionPhase(float value, ref InputActionPhase lastPhase)
		{
			bool result = false;
			switch (lastPhase)
			{
			case InputActionPhase.Started:
				if (value < 0.001f)
				{
					result = true;
					lastPhase = InputActionPhase.Canceled;
				}
				else if (value > 0.999f)
				{
					result = true;
					lastPhase = InputActionPhase.Performed;
				}
				break;
			case InputActionPhase.Canceled:
				if (value > 0.001f)
				{
					result = true;
					lastPhase = InputActionPhase.Started;
				}
				break;
			case InputActionPhase.Performed:
				if (value < 0.001f)
				{
					result = true;
					lastPhase = InputActionPhase.Canceled;
				}
				break;
			default:
				return false;
			}
			return result;
		}

		private void LogActionWithMessage(InputAction.CallbackContext ctx, string message = "")
		{
			_ = _debug;
		}

		private void StartCancelUiThrottle(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				bool started = ctx.started;
				_onThrottleHandle.Get(_eventRegistry)?.Raise(started);
			}
		}

		private void HeadlightsOnPerformed(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_onHeadlightsHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void OnStartEngine(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				bool canceled = ctx.canceled;
				_onStartEngineHandle.Get(_eventRegistry)?.Raise(canceled);
			}
		}

		private void OnHandbrakeHold(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				bool started = ctx.started;
				_onHandbrakeHoldHandle.Get(_eventRegistry)?.Raise(started);
			}
		}

		private void OnHandbrake(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				bool started = ctx.started;
				_onHandbrakeHandle.Get(_eventRegistry)?.Raise(started);
			}
		}

		private void OnHorn(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				bool started = ctx.started;
				_onHornHandle.Get(_eventRegistry)?.Raise(started);
			}
		}

		private void ShiftUp(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_shifterUpDownHandle.Get(_eventRegistry)?.Raise(value: true);
			}
		}

		private void ShiftDown(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_shifterUpDownHandle.Get(_eventRegistry)?.Raise(value: false);
			}
		}

		private void ShiftM(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_shifterMAHandle.Get(_eventRegistry)?.Raise(value: true);
			}
		}

		private void ShiftA(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_shifterMAHandle.Get(_eventRegistry)?.Raise(value: false);
			}
		}

		private void ToggleShiftMA(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_lastGroupSwitchFrame = Time.frameCount;
				_shiftToggleMAHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void StartStopLookBack(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				bool started = ctx.started;
				_startStopLookBackHandle.Get(_eventRegistry)?.Raise(started);
			}
		}

		private void StartStopLookLeft(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				bool started = ctx.started;
				_startStopLookLeftHandle.Get(_eventRegistry)?.Raise(started);
			}
		}

		private void StartStopLookRight(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				bool started = ctx.started;
				_startStopLookRightHandle.Get(_eventRegistry)?.Raise(started);
			}
		}

		private void ToggleLeftSignal(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_toggleLeftSignalHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void ToggleRightSignal(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_toggleRightSignalHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void ToggleEmergencySignal(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_toggleEmergencySignalHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void TriggerToRoad(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_toRoadHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void AimAt(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked() && FlagsStorage.IsMultiFree)
			{
				bool started = ctx.started;
				_aimAtHandle.Get(_eventRegistry)?.Raise(started);
			}
		}

		private void CameraSettings(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				StampActionFrame();
				_toggleCameraSettingsHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void PhotoMode(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				StampActionFrame();
				_openPhotoModeHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void Timer(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				StampActionFrame();
				_toggleTimerHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void Burnout(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				bool started = ctx.started;
				_startStopBurnoutHandle.Get(_eventRegistry)?.Raise(started);
			}
		}

		private void KeepLane(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_keepLaneHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void BackToCar(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				StampActionFrame();
				_backToCarHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void Restart(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_restartRaceHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void CancelRace(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_cancelRaceHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void SetGear1(InputAction.CallbackContext ctx)
		{
			if (_isInputActive && !_cursorOnlyActive)
			{
				_gearHandle.Get(_eventRegistry)?.Raise(GearEnum.G1);
			}
		}

		private void SetGear2(InputAction.CallbackContext ctx)
		{
			if (_isInputActive && !_cursorOnlyActive)
			{
				_gearHandle.Get(_eventRegistry)?.Raise(GearEnum.G2);
			}
		}

		private void SetGear3(InputAction.CallbackContext ctx)
		{
			if (_isInputActive && !_cursorOnlyActive)
			{
				_gearHandle.Get(_eventRegistry)?.Raise(GearEnum.G3);
			}
		}

		private void SetGear4(InputAction.CallbackContext ctx)
		{
			if (_isInputActive && !_cursorOnlyActive)
			{
				_gearHandle.Get(_eventRegistry)?.Raise(GearEnum.G4);
			}
		}

		private void SetGear5(InputAction.CallbackContext ctx)
		{
			if (_isInputActive && !_cursorOnlyActive)
			{
				_gearHandle.Get(_eventRegistry)?.Raise(GearEnum.G5);
			}
		}

		private void SetGear6(InputAction.CallbackContext ctx)
		{
			if (_isInputActive && !_cursorOnlyActive)
			{
				_gearHandle.Get(_eventRegistry)?.Raise(GearEnum.G6);
			}
		}

		private void SetGear7(InputAction.CallbackContext ctx)
		{
			if (_isInputActive && !_cursorOnlyActive)
			{
				_gearHandle.Get(_eventRegistry)?.Raise(GearEnum.G7);
			}
		}

		private void SetGearN(InputAction.CallbackContext ctx)
		{
			if (_isInputActive && !_cursorOnlyActive)
			{
				_gearHandle.Get(_eventRegistry)?.Raise(GearEnum.N);
			}
		}

		private void SetGearR(InputAction.CallbackContext ctx)
		{
			if (_isInputActive && !_cursorOnlyActive)
			{
				_gearHandle.Get(_eventRegistry)?.Raise(GearEnum.R);
			}
		}

		private void SetGearUp(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_shifterUpDownHandle.Get(_eventRegistry)?.Raise(value: true);
			}
		}

		private void SetGearDown(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_shifterUpDownHandle.Get(_eventRegistry)?.Raise(value: false);
			}
		}

		private void SetGearsVisible(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				InputGroupsEnum currentInputGroup = _inputMapSetEvent.CurrentInputGroup;
				if (currentInputGroup != InputGroupsEnum.CarCircleMenu && currentInputGroup != InputGroupsEnum.CarFreeRaceCircleMenu)
				{
					_shifterMoveStartedHandle.Get(_eventRegistry)?.Raise(value: true);
					ResetLookNextFrame().Forget();
				}
			}
		}

		private void SetGearsInvisible(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_shifterMoveStartedHandle.Get(_eventRegistry)?.Raise(value: false);
				ResetLookNextFrame().Forget();
			}
		}

		private async UniTask ResetLookNextFrame()
		{
			await UniTask.DelayFrame(5, PlayerLoopTiming.Update, _onDisposeCts.Token);
			_lookHandle.Get(_eventRegistry)?.Reset();
			_zoomHandle.Get(_eventRegistry)?.Reset();
			ReleaseLook();
			_lookFreeHandle.Get(_eventRegistry)?.Reset();
			_onLookFreeHandle.Get(_eventRegistry)?.Raise(value: false);
		}

		private void ResetAllLook()
		{
			_lookHandle.Get(_eventRegistry)?.Reset();
			_zoomHandle.Get(_eventRegistry)?.Reset();
			ReleaseLook();
			_lookFreeHandle.Get(_eventRegistry)?.Reset();
			_onLookFreeHandle.Get(_eventRegistry)?.Raise(value: false);
			_lookVinylHandle.Get(_eventRegistry)?.Reset();
			_zoomVinylHandle.Get(_eventRegistry)?.Reset();
		}

		private void OnChangeCamera(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_changeCameraHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void OnLook(InputAction.CallbackContext ctx)
		{
			bool flag = IsLookActive(ctx);
			if (flag == _isLookRaised)
			{
				return;
			}
			if (flag)
			{
				if (IsInputBlocked())
				{
					return;
				}
				if (!IsApplicationFocused())
				{
					InputDevice device = ctx.control.device;
					if (device is Mouse || device is Keyboard)
					{
						return;
					}
				}
				if (_clutchHandle.Get(_eventRegistry).Value > 0.5f || _lmbDragOnScroll)
				{
					return;
				}
			}
			_isLookRaised = flag;
			_onLookHandle.Get(_eventRegistry)?.Raise(flag);
		}

		private bool IsLookActive(InputAction.CallbackContext ctx)
		{
			if (ctx.canceled)
			{
				return false;
			}
			if (!(ctx.control.device is Gamepad))
			{
				return true;
			}
			if (IsShifterGesture())
			{
				return false;
			}
			float deadzone = (_controlsSettings ? _controlsSettings.StickDeadzone(left: false) : 0f);
			return ApplyStickDeadzone(ctx.ReadValue<Vector2>(), deadzone) != Vector2.zero;
		}

		private void ReleaseLook()
		{
			_isLookRaised = false;
			_onLookHandle.Get(_eventRegistry)?.Raise(value: false);
		}

		private void OnLookFree(InputAction.CallbackContext ctx)
		{
			if (_lastGroupSwitchFrame != Time.frameCount && !(_clutchHandle.Get(_eventRegistry).Value > 0.5f))
			{
				InputDevice device = ctx.control.device;
				bool started = ctx.started;
				if (device is Gamepad)
				{
					_onLookFreeHandle.Get(_eventRegistry)?.Raise(started);
				}
				else if (device is Mouse && IsApplicationFocused())
				{
					started = !IsMouseSteerHeld() && ctx.ReadValue<Vector2>().magnitude > 5f;
					_onLookFreeHandle.Get(_eventRegistry)?.Raise(started);
				}
			}
		}

		private void CursorOnConfirmClick(InputAction.CallbackContext ctx)
		{
			if (!IsApplicationFocused())
			{
				InputDevice device = ctx.control.device;
				if (device is Mouse || device is Keyboard)
				{
					return;
				}
			}
			bool value = ctx.ReadValueAsButton();
			_onConfirmClickHandle.Get(_eventRegistry)?.Raise(value);
		}

		private void StartCursorMoveWithStick(InputAction.CallbackContext ctx)
		{
			if (_lastGroupSwitchFrame != Time.frameCount && !(_uiNavigationValueEvent == null) && (ctx.started || _uiNavigationValueEvent.StoredValue != UiNavigationEnum.Stick))
			{
				float deadzone = (_controlsSettings ? _controlsSettings.StickDeadzone(left: true) : 0f);
				if (!(ApplyStickDeadzone(ctx.ReadValue<Vector2>(), deadzone) == Vector2.zero))
				{
					_uiNavigationValueEvent.Raise(UiNavigationEnum.Stick);
				}
			}
		}

		private void StartCursorMoveWithKeys(InputAction.CallbackContext ctx)
		{
			if (_lastGroupSwitchFrame != Time.frameCount)
			{
				_uiNavigationValueEvent?.Raise(UiNavigationEnum.Keys);
			}
		}

		private void CheckMouseMove(InputAction.CallbackContext ctx)
		{
			if (_lastGroupSwitchFrame != Time.frameCount && _inputMapSetEvent.TryGetCurrentMaps(out var maps) && maps.Contains(InputMapEnum.UiPointer))
			{
				InputDevice device = ctx.control.device;
				if (IsApplicationFocused() && device is Mouse && ctx.ReadValue<Vector2>().magnitude > 5f)
				{
					_uiNavigationValueEvent?.Raise(UiNavigationEnum.Mouse);
				}
				else if (device is Gamepad)
				{
					_uiNavigationValueEvent?.Raise(UiNavigationEnum.Stick);
				}
			}
		}

		private void OnInteractCalled(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_interactHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void OnDoorCanceled(InputAction.CallbackContext ctx)
		{
			if (_interactHandle.Get(_eventRegistry).AnySubscribers())
			{
				return;
			}
			if (!_doorPressAccepted)
			{
				_ = _debug;
				return;
			}
			_doorPressAccepted = false;
			if (_lastGroupSwitchFrame == Time.frameCount)
			{
				_ = _debug;
				return;
			}
			_ = _debug;
			BeginCarStateTracking();
			StampActionFrame();
			_doorStartCancelHandle.Get(_eventRegistry)?.Raise(value: false);
		}

		private void OnDoorStart(InputAction.CallbackContext ctx)
		{
			if (_interactHandle.Get(_eventRegistry).AnySubscribers())
			{
				return;
			}
			if (IsInputBlocked())
			{
				_ = _debug;
				return;
			}
			_doorPressAccepted = true;
			_doorPressFrame = Time.frameCount;
			_ = _debug;
			StampActionFrame();
			_doorStartCancelHandle.Get(_eventRegistry)?.Raise(value: true);
			FloatVariable floatVariable = _clutchHandle.Get(_eventRegistry);
			float num = 0f;
			if ((bool)floatVariable)
			{
				num = floatVariable.Value;
			}
			if (num > 0f)
			{
				_closeCircleMenuHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void OnVoice(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked() && PlayerPrefs.GetInt("VoiceChat", 1) == 1)
			{
				bool started = ctx.started;
				_onVoiceHandle.Get(_eventRegistry)?.Raise(started);
			}
		}

		private void OpenMenu(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked() && (!(ctx.control.device is Keyboard) || !_cancelHandle.Get(_eventRegistry).AnyOtherSubscribers(_cancelEventDefaults)))
			{
				StampActionFrame();
				_onMenuHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void OpenPhone(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				StampActionFrame();
				_openPhoneHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void OnConfirm(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				if ((bool)_chatInputField && UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != _chatInputField.gameObject)
				{
					UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
					UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(_chatInputField.gameObject);
				}
				StampActionFrame();
				_lastGroupSwitchFrame = Time.frameCount;
				_acceptHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void OnConfirmHold(InputAction.CallbackContext ctx)
		{
			if (!(ctx.control.device is Gamepad))
			{
				bool started = ctx.started;
				if (!started || !IsInputBlocked())
				{
					_acceptHoldHandle.Get(_eventRegistry)?.Raise(started);
				}
			}
		}

		private void OnCancel(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked() && (bool)UnityEngine.EventSystems.EventSystem.current)
			{
				GameObject currentSelectedGameObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
				if (!(currentSelectedGameObject != null) || !currentSelectedGameObject.TryGetComponent<TMP_InputField>(out var component) || !component.isFocused)
				{
					DeferredCancelAsync().Forget();
				}
			}
		}

		private async UniTaskVoid DeferredCancelAsync()
		{
			await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
			if (!IsInputBlocked())
			{
				if (_paintInputGroups.Contains(_inputMapSetEvent.CurrentInputGroup))
				{
					_onUnselectOrBackHandle.Get(_eventRegistry)?.Raise();
					return;
				}
				StampActionFrame();
				_lastGroupSwitchFrame = Time.frameCount;
				_cancelHandle.Get(_eventRegistry)?.Raise();
				_closeCircleMenuHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void OnMapMove(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				bool started = ctx.started;
				_onMapMoveHandle.Get(_eventRegistry)?.Raise(started);
			}
		}

		private void OnMapMouseDrag(InputAction.CallbackContext ctx)
		{
			bool started = ctx.started;
			if ((!started || !IsInputBlocked()) && !started)
			{
				_onMapMoveHandle.Get(_eventRegistry)?.Raise(value: false);
			}
		}

		private void OnToggleMapOn(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				StampActionFrame();
				_toggleMapHandle.Get(_eventRegistry)?.Raise(value: true);
			}
		}

		private void OnToggleMapOff(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				StampActionFrame();
				_toggleMapHandle.Get(_eventRegistry)?.Raise(value: false);
			}
		}

		private void ShowPopUpMap(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_showPupUpOnMapHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void ExpandHideMapOptions(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_uiNavigationValueEvent?.Raise(UiNavigationEnum.Keys);
				_toggleShowHideMapFiltersHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void ToggleFilterOnMap(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_uiNavigationValueEvent?.Raise(UiNavigationEnum.Keys);
				_toggleFilterOnMapHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void HideShowAllFilters(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_uiNavigationValueEvent?.Raise(UiNavigationEnum.Keys);
				_hideShowAllFiltersHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void MarkOnMap(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_markOnMapHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void Snapshot(InputAction.CallbackContext ctx)
		{
			if (IsInputBlocked())
			{
				return;
			}
			if (!IsApplicationFocused())
			{
				InputDevice device = ctx.control.device;
				if (device is Mouse || device is Keyboard)
				{
					return;
				}
			}
			_onSnapshotHandle.Get(_eventRegistry)?.Raise();
		}

		private void StartStopCameraMove(InputAction.CallbackContext ctx)
		{
			if (IsInputBlocked())
			{
				return;
			}
			if (!IsApplicationFocused())
			{
				InputDevice device = ctx.control.device;
				if (device is Mouse || device is Keyboard)
				{
					return;
				}
			}
			bool value = (double)ctx.ReadValue<Vector2>().sqrMagnitude > 0.001;
			_onCameraMoveHandle.Get(_eventRegistry)?.Raise(value);
		}

		private void StartStopCameraHeight(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				bool started = ctx.started;
				_onCameraHeightHandle.Get(_eventRegistry)?.Raise(started);
			}
		}

		private void PersonCrouch(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_personCrouchHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void PersonJump(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_personJumpHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void PersonWalkRunToggle(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_personWalkRunToggleHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void StartStopCameraTilt(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				bool started = ctx.started;
				_onCameraTiltHandle.Get(_eventRegistry)?.Raise(started);
			}
		}

		private void ResetCamera(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_onResetDroneCameraHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void ResetFilter(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_onResetFilterHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void ToggleFocus(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_toggleFocusHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void CloseDrone(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				StampActionFrame();
				_closeDroneHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void CenterDroneCamera(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_onCenterDroneCameraHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void HideDroneUi(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_toggleUiHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void SpeedToggle(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_toggleDroneSpeedHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void OpenChat(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked() && !WasActionJustTriggered())
			{
				StampActionFrame();
				_lastGroupSwitchFrame = Time.frameCount;
				bool value = ctx.control.device is Keyboard;
				_onOpenChatHandle.Get(_eventRegistry)?.Raise(value);
			}
		}

		private void PhotoCamera(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_onPhotoCameraHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void PhotoColor(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_onPhotoColorHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void PhotoHelp(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_onPhotoHelpHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void PhotoOptica(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_onPhotoOpticsHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void PhotoNavigateSettings(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				bool value = ctx.ReadValue<float>() > 0f;
				_uiNavigationValueEvent?.Raise(UiNavigationEnum.Keys);
				_navigatePhotoSettingsHandle.Get(_eventRegistry)?.Raise(value);
			}
		}

		private void PhotoToggles(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				bool value = ctx.ReadValue<float>() > 0f;
				_uiNavigationValueEvent?.Raise(UiNavigationEnum.Keys);
				_setPhotoSettingsHandle.Get(_eventRegistry)?.Raise(value);
			}
		}

		private static bool IsTextFieldFocused()
		{
			UnityEngine.EventSystems.EventSystem current = UnityEngine.EventSystems.EventSystem.current;
			if (!current)
			{
				return false;
			}
			GameObject currentSelectedGameObject = current.currentSelectedGameObject;
			if (!currentSelectedGameObject)
			{
				return false;
			}
			if (currentSelectedGameObject.TryGetComponent<TMP_InputField>(out var component) && component.isFocused)
			{
				return true;
			}
			if (currentSelectedGameObject.TryGetComponent<InputField>(out var component2))
			{
				return component2.isFocused;
			}
			return false;
		}

		private void Next(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked() && !IsTextFieldFocused())
			{
				_uiNavigationValueEvent.Raise(UiNavigationEnum.Keys);
				_onNextPreviousHandle.Get(_eventRegistry)?.Raise(value: true);
			}
		}

		private void Previous(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked() && !IsTextFieldFocused())
			{
				_uiNavigationValueEvent.Raise(UiNavigationEnum.Keys);
				_onNextPreviousHandle.Get(_eventRegistry)?.Raise(value: false);
			}
		}

		private void BlockCamera(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_blockCameraHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void DeleteAll(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_deleteAllHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void DeleteSelected(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_deleteSelectedHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void FavoritesToggle(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_toggleFavoritesHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void ToggleUi(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_toggleUiHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void SetColorCode(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_setColorHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void ColorToggle(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_onStartColorMoveHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void TintToggle(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_onStartTintMoveHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void SetSize(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_setSizeHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void Mirror(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_mirrorHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void Copy(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_copyHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void Favorite(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_addFavoriteHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void MultiSelect(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_toggleMultiSelectHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void CancelChange(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_onCancelChangesHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void ToggleFrontPlate(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_onToggleFrontPlateHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void ToggleBackPlate(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_onToggleBackPlateHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void EnterPlateTypes(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_onEnterPlateTypesHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void ToggleToSize(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_toggleToSizeHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void ToggleToColor(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_toggleToColorHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void StopScroll(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_uiNavigationValueEvent?.Raise(UiNavigationEnum.Keys);
				_scrollValueEventHandle.Get(_eventRegistry)?.Raise(ScrollEnum.None);
				_scrollFirstFire = true;
				_scrollRepeatTimer = 0.4f;
			}
		}

		private void ScrollLeftRight(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_uiNavigationValueEvent?.Raise(UiNavigationEnum.Keys);
				ScrollEnum value = ((!ctx.canceled) ? ((ctx.ReadValue<float>() > 0f) ? ScrollEnum.Right : ScrollEnum.Left) : ScrollEnum.None);
				_scrollValueEventHandle.Get(_eventRegistry)?.Raise(value);
				_scrollFirstFire = true;
				_scrollRepeatTimer = 0.4f;
			}
		}

		private void ScrollUpDown(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_uiNavigationValueEvent?.Raise(UiNavigationEnum.Keys);
				ScrollEnum value = ((!ctx.canceled) ? ((ctx.ReadValue<float>() > 0f) ? ScrollEnum.Up : ScrollEnum.Down) : ScrollEnum.None);
				_scrollValueEventHandle.Get(_eventRegistry)?.Raise(value);
				_scrollFirstFire = true;
				_scrollRepeatTimer = 0.4f;
			}
		}

		private void OnUiNavigatePerformed(InputAction.CallbackContext ctx)
		{
			if (IsInputBlocked())
			{
				return;
			}
			ScrollEnum scrollEnum = ScrollEnum.None;
			Vector2 vector = ctx.ReadValue<Vector2>();
			float x = vector.x;
			ScrollEnum scrollEnum2;
			if (!(x > 0f))
			{
				if (!(x < 0f))
				{
					if (x != 0f)
					{
						goto IL_0078;
					}
					if (vector.y > 0f)
					{
						scrollEnum2 = ScrollEnum.Up;
					}
					else
					{
						if (!(vector.y < 0f))
						{
							goto IL_0078;
						}
						scrollEnum2 = ScrollEnum.Down;
					}
				}
				else
				{
					if (vector.y != 0f)
					{
						goto IL_0078;
					}
					scrollEnum2 = ScrollEnum.Right;
				}
			}
			else
			{
				if (vector.y != 0f)
				{
					goto IL_0078;
				}
				scrollEnum2 = ScrollEnum.Left;
			}
			goto IL_007a;
			IL_007a:
			scrollEnum = scrollEnum2;
			_uiNavigationValueEvent?.Raise(UiNavigationEnum.Keys);
			_scrollValueEventHandle.Get(_eventRegistry)?.Raise(scrollEnum);
			_scrollFirstFire = true;
			_scrollRepeatTimer = 0.4f;
			return;
			IL_0078:
			scrollEnum2 = scrollEnum;
			goto IL_007a;
		}

		private void LayerUpDown(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				bool value = ctx.ReadValue<float>() > 0f;
				_onLayerUpDownHandle.Get(_eventRegistry)?.Raise(value);
			}
		}

		private void ToggleToText(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_toggleToTextHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void ToggleBold(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_toggleBoldHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void ToggleItalics(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_toggleItalicsHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void EnterText(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				_onSetTextHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void FontUpDown(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				bool value = ctx.ReadValue<float>() > 0f;
				_onFontUpDownHandle.Get(_eventRegistry)?.Raise(value);
			}
		}

		private void ToggleCircleMenu(InputAction.CallbackContext ctx)
		{
			if (_isInputActive)
			{
				bool value = ctx.control.device is Gamepad;
				if (Time.frameCount - _lastGroupSwitchFrame > 1)
				{
					StampActionFrame();
					_circleMenuHandle.Get(_eventRegistry)?.Raise(value);
					ResetLookNextFrame().Forget();
				}
			}
		}

		private void ToggleCircleMenuCam(InputAction.CallbackContext ctx)
		{
			if (_isInputActive)
			{
				bool value = ctx.control.device is Gamepad;
				if (Time.frameCount - _lastGroupSwitchFrame > 1)
				{
					StampActionFrame();
					_onCircleMenuCamHandle.Get(_eventRegistry)?.Raise(value);
					ResetLookNextFrame().Forget();
				}
			}
		}

		private void ToggleCircleMenuAnim(InputAction.CallbackContext ctx)
		{
			if (_isInputActive && FlagsStorage.OutOfCar)
			{
				bool value = ctx.control.device is Gamepad;
				if (Time.frameCount - _lastGroupSwitchFrame > 1)
				{
					StampActionFrame();
					_onCircleMenuAnimHandle.Get(_eventRegistry)?.Raise(value);
					ResetLookNextFrame().Forget();
				}
			}
		}

		private void OnCircleMenuScroll(InputAction.CallbackContext ctx)
		{
			if (!IsInputBlocked())
			{
				bool value = ctx.ReadValue<float>() > 0f;
				_circleMenuScrollHandle.Get(_eventRegistry)?.Raise(value);
			}
		}

		private void CloseCircleMenu(InputAction.CallbackContext ctx)
		{
			if (Time.frameCount - _lastGroupSwitchFrame > 1 && ctx.ReadValue<float>() > 0f)
			{
				_closeCircleMenuHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void CloseCircleMenuCam(InputAction.CallbackContext ctx)
		{
			if (Time.frameCount - _lastGroupSwitchFrame > 1 && ctx.ReadValue<float>() > 0f)
			{
				_closeCircleMenuCamHandle.Get(_eventRegistry)?.Raise();
			}
		}

		[Inject]
		public void Construct(EventRegistry eventRegistry, InputMapSetEvent inputMapSetEvent, UiNavigationValueEvent uiNavigationValueEvent, CurrentDeviceValueEvent currentDeviceValueEvent, SteeringWheelValueEvent steeringWheelValueEvent, DesktopControlsSettingsSO controlsSettingsAsset)
		{
			_eventRegistry = eventRegistry;
			_inputMapSetEvent = inputMapSetEvent;
			_uiNavigationValueEvent = uiNavigationValueEvent;
			_currentDeviceValueEvent = currentDeviceValueEvent;
			_steeringWheelValueEvent = steeringWheelValueEvent;
			_controlsSettings = controlsSettingsAsset;
		}

		private void SetInput(InputGroupsEnum inputGroup)
		{
			_lastGroupSwitchFrame = Time.frameCount;
			if (_doorPressFrame != Time.frameCount)
			{
				_doorPressAccepted = false;
			}
			TryAssignInputModule();
			CheckAllChangedMaps();
			SwitchMaps();
			if (_cursorOnlyActive)
			{
				ReapplyCursorOnly();
				return;
			}
			ApplyCursorStateForCurrentMaps();
			BeginCursorEngageWindow();
		}

		private void CheckAllChangedMaps()
		{
			foreach (InputMapEnum oldMap in _inputMapSetEvent.GetOldMaps())
			{
				CheckPreviousMap(oldMap);
			}
			foreach (InputMapEnum newMap in _inputMapSetEvent.GetNewMaps())
			{
				CheckCurrentMap(newMap);
			}
		}

		private void CheckPreviousMap(InputMapEnum inputMap)
		{
			if (inputMap == InputMapEnum.UI || inputMap == InputMapEnum.MapUi)
			{
				if ((bool)_lastInputModule)
				{
					_lastInputModule.move = null;
				}
				UISelectionCursor.HideCursor();
			}
		}

		private void CheckCurrentMap(InputMapEnum inputMap)
		{
			if (inputMap == InputMapEnum.UI || inputMap == InputMapEnum.MapUi)
			{
				if ((bool)_lastInputModule)
				{
					_lastInputModule.move = _moveReference;
				}
				UiNavigationEnum storedValue = _uiNavigationValueEvent.StoredValue;
				if (storedValue == UiNavigationEnum.Keys || storedValue == UiNavigationEnum.Stick)
				{
					UISelectionCursor.ShowCursor();
				}
			}
		}

		private void SwitchMaps()
		{
			List<InputMapEnum> mapsToDisable = _inputMapSetEvent.GetMapsToDisable();
			List<InputMapEnum> mapsToEnable = _inputMapSetEvent.GetMapsToEnable();
			_ = _debug;
			if (mapsToEnable.Contains(InputMapEnum.CarBase) && !mapsToEnable.Contains(InputMapEnum.UI))
			{
				Cursor.lockState = CursorLockMode.Locked;
			}
			else
			{
				Cursor.lockState = CursorLockMode.Confined;
			}
			bool resetHandbrake = !mapsToEnable.Contains(InputMapEnum.UI) || !mapsToDisable.Contains(InputMapEnum.CarBase);
			foreach (InputMapEnum item in mapsToDisable)
			{
				CleanupDisabledMap(item, resetHandbrake);
			}
			foreach (InputMapEnum item2 in mapsToEnable)
			{
				SetupEnabledMap(item2);
			}
		}

		private void OnCasualModeChanged(bool isCasual)
		{
			ToggleShifterForCasualMode(isCasual);
		}

		private void ToggleShifterForCasualMode(bool isCasual)
		{
			if (_inputMapSetEvent.TryGetCurrentMaps(out var maps) && maps.Contains(InputMapEnum.Shifter))
			{
				ToggleMapSafe(InputMapEnum.Shifter, !isCasual);
			}
		}

		private void OnBeforeInputUpdate()
		{
			_insideInputUpdate = true;
		}

		private void OnAfterInputUpdate()
		{
			_insideInputUpdate = false;
		}

		private void ToggleMapSafe(InputMapEnum inputMap, bool toggleOn)
		{
			if (_insideInputUpdate)
			{
				_pendingMapToggles[inputMap] = toggleOn;
				return;
			}
			_pendingMapToggles.Remove(inputMap);
			ToggleMap(inputMap, toggleOn);
		}

		private void SetupEnabledMap(InputMapEnum inputMap)
		{
			if (inputMap != InputMapEnum.Shifter || !FlagsStorage.IsCasualControl)
			{
				ToggleMapSafe(inputMap, toggleOn: true);
			}
		}

		private void CleanupDisabledMap(InputMapEnum previousMap, bool resetHandbrake)
		{
			switch (previousMap)
			{
			case InputMapEnum.SteeringWheelCar:
				_gearHandle.Get(_eventRegistry)?.Reset();
				break;
			case InputMapEnum.UiPointer:
				_stickCursorMoveHandle.Get(_eventRegistry)?.Reset();
				break;
			case InputMapEnum.Camera:
				_lookHandle.Get(_eventRegistry)?.Reset();
				_zoomHandle.Get(_eventRegistry)?.Reset();
				ReleaseLook();
				_lookFreeHandle.Get(_eventRegistry)?.Reset();
				_onLookFreeHandle.Get(_eventRegistry)?.Raise(value: false);
				_movePersonHandle.Get(_eventRegistry)?.Reset();
				_personRotateRightLeftHandle.Get(_eventRegistry)?.Reset();
				_personRunHandle.Get(_eventRegistry)?.Reset();
				break;
			case InputMapEnum.PaintBase:
				_lookHandle.Get(_eventRegistry)?.Reset();
				_zoomHandle.Get(_eventRegistry)?.Reset();
				ReleaseLook();
				_lookFreeHandle.Get(_eventRegistry)?.Reset();
				_onLookFreeHandle.Get(_eventRegistry)?.Raise(value: false);
				_lookVinylHandle.Get(_eventRegistry)?.Reset();
				_zoomVinylHandle.Get(_eventRegistry)?.Reset();
				break;
			case InputMapEnum.DroneCamera:
				_lookHandle.Get(_eventRegistry)?.Reset();
				_zoomHandle.Get(_eventRegistry)?.Reset();
				ReleaseLook();
				_lookFreeHandle.Get(_eventRegistry)?.Reset();
				_onLookFreeHandle.Get(_eventRegistry)?.Raise(value: false);
				_droneCameraMoveHandle.Get(_eventRegistry)?.Reset();
				_droneCameraHeightHandle.Get(_eventRegistry)?.Reset();
				_droneCameraTiltHandle.Get(_eventRegistry)?.Reset();
				break;
			case InputMapEnum.CircleMenu:
				_circleMenuMoveHandle.Get(_eventRegistry)?.Reset();
				break;
			case InputMapEnum.Shifter:
				_shifterMoveHandle.Get(_eventRegistry)?.Reset();
				break;
			case InputMapEnum.CarBase:
				_throttleHandle.Get(_eventRegistry)?.Reset();
				_brakeHandle.Get(_eventRegistry)?.Reset();
				_steerHandle.Get(_eventRegistry)?.Reset();
				_mouseSteerPosition = 0f;
				_mouseSteerHeld = false;
				if (resetHandbrake)
				{
					_handbrakeHandle.Get(_eventRegistry)?.Reset();
				}
				_clutchHandle.Get(_eventRegistry)?.Reset();
				break;
			case InputMapEnum.UiThrottle:
				_throttleHandle.Get(_eventRegistry)?.Reset();
				break;
			case InputMapEnum.Person:
				_movePersonHandle.Get(_eventRegistry)?.Reset();
				_personRotateRightLeftHandle.Get(_eventRegistry)?.Reset();
				_personRunHandle.Get(_eventRegistry)?.Reset();
				break;
			case InputMapEnum.MapMove:
				_mapMoveHandle.Get(_eventRegistry)?.Reset();
				_mapDragMoveValueEventHandle.Get(_eventRegistry)?.Reset();
				break;
			case InputMapEnum.PaintSelectionMade:
				_positionChangeHandle.Get(_eventRegistry)?.Reset();
				break;
			case InputMapEnum.PaintSize:
				_sizeChangeHandle.Get(_eventRegistry)?.Reset();
				_turnLeftRightHandle.Get(_eventRegistry)?.Reset();
				break;
			case InputMapEnum.PaintColor:
				_opacityChangeHandle.Get(_eventRegistry)?.Reset();
				_colorChangeHandle.Get(_eventRegistry)?.Reset();
				break;
			}
			ToggleMapSafe(previousMap, toggleOn: false);
		}

		private void TryAssignInputModule()
		{
			UnityEngine.EventSystems.EventSystem current = UnityEngine.EventSystems.EventSystem.current;
			if (!(current == null) && current.TryGetComponent<InputSystemUIInputModule>(out _lastInputModule))
			{
				if ((bool)_controlsSettings)
				{
					_lastInputModule.scrollDeltaPerTick = _controlsSettings.UiScrollDeltaPerTick;
				}
				if (!(current == _lastEventSystem))
				{
					_lastEventSystem = current;
					_lastInputModule.actionsAsset = _controls.asset;
					_lastInputModule.cancel = _cancelReference;
					_lastInputModule.point = _pointReference;
					_lastInputModule.leftClick = _leftClickReference;
					_lastInputModule.scrollWheel = _scrollWheelReference;
					_lastInputModule.rightClick = _rightClickReference;
					_lastInputModule.middleClick = _middleClickReference;
					_lastInputModule.deselectOnBackgroundClick = false;
				}
			}
		}

		private bool IsApplicationFocused()
		{
			return Application.isFocused;
		}

		private Vector2 ApplyStickDeadzone(Vector2 v, float deadzone)
		{
			if (deadzone <= 0f)
			{
				return v;
			}
			deadzone = Mathf.Min(deadzone, 0.95f);
			float magnitude = v.magnitude;
			if (magnitude < deadzone)
			{
				return Vector2.zero;
			}
			float num = (magnitude - deadzone) / (1f - deadzone);
			return v * (num / magnitude);
		}

		private float ApplyStickDeadzone(float val, float deadzone)
		{
			if (deadzone <= 0f)
			{
				return val;
			}
			deadzone = Mathf.Min(deadzone, 0.95f);
			if (val.Abs() < deadzone)
			{
				return 0f;
			}
			float num = (val.Abs() - deadzone) / (1f - deadzone);
			return Mathf.Sign(val) * num;
		}

		public void Start()
		{
			InputSystem.onBeforeUpdate += OnBeforeInputUpdate;
			InputSystem.onAfterUpdate += OnAfterInputUpdate;
			_controls = new GameInputController();
			try
			{
				ChordSlotInstaller.Install(_controls.asset);
			}
			catch (Exception)
			{
			}
			_eventRegistry.Initialize();
			_inputMapSetEvent.Initialize();
			_inputMapSetEvent.Subscribe(SetInput);
			_setActionsActiveHandle.Get(_eventRegistry)?.Subscribe(SetInputActive);
			_isInputActive = true;
			_controls.UiPointer.Enable();
			_controls.Debug.Enable();
			SubscribeToActions();
			_moveReference = InputActionReference.Create(_controls.UI.Navigate);
			_cancelReference = InputActionReference.Create(_controls.UIAcceptCancel.Cancel);
			_pointReference = InputActionReference.Create(_controls.UiPointer.Point);
			_leftClickReference = InputActionReference.Create(_controls.UiPointer.Click);
			_scrollWheelReference = InputActionReference.Create(_controls.UiPointer.ScrollWheel);
			_rightClickReference = InputActionReference.Create(_controls.UiPointer.RightClick);
			_middleClickReference = InputActionReference.Create(_controls.UiPointer.MiddleClick);
			Debug.developerConsoleEnabled = false;
			Debug.developerConsoleVisible = false;
			foreach (InputDevice device in InputSystem.devices)
			{
				_ = device;
			}
			foreach (InputDevice item in InputSystem.devices.Where(IsDeviceSteeringWheel))
			{
				_ = item;
				if (!_steeringWheelValueEvent.StoredValue)
				{
					try
					{
						MozaWrapper.RefreshWheelRange();
					}
					catch (Exception)
					{
					}
					_steeringWheelValueEvent.Raise(value: true);
				}
			}
			MozaWrapper.BaseConnectionChanged += OnMozaBaseConnectionChanged;
			InputSystem.onDeviceChange += OnDeviceChange;
			Application.focusChanged += OnApplicationFocusChanged;
			if ((bool)_controlsSettings)
			{
				_controlsSettings.CasualModeChanged += OnCasualModeChanged;
			}
		}

		private void OnMozaBaseConnectionChanged(bool connected)
		{
			if (connected)
			{
				if (_steeringWheelValueEvent.StoredValue)
				{
					return;
				}
				using IEnumerator<InputDevice> enumerator = InputSystem.devices.Where(IsDeviceSteeringWheel).GetEnumerator();
				if (enumerator.MoveNext())
				{
					_ = enumerator.Current;
					_steeringWheelValueEvent.Raise(value: true);
				}
				return;
			}
			if (_steeringWheelValueEvent.StoredValue && !InputSystem.devices.Any(IsDeviceSteeringWheel))
			{
				_steeringWheelValueEvent.Raise(value: false);
			}
		}

		private void BeginCarStateTracking()
		{
			_ = _debug;
			_carStateTrackingCts?.Cancel();
			_carStateTrackingCts?.Dispose();
			_carStateTrackingCts = CancellationTokenSource.CreateLinkedTokenSource(_onDisposeCts.Token);
			TrackCarStateAfterDoorAsync(_carStateTrackingCts.Token).Forget();
		}

		private async UniTaskVoid TrackCarStateAfterDoorAsync(CancellationToken ct)
		{
			bool startOut = FlagsStorage.OutOfCar;
			bool startPass = FlagsStorage.IsPassenger;
			_ = _debug;
			for (float elapsed = 0f; elapsed < 15f; elapsed += Time.deltaTime)
			{
				if (FlagsStorage.OutOfCar != startOut)
				{
					break;
				}
				if (FlagsStorage.IsPassenger != startPass)
				{
					break;
				}
				await UniTask.Yield(PlayerLoopTiming.Update, ct);
			}
			if (FlagsStorage.OutOfCar == startOut && FlagsStorage.IsPassenger == startPass)
			{
				_ = _debug;
				return;
			}
			_ = _debug;
			await UniTask.Delay(TimeSpan.FromSeconds(0.5), ignoreTimeScale: false, PlayerLoopTiming.Update, _onDisposeCts.Token);
			while (_inputMapSetEvent.CurrentInputGroup != _inputMapSetEvent.GameplayRootGroup)
			{
				await UniTask.Yield(PlayerLoopTiming.Update, _onDisposeCts.Token);
			}
			_ = _debug;
			ApplyInputGroupFromFlags();
		}

		private void ApplyInputGroupFromFlags()
		{
			InputGroupsEnum newGroup = (FlagsStorage.OutOfCar ? InputGroupsEnum.Person : ((!FlagsStorage.IsPassenger) ? ((!FlagsStorage.RacingGameMode) ? InputGroupsEnum.CarFree : (FlagsStorage.SingeDragRacingMode ? InputGroupsEnum.CarDrag : InputGroupsEnum.CarRacing)) : InputGroupsEnum.CarPassenger));
			_ = _debug;
			_inputMapSetEvent.TrySetGroup(newGroup, setRoot: true);
			_ = _debug;
		}

		private void OnDeviceChange(InputDevice inputDevice, InputDeviceChange change)
		{
			_ = _debug;
			switch (change)
			{
			case InputDeviceChange.Added:
			case InputDeviceChange.Reconnected:
				if (IsDeviceSteeringWheel(inputDevice))
				{
					if (!_steeringWheelValueEvent.StoredValue)
					{
						_ = _debug;
						_steeringWheelValueEvent.Raise(value: true);
					}
					else
					{
						_ = _debug;
					}
				}
				_ = _debug;
				break;
			case InputDeviceChange.Disconnected:
				_ = _debug;
				if (!InputSystem.devices.Any((InputDevice device) => IsDeviceSteeringWheel(device) && device.name != inputDevice.name))
				{
					_ = _debug;
					_steeringWheelValueEvent.Raise(value: false);
				}
				break;
			}
		}

		private bool IsInputBlocked()
		{
			if (_lastGroupSwitchFrame != Time.frameCount && _isInputActive)
			{
				return _cursorOnlyActive;
			}
			return true;
		}

		private void StampActionFrame()
		{
			_lastActionFrame = Time.frameCount;
		}

		private bool WasActionJustTriggered()
		{
			if (Time.frameCount - _lastActionFrame > 1)
			{
				return Time.frameCount - _lastGroupSwitchFrame <= 1;
			}
			return true;
		}

		private void SetInputActive(bool enable)
		{
			_isInputActive = enable;
			if (!enable)
			{
				_closeCircleMenuHandle.Get(_eventRegistry)?.Raise();
				_closeCircleMenuCamHandle.Get(_eventRegistry)?.Raise();
			}
		}

		public void Dispose()
		{
			if (_disposed)
			{
				return;
			}
			CancelCursorEngageWindow();
			InputSystem.onDeviceChange -= OnDeviceChange;
			InputSystem.onBeforeUpdate -= OnBeforeInputUpdate;
			InputSystem.onAfterUpdate -= OnAfterInputUpdate;
			_pendingMapToggles.Clear();
			Application.focusChanged -= OnApplicationFocusChanged;
			if ((bool)_controlsSettings)
			{
				_controlsSettings.CasualModeChanged -= OnCasualModeChanged;
			}
			MozaWrapper.BaseConnectionChanged -= OnMozaBaseConnectionChanged;
			if ((bool)_inputMapSetEvent)
			{
				_inputMapSetEvent.Unsubscribe(SetInput);
				_inputMapSetEvent.Reset();
			}
			if ((bool)_uiNavigationValueEvent)
			{
				_uiNavigationValueEvent.Reset();
			}
			if ((bool)_currentDeviceValueEvent)
			{
				_currentDeviceValueEvent.Reset();
			}
			if ((bool)_steeringWheelValueEvent)
			{
				_steeringWheelValueEvent.Reset();
			}
			if (_controls == null)
			{
				return;
			}
			if ((bool)_lastInputModule)
			{
				_lastInputModule.actionsAsset = null;
				_lastInputModule.move = null;
				_lastInputModule.cancel = null;
				_lastInputModule.point = null;
				_lastInputModule.leftClick = null;
				_lastInputModule.scrollWheel = null;
				_lastInputModule.rightClick = null;
				_lastInputModule.middleClick = null;
			}
			UnsubscribeToActions();
			foreach (InputMapEnum value in Enum.GetValues(typeof(InputMapEnum)))
			{
				if (value != InputMapEnum.None)
				{
					ToggleMap(value, toggleOn: false);
				}
			}
			_controls.Disable();
			_controls.Dispose();
			_onDisposeCts.Cancel();
			_onDisposeCts.Dispose();
			_carStateTrackingCts?.Dispose();
			_disposed = true;
			GC.SuppressFinalize(this);
		}

		private void ToggleMap(InputMapEnum inputMap, bool toggleOn)
		{
			switch (inputMap)
			{
			case InputMapEnum.UI:
				if (toggleOn)
				{
					_controls.UI.Enable();
				}
				else
				{
					_controls.UI.Disable();
				}
				break;
			case InputMapEnum.UIAcceptCancel:
				if (toggleOn)
				{
					_controls.UIAcceptCancel.Enable();
				}
				else
				{
					_controls.UIAcceptCancel.Disable();
				}
				break;
			case InputMapEnum.AcceptCancel:
				if (toggleOn)
				{
					_controls.AcceptCancel.Enable();
				}
				else
				{
					_controls.AcceptCancel.Disable();
				}
				break;
			case InputMapEnum.UiPointer:
				if (toggleOn)
				{
					_controls.UiPointer.Enable();
				}
				else
				{
					_controls.UiPointer.Disable();
				}
				break;
			case InputMapEnum.UiNextPrevious:
				if (toggleOn)
				{
					_controls.UiNextPrevious.Enable();
				}
				else
				{
					_controls.UiNextPrevious.Disable();
				}
				break;
			case InputMapEnum.UiThrottle:
				if (toggleOn)
				{
					_controls.UiThrottle.Enable();
				}
				else
				{
					_controls.UiThrottle.Disable();
				}
				break;
			case InputMapEnum.Menu:
				if (toggleOn)
				{
					_controls.Menu.Enable();
				}
				else
				{
					_controls.Menu.Disable();
				}
				break;
			case InputMapEnum.ShowMap:
				if (toggleOn)
				{
					_controls.ShowMap.Enable();
				}
				else
				{
					_controls.ShowMap.Disable();
				}
				break;
			case InputMapEnum.MapUi:
				if (toggleOn)
				{
					_controls.MapUi.Enable();
				}
				else
				{
					_controls.MapUi.Disable();
				}
				break;
			case InputMapEnum.MapMove:
				if (toggleOn)
				{
					_controls.MapMove.Enable();
				}
				else
				{
					_controls.MapMove.Disable();
				}
				break;
			case InputMapEnum.Drone:
				if (toggleOn)
				{
					_controls.Drone.Enable();
				}
				else
				{
					_controls.Drone.Disable();
				}
				break;
			case InputMapEnum.DroneCamera:
				if (toggleOn)
				{
					_controls.DroneCamera.Enable();
				}
				else
				{
					_controls.DroneCamera.Disable();
				}
				break;
			case InputMapEnum.Photo:
				if (toggleOn)
				{
					_controls.Photo.Enable();
				}
				else
				{
					_controls.Photo.Disable();
				}
				break;
			case InputMapEnum.PaintBase:
				if (toggleOn)
				{
					_controls.PaintBase.Enable();
				}
				else
				{
					_controls.PaintBase.Disable();
				}
				break;
			case InputMapEnum.PaintFavoritesToggle:
				if (toggleOn)
				{
					_controls.PaintFavoritesToggle.Enable();
				}
				else
				{
					_controls.PaintFavoritesToggle.Disable();
				}
				break;
			case InputMapEnum.PaintApplyCancel:
				if (toggleOn)
				{
					_controls.PaintApplyCancel.Enable();
				}
				else
				{
					_controls.PaintApplyCancel.Disable();
				}
				break;
			case InputMapEnum.PaintMultiSelect:
				if (toggleOn)
				{
					_controls.PaintMultiSelect.Enable();
				}
				else
				{
					_controls.PaintMultiSelect.Disable();
				}
				break;
			case InputMapEnum.PaintSelectNew:
				if (toggleOn)
				{
					_controls.PaintSelectNew.Enable();
				}
				else
				{
					_controls.PaintSelectNew.Disable();
				}
				break;
			case InputMapEnum.PaintPlates:
				if (toggleOn)
				{
					_controls.PaintPlates.Enable();
				}
				else
				{
					_controls.PaintPlates.Disable();
				}
				break;
			case InputMapEnum.PaintSelectSet:
				if (toggleOn)
				{
					_controls.PaintSelectSet.Enable();
				}
				else
				{
					_controls.PaintSelectSet.Disable();
				}
				break;
			case InputMapEnum.PaintCopy:
				if (toggleOn)
				{
					_controls.PaintCopy.Enable();
				}
				else
				{
					_controls.PaintCopy.Disable();
				}
				break;
			case InputMapEnum.PaintPlateTypes:
				if (toggleOn)
				{
					_controls.PaintPlateTypes.Enable();
				}
				else
				{
					_controls.PaintPlateTypes.Disable();
				}
				break;
			case InputMapEnum.PaintFavorite:
				if (toggleOn)
				{
					_controls.PaintFavorite.Enable();
				}
				else
				{
					_controls.PaintFavorite.Disable();
				}
				break;
			case InputMapEnum.PaintSelectionMade:
				if (toggleOn)
				{
					_controls.PaintSelectionMade.Enable();
				}
				else
				{
					_controls.PaintSelectionMade.Disable();
				}
				break;
			case InputMapEnum.PaintColor:
				if (toggleOn)
				{
					_controls.PaintColor.Enable();
				}
				else
				{
					_controls.PaintColor.Disable();
				}
				break;
			case InputMapEnum.PaintSize:
				if (toggleOn)
				{
					_controls.PaintSize.Enable();
				}
				else
				{
					_controls.PaintSize.Disable();
				}
				break;
			case InputMapEnum.PaintText:
				if (toggleOn)
				{
					_controls.PaintText.Enable();
				}
				else
				{
					_controls.PaintText.Disable();
				}
				break;
			case InputMapEnum.PaintEditText:
				if (toggleOn)
				{
					_controls.PaintEditText.Enable();
				}
				else
				{
					_controls.PaintEditText.Disable();
				}
				break;
			case InputMapEnum.Camera:
				if (toggleOn)
				{
					_controls.Camera.Enable();
				}
				else
				{
					_controls.Camera.Disable();
				}
				break;
			case InputMapEnum.Move:
				if (toggleOn)
				{
					_controls.Move.Enable();
				}
				else
				{
					_controls.Move.Disable();
				}
				break;
			case InputMapEnum.ZoomMouse:
				if (toggleOn)
				{
					_controls.ZoomMouse.Enable();
				}
				else
				{
					_controls.ZoomMouse.Disable();
				}
				break;
			case InputMapEnum.ZoomGamepad:
				if (toggleOn)
				{
					_controls.ZoomGamepad.Enable();
				}
				else
				{
					_controls.ZoomGamepad.Disable();
				}
				break;
			case InputMapEnum.ZoomMoza:
				if (toggleOn)
				{
					_controls.ZoomMoza.Enable();
				}
				else
				{
					_controls.ZoomMoza.Disable();
				}
				break;
			case InputMapEnum.CameraCar:
				if (toggleOn)
				{
					_controls.CameraCar.Enable();
				}
				else
				{
					_controls.CameraCar.Disable();
				}
				break;
			case InputMapEnum.AimReticle:
				if (toggleOn)
				{
					_controls.AimReticle.Enable();
				}
				else
				{
					_controls.AimReticle.Disable();
				}
				break;
			case InputMapEnum.ChangeCamera:
				if (toggleOn)
				{
					_controls.ChangeCamera.Enable();
				}
				else
				{
					_controls.ChangeCamera.Disable();
				}
				break;
			case InputMapEnum.CarRacing:
				if (toggleOn)
				{
					_controls.CarRacing.Enable();
				}
				else
				{
					_controls.CarRacing.Disable();
				}
				break;
			case InputMapEnum.CarDrag:
				if (toggleOn)
				{
					_controls.CarDrag.Enable();
				}
				else
				{
					_controls.CarDrag.Disable();
				}
				break;
			case InputMapEnum.BackToCar:
				if (toggleOn)
				{
					_controls.BackToCar.Enable();
				}
				else
				{
					_controls.BackToCar.Disable();
				}
				break;
			case InputMapEnum.ToRoad:
				if (toggleOn)
				{
					_controls.ToRoad.Enable();
				}
				else
				{
					_controls.ToRoad.Disable();
				}
				break;
			case InputMapEnum.ToRoadSteeringWheel:
				if (toggleOn)
				{
					_controls.ToRoadSteeringWheel.Enable();
				}
				else
				{
					_controls.ToRoadSteeringWheel.Disable();
				}
				break;
			case InputMapEnum.Headlights:
				if (toggleOn)
				{
					_controls.Headlights.Enable();
				}
				else
				{
					_controls.Headlights.Disable();
				}
				break;
			case InputMapEnum.Door:
				if (toggleOn)
				{
					_controls.Door.Enable();
				}
				else
				{
					_controls.Door.Disable();
				}
				break;
			case InputMapEnum.FreeRace:
				if (toggleOn)
				{
					_controls.FreeRace.Enable();
				}
				else
				{
					_controls.FreeRace.Disable();
				}
				break;
			case InputMapEnum.Phone:
				if (toggleOn)
				{
					_controls.Phone.Enable();
				}
				else
				{
					_controls.Phone.Disable();
				}
				break;
			case InputMapEnum.Voice:
				if (toggleOn)
				{
					_controls.Voice.Enable();
				}
				else
				{
					_controls.Voice.Disable();
				}
				break;
			case InputMapEnum.ChatGamepad:
				if (toggleOn)
				{
					_controls.ChatGamepad.Enable();
				}
				else
				{
					_controls.ChatGamepad.Disable();
				}
				break;
			case InputMapEnum.CircleMenu:
				if (toggleOn)
				{
					_controls.CircleMenu.Enable();
				}
				else
				{
					_controls.CircleMenu.Disable();
				}
				break;
			case InputMapEnum.OpenCircleMenu:
				if (toggleOn)
				{
					_controls.OpenCircleMenu.Enable();
				}
				else
				{
					_controls.OpenCircleMenu.Disable();
				}
				break;
			case InputMapEnum.CarBase:
				if (toggleOn)
				{
					_controls.CarBase.Enable();
				}
				else
				{
					_controls.CarBase.Disable();
				}
				break;
			case InputMapEnum.SteeringWheelCar:
				if (toggleOn)
				{
					_controls.SteeringWheelCar.Enable();
				}
				else
				{
					_controls.SteeringWheelCar.Disable();
				}
				break;
			case InputMapEnum.Shifter:
				if (toggleOn)
				{
					_controls.Shifter.Enable();
				}
				else
				{
					_controls.Shifter.Disable();
				}
				break;
			case InputMapEnum.ToggleUi:
				if (toggleOn)
				{
					_controls.ToggleUi.Enable();
				}
				else
				{
					_controls.ToggleUi.Disable();
				}
				break;
			case InputMapEnum.Debug:
				if (toggleOn)
				{
					_controls.Debug.Enable();
				}
				else
				{
					_controls.Debug.Disable();
				}
				break;
			case InputMapEnum.MozaHelp:
				if (toggleOn)
				{
					_controls.MozaHelp.Enable();
				}
				else
				{
					_controls.MozaHelp.Disable();
				}
				break;
			case InputMapEnum.Person:
				if (toggleOn)
				{
					_controls.Person.Enable();
				}
				else
				{
					_controls.Person.Disable();
				}
				break;
			case (InputMapEnum)6:
			case (InputMapEnum)9:
			case (InputMapEnum)13:
			case (InputMapEnum)24:
			case (InputMapEnum)42:
				break;
			}
		}

		private void SubscribeToActions()
		{
			_controls.AcceptCancel.Cancel.performed += OnCancel;
			_controls.AcceptCancel.Submit.performed += OnConfirm;
			_controls.AcceptCancel.Submit.started += OnConfirmHold;
			_controls.AcceptCancel.Submit.canceled += OnConfirmHold;
			_controls.UiPointer.PointerDelta.performed += CheckMouseMove;
			_controls.UiPointer.Click.performed += CursorOnConfirmClick;
			_controls.UiPointer.Click.canceled += CursorOnConfirmClick;
			_controls.UiPointer.CursorMove.started += StartCursorMoveWithStick;
			_controls.UiPointer.CursorMove.performed += StartCursorMoveWithStick;
			_controls.UI.Navigate.started += StartCursorMoveWithKeys;
			_controls.UI.goBack.performed += OnCancel;
			_controls.Camera.Look.started += OnLook;
			_controls.Camera.Look.performed += OnLook;
			_controls.Camera.Look.canceled += OnLook;
			_controls.Camera.LookFree.started += OnLookFree;
			_controls.Camera.LookFree.canceled += OnLookFree;
			_controls.ChangeCamera.ChangeCamera.performed += OnChangeCamera;
			_controls.Headlights.Headlights.performed += HeadlightsOnPerformed;
			_controls.CarBase.Horn.started += OnHorn;
			_controls.CarBase.Horn.canceled += OnHorn;
			_controls.CarBase.StartEngine.performed += OnStartEngine;
			_controls.CarBase.StartEngine.canceled += OnStartEngine;
			_controls.CarBase.Handbrake.started += OnHandbrake;
			_controls.CarBase.Handbrake.canceled += OnHandbrake;
			_controls.CarBase.HandbrakeHold.started += OnHandbrakeHold;
			_controls.CarBase.HandbrakeHold.canceled += OnHandbrakeHold;
			_controls.Door.OpenDoor.started += OnDoorStart;
			_controls.Door.OpenDoor.canceled += OnDoorCanceled;
			_controls.Door.OpenDoor.canceled += OnInteractCalled;
			_controls.Shifter.ShiftDown.started += ShiftDown;
			_controls.Shifter.ShiftUp.started += ShiftUp;
			_controls.Shifter.ShiftM.started += ShiftM;
			_controls.Shifter.ShiftPRND.started += ShiftA;
			_controls.Shifter.ToggleMA.performed += ToggleShiftMA;
			_controls.SteeringWheelCar.gear1.performed += SetGear1;
			_controls.SteeringWheelCar.gear2.performed += SetGear2;
			_controls.SteeringWheelCar.gear3.performed += SetGear3;
			_controls.SteeringWheelCar.gear4.performed += SetGear4;
			_controls.SteeringWheelCar.gear5.performed += SetGear5;
			_controls.SteeringWheelCar.gear6.performed += SetGear6;
			_controls.SteeringWheelCar.gear7.performed += SetGear7;
			_controls.SteeringWheelCar.gearR.performed += SetGearR;
			_controls.SteeringWheelCar.gearN.performed += SetGearN;
			_controls.SteeringWheelCar.gearUp.performed += SetGearUp;
			_controls.SteeringWheelCar.gearDown.performed += SetGearDown;
			_controls.SteeringWheelCar.gearAMToggle.performed += ToggleShiftMA;
			_controls.SteeringWheelCar.LeftSignal.performed += ToggleLeftSignal;
			_controls.SteeringWheelCar.RightSignal.performed += ToggleRightSignal;
			_controls.SteeringWheelCar.Emergency.performed += ToggleEmergencySignal;
			_controls.ToRoadSteeringWheel.ToRoad.performed += TriggerToRoad;
			_controls.CameraCar.CameraLookBack.started += StartStopLookBack;
			_controls.CameraCar.CameraLookLeft.started += StartStopLookLeft;
			_controls.CameraCar.CameraLookRight.started += StartStopLookRight;
			_controls.CameraCar.CameraLookBack.canceled += StartStopLookBack;
			_controls.CameraCar.CameraLookLeft.canceled += StartStopLookLeft;
			_controls.CameraCar.CameraLookRight.canceled += StartStopLookRight;
			_controls.CarBase.SetGearVisible.started += CloseCircleMenu;
			_controls.CarBase.SetGearVisible.started += CloseCircleMenuCam;
			_controls.CarBase.SetGearVisible.started += SetGearsVisible;
			_controls.CarBase.SetGearVisible.canceled += SetGearsInvisible;
			_controls.OpenCircleMenu.OpenCircleMenu.performed += ToggleCircleMenu;
			_controls.OpenCircleMenu.OpenCircleMenu.performed += SetGearsInvisible;
			_controls.OpenCircleMenu.OpenCircleMenuCam.performed += ToggleCircleMenuCam;
			_controls.OpenCircleMenu.OpenCircleMenuCam.performed += SetGearsInvisible;
			_controls.CircleMenu.CircleMenuCloseCam.performed += ToggleCircleMenuCam;
			_controls.CircleMenu.CircleMenuClose.performed += ToggleCircleMenu;
			_controls.CircleMenu.CircleMenuScroll.performed += OnCircleMenuScroll;
			_controls.ShowMap.Map.performed += OnToggleMapOn;
			_controls.Menu.Menu.performed += OpenMenu;
			_controls.Phone.Phone.performed += OpenPhone;
			_controls.Voice.VoiceChat.started += OnVoice;
			_controls.Voice.VoiceChat.canceled += OnVoice;
			_controls.Voice.Chat.performed += OpenChat;
			_controls.ChatGamepad.Chat.performed += OpenChat;
			_controls.MapUi.Expand.performed += ExpandHideMapOptions;
			_controls.MapUi.HideShowAll.performed += HideShowAllFilters;
			_controls.MapUi.HideShow.performed += ToggleFilterOnMap;
			_controls.MapUi.Mark.performed += MarkOnMap;
			_controls.MapUi.CloseMap.performed += OnToggleMapOff;
			_controls.MapUi.ShowMarkDetails.performed += ShowPopUpMap;
			_controls.MapMove.Move.started += CheckMouseMove;
			_controls.MapMove.Move.started += OnMapMove;
			_controls.MapMove.Move.canceled += OnMapMove;
			_controls.MapMove.MouseDragMove.started += CheckMouseMove;
			_controls.MapMove.MouseDragMove.started += OnMapMouseDrag;
			_controls.MapMove.MouseDragMove.canceled += OnMapMouseDrag;
			_controls.UiNextPrevious.Next.performed += Next;
			_controls.UiNextPrevious.Previous.performed += Previous;
			_controls.PaintBase.BlockCam.performed += BlockCamera;
			_controls.PaintBase.DeleteAll.performed += DeleteAll;
			_controls.PaintBase.DeleteSelected.performed += DeleteSelected;
			_controls.PaintFavoritesToggle.FavoritesToggle.performed += FavoritesToggle;
			_controls.PaintBase.ToggleUi.performed += ToggleUi;
			_controls.PaintColor.ColorCode.performed += SetColorCode;
			_controls.PaintColor.ColorToggle.performed += ColorToggle;
			_controls.PaintColor.TintToggle.performed += TintToggle;
			_controls.PaintSize.SetSize.performed += SetSize;
			_controls.PaintSize.Mirror.performed += Mirror;
			_controls.PaintCopy.Copy.performed += Copy;
			_controls.PaintFavorite.Favorite.performed += Favorite;
			_controls.PaintMultiSelect.MultiSelect.performed += MultiSelect;
			_controls.PaintApplyCancel.CancelChange.performed += CancelChange;
			_controls.PaintSelectionMade.LayerUpDown.canceled += StopScroll;
			_controls.PaintSelectionMade.LayerUpDown.performed += LayerUpDown;
			_controls.PaintSelectNew.SelectNew.started += ScrollLeftRight;
			_controls.PaintSelectNew.SelectNew.canceled += ScrollLeftRight;
			_controls.PaintSelectSet.SelectSetVinyl.started += ScrollUpDown;
			_controls.PaintSelectSet.SelectSetVinyl.canceled += ScrollUpDown;
			_controls.PaintSelectionMade.SetSize.performed += ToggleToSize;
			_controls.PaintSelectionMade.SetColor.performed += ToggleToColor;
			_controls.PaintEditText.EditTextStyle.performed += ToggleToText;
			_controls.PaintText.Bold.performed += ToggleBold;
			_controls.PaintText.Italics.performed += ToggleItalics;
			_controls.PaintText.EnterText.performed += EnterText;
			_controls.PaintText.Font.performed += FontUpDown;
			_controls.PaintPlateTypes.EnterPlateTypes.performed += EnterPlateTypes;
			_controls.PaintPlates.NavigatePlateType.started += ScrollLeftRight;
			_controls.PaintPlates.NavigatePlateType.canceled += ScrollLeftRight;
			_controls.PaintPlates.FrontToggle.performed += ToggleFrontPlate;
			_controls.PaintPlates.BackToggle.performed += ToggleBackPlate;
			_controls.DroneCamera.Snapshot.performed += Snapshot;
			_controls.DroneCamera.Reset.performed += ResetCamera;
			_controls.Move.Move.performed += StartStopCameraMove;
			_controls.Move.Height.started += StartStopCameraHeight;
			_controls.Move.Height.canceled += StartStopCameraHeight;
			_controls.Person.Crouch.performed += PersonCrouch;
			_controls.Person.Jump.performed += PersonJump;
			_controls.Person.WalkRunToggle.performed += PersonWalkRunToggle;
			_controls.Drone.Tilt.started += StartStopCameraTilt;
			_controls.Drone.Tilt.canceled += StartStopCameraTilt;
			_controls.Drone.Back.performed += CloseDrone;
			_controls.Drone.Center.performed += CenterDroneCamera;
			_controls.Drone.HideUi.performed += HideDroneUi;
			_controls.Drone.SpeedToggle.performed += SpeedToggle;
			_controls.Drone.OpenChat.performed += OpenChat;
			_controls.Photo.Camera.performed += PhotoCamera;
			_controls.Photo.Color.performed += PhotoColor;
			_controls.Photo.Help.performed += PhotoHelp;
			_controls.Photo.Optica.performed += PhotoOptica;
			_controls.Photo.NavigateSettings.performed += PhotoNavigateSettings;
			_controls.Photo.Toggles.performed += PhotoToggles;
			_controls.Photo.ResetFilter.performed += ResetFilter;
			_controls.Photo.ToggleFocus.performed += ToggleFocus;
			_controls.UiThrottle.Throttle.started += StartCancelUiThrottle;
			_controls.UiThrottle.Throttle.canceled += StartCancelUiThrottle;
			_controls.ToRoad.ToRoad.performed += TriggerToRoad;
			_controls.CarRacing.PhotoMode.performed += PhotoMode;
			_controls.CarRacing.Timer.performed += Timer;
			_controls.CarDrag.Burnout.started += Burnout;
			_controls.CarDrag.Burnout.canceled += Burnout;
			_controls.CarDrag.KeepLane.performed += KeepLane;
			_controls.BackToCar.BackToCar.performed += BackToCar;
			_controls.FreeRace.Restart.performed += Restart;
			_controls.FreeRace.CancelRace.performed += CancelRace;
			_controls.AimReticle.AimAt.started += AimAt;
			_controls.AimReticle.AimAt.canceled += AimAt;
			_controls.ToggleUi.ToggleUi.performed += ToggleUi;
			_controls.Debug.StartCursorOnly.performed += ShowCursorOnly;
			_controls.Debug.StopCursorOnly.performed += StopCursorOnly;
		}

		private void UnsubscribeToActions()
		{
			_controls.Debug.StartCursorOnly.performed -= ShowCursorOnly;
			_controls.Debug.StopCursorOnly.performed -= StopCursorOnly;
			_controls.ToggleUi.ToggleUi.performed -= ToggleUi;
			_controls.AimReticle.AimAt.performed -= AimAt;
			_controls.AimReticle.AimAt.canceled -= AimAt;
			_controls.ToRoad.ToRoad.performed -= TriggerToRoad;
			_controls.CarRacing.PhotoMode.performed -= PhotoMode;
			_controls.CarRacing.Timer.performed -= Timer;
			_controls.CarDrag.Burnout.started -= Burnout;
			_controls.CarDrag.Burnout.canceled -= Burnout;
			_controls.CarDrag.KeepLane.performed -= KeepLane;
			_controls.BackToCar.BackToCar.performed -= BackToCar;
			_controls.FreeRace.Restart.performed += Restart;
			_controls.FreeRace.CancelRace.performed += CancelRace;
			_controls.UiThrottle.Throttle.started -= StartCancelUiThrottle;
			_controls.UiThrottle.Throttle.canceled -= StartCancelUiThrottle;
			_controls.AcceptCancel.Cancel.performed -= OnCancel;
			_controls.AcceptCancel.Submit.performed -= OnConfirm;
			_controls.AcceptCancel.Submit.started -= OnConfirmHold;
			_controls.AcceptCancel.Submit.canceled -= OnConfirmHold;
			_controls.UiPointer.PointerDelta.performed -= CheckMouseMove;
			_controls.UiPointer.Click.performed -= CursorOnConfirmClick;
			_controls.UiPointer.Click.canceled -= CursorOnConfirmClick;
			_controls.UiPointer.CursorMove.started -= StartCursorMoveWithStick;
			_controls.UiPointer.CursorMove.performed -= StartCursorMoveWithStick;
			_controls.UI.Navigate.started -= StartCursorMoveWithKeys;
			_controls.UI.goBack.performed -= OnCancel;
			_controls.Camera.Look.started -= OnLook;
			_controls.Camera.Look.performed -= OnLook;
			_controls.Camera.Look.canceled -= OnLook;
			_controls.Camera.LookFree.started -= OnLookFree;
			_controls.Camera.LookFree.canceled -= OnLookFree;
			_controls.ChangeCamera.ChangeCamera.performed -= OnChangeCamera;
			_controls.Headlights.Headlights.performed -= HeadlightsOnPerformed;
			_controls.CarBase.Horn.started -= OnHorn;
			_controls.CarBase.Horn.canceled -= OnHorn;
			_controls.CarBase.StartEngine.performed -= OnStartEngine;
			_controls.CarBase.StartEngine.canceled -= OnStartEngine;
			_controls.CarBase.Handbrake.started -= OnHandbrake;
			_controls.CarBase.Handbrake.canceled -= OnHandbrake;
			_controls.CarBase.HandbrakeHold.started -= OnHandbrakeHold;
			_controls.CarBase.HandbrakeHold.canceled -= OnHandbrakeHold;
			_controls.Door.OpenDoor.started -= OnDoorStart;
			_controls.Door.OpenDoor.canceled -= OnDoorCanceled;
			_controls.Door.OpenDoor.canceled -= OnInteractCalled;
			_controls.Shifter.ShiftDown.started -= ShiftDown;
			_controls.Shifter.ShiftUp.started -= ShiftUp;
			_controls.Shifter.ShiftM.started -= ShiftM;
			_controls.Shifter.ShiftPRND.started -= ShiftA;
			_controls.Shifter.ToggleMA.performed -= ToggleShiftMA;
			_controls.SteeringWheelCar.gear1.performed -= SetGear1;
			_controls.SteeringWheelCar.gear2.performed -= SetGear2;
			_controls.SteeringWheelCar.gear3.performed -= SetGear3;
			_controls.SteeringWheelCar.gear4.performed -= SetGear4;
			_controls.SteeringWheelCar.gear5.performed -= SetGear5;
			_controls.SteeringWheelCar.gear6.performed -= SetGear6;
			_controls.SteeringWheelCar.gear7.performed -= SetGear7;
			_controls.SteeringWheelCar.gearR.performed -= SetGearR;
			_controls.SteeringWheelCar.gearN.performed -= SetGearN;
			_controls.SteeringWheelCar.gearUp.performed -= SetGearUp;
			_controls.SteeringWheelCar.gearDown.performed -= SetGearDown;
			_controls.SteeringWheelCar.gearAMToggle.performed -= ToggleShiftMA;
			_controls.SteeringWheelCar.LeftSignal.performed -= ToggleLeftSignal;
			_controls.SteeringWheelCar.RightSignal.performed -= ToggleRightSignal;
			_controls.SteeringWheelCar.Emergency.performed -= ToggleEmergencySignal;
			_controls.ToRoadSteeringWheel.ToRoad.performed -= TriggerToRoad;
			_controls.CameraCar.CameraLookBack.started -= StartStopLookBack;
			_controls.CameraCar.CameraLookLeft.started -= StartStopLookLeft;
			_controls.CameraCar.CameraLookRight.started -= StartStopLookRight;
			_controls.CameraCar.CameraLookBack.canceled -= StartStopLookBack;
			_controls.CameraCar.CameraLookLeft.canceled -= StartStopLookLeft;
			_controls.CameraCar.CameraLookRight.canceled -= StartStopLookRight;
			_controls.CarBase.SetGearVisible.started -= CloseCircleMenu;
			_controls.CarBase.SetGearVisible.started -= CloseCircleMenuCam;
			_controls.CarBase.SetGearVisible.started -= SetGearsVisible;
			_controls.CarBase.SetGearVisible.canceled -= SetGearsInvisible;
			_controls.OpenCircleMenu.OpenCircleMenu.performed -= ToggleCircleMenu;
			_controls.OpenCircleMenu.OpenCircleMenu.performed -= SetGearsInvisible;
			_controls.OpenCircleMenu.OpenCircleMenuCam.performed -= ToggleCircleMenuCam;
			_controls.OpenCircleMenu.OpenCircleMenuCam.performed -= SetGearsInvisible;
			_controls.CircleMenu.CircleMenuCloseCam.performed -= ToggleCircleMenuCam;
			_controls.CircleMenu.CircleMenuClose.performed -= ToggleCircleMenu;
			_controls.CircleMenu.CircleMenuScroll.performed -= OnCircleMenuScroll;
			_controls.ShowMap.Map.performed -= OnToggleMapOn;
			_controls.Menu.Menu.performed -= OpenMenu;
			_controls.Phone.Phone.performed -= OpenPhone;
			_controls.Voice.VoiceChat.started -= OnVoice;
			_controls.Voice.VoiceChat.canceled -= OnVoice;
			_controls.Voice.Chat.performed -= OpenChat;
			_controls.ChatGamepad.Chat.performed -= OpenChat;
			_controls.MapUi.Expand.performed -= ExpandHideMapOptions;
			_controls.MapUi.HideShowAll.performed -= HideShowAllFilters;
			_controls.MapUi.HideShow.performed -= ToggleFilterOnMap;
			_controls.MapUi.Mark.performed -= MarkOnMap;
			_controls.MapUi.CloseMap.performed -= OnToggleMapOff;
			_controls.MapUi.ShowMarkDetails.performed -= ShowPopUpMap;
			_controls.MapMove.Move.started -= CheckMouseMove;
			_controls.MapMove.Move.started -= OnMapMove;
			_controls.MapMove.Move.canceled -= OnMapMove;
			_controls.MapMove.MouseDragMove.started -= CheckMouseMove;
			_controls.MapMove.MouseDragMove.started -= OnMapMouseDrag;
			_controls.MapMove.MouseDragMove.canceled -= OnMapMouseDrag;
			_controls.PaintBase.BlockCam.performed -= BlockCamera;
			_controls.PaintBase.DeleteAll.performed -= DeleteAll;
			_controls.PaintBase.DeleteSelected.performed -= DeleteSelected;
			_controls.PaintFavoritesToggle.FavoritesToggle.performed -= FavoritesToggle;
			_controls.PaintBase.ToggleUi.performed -= ToggleUi;
			_controls.PaintColor.ColorCode.performed -= SetColorCode;
			_controls.PaintColor.ColorToggle.performed -= ColorToggle;
			_controls.PaintColor.TintToggle.performed -= TintToggle;
			_controls.PaintSize.SetSize.performed -= SetSize;
			_controls.PaintSize.Mirror.performed -= Mirror;
			_controls.PaintCopy.Copy.performed -= Copy;
			_controls.PaintFavorite.Favorite.performed -= Favorite;
			_controls.PaintMultiSelect.MultiSelect.performed -= MultiSelect;
			_controls.PaintApplyCancel.CancelChange.performed -= CancelChange;
			_controls.PaintSelectionMade.LayerUpDown.canceled -= StopScroll;
			_controls.PaintSelectionMade.LayerUpDown.performed -= LayerUpDown;
			_controls.PaintSelectNew.SelectNew.started -= ScrollLeftRight;
			_controls.PaintSelectNew.SelectNew.canceled -= ScrollLeftRight;
			_controls.PaintSelectSet.SelectSetVinyl.started -= ScrollUpDown;
			_controls.PaintSelectSet.SelectSetVinyl.canceled -= ScrollUpDown;
			_controls.PaintSelectionMade.SetSize.performed -= ToggleToSize;
			_controls.PaintSelectionMade.SetColor.performed -= ToggleToColor;
			_controls.PaintEditText.EditTextStyle.performed -= ToggleToText;
			_controls.PaintText.Bold.performed -= ToggleBold;
			_controls.PaintText.Italics.performed -= ToggleItalics;
			_controls.PaintText.EnterText.performed -= EnterText;
			_controls.PaintText.Font.performed -= FontUpDown;
			_controls.PaintPlateTypes.EnterPlateTypes.performed -= EnterPlateTypes;
			_controls.PaintPlates.NavigatePlateType.started -= ScrollLeftRight;
			_controls.PaintPlates.NavigatePlateType.canceled -= ScrollLeftRight;
			_controls.PaintPlates.FrontToggle.performed -= ToggleFrontPlate;
			_controls.PaintPlates.BackToggle.performed -= ToggleBackPlate;
			_controls.DroneCamera.Snapshot.performed -= Snapshot;
			_controls.DroneCamera.Reset.performed -= ResetCamera;
			_controls.Move.Move.performed -= StartStopCameraMove;
			_controls.Move.Height.started -= StartStopCameraHeight;
			_controls.Move.Height.canceled -= StartStopCameraHeight;
			_controls.Person.Crouch.performed -= PersonCrouch;
			_controls.Person.Jump.performed -= PersonJump;
			_controls.Person.WalkRunToggle.performed -= PersonWalkRunToggle;
			_controls.Drone.Tilt.started -= StartStopCameraTilt;
			_controls.Drone.Tilt.canceled -= StartStopCameraTilt;
			_controls.Drone.Back.performed -= CloseDrone;
			_controls.Drone.Center.performed -= CenterDroneCamera;
			_controls.Drone.HideUi.performed -= HideDroneUi;
			_controls.Drone.SpeedToggle.performed -= SpeedToggle;
			_controls.Drone.OpenChat.performed -= OpenChat;
			_controls.Photo.Camera.performed -= PhotoCamera;
			_controls.Photo.Color.performed -= PhotoColor;
			_controls.Photo.Help.performed -= PhotoHelp;
			_controls.Photo.Optica.performed -= PhotoOptica;
			_controls.Photo.NavigateSettings.performed -= PhotoNavigateSettings;
			_controls.Photo.Toggles.performed -= PhotoToggles;
			_controls.Photo.ResetFilter.performed -= ResetFilter;
			_controls.Photo.ToggleFocus.performed -= ToggleFocus;
		}

		public void FixedTick()
		{
			if (IsInputBlocked())
			{
				return;
			}
			if (_controls.CarBase.enabled)
			{
				float value = (_controls.CarBase.Throttle.IsInProgress() ? _controls.CarBase.Throttle.ReadValue<float>() : 0f);
				float value2 = (_controls.CarBase.Brake.IsInProgress() ? _controls.CarBase.Brake.ReadValue<float>() : 0f);
				float value3 = (_controls.CarBase.Clutch.IsInProgress() ? _controls.CarBase.Clutch.ReadValue<float>() : 0f);
				float steerInput = _controls.CarBase.Steering.ReadValue<float>();
				if (_controls.CarBase.Steering.activeControl?.device is Gamepad)
				{
					steerInput = ApplyStickDeadzone(steerInput, _controlsSettings.StickDeadzone(left: false));
				}
				float keyboardOrStickSteer = steerInput;
				float num = Mathf.Max(_controls.CarBase.Handbrake.IsInProgress() ? _controls.CarBase.Handbrake.ReadValue<float>() : 0f, _controls.CarBase.HandbrakeHold.ReadValue<float>());
				if (_steeringWheelValueEvent.StoredValue)
				{
					float mozaSteerAngle = MozaWrapper.GetMozaSteerAngle();
					steerInput = (((double)Mathf.Abs(steerInput) > 0.0001) ? steerInput : mozaSteerAngle);
					num = (MozaWrapper.GetMozaHandbrake() ? 1f : num);
				}
				bool gamepad = _controls.CarBase.Steering.activeControl?.device is Gamepad;
				if (ApplyMouseSteering(keyboardOrStickSteer, ref steerInput))
				{
					gamepad = false;
				}
				if (_controlsSettings.InvertSteering(gamepad))
				{
					steerInput = 0f - steerInput;
				}
				_steerHandle.Get(_eventRegistry).Value = steerInput;
				_throttleHandle.Get(_eventRegistry).Value = value;
				_brakeHandle.Get(_eventRegistry).Value = value2;
				_clutchHandle.Get(_eventRegistry).Value = value3;
				if (FlagsStorage.IsMainMenu || _steeringWheelValueEvent.StoredValue)
				{
					_handbrakeHandle.Get(_eventRegistry).Value = num;
				}
			}
			if (_controls.UiThrottle.enabled && FlagsStorage.IsMainMenu)
			{
				_throttleHandle.Get(_eventRegistry).Value = _controls.UiThrottle.Throttle.ReadValue<float>();
			}
			if (_controls.Move.enabled && _controls.Person.enabled)
			{
				Vector2 vector = _controls.Move.Move.ReadValue<Vector2>();
				if (_controls.Move.Move.activeControl?.device is Gamepad)
				{
					Vector2 vector2 = ((Gamepad.current != null) ? ApplyStickDeadzone(Gamepad.current.leftStick.ReadValue(), _controlsSettings.StickDeadzone(left: false)) : Vector2.zero);
					Vector2 vector3 = ReadKeyboardMove();
					vector = ((vector3.sqrMagnitude > 0.0001f) ? vector3 : vector2);
				}
				vector = vector.normalized;
				_movePersonHandle.Get(_eventRegistry).Value = vector;
				_personRotateRightLeftHandle.Get(_eventRegistry).Value = _controls.Move.Rotate.ReadValue<float>() * _controlsSettings.GetPersonTurnSpeed();
				_personRunHandle.Get(_eventRegistry).Value = _controls.Person.Run.ReadValue<float>();
			}
		}

		private Vector2 ReadKeyboardMove()
		{
			Keyboard current = Keyboard.current;
			if (current == null)
			{
				return Vector2.zero;
			}
			Vector2 zero = Vector2.zero;
			ReadOnlyArray<InputBinding> bindings = _controls.Move.Move.bindings;
			for (int i = 0; i < bindings.Count; i++)
			{
				InputBinding inputBinding = bindings[i];
				if (inputBinding.isPartOfComposite && InputControlPath.TryFindControl(current, inputBinding.effectivePath) is ButtonControl { isPressed: not false })
				{
					switch (inputBinding.name)
					{
					case "up":
						zero.y++;
						break;
					case "down":
						zero.y--;
						break;
					case "right":
						zero.x++;
						break;
					case "left":
						zero.x--;
						break;
					}
				}
			}
			return zero;
		}

		private Vector2 ApplyCameraSettings(bool isGamepad, Vector2 lookVector)
		{
			var (flag, flag2, _) = _controlsSettings.GetInvert(isGamepad);
			if (isGamepad)
			{
				lookVector = ApplyStickDeadzone(lookVector, _controlsSettings.StickDeadzone(left: false)) * Time.unscaledDeltaTime;
			}
			lookVector = Vector2.Scale(lookVector, _controlsSettings.GetCameraMultiplier(isGamepad));
			if (flag)
			{
				lookVector.x = 0f - lookVector.x;
			}
			if (flag2)
			{
				lookVector.y = 0f - lookVector.y;
			}
			return lookVector;
		}

		private void TickMouseSteering()
		{
			_mouseSteerHeld = IsMouseSteerHeld();
			float deltaX = (_mouseSteerHeld ? _controls.CarBase.SteeringMouse.ReadValue<float>() : 0f);
			_mouseSteerPosition = MouseSteering.Step(_mouseSteerPosition, _mouseSteerHeld, deltaX, _controlsSettings.MouseSteerMaxDisplacement, _controlsSettings.MouseSteerReturnRate, Time.deltaTime);
		}

		private bool IsMouseSteerHeld()
		{
			if (!_controls.CarBase.enabled || !IsApplicationFocused() || Cursor.lockState != CursorLockMode.Locked || _lmbDragOnScroll)
			{
				return false;
			}
			return _controls.CarBase.SteeringMouse.controls.Any((InputControl t) => t is ButtonControl buttonControl && buttonControl.isPressed);
		}

		private bool ApplyMouseSteering(float keyboardOrStickSteer, ref float steerInput)
		{
			if (!_mouseSteerHeld && _mouseSteerPosition == 0f)
			{
				return false;
			}
			if (!_mouseSteerHeld && Mathf.Abs(keyboardOrStickSteer) > 0.0001f)
			{
				_mouseSteerPosition = 0f;
				return false;
			}
			steerInput = _mouseSteerPosition;
			return true;
		}

		private bool IsPointerOverScrollRect()
		{
			UnityEngine.EventSystems.EventSystem current = UnityEngine.EventSystems.EventSystem.current;
			if (current == null || Mouse.current == null)
			{
				return false;
			}
			Vector2 screenPos = Mouse.current.position.ReadValue();
			PointerEventData eventData = new PointerEventData(current)
			{
				position = screenPos
			};
			_lookScrollRaycast.Clear();
			current.RaycastAll(eventData, _lookScrollRaycast);
			foreach (RaycastResult item in _lookScrollRaycast)
			{
				GameObject gameObject = item.gameObject;
				if (gameObject == null)
				{
					continue;
				}
				if (gameObject.GetComponentInParent<ScrollRect>() != null)
				{
					return true;
				}
				Canvas componentInParent = gameObject.GetComponentInParent<Canvas>();
				if (!(componentInParent == null))
				{
					Camera cam = ((item.module != null) ? item.module.eventCamera : null);
					if ((from sr in componentInParent.rootCanvas.GetComponentsInChildren<ScrollRect>(includeInactive: false)
						where sr != null && sr.isActiveAndEnabled
						select (!(sr.viewport != null)) ? ((RectTransform)sr.transform) : sr.viewport).Any((RectTransform rt) => RectTransformUtility.RectangleContainsScreenPoint(rt, screenPos, cam)))
					{
						return true;
					}
				}
			}
			return false;
		}

		public void Tick()
		{
			_insideInputUpdate = false;
			if (_cursorOnlyActive && (!IsApplicationFocused() || !_controls.Debug.StartCursorOnly.IsPressed()))
			{
				ForceStopCursorOnly();
			}
			ProcessPendingMapToggles();
			ProcessPendingUiPointerToggle();
			if (_cursorOnlyActive)
			{
				return;
			}
			ReapplyCursorOnFocusRegained();
			float zoom = GetZoom();
			bool flag = IsShifterGesture();
			Mouse current = Mouse.current;
			if (current != null)
			{
				if (current.leftButton.wasPressedThisFrame)
				{
					_lmbDragOnScroll = IsPointerOverScrollRect();
				}
				else if (!current.leftButton.isPressed)
				{
					_lmbDragOnScroll = false;
				}
			}
			TickMouseSteering();
			Vector2 vector = Vector2.zero;
			if (_controls.Camera.enabled && !flag)
			{
				vector = _controls.Camera.Look.ReadValue<Vector2>();
				vector = ApplyCameraSettings(_controls.Camera.Look.activeControl?.device is Gamepad, vector);
				if (_lmbDragOnScroll || _mouseSteerHeld)
				{
					vector = Vector2.zero;
				}
			}
			if ((_controls.CameraCar.enabled || _controls.ChangeCamera.enabled || _inputMapSetEvent.CurrentInputGroup == InputGroupsEnum.PersonDrag) && !flag)
			{
				Vector2 lookVector = _controls.Camera.LookFree.ReadValue<Vector2>();
				Vector2 vector2 = ApplyCameraSettings(_controls.Camera.LookFree.activeControl?.device is Gamepad, lookVector);
				_lookFreeHandle.Get(_eventRegistry).Value = (_mouseSteerHeld ? Vector2.zero : vector2);
			}
			else if (_inputMapSetEvent.CurrentInputGroup == InputGroupsEnum.VisualTuning && !flag)
			{
				_lookFreeHandle.Get(_eventRegistry).Value = vector * 1.2f;
			}
			if (_inputMapSetEvent.CurrentInputGroup == InputGroupsEnum.MenuWithRotation)
			{
				_lookRotateHandle.Get(_eventRegistry).Value = vector;
			}
			else
			{
				if (!_controls.CameraCar.enabled && !_controls.DroneCamera.enabled && !_controls.ChangeCamera.enabled)
				{
					InputGroupsEnum currentInputGroup = _inputMapSetEvent.CurrentInputGroup;
					if (currentInputGroup != InputGroupsEnum.Garage && currentInputGroup != InputGroupsEnum.DynoRun && currentInputGroup != InputGroupsEnum.CameraSetup && currentInputGroup != InputGroupsEnum.DragSpectator)
					{
						goto IL_02e5;
					}
				}
				_lookHandle.Get(_eventRegistry).Value = vector;
				_zoomHandle.Get(_eventRegistry).Value = ((zoom != 0f && IsPointerOverScrollRect()) ? 0f : zoom);
			}
			goto IL_02e5;
			IL_02e5:
			if (_controls.PaintBase.enabled || _inputMapSetEvent.CurrentInputGroup == InputGroupsEnum.VisualTuning)
			{
				_lookVinylHandle.Get(_eventRegistry).Value = vector;
				_zoomVinylHandle.Get(_eventRegistry).Value = zoom;
			}
			if (IsInputBlocked())
			{
				return;
			}
			if (_inputMapSetEvent.CurrentInputGroup == InputGroupsEnum.CameraSetup && _controls.Move.enabled)
			{
				Vector2 vector3 = _controls.Move.Move.ReadValue<Vector2>();
				if (_controls.Move.Move.activeControl?.device is Gamepad)
				{
					vector3 = ApplyStickDeadzone(vector3, _controlsSettings.StickDeadzone(left: false));
				}
				_cameraSetupMoveHandle.Get(_eventRegistry).Value = vector3 * Time.unscaledDeltaTime;
				_cameraSetupHeightHandle.Get(_eventRegistry).Value = _controls.Move.Height.ReadValue<float>() * Time.unscaledDeltaTime;
			}
			if (_controls.DroneCamera.enabled && _controls.Move.enabled)
			{
				Vector2 vector4 = _controls.Move.Move.ReadValue<Vector2>();
				if (_controls.Move.Move.activeControl?.device is Gamepad)
				{
					vector4 = ApplyStickDeadzone(vector4, _controlsSettings.StickDeadzone(left: false));
				}
				_droneCameraMoveHandle.Get(_eventRegistry).Value = vector4 * _controlsSettings.GetUniqueSlider(UniqueSliders.DroneMove).Current * Time.unscaledDeltaTime;
				_droneCameraHeightHandle.Get(_eventRegistry).Value = _controls.Move.Height.ReadValue<float>() * _controlsSettings.GetUniqueSlider(UniqueSliders.DroneVertical).Current * Time.unscaledDeltaTime;
			}
			if (_controls.Drone.enabled)
			{
				_droneCameraTiltHandle.Get(_eventRegistry).Value = _controls.Drone.Tilt.ReadValue<float>() * _controlsSettings.GetUniqueSlider(UniqueSliders.DroneTilt).Current * Time.unscaledDeltaTime;
			}
			if (_controls.MapMove.enabled)
			{
				Vector2 vector5 = _controls.MapMove.Move.ReadValue<Vector2>();
				if (_controls.MapMove.Move.activeControl?.device is Gamepad)
				{
					vector5 = ApplyStickDeadzone(vector5, _controlsSettings.StickDeadzone(left: false));
				}
				vector5 = vector5 * _controlsSettings.GetLookScale(CameraSettingsContext.Map) * Time.unscaledDeltaTime;
				_mapMoveHandle.Get(_eventRegistry).Value = new Vector3(vector5.x, vector5.y, 0f);
				_zoomHandle.Get(_eventRegistry).Value = zoom;
				if (_controls.MapMove.MouseDragMove.inProgress)
				{
					_mapDragMoveValueEventHandle.Get(_eventRegistry)?.Raise(_controls.MapMove.MouseDragMove.ReadValue<Vector2>());
					if (_controls.MapMove.MouseDelta.ReadValue<Vector2>().sqrMagnitude > 5f)
					{
						_onMapMoveHandle.Get(_eventRegistry)?.Raise(value: true);
					}
				}
			}
			if (_controls.CircleMenu.enabled)
			{
				Vector2 vector6 = _controls.CircleMenu.CircleMenuMove.ReadValue<Vector2>();
				if (_controls.CircleMenu.CircleMenuMove.activeControl?.device is Gamepad)
				{
					vector6 = ApplyStickDeadzone(vector6, _controlsSettings.StickDeadzone(left: false));
				}
				_circleMenuMoveHandle.Get(_eventRegistry).Value = vector6;
			}
			if (_controls.Shifter.enabled)
			{
				Vector2 vector7 = _controls.Shifter.ShifterMove.ReadValue<Vector2>();
				bool flag2 = _controls.Shifter.ShifterMove.activeControl?.device is Gamepad;
				if (flag2)
				{
					vector7 = ApplyStickDeadzone(vector7, _controlsSettings.StickDeadzone(left: false));
				}
				vector7 *= _controlsSettings.ShifterKnobScale(flag2);
				_shifterMoveHandle.Get(_eventRegistry).Value = vector7;
			}
			if (_controls.PaintColor.enabled)
			{
				_opacityChangeHandle.Get(_eventRegistry).Value = _controls.PaintColor.AlphaValue.ReadValue<float>();
				Vector2 vector8 = _controls.PaintColor.ColorMove.ReadValue<Vector2>().normalized;
				if (_controls.PaintColor.ColorMove.activeControl?.device is Gamepad)
				{
					vector8 = ApplyStickDeadzone(vector8, _controlsSettings.StickDeadzone(left: false));
				}
				_colorChangeHandle.Get(_eventRegistry).Value = vector8;
			}
			if (_controls.PaintSize.enabled)
			{
				Vector2 vector9 = _controls.PaintSize.SizeMove.ReadValue<Vector2>().normalized;
				if (_controls.PaintSize.SizeMove.activeControl?.device is Gamepad)
				{
					vector9 = ApplyStickDeadzone(vector9, _controlsSettings.StickDeadzone(left: false));
				}
				_sizeChangeHandle.Get(_eventRegistry).Value = vector9;
				_turnLeftRightHandle.Get(_eventRegistry).Value = _controls.PaintSize.Turn.ReadValue<float>();
			}
			if (_controls.PaintSelectionMade.enabled)
			{
				Vector2 vector10 = _controls.PaintSelectionMade.Move.ReadValue<Vector2>().normalized;
				if (_controls.PaintSelectionMade.Move.activeControl?.device is Gamepad)
				{
					vector10 = ApplyStickDeadzone(vector10, _controlsSettings.StickDeadzone(left: false));
				}
				_positionChangeHandle.Get(_eventRegistry).Value = vector10;
			}
			if (_controls.UiPointer.enabled)
			{
				Vector2 vector11 = _controls.UiPointer.CursorMove.ReadValue<Vector2>();
				vector11 = vector11.normalized * vector11.magnitude;
				vector11 = ApplyStickDeadzone(vector11, _controlsSettings.StickDeadzone(left: true));
				vector11 *= _controlsSettings.CursorSensitivity;
				_stickCursorMoveHandle.Get(_eventRegistry).Value = vector11;
			}
			TickScrollRepeat();
		}

		private void ProcessPendingMapToggles()
		{
			if (_pendingMapToggles.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<InputMapEnum, bool> pendingMapToggle in _pendingMapToggles)
			{
				ToggleMap(pendingMapToggle.Key, pendingMapToggle.Value);
			}
			_pendingMapToggles.Clear();
		}

		private void ProcessPendingUiPointerToggle()
		{
			if (_pendingUiPointerToggle != PendingMapToggle.None)
			{
				bool toggleOn = _pendingUiPointerToggle == PendingMapToggle.Enable;
				_pendingUiPointerToggle = PendingMapToggle.None;
				ToggleMap(InputMapEnum.UiPointer, toggleOn);
			}
		}

		private void ReapplyCursorOnFocusRegained()
		{
			bool flag = IsApplicationFocused();
			if (flag && !_wasApplicationFocused)
			{
				ApplyCursorStateForCurrentMaps();
			}
			_wasApplicationFocused = flag;
		}

		private void ApplyCursorStateForCurrentMaps()
		{
			if (!_cursorOnlyActive && _inputMapSetEvent.TryGetCurrentMaps(out var maps))
			{
				Cursor.lockState = ((maps.Contains(InputMapEnum.CarBase) && !maps.Contains(InputMapEnum.UI)) ? CursorLockMode.Locked : CursorLockMode.Confined);
				if (!FlagsStorage.IsSteamDeck)
				{
					Cursor.visible = maps.Contains(InputMapEnum.UiPointer) && _uiNavigationValueEvent.StoredValue == UiNavigationEnum.Mouse;
				}
			}
		}

		private void BeginCursorEngageWindow()
		{
			CancelCursorEngageWindow();
			if (!_cursorOnlyActive && !FlagsStorage.IsSteamDeck && _inputMapSetEvent.TryGetCurrentMaps(out var maps))
			{
				bool num = !maps.Contains(InputMapEnum.UiPointer) || _uiNavigationValueEvent.StoredValue != UiNavigationEnum.Mouse;
				bool flag = maps.Contains(InputMapEnum.CarBase) && !maps.Contains(InputMapEnum.UI);
				if (num && flag)
				{
					_ = _debug;
					_cursorEngageCts = CancellationTokenSource.CreateLinkedTokenSource(_onDisposeCts.Token);
					CursorEngageWindowAsync(_cursorEngageCts.Token).Forget();
				}
			}
		}

		private void CancelCursorEngageWindow()
		{
			if (_cursorEngageCts != null)
			{
				_cursorEngageCts.Cancel();
				_cursorEngageCts.Dispose();
				_cursorEngageCts = null;
			}
		}

		private async UniTaskVoid CursorEngageWindowAsync(CancellationToken ct)
		{
			for (int i = 0; i < 12; i++)
			{
				if (ct.IsCancellationRequested)
				{
					break;
				}
				ApplyCursorStateForCurrentMaps();
				if (i == 0)
				{
					NudgeCursorMessage();
				}
				if (await NextFrame(ct))
				{
					break;
				}
			}
		}

		private async UniTask<bool> NextFrame(CancellationToken ct)
		{
			await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
			return ct.IsCancellationRequested;
		}

		[DllImport("user32.dll")]
		private static extern void mouse_event(uint dwFlags, int dx, int dy, uint data, UIntPtr extra);

		private static void NudgeCursorMessage()
		{
			mouse_event(1u, 0, 0, 0u, UIntPtr.Zero);
		}

		private bool IsShifterGesture()
		{
			if (_controls.Shifter.enabled)
			{
				return _controls.Shifter.ShifterMove.ReadValue<Vector2>().sqrMagnitude > 0.0001f;
			}
			return false;
		}

		private float GetZoom()
		{
			float num = 0f;
			bool gamepad = false;
			if (_controls.ZoomMouse.enabled)
			{
				num = _controls.ZoomMouse.Zoom.ReadValue<float>();
				if (num != 0f && IsPointerOverScrollRect())
				{
					num = 0f;
				}
			}
			if (_controls.ZoomGamepad.enabled)
			{
				float num2 = _controls.ZoomGamepad.Zoom.ReadValue<float>();
				if (Mathf.Abs(num2) > Mathf.Abs(num))
				{
					num = num2;
					gamepad = true;
				}
			}
			if (_controls.ZoomMoza.enabled)
			{
				float num3 = _controls.ZoomMoza.Zoom.ReadValue<float>();
				if (Mathf.Abs(num3) > Mathf.Abs(num))
				{
					num = num3;
					gamepad = true;
				}
			}
			float num4 = _controlsSettings.GetZoomSpeedScalar(gamepad);
			if (_controlsSettings.GetZoomInvert(gamepad))
			{
				num4 = 0f - num4;
			}
			return num * num4 * Time.unscaledDeltaTime;
		}

		private void TickScrollRepeat()
		{
			if (!_scrollValueEventHandle.Get(_eventRegistry))
			{
				return;
			}
			ScrollEnum storedValue = _scrollValueEventHandle.Get(_eventRegistry).StoredValue;
			if (storedValue != ScrollEnum.None)
			{
				_scrollRepeatTimer -= Time.unscaledDeltaTime;
				if (_scrollRepeatTimer <= 0f)
				{
					_scrollValueEventHandle.Get(_eventRegistry)?.Raise(storedValue);
					_scrollRepeatTimer = (_scrollFirstFire ? 0.4f : 0.1f);
					_scrollFirstFire = false;
				}
			}
		}
	}
}
