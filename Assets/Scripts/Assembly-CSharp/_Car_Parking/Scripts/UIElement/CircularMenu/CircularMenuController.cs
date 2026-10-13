using System;
using System.Collections.Generic;
using CPM.Scripts.Garage;
using CPMEngine;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Car.CruiseControl;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.GameInput;
using _Car_Parking.Scripts.MenuUI;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.Person;
using _Car_Parking.Scripts.Person.Locomotion;
using _Car_Parking.Scripts.Police;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement.AnimationUI;

namespace _Car_Parking.Scripts.UIElement.CircularMenu
{
	public class CircularMenuController : MonoBehaviour
	{
		[Header("UI References")]
		[SerializeField]
		private GameObject _namePanel;

		[SerializeField]
		private TextMeshProUGUI _namePanelText;

		[SerializeField]
		[Header("Panel Groups")]
		private CircularMenuPanelGroup _carPanelGroup;

		[SerializeField]
		private CircularMenuPanelGroup _carPassengerPanelGroup;

		[SerializeField]
		private CircularMenuPanelGroup _personPanelGroup;

		[SerializeField]
		private CircularMenuPanelGroup _uiPanelGroup;

		[Header("Initial Mode")]
		[SerializeField]
		private CircularMenuMode _initialMode;

		private CircularMenuMode _currentMode;

		[SerializeField]
		[Header("Circular Menu Panels Config")]
		private CircularMenuPanelsConfig _circularMenuPanelsConfig;

		[Tooltip("Which input event toggles THIS circular menu. Use a dedicated event (e.g. a camera-menu event) for a separate menu instance so it opens on its own button.")]
		[Header("Open Trigger Event")]
		[SerializeField]
		private EventRegistryIDs _openEventId = EventRegistryIDs.CircleMenu;

		[SerializeField]
		[Tooltip("When true, this menu binds items by CircularMenuItem.AnimationSlot instead of ButtonActionId.")]
		[Header("Animation Menu Mode")]
		private bool _isAnimationMenu;

		[SerializeField]
		private AnimationDataSO _animationDataSo;

		private const int ShopAnimationSlot = -2;

		private IDisposable _animationResetDisposable;

		private CircularMenuPanel[] _activePanels;

		private int _activePanelIndex;

		private CircularMenuPanel _activePanel;

		private CircularMenuItem[] _items;

		private CircularMenuItem _hoveredItem;

		private Vector2 _inputDirection;

		private Vector2 _accumulatedInput;

		private Tween _showTween;

		private Tween _fadeTween;

		private bool _isGamepad;

		private bool _wasStickActive;

		private bool _isOpen;

		private int _middleZoneFrames;

		private RegistryHandle<ScriptableEvent> _closeCircleMenu = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.CloseCircleMenu);

		private RegistryHandle<BoolEvent> _circleMenuEvent;

		private RegistryHandle<BoolEvent> _circleMenuScrollEvent = new RegistryHandle<BoolEvent>(EventRegistryIDs.CircleMenuScroll);

		private RegistryHandle<Vector2Variable> _circleMenuMoveEvent = new RegistryHandle<Vector2Variable>(EventRegistryIDs.CircleMenuMove);

		private RegistryHandle<ScriptableEvent> _changeCameraHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ChangeCamera);

		private readonly List<CircularItemBinding> _cameraCycleBindings = new List<CircularItemBinding>();

		private CircularMenuItem _lastCameraItem;

		private EventRegistry _eventRegistry;

		private InputMapSetEvent _inputMapSetEvent;

		private CruiseControl _cruiseControl;

		private QuickCarSettings _quickCarSettings;

		private FreeDriveProvider _freeDriveProvider;

		private MenuLevelParent _menuLevelParent;

		private ParkingMenuUI _parkingMenuUI;

		private LockCarButtonController _lockCarButtonController;

		private ChatControl _chatControl;

		private GarageController _garageController;

		private PoliceButtonsControllerRegistry _policeRegistry;

		private OptionalDependency<CruiseControl> _cruiseControlDep;

		private OptionalDependency<QuickCarSettings> _quickCarSettingsDep;

		private OptionalDependency<FreeDriveProvider> _freeDriveProviderDep;

		private OptionalDependency<MenuLevelParent> _menuLevelParentDep;

		private OptionalDependency<ParkingMenuUI> _parkingMenuUIDep;

		private OptionalDependency<LockCarButtonController> _lockCarButtonControllerDep;

		private OptionalDependency<PoliceButtonsControllerRegistry> _policeRegistryDep;

		private OptionalDependency<ChatControl> _chatControlDep;

		private OptionalDependency<GarageController> _garageControllerDep;

		private OptionalDependency<LevelsProvider> _levelsProviderDep;

		private LevelsProvider _levelsProvider;

		private OptionalDependency<RacingProvider> _racingProviderDep;

		private RacingProvider _racingProvider;

		private OptionalDependency<ManagerUI> _managerUiDep;

		private ManagerUI _managerUi;

		private OptionalDependency<CarInputManager> _carInputManagerDep;

		private GamepadHapticService _gamepadHaptics;

		private CarInputManager _carInputManager;

		private readonly Dictionary<CircularMenuItem, Action> _subscribedHandlers = new Dictionary<CircularMenuItem, Action>();

		private readonly Dictionary<ButtonActionId, Action<CircularMenuItem>> _visualRefreshMap = new Dictionary<ButtonActionId, Action<CircularMenuItem>>();

		private readonly Dictionary<int, CircularMenuPanelConfig> _circularMenuPanelsConfigs = new Dictionary<int, CircularMenuPanelConfig>();

		private Dictionary<ButtonActionId, Action> _actionMap;

		private Dictionary<ButtonActionId, Func<bool>> _activeStateAvailabilityMap;

		private Dictionary<ButtonActionId, Func<bool>> _colorChangeAvailabilityMap;

		private List<CanvasGroup> _hiddenCheckGroups;

		private InputGroupsEnum _groupBeforeMenu;

		private bool CanScroll
		{
			get
			{
				CircularMenuPanel[] activePanels = _activePanels;
				if (activePanels != null)
				{
					return activePanels.Length > 1;
				}
				return false;
			}
		}

		private int ItemsCount => _items.Length;

		[Inject]
		public void Construct(EventRegistry eventRegistry, InputMapSetEvent inputMapSetEvent, OptionalDependency<CruiseControl> cruiseControlDep, OptionalDependency<QuickCarSettings> quickCarSettingsDep, OptionalDependency<FreeDriveProvider> freeDriveProviderDep, OptionalDependency<MenuLevelParent> menuLevelParentDep, OptionalDependency<ParkingMenuUI> parkingMenuUIDep, OptionalDependency<LockCarButtonController> lockCarButtonControllerDep, OptionalDependency<PoliceButtonsControllerRegistry> policeRegistryDep, OptionalDependency<ChatControl> chatControlDep, OptionalDependency<GarageController> garageControllerDep, OptionalDependency<LevelsProvider> levelsProviderDep, OptionalDependency<RacingProvider> racingProviderDep, OptionalDependency<ManagerUI> managerUiDep, GamepadHapticService gamepadHaptics, OptionalDependency<CarInputManager> carInputManagerDep)
		{
			_eventRegistry = eventRegistry;
			_inputMapSetEvent = inputMapSetEvent;
			_cruiseControlDep = cruiseControlDep;
			_quickCarSettingsDep = quickCarSettingsDep;
			_freeDriveProviderDep = freeDriveProviderDep;
			_menuLevelParentDep = menuLevelParentDep;
			_parkingMenuUIDep = parkingMenuUIDep;
			_lockCarButtonControllerDep = lockCarButtonControllerDep;
			_policeRegistryDep = policeRegistryDep;
			_chatControlDep = chatControlDep;
			_garageControllerDep = garageControllerDep;
			_levelsProviderDep = levelsProviderDep;
			_racingProviderDep = racingProviderDep;
			_managerUiDep = managerUiDep;
			_carInputManagerDep = carInputManagerDep;
			_gamepadHaptics = gamepadHaptics;
		}

		private void Awake()
		{
			_circleMenuEvent = new RegistryHandle<BoolEvent>(_openEventId);
			if (_cruiseControlDep.HasValue)
			{
				_cruiseControl = _cruiseControlDep.Value;
			}
			if (_quickCarSettingsDep.HasValue)
			{
				_quickCarSettings = _quickCarSettingsDep.Value;
			}
			if (_freeDriveProviderDep.HasValue)
			{
				_freeDriveProvider = _freeDriveProviderDep.Value;
			}
			if (_menuLevelParentDep.HasValue)
			{
				_menuLevelParent = _menuLevelParentDep.Value;
			}
			if (_parkingMenuUIDep.HasValue)
			{
				_parkingMenuUI = _parkingMenuUIDep.Value;
			}
			if (_lockCarButtonControllerDep.HasValue)
			{
				_lockCarButtonController = _lockCarButtonControllerDep.Value;
			}
			if (_policeRegistryDep.HasValue)
			{
				_policeRegistry = _policeRegistryDep.Value;
			}
			if (_chatControlDep.HasValue)
			{
				_chatControl = _chatControlDep.Value;
			}
			if (_garageControllerDep.HasValue)
			{
				_garageController = _garageControllerDep.Value;
			}
			if (_levelsProviderDep.HasValue)
			{
				_levelsProvider = _levelsProviderDep.Value;
			}
			if (_racingProviderDep.HasValue)
			{
				_racingProvider = _racingProviderDep.Value;
			}
			if (_managerUiDep.HasValue)
			{
				_managerUi = _managerUiDep.Value;
			}
			if (_carInputManagerDep.HasValue)
			{
				_carInputManager = _carInputManagerDep.Value;
			}
			if ((bool)_circularMenuPanelsConfig)
			{
				CircularMenuPanelConfig[] circularMenuPanelsConfigs = _circularMenuPanelsConfig.CircularMenuPanelsConfigs;
				for (int i = 0; i < circularMenuPanelsConfigs.Length; i++)
				{
					CircularMenuPanelConfig value = circularMenuPanelsConfigs[i];
					_circularMenuPanelsConfigs.Add(value.amountOfElements, value);
				}
			}
			BuildActionMaps();
			HideAllPanels(_carPanelGroup.Panels);
			if (_carPassengerPanelGroup.HasPanels)
			{
				HideAllPanels(_carPassengerPanelGroup.Panels);
			}
			HideAllPanels(_personPanelGroup.Panels);
			HideAllPanels(_uiPanelGroup.Panels);
			_namePanel.SetActive(value: false);
			SetMode(_initialMode);
			_circleMenuEvent.Get(_eventRegistry).Subscribe(ChangeCircleMenuActiveState);
			_circleMenuScrollEvent.Get(_eventRegistry).Subscribe(ScrollPanel);
			_closeCircleMenu.Get(_eventRegistry).Subscribe(HideSubscription);
			if (_openEventId == EventRegistryIDs.OnCircleMenuCam)
			{
				_changeCameraHandle.Get(_eventRegistry).Subscribe(OnChangeCameraButton);
			}
			SubscribeAllGroups();
			V.OnOutOfCarChanged = (Action<bool>)Delegate.Combine(V.OnOutOfCarChanged, new Action<bool>(OnOutOfCarChanged));
			_inputMapSetEvent.Subscribe(HideIfCancelled);
		}

		private void OnDestroy()
		{
			_showTween?.Kill();
			_fadeTween?.Kill();
			_animationResetDisposable?.Dispose();
			_circleMenuEvent.Get(_eventRegistry).Unsubscribe(ChangeCircleMenuActiveState);
			_circleMenuScrollEvent.Get(_eventRegistry).Unsubscribe(ScrollPanel);
			_closeCircleMenu.Get(_eventRegistry).Unsubscribe(HideSubscription);
			if (_openEventId == EventRegistryIDs.OnCircleMenuCam)
			{
				_changeCameraHandle.Get(_eventRegistry).Unsubscribe(OnChangeCameraButton);
			}
			_inputMapSetEvent.Unsubscribe(HideIfCancelled);
			UnsubscribeAllGroups();
			V.OnOutOfCarChanged = (Action<bool>)Delegate.Remove(V.OnOutOfCarChanged, new Action<bool>(OnOutOfCarChanged));
		}

		private void HideSubscription()
		{
			Hide(isCancelled: true);
		}

		public void SetMode(CircularMenuMode mode, bool isCancelled = false)
		{
			_currentMode = mode;
			if (_isOpen)
			{
				Hide(isCancelled);
			}
			if ((bool)_activePanel)
			{
				DeactivatePanel(_activePanel);
			}
			CircularMenuPanelGroup circularMenuPanelGroup = GetGroup(mode);
			if (circularMenuPanelGroup.HasPanels)
			{
				SetActiveGroup(circularMenuPanelGroup.Panels);
				return;
			}
			_activePanels = null;
			_activePanel = null;
		}

		private CircularMenuPanelGroup GetGroup(CircularMenuMode mode)
		{
			return mode switch
			{
				CircularMenuMode.Car => _carPanelGroup, 
				CircularMenuMode.CarPassenger => _carPassengerPanelGroup.HasPanels ? _carPassengerPanelGroup : _carPanelGroup, 
				CircularMenuMode.Person => _personPanelGroup, 
				CircularMenuMode.UI => _uiPanelGroup, 
				_ => _carPanelGroup, 
			};
		}

		private void BuildActionMaps()
		{
			_actionMap = new Dictionary<ButtonActionId, Action>();
			_activeStateAvailabilityMap = new Dictionary<ButtonActionId, Func<bool>>();
			_colorChangeAvailabilityMap = new Dictionary<ButtonActionId, Func<bool>>();
			Func<bool> value = () => false;
			_activeStateAvailabilityMap[ButtonActionId.PoliceSirenSound] = value;
			_activeStateAvailabilityMap[ButtonActionId.PoliceSirenStatus] = value;
			_activeStateAvailabilityMap[ButtonActionId.SideDoor] = value;
			_activeStateAvailabilityMap[ButtonActionId.CarRoof] = value;
			_activeStateAvailabilityMap[ButtonActionId.Chat] = value;
			_actionMap[ButtonActionId.TransferCaseH2] = () =>
			{
				Powertrain powertrain = GetPowertrain();
				if ((bool)powertrain)
				{
					powertrain.TrySetTransferCaseMode(TransferCaseMode.H2, out var _);
				}
			};
			_actionMap[ButtonActionId.TransferCaseH4] = () =>
			{
				Powertrain powertrain = GetPowertrain();
				if ((bool)powertrain)
				{
					powertrain.TrySetTransferCaseMode(TransferCaseMode.H4, out var _);
				}
			};
			_actionMap[ButtonActionId.TransferCaseL4] = () =>
			{
				Powertrain powertrain = GetPowertrain();
				if ((bool)powertrain)
				{
					powertrain.TrySetTransferCaseMode(TransferCaseMode.L4, out var _);
				}
			};
			_actionMap[ButtonActionId.TransferCaseFrontLock] = () =>
			{
				Powertrain powertrain = GetPowertrain();
				if ((bool)powertrain && powertrain.TransferCase != null)
				{
					powertrain.TrySetTransferCaseFrontLock(!powertrain.TransferCase.FrontLockOn, out var _);
				}
			};
			_actionMap[ButtonActionId.TransferCaseCenterLock] = () =>
			{
				Powertrain powertrain = GetPowertrain();
				if ((bool)powertrain && powertrain.TransferCase != null)
				{
					powertrain.TrySetTransferCaseCenterLock(!powertrain.TransferCase.CenterLockOn, out var _);
				}
			};
			_actionMap[ButtonActionId.TransferCaseRearLock] = () =>
			{
				Powertrain powertrain = GetPowertrain();
				if ((bool)powertrain && powertrain.TransferCase != null)
				{
					powertrain.TrySetTransferCaseRearLock(!powertrain.TransferCase.RearLockOn, out var _);
				}
			};
			_activeStateAvailabilityMap[ButtonActionId.TransferCaseH2] = () =>
			{
				TransferCase transferCase = GetTransferCase();
				return transferCase != null && !transferCase.IsFullTime;
			};
			_activeStateAvailabilityMap[ButtonActionId.TransferCaseH4] = () => GetTransferCase() != null;
			_activeStateAvailabilityMap[ButtonActionId.TransferCaseL4] = () => GetTransferCase()?.HasLowRange ?? false;
			_activeStateAvailabilityMap[ButtonActionId.TransferCaseFrontLock] = () => GetTransferCase()?.FrontLockAvailable ?? false;
			_activeStateAvailabilityMap[ButtonActionId.TransferCaseCenterLock] = () =>
			{
				TransferCase transferCase = GetTransferCase();
				return transferCase != null && transferCase.IsFullTime && transferCase.HasCenterLock;
			};
			_activeStateAvailabilityMap[ButtonActionId.TransferCaseRearLock] = () => GetTransferCase()?.HasRearLock ?? false;
			_colorChangeAvailabilityMap[ButtonActionId.TransferCaseCenterLock] = () =>
			{
				TransferCase transferCase = GetTransferCase();
				return transferCase != null && transferCase.Mode != TransferCaseMode.L4;
			};
			_actionMap[ButtonActionId.AutoRunToggle] = () =>
			{
				GetPersonLocomotion()?.ToggleAutoRun();
			};
			if (_cruiseControl != null)
			{
				_actionMap[ButtonActionId.CruiseToggle] = _cruiseControl.EnableToggle;
				_actionMap[ButtonActionId.CruisePlus] = _cruiseControl.Plus;
				_actionMap[ButtonActionId.CruiseMinus] = _cruiseControl.Minus;
				_colorChangeAvailabilityMap[ButtonActionId.CruisePlus] = _cruiseControl.GetPlusAndMinusButtonActiveState;
				_colorChangeAvailabilityMap[ButtonActionId.CruiseMinus] = _cruiseControl.GetPlusAndMinusButtonActiveState;
			}
			if ((bool)_garageController)
			{
				_actionMap[ButtonActionId.ResetCarPosition] = () =>
				{
					_garageController.MenuGarageNavigationWindow?.ResetCarPosition();
				};
				if (!_quickCarSettings)
				{
					_quickCarSettings = _garageController.MenuGarageNavigationWindow?.GetQuickCarSettings();
				}
			}
			if ((bool)_quickCarSettings)
			{
				_actionMap[ButtonActionId.KeepRow] = _quickCarSettings.KeepRow;
				_actionMap[ButtonActionId.TurnSignalCenter] = _quickCarSettings.TurnOnEmergency;
				_actionMap[ButtonActionId.TurnSignalLeft] = _quickCarSettings.LeftIndicatorOn;
				_actionMap[ButtonActionId.TurnSignalRight] = _quickCarSettings.RightIndicatorOn;
				_actionMap[ButtonActionId.CarRoof] = _quickCarSettings.RoofButtonPressed;
				_actionMap[ButtonActionId.CarHood] = _quickCarSettings.HoodButtonPressed;
				_actionMap[ButtonActionId.CarTrunk] = _quickCarSettings.TrunkButtonPressed;
				_actionMap[ButtonActionId.CarHeadlights] = _quickCarSettings.TurnOnLights;
				_activeStateAvailabilityMap[ButtonActionId.SideDoor] = _quickCarSettings.GetSideDoorButtonActiveState;
				_activeStateAvailabilityMap[ButtonActionId.CarRoof] = _quickCarSettings.GetCarRoofButtonActiveState;
				_colorChangeAvailabilityMap[ButtonActionId.CarHood] = _quickCarSettings.GetCarHoodInteractableLive;
				_colorChangeAvailabilityMap[ButtonActionId.CarTrunk] = _quickCarSettings.GetCarTrunkInteractableLive;
				_visualRefreshMap[ButtonActionId.CarHeadlights] = RefreshHeadlightIcon;
			}
			if (_freeDriveProvider != null)
			{
				_actionMap[ButtonActionId.ToRoad] = () =>
				{
					_freeDriveProvider.FreeDriveControl?.ToRoad();
				};
				_actionMap[ButtonActionId.ShowPlayerList] = () =>
				{
					_freeDriveProvider.FreeDriveControl?.ShowPlayersList();
				};
				_actionMap[ButtonActionId.InviteToRace] = () =>
				{
					_freeDriveProvider.FreeDriveControl?.InviteToRace();
				};
				_colorChangeAvailabilityMap[ButtonActionId.ToRoad] = () => (bool)_freeDriveProvider.FreeDriveControl && _freeDriveProvider.FreeDriveControl.GetToRoadButtonActiveState();
			}
			if ((bool)_parkingMenuUI)
			{
				_colorChangeAvailabilityMap[ButtonActionId.ChangeCameraSettings] = _parkingMenuUI.GetCameraSettingsButtonActiveState;
			}
			if ((bool)_lockCarButtonController)
			{
				_colorChangeAvailabilityMap[ButtonActionId.CarLock] = _lockCarButtonController.GetLockButtonActiveState;
				_visualRefreshMap[ButtonActionId.CarLock] = (CircularMenuItem item) =>
				{
					_lockCarButtonController.UpdateLockButtonImage(item.Icon);
				};
			}
			_activeStateAvailabilityMap[ButtonActionId.PoliceSirenSound] = () => _policeRegistry?.Controller != null && _policeRegistry.Controller.GetPoliceButtonsActiveState();
			_activeStateAvailabilityMap[ButtonActionId.PoliceSirenStatus] = () => _policeRegistry?.Controller != null && _policeRegistry.Controller.GetPoliceButtonsActiveState();
			_visualRefreshMap[ButtonActionId.PoliceSirenSound] = (CircularMenuItem item) =>
			{
				_policeRegistry?.Controller?.PoliceSirenSound?.UpdatePoliceSirenSoundButtonImage(item.Icon);
			};
			_visualRefreshMap[ButtonActionId.PoliceSirenStatus] = (CircularMenuItem item) =>
			{
				_policeRegistry?.Controller?.PoliceSirenStatus?.UpdatePoliceSirenStatusButtonImage(item.Icon);
			};
			if ((bool)_chatControl)
			{
				_actionMap[ButtonActionId.Chat] = () =>
				{
					_chatControl?.ShowChatFull();
				};
				_activeStateAvailabilityMap[ButtonActionId.Chat] = () => (bool)_chatControl.ChatWindow && _chatControl.ChatWindow.GetChatMaximizeButtonActiveState();
			}
			if ((bool)_menuLevelParent)
			{
				_actionMap[ButtonActionId.Emoji] = ShowEmoteLocker;
			}
			if (_levelsProvider != null)
			{
				_actionMap[ButtonActionId.CameraDirection] = () =>
				{
					_levelsProvider.LevelScript?.SetCameraByIndex(2);
				};
				_actionMap[ButtonActionId.CameraDynamic] = () =>
				{
					_levelsProvider.LevelScript?.SetCameraByIndex(5);
				};
			}
			if (_racingProvider != null)
			{
				_actionMap[ButtonActionId.CameraClose] = () =>
				{
					SwitchRacingCamera(DragRacingCamera.CameraType.close);
				};
				_actionMap[ButtonActionId.CameraMiddle] = () =>
				{
					SwitchRacingCamera(DragRacingCamera.CameraType.middle);
				};
				_actionMap[ButtonActionId.CameraFar] = () =>
				{
					SwitchRacingCamera(DragRacingCamera.CameraType.far);
				};
			}
			_actionMap[ButtonActionId.CameraInside] = SwitchInsideCamera;
			_actionMap[ButtonActionId.CameraFree] = SwitchFreeCamera;
			_actionMap[ButtonActionId.CameraFixed] = SwitchFixedCamera;
			_actionMap[ButtonActionId.CameraPersonThird] = () =>
			{
				SwitchPersonCamera(0);
			};
			_actionMap[ButtonActionId.CameraPersonBack] = () =>
			{
				SwitchPersonCamera(1);
			};
			_actionMap[ButtonActionId.CameraPersonTop] = () =>
			{
				SwitchPersonCamera(2);
			};
			_actionMap[ButtonActionId.CameraPersonFirst] = () =>
			{
				SwitchPersonCamera(3);
			};
			_actionMap[ButtonActionId.FpvCameraSetup] = StartFpvCameraSetup;
			_colorChangeAvailabilityMap[ButtonActionId.FpvCameraSetup] = IsFirstPersonView;
		}

		private void SwitchInsideCamera()
		{
			MenuGarageNavigationWindow menuGarageNavigationWindow = (_garageController ? _garageController.MenuGarageNavigationWindow : null);
			if (menuGarageNavigationWindow != null)
			{
				menuGarageNavigationWindow.SetInsideCamera();
			}
			else if (_racingProvider != null && ((bool)_racingProvider.RacingCameraController || (_racingProvider.DragRacingController != null && (bool)_racingProvider.DragRacingController.dragRacingCamera)))
			{
				SwitchRacingCamera(DragRacingCamera.CameraType.driverView);
			}
			else
			{
				_levelsProvider?.LevelScript?.SetCameraByIndex(0);
			}
		}

		private void SwitchFreeCamera()
		{
			MenuGarageNavigationWindow menuGarageNavigationWindow = (_garageController ? _garageController.MenuGarageNavigationWindow : null);
			if (menuGarageNavigationWindow != null)
			{
				menuGarageNavigationWindow.SetFreeCamera();
			}
			else
			{
				_levelsProvider?.LevelScript?.SetCameraByIndex(1);
			}
		}

		private void SwitchFixedCamera()
		{
			if (_racingProvider != null && ((bool)_racingProvider.RacingCameraController || (_racingProvider.DragRacingController != null && (bool)_racingProvider.DragRacingController.dragRacingCamera)))
			{
				SwitchRacingCamera(DragRacingCamera.CameraType.far);
			}
			else
			{
				_levelsProvider?.LevelScript?.SetCameraByIndex(3);
			}
		}

		private void RefreshHeadlightIcon(CircularMenuItem item)
		{
			if ((bool)_quickCarSettings)
			{
				_quickCarSettings.UpdateHeadlightIcon(item.Icon);
			}
		}

		private void SwitchRacingCamera(DragRacingCamera.CameraType type)
		{
			if (_racingProvider == null)
			{
				return;
			}
			if ((bool)_racingProvider.RacingCameraController)
			{
				_racingProvider.RacingCameraController.ChangeCameraOnCar(type);
				return;
			}
			DragRacingController dragRacingController = _racingProvider.DragRacingController;
			if (dragRacingController != null && (bool)dragRacingController.dragRacingCamera)
			{
				dragRacingController.dragRacingCamera.ChangeCameraOnCar(type);
			}
		}

		private void SwitchPersonCamera(int index)
		{
			CarCamerasController carCamerasController = (V.CoreGameManager ? V.CoreGameManager.carCamerasController : null);
			if (carCamerasController != null)
			{
				carCamerasController.SetPersonCameraIndex(index);
			}
		}

		private bool IsFirstPersonView()
		{
			if (_racingProvider != null)
			{
				if ((bool)_racingProvider.RacingCameraController)
				{
					return _racingProvider.RacingCameraController.currentCamera == DragRacingCamera.CameraType.driverView;
				}
				DragRacingController dragRacingController = _racingProvider.DragRacingController;
				if (dragRacingController != null && (bool)dragRacingController.dragRacingCamera)
				{
					return dragRacingController.dragRacingCamera.currentCamera == DragRacingCamera.CameraType.driverView;
				}
			}
			CarCamerasController carCamerasController = (V.CoreGameManager ? V.CoreGameManager.carCamerasController : null);
			if ((bool)carCamerasController)
			{
				return carCamerasController.IsFirstPersonView;
			}
			return false;
		}

		private void StartFpvCameraSetup()
		{
			_inputMapSetEvent.TrySetGroup(InputGroupsEnum.CameraSetup, setRoot: true);
		}

		private void SubscribeAllGroups()
		{
			SubscribePanels(_carPanelGroup.Panels);
			if (_carPassengerPanelGroup.HasPanels)
			{
				SubscribePanels(_carPassengerPanelGroup.Panels);
			}
			SubscribePanels(_personPanelGroup.Panels);
			SubscribePanels(_uiPanelGroup.Panels);
		}

		private void UnsubscribeAllGroups()
		{
			UnsubscribePanels(_carPanelGroup.Panels);
			if (_carPassengerPanelGroup.HasPanels)
			{
				UnsubscribePanels(_carPassengerPanelGroup.Panels);
			}
			UnsubscribePanels(_personPanelGroup.Panels);
			UnsubscribePanels(_uiPanelGroup.Panels);
		}

		private void SubscribePanels(CircularMenuPanel[] panels)
		{
			if (panels != null)
			{
				for (int i = 0; i < panels.Length; i++)
				{
					SubscribePanel(panels[i]);
				}
			}
		}

		private void UnsubscribePanels(CircularMenuPanel[] panels)
		{
			if (panels != null)
			{
				for (int i = 0; i < panels.Length; i++)
				{
					UnsubscribePanel(panels[i]);
				}
			}
		}

		private void SubscribePanel(CircularMenuPanel panel)
		{
			if (!panel)
			{
				return;
			}
			if (_isAnimationMenu)
			{
				SubscribeAnimationPanel(panel);
				return;
			}
			IReadOnlyList<CircularItemBinding> bindings = panel.Bindings;
			for (int i = 0; i < bindings.Count; i++)
			{
				SubscribeBinding(bindings[i]);
			}
		}

		private void UnsubscribePanel(CircularMenuPanel panel)
		{
			if (!panel)
			{
				return;
			}
			if (_isAnimationMenu)
			{
				CircularMenuItem[] items = panel.GetItems();
				foreach (CircularMenuItem circularMenuItem in items)
				{
					if ((bool)circularMenuItem && _subscribedHandlers.TryGetValue(circularMenuItem, out var value))
					{
						circularMenuItem.onSelectedEvent = (Action)Delegate.Remove(circularMenuItem.onSelectedEvent, value);
						_subscribedHandlers.Remove(circularMenuItem);
					}
				}
				return;
			}
			IReadOnlyList<CircularItemBinding> bindings = panel.Bindings;
			for (int j = 0; j < bindings.Count; j++)
			{
				CircularMenuItem circularMenuItem2 = bindings[j].CircularMenuItem;
				if ((bool)circularMenuItem2 && _subscribedHandlers.TryGetValue(circularMenuItem2, out var value2))
				{
					circularMenuItem2.onSelectedEvent = (Action)Delegate.Remove(circularMenuItem2.onSelectedEvent, value2);
					_subscribedHandlers.Remove(circularMenuItem2);
				}
			}
		}

		private void SubscribeAnimationPanel(CircularMenuPanel panel)
		{
			CircularMenuItem[] items = panel.GetItems();
			foreach (CircularMenuItem circularMenuItem in items)
			{
				if ((bool)circularMenuItem && circularMenuItem.AnimationSlot != -1)
				{
					int slot = circularMenuItem.AnimationSlot;
					Action action = ((slot == -2) ? new Action(OpenAnimationShop) : ((Action)(() =>
					{
						PlayAnimationSlot(slot);
					})));
					circularMenuItem.onSelectedEvent = (Action)Delegate.Combine(circularMenuItem.onSelectedEvent, action);
					_subscribedHandlers[circularMenuItem] = action;
				}
			}
		}

		private MenuGarageAnimationPanel FindGarageAnimationPanel()
		{
			MenuGarageNavigationWindow menuGarageNavigationWindow = (_garageController ? _garageController.MenuGarageNavigationWindow : null);
			if (!menuGarageNavigationWindow)
			{
				return null;
			}
			return menuGarageNavigationWindow.GetComponentInChildren<MenuGarageAnimationPanel>(includeInactive: true);
		}

		private void ShowEmoteLocker()
		{
			MenuGarageAnimationPanel menuGarageAnimationPanel = FindGarageAnimationPanel();
			if ((bool)menuGarageAnimationPanel && (bool)_managerUi && !_managerUi.IsWindowActive("EmoteLockerWindow"))
			{
				menuGarageAnimationPanel.OpenEmoteLockerExternal();
			}
			else
			{
				_menuLevelParent.ShowEmoji();
			}
		}

		private void OpenAnimationShop()
		{
			MenuGarageAnimationPanel menuGarageAnimationPanel = FindGarageAnimationPanel();
			if ((bool)menuGarageAnimationPanel)
			{
				menuGarageAnimationPanel.OpenAnimationShopExternal();
			}
			else
			{
				_managerUi?.ShowFreeAnimationWindow();
			}
		}

		private void PlayAnimationSlot(int slot)
		{
			if (_animationDataSo == null || !V.DriverPerson)
			{
				return;
			}
			int num = PlayerPrefs.GetInt("currentAnimation_" + slot, slot + 1);
			if (num < 0 || num >= _animationDataSo.animations.Count || !V.DriverPerson.TryGetComponent<NewMoveController>(out var component))
			{
				return;
			}
			AnimationDataSO.AnimationDataItem animationDataItem = _animationDataSo.animations[num];
			component.Input.Emotion = animationDataItem.Y;
			_animationResetDisposable?.Dispose();
			if (animationDataItem.animationClip != null && !animationDataItem.animationClip.isLooping)
			{
				_animationResetDisposable = Observable.Timer(TimeSpan.FromSeconds(animationDataItem.duration)).Subscribe((long _) =>
				{
					ResetAnimationEmotion();
				}).AddTo(this);
			}
		}

		private void ResetAnimationEmotion()
		{
			if ((bool)V.DriverPerson && V.DriverPerson.TryGetComponent<NewMoveController>(out var component))
			{
				component.Input.Emotion = 0;
			}
		}

		private void RefreshAnimationSlotIcons()
		{
			if (!_isAnimationMenu || _animationDataSo == null || _items == null)
			{
				return;
			}
			for (int i = 0; i < _items.Length; i++)
			{
				CircularMenuItem circularMenuItem = _items[i];
				if ((bool)circularMenuItem && circularMenuItem.AnimationSlot >= 0)
				{
					int num = PlayerPrefs.GetInt("currentAnimation_" + circularMenuItem.AnimationSlot, circularMenuItem.AnimationSlot + 1);
					if (num >= 0 && num < _animationDataSo.animations.Count)
					{
						circularMenuItem.Icon.sprite = _animationDataSo.animations[num].sprite;
					}
				}
			}
		}

		private void SubscribeBinding(CircularItemBinding binding)
		{
			if ((bool)binding.CircularMenuItem && binding.ActionId != ButtonActionId.None && !_subscribedHandlers.ContainsKey(binding.CircularMenuItem))
			{
				Action action = ResolveHandler(binding);
				if (action != null)
				{
					CircularMenuItem circularMenuItem = binding.CircularMenuItem;
					circularMenuItem.onSelectedEvent = (Action)Delegate.Combine(circularMenuItem.onSelectedEvent, action);
					_subscribedHandlers[binding.CircularMenuItem] = action;
				}
			}
		}

		private Action ResolveHandler(CircularItemBinding binding)
		{
			switch (binding.ActionId)
			{
			case ButtonActionId.SideDoor:
			{
				CircularMenuItem doorBtn = binding.CircularMenuItem;
				return () =>
				{
					_quickCarSettings.OpenCloseSideDoor(doorBtn.Icon);
				};
			}
			case ButtonActionId.CarLock:
			{
				CircularMenuItem lockButton = binding.CircularMenuItem;
				return () =>
				{
					_lockCarButtonController.KickPassengersButtonPressed();
					_lockCarButtonController.UpdateLockButtonImage(lockButton.Icon);
				};
			}
			case ButtonActionId.ChangeCameraSettings:
				if (!_parkingMenuUI)
				{
					return null;
				}
				return _parkingMenuUI.OpenCameraSettingsWindow;
			case ButtonActionId.PoliceSirenSound:
			{
				CircularMenuItem policeSirenSoundButton = binding.CircularMenuItem;
				return () =>
				{
					_policeRegistry?.Controller?.PoliceSirenSound?.PoliceSirenSound();
					_policeRegistry?.Controller?.PoliceSirenSound?.UpdatePoliceSirenSoundButtonImage(policeSirenSoundButton.Icon);
				};
			}
			case ButtonActionId.PoliceSirenStatus:
			{
				CircularMenuItem policeSirenStatusButton = binding.CircularMenuItem;
				return () =>
				{
					_policeRegistry?.Controller?.PoliceSirenStatus?.PoliceSirenStatus();
					_policeRegistry?.Controller?.PoliceSirenStatus?.UpdatePoliceSirenStatusButtonImage(policeSirenStatusButton.Icon);
				};
			}
			default:
			{
				_actionMap.TryGetValue(binding.ActionId, out var value);
				return value;
			}
			}
		}

		private void OnOutOfCarChanged(bool isPerson)
		{
			CircularMenuMode mode = (isPerson ? CircularMenuMode.Person : (FlagsStorage.IsPassenger ? CircularMenuMode.CarPassenger : CircularMenuMode.Car));
			SetMode(mode, isCancelled: true);
		}

		private void SyncModeToState()
		{
			CircularMenuMode circularMenuMode = (FlagsStorage.OutOfCar ? CircularMenuMode.Person : (FlagsStorage.IsPassenger ? CircularMenuMode.CarPassenger : CircularMenuMode.Car));
			if (circularMenuMode != _currentMode)
			{
				SetMode(circularMenuMode, isCancelled: true);
			}
		}

		private Powertrain GetPowertrain()
		{
			if (!_carInputManager)
			{
				return null;
			}
			return _carInputManager.GetPowertrain;
		}

		private PersonLocomotion GetPersonLocomotion()
		{
			if (!V.DriverPerson)
			{
				return null;
			}
			return V.DriverPerson.GetComponent<PersonLocomotion>();
		}

		private TransferCase GetTransferCase()
		{
			Powertrain powertrain = GetPowertrain();
			if (!powertrain)
			{
				return null;
			}
			return powertrain.TransferCase;
		}

		private CircularMenuPanel[] FilterAvailablePanels(CircularMenuPanel[] panels)
		{
			if (panels == null || panels.Length == 0)
			{
				return panels;
			}
			bool flag = false;
			for (int i = 0; i < panels.Length; i++)
			{
				if ((bool)panels[i] && panels[i].RequiresTransferCase && !HasEnoughTransferCaseOptions(panels[i]))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return panels;
			}
			List<CircularMenuPanel> list = new List<CircularMenuPanel>(panels.Length);
			for (int j = 0; j < panels.Length; j++)
			{
				if (!panels[j] || !panels[j].RequiresTransferCase || HasEnoughTransferCaseOptions(panels[j]))
				{
					list.Add(panels[j]);
				}
			}
			return list.ToArray();
		}

		private bool HasEnoughTransferCaseOptions(CircularMenuPanel panel)
		{
			if (GetTransferCase() == null)
			{
				return false;
			}
			int num = 0;
			IReadOnlyList<CircularItemBinding> bindings = panel.Bindings;
			for (int i = 0; i < bindings.Count; i++)
			{
				ButtonActionId actionId = bindings[i].ActionId;
				if ((!_activeStateAvailabilityMap.TryGetValue(actionId, out var value) || value()) && ++num >= 2)
				{
					return true;
				}
			}
			return false;
		}

		private void SetActiveGroup(CircularMenuPanel[] panels)
		{
			_activePanels = FilterAvailablePanels(panels);
			_activePanelIndex = 0;
			if (_activePanels == null || _activePanels.Length == 0)
			{
				_activePanel = null;
				return;
			}
			_activePanel = _activePanels[_activePanelIndex];
			_items = _activePanel.GetItems();
			_namePanelText.text = _activePanel.panelNameID.Value;
			UpdateCircularMenuPanelUIElements(ItemsCount);
		}

		private void RefreshActiveGroup()
		{
			CircularMenuPanelGroup circularMenuPanelGroup = GetGroup(_currentMode);
			if (!circularMenuPanelGroup.HasPanels || SamePanels(FilterAvailablePanels(circularMenuPanelGroup.Panels), _activePanels))
			{
				return;
			}
			CircularMenuPanel activePanel = _activePanel;
			SetActiveGroup(circularMenuPanelGroup.Panels);
			if (!activePanel || _activePanels == null)
			{
				return;
			}
			for (int i = 0; i < _activePanels.Length; i++)
			{
				if (!(_activePanels[i] != activePanel))
				{
					_activePanelIndex = i;
					_activePanel = activePanel;
					_items = _activePanel.GetItems();
					_namePanelText.text = _activePanel.panelNameID.Value;
					UpdateCircularMenuPanelUIElements(ItemsCount);
					break;
				}
			}
		}

		private static bool SamePanels(CircularMenuPanel[] a, CircularMenuPanel[] b)
		{
			if (a == null || b == null)
			{
				return a == b;
			}
			if (a.Length != b.Length)
			{
				return false;
			}
			for (int i = 0; i < a.Length; i++)
			{
				if (a[i] != b[i])
				{
					return false;
				}
			}
			return true;
		}

		private void ScrollPanel(bool isForward)
		{
			if (_isOpen && CanScroll)
			{
				DeactivatePanel(_activePanel);
				_activePanelIndex = (isForward ? ((_activePanelIndex + 1) % _activePanels.Length) : ((_activePanelIndex - 1 + _activePanels.Length) % _activePanels.Length));
				ActivatePanel(_activePanels[_activePanelIndex]);
				RefreshButtonVisuals();
				UpdateCircularMenuPanelUIElements(ItemsCount);
				ClearHover();
			}
		}

		private void RefreshConditionalButtons()
		{
			if (_isAnimationMenu)
			{
				return;
			}
			bool flag = false;
			IReadOnlyList<CircularItemBinding> bindings = _activePanel.Bindings;
			for (int i = 0; i < bindings.Count; i++)
			{
				CircularItemBinding circularItemBinding = bindings[i];
				if (!circularItemBinding.CircularMenuItem)
				{
					continue;
				}
				if (_activeStateAvailabilityMap.TryGetValue(circularItemBinding.ActionId, out var value))
				{
					bool flag2 = value();
					if (circularItemBinding.CircularMenuItem.gameObject.activeSelf != flag2)
					{
						circularItemBinding.CircularMenuItem.SetActive(flag2);
						flag = true;
						if (!flag2 && _hoveredItem == circularItemBinding.CircularMenuItem)
						{
							ClearHover();
						}
					}
				}
				if (_colorChangeAvailabilityMap.TryGetValue(circularItemBinding.ActionId, out var value2))
				{
					bool flag3 = value2();
					circularItemBinding.CircularMenuItem.IsActive = flag3;
					circularItemBinding.CircularMenuItem.ChangeColorOfIcon(flag3);
					if (!flag3 && _hoveredItem == circularItemBinding.CircularMenuItem)
					{
						ClearHover();
					}
				}
			}
			if (flag)
			{
				_activePanel.InvalidateItemsCache();
				_items = _activePanel.GetItems();
				UpdateCircularMenuPanelUIElements(ItemsCount);
			}
		}

		private void ChangeCircleMenuActiveState(bool isGamepad)
		{
			_isGamepad = isGamepad;
			if (_isOpen)
			{
				CircularMenuItem hoveredItem = _hoveredItem;
				Hide();
				hoveredItem?.SetPressed(_circularMenuPanelsConfigs[ItemsCount].pressedSprite);
				hoveredItem?.OnSelect();
			}
			else
			{
				bool flag = _openEventId == EventRegistryIDs.OnCircleMenuCam;
				if (!IsUiHidden() && (!FlagsStorage.IsSpectatorMode || flag) && !IsCircleMenuGroupActive() && !IsPersonRagdolled())
				{
					Show();
				}
			}
		}

		private bool IsPersonRagdolled()
		{
			RagdollCPM ragdollCPM = ((!V.DriverPerson) ? null : V.DriverPerson.GetComponent<DriverActionController>()?.ragdollCpm);
			if ((bool)ragdollCPM)
			{
				if (!ragdollCPM.isRagdoll)
				{
					return ragdollCPM.isStadningUp;
				}
				return true;
			}
			return false;
		}

		private bool IsCircleMenuGroupActive()
		{
			InputGroupsEnum currentInputGroup = _inputMapSetEvent.CurrentInputGroup;
			return currentInputGroup == InputGroupsEnum.CarCircleMenu || currentInputGroup == InputGroupsEnum.PersonCircleMenu || currentInputGroup == InputGroupsEnum.CarFreeRaceCircleMenu || currentInputGroup == InputGroupsEnum.CarPassengerCircleMenu;
		}

		private bool IsUiHidden()
		{
			if (_hiddenCheckGroups == null)
			{
				_hiddenCheckGroups = new List<CanvasGroup>();
			}
			GetComponentsInParent(includeInactive: true, _hiddenCheckGroups);
			for (int i = 0; i < _hiddenCheckGroups.Count; i++)
			{
				CanvasGroup canvasGroup = _hiddenCheckGroups[i];
				if (!canvasGroup.gameObject.activeInHierarchy || canvasGroup.alpha < 0.01f)
				{
					return true;
				}
				if (canvasGroup.ignoreParentGroups)
				{
					break;
				}
			}
			return false;
		}

		private void OnChangeCameraButton()
		{
			if (FlagsStorage.InGarageMenu || _isAnimationMenu || _activePanel == null || IsUiHidden())
			{
				return;
			}
			_cameraCycleBindings.Clear();
			IReadOnlyList<CircularItemBinding> bindings = _activePanel.Bindings;
			for (int i = 0; i < bindings.Count; i++)
			{
				CircularItemBinding item = bindings[i];
				if ((bool)item.CircularMenuItem && item.CircularMenuItem.gameObject.activeSelf && IsCameraAction(item.ActionId))
				{
					_cameraCycleBindings.Add(item);
				}
			}
			if (_cameraCycleBindings.Count == 0)
			{
				return;
			}
			int num = -1;
			for (int j = 0; j < _cameraCycleBindings.Count; j++)
			{
				if (_cameraCycleBindings[j].CircularMenuItem == _lastCameraItem)
				{
					num = j;
					break;
				}
			}
			int index = (num + 1) % _cameraCycleBindings.Count;
			_lastCameraItem = _cameraCycleBindings[index].CircularMenuItem;
			_lastCameraItem.OnSelect();
		}

		private static bool IsCameraAction(ButtonActionId id)
		{
			if (id >= ButtonActionId.CameraInside)
			{
				return id <= ButtonActionId.CameraPersonFirst;
			}
			return false;
		}

		private void HideIfCancelled(InputGroupsEnum inputGroup)
		{
			if (inputGroup != InputGroupsEnum.CarCircleMenu && inputGroup != InputGroupsEnum.PersonCircleMenu && inputGroup != InputGroupsEnum.CarFreeRaceCircleMenu && inputGroup != InputGroupsEnum.CarPassengerCircleMenu)
			{
				Hide(isCancelled: true);
			}
		}

		private bool TryToSetInputGroup()
		{
			InputGroupsEnum newGroup;
			switch (_inputMapSetEvent.CurrentInputGroup)
			{
			case InputGroupsEnum.CarFree:
				_groupBeforeMenu = InputGroupsEnum.CarFree;
				newGroup = InputGroupsEnum.CarCircleMenu;
				break;
			case InputGroupsEnum.Person:
				_groupBeforeMenu = InputGroupsEnum.Person;
				newGroup = InputGroupsEnum.PersonCircleMenu;
				break;
			case InputGroupsEnum.CarPassenger:
				_groupBeforeMenu = InputGroupsEnum.CarPassenger;
				newGroup = InputGroupsEnum.CarPassengerCircleMenu;
				break;
			case InputGroupsEnum.Spectator:
				_groupBeforeMenu = InputGroupsEnum.Spectator;
				newGroup = InputGroupsEnum.CarCircleMenu;
				break;
			case InputGroupsEnum.CarRacing:
			case InputGroupsEnum.CarDrag:
			case InputGroupsEnum.CarRally:
			case InputGroupsEnum.CarFreeRace:
				_groupBeforeMenu = _inputMapSetEvent.CurrentInputGroup;
				newGroup = InputGroupsEnum.CarFreeRaceCircleMenu;
				break;
			case InputGroupsEnum.CarCircleMenu:
			case InputGroupsEnum.PersonCircleMenu:
			case InputGroupsEnum.CarPassengerCircleMenu:
			case InputGroupsEnum.CarFreeRaceCircleMenu:
				newGroup = ((_groupBeforeMenu != InputGroupsEnum.None) ? _groupBeforeMenu : InputGroupsEnum.CarFree);
				break;
			default:
				newGroup = InputGroupsEnum.None;
				break;
			}
			return _inputMapSetEvent.TrySetGroup(newGroup);
		}

		public void Show()
		{
			if (!_isOpen)
			{
				SyncModeToState();
				RefreshActiveGroup();
				if ((bool)_activePanel && TryToSetInputGroup())
				{
					_activePanel.SetActive(active: true);
					RefreshConditionalButtons();
					RefreshButtonVisuals();
					_isOpen = true;
					_items = _activePanel.GetItems();
					RefreshAnimationSlotIcons();
					_namePanel.SetActive(CanScroll);
					ClearHover();
					_accumulatedInput = Vector2.zero;
					_wasStickActive = false;
					_showTween?.Kill();
					_fadeTween?.Kill();
					CanvasGroup contentParentCanvasGroup = _activePanel.contentParentCanvasGroup;
					_showTween = contentParentCanvasGroup.transform.DOScale(Vector3.one, _activePanel.showDuration).SetEase(Ease.OutBack).SetUpdate(isIndependentUpdate: true);
					_fadeTween = contentParentCanvasGroup.DOFade(1f, _activePanel.showDuration).SetUpdate(isIndependentUpdate: true);
					contentParentCanvasGroup.interactable = true;
					contentParentCanvasGroup.blocksRaycasts = true;
				}
			}
		}

		public void Hide(bool isCancelled = false)
		{
			if (_isOpen)
			{
				_isOpen = false;
				ClearHover();
				_showTween?.Kill();
				_fadeTween?.Kill();
				_namePanel.SetActive(value: false);
				CanvasGroup contentParentCanvasGroup = _activePanel.contentParentCanvasGroup;
				CircularMenuPanel panelToHide = _activePanel;
				_showTween = contentParentCanvasGroup.transform.DOScale(Vector3.zero, _activePanel.hideDuration).SetEase(Ease.InBack).SetUpdate(isIndependentUpdate: true);
				_fadeTween = contentParentCanvasGroup.DOFade(0f, _activePanel.hideDuration).SetUpdate(isIndependentUpdate: true).OnComplete(() =>
				{
					panelToHide.SetActive(active: false);
					ResetCircularMenuItems(ItemsCount);
				});
				contentParentCanvasGroup.interactable = false;
				contentParentCanvasGroup.blocksRaycasts = false;
				if (!isCancelled)
				{
					TryToSetInputGroup();
				}
			}
		}

		private void Update()
		{
			if (!_isOpen || _items == null || _items.Length == 0)
			{
				return;
			}
			if (IsPersonRagdolled())
			{
				Hide(isCancelled: true);
				return;
			}
			RefreshConditionalButtons();
			Vector2 value = _circleMenuMoveEvent.Get(_eventRegistry).Value;
			if (_isGamepad)
			{
				_accumulatedInput = value;
				float magnitude = _accumulatedInput.magnitude;
				if (magnitude >= _activePanel.selectionThreshold)
				{
					_wasStickActive = true;
				}
				else if (_wasStickActive && magnitude < _activePanel.deadZone)
				{
					CircularMenuItem hoveredItem = _hoveredItem;
					Hide();
					hoveredItem?.SetPressed(_circularMenuPanelsConfigs[ItemsCount].pressedSprite);
					hoveredItem?.OnSelect();
					return;
				}
			}
			else
			{
				_accumulatedInput += value * _activePanel.mouseSensitivity;
				if (_accumulatedInput.sqrMagnitude > 1f)
				{
					_accumulatedInput = _accumulatedInput.normalized;
				}
			}
			_inputDirection = _accumulatedInput;
			UpdateSelection();
		}

		public void Confirm()
		{
			if ((bool)_hoveredItem)
			{
				_hoveredItem.OnSelect();
			}
		}

		private void UpdateSelection()
		{
			float magnitude = _inputDirection.magnitude;
			if (magnitude < _activePanel.deadZone)
			{
				ClearHover();
				return;
			}
			Vector2 normalized = _inputDirection.normalized;
			Mathf.Clamp01(magnitude);
			if (magnitude < _activePanel.selectionThreshold)
			{
				if (_isGamepad && (bool)_hoveredItem)
				{
					_hoveredItem.SetDefault(_circularMenuPanelsConfigs[ItemsCount].normalSprite);
					_middleZoneFrames++;
					if (_middleZoneFrames > 3)
					{
						ClearHover();
					}
				}
				else
				{
					ClearHover();
				}
				return;
			}
			_middleZoneFrames = 0;
			float current = Mathf.Atan2(normalized.y, normalized.x) * 57.29578f;
			CircularMenuItem circularMenuItem = null;
			float num = float.MaxValue;
			for (int i = 0; i < _items.Length; i++)
			{
				if (_items[i].IsActive)
				{
					Vector3 localPosition = _items[i].transform.localPosition;
					float target;
					if (_activePanel.parentRadialLayout.fDistance > 0.01f)
					{
						target = Mathf.Atan2(localPosition.y, localPosition.x) * 57.29578f;
					}
					else
					{
						RectTransform rectTransform = (RectTransform)_items[i].transform;
						Vector2 vector = new Vector2((0.5f - rectTransform.pivot.x) * rectTransform.rect.width, (0.5f - rectTransform.pivot.y) * rectTransform.rect.height);
						float f = rectTransform.localEulerAngles.z * ((float)Math.PI / 180f);
						float num2 = Mathf.Cos(f);
						float num3 = Mathf.Sin(f);
						Vector2 vector2 = new Vector2(vector.x * num2 - vector.y * num3, vector.x * num3 + vector.y * num2);
						target = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
					}
					float num4 = Mathf.Abs(Mathf.DeltaAngle(current, target));
					if (num4 < num)
					{
						num = num4;
						circularMenuItem = _items[i];
					}
				}
			}
			if (num > _activePanel.itemSelectionAngle)
			{
				ClearHover();
			}
			else
			{
				if (!(circularMenuItem != _hoveredItem))
				{
					return;
				}
				if ((bool)_hoveredItem)
				{
					_hoveredItem.SetDefault(_circularMenuPanelsConfigs[ItemsCount].normalSprite);
				}
				_hoveredItem = circularMenuItem;
				if ((bool)_hoveredItem)
				{
					_hoveredItem.SetHovered(_circularMenuPanelsConfigs[ItemsCount].hoveredSprite);
					if (_isGamepad)
					{
						_gamepadHaptics?.PulseCircularHover();
					}
				}
			}
		}

		private void RefreshButtonVisuals()
		{
			if (_isAnimationMenu)
			{
				return;
			}
			IReadOnlyList<CircularItemBinding> bindings = _activePanel.Bindings;
			for (int i = 0; i < bindings.Count; i++)
			{
				CircularItemBinding circularItemBinding = bindings[i];
				if ((bool)circularItemBinding.CircularMenuItem && circularItemBinding.CircularMenuItem.gameObject.activeSelf && _visualRefreshMap.TryGetValue(circularItemBinding.ActionId, out var value))
				{
					value(circularItemBinding.CircularMenuItem);
				}
			}
		}

		private void ClearHover()
		{
			if ((bool)_hoveredItem)
			{
				_hoveredItem.SetDefault(_circularMenuPanelsConfigs[ItemsCount].normalSprite);
				_hoveredItem = null;
			}
		}

		private void HideAllPanels(CircularMenuPanel[] panels)
		{
			if (panels != null)
			{
				for (int i = 0; i < panels.Length; i++)
				{
					panels[i].SetActive(active: false);
				}
			}
		}

		private void DeactivatePanel(CircularMenuPanel panel)
		{
			if ((bool)panel)
			{
				panel.SetActive(active: false);
				CanvasGroup contentParentCanvasGroup = panel.contentParentCanvasGroup;
				contentParentCanvasGroup.interactable = false;
				contentParentCanvasGroup.blocksRaycasts = false;
				contentParentCanvasGroup.transform.localScale = Vector3.zero;
				contentParentCanvasGroup.alpha = 0f;
			}
		}

		private void ActivatePanel(CircularMenuPanel panel)
		{
			panel.SetActive(active: true);
			_activePanel = panel;
			CanvasGroup contentParentCanvasGroup = panel.contentParentCanvasGroup;
			contentParentCanvasGroup.transform.localScale = Vector3.one;
			contentParentCanvasGroup.alpha = 1f;
			contentParentCanvasGroup.interactable = true;
			contentParentCanvasGroup.blocksRaycasts = true;
			_items = panel.GetItems();
			_namePanelText.text = panel.panelNameID.Value;
		}

		private void UpdateCircularMenuPanelUIElements(int amountOfElements)
		{
			if (_circularMenuPanelsConfigs.ContainsKey(amountOfElements))
			{
				_activePanel.parentRadialLayout.CellPivot = _circularMenuPanelsConfigs[amountOfElements].cellPivot;
				_activePanel.parentRadialLayout.CellSize = _circularMenuPanelsConfigs[amountOfElements].cellSize;
				CircularMenuItem[] items = _activePanel.GetItems();
				foreach (CircularMenuItem obj in items)
				{
					obj.Icon.rectTransform.anchoredPosition = new Vector2(_circularMenuPanelsConfigs[amountOfElements].itemXOffset, _circularMenuPanelsConfigs[amountOfElements].itemYOffset);
					obj.SetDefault(_circularMenuPanelsConfigs[amountOfElements].normalSprite);
				}
			}
		}

		private void ResetCircularMenuItems(int amountOfElements)
		{
			if (_circularMenuPanelsConfigs.ContainsKey(amountOfElements))
			{
				CircularMenuItem[] items = _activePanel.GetItems();
				for (int i = 0; i < items.Length; i++)
				{
					items[i].SetDefault(_circularMenuPanelsConfigs[amountOfElements].normalSprite);
				}
			}
		}
	}
}
