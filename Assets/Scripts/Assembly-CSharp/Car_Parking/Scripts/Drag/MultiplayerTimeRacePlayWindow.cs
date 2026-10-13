using System;
using System.Collections.Generic;
using System.Linq;
using Arikan;
using CPMEngine;
using Car_Parking.Scripts.Racing;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.UIElement;

namespace Car_Parking.Scripts.Drag
{
	public class MultiplayerTimeRacePlayWindow : WindowScript
	{
		private const string LogTag = "[MpTimeRaceWindow]";

		private const float ToRoadButtonShowDelay = 3f;

		[SerializeField]
		private TextMeshProUGUI TimeText = default;

		[SerializeField]
		private TextMeshProUGUI CurrentPenaltyText = default;

		[SerializeField]
		private TextMeshProUGUI FinalPenaltyText = default;

		[SerializeField]
		private TextMeshProUGUI BestTimeText = default;

		[SerializeField]
		private Button StopRaceButton = default;

		[SerializeField]
		private Button RestartRaceButton = default;

		[SerializeField]
		private Button ToRoadButton = default;

		[SerializeField]
		private GameObject OutOffRoadPanel = default;

		[Header("MapElements")]
		[SerializeField]
		private MiniMapView MiniMapView = default;

		[SerializeField]
		private Sprite UserIcon = default;

		[SerializeField]
		private RawImage SmallMapImage = default;

		[SerializeField]
		[Header("LightButton")]
		private Button LightOnButton = default;

		[SerializeField]
		private Image LightOnImage = default;

		[SerializeField]
		private Sprite[] LightOnSprites = default;

		[SerializeField]
		private FinishFlashController FinishFlashController = default;

		private IDisposable ToRoadButtonShowDisposable;

		private List<CanvasGroup> _hidableCanvasGroups = new List<CanvasGroup>();

		private FreeTimeAttackController _timeAttackController;

		private Dictionary<string, Image> _otherUsersMapIconDictionary = new Dictionary<string, Image>();

		private FreeDriveControl _freeDriveControl;

		private float _userTime;

		private FreeRaceCar _raceCar;

		private FreeDriveProvider _freeDriveProvider;

		private RaceTriggersValidator _raceTriggersValidator;

		private Tween _tween1;

		private Tween _tween2;

		private float _cachedPenaltyTime;

		private float _cachedFinalPenaltyTime;

		private bool _isDestroyed;

		private ParkingMenuUI _parkingMenuUI;

		private LightController _lightController;

		private QuickCarSettings _quickCarSettings;

		private EventRestrictionService _eventRestrictionService;

		private OnlineCollectionsProvider _onlineCollectionsProvider;

		private LevelScript _levelScript;

		private UiNotificationsManager _uiNotificationsManager;

		private CPMGraphicsSettings.ColorCorrectionLut _defaultColorCorrectionLut;

		private EventRegistry _eventRegistry;

		private RegistryHandle<ScriptableEvent> _toRoadHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToRoad);

		private RegistryHandle<ScriptableEvent> _restartRaceHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.RestartRace);

		private RegistryHandle<ScriptableEvent> _cancelRaceHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.CancelRace);

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.CarFreeRace;

		protected override bool ChangeRootGameInputGroup => true;

		[Inject]
		public void Construct(FreeDriveControl freeDriveControl, FreeDriveProvider freeDriveProvider, FreeTimeAttackController timeAttackController, ParkingMenuUI parkingMenuUI, QuickCarSettings quickCarSettings, EventRestrictionService eventRestrictionService, OnlineCollectionsProvider onlineCollectionsProvider, LevelScript levelScript, UiNotificationsManager uiNotificationsManager, EventRegistry eventRegistry)
		{
			_freeDriveControl = freeDriveControl;
			_freeDriveProvider = freeDriveProvider;
			_timeAttackController = timeAttackController;
			_parkingMenuUI = parkingMenuUI;
			_quickCarSettings = quickCarSettings;
			_eventRestrictionService = eventRestrictionService;
			_onlineCollectionsProvider = onlineCollectionsProvider;
			_levelScript = levelScript;
			_uiNotificationsManager = uiNotificationsManager;
			_eventRegistry = eventRegistry;
		}

		protected override void Awake()
		{
			_raceTriggersValidator = _timeAttackController.RaceTriggersValidator;
			if (_timeAttackController.TimeAttackMode != TimeAttackMode.Rally)
			{
				MiniMapView.SetMiniMapBounds(_raceTriggersValidator.MapBounds);
			}
			base.Awake();
		}

		private void SubscribeToActions()
		{
			_toRoadHandle.Get(_eventRegistry).Subscribe(ToRoad);
			_restartRaceHandle.Get(_eventRegistry).Subscribe(RestartRace);
			_cancelRaceHandle.Get(_eventRegistry).Subscribe(StopRace);
		}

		private void UnsubscribeFromActions()
		{
			_toRoadHandle.Get(_eventRegistry)?.Unsubscribe(ToRoad);
			_restartRaceHandle.Get(_eventRegistry)?.Unsubscribe(RestartRace);
			_cancelRaceHandle.Get(_eventRegistry)?.Unsubscribe(StopRace);
		}

		private void DisableParkingButtons()
		{
			if ((bool)_freeDriveControl.inviteToRaceButton)
			{
				_freeDriveControl.inviteToRaceButton.SetActive(active: false);
			}
			if ((bool)_freeDriveControl.toRoadButton)
			{
				_freeDriveControl.toRoadButton.SetActive(value: false);
			}
		}

		protected override void Start()
		{
			base.Start();
			_timeAttackController.OnResume(state: true);
			int defaultColorCorrectionLut = PlayerPrefs.GetInt("colorCorrection", 0);
			_defaultColorCorrectionLut = (CPMGraphicsSettings.ColorCorrectionLut)defaultColorCorrectionLut;
			DisableParkingButtons();
			_lightController = V.Car.GetComponent<LightController>();
			LightOnButton.onClick.AddListener(ChangeLightState);
			LightOnImage.sprite = LightOnSprites[_lightController.HState];
			_parkingMenuUI.FreeRacingShowElements(state: false);
			if (_timeAttackController.TimeAttackMode != TimeAttackMode.Rally)
			{
				InitMap();
			}
			_raceCar = _freeDriveProvider.FirstCarMultiPlayer.GetComponent<FreeRaceCar>();
			StopRaceButton.onClick.AddListener(StopRace);
			ToRoadButton.onClick.AddListener(ToRoad);
			RestartRaceButton.onClick.AddListener(RestartRace);
			SubscribeMethods();
			PersonMultiplayerDataSending.OnPersonDataApplied += OnPlayerDataApplied;
			PersonMultiplayerDataSending.OnPersonDataRemoved += OnPlayerDataRemoved;
			InitOtherUsersIcon();
			bool state = _timeAttackController.RaceStatus == Car_Parking.Scripts.Racing.TimeAttackStatus.Race;
			HidePreStartRaceButtons(state);
			BestTimeText.text = TimeAttackUIStringHelper.GetFormattedBestTime(_timeAttackController.GetBestTime(_timeAttackController.CurrentTrackIndex, _raceCar.TimeAttackMode));
			AddHidableButtons();
			SetAlphaOnParkingMenuItems(state: false);
			SubscribeToActions();
		}

		private void HidePreStartRaceButtons(bool state)
		{
			RestartRaceButton.SetActive(state);
			ToRoadButton.SetActive(state);
		}

		private void AddHidableButtons()
		{
			CanvasGroup component = _freeDriveControl.toRoadButton.GetComponent<CanvasGroup>();
			CanvasGroup component2 = _freeDriveControl.inviteToRaceButton.GetComponent<CanvasGroup>();
			if ((bool)component)
			{
				_hidableCanvasGroups.Add(component);
			}
			if ((bool)component2)
			{
				_hidableCanvasGroups.Add(component2);
			}
		}

		private void SetAlphaOnParkingMenuItems(bool state)
		{
			foreach (CanvasGroup item in _hidableCanvasGroups.Where((CanvasGroup t) => t))
			{
				item.alpha = (state ? 1 : 0);
			}
		}

		private void RestartRace()
		{
			_timeAttackController.Restart();
		}

		private void ActivateDoorButton(bool state)
		{
			if ((bool)_quickCarSettings && (bool)_quickCarSettings.Door)
			{
				_quickCarSettings.Door.SetInteractable(state);
			}
		}

		private void InitOtherUsersIcon()
		{
			_onlineCollectionsProvider.PlayersOnlineNow?.Count();
			MultiPersonControl[] playersOnlineNow = _onlineCollectionsProvider.PlayersOnlineNow;
			foreach (MultiPersonControl multiPersonControl in playersOnlineNow)
			{
				if ((bool)multiPersonControl && !multiPersonControl.ownPlayer && (bool)multiPersonControl.FirstAndOwnCar)
				{
					_otherUsersMapIconDictionary.Add(multiPersonControl.ThisPlayerID, MiniMapView.Follow(multiPersonControl.FirstAndOwnCar.transform));
				}
			}
		}

		private void OnPlayerDataApplied(MultiPersonControl mpc)
		{
			if ((bool)mpc && (bool)mpc.FirstAndOwnCar)
			{
				_otherUsersMapIconDictionary.Add(mpc.ThisPlayerID, MiniMapView.Follow(mpc.FirstAndOwnCar.transform));
			}
		}

		private void OnPlayerDataRemoved(MultiPersonControl mpc)
		{
			if ((bool)mpc && !mpc.ownPlayer && (bool)mpc.FirstAndOwnCar)
			{
				MiniMapView.UnfollowTarget(mpc.FirstAndOwnCar.transform);
			}
			else
			{
				MiniMapView.RemoveNullTargets();
			}
			if ((bool)mpc && _otherUsersMapIconDictionary.TryGetValue(mpc.ThisPlayerID, out var value))
			{
				_otherUsersMapIconDictionary.Remove(mpc.ThisPlayerID);
				if ((bool)value)
				{
					MiniMapView.DestroyMapItem(value.gameObject);
				}
			}
		}

		private void StopRace()
		{
			_timeAttackController.StopRaceImmediately(FailReason.CANCEL_BUTTON);
		}

		private void ToRoad()
		{
			_timeAttackController.TryCarToRoad();
			ShowToRoadButton(state: false);
			ToRoadButtonShowDisposable?.Dispose();
			ToRoadButtonShowDisposable = Observable.Timer(TimeSpan.FromSeconds(3.0)).Subscribe((long _) =>
			{
				ShowToRoadButton(state: true);
			}).AddTo(this);
		}

		private void ShowToRoadButton(bool state)
		{
			if ((bool)ToRoadButton)
			{
				ToRoadButton.SetActive(state);
			}
		}

		private void RaceStarted()
		{
			HidePreStartRaceButtons(state: true);
		}

		private void OnCarToRoad(GameObject car)
		{
			if (!(car != V.Car))
			{
				float num = 10f;
				if (_raceCar.TimeAttackMode == TimeAttackMode.TimeAttack)
				{
					string format = new LocalizedString("FreeRace.AddPenaltyToRoad");
					_managerUI.ShowNotificationWindow(string.Format(format, num), 2f).Forget();
				}
			}
		}

		private void OnUserTimeUpdate(float time, float currentPenaltyTime, float finalPenaltyTime)
		{
			_userTime = time;
			TimeText.text = Utils.FormatTime(_userTime);
			bool flag = currentPenaltyTime > 0f;
			OutOffRoadPanel.SetActive(flag);
			if (flag)
			{
				if (_cachedPenaltyTime < currentPenaltyTime)
				{
					_cachedPenaltyTime = currentPenaltyTime;
					_tween1?.Kill();
					_tween1 = UIHelper.AnimatedChangeTimeText(CurrentPenaltyText, currentPenaltyTime);
				}
			}
			else
			{
				_cachedPenaltyTime = 0f;
				CurrentPenaltyText.text = "";
			}
			if (finalPenaltyTime > 0f)
			{
				if (_cachedFinalPenaltyTime < finalPenaltyTime)
				{
					_cachedFinalPenaltyTime = finalPenaltyTime;
					_tween2?.Kill();
					_tween2 = UIHelper.AnimatedChangeTimeText(FinalPenaltyText, finalPenaltyTime);
				}
			}
			else
			{
				FinalPenaltyText.text = "--:--";
			}
		}

		private void SubscribeMethods()
		{
			FreeDriveControl freeDriveControl = _freeDriveControl;
			freeDriveControl.OnCarToRoad = (Action<GameObject>)Delegate.Combine(freeDriveControl.OnCarToRoad, new Action<GameObject>(OnCarToRoad));
			FreeTimeAttackController timeAttackController = _timeAttackController;
			timeAttackController.OnRaceFinish = (Action<bool, float, TimeAttackMode>)Delegate.Combine(timeAttackController.OnRaceFinish, new Action<bool, float, TimeAttackMode>(OnFinishRace));
			if ((bool)_raceCar)
			{
				FreeRaceCar raceCar = _raceCar;
				raceCar.UserTimeAction = (Action<float, float, float>)Delegate.Combine(raceCar.UserTimeAction, new Action<float, float, float>(OnUserTimeUpdate));
				FreeRaceCar raceCar2 = _raceCar;
				raceCar2.UserStartRace = (Action)Delegate.Combine(raceCar2.UserStartRace, new Action(RaceStarted));
			}
		}

		private void Flash()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.UITimeAttackFinishFlash, base.gameObject);
			FinishFlashController.TriggerFinishFlash();
		}

		private void OnFinishRace(bool finishCorrectly, float currentTime, TimeAttackMode timeAttackMode)
		{
			Flash();
			FinishFlashController.TriggerFinishFlash();
			HidePreStartRaceButtons(state: false);
			PopupData popupData = new PopupData();
			popupData.PopupKey = "TimeNotBeatPopup";
			RaceBestData bestTime = _timeAttackController.GetBestTime(_timeAttackController.CurrentTrackIndex);
			if (finishCorrectly)
			{
				if (_timeAttackController.IsResultBeated(currentTime, bestTime))
				{
					popupData.PopupKey = "TimeBeatPopup";
					popupData.AudioKey = SoundEventsDataHelper.UITimeAttackBestResultShowSwitch;
				}
				else
				{
					popupData.AudioKey = SoundEventsDataHelper.UITimeAttackDefaultResultShowSwitch;
				}
				popupData.Title = TimeAttackUIStringHelper.GetFormattedFinishTime(currentTime, _raceCar.TimeAttackMode);
				_uiNotificationsManager.ShowPopup(popupData);
			}
			DelayedClose().Forget();
		}

		private async UniTaskVoid DelayedClose()
		{
			await UniTask.Delay(TimeSpan.FromSeconds(1.0));
			if (!_isDestroyed)
			{
				Close();
			}
		}

		public override void Close()
		{
			if (base.IsCloseCanceled)
			{
				return;
			}
			_parkingMenuUI.FreeRacingShowElements(state: true);
			if (_timeAttackController.RaceStatus == Car_Parking.Scripts.Racing.TimeAttackStatus.Finish)
			{
				_parkingMenuUI.Show();
				if (FlagsStorage.OutOfCar)
				{
					_levelScript.CleanForPersonView();
				}
				else
				{
					_levelScript.UnCleanScreen();
				}
			}
			else
			{
				_timeAttackController.OnResume(state: false);
			}
			base.Close();
		}

		private void UnsubscribeMethods()
		{
			FreeDriveControl freeDriveControl = _freeDriveControl;
			freeDriveControl.OnCarToRoad = (Action<GameObject>)Delegate.Remove(freeDriveControl.OnCarToRoad, new Action<GameObject>(OnCarToRoad));
			FreeTimeAttackController timeAttackController = _timeAttackController;
			timeAttackController.OnRaceFinish = (Action<bool, float, TimeAttackMode>)Delegate.Remove(timeAttackController.OnRaceFinish, new Action<bool, float, TimeAttackMode>(OnFinishRace));
			if ((bool)_raceCar)
			{
				FreeRaceCar raceCar = _raceCar;
				raceCar.UserTimeAction = (Action<float, float, float>)Delegate.Remove(raceCar.UserTimeAction, new Action<float, float, float>(OnUserTimeUpdate));
				FreeRaceCar raceCar2 = _raceCar;
				raceCar2.UserStartRace = (Action)Delegate.Remove(raceCar2.UserStartRace, new Action(RaceStarted));
			}
			UnsubscribeFromActions();
		}

		private void InitMap()
		{
			SmallMapImage.SetActive(active: true);
			SmallMapImage.texture = _raceTriggersValidator.GetMapTexture(_timeAttackController.CurrentTrackIndex, _timeAttackController.CurrentTrajectoryIndex);
			SmallMapImage.SetNativeSize();
			MiniMapView.FollowCentered(V.Car.transform, UserIcon, needCacheTransform: true);
		}

		private void ChangeLightState()
		{
			_quickCarSettings.TurnOnLights();
			LightOnImage.sprite = LightOnSprites[_lightController.HState];
		}

		private void OnDisable()
		{
			SetAlphaOnParkingMenuItems(state: true);
			UnsubscribeMethods();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			PersonMultiplayerDataSending.OnPersonDataApplied -= OnPlayerDataApplied;
			PersonMultiplayerDataSending.OnPersonDataRemoved -= OnPlayerDataRemoved;
			ActivateDoorButton(state: true);
			_isDestroyed = true;
			ToRoadButtonShowDisposable?.Dispose();
			_tween1?.Kill();
			_tween2?.Kill();
			UnsubscribeMethods();
			if ((bool)MiniMapView && (bool)V.Car)
			{
				MiniMapView.UnfollowTarget(V.Car.transform);
			}
		}
	}
}
