using System;
using System.Collections.Generic;
using Car_Parking.Scripts.Racing;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Multiplayer.Arena;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Person;
using _Car_Parking.Scripts.Person.Locomotion;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement.VoiceChat;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.UIElement
{
	public class ParkingMenuUI : MonoBehaviour
	{
		private const float HUDDefaultAlpha = 1f;

		private const float HiddenHUDAlpha = 0.04f;

		private const int CustomCamIndex = 4;

		private readonly Vector3[] _srcCorners = new Vector3[4];

		private readonly Vector3[] _candidateCorners = new Vector3[4];

		private readonly List<RectTransform> _hiddenObjects = new List<RectTransform>();

		[SerializeField]
		private CanvasGroup ParkingMenuCanvasGroup;

		[SerializeField]
		private Button UpdatePositionButton;

		[SerializeField]
		private Button SettingsButton;

		[SerializeField]
		private Button ScreenshotButton;

		[SerializeField]
		private Button AnimationsPresetCloseButton;

		[SerializeField]
		private Button AnimationShopButton;

		[SerializeField]
		private Toggle HideUIToggle;

		[SerializeField]
		private Toggle FocusCarToggle;

		[SerializeField]
		private Toggle AnimationPresetShowToggle;

		[SerializeField]
		private GameObject HUDPersonPanelOutCar;

		[SerializeField]
		private GameObject HUDPersonPanelInCar;

		[SerializeField]
		private Slider ClutchSlider;

		[SerializeField]
		private GameObject EmojiButton;

		[SerializeField]
		private GameObject PlayerListButton;

		[SerializeField]
		private GameObject AnimationsPreset;

		[SerializeField]
		private GameObject OnImage;

		[SerializeField]
		private GameObject OffImage;

		[SerializeField]
		private GameObject VoiceActivator;

		[SerializeField]
		private GameObject[] ToHideOnPersonViewObjects;

		[SerializeField]
		private Button KeepLineToggle;

		[SerializeField]
		private InputTouch InputTouch;

		[SerializeField]
		private GameObject[] HideFreeRacingObjects;

		[SerializeField]
		private GameObject TurnSignal;

		[SerializeField]
		private RectTransform[] ObjectsToHide;

		[Header("Levels")]
		[SerializeField]
		private LevelUIController LevelUIController;

		[Header("Camera settings")]
		[SerializeField]
		private Button CustomCamButton;

		[SerializeField]
		private CustomCameraSettingsWindow CustomCameraSettingsWindow;

		[Header("Race invite")]
		[SerializeField]
		private GameObject InvitationPanel;

		[SerializeField]
		private Button CancelInvitationButton;

		private bool _isAlreadyHidden;

		private bool _raceInvitationIsActive;

		private MenuLevelParent _menuLevelParent;

		private CoreGameManager _coreGameManager;

		private ManagerUI _managerUI;

		private LevelScript _levelScript;

		private OptionalDependency<LevelScript> _levelScriptDep;

		private FreeDriveControl _freeDriveControl;

		private OptionalDependency<FreeDriveControl> _freeDriveControlDep;

		private ChatControl _chatControl;

		private OptionalDependency<ChatControl> _chatControlDep;

		private FreeDriveProvider _freeDriveProvider;

		private FreeTimeAttackController _freeTimeAttackController;

		private OptionalDependency<FreeTimeAttackController> _freeTimeAttackControllerDep;

		private InputItemsProvider _inputItemsProvider;

		private MultiPlayer _multiPlayer;

		private EventRegistry _eventRegistry;

		private InputMapSetEvent _inputMapSetEvent;

		private RegistryHandle<ScriptableEvent> _onMenuHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnMenu);

		private PersonLocomotion _cachedDriverLocomotion;

		[field: SerializeField]
		public ServiceProgressWindow ServiceProgressWindow { get; private set; }

		[Inject]
		public void Construct(MenuLevelParent menuLevelParent, FreeDriveProvider freeDriveProvider, CoreGameManager coreGameManager, ManagerUI managerUI, EventRegistry eventRegistry, InputMapSetEvent inputMapSetEvent, OptionalDependency<LevelScript> levelScript, OptionalDependency<ChatControl> chatControl, OptionalDependency<FreeDriveControl> freeDriveControl, OptionalDependency<FreeTimeAttackController> freeTimeAttackController)
		{
			_freeDriveProvider = freeDriveProvider;
			_chatControlDep = chatControl;
			_freeDriveControlDep = freeDriveControl;
			_levelScriptDep = levelScript;
			_managerUI = managerUI;
			_coreGameManager = coreGameManager;
			_menuLevelParent = menuLevelParent;
			_freeTimeAttackControllerDep = freeTimeAttackController;
			_eventRegistry = eventRegistry;
			_inputMapSetEvent = inputMapSetEvent;
		}

		private void Awake()
		{
			_levelScript = _levelScriptDep?.Value;
			_chatControl = _chatControlDep?.Value;
			_freeDriveControl = _freeDriveControlDep?.Value;
			_freeTimeAttackController = _freeTimeAttackControllerDep?.Value;
		}

		private void Start()
		{
			if ((bool)AnimationShopButton && FlagsStorage.IsLevelScene)
			{
				AnimationShopButton.gameObject.SetActive(value: false);
			}
			if ((bool)KeepLineToggle && (bool)_freeDriveProvider.QuickCarSettings)
			{
				KeepLineToggle.onClick.AddListener(_freeDriveProvider.QuickCarSettings.KeepRow);
			}
			if ((bool)AnimationPresetShowToggle)
			{
				ShowAnimationToggle(isOn: false);
				HUDPersonPanelOutCar.SetActive(value: false);
				HUDPersonPanelInCar.SetActive(value: true);
			}
			if ((bool)SettingsButton)
			{
				SettingsButton.onClick.AddListener(OpenSettings);
			}
			UpdatePositionButton.onClick.AddListener(UpdateStaticCameraPosition);
			ScreenshotButton.onClick.AddListener(MakeScreenshot);
			HideUIToggle.onValueChanged.AddListener(HideUI);
			FocusCarToggle.onValueChanged.AddListener(_menuLevelParent.OnFocusCarPressed);
			if ((bool)AnimationsPresetCloseButton)
			{
				AnimationsPresetCloseButton.onClick.AddListener(() =>
				{
					OpenAnimationsPresetWindow(isOn: false);
				});
			}
			if ((bool)AnimationShopButton)
			{
				AnimationShopButton.onClick.AddListener(OpenAnimationShop);
			}
			ActivateVoiceChatButton(VoiceChatButtonHandler.IsActiveMicState);
			if ((bool)_levelScript)
			{
				_levelScript.OnLevelFailMenuOpen += CloseAnimationsPresetWindow;
			}
			if ((bool)CustomCamButton)
			{
				CustomCamButton.onClick.AddListener(OpenCameraSettingsWindow);
			}
			if ((bool)CancelInvitationButton)
			{
				CancelInvitationButton.onClick.AddListener(CancelRaceInvite);
			}
			if ((bool)InputTouch)
			{
				InputTouch.EnableComponent();
			}
			ManagerUI managerUI = _managerUI;
			managerUI.OnPersonCustomizationWindowClosed = (Action)Delegate.Combine(managerUI.OnPersonCustomizationWindowClosed, new Action(ActivateThisMenu));
			if ((bool)_chatControl)
			{
				ChatControl chatControl = _chatControl;
				chatControl.OnChatOpened = (Action<RectTransform, bool>)Delegate.Combine(chatControl.OnChatOpened, new Action<RectTransform, bool>(HideUnderChatObjects));
			}
			ManagerUI managerUI2 = _managerUI;
			managerUI2.OnShowScreen = (Action<bool>)Delegate.Combine(managerUI2.OnShowScreen, new Action<bool>(ShowUI));
		}

		private void OnEnable()
		{
			_managerUI.IsParkingMenuActive = true;
			FlagsStorage.IsInputEnabledInThisWindow = true;
			_onMenuHandle.Get(_eventRegistry).Subscribe(OpenSettings);
			if ((bool)KeepLineToggle)
			{
				KeepLineToggle.gameObject.SetActive(!FlagsStorage.IsPassenger);
			}
			if (ScenesHelper.IsParkingLevelActive())
			{
				_inputMapSetEvent.TrySetRootGroup();
			}
		}

		private void OnDisable()
		{
			_managerUI.IsParkingMenuActive = false;
			FlagsStorage.IsInputEnabledInThisWindow = false;
			SwitchTurnSignal();
			_onMenuHandle.Get(_eventRegistry).Unsubscribe(OpenSettings);
			if (ScenesHelper.IsParkingLevelActive())
			{
				_inputMapSetEvent.TrySetGroup(InputGroupsEnum.Ui);
			}
		}

		private void Update()
		{
			if ((bool)AnimationsPreset && AnimationsPreset.activeSelf && !CanOpenAnimationsPreset())
			{
				OpenAnimationsPresetWindow(isOn: false);
			}
		}

		private void OnDestroy()
		{
			if ((bool)_levelScript)
			{
				_levelScript.OnLevelFailMenuOpen -= CloseAnimationsPresetWindow;
			}
			if ((bool)_chatControl)
			{
				ChatControl chatControl = _chatControl;
				chatControl.OnChatOpened = (Action<RectTransform, bool>)Delegate.Remove(chatControl.OnChatOpened, new Action<RectTransform, bool>(HideUnderChatObjects));
			}
			ManagerUI managerUI = _managerUI;
			managerUI.OnPersonCustomizationWindowClosed = (Action)Delegate.Remove(managerUI.OnPersonCustomizationWindowClosed, new Action(ActivateThisMenu));
			ManagerUI managerUI2 = _managerUI;
			managerUI2.OnShowScreen = (Action<bool>)Delegate.Remove(managerUI2.OnShowScreen, new Action<bool>(ShowUI));
		}

		public void ActivateVoiceChatButton(bool activate)
		{
			if ((bool)VoiceActivator)
			{
				VoiceActivator.SetActive(activate);
			}
		}

		public void SwitchTurnSignal()
		{
			if ((bool)TurnSignal)
			{
				VehicleData vehicleData = (V.Car ? V.Car.GetComponent<VehicleData>() : null);
				if ((bool)vehicleData)
				{
					TurnSignal.SetActive(!CarIdStorage.IsCarWithoutTurnSignals(vehicleData.CarId));
				}
			}
		}

		private void ActivateThisMenu()
		{
			base.gameObject.SetActive(value: true);
		}

		private void HideUnderChatObjects(RectTransform chatRect, bool hide)
		{
			if (hide)
			{
				CloseAnimationsPresetWindow();
				_hiddenObjects.Clear();
				FindOverlappingRects(chatRect, ObjectsToHide, _hiddenObjects);
			}
			foreach (RectTransform hiddenObject in _hiddenObjects)
			{
				if (hiddenObject.TryGetComponent<CanvasGroup>(out var component))
				{
					component.alpha = ((!hide) ? 1 : 0);
					component.interactable = !hide;
				}
			}
		}

		private void FindOverlappingRects(RectTransform sourceRect, RectTransform[] candidates, List<RectTransform> results)
		{
			sourceRect.GetWorldCorners(_srcCorners);
			float x = _srcCorners[0].x;
			float x2 = _srcCorners[2].x;
			float y = _srcCorners[0].y;
			float y2 = _srcCorners[2].y;
			foreach (RectTransform rectTransform in candidates)
			{
				if (!(rectTransform == sourceRect))
				{
					rectTransform.GetWorldCorners(_candidateCorners);
					float x3 = _candidateCorners[0].x;
					float x4 = _candidateCorners[2].x;
					float y3 = _candidateCorners[0].y;
					float y4 = _candidateCorners[2].y;
					if (x < x4 && x2 > x3 && y < y4 && y2 > y3)
					{
						results.Add(rectTransform);
					}
				}
			}
		}

		private void ShowUI(bool show)
		{
			if (!show)
			{
				_isAlreadyHidden = ParkingMenuCanvasGroup.alpha == 0f;
			}
			if (!_isAlreadyHidden)
			{
				float menuAlpha = (show ? 1f : 0f);
				SetMenuAlpha(menuAlpha);
			}
		}

		private void HideUI(bool isOn)
		{
			float alpha = (isOn ? 1f : 0.04f);
			ParkingMenuCanvasGroup.alpha = alpha;
			ParkingMenuCanvasGroup.interactable = isOn;
			FlagsStorage.IsInputEnabledInThisWindow = isOn;
			if ((bool)_chatControl)
			{
				if (isOn)
				{
					_chatControl.ShowPartiallyChat();
				}
				else
				{
					_chatControl.HideAllChat();
				}
			}
		}

		private void SetMenuAlpha(float value)
		{
			ParkingMenuCanvasGroup.alpha = value;
		}

		public void Show(bool showPartiallyChat = true, bool closeSeveralWindows = true)
		{
			FlagsStorage.IsEnabledJoy = true;
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
			ParkingMenuCanvasGroup.alpha = 1f;
			ParkingMenuCanvasGroup.interactable = true;
			if (((bool)_chatControl & showPartiallyChat) && !_raceInvitationIsActive)
			{
				_chatControl.ShowPartiallyChat();
			}
			ParkingMenuCanvasGroup.blocksRaycasts = true;
			_managerUI.IsParkingMenuActive = true;
			FlagsStorage.IsInputEnabledInThisWindow = true;
			if (closeSeveralWindows)
			{
				_managerUI.CloseSeveralWindows();
			}
			if (_raceInvitationIsActive)
			{
				ShowInvitationPanel(isActive: true);
			}
			if (_freeTimeAttackController != null && _freeTimeAttackController.IsRaceInProgress())
			{
				_managerUI.ShowMultiplayerRacePlayWindow();
			}
		}

		public void FreeRacingShowElements(bool state)
		{
			for (int i = 0; i < HideFreeRacingObjects.Length; i++)
			{
				HideFreeRacingObjects[i].SetActive(state);
			}
			if (state)
			{
				SwitchTurnSignal();
			}
		}

		public void Hide(bool hideChat = true)
		{
			ParkingMenuCanvasGroup.alpha = 0f;
			ParkingMenuCanvasGroup.interactable = false;
			FlagsStorage.IsInputEnabledInThisWindow = false;
			ParkingMenuCanvasGroup.blocksRaycasts = false;
			_managerUI.IsParkingMenuActive = false;
			if ((bool)_chatControl & hideChat)
			{
				_chatControl.HideAllChat();
			}
		}

		public void CleanForPersonView(bool isOut)
		{
			if (ToHideOnPersonViewObjects != null)
			{
				GameObject[] toHideOnPersonViewObjects = ToHideOnPersonViewObjects;
				foreach (GameObject gameObject in toHideOnPersonViewObjects)
				{
					if ((bool)gameObject)
					{
						gameObject.SetActive(!isOut);
					}
				}
			}
			if (!isOut && V.CurrentCamIndex == 4)
			{
				ShowCustomCamButton(isActive: true);
			}
			_managerUI.IsParkingMenuActive = true;
			FlagsStorage.IsInputEnabledInThisWindow = true;
		}

		private void UpdateStaticCameraPosition()
		{
			_menuLevelParent.UpdateFixedCamPos();
		}

		private void MakeScreenshot()
		{
			_coreGameManager.MakeScreenShot();
		}

		private void CloseAnimationsPresetWindow()
		{
			OpenAnimationsPresetWindow(isOn: false);
		}

		public void OpenAnimationsPresetWindow(bool isOn)
		{
			OpenAnimationsPresetWindow(isOn, playSound: true);
		}

		public void OpenAnimationsPresetWindow(bool isOn, bool playSound)
		{
			if (isOn && (!CanOpenAnimationsPreset() || RagdollCPM.IsDriverRagdolled()))
			{
				if (AnimationPresetShowToggle.isOn)
				{
					AnimationPresetShowToggle.SetIsOnWithoutNotify(value: false);
				}
				return;
			}
			AnimationPresetShowToggle.image.color = (isOn ? new Color(1f, 1f, 1f, 0.1f) : new Color(0f, 0f, 0f, 0.4f));
			OnImage.SetActive(isOn);
			OffImage.SetActive(!isOn);
			AnimationsPreset.SetActive(isOn);
			if (AnimationPresetShowToggle.isOn != isOn)
			{
				AnimationPresetShowToggle.SetIsOnWithoutNotify(isOn);
				HUDPersonPanelOutCar.SetActive(isOn);
				HUDPersonPanelInCar.SetActive(!isOn);
			}
			if (playSound)
			{
				SoundEventsDataHelper.PlayUISound(isOn ? SoundEventsDataHelper.SmallClickSwitch : SoundEventsDataHelper.ClickBackSwitch, base.gameObject);
			}
			if (isOn)
			{
				GlobalEventSystem.OnHudClicked?.Invoke();
			}
		}

		private bool CanOpenAnimationsPreset()
		{
			if (!V.DriverPerson)
			{
				return true;
			}
			if (NewMoveController.IsDriverHoldingAnimation())
			{
				return true;
			}
			if (!_cachedDriverLocomotion || _cachedDriverLocomotion.gameObject != V.DriverPerson)
			{
				_cachedDriverLocomotion = V.DriverPerson.GetComponent<PersonLocomotion>();
			}
			if ((bool)_cachedDriverLocomotion && _cachedDriverLocomotion.IsActive)
			{
				return _cachedDriverLocomotion.IsFullyStopped;
			}
			return true;
		}

		public void ShowAnimationToggle(bool isOn)
		{
			AnimationPresetShowToggle.SetActive(isOn);
			HUDPersonPanelOutCar.SetActive(isOn);
			HUDPersonPanelInCar.SetActive(!isOn);
			if (isOn)
			{
				OpenAnimationsPresetWindow(isOn: false, playSound: false);
			}
		}

		public void ShowWalkElements(bool canShow)
		{
			ShowAnimationToggle(canShow);
			if ((bool)LevelUIController && FlagsStorage.IsLevelScene)
			{
				LevelUIController.ShowWalkElements(canShow);
			}
		}

		public void OpenSettings()
		{
			if (!FlagsStorage.IsLevelScene)
			{
				if (_freeDriveProvider.RoomArenaId == ArenaId.ObstacleRace_1)
				{
					_managerUI.ShowSettingsWindow(null, cleanScreen: false);
				}
				else
				{
					_managerUI.ShowSettingsWindow();
				}
				SoundEventsDataHelper.SetGroupState(SoundEventsDataHelper.StateReverPause, SoundEventsDataHelper.OffReverState);
				_levelScript.CleanScreen();
				_levelScript.SetActiveFalseEachMenu(3);
				if (FlagsStorage.IsMultiFree)
				{
					_Car_Parking.Scripts.Audio.Audio.SetPauseSoundFilter(on: true);
				}
			}
		}

		public void ClosePersonAnimationWindow()
		{
			OpenAnimationsPresetWindow(isOn: false);
			_managerUI.CloseFreeAnimationWindow();
		}

		private void OpenAnimationShop()
		{
			if (!RagdollCPM.IsDriverRagdolled())
			{
				_managerUI.ShowFreeAnimationWindow();
				OpenAnimationsPresetWindow(isOn: false);
			}
		}

		public void CloseCameraSettingsWindowWithButton()
		{
			if ((bool)CustomCameraSettingsWindow && (bool)CustomCamButton)
			{
				CustomCameraSettingsWindow.Close();
				CustomCamButton.SetActive(active: false);
			}
		}

		public bool GetCameraSettingsButtonActiveState()
		{
			return CustomCamButton.gameObject.activeSelf;
		}

		public void ShowCustomCamButton(bool isActive)
		{
			if ((bool)CustomCamButton)
			{
				CustomCamButton.SetActive(isActive);
			}
		}

		public void OpenCameraSettingsWindow()
		{
			if ((bool)CustomCameraSettingsWindow)
			{
				CustomCameraSettingsWindow.OpenCustomCarSettings();
			}
		}

		public void CancelRaceInvite()
		{
			if (FlagsStorage.IsMultiFree && (bool)_freeDriveControl)
			{
				_freeDriveControl.CancelInvite();
				_freeDriveControl.inviteToRaceButton.SetActive(active: true);
			}
		}

		public void ShowInvitationPanel(bool isActive, bool isInitStart = false)
		{
			if (!FlagsStorage.IsMultiFree)
			{
				return;
			}
			_raceInvitationIsActive = isActive;
			InvitationPanel.SetActive(isActive);
			PlayerListButton.SetActive(!isActive);
			EmojiButton.SetActive(!isActive);
			if ((bool)_chatControl)
			{
				if (isActive)
				{
					_chatControl.HideAllChat();
				}
				else if (!isInitStart)
				{
					_chatControl.ShowPartiallyChat();
				}
			}
		}
	}
}
