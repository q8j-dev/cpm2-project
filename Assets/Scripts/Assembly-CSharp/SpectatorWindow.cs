using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;

public class SpectatorWindow : MonoBehaviour, IBackableWindow
{
	[SerializeField]
	private GameObject Background;

	[SerializeField]
	private PlayerProfileWindow PlayerProfileWindow;

	[SerializeField]
	private GameObject UsersPanel;

	[SerializeField]
	private Button BackButton;

	[SerializeField]
	private Button CameraButton;

	[SerializeField]
	private Button SettingsButton;

	[SerializeField]
	private Toggle HideUIToggle;

	[SerializeField]
	private CanvasGroup[] HidableIcons;

	[SerializeField]
	private Dropdown UsersDropdown;

	[SerializeField]
	private TextMeshProUGUI HideUIText;

	private bool _isFromChat;

	private string _spectatorUserId;

	private ManagerUI _managerUI;

	private FreeDriveProvider _freeDriveProvider;

	private OnlineCollectionsProvider _onlineCollectionsProvider;

	private string _currentUserId;

	private List<MultiPersonControl> _multiPersonControlsList = new List<MultiPersonControl>();

	private MultiPersonControl _multiPersonControl;

	private MenuLevelParent _menuLevelParent;

	private OptionalDependency<MenuLevelParent> _menuLevelParentDep;

	private int _cachedCamIndex;

	private Transform _cachedCarTransform;

	private IDisposable _changeCameraDisposable;

	private IDisposable _tick;

	private ChatControl _chatControl;

	private CancellationTokenSource _setEngineDoplerCTS;

	private LevelScript _levelScript;

	private EventRegistry _eventRegistry;

	private InputMapSetEvent _inputMapSetEvent;

	private RegistryHandle<BoolEvent> _onNextPreviousHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnNextPrevious);

	private RegistryHandle<ScriptableEvent> _toggleUiHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleUi);

	[Inject]
	public void Construct(ManagerUI managerUI, FreeDriveProvider freeDriveProvider, OnlineCollectionsProvider onlineCollectionsProvider, SceneControl sceneControl, ChatControl chatControl, LevelScript levelScript, OptionalDependency<MenuLevelParent> menuLevelParent, EventRegistry eventRegistry, InputMapSetEvent inputMapSetEvent)
	{
		_levelScript = levelScript;
		_chatControl = chatControl;
		_managerUI = managerUI;
		_freeDriveProvider = freeDriveProvider;
		_onlineCollectionsProvider = onlineCollectionsProvider;
		_menuLevelParentDep = menuLevelParent;
		_eventRegistry = eventRegistry;
		_inputMapSetEvent = inputMapSetEvent;
	}

	private void Awake()
	{
		_menuLevelParent = _menuLevelParentDep?.Value;
	}

	private void Start()
	{
		BackButton.onClick.AddListener(Back);
		HideUIToggle.onValueChanged.AddListener(HideUI);
		CameraButton.onClick.AddListener(ChangeCamera);
		SettingsButton.onClick.AddListener(OpenSettings);
		UsersDropdown.onValueChanged.AddListener(ChangeUser);
		ChatControl chatControl = _chatControl;
		chatControl.OnChatOpened = (Action<RectTransform, bool>)Delegate.Combine(chatControl.OnChatOpened, new Action<RectTransform, bool>(OnChatOpened));
		Navigation navigation = HideUIToggle.navigation;
		navigation.mode = Navigation.Mode.None;
		HideUIToggle.navigation = navigation;
		navigation = CameraButton.navigation;
		navigation.mode = Navigation.Mode.None;
		CameraButton.navigation = navigation;
	}

	private void EnableTouchCamera(bool state)
	{
		if ((bool)_freeDriveProvider.PlayerListControl)
		{
			_freeDriveProvider.PlayerListControl.SetInteractableTouch(!state);
		}
	}

	private void SubscribeNewTargetToCamera(MultiPersonControl multiPersonControl)
	{
		_cachedCarTransform = (V.Car ? V.Car.transform : (V.DriverPerson ? V.DriverPerson.transform : null));
		_multiPersonControl = multiPersonControl;
		_menuLevelParent.JoyStickControl.CachedCar = multiPersonControl.FirstAndOwnCar;
		_menuLevelParent.JoyStickControl.ResetPoints();
		V.CoreGameManager.carCamerasController.CachedDriverPerson = _multiPersonControl.gameObject;
		_freeDriveProvider.CarCameras.NewCarEntered(_multiPersonControl.FirstAndOwnCar);
	}

	private void ChangeUser(int index)
	{
		if (_multiPersonControlsList.Count > index && (bool)_multiPersonControlsList[index])
		{
			if (!_multiPersonControlsList[index].FirstAndOwnCar)
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("SpectatorWindow.CantSpectatePlayer"), 3f).Forget();
				UsersDropdown.SetValueWithoutNotify(_multiPersonControlsList.IndexOf(_multiPersonControl));
			}
			else
			{
				PlayerProfileWindow.SpectatePlayer(_multiPersonControl, state: false);
				SubscribeNewTargetToCamera(_multiPersonControlsList[index]);
				PlayerProfileWindow.SpectatePlayer(_multiPersonControl, state: true);
			}
		}
	}

	private void CheckStateTick()
	{
		_ = (bool)_multiPersonControl.FirstAndOwnCar;
		if ((bool)_multiPersonControl.FirstAndOwnCar)
		{
			_ = _multiPersonControl.FirstAndOwnCar.activeSelf;
		}
		if (!_multiPersonControl.FirstAndOwnCar || ((bool)_multiPersonControl.FirstAndOwnCar && !_multiPersonControl.FirstAndOwnCar.activeSelf))
		{
			ReturnToMainUser();
		}
	}

	private void ReturnToMainUser()
	{
		_managerUI.ShowNotificationWindow(new LocalizedString("SpectatorWindow.CantSpectatePlayer"), 3f).Forget();
		Back();
	}

	private void OnPlayerDataRemoved(MultiPersonControl multiPersonControl)
	{
		if (_multiPersonControl == multiPersonControl)
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("SpectatorWindow.UserLeave"), 3f, NotificationType.Error).Forget();
			Back();
		}
		else if (_multiPersonControlsList.Contains(multiPersonControl))
		{
			int index = _multiPersonControlsList.IndexOf(multiPersonControl);
			_multiPersonControlsList.Remove(multiPersonControl);
			UsersDropdown.options.RemoveAt(index);
		}
	}

	private void SetEngineDopler(GameObject car, bool state)
	{
		CommonExtensions.CancelAndDisposeToken(ref _setEngineDoplerCTS);
		_setEngineDoplerCTS = new CancellationTokenSource();
		CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_setEngineDoplerCTS.Token, base.destroyCancellationToken);
		SetEngineDoplerAsync(car, state, cancellationTokenSource.Token).Forget();
	}

	private async UniTask SetEngineDoplerAsync(GameObject car, bool state, CancellationToken cancellationToken)
	{
		SoundController soundController = car.GetComponent<SoundController>();
		await UniTask.WaitUntil(() => soundController.EngineSound, PlayerLoopTiming.Update, cancellationToken);
		soundController.EngineSound.SpectateThisCar(state);
	}

	public void Init(MultiPersonControl multiPersonControl, bool isFromChat)
	{
		if (!multiPersonControl.FirstAndOwnCar)
		{
			ReturnToMainUser();
			return;
		}
		SoundEventsDataHelper.SetGroupState(SoundEventsDataHelper.StateReverPause, SoundEventsDataHelper.OffReverState);
		Audio.SetPauseSoundFilter(on: false);
		if (!FlagsStorage.IsSpectatorMode)
		{
			_isFromChat = isFromChat;
		}
		SetEngineDopler(multiPersonControl.FirstAndOwnCar, state: true);
		FlagsStorage.IsSpectatorMode = true;
		_chatControl.ChatWindow?.RefreshSpectatorInputHint();
		SubscribeNewTargetToCamera(multiPersonControl);
		CreateUsersList();
		_chatControl.ShowPartiallyChat();
		V.CurrentCamIndex = _cachedCamIndex;
		_changeCameraDisposable?.Dispose();
		_tick?.Dispose();
		_tick = Observable.Interval(TimeSpan.FromSeconds(1.0)).DelaySubscription(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
		{
			CheckStateTick();
		})
			.AddTo(this);
		_changeCameraDisposable = Observable.Timer(TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
		{
			_menuLevelParent.ChangeCam();
			V.CurrentCamIndex = _cachedCamIndex;
			CameraChanged();
		}).AddTo(this);
	}

	private void SetIconsAlpha(float alpha)
	{
		for (int i = 0; i < HidableIcons.Length; i++)
		{
			HidableIcons[i].alpha = alpha;
		}
	}

	public void Back()
	{
		if ((bool)_multiPersonControl && (bool)_multiPersonControl.FirstAndOwnCar)
		{
			SetEngineDopler(_multiPersonControl.FirstAndOwnCar, state: false);
		}
		PlayerProfileWindow.IsFromChat = _isFromChat;
		if (_isFromChat)
		{
			_levelScript.UnCleanScreen(saveGearState: false, closeSeveralWindows: false);
			_chatControl.OnOpenChatWithProfileWindow?.Invoke();
		}
		else
		{
			bool isChatProfileOpen = PlayerProfileWindow.IsChatProfileOpen;
			_chatControl.ChatWindow?.HideAllChatCollapsed();
			PlayerProfileWindow.OnSpectatorClose();
			if (isChatProfileOpen)
			{
				PlayerProfileWindow.KeepOpenAfterSpectatorExit();
			}
			else
			{
				_managerUI.ShowUserBalanceOnlyWindow();
			}
		}
		Background.SetActive(value: true);
		UsersPanel.SetActive(value: true);
		PlayerProfileWindow.SpectatePlayer(_multiPersonControl, state: false);
		UnsubscribeNewTargetToCamera();
		base.gameObject.SetActive(value: false);
		ShowWaitingWindowOnBackAsync(base.destroyCancellationToken).Forget();
		SoundEventsDataHelper.SetGroupState(SoundEventsDataHelper.StateReverPause, SoundEventsDataHelper.OnReverState);
		Audio.SetPauseSoundFilter(on: true);
		_managerUI.TryRemoveFromCurrentWindows(this);
	}

	private void UnsubscribeNewTargetToCamera()
	{
		GameObject firstCarMultiPlayer = _freeDriveProvider.FirstCarMultiPlayer;
		if ((bool)_cachedCarTransform)
		{
			_freeDriveProvider.CarCameras.Target = _cachedCarTransform;
		}
		else if ((bool)firstCarMultiPlayer)
		{
			_freeDriveProvider.CarCameras.Target = firstCarMultiPlayer.transform;
		}
		V.CoreGameManager.carCamerasController.CachedDriverPerson = null;
		if ((bool)firstCarMultiPlayer && firstCarMultiPlayer.TryGetComponent<VehicleData>(out var component))
		{
			V.CoreGameManager.carCamerasController.SetCachedCar(V.DriverPerson, component);
		}
		_menuLevelParent.JoyStickControl.CachedCar = null;
		V.CurrentCamIndex = 0;
		_menuLevelParent.ChangeCam();
		SetDefaultCamera();
	}

	private void OnEnable()
	{
		_cachedCamIndex = V.CurrentCamIndex;
		PersonMultiplayerDataSending.OnPersonDataApplied += OnPlayerDataApplied;
		PersonMultiplayerDataSending.OnPersonDataRemoved += OnPlayerDataRemoved;
		Background.SetActive(value: false);
		UsersPanel.SetActive(value: false);
		HideUIToggle.SetIsOnWithoutNotify(value: false);
		SetIconsAlpha(1f);
		HideUIText.text = "Hide UI";
		HideUIToggle.SetActive(active: true);
		EnableTouchCamera(state: true);
		InitBackButton();
		BackButton.SetActive(active: false);
		_onNextPreviousHandle.Get(_eventRegistry).Subscribe(NextPreviousPlayer);
		_toggleUiHandle.Get(_eventRegistry).Subscribe(ToggleUi);
		_inputMapSetEvent.TrySetGroup(InputGroupsEnum.Spectator, setRoot: true);
	}

	public void InitBackButton()
	{
		_managerUI.SetCurrentWindow(this);
	}

	private void ToggleUi()
	{
		HideUIToggle.isOn = !HideUIToggle.isOn;
	}

	private void NextPreviousPlayer(bool isNext)
	{
		if (!_multiPersonControl || _multiPersonControlsList.Count < 2)
		{
			return;
		}
		int num = _multiPersonControlsList.IndexOf(_multiPersonControl);
		while ((bool)_multiPersonControlsList[num])
		{
			int num2 = (isNext ? ((num + 1 + _multiPersonControlsList.Count) % _multiPersonControlsList.Count) : ((num - 1 + _multiPersonControlsList.Count) % _multiPersonControlsList.Count));
			if (!_multiPersonControlsList[num2])
			{
				num = num2;
				continue;
			}
			UsersDropdown.value = num2;
			break;
		}
	}

	private void SetDefaultCamera()
	{
		V.CurrentCamIndex = _cachedCamIndex;
		if (!_cachedCarTransform)
		{
			return;
		}
		_freeDriveProvider.CarCameras.NewCarEntered(_cachedCarTransform.gameObject);
		V.CoreGameManager.carCamerasController.SetCamera(_cachedCamIndex, _cachedCarTransform, _cachedCamIndex != 0);
		if (_cachedCamIndex == 0)
		{
			Observable.Timer(TimeSpan.FromSeconds(0.20000000298023224)).Subscribe((long _) =>
			{
				GlobalEventSystem.InvokeCarForcePositioned(null);
			}).AddTo(this);
		}
	}

	private void OnPlayerDataApplied(MultiPersonControl multiPersonControl)
	{
		if (!_multiPersonControlsList.Contains(multiPersonControl))
		{
			_multiPersonControlsList.Add(multiPersonControl);
			UsersDropdown.options.Add(new Dropdown.OptionData
			{
				text = multiPersonControl.ThisPlayerName,
				image = GetRankIcon(multiPersonControl)
			});
		}
	}

	private void OnDisable()
	{
		FlagsStorage.IsSpectatorMode = false;
		_chatControl.ChatWindow?.RefreshSpectatorInputHint();
		_changeCameraDisposable?.Dispose();
		_tick?.Dispose();
		PersonMultiplayerDataSending.OnPersonDataRemoved -= OnPlayerDataRemoved;
		PersonMultiplayerDataSending.OnPersonDataApplied -= OnPlayerDataApplied;
		EnableTouchCamera(state: false);
		_onNextPreviousHandle.Get(_eventRegistry)?.Unsubscribe(NextPreviousPlayer);
		_toggleUiHandle.Get(_eventRegistry)?.Unsubscribe(ToggleUi);
	}

	private void OnDestroy()
	{
		PersonMultiplayerDataSending.OnPersonDataRemoved -= OnPlayerDataRemoved;
		PersonMultiplayerDataSending.OnPersonDataApplied -= OnPlayerDataApplied;
		ChatControl chatControl = _chatControl;
		chatControl.OnChatOpened = (Action<RectTransform, bool>)Delegate.Remove(chatControl.OnChatOpened, new Action<RectTransform, bool>(OnChatOpened));
	}

	private void HideUI(bool state)
	{
		HideUIText.text = (state ? "Show UI" : "Hide UI");
		SetIconsAlpha((!state) ? 1 : 0);
		if (state)
		{
			_chatControl.HideAllChat();
		}
		else
		{
			_chatControl.ShowPartiallyChat();
		}
		HideUIToggle.SetActive(!state);
	}

	private Sprite GetRankIcon(MultiPersonControl multiPersonControl)
	{
		return UtilsRank.LoadRankSpriteFromResources((UtilsRank.RankType)(int)multiPersonControl.rank.GetValueOrDefault());
	}

	private void CreateUsersList()
	{
		_multiPersonControlsList = new List<MultiPersonControl>();
		List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
		for (int i = 0; i < _onlineCollectionsProvider.PlayersOnlineNow.Length; i++)
		{
			if ((bool)_onlineCollectionsProvider.PlayersOnlineNow[i])
			{
				if (!_multiPersonControlsList.Contains(_onlineCollectionsProvider.PlayersOnlineNow[i]))
				{
					_multiPersonControlsList.Add(_onlineCollectionsProvider.PlayersOnlineNow[i]);
					list.Add(new Dropdown.OptionData
					{
						text = _onlineCollectionsProvider.PlayersOnlineNow[i].ThisPlayerName,
						image = GetRankIcon(_onlineCollectionsProvider.PlayersOnlineNow[i])
					});
				}
				UsersDropdown.options = list;
				int valueWithoutNotify = _multiPersonControlsList.IndexOf(_multiPersonControl);
				UsersDropdown.SetValueWithoutNotify(valueWithoutNotify);
			}
		}
	}

	private void ChangeCamera()
	{
		if ((bool)_menuLevelParent && (bool)_multiPersonControl)
		{
			V.CurrentCamIndex++;
			if (V.CurrentCamIndex > 5)
			{
				V.CurrentCamIndex = 0;
			}
			CameraChanged();
		}
	}

	private void CameraChanged()
	{
		if ((bool)_menuLevelParent && (bool)_multiPersonControl && (bool)_multiPersonControl.FirstAndOwnCar)
		{
			VehicleData component = _multiPersonControl.FirstAndOwnCar.GetComponent<VehicleData>();
			V.CoreGameManager.carCamerasController.SetCachedCarForSpectator(component);
			_multiPersonControl.gameObject.SetActive(value: true);
			V.CoreGameManager.carCamerasController.SetCamera(V.CurrentCamIndex, _multiPersonControl.FirstAndOwnCar.transform, newTarget: false);
			_freeDriveProvider.CarCameras.Target = _multiPersonControl.FirstAndOwnCar.transform;
		}
	}

	private void OpenSettings()
	{
		_managerUI.ShowSettingsWindow();
	}

	private async UniTask ShowWaitingWindowOnBackAsync(CancellationToken cancellationToken)
	{
		if (!V.CoreGameManager || V.CoreGameManager.FreeDriveProvider == null || !V.CoreGameManager.FreeDriveProvider.FirstCarMultiPlayer || !V.SceneControl)
		{
			return;
		}
		Rigidbody carRigidbody = V.CoreGameManager.FreeDriveProvider.FirstCarMultiPlayer.GetComponent<Rigidbody>();
		if (!carRigidbody)
		{
			return;
		}
		List<int> scenesNeed = V.SceneControl.GetSceneIndexByPosition(carRigidbody.transform.position);
		if (ScenesHelper.AllScenesLoaded(scenesNeed))
		{
			return;
		}
		await _managerUI.ShowWaitingWindowNotTransparent(set: true);
		float timeNow = 0f;
		while (timeNow < 5f && !cancellationToken.IsCancellationRequested && (bool)carRigidbody)
		{
			carRigidbody.isKinematic = true;
			timeNow += Time.deltaTime;
			if (ScenesHelper.AllScenesLoaded(scenesNeed))
			{
				break;
			}
			await UniTask.Yield();
		}
		if (!cancellationToken.IsCancellationRequested)
		{
			_managerUI.CloseNotTransparentWaitingWindow();
			if ((bool)carRigidbody)
			{
				carRigidbody.isKinematic = false;
			}
		}
	}

	private void OnChatOpened(RectTransform chatRect, bool chatOpened)
	{
		HideUIToggle.SetActive(!chatOpened);
	}
}
