using System;
using System.Collections.Generic;
using System.Threading;
using Arikan;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.DataStorageService.PlayerData;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Racing_Scripts;

public class CircuitRacingUIControl : MonoBehaviour
{
	private const string UISwitch = "UiSwitch";

	private const string UIEvent = "Ui_Event";

	private const string LapTimeCountState = "Lap_Time_Count";

	public ManagerUI _managerUI;

	private int prevMenu;

	public GameObject loadingWindow;

	public GameObject allMenu;

	public GameObject WrongWayPanel;

	public TextMeshProUGUI FinishResultText;

	public TextMeshProUGUI FinishPlaceResultText;

	public Color[] RewardColors;

	public Texture2D WinBorderSprite;

	public Texture2D DefaultBorderSprite;

	public Sprite MoneySprite;

	public Sprite CoinsSprite;

	public MiniMapBounds MapBounds;

	private LapRaceTrafficLightUI _lapRaceTrafficLightUI;

	[SerializeField]
	private CircuitRealtimeChartlist CircuitRealtimeChartlistPrefab;

	[SerializeField]
	private QualificationResultPanel QualificationResultPanel;

	[SerializeField]
	private CircuitRaceControlller CircuitRaceControlller;

	private CircuitRacingConfig _circuitRacingConfig;

	private IPrefabFactory _prefabFactory;

	[SerializeField]
	private GameObject GarageMenuWindow;

	private IPlayerStorageService _playerStorageService;

	[SerializeField]
	private TextMeshProUGUI RaceResultText;

	[SerializeField]
	private Sprite[] MaintenanceIcons;

	private CircuitPlayWindow _circuitPlayWindow;

	private CircuitRealtimeChartlist _circuitRealtimeChartlist;

	private float _timeLeft;

	private IPlayerStatusSystem _playerStatusSystem;

	private LevelScript _levelScript;

	private RacingCameraController _racingCameraController;

	[SerializeField]
	[Header("RaceClassUI")]
	private GameObject CircuitClassWindow;

	[SerializeField]
	private Transform CircuitRaceClassUiElementsParent;

	private LapRaceLeaderboardUIPanel _lapRaceLeaderboardUIPanel;

	[SerializeField]
	[Header("RaceMapSelectUI")]
	private GameObject RaceMapSelectWindow;

	[SerializeField]
	private CircuitRaceMapElement _circuitRaceMapElementPrefab;

	[SerializeField]
	private Transform _circuitRaceMapElementsParent;

	private List<CircuitRaceMapElement> _circuitRaceMapElements;

	private MenuLevelParent _menuLevelParent;

	public CircuitPlayWindow CircuitPlayWindow => _circuitPlayWindow;

	[Inject]
	public void Construct(ManagerUI managerUI, CircuitRacingConfig circuitRacingConfig, IPrefabFactory prefabFactory, IPlayerStorageService playerStorageService, MenuLevelParent menuLevelParent, IPlayerStatusSystem playerStatusSystem, LapRaceTrafficLightUI lapRaceTrafficLightUI, RacingCameraController racingCameraController)
	{
		_menuLevelParent = menuLevelParent;
		_managerUI = managerUI;
		_circuitRacingConfig = circuitRacingConfig;
		_prefabFactory = prefabFactory;
		_playerStorageService = playerStorageService;
		_playerStatusSystem = playerStatusSystem;
		_lapRaceTrafficLightUI = lapRaceTrafficLightUI;
		_racingCameraController = racingCameraController;
	}

	private void Start()
	{
		CreateRealtimeChartlist();
		_playerStatusSystem.OnGameModePlayed();
	}

	private void CreateRealtimeChartlist()
	{
		_circuitRealtimeChartlist = _prefabFactory.Instantiate(CircuitRealtimeChartlistPrefab, _managerUI.GetWindowsParent());
		_circuitRealtimeChartlist.Init(CircuitRaceControlller);
		ShowChartList(state: false);
	}

	public void SetCircuitPlayWindow(CircuitPlayWindow circuitPlayWindow)
	{
		_circuitPlayWindow = circuitPlayWindow;
	}

	public LapRaceLeaderboardUIPanel GetLapRaceLeaderboardUI()
	{
		return _lapRaceLeaderboardUIPanel;
	}

	public void SetProgress(string lapProgress, string lapsCount, string placeProgress, string placesCount)
	{
		if ((bool)_circuitPlayWindow)
		{
			_circuitRealtimeChartlist.SetLap(lapProgress, lapsCount);
			_circuitRealtimeChartlist.SetProgress(placeProgress, placesCount);
		}
	}

	public void ShowCircuitPlayWindow(bool state, bool resetRacingState = false)
	{
		if (state)
		{
			SetCameraTouchEnabled(state: true);
			if ((bool)_circuitPlayWindow)
			{
				_circuitPlayWindow.gameObject.SetActive(value: true);
				_circuitPlayWindow.Resume(resetRacingState);
				FlagsStorage.IsInputEnabledInThisWindow = true;
			}
			else
			{
				_managerUI.ShowCircuitPlayWindow();
			}
			ShowChartList(CircuitRaceControlller.IsUserCrossStartLine);
		}
	}

	public void ShowTimerPanel()
	{
		_circuitPlayWindow.ShowTimerPanel();
	}

	public void SetLap(string lap, string lapsCount)
	{
		_circuitRealtimeChartlist.SetLap(lap, lapsCount);
	}

	public void SetPlaceProgress(string placeProgress, string placesCount)
	{
		_circuitRealtimeChartlist.SetProgress(placeProgress, placesCount);
	}

	public bool IsLeaderBoardPanelActive()
	{
		if (!_lapRaceLeaderboardUIPanel)
		{
			return false;
		}
		return _lapRaceLeaderboardUIPanel.gameObject.activeSelf;
	}

	public void HideLapRaceTrafficLight()
	{
		_lapRaceTrafficLightUI.SetActive(active: false);
	}

	public void ShowRaceHudElemetns(bool state)
	{
		if (state)
		{
			_lapRaceTrafficLightUI.Reset();
			_lapRaceTrafficLightUI.SetActive(active: false);
		}
		if ((bool)_circuitPlayWindow)
		{
			_circuitRealtimeChartlist.SetLap("", "");
			_circuitRealtimeChartlist.SetProgress("", "");
		}
	}

	public void ResetLeaderBoard()
	{
		_lapRaceLeaderboardUIPanel.Reset();
	}

	public async UniTask SmoothListerVolumeAsync(CancellationToken ct = default(CancellationToken))
	{
		AudioListener.volume = 0f;
		while (AudioListener.volume <= 1f)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(0.05000000074505806), DelayType.Realtime, PlayerLoopTiming.Update, ct);
			AudioListener.volume += 0.01f;
			if (AudioListener.volume >= 1f)
			{
				AudioListener.volume = 1f;
				break;
			}
		}
	}

	private async UniTask ShowLoadingAsync(CancellationToken ct = default(CancellationToken))
	{
		if (!loadingWindow.activeSelf)
		{
			CanvasGroup canvasGroup = loadingWindow.GetComponent<CanvasGroup>();
			loadingWindow.SetActive(value: true);
			canvasGroup.DOFade(0.8f, 1f);
			allMenu.SetActive(value: false);
			await UniTask.Delay(TimeSpan.FromSeconds(0.800000011920929), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			canvasGroup.DOFade(0f, 1f);
			await UniTask.Delay(TimeSpan.FromSeconds(0.20000000298023224), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			loadingWindow.SetActive(value: false);
			allMenu.SetActive(value: true);
		}
	}

	public void SetUIForGarage()
	{
		_managerUI.ShowCircuitRaceMenuWindow();
	}

	public void SetWrongWay(bool state)
	{
		if ((bool)_circuitPlayWindow)
		{
			_circuitPlayWindow.ShowWrongWayPanel(state);
		}
	}

	public void SetOutOfRoad(bool state)
	{
		if ((bool)_circuitPlayWindow)
		{
			if (CircuitRaceControlller.IsCurrentMapLeman())
			{
				float penaltyDistance = CircuitRaceControlller.circuitCarsControl.GetUserLapRaceCar().GetPenaltyDistance();
				string format = new LocalizedString("CircuitPlayWindow.PenaltyDistance");
				_circuitPlayWindow.ShowOutOfRoadPanel(state, string.Format(format, penaltyDistance));
			}
			else
			{
				float penaltyTime = CircuitRaceControlller.circuitCarsControl.GetUserLapRaceCar().GetPenaltyTime();
				string format2 = new LocalizedString("CircuitPlayWindow.PenaltyTime");
				_circuitPlayWindow.ShowOutOfRoadPanel(state, string.Format(format2, penaltyTime));
			}
		}
	}

	public void SetCrashPanel(bool state)
	{
		if ((bool)_circuitPlayWindow)
		{
			_circuitPlayWindow.ShowCrashPanel(state, CircuitRaceControlller.circuitCarsControl.GetUserLapRaceCar().GetPenaltyTime());
		}
	}

	public void ShowSettings()
	{
		_managerUI.ShowSettingsWindow();
	}

	private void Update()
	{
		if (CircuitRaceControlller.GetRaceState() == RaceState.RaceStarted && (bool)_circuitPlayWindow)
		{
			_timeLeft += Time.deltaTime;
			_circuitPlayWindow.SetCurrentTime(_timeLeft);
		}
	}

	public void OnQualificationFinish(LapRaceCar raceModel)
	{
		_managerUI.CloseCircuitPlayWindow();
		_managerUI.ShowQualificationWindow();
	}

	public async UniTask OnRaceFinish(List<LapRaceCar> sortedLapRaceCars, bool isLeman = false)
	{
		_lapRaceLeaderboardUIPanel = await _managerUI.CreateWindowGeneric<LapRaceLeaderboardUIPanel>("CircuitRaceResultWindow", _managerUI.GetWindowsParent());
		_lapRaceLeaderboardUIPanel.Init(_prefabFactory, CircuitRaceControlller, _managerUI);
		_managerUI.CloseCircuitPlayWindow();
		if ((bool)_lapRaceLeaderboardUIPanel)
		{
			if (!isLeman)
			{
				_lapRaceLeaderboardUIPanel.FinishInit(sortedLapRaceCars).Forget();
			}
			else
			{
				_lapRaceLeaderboardUIPanel.FinishInitLeman(sortedLapRaceCars);
			}
		}
	}

	public void OnOneRaceCarFinishRace(LapRaceCar lapRaceCar)
	{
		if ((bool)_lapRaceLeaderboardUIPanel)
		{
			_lapRaceLeaderboardUIPanel.FinishInit(lapRaceCar);
		}
	}

	public void SortUsers()
	{
		if ((bool)_lapRaceLeaderboardUIPanel)
		{
			_lapRaceLeaderboardUIPanel.SortUsersFinal();
		}
	}

	public void ShowResult(int place)
	{
		bool flag = place < 3;
		FinishResultText.text = (flag ? "WIN" : "LOSE");
		place++;
		FinishPlaceResultText.text = (flag ? ("Congratulations! you win race with " + place + " place") : "Try again)");
	}

	public void OpenGarage()
	{
		GarageMenuWindow.SetActive(value: true);
		CircuitClassWindow.SetActive(value: false);
		RaceMapSelectWindow.SetActive(value: false);
	}

	public void OpenRaceMapWindow()
	{
		GarageMenuWindow.SetActive(value: false);
		CircuitClassWindow.SetActive(value: false);
		RaceMapSelectWindow.SetActive(value: true);
	}

	public void SetRealtimeChartList(List<LapRaceCar> lapRaceCars, int userIndex)
	{
		_circuitRealtimeChartlist.ParseList(lapRaceCars, userIndex);
	}

	public void ShowChartList(bool state)
	{
		bool active = state;
		if (CircuitRaceControlller.IsQualification)
		{
			active = false;
		}
		_circuitRealtimeChartlist.gameObject.SetActive(active);
		if ((bool)_circuitPlayWindow)
		{
			_circuitPlayWindow.ShowRaceUIElements(state);
		}
	}

	public void ShowRaceMapWindow()
	{
		int num = CircuitRaceControlller.RaceMapModels.Length;
		DestroyRaceMapWindowElements();
		Dictionary<int, List<CircuitRacingRecord>> dictionary = CircuitRaceControlller.GetTrackRecords();
		if (_playerStorageService.Data.CircuitRacingProgress != null && _playerStorageService.Data.CircuitRacingProgress.RacingTrackRecords != null && _playerStorageService.Data.CircuitRacingProgress.RacingTrackRecords.Count > 0)
		{
			dictionary = _playerStorageService.Data.CircuitRacingProgress.RacingTrackRecords;
		}
		for (int i = 0; i < num; i++)
		{
			CircuitRaceMapElement circuitRaceMapElement = _prefabFactory.Instantiate(_circuitRaceMapElementPrefab, _circuitRaceMapElementsParent);
			circuitRaceMapElement.Init(i, dictionary[i], this, CircuitRaceControlller.RaceMapModels[i].RaceConfig.Direction);
			_circuitRaceMapElements.Add(circuitRaceMapElement);
		}
	}

	public void SetDirection(int index, int Direction)
	{
		CircuitRaceControlller.RaceMapModels[index].RaceConfig.Direction = Direction;
	}

	private void DestroyRaceMapWindowElements()
	{
		if (_circuitRaceMapElements != null)
		{
			for (int i = 0; i < _circuitRaceMapElements.Count; i++)
			{
				UnityEngine.Object.Destroy(_circuitRaceMapElements[i].gameObject);
			}
		}
		_circuitRaceMapElements = new List<CircuitRaceMapElement>();
	}

	public void ShowPitStop(bool state)
	{
		if ((bool)_circuitPlayWindow)
		{
			CircuitRaceControlller.SetInPitstop(state);
			CircuitPlayWindow.ShowToRoad(!state);
			CircuitPlayWindow.ShowReverseCam(!state);
			if (state)
			{
				_circuitPlayWindow.InitPitStop();
			}
			else
			{
				_circuitPlayWindow.Exit();
			}
		}
	}

	public void SetMaintenanceMessage(MaintenanceType maintenanceId, MaintenanceStatus maintenanceStatus)
	{
		string maintenanceText = "";
		switch (maintenanceId)
		{
		case MaintenanceType.Fuel:
			maintenanceText = "Fuel";
			break;
		case MaintenanceType.AllFix:
			maintenanceText = "AllFix";
			break;
		case MaintenanceType.Wheel:
			maintenanceText = "Wheel";
			break;
		}
		if ((bool)_circuitPlayWindow)
		{
			_circuitPlayWindow.SetPitStopMessage(maintenanceId, maintenanceStatus, maintenanceText);
		}
	}

	public void OnMapSelected(int mapIndex)
	{
	}

	public void UnPause()
	{
		Audio.SetPauseSoundFilter(on: false);
		Audio.ResetAllVolume();
		Time.timeScale = 1f;
		SetCameraTouchEnabled(state: true);
	}

	public void Pause()
	{
		Time.timeScale = 0f;
		Audio.SetPauseSoundFilter(on: true);
		SetCameraTouchEnabled(state: false);
	}

	private void SetCameraTouchEnabled(bool state)
	{
		if ((bool)_racingCameraController)
		{
			_racingCameraController.SetTouchInputEnabled(state);
		}
	}

	public void SetSmallMap(Texture mapCameraTargetTexture)
	{
		if ((bool)_circuitPlayWindow)
		{
			_circuitPlayWindow.SetMapImage(mapCameraTargetTexture);
		}
	}

	public void ShowToRoad(bool state)
	{
		if ((bool)_circuitPlayWindow)
		{
			_circuitPlayWindow.ShowToRoad(state);
		}
	}

	public void SetOwnCarOnMap(Transform carTransform)
	{
		if ((bool)_circuitPlayWindow)
		{
			_circuitPlayWindow.SetOwnCarOnMap(carTransform);
		}
	}

	public void SetCarOnMap(Transform carTransform)
	{
		if ((bool)_circuitPlayWindow)
		{
			_circuitPlayWindow.SetCarOnMap(carTransform);
		}
	}

	public void ClearMap()
	{
		if ((bool)_circuitPlayWindow)
		{
			_circuitPlayWindow.ClearMap();
		}
	}

	public void ShowDisqualifiedWindow()
	{
		_managerUI.CloseCircuitPlayWindow();
		_managerUI.ShowDisqualifiedWindow();
	}

	public void ShowCrashWithTimeMessage(float penaltyCrashSecs)
	{
		string format = new LocalizedString("CircuitPlayWindow.CrashPenaltyTime");
		_managerUI.ShowNotificationWindow(string.Format(format, penaltyCrashSecs), 2f, NotificationType.Error);
	}

	public void ShowCrashWithDistanceMessage(float penaltyCrashDistance)
	{
		string format = new LocalizedString("CircuitPlayWindow.CrashPenaltyDistance");
		_managerUI.ShowNotificationWindow(string.Format(format, penaltyCrashDistance), 2f, NotificationType.Error);
	}

	public void ResetTimer(bool isPlaySound = false)
	{
		_timeLeft = 0f;
		if (isPlaySound)
		{
			AudioWrapper.SetSwitch("UiSwitch", "Lap_Time_Count", base.gameObject);
			AudioWrapper.PostEvent("Ui_Event", base.gameObject);
		}
	}

	public void BestTimeChanged()
	{
		if ((bool)_circuitPlayWindow)
		{
			_circuitPlayWindow.BestTimeChanged();
		}
	}
}
