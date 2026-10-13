using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Addler.Runtime.Core.LifetimeBinding;
using CPM.Account;
using CPM.Connection;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Other;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.PlayerProperty.Business;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.WindowSystem;
using _Car_Parking.Scripts.WindowSystem.Confirmation;
using _Car_Parking.Scripts.WindowSystem.InitializersWindows;
using _Car_Parking.Scripts.WindowSystem.InitializersWindows.GameOverlayWindowInitializers;
using _Car_Parking.Scripts.WindowSystem.PrivacyPolicy;
using _Car_Parking.Scripts.WindowSystem.ProgressWindows;

namespace _Car_Parking.Scripts.Drag
{
	public class ManagerUI : MonoBehaviour
	{
		private readonly struct OpeningEntry
		{
			public readonly string Guid;

			public readonly CancellationTokenSource Cts;

			public readonly bool AlwaysOnTop;

			public OpeningEntry(string guid, CancellationTokenSource cts, bool alwaysOnTop)
			{
				Guid = guid;
				Cts = cts;
				AlwaysOnTop = alwaysOnTop;
			}
		}

		public struct CurrentDealerWindowUpdatedData
		{
			public DealerWindow OldValue;

			public DealerWindow NewValue;

			public CurrentDealerWindowUpdatedData(DealerWindow newValue, DealerWindow oldValue)
			{
				OldValue = oldValue;
				NewValue = newValue;
			}
		}

		[SerializeField]
		private Transform WindowsParent;

		[SerializeField]
		private Transform WindowsParentAlwaysOnTop;

		[SerializeField]
		private List<Sprite> EngineIcons;

		public Image BlackImage;

		private IPrefabFactory _prefabFactory;

		private static readonly string[] ExcludedWindows = new string[9] { "CoinsAndMoneyPurchaseWindow", "UserBalanceWindow", "WaitingWindow", "MarketInventoryFilterWindow", "AllInventoryFilterWindow", "DailyTasksFull", "GearboxFilterWindow", "EngineFilterWindow", "ArenaLobbyWindow" };

		private static readonly string[] InputBlockWindows = new string[9] { "MainMenuWindow", "GarageWindow", "CarTuningWindow", "VisualTuningWindow", "DynoWindow", "SuspensionTuningWindow", "RallyMenuWindow", "RacingCarChangeTyreWindow", "RallyLeaderboardWindow" };

		private static readonly string[] InputAllowMainMenuWindows = new string[1] { "MenuControllerHUD" };

		private const string MainMenuWindow = "MainMenuWindow";

		private const string DroneControlWindow = "DroneControlWindow";

		private const string PlayerProfileWindowName = "PlayerProfileWindow";

		private readonly Dictionary<string, WindowScript> activeWindowsDictionary = new Dictionary<string, WindowScript>();

		private readonly List<string> _prevWindows = new List<string>();

		private bool _isDestroyed;

		private bool _isCleaningScreen;

		private bool _carInputHardLock;

		private string m_prevWindow;

		private string m_currentWindow;

		private WindowScript _currentWindowScript;

		private List<IBackableWindow> _currentBackableWindow = new List<IBackableWindow>();

		private PartData selectPartToMarket;

		private int selectPartInstanceIndex;

		private bool _isDebugging = true;

		private readonly HashSet<string> _startOpenWindows = new HashSet<string>();

		private readonly HashSet<string> _alwaysOnTopWindows = new HashSet<string>();

		private readonly HashSet<string> _activeRegularWindows = new HashSet<string>();

		private readonly CancellationTokenSource _destroyCts = new CancellationTokenSource();

		private readonly Dictionary<int, OpeningEntry> _openingCts = new Dictionary<int, OpeningEntry>();

		private int _nextOpeningTicket;

		private Canvas _canvas;

		private bool _isFallbackAdded;

		private DealerWindow _currentDealerWindow;

		private NotificationWindow lastNotificationWindow;

		private ActionButton _lastActionButton;

		private WindowScript _lastConfirmWindow;

		private CurrencyManagerController _currencyManagerController;

		private ICarTuningFactory _carTuningFactory;

		private bool _openSettingsFromDrone;

		private IAccountController _accountController;

		private FreeDriveProvider _freeDriveProvider;

		private LevelsProvider _levelsProvider;

		public bool IsSettingsWindowActive;

		public bool RestoreButtonPressed;

		[SerializeField]
		private GraphicRaycaster GraphicRaycaster;

		private Color32 hideColor = new Color32(0, 0, 0, 0);

		public Action<bool> OnShowScreen;

		public Action<bool> OnRallyPopupShowed;

		public Action<string> OnRegularWindowOpening;

		public Action<string> OnRegularWindowClosed;

		public Action<MarketType> OnMarketTypeClick;

		public bool IsParkingMenuActive;

		private bool _isRacingCarSelect;

		private bool AllWindowsWasPreloaded;

		private Image _windowPanelBackImage;

		public PlayerProfileWindow CurrentPlayerProfile;

		[SerializeField]
		private GameObject _waitingWindow;

		private EventRegistry _eventRegistry;

		private InputMapSetEvent _inputMapSetEvent;

		private RegistryHandle<ScriptableEvent> _cancelHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.Cancel);

		public EngineKitData EditingEngineKitData;

		public MarketInventoryFilters CurrentInventoryElement;

		public AllInventoryInstalledFilter CurrentAllInventoryInstalledElement;

		public HashSet<AllInventoryFilters> CurrentAllInventoryFilters = new HashSet<AllInventoryFilters> { AllInventoryFilters.All };

		public HashSet<GearBoxTransmissionTypeFilter> CurrentGearBoxTransMissionTypes = new HashSet<GearBoxTransmissionTypeFilter> { GearBoxTransmissionTypeFilter.All };

		public HashSet<GearBoxTypeFilters> CurrentGearBoxTypeFilters = new HashSet<GearBoxTypeFilters> { GearBoxTypeFilters.All };

		public HashSet<EngineFilterTypeFilter> CurrentEngineFilterTypeFilters = new HashSet<EngineFilterTypeFilter> { EngineFilterTypeFilter.All };

		private IConnectionDataHandler _connectionDataHandler;

		public bool UserBalanceWindowActive { get; private set; }

		public ScreenFader ScreenFade { get; private set; }

		public Action<bool> OnBackButtonActivated { get; set; }

		public Action<bool> OnSettingsButtonActivated { get; set; }

		public Action<bool> OnAchievmentsButtonActivated { get; set; }

		public Action<bool> EnableAllBalanceWindow { get; set; }

		public Action<bool> OnActiveAllBalanceWindow { get; set; }

		public Action<bool> OnActivateBalancePanel { get; set; }

		public Action<bool> ShowUI { get; set; }

		public Action<bool> ChangeBalancePanelActiveState { get; set; }

		public Action<bool> SetBackground { get; set; }

		public Action<bool> IncreaseTimerButtonShow { get; set; }

		public Action OnCurrencyRestored { get; set; }

		public Action OnPersonCustomizationWindowClosed { get; set; }

		public MarketType CurrentMarketType { get; private set; }

		public string WaitingWindowText { get; set; }

		public DealerWindow CurrentDealerWindow
		{
			get
			{
				return _currentDealerWindow;
			}
			set
			{
				CurrentDealerWindowUpdatedData obj = new CurrentDealerWindowUpdatedData(value, _currentDealerWindow);
				_currentDealerWindow = value;
				OnCurrentDealerWindowUpdated?.Invoke(obj);
			}
		}

		public BaseCircleNotification CircleNotification { get; set; }

		public RequestWindowTemplate RequestWindowTemplate { get; set; }

		public AchievementsWindow AchievementsWindow { get; set; }

		public PhotoModeWindow PhotoModeWindow { get; set; }

		public AppVersionLabel AppVersion { get; set; }

		public IWindowInitializer AuthorizationWindowInitializer { get; set; }

		public IWindowInitializer PersonWindowInitializer { get; set; }

		public IWindowInitializer GameOverlayWindowInitializer { get; set; }

		public IWindowInitializer CurrencyShopWindowInitializer { get; set; }

		public IWindowInitializer DarkLoadingWindowInitializer { get; set; }

		public bool PersonWindowUnpurchasedWindowActive { get; set; }

		private Action OnCanPlayFalseHandler => () =>
		{
			ShowNotificationWindow(new LocalizedString("ConnectionDataHandler.YouCanTPlayOnThisAccountSignOu"), 3f, NotificationType.Error).Forget();
		};

		public PartData GetPartToMarket => selectPartToMarket;

		public int GetPartToMarketIndex => selectPartInstanceIndex;

		public Color32 HideColor
		{
			get
			{
				return hideColor;
			}
			set
			{
				hideColor = value;
			}
		}

		public bool IsCanvasEnabled
		{
			get
			{
				return _canvas.enabled;
			}
			set
			{
				_canvas.enabled = value;
			}
		}

		public event Action<CurrentDealerWindowUpdatedData> OnCurrentDealerWindowUpdated;

		public event Action<WindowScript> OnCurrentWindowUpdated;

		public event Action<string> OnWindowClosed;

		[Inject]
		public void Construct(IPrefabFactory prefabFactory, FreeDriveProvider freeDriveProvider, ScreenFader screenFader, CurrencyManagerController currencyManagerController, ICarTuningFactory carTuningFactory, IAccountController accountController, LevelsProvider levelsProvider, IConnectionDataHandler connectionDataHandler, EventRegistry eventRegistry, InputMapSetEvent inputMapSetEvent)
		{
			_connectionDataHandler = connectionDataHandler;
			_levelsProvider = levelsProvider;
			_freeDriveProvider = freeDriveProvider;
			_accountController = accountController;
			_carTuningFactory = carTuningFactory;
			_currencyManagerController = currencyManagerController;
			ScreenFade = screenFader;
			_prefabFactory = prefabFactory;
			_eventRegistry = eventRegistry;
			_inputMapSetEvent = inputMapSetEvent;
		}

		public Transform GetWindowsParent()
		{
			return WindowsParent;
		}

		public void SetPreviousWindow(string guid)
		{
			m_prevWindow = guid;
		}

		private bool IsCurrentBackableWindowListEmpty()
		{
			return !_currentBackableWindow.Any();
		}

		public void SetCurrentWindow(IBackableWindow window)
		{
			_currentBackableWindow.Remove(window);
			_currentBackableWindow.Add(window);
			((MonoBehaviour)window).OnDestroyAsObservable().Subscribe((Unit _) =>
			{
				RemoveFromCurrentWindows(window);
			});
			List<IBackableWindow> currentBackableWindow = _currentBackableWindow;
			if (currentBackableWindow != null)
			{
				_ = currentBackableWindow.Count;
				_ = 0;
			}
		}

		public void TryRemoveFromCurrentWindows(IBackableWindow window)
		{
			RemoveFromCurrentWindows(window);
		}

		private void RemoveFromCurrentWindows(IBackableWindow window)
		{
			List<IBackableWindow> currentBackableWindow = _currentBackableWindow;
			if (currentBackableWindow != null)
			{
				_ = currentBackableWindow.Count;
				_ = 0;
			}
			_currentBackableWindow.Remove(window);
		}

		public void ClearWindows()
		{
			_currentBackableWindow.Clear();
		}

		private void Awake()
		{
			PreloadWindows().Forget();
			_connectionDataHandler.OnCanPlayFalse += OnCanPlayFalseHandler;
		}

		private void Start()
		{
			_windowPanelBackImage = WindowsParent.GetComponent<Image>();
			if (_canvas == null)
			{
				_canvas = GetComponent<Canvas>();
			}
			if (GraphicRaycaster == null)
			{
				GraphicRaycaster = GetComponent<GraphicRaycaster>();
			}
			LoadSpriteAtlasAsync();
			if (ScenesHelper.IsMenuSceneActive())
			{
				OnMarketTypeClick = (Action<MarketType>)Delegate.Combine(OnMarketTypeClick, new Action<MarketType>(ShowCoinsOrMoneyWindow));
			}
			_cancelHandle.Get(_eventRegistry).Subscribe(Back);
			Subscribes();
		}

		private void OnDestroy()
		{
			_isDestroyed = true;
			if (_connectionDataHandler != null)
			{
				_connectionDataHandler.OnCanPlayFalse -= OnCanPlayFalseHandler;
			}
			_cancelHandle.Get(_eventRegistry)?.Unsubscribe(Back);
			Unsubscribes();
			_destroyCts.Cancel();
		}

		private void Subscribes()
		{
			_currencyManagerController.NotEnoughMoney += NotEnoughMoney;
			_currencyManagerController.NotEnoughCoins += NotEnoughCoins;
			_currencyManagerController.NoInternetConnection += NoInternetConnection;
			_currencyManagerController.ShowWaitingWindow += ShowWaitingWindowWhenCurrencyManagerOperation;
			_carTuningFactory.OnUpdateCar += ReloadActiveWindowsFromEvent;
			_carTuningFactory.MessageNotification += ShowNotification;
			_carTuningFactory.MessageNotificationWithOtherSound += ShowNotificationWithOtherSound;
			_accountController.OnForceLogout += HandleForceLogout;
		}

		private void Unsubscribes()
		{
			_currencyManagerController.NotEnoughMoney -= NotEnoughMoney;
			_currencyManagerController.NotEnoughCoins -= NotEnoughCoins;
			_currencyManagerController.NoInternetConnection -= NoInternetConnection;
			_currencyManagerController.ShowWaitingWindow -= ShowWaitingWindowWhenCurrencyManagerOperation;
			_carTuningFactory.OnUpdateCar -= ReloadActiveWindowsFromEvent;
			_carTuningFactory.MessageNotification -= ShowNotification;
			_carTuningFactory.MessageNotificationWithOtherSound -= ShowNotificationWithOtherSound;
			_accountController.OnForceLogout -= HandleForceLogout;
		}

		private void HandleForceLogout()
		{
			ShowNotificationWindow(new LocalizedString("GetBalancesResult.Youloggedinonanotherdevice.Youwillbeloggedout."), 3f, NotificationType.Error).Forget();
		}

		private void ShowWaitingWindowWhenCurrencyManagerOperation(bool enable, bool showBackButton = true)
		{
			if (enable)
			{
				ShowWaitingWindow(set: true, showBackButton).Forget();
			}
			else
			{
				CloseWaitingWindow();
			}
		}

		private async UniTaskVoid PreloadWindows()
		{
			if (ScenesHelper.IsMenuSceneActive())
			{
				await UniTask.WhenAll(LoadWindowPrefab("MainMenuWindow"), LoadWindowPrefab("WaitingWindow"), LoadWindowPrefab("GameModesWindow"), LoadWindowPrefab("UserBalanceWindow"));
				AllWindowsWasPreloaded = true;
			}
			else
			{
				await UniTask.WhenAll<GameObject>(LoadWindowPrefab("SettingsMenuWindow"));
				AllWindowsWasPreloaded = true;
			}
		}

		public void CheckWindow(string guid, bool alwaysOnTop)
		{
			LogActiveWindows("CleanScreen-1_" + guid);
			_isCleaningScreen = true;
			try
			{
				if (!_startOpenWindows.IsNullOrEmpty())
				{
					foreach (string startOpenWindow in _startOpenWindows)
					{
						if (CancelAndDisposeByGuidByOnTop(startOpenWindow, alwaysOnTop))
						{
							CloseWindow(startOpenWindow);
						}
					}
					_startOpenWindows.Clear();
				}
				if (!activeWindowsDictionary.IsNullOrEmpty())
				{
					foreach (string item in from window in activeWindowsDictionary.Keys.ToList()
						where _alwaysOnTopWindows.Contains(window) == alwaysOnTop
						select window)
					{
						CloseWindow(item);
					}
				}
			}
			finally
			{
				_isCleaningScreen = false;
			}
			LogActiveWindows("CleanScreen-2_" + guid);
		}

		private bool IsWindowAlive(string windowName)
		{
			if (activeWindowsDictionary.TryGetValue(windowName, out var value))
			{
				return value;
			}
			return false;
		}

		private void LoadSpriteAtlasAsync()
		{
			AsyncOperationHandle<IList<Sprite>> operationHandle = Addressables.LoadAssetAsync<IList<Sprite>>("EngineIcons");
			operationHandle.Completed += (AsyncOperationHandle<IList<Sprite>> _) =>
			{
				if (operationHandle.Status == AsyncOperationStatus.Succeeded)
				{
					foreach (Sprite item in operationHandle.Result)
					{
						EngineIcons.Add(item);
					}
				}
			};
			Addressables.Release(operationHandle);
		}

		public Sprite GetEngineIcon(int partId)
		{
			int num = partId;
			switch (partId)
			{
			case 8:
			case 9:
			case 10:
				num = 8;
				break;
			case 11:
			case 12:
			case 13:
			case 31:
				num = 11;
				break;
			case 36:
			case 37:
				num = 36;
				break;
			}
			if (EngineIcons.Count > 0)
			{
				foreach (Sprite engineIcon in EngineIcons)
				{
					int.TryParse(Regex.Match(engineIcon.name, "\\d+").Value, out var result);
					if (result == num)
					{
						return engineIcon;
					}
				}
			}
			return Sprite.Create(new Texture2D(2, 2, TextureFormat.Alpha8, mipChain: false), new Rect(0f, 0f, 2f, 2f), Vector2.zero);
		}

		public void SelectPartToMarket(PartData part, int index = 0)
		{
			selectPartToMarket = part;
			selectPartInstanceIndex = index;
		}

		public string GetPrevWindow()
		{
			return m_prevWindow;
		}

		public void SetPrevWindow(string prevWindow)
		{
			m_prevWindow = prevWindow.Replace("(Clone)", string.Empty);
		}

		public string GetCurrentWindow()
		{
			return m_currentWindow;
		}

		public void OnSignedIn()
		{
		}

		public void OnSignedOut()
		{
		}

		public void OnWindowClose(string windowName, bool changeRootGameInputGroup, WindowScript closingWindow = null)
		{
			windowName = windowName.Replace("(Clone)", string.Empty);
			LogActiveWindows("close-" + windowName);
			CancelAndDisposeByGuid(windowName);
			OnWindowClosed?.Invoke(windowName);
			if (activeWindowsDictionary.ContainsKey(windowName))
			{
				activeWindowsDictionary.Remove(windowName);
			}
			_alwaysOnTopWindows.Remove(windowName);
			bool flag = _activeRegularWindows.Remove(windowName);
			UiInputSetup.DisabledWindows.Remove(windowName);
			if (windowName == m_currentWindow)
			{
				string text = _prevWindows.LastOrDefault();
				if (!text.IsNullOrEmpty() && activeWindowsDictionary.TryGetValue(text, out var value) && (bool)value)
				{
					value.RestoreUiNavigation();
					_prevWindows.Remove(text);
					m_currentWindow = text;
					_currentWindowScript = value;
				}
				else
				{
					m_currentWindow = null;
					_currentWindowScript = null;
					OnBackButtonActivated?.Invoke(obj: false);
				}
				if (m_currentWindow != null)
				{
					SetInputEnable(m_currentWindow);
				}
				else if (FlagsStorage.IsMainMenu)
				{
					FlagsStorage.IsInputEnabledInThisWindow = true;
				}
			}
			if (windowName == "ConfirmWindow")
			{
				if (activeWindowsDictionary.TryGetValue("UserBalanceWindow", out var value2))
				{
					value2.RestoreUiNavigation();
				}
				if ((bool)_currentWindowScript)
				{
					_currentWindowScript.RestoreUiNavigation();
				}
			}
			if (UiInputSetup.IsBlockBackground(windowName))
			{
				foreach (WindowScript value3 in activeWindowsDictionary.Values)
				{
					value3.RestoreUiNavigation();
				}
			}
			if (flag)
			{
				SetPreviousInputGroup(changeRootGameInputGroup);
			}
			else if ((bool)closingWindow && closingWindow.DidPushInputGroup)
			{
				_inputMapSetEvent.TrySetPreviousGroup();
			}
			if (flag)
			{
				OnRegularWindowClosed?.Invoke(windowName);
			}
			UISelectionCursor.HideCursor();
			LogActiveWindows("OnWindowClose-" + windowName);
		}

		private void CancelAndDisposeByGuid(string guid)
		{
			foreach (OpeningEntry value in _openingCts.Values)
			{
				if (value.Guid == guid && !value.Cts.IsCancellationRequested)
				{
					value.Cts.Cancel();
				}
			}
		}

		private bool CancelAndDisposeByGuidByOnTop(string guid, bool alwaysOnTop)
		{
			using (IEnumerator<OpeningEntry> enumerator = _openingCts.Values.Where((OpeningEntry entry) => entry.Guid == guid && !entry.Cts.IsCancellationRequested && entry.AlwaysOnTop == alwaysOnTop).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					enumerator.Current.Cts.Cancel();
					return true;
				}
			}
			return false;
		}

		private void DisposeOwnCts(int ticket)
		{
			if (_openingCts.Remove(ticket, out var value))
			{
				if (!value.Cts.IsCancellationRequested)
				{
					value.Cts.Cancel();
				}
				value.Cts.Dispose();
			}
		}

		public async UniTask<WindowScript> ShowWindow(string guid, bool alwaysOnTop = false, bool show = true, UnityAction intermediateCallback = null, bool fadeBlack = false, bool enableBackButton = false, bool cleanScreen = true, string additionalData = null, bool withAnimate = false, bool updateBackButton = true, bool trackForCleanup = true)
		{
			LogActiveWindows("show0-" + guid);
			UISelectionCursor.HideCursor();
			if (withAnimate && (bool)_currentWindowScript)
			{
				await _currentWindowScript.ShowUIAsync(state: false);
			}
			_ = _destroyCts.Token;
			if (cleanScreen)
			{
				CheckWindow(guid, alwaysOnTop);
			}
			if (trackForCleanup)
			{
				_startOpenWindows.Add(guid);
			}
			CancelAndDisposeByGuidByOnTop(guid, alwaysOnTop);
			CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_destroyCts.Token);
			int ticket = _nextOpeningTicket++;
			_openingCts[ticket] = new OpeningEntry(guid, cancellationTokenSource, alwaysOnTop);
			CancellationToken ct = cancellationTokenSource.Token;
			if ((bool)GraphicRaycaster)
			{
				GraphicRaycaster.enabled = false;
			}
			if (await UniTask.WaitUntil(() => AllWindowsWasPreloaded, PlayerLoopTiming.Update, ct).SuppressCancellationThrow())
			{
				_startOpenWindows.Remove(guid);
				DisposeOwnCts(ticket);
				return null;
			}
			LogActiveWindows("show1-" + guid);
			if (activeWindowsDictionary.TryGetValue(guid, out var value) && !value)
			{
				activeWindowsDictionary.Remove(guid);
			}
			if (updateBackButton)
			{
				OnBackButtonActivated?.Invoke(enableBackButton);
			}
			if (!show)
			{
				if ((bool)value)
				{
					value.Close();
					activeWindowsDictionary.Remove(guid);
				}
				_startOpenWindows.Remove(guid);
				DisposeOwnCts(ticket);
				GraphicRaycaster.enabled = true;
				return null;
			}
			if ((bool)value)
			{
				if (!alwaysOnTop)
				{
					OnRegularWindowOpening?.Invoke(guid);
				}
				value.RestoreUiNavigation();
				_startOpenWindows.Remove(guid);
				DisposeOwnCts(ticket);
				GraphicRaycaster.enabled = true;
				return value;
			}
			int num = _prevWindows.IndexOf(guid);
			bool isBackwardNavigation = num >= 0;
			if (isBackwardNavigation)
			{
				_prevWindows.RemoveRange(num, _prevWindows.Count - num);
			}
			if (fadeBlack)
			{
				ScreenFade.FadeOut(0.25f);
			}
			string previousWindowName = m_currentWindow;
			if (!alwaysOnTop || m_currentWindow == null)
			{
				m_currentWindow = guid;
			}
			SetInputEnable(m_currentWindow);
			GameObject obj = await LoadWindowPrefab(guid);
			if (ct.IsCancellationRequested)
			{
				_startOpenWindows.Remove(guid);
				DisposeOwnCts(ticket);
				return null;
			}
			Transform parent = (alwaysOnTop ? WindowsParentAlwaysOnTop : WindowsParent);
			GameObject windowSpawned = _prefabFactory.Instantiate(obj, parent);
			WindowScript windowScript = windowSpawned.GetComponent<WindowScript>();
			windowScript.GUID = guid;
			if (!string.IsNullOrEmpty(additionalData))
			{
				windowScript.SetAdditionalData(additionalData);
			}
			using CancellationTokenSource windowDestroyCts = CancellationTokenSource.CreateLinkedTokenSource(ct, windowSpawned.GetCancellationTokenOnDestroy());
			CancellationToken windowCt = windowDestroyCts.Token;
			if (await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, windowCt).SuppressCancellationThrow())
			{
				if ((bool)windowSpawned)
				{
					UnityEngine.Object.Destroy(windowSpawned);
				}
				_startOpenWindows.Remove(guid);
				DisposeOwnCts(ticket);
				return null;
			}
			if (!alwaysOnTop || m_currentWindow == null)
			{
				_currentWindowScript = windowScript;
			}
			if (!_isFallbackAdded && (bool)TMP_FontAsset_Placeholder.FallbackFont)
			{
				_isFallbackAdded = true;
				windowScript.AddFallbackFont(TMP_FontAsset_Placeholder.FallbackFont);
			}
			if (!activeWindowsDictionary.TryAdd(guid, windowScript))
			{
				windowScript.Close();
			}
			else if (!previousWindowName.IsNullOrEmpty() && !ExcludedWindows.Contains(previousWindowName) && !alwaysOnTop && !isBackwardNavigation)
			{
				int num2 = _prevWindows.IndexOf(previousWindowName);
				if (num2 >= 0)
				{
					_prevWindows.RemoveRange(num2 + 1, _prevWindows.Count - num2 - 1);
				}
				else
				{
					_prevWindows.Add(previousWindowName);
				}
			}
			if (!_prevWindows.IsNullOrEmpty() && !cleanScreen && !alwaysOnTop && previousWindowName != null && activeWindowsDictionary.TryGetValue(previousWindowName, out var value2) && (bool)value2)
			{
				value2.SuspendUiNavigation();
			}
			if (UiInputSetup.IsBlockBackground(windowSpawned.name.Replace("(Clone)", "")))
			{
				foreach (WindowScript item in activeWindowsDictionary.Values.Where((WindowScript otherActiveWindow) => otherActiveWindow.name != windowSpawned.name))
				{
					item.SuspendUiNavigation();
				}
			}
			if (!alwaysOnTop)
			{
				OnRegularWindowOpening?.Invoke(guid);
			}
			windowSpawned.SetActive(value: true);
			if (withAnimate)
			{
				if (await windowScript.ShowUIAsync(state: true).AttachExternalCancellation(windowCt).SuppressCancellationThrow())
				{
					_startOpenWindows.Remove(guid);
					DisposeOwnCts(ticket);
					return null;
				}
			}
			else
			{
				windowScript.RestoreUiNavigation();
			}
			if (intermediateCallback != null && windowSpawned.TryGetComponent<WaitingWindow>(out var component))
			{
				component.InitCallback(intermediateCallback);
			}
			_startOpenWindows.Remove(guid);
			DisposeOwnCts(ticket);
			if (alwaysOnTop)
			{
				_alwaysOnTopWindows.Add(guid);
			}
			else
			{
				_activeRegularWindows.Add(guid);
			}
			GraphicRaycaster.enabled = true;
			OnCurrentWindowUpdated?.Invoke(windowScript);
			LogActiveWindows("show2-" + guid);
			UiInputSetup.DisableUnnecessaryNavigation(windowSpawned);
			return windowScript;
		}

		public void SetPreviousInputGroup(bool changeRootGameInputGroup)
		{
			if (_isCleaningScreen)
			{
				return;
			}
			bool flag = false;
			if (changeRootGameInputGroup)
			{
				flag = _inputMapSetEvent.SetPreviousRootGroup();
			}
			else if (_activeRegularWindows.IsNullOrEmptyCollection())
			{
				if (_freeDriveProvider != null && (bool)_freeDriveProvider.MapCamera && _freeDriveProvider.MapCamera.ShowingBigMap)
				{
					flag = _inputMapSetEvent.TrySetGroup(InputGroupsEnum.Map);
				}
				else
				{
					flag = _inputMapSetEvent.TrySetRootGroup();
				}
			}
			else if (!_currentWindowScript || _inputMapSetEvent.CurrentInputGroup != _currentWindowScript.GetWindowInputGroup())
			{
				flag = _inputMapSetEvent.TrySetPreviousGroup();
			}
		}

		public void ShowConfirmGarageSellWindow(int index)
		{
			ShowWindow("ConfirmGarageSellWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: true, index.ToString()).Forget();
		}

		public void ShowBusinessWindow(BusinessModel model)
		{
			ShowWindow("BusinessWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: true, JsonConvert.SerializeObject(model)).Forget();
		}

		public void ShowBusinessProfitWindow(BusinessModel model)
		{
			ShowWindow("BusinessProfitWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: true, JsonConvert.SerializeObject(model)).Forget();
		}

		public void ShowPreviousWindow()
		{
			ShowWindow((_prevWindows.Count < 1) ? "MainMenuWindow" : _prevWindows.Last()).Forget();
		}

		public void ShowAchievmentsWindow(bool cleanScreen = true)
		{
			ShowWindow("AchievmentsWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen).Forget();
		}

		public async UniTask<WindowScript> ShowGearboxFilterWindow()
		{
			WindowScript windowScript = await ShowWindow("GearboxFilterWindow", alwaysOnTop: true, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false);
			SuspendNavigationForWindow(windowScript);
			return windowScript;
		}

		public async UniTask<WindowScript> ShowEngineFilterWindow()
		{
			WindowScript windowScript = await ShowWindow("EngineFilterWindow", alwaysOnTop: true, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false);
			SuspendNavigationForWindow(windowScript);
			return windowScript;
		}

		public async UniTask<WindowScript> ShowMarketInventoryFilter()
		{
			WindowScript windowScript = await ShowWindow("MarketInventoryFilterWindow", alwaysOnTop: true, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false);
			SuspendNavigationForWindow(windowScript);
			return windowScript;
		}

		public async UniTask<WindowScript> ShowAllInventoryFilterWindow()
		{
			WindowScript windowScript = await ShowWindow("AllInventoryFilterWindow", alwaysOnTop: true, show: true, null, fadeBlack: false, enableBackButton: true, cleanScreen: false);
			SuspendNavigationForWindow(windowScript);
			return windowScript;
		}

		private void SuspendNavigationForWindow(WindowScript window)
		{
			if (!window)
			{
				return;
			}
			foreach (WindowScript item in activeWindowsDictionary.Values.Where((WindowScript activeWindow) => (bool)activeWindow && activeWindow != window))
			{
				item.SuspendUiNavigation();
			}
			window.OnDestroyAsObservable().Subscribe((Unit _) =>
			{
				foreach (WindowScript item2 in activeWindowsDictionary.Values.Where((WindowScript activeWindow) => activeWindow))
				{
					item2.RestoreUiNavigation();
				}
			});
		}

		public void ShowPlayersList()
		{
			ShowWindow("PlayersListWindow").Forget();
		}

		public void ShowRequestWindow()
		{
			ShowWindow("RequestWindow").Forget();
		}

		public async UniTask<WindowScript> OpenSellCarWindow()
		{
			return await ShowWindow("SellCarWindow");
		}

		public void ShowSubmitCircle()
		{
			ShowWindow("SubmitCircle").Forget();
		}

		public async UniTask<WindowScript> ShowPhoneWindowAsync(bool fadeIn = false, bool cleanScreen = true)
		{
			string text = (fadeIn ? JsonConvert.SerializeObject(new PhoneWindow.AdditionalData
			{
				FadeIn = true
			}) : null);
			ManagerUI managerUI = this;
			string additionalData = text;
			return await managerUI.ShowWindow("PhoneWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen, additionalData);
		}

		public void ShowEventViewWindow(_Car_Parking.Scripts.EventSystem.EventType eventType, bool showOnTop = false)
		{
			if (ScenesHelper.IsMenuSceneActive())
			{
				CloseBalanceWindow();
			}
			EventViewWindow.AdditionalData value = new EventViewWindow.AdditionalData(new IBigBannerView.BannerData(eventType));
			string additionalData = JsonConvert.SerializeObject(value);
			ShowWindow("EventViewWindow", showOnTop, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: true, additionalData).Forget();
		}

		public void ShowSingleEventViewWindow(_Car_Parking.Scripts.EventSystem.EventType eventType, bool showOnTop = false)
		{
			if (ScenesHelper.IsMenuSceneActive())
			{
				CloseBalanceWindow();
			}
			IBigBannerView.BannerData value = new IBigBannerView.BannerData(eventType);
			string additionalData = JsonConvert.SerializeObject(value);
			ShowWindow("SingleEventViewWindow", showOnTop, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: true, additionalData).Forget();
		}

		public async UniTask<WindowScript> ShowBundleOfferWindow(OfferModel offer, string previousWindow = null)
		{
			if (offer == null)
			{
				return null;
			}
			if (ScenesHelper.IsMenuSceneActive())
			{
				CloseBalanceWindow();
			}
			CloseWindow("EventViewWindow");
			EventViewWindow.AdditionalData value = new EventViewWindow.AdditionalData(new IBigBannerView.BannerData(offer))
			{
				SingleOfferOnly = true,
				PreviousWindow = previousWindow
			};
			WindowScript windowScript = await ShowWindow("EventViewWindow", alwaysOnTop: true, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: true, JsonConvert.SerializeObject(value));
			SuspendNavigationForWindow(windowScript);
			return windowScript;
		}

		public void ShowSettingsWindow()
		{
			ShowSettingsWindow(null, true);
		}

		public void ShowSettingsWindow(string additionalData = null, bool cleanScreen = true)
		{
			ShowWindow("SettingsMenuWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen, additionalData).Forget();
		}

		public void CloseSettingsWindow()
		{
			CloseWindow("SettingsMenuWindow");
		}

		public void NotEnoughMoney()
		{
			ShowNotificationWindow(new LocalizedString("ManagerUI.Notenoughmoney"), 3f, Color.red);
			ShowMoneyPurchaseWindow();
		}

		public void NotEnoughCoins()
		{
			ShowNotificationWindow(new LocalizedString("ManagerUI.Notenoughcoins"), 3f, Color.red);
			ShowCoinsPurchaseWindow();
		}

		private void NoInternetConnection()
		{
			ShowNotificationWindow(new LocalizedString("MultiplayerLobbyHandler.Nointernetconnection"), 3f, Color.red);
		}

		public void ShowMoneyPurchaseWindow()
		{
			ShowCoinsOrMoneyWindow(MarketType.MoneysMarket);
		}

		public void ShowCoinsPurchaseWindow()
		{
			ShowCoinsOrMoneyWindow(MarketType.CoinsMarket);
		}

		private void ShowCoinsOrMoneyWindow(MarketType marketType)
		{
			CurrentMarketType = marketType;
			ShowWindow("CoinsAndMoneyPurchaseWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false).Forget();
		}

		public void ShowGameModeWindow()
		{
			ShowWindow("GameModesWindow").Forget();
		}

		public void ShowDisqualifiedWindow()
		{
			ShowWindow("DisqualifiedWindow").Forget();
		}

		public void ShowCircuitFinishSplashWindow()
		{
			ShowWindow("CircuitFinishSplashWindow").Forget();
		}

		public void CloseCircuitFinishSplashWindow()
		{
			CloseWindow("CircuitFinishSplashWindow");
		}

		public void ShowCircuitMapWindow(bool cleanScreen = true, string additionalData = "")
		{
			ShowWindow("CircuitRaceSelectMapWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen, additionalData).Forget();
		}

		public void ShowLoginWindow()
		{
			ShowWindow("LoginWindow").Forget();
		}

		public void ShowParkingLevelsWindow(string additionalData = null)
		{
			ShowWindow("LevelsMenuWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: true, additionalData).Forget();
		}

		public void ShowConfirmGarageBuyWindow(ConfirmGarageBuyWindow.AdditionalData additionalData)
		{
			ShowWindow("ConfirmGarageBuyWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: true, JsonConvert.SerializeObject(additionalData)).Forget();
		}

		public void ShowApartmentGarageBuyWindow(int additionalData = -1)
		{
			ShowWindow("ConfirmApartmentBuyWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: true, additionalData.ToString()).Forget();
		}

		public void CloseConfirmGarageBuyWindow()
		{
			CloseWindow("ConfirmGarageBuyWindow");
		}

		public void ShowRaceInfoWindow()
		{
			ShowWindow("RaceInfoWindow").Forget();
		}

		public void ShowSelectRacigModeWindow()
		{
			ShowWindow("SelectRaceModeWindow").Forget();
		}

		public void ShowOSDWindow()
		{
			ShowWindow("OSDWindow").Forget();
		}

		public void ShowDashboard(bool _enable)
		{
			if (_enable)
			{
				ShowOSDWindow();
			}
			else
			{
				CloseWindow("OSDWindow");
			}
		}

		public void ShowUserBalanceWindow(IWindowInitializer initializer = null, string additionalData = null)
		{
			if (!activeWindowsDictionary.ContainsKey("UserBalanceWindow"))
			{
				GameOverlayWindowInitializer = initializer ?? _prefabFactory.Create<GameOverlayWindowDefaultInitializer>(Array.Empty<object>());
				ShowWindow("UserBalanceWindow", alwaysOnTop: true, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false, additionalData).Forget();
			}
		}

		public void ShowGarageSelectWindow()
		{
			ShowPropertyWindow();
		}

		public void ShowApartmentSelectWindow()
		{
			ShowPropertyWindow("1");
		}

		public void ShowPropertyWindow(string additionalData = "0")
		{
			ShowWindow("PropertyWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: true, additionalData).Forget();
		}

		public void ShowFirstGameWindow()
		{
			ShowWindow("FirstGameWindow").Forget();
		}

		public void ShowCarTuningWindow()
		{
			ShowWindow("CarTuningWindow").Forget();
		}

		public void ShowVisualTuningWindow(bool backButtonState = false, bool cleanScreen = true)
		{
			ShowWindow("VisualTuningWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, backButtonState).Forget();
		}

		public void ShowEngineUIWindow()
		{
			ShowWindow("EngineUIWindow").Forget();
		}

		public void ShowMaintenanceWindow()
		{
			ShowWindow("MaintenanceWindow").Forget();
		}

		public void ShowAirFuelWindow()
		{
			ShowWindow("AirFuelUIWindow").Forget();
		}

		public void ShowWheelsTuningWindow()
		{
			ShowWindow("WheelsUIWindow").Forget();
		}

		public void ShowIntakeExhoustTuningWindow()
		{
			ShowWindow("IntakeExhoustUIWindow").Forget();
		}

		public void ShowDefaultEnginesMenu()
		{
			ShowWindow("DefaultEnginesMarketWindow").Forget();
		}

		public void ShowHeadTunningWindow()
		{
			ShowWindow("HeadUIWindow").Forget();
		}

		public void ShowGearboxTuningWindow()
		{
			ShowWindow("GearboxUIWindow").Forget();
		}

		public void ShowGarageWindow()
		{
			ShowWindow("GarageWindow").Forget();
		}

		public void ShowAllEngineInventoryWindow()
		{
			ShowWindow("AllEngineInventoryWindow").Forget();
		}

		public void ShowMarketInventoryWindow(PartData part, int partIndex)
		{
			m_prevWindow = m_currentWindow;
			ShowWindow("MarketInventoryWindow").Forget();
			SelectPartToMarket(part, partIndex);
		}

		public void ShowBlockUIWindow()
		{
			ShowWindow("BlockUIWindow").Forget();
		}

		public void ShowRotorBlockUIWindow()
		{
			ShowWindow("RotorBlockUIWindow").Forget();
		}

		public void ShowApartmentWindow()
		{
			ShowWindow("SelectApartmentWindow").Forget();
		}

		public WindowScript GetWindow(string windowName)
		{
			return activeWindowsDictionary.GetValueOrDefault(windowName);
		}

		public void ShowCircuitCarSelectWindow(string additionalData = null)
		{
			ShowWindow("CircuitCarSelectWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: true, additionalData).Forget();
		}

		public void ShowFuelDriveUIWindow()
		{
			ShowWindow("FuelDriveUIWindow").Forget();
		}

		public void ShowQualificationWindow()
		{
			ShowWindow("QualificationWindow").Forget();
		}

		public void CloseQualificationWindow()
		{
			CloseWindow("QualificationWindow");
		}

		public void ShowCircuitPlayWindow()
		{
			ShowWindow("CircuitPlayWindow").Forget();
		}

		public void CloseCircuitPlayWindow()
		{
			CloseWindow("CircuitPlayWindow");
		}

		public void ShowFreeWalkWindow()
		{
			ShowWindow("FreeWalkWindow").Forget();
		}

		public void ShowCircuitRaceMenuWindow()
		{
			ShowWindow("CircuitRaceMenuWindow").Forget();
		}

		public void ShowMultiplayerRacePlayWindow()
		{
			ShowWindow("MultiplayerRacePlayWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false).Forget();
		}

		public void CloseMultiplayerRacePlayWindow()
		{
			CloseWindow("MultiplayerRacePlayWindow");
		}

		public void ShowMainMenuWindow()
		{
			ShowWindow("MainMenuWindow").Forget();
		}

		public void ShowUpdateAndroidMessageWindow()
		{
			ShowWindow("UpdateAndroidMessageWindow", alwaysOnTop: true, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false).Forget();
		}

		public void ShowMenuGarageControllerHUD(string previousWindow = "-1")
		{
			CloseWindow("VisualTuningWindow");
			Observable.Timer(TimeSpan.FromMilliseconds(100.0)).Subscribe((long _) =>
			{
				CloseWindow("CarTuningWindow");
			}).AddTo(this);
			CloseWindow("MainMenuWindow");
			ShowWindow("MenuControllerHUD", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: true, previousWindow).Forget();
		}

		public void CloseMenuControllerHUD()
		{
			CloseWindow("MenuControllerHUD");
		}

		public void ShowPrevWindow(bool fade)
		{
			if (m_prevWindow != string.Empty)
			{
				string currentWindow = m_currentWindow;
				ShowWindow(m_prevWindow, alwaysOnTop: false, show: true, null, fade).Forget();
				m_prevWindow = currentWindow;
			}
		}

		public void ShowEventsWindow()
		{
			ShowWindow("EventsWindow").Forget();
		}

		public void ShowSingleRaceWindow()
		{
			ShowWindow("SingleRaceWindow").Forget();
		}

		public void ShowNewCarsWindow(string additionalData = "")
		{
			ShowWindow("NewCarsWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: true, additionalData).Forget();
		}

		public void ShowDynoWindow()
		{
			ShowWindow("DynoWindow").Forget();
		}

		public void ShowEventRaceWindow()
		{
			ShowWindow("EventRaceWindow").Forget();
		}

		public async UniTask ShowWaitingWindow(bool set, bool backButtonState = false, bool alwaysOnTop = true, string additionalData = "", bool updateBackButton = true)
		{
			await ShowWindow("WaitingWindow", alwaysOnTop, set, null, fadeBlack: false, backButtonState, cleanScreen: false, additionalData, withAnimate: false, updateBackButton);
		}

		public async UniTask ShowWaitingWindowNotTransparent(bool set, bool backButtonState = false, bool alwaysOnTop = true, bool updateBackButton = true)
		{
			await ShowWindow("WaitingWindowNotTransparent", alwaysOnTop, set, null, fadeBlack: false, backButtonState, cleanScreen: false, null, withAnimate: false, updateBackButton);
		}

		public void CloseNotTransparentWaitingWindow()
		{
			CloseWindow("WaitingWindowNotTransparent");
		}

		public async UniTask ShowWaitingWindowDark(bool set, bool backButtonState = false, bool alwaysOnTop = true)
		{
			AsyncOperationHandle<GameObject> asyncOperationHandle = Addressables.LoadAssetAsync<GameObject>("WaitingWindowDark");
			await asyncOperationHandle.Task;
			GameObject result = asyncOperationHandle.Result;
			if (_destroyCts.IsCancellationRequested)
			{
				if (asyncOperationHandle.IsValid())
				{
					asyncOperationHandle.Release();
				}
			}
			else
			{
				GameObject target = _prefabFactory.Instantiate(result);
				asyncOperationHandle.BindTo(target);
				UnityEngine.Object.DontDestroyOnLoad(target);
			}
		}

		public void ShowCoolingTuningMenu()
		{
			ShowWindow("CoolingTuningWindow").Forget();
		}

		public void ShowLobbyWindow()
		{
			ShowWindow("LobbyWindow").Forget();
		}

		public void ShowFreeDragRacingWindow()
		{
			if (V.DriverPerson.TryGetComponent<MainPerson>(out var component) && (bool)component.PersonAnimationsController && component.PersonAnimationsController.AnimationService.IsAnimatingPlayer())
			{
				_freeDriveProvider.FreeDriveControl.CancelButtonNegWindow();
			}
			else
			{
				ShowWindow("FreeDragRacingWindow").Forget();
			}
		}

		public void ShowClassChooseWindow()
		{
			ShowWindow("ClassChooseWindow").Forget();
		}

		public async UniTask ShowUserBalanceOnlyWindow()
		{
			UserBalanceWindowActive = true;
			await ShowWindow("UserBalanceOnlyWindow", alwaysOnTop: true);
		}

		public void CloseSeveralWindows(bool userBalanceWindowOnly = false)
		{
			if (!userBalanceWindowOnly)
			{
				CloseWindow("PlayersListWindow");
				CloseWindow("PlayerProfileWindow");
				CloseWindow("AchievmentsWindow");
				CloseWindow("CoinsAndMoneyPurchaseWindow");
			}
			CloseBalanceWindow();
			CloseBalanceOnlyWindow();
		}

		public void CloseBalanceOnlyWindow()
		{
			CloseWindow("UserBalanceOnlyWindow");
			UserBalanceWindowActive = false;
		}

		public void CloseBalanceWindow()
		{
			CloseWindow("UserBalanceWindow");
		}

		public void ShowFreeAnimationWindow()
		{
			ShowUserBalanceOnlyWindow().Forget();
			ShowWindow("AnimationShopWindowForFree").Forget();
		}

		public void CloseFreeAnimationWindow()
		{
			CloseSeveralWindows();
			CloseWindow("AnimationShopWindowForFree");
		}

		public void ShowPersonCustomizationWindow(string additionalData = "")
		{
			ShowWindow("PersonCustomizationWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: true, additionalData).Forget();
			if (ScenesHelper.IsFreeDriveSceneActive() || ScenesHelper.IsParkingLevelActive())
			{
				ShowUserBalanceOnlyWindow().Forget();
			}
		}

		public void ShowServiceProgressWindow()
		{
			ShowWindow("ServiceProgressWindow").Forget();
		}

		public async UniTaskVoid ShowServiceResultWindow()
		{
			(await ShowPopupNotificationWindow()).InitServiceResultWindow();
		}

		public void ShowDailyTasksFull()
		{
			ShowWindow("DailyTasksFull", alwaysOnTop: true, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false).Forget();
		}

		public void ShowDroneControlWindow()
		{
			ShowWindow("DroneControlWindow").Forget();
		}

		public void ShowAuthorizationWindow(string additionalData = "")
		{
			ShowWindow("AuthorizationWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: true, additionalData).Forget();
		}

		public void ShowAchievmentsAuthorizationWindow()
		{
			ShowWindow("AchievmentsAuthorizationWindow").Forget();
		}

		public UniTask<WindowScript> ShowColorPresetWindow()
		{
			return ShowWindow("ColorPresetWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false);
		}

		public void ShowLevelPauseWindow()
		{
			ShowWindow("LevelPauseWindow").Forget();
		}

		public void CloseWaitingWindow(bool canUseDelay = true)
		{
			if (activeWindowsDictionary.ContainsKey("WaitingWindow"))
			{
				CloseWindow("WaitingWindow");
			}
			else if (canUseDelay)
			{
				DelayedCloseWindow("WaitingWindow").Forget();
			}
		}

		public void CloseBootLoadingWindow()
		{
			if ((bool)_waitingWindow)
			{
				UnityEngine.Object.Destroy(_waitingWindow);
			}
		}

		private async UniTask DelayedCloseWindow(string key, int delayFrame = 120)
		{
			await UniTask.DelayFrame(delayFrame, PlayerLoopTiming.Update, _destroyCts.Token);
			CloseWindow(key);
		}

		public async UniTask ShowWaitingWindow(UnityAction intermediateCallback)
		{
			string guid = "WaitingWindow";
			GameObject gameObject = await LoadWindowPrefab(guid);
			if ((bool)gameObject)
			{
				GameObject obj = _prefabFactory.Instantiate(gameObject, WindowsParentAlwaysOnTop);
				WaitingWindow component = obj.GetComponent<WaitingWindow>();
				obj.SetActive(value: true);
				obj.transform.SetAsLastSibling();
				component.InitCallback(intermediateCallback);
			}
		}

		public async UniTask<PopupNotificationWindow> ShowPopupNotificationWindow()
		{
			GameObject gameObject = await LoadWindowPrefab("PopupNotificationWindow");
			if (!gameObject)
			{
				return null;
			}
			return _prefabFactory.Instantiate(gameObject, WindowsParentAlwaysOnTop).GetComponent<PopupNotificationWindow>();
		}

		public void ShowCarBlockNotification(NotificationMessages notificationMessages, float duration, NotificationType notificationType, string description = "")
		{
			string text = "";
			switch (notificationMessages)
			{
			case NotificationMessages.IncompatibleParts:
				text = string.Concat(new LocalizedString("CarBlock.Incompatiblepartsareinstalledintheengine"), " \n", description);
				break;
			case NotificationMessages.CheatEngine:
				text = new LocalizedString("CarBlock.Yourcarinstalledwithanincompatibleengine.Pleasereplaceitwithacompatibleone!");
				break;
			case NotificationMessages.NfsCar:
				text = new LocalizedString("CarBlock.Youcan'tdrivethiscar");
				break;
			case NotificationMessages.CarIsBroken:
				text = new LocalizedString("CarBlock.Carisbroken!!!Pleaserepair");
				break;
			case NotificationMessages.CantDriveThisCar:
				text = new LocalizedString("CarBlock.Youcan'tdrivethiscar");
				break;
			case NotificationMessages.CarInService:
				text = new LocalizedString("CarBlock.CarInService");
				break;
			case NotificationMessages.NoAccount:
				text = new LocalizedString("CarBlock.Youmusthaveanaccounttoexchangeorsellcars!");
				break;
			case NotificationMessages.SitInCar:
				text = new LocalizedString("CarBlock.Youmustbeinyourcar!");
				break;
			case NotificationMessages.NonTradeableCar:
				text = new LocalizedString("CarBlock.Can'ttradethiscar!");
				break;
			}
			ShowNotificationWindow(text, duration, notificationType).Forget();
		}

		public async UniTask ShowLocalizedErrorNotification(string localizationKey)
		{
			await V.CoreGameManager.ManagerUI.ShowNotificationWindow(new LocalizedString(localizationKey), 3f, NotificationType.Error);
		}

		public async UniTask ShowNotificationWindow(string text, float duration, NotificationType type = NotificationType.Success, uint soundSwitchName = 0u, Sprite icon = null, bool withSign = false, bool showBackground = true, float sparkFadeInDuration = -1f, float sparkFadeOutDuration = -1f)
		{
			if (_isDestroyed || !this)
			{
				return;
			}
			CloseNotificationWindow();
			if (!(text == ""))
			{
				GameObject prefab = null;
				try
				{
					string guid = (withSign ? "NotificationWindowWithSign" : "NotificationWindow");
					prefab = await LoadWindowPrefab(guid);
				}
				catch (Exception ex) when (!(ex is OperationCanceledException))
				{
				}
				if (!_isDestroyed && (bool)this && (bool)prefab)
				{
					GameObject obj = _prefabFactory.Instantiate(prefab, WindowsParentAlwaysOnTop);
					(lastNotificationWindow = obj.GetComponent<NotificationWindow>()).Init(text, duration, type, soundSwitchName, icon, showBackground, sparkFadeInDuration, sparkFadeOutDuration);
					obj.SetActive(value: true);
					obj.transform.SetAsLastSibling();
				}
			}
		}

		public void CloseNotificationWindow()
		{
			if ((bool)lastNotificationWindow)
			{
				lastNotificationWindow.Close();
			}
		}

		public void ShowNotificationWindow(string message, float duration, Color color)
		{
			string text = "<color=#" + ColorUtility.ToHtmlStringRGBA(color) + ">" + message + "</color>";
			ShowNotificationWindow(text, duration).Forget();
		}

		private void ShowNotification(string text, float duration)
		{
			ShowNotificationWindow(text, duration).Forget();
		}

		private void ShowNotificationWithOtherSound(string text, float duration, uint switchName)
		{
			ShowNotificationWindow(text, duration, NotificationType.Success, switchName).Forget();
		}

		public async UniTask<PlayerProfileWindow> ShowPlayerProfileWindow()
		{
			if (TryGetAliveWindow<PlayerProfileWindow>("PlayerProfileWindow", out var window))
			{
				return window;
			}
			GameObject gameObject = await LoadWindowPrefab("PlayerProfileWindow");
			if (!gameObject)
			{
				return null;
			}
			if (_isDestroyed)
			{
				return null;
			}
			if (TryGetAliveWindow<PlayerProfileWindow>("PlayerProfileWindow", out window))
			{
				return window;
			}
			PlayerProfileWindow component = _prefabFactory.Instantiate(gameObject, WindowsParent).gameObject.GetComponent<PlayerProfileWindow>();
			component.GUID = "PlayerProfileWindow";
			activeWindowsDictionary["PlayerProfileWindow"] = component;
			return component;
		}

		private bool TryGetAliveWindow<T>(string windowName, out T window) where T : WindowScript
		{
			window = null;
			if (!activeWindowsDictionary.TryGetValue(windowName, out var value))
			{
				return false;
			}
			if (!value)
			{
				activeWindowsDictionary.Remove(windowName);
				RemoveFromPrevWindows(windowName);
				return false;
			}
			if (value is T val)
			{
				window = val;
				return true;
			}
			return false;
		}

		public async UniTask ShowActionButton(string text, bool isPurchase = false, CostType costType = CostType.Money)
		{
			GameObject prefab = null;
			try
			{
				prefab = await LoadWindowPrefab("ActionButton");
			}
			catch (Exception ex) when (!(ex is OperationCanceledException))
			{
			}
			if (!prefab)
			{
				return;
			}
			if ((bool)_lastActionButton)
			{
				UnityEngine.Object.Destroy(_lastActionButton.gameObject);
			}
			GameObject obj = _prefabFactory.Instantiate(prefab, WindowsParent);
			obj.transform.SetSiblingIndex(0);
			ActionButton actionButton = (_lastActionButton = obj.GetComponent<ActionButton>());
			actionButton.Init(text, isPurchase, costType);
			if (ScenesHelper.IsFreeDriveSceneActive())
			{
				actionButton.GetActionButton().onClick.AddListener(() =>
				{
					_freeDriveProvider.FreeDriveControl.OnActionButtonPressed();
				});
			}
			else
			{
				actionButton.GetActionButton().onClick.AddListener(() =>
				{
					_levelsProvider.LevelScript.OnActionButtonPressed();
				});
			}
			obj.SetActive(value: true);
			obj.transform.SetSiblingIndex(WindowsParent.childCount + 1);
		}

		public void HideActionButton()
		{
			if ((bool)_lastActionButton)
			{
				UnityEngine.Object.Destroy(_lastActionButton.gameObject);
			}
		}

		public void CloseAllWindows()
		{
			foreach (WindowScript value in activeWindowsDictionary.Values)
			{
				if ((bool)value)
				{
					value.Close();
				}
			}
		}

		public void CloseCurrentWindow()
		{
			if (activeWindowsDictionary.TryGetValue(m_currentWindow, out var value))
			{
				if ((bool)value)
				{
					value.Close();
				}
				activeWindowsDictionary.Remove(m_currentWindow);
				bool flag = m_currentWindow == "MoneyPurchaseWindow" || m_currentWindow == "CoinsPurchaseWindow";
				if ((IsMainMenuActive() & flag) && _prevWindows.Count > 0 && activeWindowsDictionary.TryGetValue(_prevWindows.Last(), out var value2) && (bool)value2)
				{
					value2.RestoreUiNavigation();
				}
			}
		}

		private void RemoveFromPrevWindows(string windowName)
		{
			if (_prevWindows.Contains(windowName))
			{
				_prevWindows.Remove(windowName);
			}
		}

		public bool IsMainMenuActive()
		{
			if (!activeWindowsDictionary.TryGetValue("MainMenuWindow", out var value) || !value)
			{
				if (!m_currentWindow.IsNullOrEmpty())
				{
					return m_currentWindow.Equals("MainMenuWindow");
				}
				return false;
			}
			return true;
		}

		public void ShowBlackPanel(bool state)
		{
			if ((bool)_windowPanelBackImage)
			{
				_windowPanelBackImage.enabled = state;
			}
		}

		public void CloseWindow(string nameWindow)
		{
			LogActiveWindows("close-" + nameWindow);
			if (activeWindowsDictionary.Remove(nameWindow, out var value))
			{
				if ((bool)value)
				{
					value.Close();
				}
				activeWindowsDictionary.Remove(nameWindow);
			}
		}

		private void LogActiveWindows(string key = "")
		{
			if (_isDebugging)
			{
				Debug.Log("[uiNavigation] _" + key + "_\r\nactive windows: " + activeWindowsDictionary.Keys.Beautify() + "; \r\nprevious windows: " + _prevWindows.Beautify(" -> ") + ";\r\ncurrent window: " + m_currentWindow + ";\r\nstartOpenWindows: " + _startOpenWindows.Beautify() + ";\r\nalwaysOnTopWindows: " + _alwaysOnTopWindows.Beautify());
			}
		}

		public async UniTask<ConfirmWindow> ShowConfirmWindow(UnityAction intermediateCallback, string title, string content, UnityAction cancelCallback = null, string confirmButtonText = "Confirm", Texture texture = null, ConfirmWindow.AdditionalData additionalData = null)
		{
			UISelectionCursor.HideCursor();
			if ((bool)_lastConfirmWindow)
			{
				_lastConfirmWindow.Close();
			}
			GameObject obj = await LoadWindowPrefab("ConfirmWindow");
			GameObject gameObject = _prefabFactory.Instantiate(obj, WindowsParentAlwaysOnTop);
			using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_destroyCts.Token, gameObject.GetCancellationTokenOnDestroy());
			CancellationToken token = cancellationTokenSource.Token;
			ConfirmWindow confirmWindow = (ConfirmWindow)(_lastConfirmWindow = gameObject.GetComponent<ConfirmWindow>());
			if ((bool)_currentWindowScript)
			{
				_currentWindowScript.SuspendUiNavigation();
			}
			if (activeWindowsDictionary.TryGetValue("UserBalanceWindow", out var value))
			{
				value.SuspendUiNavigation();
			}
			gameObject.SetActive(value: true);
			if (cancelCallback != null)
			{
				confirmWindow.InitCallback(intermediateCallback, cancelCallback, title, content, confirmButtonText, texture);
			}
			else
			{
				confirmWindow.InitCallback(intermediateCallback, title, content, confirmButtonText);
			}
			if (additionalData != null)
			{
				confirmWindow.SetAdditionalData(additionalData);
			}
			if (!gameObject)
			{
				return null;
			}
			SelectAnythingOrHideCursor(gameObject, token).Forget();
			return confirmWindow;
		}

		private async UniTask SelectAnythingOrHideCursor(GameObject window, CancellationToken windowCt)
		{
			if ((bool)window)
			{
				(bool, bool) tuple = await UiInputSetup.AutoSelectFirstInteractable(window).AttachExternalCancellation(windowCt).SuppressCancellationThrow();
				if (!tuple.Item1 && !tuple.Item2)
				{
					UISelectionCursor.HideCursor();
				}
			}
		}

		public async UniTask ShowMarketConfirmWindow(PartData part, Action onBuyConfirmed, EngineItem engine = null)
		{
			GameObject gameObject = await LoadWindowPrefab("MarketPopupWindow");
			if ((bool)gameObject)
			{
				GameObject obj = _prefabFactory.Instantiate(gameObject, WindowsParentAlwaysOnTop);
				MarketPopupWindow component = obj.GetComponent<MarketPopupWindow>();
				obj.SetActive(value: true);
				if (engine != null)
				{
					component.InitEngine(engine, onBuyConfirmed);
				}
				else
				{
					component.InitPart(part, onBuyConfirmed);
				}
			}
		}

		public async UniTask<GameObject> LoadWindowPrefab(string guid)
		{
			if (_isDestroyed || !this)
			{
				return null;
			}
			AsyncOperationHandle<GameObject> asyncOperationHandle = Addressables.LoadAssetAsync<GameObject>(guid).BindTo(base.gameObject);
			await asyncOperationHandle.Task;
			_destroyCts.Token.ThrowIfCancellationRequested();
			return asyncOperationHandle.Result;
		}

		public async UniTask<T> CreateWindowGeneric<T>(string guid, bool onTop)
		{
			UISelectionCursor.HideCursor();
			GameObject gameObject = await LoadWindowPrefab(guid);
			if (!gameObject)
			{
				return default;
			}
			GameObject gameObject2 = _prefabFactory.Instantiate(gameObject, onTop ? WindowsParentAlwaysOnTop : WindowsParent);
			gameObject2.OnDestroyAsObservable().Subscribe((Unit _) =>
			{
				foreach (WindowScript value in activeWindowsDictionary.Values)
				{
					value.RestoreUiNavigation();
				}
			});
			foreach (WindowScript value2 in activeWindowsDictionary.Values)
			{
				value2.SuspendUiNavigation();
			}
			UiInputSetup.AutoSelectFirstInteractable(gameObject2).Forget();
			return gameObject2.GetComponent<T>();
		}

		private void ReloadActiveWindowsFromEvent(object sender)
		{
			ReloadActiveWindows();
		}

		public void ReloadActiveWindows()
		{
			foreach (KeyValuePair<string, WindowScript> item in activeWindowsDictionary)
			{
				WindowScript value = item.Value;
				if (value != null)
				{
					value.ReloadWindow();
				}
			}
		}

		public bool ShouldHideBackButtonInputHint()
		{
			if (!IsCurrentBackableWindowListEmpty())
			{
				return m_currentWindow == "MainMenuWindow";
			}
			return true;
		}

		public void Back()
		{
			if (_startOpenWindows.Count <= 0)
			{
				List<IBackableWindow> currentBackableWindow = _currentBackableWindow;
				if (currentBackableWindow != null && currentBackableWindow.Count > 0)
				{
					IBackableWindow backableWindow = _currentBackableWindow.Last();
					backableWindow.Back();
					RemoveFromCurrentWindows(backableWindow);
				}
				else if (m_currentWindow != "MainMenuWindow" && m_currentWindow != "FirstGameWindow" && m_currentWindow != "NewCarsWindow" && FlagsStorage.IsMainMenu)
				{
					ShowMainMenuWindow();
				}
				else
				{
					LogActiveWindows("[managerui] i don't know how we got here");
				}
			}
		}

		public void ShowSuspensionWindow()
		{
			ShowWindow("SuspensionTuningWindow").Forget();
		}

		public void ToggleBalanceWindow(bool enable)
		{
			EnableAllBalanceWindow?.Invoke(enable);
		}

		public void ShowEmojiWindow()
		{
			ShowWindow("EmojiWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false).Forget();
		}

		public void ShowEmoteLockerWindow()
		{
			ShowWindow("EmoteLockerWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false, null, withAnimate: false, updateBackButton: false).Forget();
		}

		public void ShowMapLeaderboardWindow()
		{
			ShowWindow("MapLeaderboardWindow", alwaysOnTop: true, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false).Forget();
		}

		public void CloseMapLeaderboardWindow()
		{
			CloseWindow("MapLeaderboardWindow");
		}

		public void ShowAnimationShopWindow()
		{
			ShowWindow("AnimationShopWindow").Forget();
		}

		public async UniTask<ProgressWindow> ShowProgressWindow(string progressWindowTitle)
		{
			GameObject gameObject = await LoadWindowPrefab("ProgressWindow");
			if (!gameObject)
			{
				return null;
			}
			ProgressWindow component = _prefabFactory.Instantiate(gameObject, WindowsParentAlwaysOnTop).gameObject.GetComponent<ProgressWindow>();
			component.Init(progressWindowTitle);
			return component;
		}

		public void ShowTimeAttackEventPopupWindow(string additionalData)
		{
			ShowWindow("TimeAttackEventPopupWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: true, additionalData).Forget();
		}

		public void ShowSelectMapWindow()
		{
			ShowWindow("SelectLocationWindow").Forget();
		}

		public bool IsWindowActive(string windowName)
		{
			if (!activeWindowsDictionary.TryGetValue(windowName, out var value))
			{
				return false;
			}
			if (!value)
			{
				activeWindowsDictionary.Remove(windowName);
				_startOpenWindows.Remove(windowName);
				return false;
			}
			return true;
		}

		public bool IsWindowActiveOrOpening(string windowName)
		{
			if (!activeWindowsDictionary.ContainsKey(windowName))
			{
				return _startOpenWindows.Contains(windowName);
			}
			return true;
		}

		public bool IsWindowSpawnedOnTop(string windowName)
		{
			if (!WindowsParentAlwaysOnTop || string.IsNullOrEmpty(windowName))
			{
				return false;
			}
			for (int i = 0; i < WindowsParentAlwaysOnTop.childCount; i++)
			{
				Transform child = WindowsParentAlwaysOnTop.GetChild(i);
				if (child.gameObject.activeSelf && child.name.StartsWith(windowName, StringComparison.Ordinal))
				{
					return true;
				}
			}
			return false;
		}

		public async UniTask TryShowOnlineWarningWindowAsync()
		{
		}

		public async UniTask TryShowPrivacyAgeConfirmationWindow(bool dontShowNexWindow)
		{
		}

		public void SetCarSelect(bool active)
		{
			_isRacingCarSelect = active;
		}

		public bool IsRacingCarSelect()
		{
			return _isRacingCarSelect;
		}

		public UniTask<WindowScript> ShowPhotoModeWindow(bool cleanScreen = true)
		{
			return ShowWindow("PhotoModeWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen);
		}

		public void ClosePhotoModeWindow()
		{
			CloseWindow("PhotoModeWindow");
		}

		public void ShowPhotoModePauseWindow()
		{
			ShowWindow("PhotoModePauseWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false).Forget();
		}

		public void ClosePhotoModePauseWindow()
		{
			CloseWindow("PhotoModePauseWindow");
		}

		public void ClosePhoneWindow()
		{
			CloseWindow("PhoneWindow");
		}

		public async UniTask<UIActionButton> CreateActionButton(string buttonName, Action activeActionButton, Action notActiveActionButton)
		{
			Delegate[] invocationList = activeActionButton.GetInvocationList();
			foreach (Delegate obj in invocationList)
			{
				if (obj.Target != null)
				{
					_ = obj.Target.GetType().FullName;
				}
				if (obj.Method != null)
				{
					_ = obj.Method.Name;
				}
			}
			invocationList = notActiveActionButton.GetInvocationList();
			foreach (Delegate obj2 in invocationList)
			{
				if (obj2.Target != null)
				{
					_ = obj2.Target.GetType().FullName;
				}
				if (obj2.Method != null)
				{
					_ = obj2.Method.Name;
				}
			}
			GameObject gameObject = await LoadWindowPrefab(buttonName);
			if (!gameObject)
			{
				return null;
			}
			UIActionButton component = _prefabFactory.Instantiate(gameObject, GetWindowsParent()).GetComponent<UIActionButton>();
			component.Init(activeActionButton, notActiveActionButton);
			return component;
		}

		private void SetInputEnable(string windowName)
		{
			if (FlagsStorage.IsMainMenu)
			{
				FlagsStorage.IsInputEnabledInThisWindow = IsCarInputEnabledInWindow(windowName.Replace("(Clone)", ""), isMainMenu: true, _carInputHardLock);
			}
		}

		public void SetCarInputHardLock(bool locked)
		{
			_carInputHardLock = locked;
			FlagsStorage.IsInputEnabledInThisWindow = !locked;
		}

		public static bool IsCarInputEnabledInWindow(string windowName, bool isMainMenu, bool hardLocked = false)
		{
			if (hardLocked)
			{
				return false;
			}
			if (!isMainMenu)
			{
				return !InputBlockWindows.Contains(windowName);
			}
			return InputAllowMainMenuWindows.Contains(windowName);
		}

		public void ShowPlayerReviewWindow()
		{
		}

		public void ShowSnowManEventHud()
		{
			throw new NotImplementedException();
		}

		public void ShowSnowMenMapWindow()
		{
			ShowWindow("SnowMenMapWindow", alwaysOnTop: true, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false).Forget();
		}

		public void SuspendUiNavigationCurrentWindow()
		{
			if ((bool)_currentWindowScript)
			{
				_currentWindowScript.SuspendUiNavigation();
			}
		}

		public void RestoreUiNavigationCurrentWindow()
		{
			if ((bool)_currentWindowScript)
			{
				_currentWindowScript.RestoreUiNavigation();
			}
		}

		public void ShowBuyThisCarWindow()
		{
			ShowWindow("BuyThisCarWindow").Forget();
		}

		public async UniTask ShowSimpleConfirmWindow(string title, string text, Action confirmAction, Action cancelAction = null, string confirmButtonText = null)
		{
			SimpleConfirmWindow simpleConfirmWindow = (await ShowWindow("SimpleConfirmWindow", alwaysOnTop: true, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false)) as SimpleConfirmWindow;
			if ((bool)simpleConfirmWindow)
			{
				simpleConfirmWindow.Init(title, text, confirmAction, cancelAction, confirmButtonText);
			}
		}

		public async UniTask ShowLegalDocumentWindow(string title, string content, string url)
		{
			LegalDocumentWindow legalDocumentWindow = (await ShowWindow("LegalDocumentWindow", alwaysOnTop: true, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false, null, withAnimate: false, updateBackButton: false)) as LegalDocumentWindow;
			if ((bool)legalDocumentWindow)
			{
				legalDocumentWindow.Init(title, content, url);
			}
		}

		public async UniTask ShowChatWindow()
		{
			await ShowWindow("ChatWindow", alwaysOnTop: true, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false);
			activeWindowsDictionary.Remove("ChatWindow");
		}

		public void ShowRallyPauseWindow()
		{
			ShowWindow("RallyPauseMenuWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false).Forget();
		}

		public void CloseEventViewWindow()
		{
			CloseWindow("EventViewWindow");
		}

		public void ShowSelectRallyLocationsMenuWindow()
		{
			ShowWindow("RallySelectMainMenuWindow").Forget();
		}

		public async UniTask<WindowScript> ShowRallyInfoWindow()
		{
			return await ShowWindow("RallyInfoWindow", alwaysOnTop: true, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false);
		}

		public async UniTask<WindowScript> ShowRallySettingsWindow()
		{
			return await ShowWindow("RallySettingsWindow", alwaysOnTop: true, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false);
		}
	}
}
