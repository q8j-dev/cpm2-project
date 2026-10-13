using System;
using Arikan;
using CPMEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using RacingScripts.RaceControllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	public class CircuitPlayWindow : WindowScript
	{
		private const int DelayEnableToRoad = 3;

		[SerializeField]
		private GameObject[] ElementsForPitStop;

		[SerializeField]
		private GameObject MapPanel;

		[SerializeField]
		private CircuitPitStopWindow CircuitPitStopWindow;

		[SerializeField]
		private GameObject WrongWayPanel;

		[SerializeField]
		private GameObject OutOfRoadPanel;

		[SerializeField]
		private GameObject CrashPanel;

		[SerializeField]
		private TextMeshProUGUI RaceResultText;

		[SerializeField]
		private TextMeshProUGUI CrashResultText;

		[SerializeField]
		private TextMeshProUGUI PenaltyText;

		[SerializeField]
		private RawImage SmallMapImage;

		[SerializeField]
		private Button SettingsButton;

		[SerializeField]
		private Button PhotoModeButton;

		[SerializeField]
		private MiniMapView MiniMapView;

		[SerializeField]
		private Button ToRoadButton;

		[SerializeField]
		private Sprite UsersIcon;

		[SerializeField]
		private ImageWithTextElement FuelPitStopPanel;

		[SerializeField]
		private ImageWithTextElement WheelPitStopPanel;

		[SerializeField]
		private Button ArrowButton;

		[SerializeField]
		private DealerWindowFilterClassArrow DealerWindowFilterClassArrow;

		[SerializeField]
		private TextMeshProUGUI CurrentTimeText;

		[SerializeField]
		private TextMeshProUGUI BestTimeText;

		[SerializeField]
		private GameObject TimerPanel;

		[SerializeField]
		private GameObject TimerButton;

		[SerializeField]
		private GameObject[] PitStopControlButtons;

		[SerializeField]
		private GameObject[] RaceControlButtons;

		[SerializeField]
		private RacingCarControllerUI RacingCarControllerUI;

		private CircuitRacingUIControl _circuitRacingUIControl;

		private CircuitRaceControlller _circuitRaceControlller;

		private RacingUIState _currentRacingUIState;

		private bool _timerPanelVisible;

		private Tween _bestTimeColorTween;

		private RaceModeController _raceModeController;

		private EventRegistry _eventRegistry;

		private RegistryHandle<ScriptableEvent> _openPhotoModeHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OpenPhotoMode);

		private RegistryHandle<ScriptableEvent> _onMenuHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnMenu);

		private RegistryHandle<ScriptableEvent> _toggleTimerHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleTimer);

		private RegistryHandle<ScriptableEvent> _toRoadHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToRoad);

		public CircuitPitStopWindow WindowCircuitPitStop => CircuitPitStopWindow;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.CarRacing;

		protected override bool ChangeRootGameInputGroup => true;

		[Inject]
		public void Construct(RaceModeController raceModeController, EventRegistry eventRegistry)
		{
			_raceModeController = raceModeController;
			_eventRegistry = eventRegistry;
		}

		public void SetCurrentTime(float time)
		{
			CurrentTimeText.text = Utils.ConvertToTime(time);
		}

		public void RestartUI()
		{
			_circuitRacingUIControl.ShowChartList(state: false);
			MiniMapView.ClearTargets();
			ShowToRoad(state: true);
		}

		protected override void Start()
		{
			_circuitRaceControlller = _raceModeController.SpawnedObject.GetComponent<CircuitRaceControlller>();
			_circuitRacingUIControl = _raceModeController.SpawnedObject.GetComponent<CircuitRacingUIControl>();
			MiniMapView.SetMiniMapBounds(_circuitRacingUIControl.MapBounds);
			_circuitRacingUIControl.SetCircuitPlayWindow(this);
			ToRoadButton.onClick.AddListener(ToRoad);
			ArrowButton.onClick.AddListener(ArrowButtonOnClickCallback);
			SettingsButton.onClick.AddListener(ShowSettingsWindow);
			PhotoModeButton.onClick.AddListener(OpenPhotoMode);
			ShowToRoad(state: false);
			FuelPitStopPanel.SetActive(active: false);
			WheelPitStopPanel.SetActive(active: false);
			_onMenuHandle.Get(_eventRegistry).Subscribe(ShowSettingsWindow);
			_openPhotoModeHandle.Get(_eventRegistry).Subscribe(OpenPhotoMode);
			_toggleTimerHandle.Get(_eventRegistry).Subscribe(ArrowButtonOnClickCallback);
			_toRoadHandle.Get(_eventRegistry).Subscribe(ToRoad);
			base.Start();
		}

		protected override void OnDestroy()
		{
			ToRoadButton.onClick.RemoveAllListeners();
			SettingsButton.onClick.RemoveAllListeners();
			_onMenuHandle.Get(_eventRegistry)?.Unsubscribe(ShowSettingsWindow);
			_openPhotoModeHandle.Get(_eventRegistry)?.Unsubscribe(OpenPhotoMode);
			_toggleTimerHandle.Get(_eventRegistry)?.Unsubscribe(ArrowButtonOnClickCallback);
			_toRoadHandle.Get(_eventRegistry)?.Unsubscribe(ToRoad);
			base.OnDestroy();
		}

		private void Restart()
		{
			_circuitRaceControlller.RestartRace();
		}

		private void ArrowButtonOnClickCallback()
		{
			_timerPanelVisible = !_timerPanelVisible;
			if (_timerPanelVisible)
			{
				SetTimerValues();
			}
			if ((bool)DealerWindowFilterClassArrow)
			{
				DealerWindowFilterClassArrow.UpdateState(_timerPanelVisible);
			}
			if ((bool)TimerPanel)
			{
				TimerPanel.SetActive(_timerPanelVisible);
			}
		}

		public void ShowRaceUIElements(bool state)
		{
			TimerButton.SetActive(state);
		}

		public void ShowTimerPanel()
		{
			_timerPanelVisible = false;
			ArrowButtonOnClickCallback();
			SetTimerValues();
		}

		public void SetPitStopMessage(MaintenanceType maintenanceType, MaintenanceStatus maintenanceStatus, string maintenanceText)
		{
			Color newColor = ((maintenanceStatus == MaintenanceStatus.Warning) ? Color.yellow : Color.red);
			float duration = ((maintenanceStatus == MaintenanceStatus.Critical) ? (-1f) : 7f);
			switch (maintenanceType)
			{
			case MaintenanceType.Fuel:
				FuelPitStopPanel.ChangeState(newColor, maintenanceText, duration);
				break;
			case MaintenanceType.Wheel:
				WheelPitStopPanel.ChangeState(newColor, maintenanceText, duration);
				break;
			}
		}

		private void ShowSettingsWindow()
		{
			if (InputMapSetEvent.CurrentInputGroup == InputGroupsEnum.CarRacing && (!_circuitRaceControlller || !_circuitRaceControlller.IsUserFinishSequenceStarted))
			{
				_circuitRacingUIControl.ShowChartList(state: false);
				_managerUI.ShowWindow("CircuitRacePauseWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false).Forget();
				SoundEventsDataHelper.SetGroupState(SoundEventsDataHelper.StateReverPause, SoundEventsDataHelper.OffReverState);
				_Car_Parking.Scripts.Audio.Audio.MuteExcludeUI();
				_circuitRacingUIControl.Pause();
				base.gameObject.SetActive(value: false);
			}
		}

		public void SetTimerValues()
		{
			LapRaceCar userLapRaceCar = _circuitRaceControlller.circuitCarsControl.GetUserLapRaceCar();
			if ((bool)userLapRaceCar)
			{
				float bestLap = userLapRaceCar.GetBestLap();
				BestTimeText.text = Utils.ConvertToTime(bestLap);
			}
		}

		private void OpenPhotoMode()
		{
			if (!_circuitRaceControlller || !_circuitRaceControlller.IsUserFinishSequenceStarted)
			{
				_managerUI.ShowPhotoModeWindow(cleanScreen: false);
				SoundEventsDataHelper.SetGroupState(SoundEventsDataHelper.StateReverPause, SoundEventsDataHelper.OffReverState);
				if ((bool)V.DriverPerson && V.DriverPerson.TryGetComponent<MainPerson>(out var component))
				{
					component.EnterPhotoModeHeadVisible();
				}
			}
		}

		protected override void Awake()
		{
			ChangeUIState(RacingUIState.Race);
			base.Awake();
		}

		public void Resume(bool resetRacingState = false)
		{
			RacingCarControllerUI.Resume();
		}

		public void InitPitStop()
		{
			ChangeUIState(RacingUIState.PitStop);
			if (!CircuitPitStopWindow.gameObject.activeSelf)
			{
				RacingCarControllerUI.SetVisibleSpeedometer(visible: false);
				MapPanel.SetActive(value: false);
				CircuitPitStopWindow.SetActive(active: true);
				EnableHideLoadingObjects(state: false);
			}
		}

		public void EnableHideLoadingObjects(bool state)
		{
			GameObject[] elementsForPitStop = ElementsForPitStop;
			foreach (GameObject gameObject in elementsForPitStop)
			{
				if ((bool)gameObject)
				{
					gameObject.SetActive(state);
				}
			}
		}

		public void SetMapImage(Texture texture)
		{
			SmallMapImage.SetActive(active: true);
			SmallMapImage.texture = texture;
			SmallMapImage.SetNativeSize();
		}

		public void Exit()
		{
			ChangeUIState(RacingUIState.Race);
			MapPanel.SetActive(value: true);
			RacingCarControllerUI.SetVisibleSpeedometer(visible: true);
			CircuitPitStopWindow.SetActive(active: false);
			EnableHideLoadingObjects(state: true);
		}

		public override void Close()
		{
			if (!base.IsCloseCanceled)
			{
				RacingCarControllerUI.SetVisibleSpeedometer(visible: true);
				CircuitPitStopWindow.SetActive(active: false);
				base.Close();
			}
		}

		public void ShowWrongWayPanel(bool state)
		{
			WrongWayPanel.SetActive(state);
		}

		public void ShowOutOfRoadPanel(bool state, string penaltyText)
		{
			string text = new LocalizedString("CircuitPlayWindow.OutOfRoad");
			PenaltyText.text = text + " \n " + penaltyText;
			_circuitRaceControlller.RaceCrewTalkSoundController.PlayOffRoadSoundEvent(state);
			OutOfRoadPanel.SetActive(state);
		}

		public void ShowCrashPanel(bool state, float secs)
		{
			string text = new LocalizedString("CircuitPlayWindow.Crash");
			string format = new LocalizedString("CircuitPlayWindow.PenaltyTime");
			CrashResultText.text = text + " \n " + string.Format(format, secs);
			CrashPanel.SetActive(state);
		}

		private void ChangeUIState(RacingUIState state)
		{
			_currentRacingUIState = state;
			if (_currentRacingUIState == RacingUIState.PitStop)
			{
				GameObject[] pitStopControlButtons = PitStopControlButtons;
				for (int i = 0; i < pitStopControlButtons.Length; i++)
				{
					pitStopControlButtons[i].SetActive(value: true);
				}
				pitStopControlButtons = RaceControlButtons;
				for (int i = 0; i < pitStopControlButtons.Length; i++)
				{
					pitStopControlButtons[i].SetActive(value: false);
				}
			}
			else
			{
				GameObject[] pitStopControlButtons = PitStopControlButtons;
				for (int i = 0; i < pitStopControlButtons.Length; i++)
				{
					pitStopControlButtons[i].SetActive(value: false);
				}
				pitStopControlButtons = RaceControlButtons;
				for (int i = 0; i < pitStopControlButtons.Length; i++)
				{
					pitStopControlButtons[i].SetActive(value: true);
				}
			}
		}

		public void ToRoad()
		{
			if (!ToRoadButton || !ToRoadButton.gameObject.activeInHierarchy || !ToRoadButton.interactable)
			{
				return;
			}
			LapRaceCar userLapRaceCar = _circuitRaceControlller.circuitCarsControl.GetUserLapRaceCar();
			if (!userLapRaceCar)
			{
				return;
			}
			userLapRaceCar.ToRoad();
			ShowToRoad(state: false);
			DelayedEnableToRoad(3).Forget();
			RaceMapModel raceMapModel = _circuitRaceControlller.GetRaceMapModel();
			RaceConfigSO raceConfigSO = (raceMapModel ? raceMapModel.RaceConfig : null);
			if (raceConfigSO != null)
			{
				if (raceConfigSO.IsLeman)
				{
					userLapRaceCar.AddPenaltyDistance(raceConfigSO.PenaltyMetersForToRoad);
					_managerUI.ShowNotificationWindow($"Penalty: - {raceConfigSO.PenaltyMetersForToRoad} meters!", 2f).Forget();
				}
				else
				{
					userLapRaceCar.AddPenaltyTime(raceConfigSO.PenaltySecondsForToRoad);
					_managerUI.ShowNotificationWindow($"Penalty: + {raceConfigSO.PenaltySecondsForToRoad} seconds!", 2f).Forget();
				}
			}
		}

		private async UniTaskVoid DelayedEnableToRoad(int delay)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
			ShowToRoad(state: true);
		}

		public void ShowToRoad(bool state)
		{
			ToRoadButton.SetActive(state);
		}

		public void ShowReverseCam(bool state)
		{
			RacingCarControllerUI.ReverseCamButton.SetActive(state);
		}

		public void SetOwnCarOnMap(Transform carTransform)
		{
			MiniMapView.FollowCentered(carTransform, UsersIcon);
		}

		public void SetCarOnMap(Transform carTransform)
		{
			MiniMapView.Follow(carTransform);
		}

		public void ClearMap()
		{
			MiniMapView.ClearTargets();
		}

		public void BestTimeChanged()
		{
			_bestTimeColorTween?.Kill();
			_bestTimeColorTween = BestTimeText.DOColor(Color.green, 0.5f).OnComplete(() =>
			{
				BestTimeText.DOColor(Color.white, 0.5f);
			});
		}

		public void HideWheelWarning()
		{
			WheelPitStopPanel.SetActive(active: false);
		}
	}
}
