using RacingScripts.RaceControllers;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.Racing_Scripts.Rally.Analytics;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.Rally
{
	public class RallyPauseMenuWindow : WindowScript, IBackableWindow
	{
		[SerializeField]
		private Button CloseButton = default;

		[SerializeField]
		private Button BackToMainButton = default;

		[SerializeField]
		private Button PickATrackButton = default;

		[SerializeField]
		private Button RestartButton = default;

		[SerializeField]
		private Button SettingsButton = default;

		[SerializeField]
		private Button StageButton = default;

		private RallyController _rallyController;

		private RaceModeController _raceModeController;

		private RallyAnalytic _rallyAnalytic;

		private RallySessionTracker _rallySessionTracker;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

		[Inject]
		public void Construct(ManagerUI managerUI, RaceModeController raceModeController, RallyAnalytic rallyAnalytic, RallySessionTracker rallySessionTracker)
		{
			_raceModeController = raceModeController;
			_managerUI = managerUI;
			_rallyAnalytic = rallyAnalytic;
			_rallySessionTracker = rallySessionTracker;
		}

		protected override void Start()
		{
			base.Start();
			InitBackButton();
		}

		protected override void OnEnable()
		{
			_rallyController = _raceModeController.SpawnedObject.GetComponent<RallyController>();
			CloseButton.onClick.AddListener(Back);
			BackToMainButton.onClick.AddListener(OnBackToMainMenuClickCallback);
			PickATrackButton.onClick.AddListener(OnPickATrackClickCallback);
			RestartButton.onClick.AddListener(OnRestartLevelClickCallback);
			SettingsButton.onClick.AddListener(OnOpenSettingsClickCallback);
			StageButton.onClick.AddListener(OnOpenStageClickCallback);
			base.OnEnable();
		}

		private void OnDisable()
		{
			CloseButton.onClick.RemoveListener(Back);
			BackToMainButton.onClick.RemoveListener(OnBackToMainMenuClickCallback);
			PickATrackButton.onClick.RemoveListener(OnPickATrackClickCallback);
			RestartButton.onClick.RemoveListener(OnRestartLevelClickCallback);
			SettingsButton.onClick.RemoveListener(OnOpenSettingsClickCallback);
			StageButton.onClick.RemoveListener(OnOpenStageClickCallback);
		}

		protected override void OnDestroy()
		{
			_rallyController = null;
			_managerUI = null;
			base.OnDestroy();
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			_rallyController.Pause(state: false);
			Close();
		}

		private void OnBackToMainMenuClickCallback()
		{
			LogCancelIfStageActive();
			_rallyController.ExitToMainMenu();
			Close();
		}

		private void OnPickATrackClickCallback()
		{
			_rallyController.DisableLapRaceTrafficLightUI();
			LogCancelIfStageActive();
			Time.timeScale = 1f;
			_Car_Parking.Scripts.Audio.Audio.ResetAllVolume();
			_rallyController.OpenMapWindow();
			Close();
		}

		private void OnRestartLevelClickCallback()
		{
			SoundEventsDataHelper.SetGroupState(SoundEventsDataHelper.StateReverPause, SoundEventsDataHelper.OnReverState);
			_rallyController.Restart();
			Close();
		}

		private void LogCancelIfStageActive()
		{
			if (_rallyAnalytic != null && _rallySessionTracker != null && !(_rallyController == null))
			{
				RallyState rallyState = _rallyController.RallyState;
				if (rallyState == RallyState.Race || rallyState == RallyState.Stage || rallyState == RallyState.Start)
				{
					int attempt = _rallySessionTracker.GetAttempt(RallyController.LocationIndex, RallyController.TrajectoryIndex);
					int restartCount = _rallySessionTracker.GetRestartCount();
					_rallyAnalytic.LogStageFailed(_rallyController.BuildStageInfo(), RallyFailReason.CANCEL_BUTTON, attempt, restartCount);
				}
			}
		}

		private void OnOpenSettingsClickCallback()
		{
			_Car_Parking.Scripts.Audio.Audio.SetPauseSoundFilter(on: true);
			_ = _managerUI.ShowWindow("SettingsMenuWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false);
			Close();
		}

		private void OnOpenStageClickCallback()
		{
			_Car_Parking.Scripts.Audio.Audio.SetPauseSoundFilter(on: true);
			_rallyController.GoToGarage();
			Close();
		}
	}
}
