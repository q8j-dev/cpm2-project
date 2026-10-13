using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using RacingScripts.RaceControllers;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Trade;
using _Car_Parking.Scripts.UIElement.Settings;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class SettingsWindow : WindowScript, IBackableWindow
	{
		[Header("Elements to hide")]
		[SerializeField]
		private CanvasGroup[] MainMenuElements;

		[SerializeField]
		private CanvasGroup[] FreeSceneElements;

		[SerializeField]
		private ScrollRect MainScrollRect;

		[Header("Sections")]
		[SerializeField]
		private SettingsSectionBase[] Sections;

		[Header("Top")]
		[SerializeField]
		private Button BackButton;

		[SerializeField]
		private GameObject TopElements;

		[Header("Bottom buttons")]
		[SerializeField]
		private Button BackToMenuButton;

		[SerializeField]
		private Button ExitGameButton;

		[Header("Toggles")]
		[SerializeField]
		private Toggle AccountToggle;

		[SerializeField]
		private Toggle ControlsToggle;

		[Header("Sections Order")]
		[SerializeField]
		private Transform[] DefaultOrder;

		[SerializeField]
		private Transform[] MainOrder;

		private const string ExitGameConfirmString = "SteamSettings.ExitGameConfirm";

		private const string LastSectionKey = "SettingsWindow.LastSection";

		private const string MainScrollKey = "SettingsWindow.MainScroll";

		private bool _isMainScrollRestored;

		private string _requestedSection;

		private float _screenWidth;

		private float _screenHeight;

		private int _qualityId;

		private int _controlType;

		private bool _alreadyPressedMenu;

		private SettingsSectionBase _currentSection;

		private MenuLevelParent _menuLevelParent;

		private LevelScript _levelScript;

		private IDisposable _disposableSetShadersAfter;

		private DragRacingUI _dragRacingUI;

		private IRaceController _raceController;

		private ParkTronicControll _parkTronicControl;

		private OpenWorldAnalytic _openWorldAnalytic;

		private ITradeService _tradeService;

		private ExitToMainMenuService _exitToMainMenu;

		private RaceModeController _raceModeController;

		private OptionalDependency<MenuLevelParent> _menuLevelParentDep;

		private OptionalDependency<LevelScript> _levelScriptDep;

		private OptionalDependency<DragRacingUI> _dragRacingUIDep;

		private OptionalDependency<ParkTronicControll> _parkTronicControlDep;

		private OptionalDependency<RaceModeController> _raceModeControllerDep;

		[field: SerializeField]
		public CanvasGroup AllElements { get; private set; }

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

		[Inject]
		public void Construct(ExitToMainMenuService exitToMainMenu, OptionalDependency<MenuLevelParent> menuLevelParent, OptionalDependency<LevelScript> levelScript, OptionalDependency<DragRacingUI> dragRacingUI, OptionalDependency<ParkTronicControll> parkTronicControl, OptionalDependency<RaceModeController> raceModeController)
		{
			_parkTronicControlDep = parkTronicControl;
			_dragRacingUIDep = dragRacingUI;
			_menuLevelParentDep = menuLevelParent;
			_levelScriptDep = levelScript;
			_exitToMainMenu = exitToMainMenu;
			_raceModeControllerDep = raceModeController;
		}

		protected override void Awake()
		{
			base.Awake();
			_parkTronicControl = _parkTronicControlDep?.Value;
			_dragRacingUI = _dragRacingUIDep?.Value;
			_menuLevelParent = _menuLevelParentDep?.Value;
			_levelScript = _levelScriptDep?.Value;
			_raceModeController = _raceModeControllerDep.Value;
		}

		protected override void OnEnable()
		{
			BackButton.gameObject.SetActive(value: false);
			base.OnEnable();
		}

		protected override void Start()
		{
			if ((bool)_raceModeController)
			{
				_raceController = _raceModeController.SpawnedObject.GetComponent<IRaceController>();
			}
			else
			{
				_raceController = null;
			}
			InitBackButton();
			base.Start();
			_managerUI.IsSettingsWindowActive = true;
			_managerUI.IncreaseTimerButtonShow?.Invoke(obj: false);
			GlobalEventSystem.OnGamePaused?.Invoke(obj: true);
			if ((bool)MainScrollRect)
			{
				MainScrollRect.onValueChanged.AddListener(OnMainScrollChanged);
			}
			string sectionName = PlayerPrefs.GetString("SettingsWindow.LastSection", string.Empty);
			SettingsSectionBase[] sections = Sections;
			for (int i = 0; i < sections.Length; i++)
			{
				sections[i].Init();
			}
			if (ScenesHelper.IsMenuSceneActive())
			{
				BackToMenuButton.SetActive(active: false);
				ExitGameButton.onClick.AddListener(ExitGame);
				ExitGameButton.SetActive(active: true);
				SetSectionsOrder(MainOrder);
				if (FlagsStorage.IsPhotoMode)
				{
					BackButton.onClick.AddListener(Resume);
				}
				else
				{
					TopElements.SetActive(value: false);
				}
			}
			else
			{
				AccountToggle.SetActive(active: false);
				TopElements.SetActive(value: true);
				BackToMenuButton.SetActive(!ScenesHelper.IsParkingLevelActive());
				ExitGameButton.SetActive(active: false);
				BackButton.onClick.AddListener(Resume);
				BackToMenuButton.onClick.AddListener(OpenMainMenu);
				SetSectionsOrder(DefaultOrder);
			}
			RestoreLastSection(sectionName);
			RestoreMainScrollPosition();
			if ((bool)_parkTronicControl)
			{
				_parkTronicControl.ResetValues();
			}
		}

		protected override void OnDestroy()
		{
			if ((bool)_currentSection)
			{
				_currentSection.SaveScrollPosition();
			}
			base.OnDestroy();
			_managerUI.IsSettingsWindowActive = false;
			_managerUI.RestoreButtonPressed = false;
			_Car_Parking.Scripts.Audio.Audio.SetPauseSoundFilter(on: false);
			PlayerPrefs.Save();
		}

		private void ExitGame()
		{
			Hide();
			_managerUI.ShowConfirmWindow(() =>
			{
				Application.Quit();
			}, new LocalizedString("SteamSettings.ExitGameConfirm"), "", () =>
			{
				_managerUI.CloseWindow("ConfirmWindow");
				Show();
			}).Forget();
		}

		public void SetSection(SettingsSectionBase settingsSection)
		{
			_currentSection = settingsSection;
			_currentSection.Restore();
			UiInputSetup.AutoSelectFirstInteractable(_currentSection.gameObject).Forget();
			PlayerPrefs.SetString("SettingsWindow.LastSection", settingsSection.name);
		}

		private void OnMainScrollChanged(Vector2 position)
		{
			if (_isMainScrollRestored)
			{
				ScrollPositionMemory.Save(MainScrollRect, "SettingsWindow.MainScroll");
			}
		}

		private void RestoreMainScrollPosition()
		{
			if ((bool)MainScrollRect)
			{
				_isMainScrollRestored = false;
				RestoreMainScrollPositionAsync(this.GetCancellationTokenOnDestroy()).Forget();
			}
		}

		private async UniTaskVoid RestoreMainScrollPositionAsync(CancellationToken token)
		{
			await ScrollPositionMemory.RestoreAsync(MainScrollRect, "SettingsWindow.MainScroll", token);
			_isMainScrollRestored = true;
		}

		public override void SetAdditionalData(string data)
		{
			base.SetAdditionalData(data);
			if (!string.IsNullOrEmpty(data))
			{
				SettingsSectionBase settingsSectionBase = FindSection(data);
				if ((bool)settingsSectionBase)
				{
					_requestedSection = data;
					SelectOnly(settingsSectionBase);
				}
			}
		}

		private void RestoreLastSection(string sectionName)
		{
			if (string.IsNullOrEmpty(_requestedSection) && !string.IsNullOrEmpty(sectionName))
			{
				SettingsSectionBase settingsSectionBase = FindSection(sectionName);
				if ((bool)settingsSectionBase && settingsSectionBase.IsAvailable && !(settingsSectionBase == _currentSection))
				{
					SelectOnly(settingsSectionBase);
				}
			}
		}

		private SettingsSectionBase FindSection(string sectionName)
		{
			SettingsSectionBase[] sections = Sections;
			foreach (SettingsSectionBase settingsSectionBase in sections)
			{
				if ((bool)settingsSectionBase && settingsSectionBase.name == sectionName)
				{
					return settingsSectionBase;
				}
			}
			return null;
		}

		private void SelectOnly(SettingsSectionBase target)
		{
			SettingsSectionBase[] sections = Sections;
			foreach (SettingsSectionBase settingsSectionBase in sections)
			{
				if ((bool)settingsSectionBase && settingsSectionBase != target)
				{
					settingsSectionBase.SelectSectionManually(isOn: false);
				}
			}
			target.SelectSectionManually(isOn: true);
		}

		public void Show()
		{
			AllElements.alpha = 1f;
			AllElements.interactable = true;
		}

		public void Hide()
		{
			AllElements.alpha = 0f;
			AllElements.interactable = false;
		}

		private void Save()
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("SettingsWindow.ChangesSaved"), 2f, Color.white);
		}

		private void ResetChanges()
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("SettingsWindow.ChangesResetToDefault"), 2f, Color.white);
		}

		private void OpenMainMenu()
		{
			if (ScenesHelper.IsFreeDriveSceneActive() || ScenesHelper.IsParkingLevelActive())
			{
				Hide();
				_managerUI.ShowConfirmWindow(() =>
				{
					_exitToMainMenu.Exit(_menuLevelParent, "quit_manual");
				}, new LocalizedString("SettingsWindow.DoYouWantToReturnToMainMenu"), "", () =>
				{
					_managerUI.CloseWindow("ConfirmWindow");
					Show();
				}).Forget();
				return;
			}
			if ((bool)_dragRacingUI)
			{
				_dragRacingUI.OpenMainMenuFromDragRacing();
			}
			if ((bool)_raceModeController)
			{
				_raceModeController.ShowMainMenu();
			}
			Close();
		}

		private void HideUnusedElements()
		{
			CanvasGroup[] mainMenuElements = MainMenuElements;
			foreach (CanvasGroup obj in mainMenuElements)
			{
				obj.alpha = (ScenesHelper.IsMenuSceneActive() ? 1f : 0.5f);
				obj.blocksRaycasts = ScenesHelper.IsMenuSceneActive();
			}
			mainMenuElements = FreeSceneElements;
			foreach (CanvasGroup obj2 in mainMenuElements)
			{
				obj2.alpha = (ScenesHelper.IsMenuSceneActive() ? 0.5f : 1f);
				obj2.blocksRaycasts = !ScenesHelper.IsMenuSceneActive();
			}
		}

		private void Resume()
		{
			if (!FlagsStorage.IsMainMenu || FlagsStorage.IsPhotoMode)
			{
				SoundEventsDataHelper.SetGroupState(SoundEventsDataHelper.StateReverPause, SoundEventsDataHelper.OnReverState);
				BackToGame();
				GlobalEventSystem.OnGamePaused?.Invoke(obj: false);
			}
		}

		private void BackToGame()
		{
			AllElements.alpha = 1f;
			if (FlagsStorage.IsPhotoMode)
			{
				_managerUI.ShowPhotoModePauseWindow();
			}
			else if (ScenesHelper.IsFreeDriveSceneActive() || ScenesHelper.IsParkingLevelActive())
			{
				_levelScript.Resume(ignoreBrakes: true);
			}
			else
			{
				if ((bool)_dragRacingUI)
				{
					_dragRacingUI.HideCurrentMenu(isActive: true);
				}
				if (_raceController != null)
				{
					_raceController.Resume();
				}
			}
			Close();
		}

		private void SetSectionsOrder(Transform[] order)
		{
			for (int i = 0; i < order.Length; i++)
			{
				if (order[i].gameObject.name == "Patch Notes")
				{
					order[i].gameObject.SetActive(value: false);
				}
				order[i].SetSiblingIndex(i);
			}
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			if (FlagsStorage.IsMainMenu)
			{
				BackToMain();
			}
			else
			{
				Resume();
			}
		}

		private void BackToMain()
		{
			_managerUI.ShowMainMenuWindow();
			Close();
		}
	}
}
