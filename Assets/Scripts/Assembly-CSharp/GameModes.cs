using System.Collections.Generic;
using CPM.Database;
using CPMEngine;
using CodeStage.AntiCheat.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Car.CarEligibilityService;
using _Car_Parking.Scripts.Database.PlayerData;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Racing_Scripts.RaceInfo;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.VinylsAi;

public class GameModes : WindowScript, IBackableWindow
{
	private const int NewModeTextShowMaxCount = 3;

	[SerializeField]
	private Button BackButton;

	[SerializeField]
	[Header("Levels")]
	private Button LevelsButton;

	[SerializeField]
	private TextMeshProUGUI LevelsProgressText;

	[SerializeField]
	private Image LevelsFilledImage;

	[SerializeField]
	[Header("Drag Racing")]
	private Button RacingButton;

	[SerializeField]
	private GameObject NewModeText;

	[SerializeField]
	private TextMeshProUGUI DragProgressText;

	[SerializeField]
	private Image DragFilledImage;

	[SerializeField]
	private List<RaceWonPlacesItem> DragClassRecordsUis;

	[Header("Single Player")]
	[SerializeField]
	private Button SinglePlayerButton;

	[SerializeField]
	private TextMeshProUGUI DistanceText;

	[Header("Multiplayer")]
	[SerializeField]
	private Button MultiplayerButton;

	[SerializeField]
	private TextMeshProUGUI PlayTimeText;

	private ITimerTuningService _timerTuningService;

	private PlayerVinylCensorChecker _playerVinylCensorChecker;

	private GlobalCollectionsProvider _collectionsProvider;

	private OnlineCollectionsProvider _onlineCollectionsProvider;

	private ICarEligibilityService _carEligibilityService;

	private LevelsConfig _levelsConfig;

	private AllCarsInfo _allCarsInfo;

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

	[Inject]
	public void Construct(ITimerTuningService timerTuningService, ICarTuningFactory carTuningFactory, PlayerVinylCensorChecker playerVinylCensorChecker, GlobalCollectionsProvider collectionsProvider, OnlineCollectionsProvider onlineCollectionsProvider, ICarEligibilityService carEligibilityService, LevelsConfig levelsConfig, AllCarsInfo allCarsInfo)
	{
		_onlineCollectionsProvider = onlineCollectionsProvider;
		_collectionsProvider = collectionsProvider;
		_timerTuningService = timerTuningService;
		_playerVinylCensorChecker = playerVinylCensorChecker;
		_carEligibilityService = carEligibilityService;
		_levelsConfig = levelsConfig;
		_allCarsInfo = allCarsInfo;
	}

	protected override void OnEnable()
	{
		_collectionsProvider.ResetAllCollections(resetRenderTex: false);
		_onlineCollectionsProvider.ResetAllCollections();
		CommonExtensions.ClearGOCollection(Skidmarks.allSkidmarks);
		CalculateProgress();
		CalculateMultiFreePlayTime();
		CalculateSingleFreeDistance();
		base.OnEnable();
		BackButton.gameObject.SetActive(value: false);
	}

	protected override void Start()
	{
		InitBackButton();
		base.Start();
		_managerUI.CloseWindow("UserBalanceWindow");
		LevelsButton.onClick.AddListener(OpenLevels);
		SinglePlayerButton.onClick.AddListener(RunSinglePlayer);
		MultiplayerButton.onClick.AddListener(RunMultiplayer);
		BackButton.onClick.AddListener(Back);
		RacingButton.onClick.AddListener(ShowRacingWindow);
		Audio.SetPauseSoundFilter(on: true);
		int num = PlayerPrefs.GetInt("NewRaceStarted");
		NewModeText.SetActive(num < 3);
	}

	protected override void OnDestroy()
	{
		LevelsButton.onClick.RemoveListener(OpenLevels);
		RacingButton.onClick.RemoveListener(ShowRacingWindow);
		SinglePlayerButton.onClick.RemoveListener(RunSinglePlayer);
		MultiplayerButton.onClick.RemoveListener(RunMultiplayer);
	}

	private void ShowRacingWindow()
	{
		if (!V.CoreGameManager.HasFirstCar())
		{
			V.CoreGameManager.SetMessage(new LocalizedString("Mainmenuwindow.YouNeedCar"), 3f);
			return;
		}
		if (_timerTuningService.IsCarBlocked(V.SlotID))
		{
			_managerUI.ShowCarBlockNotification(NotificationMessages.CarInService, 3f, NotificationType.Error);
			return;
		}
		int num = PlayerPrefs.GetInt("NewRaceStarted");
		if (num < 3)
		{
			PlayerPrefs.SetInt("NewRaceStarted", num + 1);
		}
		_managerUI.ShowSelectRacigModeWindow();
	}

	private void OpenLevels()
	{
		_managerUI.ShowParkingLevelsWindow();
		Close();
	}

	private void CalculateProgress()
	{
		HashSet<int> hashSet = new HashSet<int>();
		LevelCategory[] levelCategories = _levelsConfig.LevelCategories;
		for (int i = 0; i < levelCategories.Length; i++)
		{
			if (levelCategories[i].isActive)
			{
				hashSet.Add(i);
			}
		}
		int num = 0;
		int num2 = 0;
		LevelDataScript[] levelsDatas = _levelsConfig.LevelsDatas;
		foreach (LevelDataScript levelDataScript in levelsDatas)
		{
			if (hashSet.Contains(levelDataScript.CategoryIndex) && !IsLevelCarBanned(levelDataScript))
			{
				num++;
				if (ObscuredPrefs.HasKey($"bestResultID{levelDataScript.ID}"))
				{
					num2++;
				}
			}
		}
		if (LevelsProgressText != null)
		{
			LevelsProgressText.text = $"{num2}/{num}";
		}
		if ((bool)LevelsFilledImage)
		{
			float fillAmount = ((num > 0) ? Utils.RoundTo((float)num2 / (float)num, 2) : 0f);
			LevelsFilledImage.fillAmount = fillAmount;
		}
	}

	private bool IsLevelCarBanned(LevelDataScript level)
	{
		if (_allCarsInfo == null)
		{
			return false;
		}
		if (level.CarId > 0 && _allCarsInfo.IsBannedCar(level.CarId))
		{
			return true;
		}
		if (level.RacingCarId > 0)
		{
			return _allCarsInfo.IsBannedCar(level.RacingCarId);
		}
		return false;
	}

	private void RunSinglePlayer()
	{
		if (_carEligibilityService.CanUseCar())
		{
			_managerUI.ShowSelectMapWindow();
			Close();
		}
	}

	private void RunMultiplayer()
	{
		if (_carEligibilityService.CanUseCar())
		{
			_playerVinylCensorChecker.CheckCurrentCar();
			_managerUI.ShowLobbyWindow();
			Close();
		}
	}

	private void CalculateMultiFreePlayTime()
	{
		int num = AllTimeGamePlayUtil.Get();
		PlayTimeText.text = ((num < 60) ? (num + "m") : (num / 60 + "h"));
	}

	private void CalculateSingleFreeDistance()
	{
		int num = PlayerTotalDistance.Get();
		DistanceText.text = num + "km";
	}

	public void InitBackButton()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		_managerUI.SetCurrentWindow(this);
	}

	public void Back()
	{
		Close();
		_managerUI.ShowUserBalanceWindow();
		_managerUI.ShowMainMenuWindow();
	}
}
