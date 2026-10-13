using Cysharp.Threading.Tasks;
using RacingScripts.RaceControllers;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class CircuitRacePauseWindow : WindowScript, IBackableWindow
	{
		[SerializeField]
		private Button CloseButton;

		[SerializeField]
		private Button BackToMainButton;

		[SerializeField]
		private Button PickATrackButton;

		[SerializeField]
		private Button RestartButton;

		[SerializeField]
		private Button SettingsButton;

		private CircuitRaceControlller _circuitRaceControlller;

		private CircuitRacingUIControl _circuitRacingUI;

		private RaceModeController _raceModeController;

		private LevelScript _levelScript;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

		[Inject]
		public void Construct(ManagerUI managerUI, RaceModeController raceModeController)
		{
			_raceModeController = raceModeController;
			_managerUI = managerUI;
		}

		protected override void OnEnable()
		{
			_circuitRacingUI = _raceModeController.SpawnedObject.GetComponent<CircuitRacingUIControl>();
			_circuitRaceControlller = _raceModeController.SpawnedObject.GetComponent<CircuitRaceControlller>();
			CloseButton.onClick.AddListener(OnCloseWindowClickCallback);
			BackToMainButton.onClick.AddListener(OnBackToMainMenuClickCallback);
			PickATrackButton.onClick.AddListener(OnPickATrackClickCallback);
			RestartButton.onClick.AddListener(OnRestartLevelClickCallback);
			SettingsButton.onClick.AddListener(OnOpenSettingsClickCallback);
			FlagsStorage.IsInputEnabledInThisWindow = false;
			base.OnEnable();
			InitBackButton();
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			OnCloseWindowClickCallback();
		}

		private void OnDisable()
		{
			CloseButton.onClick.RemoveListener(OnCloseWindowClickCallback);
			BackToMainButton.onClick.RemoveListener(OnBackToMainMenuClickCallback);
			PickATrackButton.onClick.RemoveListener(OnPickATrackClickCallback);
			RestartButton.onClick.RemoveListener(OnRestartLevelClickCallback);
			SettingsButton.onClick.RemoveListener(OnOpenSettingsClickCallback);
		}

		protected override void OnDestroy()
		{
			_circuitRaceControlller = null;
			_circuitRacingUI = null;
			_managerUI = null;
			base.OnDestroy();
		}

		private void OnCloseWindowClickCallback()
		{
			_circuitRacingUI.ShowCircuitPlayWindow(state: true);
			_circuitRacingUI.UnPause();
			Close();
			FlagsStorage.IsInputEnabledInThisWindow = true;
		}

		private void OnBackToMainMenuClickCallback()
		{
			_circuitRacingUI.UnPause();
			_circuitRaceControlller.GoMainMenu();
			Close();
		}

		private void OnPickATrackClickCallback()
		{
			_managerUI.ShowCircuitMapWindow(cleanScreen: false, WindowScriptType.CircuitRacePauseWindow.ToString());
			base.Close();
		}

		private void OnRestartLevelClickCallback()
		{
			SoundEventsDataHelper.SetGroupState(SoundEventsDataHelper.StateReverPause, SoundEventsDataHelper.OnReverState);
			_circuitRacingUI.ShowCircuitPlayWindow(state: true, resetRacingState: true);
			_circuitRaceControlller.RestartRace();
			Close();
		}

		private void OnOpenSettingsClickCallback()
		{
			_managerUI.ShowWindow("SettingsMenuWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false).Forget();
			base.Close();
		}

		public override void Close()
		{
			if (!base.IsCloseCanceled)
			{
				_circuitRacingUI.UnPause();
				base.Close();
			}
		}
	}
}
