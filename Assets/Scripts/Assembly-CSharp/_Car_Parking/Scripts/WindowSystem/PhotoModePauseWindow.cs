using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class PhotoModePauseWindow : WindowScript, IBackableWindow
	{
		[SerializeField]
		private Button CloseButton;

		[SerializeField]
		private Button ExitButton;

		[SerializeField]
		private Button ResetSettingsButton;

		[SerializeField]
		private Button SettingsButton;

		private LevelScript _levelScript;

		private OptionalDependency<LevelScript> _levelScriptDep;

		private CircuitRacingUIControl _circuitRacingUI;

		private OptionalDependency<CircuitRacingUIControl> _circuitRacingUIDep;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

		[Inject]
		public void Construct(OptionalDependency<LevelScript> levelScript, OptionalDependency<CircuitRacingUIControl> circuitRacingUI)
		{
			_levelScriptDep = levelScript;
			_circuitRacingUIDep = circuitRacingUI;
		}

		protected override void Start()
		{
			base.Start();
			InitBackButton();
			_levelScript = _levelScriptDep?.Value;
			_circuitRacingUI = _circuitRacingUIDep?.Value;
			CloseButton.onClick.AddListener(CloseWindow);
			ExitButton.onClick.AddListener(ExitPhotoMode);
			ResetSettingsButton.onClick.AddListener(ResetSettings);
			SettingsButton.onClick.AddListener(OpenSettings);
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			CloseWindow();
		}

		private void CloseWindow()
		{
			Close();
			if ((bool)_managerUI && (bool)_managerUI.PhotoModeWindow)
			{
				_managerUI.PhotoModeWindow.ShowPhotoModeControlPanel(show: true);
			}
		}

		private void ExitPhotoMode()
		{
			_managerUI.PhotoModeWindow.Exit();
			_managerUI.PhotoModeWindow.Close();
			Time.timeScale = 1f;
			SoundEventsDataHelper.SetGroupState(SoundEventsDataHelper.StateReverPause, SoundEventsDataHelper.OnReverState);
			if (ScenesHelper.IsMenuSceneActive())
			{
				_managerUI.ShowUserBalanceWindow();
				_managerUI.ChangeBalancePanelActiveState?.Invoke(obj: true);
				_managerUI.ShowMenuGarageControllerHUD("PhoneWindow");
			}
			if ((bool)_circuitRacingUI)
			{
				_levelScript.UnCleanScreen();
				_circuitRacingUI.ShowCircuitPlayWindow(state: true);
			}
			else if ((bool)_levelScript)
			{
				_levelScript.Resume(ignoreBrakes: true);
			}
			FlagsStorage.IsInputEnabledInThisWindow = true;
			Close();
		}

		private void ResetSettings()
		{
			if ((bool)_managerUI && (bool)_managerUI.PhotoModeWindow)
			{
				_managerUI.PhotoModeWindow.ResetAll();
			}
		}

		private void OpenSettings()
		{
			_managerUI.ShowSettingsWindow(null, cleanScreen: false);
			Close();
		}
	}
}
