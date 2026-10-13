using Cysharp.Threading.Tasks;
using RacingScripts.RaceControllers;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.Rally
{
	public class RallyMenuWindow : WindowScript, IBackableWindow
	{
		[SerializeField]
		private Button StartButton = default;

		[SerializeField]
		private Button TyreSelectionButton = default;

		[SerializeField]
		private Button BackButton = default;

		[SerializeField]
		private Button LeaderboardButton = default;

		[SerializeField]
		private Image LocationIcon = default;

		[SerializeField]
		private Image TrajectoriesIcon = default;

		[SerializeField]
		private TextMeshProUGUI TrajectotyText = default;

		[SerializeField]
		private RallyRecordsUIPanel RallyRecordsUIPanel = default;

		private RallyMapDataSO _rallyMapDataSo;

		private RallyController _rallyController;

		private RaceModeController _raceModeController;

		private RacingCameraController _racingCameraController;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Garage;

		protected override bool ChangeRootGameInputGroup => true;

		[Inject]
		[UnityEngine.Scripting.Preserve]
		public void Construct(ManagerUI managerUI, RaceModeController raceModeController, RacingCameraController racingCameraController)
		{
			_managerUI = managerUI;
			_raceModeController = raceModeController;
			_racingCameraController = racingCameraController;
		}

		protected override void Start()
		{
			_racingCameraController.TrySwitchVisibleHeadPerson(isVisible: true);
			_rallyController = _raceModeController.SpawnedObject.GetComponent<RallyController>();
			_rallyController.SetCarLights();
			RallyRecordsUIPanel.SetTargetTimes(_rallyController);
			base.Start();
			StartButton.onClick.AddListener(StartRace);
			TyreSelectionButton.onClick.AddListener(ShowSelectTyreWindow);
			BackButton.onClick.AddListener(Back);
			if ((bool)LeaderboardButton)
			{
				LeaderboardButton.onClick.AddListener(OpenLeaderboard);
			}
			_rallyMapDataSo = _rallyController.GetMapData();
			SetImages();
			InitBackButton();
		}

		private void SetTimes()
		{
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			BackToMain();
		}

		private void StartRace()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.ClickBackSwitch, base.gameObject);
			_rallyController.StartStage(force: true).Forget();
			Close();
		}

		private void SetImages()
		{
			LocationIcon.sprite = _rallyMapDataSo.RallyLocationDatas[RallyController.LocationIndex].LocationLogo;
			TrajectoriesIcon.sprite = _rallyMapDataSo.RallyLocationDatas[RallyController.LocationIndex].TrajectoryData.RallySequenceDataList[RallyController.TrajectoryIndex].TrajectoryLogo;
			TrajectotyText.text = new LocalizedString(_rallyMapDataSo.RallyLocationDatas[RallyController.LocationIndex].NameKey);
		}

		private void ShowSelectTyreWindow()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.ClickBackSwitch, base.gameObject);
			_managerUI.ShowWindow("RacingCarChangeTyreWindow").Forget();
			Close();
		}

		private void BackToMain()
		{
			_rallyController.OpenMapWindow();
			Close();
		}

		private void OpenLeaderboard()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.ClickBackSwitch, base.gameObject);
			_managerUI.ShowWindow("RallyLeaderboardWindow").Forget();
			Close();
		}
	}
}
