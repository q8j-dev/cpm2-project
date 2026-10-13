using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class LevelPauseWindow : WindowScript, IBackableWindow
	{
		[SerializeField]
		private Button CloseButton = default;

		[SerializeField]
		private Button BackToMainButton = default;

		[SerializeField]
		private Button LevelsButton = default;

		[SerializeField]
		private Button RestartButton = default;

		[SerializeField]
		private Button SettingsButton = default;

		private LevelScript _levelScript;

		private MenuLevelParent _menuLevelParent;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

		[Inject]
		public void Construct(LevelScript levelScript, MenuLevelParent menuLevelParent)
		{
			_menuLevelParent = menuLevelParent;
			_levelScript = levelScript;
		}

		protected override void Start()
		{
			InitBackButton();
			base.Start();
			CloseButton.onClick.AddListener(CloseWindow);
			BackToMainButton.onClick.AddListener(BackToMainMenu);
			LevelsButton.onClick.AddListener(OpenLevelsList);
			RestartButton.onClick.AddListener(RestartLevel);
			SettingsButton.onClick.AddListener(OpenSettings);
		}

		public void Back()
		{
			CloseWindow();
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}

		private void CloseWindow()
		{
			_levelScript.Resume();
			Close();
		}

		private void BackToMainMenu()
		{
			_menuLevelParent.OpenMainMenu();
		}

		private void OpenLevelsList()
		{
			_menuLevelParent.OpenOtherLevelsMenu();
			Close();
		}

		private void RestartLevel()
		{
			_menuLevelParent.StartLevelReplay();
			Close();
		}

		private void OpenSettings()
		{
			_managerUI.ShowSettingsWindow();
			Close();
		}
	}
}
