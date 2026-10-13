using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Database;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Racing_Scripts.RewardUI;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.UIElement.UIAnimations;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	public class RallySelectLocationWindow : WindowScript, IBackableWindow
	{
		[SerializeField]
		[Header("NGPlusPanel")]
		private GameObject ScrollView = default;

		[SerializeField]
		private LockableUiToggleElement NGPlusToggle = default;

		[SerializeField]
		private TextMeshProUGUI NgCountdownText = default;

		[SerializeField]
		private GameObject NGPlusPanel = default;

		[SerializeField]
		private GameObject ScrollBar = default;

		[SerializeField]
		private GameObject RightCarPanel = default;

		[SerializeField]
		private GameObject WaitingWindow = default;

		[SerializeField]
		private GameObject TimePanel = default;

		[SerializeField]
		private RallyMapDataSO RallyMapDataSo = default;

		[SerializeField]
		private RallyTrajectoryWeatherDataSO RallyTrajectoryWeatherDataSO = default;

		[SerializeField]
		private RallyCarsDatasSO RallyCarsDatasSo = default;

		[Header("Progress")]
		[SerializeField]
		private TextMeshProUGUI GoldRewardText;

		[SerializeField]
		private TextMeshProUGUI PersonalRecordTimeText = default;

		[SerializeField]
		private RaceRewardItem[] RaceRewardItems = default;

		[Header("All Rewards Panel")]
		[SerializeField]
		private TextMeshProUGUI GoldCupsCountText = default;

		[SerializeField]
		private TextMeshProUGUI SilverCupsCountText = default;

		[SerializeField]
		private TextMeshProUGUI CopperCupsCountText = default;

		[Header("Location")]
		[SerializeField]
		private RallyLockableUiToggleElement LocationElementPrefab = default;

		[SerializeField]
		private ScrollRect LocationScrollRect = default;

		[SerializeField]
		private ToggleGroup LocationGroup = default;

		[SerializeField]
		private CrossFadeImageSwitcher LocationBackgroundSwitcher = default;

		[Header("Trajectory")]
		[SerializeField]
		private ScrollRect TrajectoryScrollRect = default;

		[SerializeField]
		private RallyTrajectoryLockableToggle TrajectoryElementPrefab = default;

		[SerializeField]
		private ToggleGroup TrajectoryGroup = default;

		private List<RallyTrajectoryLockableToggle> _cachedTrajectoryElements = new List<RallyTrajectoryLockableToggle>();

		private List<RallyLockableUiToggleElement> _cachedLocationElements = new List<RallyLockableUiToggleElement>();

		private CancellationTokenSource _ngCountdownCts;

		[SerializeField]
		[Header("UI")]
		private Button SelectcarButton = default;

		[SerializeField]
		private Button BackButton = default;

		[SerializeField]
		private Button SettingsButton = default;

		[SerializeField]
		private Button InfoButton = default;

		[SerializeField]
		private UIPulseAnimator InfoButtonAnimator = default;

		[SerializeField]
		private CanvasGroup MainPanel = default;

		[SerializeField]
		[Header("StartButton")]
		private Button StartButton = default;

		[SerializeField]
		private TextMeshProUGUI StartButtonText = default;

		[Header("CarPanel")]
		[SerializeField]
		private CarSelectorPanel CarSelectorPanel = default;

		private RallyController _rallyController;

		private int _selectedLocationId;

		private int _selectedTrajectoryId;

		private IPrefabFactory _prefabFactory;

		private OptionalDependency<CircuitRacingWaitingWindow> _circuitRacingWaitingWindowDep;

		private CircuitRacingWaitingWindow _circuitRacingWaitingWindow;

		private V.placeToSpawn _placeToSpawn;

		private int _frontierLocationId;

		private int _frontierTrajectoryId;

		private bool _allCompleted;

		private RallyProgressController _rallyProgressController;

		private RallyData _rallyData;

		private AllCarsInfo _allCarsInfo;

		private WindowScript _trackedWindow;

		private RainSound _rainSound;

		private bool _rainPaused;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

		[Inject]
		public void Construct(OptionalDependency<CircuitRacingWaitingWindow> circuitRacingWaitingWindowDep, IPrefabFactory prefabFactory, RallyData rallyData, AllCarsInfo allCarsInfo, RainSound rainSound)
		{
			_circuitRacingWaitingWindowDep = circuitRacingWaitingWindowDep;
			_prefabFactory = prefabFactory;
			_rallyData = rallyData;
			_allCarsInfo = allCarsInfo;
			_rainSound = rainSound;
		}

		public void Init(RallyController rallyController, RallyProgressController rallyProgressController)
		{
			_rallyController = rallyController;
			_rallyProgressController = rallyProgressController;
			SetAllCupsData();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			BackButton.SetActive(active: false);
		}

		protected override void Start()
		{
			RallyTrajectoryWeatherDataSO.Init();
			base.Start();
			MapAmbienceOn();
			SetProgress().Forget();
			_circuitRacingWaitingWindow = _circuitRacingWaitingWindowDep?.Value;
			InitBackButton();
			SelectcarButton.onClick.AddListener(ChangeCar);
			StartButton.onClick.AddListener(StartGame);
			BackButton.onClick.AddListener(Back);
			NGPlusToggle.onValueChanged.AddListener(ChangeNGPlusButtonState);
			InfoButton.onClick.AddListener(ShowRallyInfoWindow);
			SettingsButton.onClick.AddListener(ShowSettingsWindow);
			if ((bool)_circuitRacingWaitingWindow)
			{
				_circuitRacingWaitingWindow.gameObject.SetActive(value: false);
			}
			if ((bool)LocationBackgroundSwitcher)
			{
				LocationBackgroundSwitcher.SwitchTo(0);
			}
			if (ScenesHelper.IsMenuSceneActive())
			{
				ManagerUIHelper.CloseNotTransparentWaitingWindow().Forget();
			}
			if (PlayerPrefs.GetInt("InfoButtonClicked", 0) != 1)
			{
				InfoButtonAnimator.StartPulse();
			}
			if (PlayerPrefs.GetInt("InfoAutoShowed", 0) != 1)
			{
				PlayerPrefs.SetInt("InfoAutoShowed", 1);
				_ = _managerUI.ShowRallyInfoWindow();
			}
			ManagerUI managerUI = _managerUI;
			managerUI.OnRallyPopupShowed = (Action<bool>)Delegate.Combine(managerUI.OnRallyPopupShowed, new Action<bool>(HideMainPanel));
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			ManagerUI managerUI = _managerUI;
			managerUI.OnRallyPopupShowed = (Action<bool>)Delegate.Remove(managerUI.OnRallyPopupShowed, new Action<bool>(HideMainPanel));
			_trackedWindow?.Close();
		}

		private void HideMainPanel(bool hide)
		{
			MainPanel.alpha = ((!hide) ? 1 : 0);
			MainPanel.interactable = !hide;
		}

		private void SetNGButtonState()
		{
			NGPlusToggle.LockVisual(!_rallyData.NgAvailable);
		}

		public void ResetProgress()
		{
			SetNGButtonState();
			(_frontierLocationId, _frontierTrajectoryId, _allCompleted) = _rallyProgressController.GetUnlockFrontier();
			SetAllCupsData();
			for (int i = 0; i < RaceRewardItems.Length; i++)
			{
				RaceRewardItems[i].SetState(win: false);
			}
			if (_cachedLocationElements.Count > 0)
			{
				_cachedLocationElements[0].SetIsOnWithoutNotify(value: true);
				OnLocationChanged(0);
			}
		}

		private async UniTaskVoid SetProgress()
		{
			if (ScenesHelper.IsMenuSceneActive())
			{
				WaitingWindow.gameObject.SetActive(value: true);
				_rallyProgressController = new RallyProgressController(RallyMapDataSo);
				await _rallyProgressController.GetAllLocationsProgressAsync(_rallyData);
				WaitingWindow.gameObject.SetActive(value: false);
			}
			CreateUIElements().Forget();
			SetNGButtonState();
		}

		private void ChangeNGPlusButtonState(bool state)
		{
			if (state)
			{
				ShowNgPanel(state);
			}
		}

		private void ShowNgPanel(bool state)
		{
			StartButton.interactable = _rallyData.NgAvailable;
			ScrollView.SetActive(!state);
			RightCarPanel.SetActive(!state);
			ScrollBar.SetActive(!state);
			NGPlusPanel.SetActive(state);
			TimePanel.SetActive(state);
			StartButtonText.text = (state ? new LocalizedString("RallyMode.NewCampaign") : new LocalizedString("Mainmenuwindow.start"));
			if (state)
			{
				StartNgCountdown();
			}
			else
			{
				StopNgCountdown();
			}
		}

		private void StartNgCountdown()
		{
			StopNgCountdown();
			RunNgCountdownAsync((_ngCountdownCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy())).Token).Forget();
		}

		private void StopNgCountdown()
		{
			_ngCountdownCts?.Cancel();
			_ngCountdownCts?.Dispose();
			_ngCountdownCts = null;
		}

		private async UniTaskVoid RunNgCountdownAsync(CancellationToken ct)
		{
			long num = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - _rallyData.LastNgPlusAt;
			int remainingSeconds = (int)Math.Max(0L, _rallyData.NgCooldown - num);
			if (remainingSeconds > 0)
			{
				StartButton.interactable = false;
				TimePanel.SetActive(value: true);
			}
			while (remainingSeconds > 0)
			{
				TimeSpan timeSpan = TimeSpan.FromSeconds(remainingSeconds);
				NgCountdownText.text = ((timeSpan.TotalDays >= 1.0) ? $"{(int)timeSpan.TotalDays}d {timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}" : $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}");
				if (await UniTask.Delay(1000, ignoreTimeScale: false, PlayerLoopTiming.Update, ct).SuppressCancellationThrow())
				{
					return;
				}
				remainingSeconds--;
			}
			StartButton.interactable = _rallyData.NgAvailable;
			TimePanel.SetActive(value: false);
			NgCountdownText.text = "--:--:--";
		}

		private void SetNewGamePlus()
		{
			if (_rallyData.NgAvailable)
			{
				SetNewGamePlusAsync().Forget();
				return;
			}
			string text = new LocalizedString("RallyMode.NGPlusNotAvailble");
			_ = _managerUI.ShowNotificationWindow(text, 3f, NotificationType.Error);
		}

		private async UniTaskVoid SetNewGamePlusAsync()
		{
			WaitingWindow.SetActive(value: true);
			if (await _rallyProgressController.SetNewGamePlus())
			{
				ResetProgress();
			}
			RallyTrajectoryWeatherDataSO.ShuffleData();
			WaitingWindow.SetActive(value: false);
		}

		private void ChangeCar()
		{
			string text = new LocalizedString("RallyMode.CantSelectCar");
			_managerUI.ShowNotificationWindow(text, 3f, NotificationType.Error).Forget();
		}

		private async UniTaskVoid CreateLocations()
		{
			(int, int, bool) unlockFrontier = _rallyProgressController.GetUnlockFrontier();
			_frontierLocationId = unlockFrontier.Item1;
			_frontierTrajectoryId = unlockFrontier.Item2;
			_allCompleted = unlockFrontier.Item3;
			for (int i = 0; i < RallyMapDataSo.RallyLocationDatas.Count; i++)
			{
				RallyLockableUiToggleElement rallyLockableUiToggleElement = _prefabFactory.Instantiate(LocationElementPrefab, LocationScrollRect.content);
				rallyLockableUiToggleElement.Init(RallyMapDataSo.RallyLocationDatas[i], i, OnLocationChanged);
				rallyLockableUiToggleElement.group = LocationGroup;
				bool state = !_allCompleted && i > _frontierLocationId;
				rallyLockableUiToggleElement.LockVisual(state);
				_cachedLocationElements.Add(rallyLockableUiToggleElement);
			}
			_cachedLocationElements[0].SetIsOnWithoutNotify(value: true);
			OnLocationChanged(_cachedLocationElements[0].Index);
			NGPlusToggle.transform.SetAsLastSibling();
			UIHelper.DelayedRebuildLayout(LocationScrollRect.content).Forget();
		}

		private async UniTask CreateTrajectory(RallyMapTrajectorySO trajectory, int locationId)
		{
			PersonalRecordTimeText.text = "--:--:--";
			int num = 0;
			for (int i = 0; i < _cachedTrajectoryElements.Count; i++)
			{
				_cachedTrajectoryElements[i].gameObject.SetActive(value: false);
			}
			for (int j = 0; j < trajectory.RallySequenceDataList.Count; j++)
			{
				if (_cachedTrajectoryElements.Count <= j)
				{
					RallyTrajectoryLockableToggle rallyTrajectoryLockableToggle = _prefabFactory.Instantiate(TrajectoryElementPrefab, TrajectoryScrollRect.content);
					_cachedTrajectoryElements.Add(rallyTrajectoryLockableToggle);
					rallyTrajectoryLockableToggle.group = TrajectoryGroup;
				}
				RallyWeatherData weatherFor = GetWeatherFor(locationId, j);
				_cachedTrajectoryElements[j].Init(j, OnTrajectoryChanged);
				_cachedTrajectoryElements[j].UpdateUI(trajectory.RallySequenceDataList[j], j, weatherFor.WeatherType, weatherFor.DayTimeType);
				_cachedTrajectoryElements[j].gameObject.SetActive(value: true);
				bool flag = IsTrajectoryLocked(j);
				if (!flag)
				{
					num = j;
				}
				_cachedTrajectoryElements[j].LockVisual(flag);
			}
			_cachedTrajectoryElements[num].SetIsOnWithoutNotify(value: true);
			OnTrajectoryChanged(num);
			if (_cachedTrajectoryElements.Count > num && _cachedTrajectoryElements[num].TryGetComponent<RectTransform>(out var component))
			{
				await TrajectoryScrollRect.SetScrollToRectTransform(component);
			}
			UIHelper.DelayedRebuildLayout(TrajectoryScrollRect.content).Forget();
		}

		private RallyWeatherData GetWeatherFor(int locationPosition, int trajectoryPosition)
		{
			if (!RallyMapDataSo.TryGetConfigIds(locationPosition, trajectoryPosition, out var locationId, out var trajectoryId))
			{
				return RallyTrajectoryWeatherDataSO.GetWeatherData(locationPosition, trajectoryPosition);
			}
			return RallyTrajectoryWeatherDataSO.GetWeatherData(locationId, trajectoryId);
		}

		private bool IsTrajectoryLocked(int index)
		{
			bool flag = false;
			if (_allCompleted || _selectedLocationId < _frontierLocationId)
			{
				return false;
			}
			if (_selectedLocationId == _frontierLocationId)
			{
				return index > _frontierTrajectoryId;
			}
			return true;
		}

		private async UniTaskVoid CreateUIElements()
		{
			CreateLocations().Forget();
		}

		private void OnLocationChanged(int index)
		{
			ShowNgPanel(state: false);
			StartButton.interactable = true;
			_selectedLocationId = index;
			_placeToSpawn = _cachedLocationElements[index].PlaceToSpawn;
			CreateTrajectory(RallyMapDataSo.RallyLocationDatas[index].TrajectoryData, index).Forget();
			SetProgressData(index);
			if ((bool)LocationBackgroundSwitcher)
			{
				LocationBackgroundSwitcher.SwitchTo(index);
			}
		}

		private void OnTrajectoryChanged(int index)
		{
			StartButton.interactable = !IsTrajectoryLocked(index);
			_selectedTrajectoryId = index;
			SetCarData();
			ParseTrajectory(index);
		}

		private void ParseTrajectory(int index)
		{
			if (index < 0 || index >= _cachedTrajectoryElements.Count)
			{
				return;
			}
			PersonalRecordTimeText.text = _cachedTrajectoryElements[index].BestTimeTextString;
			int progress = _cachedTrajectoryElements[index].Progress;
			for (int i = 0; i < RaceRewardItems.Length; i++)
			{
				RaceRewardItems[i].SetState(i < progress);
			}
			if (!RallyMapDataSo || _selectedTrajectoryId < 0 || _selectedTrajectoryId >= RallyMapDataSo.RallyLocationDatas.Count)
			{
				return;
			}
			RallyMapTrajectorySO trajectoryData = RallyMapDataSo.RallyLocationDatas[_selectedTrajectoryId].TrajectoryData;
			if (index >= 0 && index < trajectoryData.RallySequenceDataList.Count)
			{
				int num = Mathf.Min(RaceRewardItems.Length, trajectoryData.RallySequenceDataList[index].RallyTimes.Length);
				for (int j = 0; j < num; j++)
				{
					RaceRewardItems[num - j - 1].UpdateText(trajectoryData.RallySequenceDataList[index].RallyTimes[j]);
				}
			}
		}

		private void SetAllCupsData()
		{
			int[] array = _rallyProgressController.RallyData?.TotalCups;
			if (array != null && array.Length >= 3)
			{
				GoldCupsCountText.text = array[0].ToString();
				SilverCupsCountText.text = array[1].ToString();
				CopperCupsCountText.text = array[2].ToString();
			}
		}

		private void SetProgressData(int locationId)
		{
			if (locationId >= 0 && locationId < RallyMapDataSo.RallyLocationDatas.Count)
			{
				RallyMapTrajectorySO trajectoryData = RallyMapDataSo.RallyLocationDatas[locationId].TrajectoryData;
				int b = ((trajectoryData != null) ? trajectoryData.RallySequenceDataList.Count : 0);
				int num = Mathf.Min(_cachedTrajectoryElements.Count, b);
				for (int i = 0; i < num; i++)
				{
					RallyProgress progress = _rallyProgressController.GetProgress(locationId, i);
					if (progress != null && !(progress.PersonalBest <= 0f))
					{
						_cachedTrajectoryElements[i].SetBestTime(progress.PersonalBest, progress.RallyTier, trajectoryData.RallySequenceDataList[i]);
					}
				}
			}
			ParseTrajectory(_selectedTrajectoryId);
		}

		private void StartGame()
		{
			if (_rallyData != null && _rallyData.NgAvailable && NGPlusToggle.isOn)
			{
				SetNewGamePlus();
				return;
			}
			if (ScenesHelper.IsMenuSceneActive())
			{
				RallyController.LocationIndex = _selectedLocationId;
				RallyController.TrajectoryIndex = _selectedTrajectoryId;
				_managerUI.ShowWaitingWindow(set: true).Forget();
				SceneManager.LoadSceneAsync(41);
				Close();
			}
			else
			{
				_ = _rallyController.InitRally(_placeToSpawn, _selectedLocationId, _selectedTrajectoryId, RallyMapDataSo.RallyLocationDatas[_selectedLocationId].TrajectoryData);
			}
			Close();
		}

		private void SetCarData()
		{
			if (RallyMapDataSo.TryGetConfigIds(_selectedLocationId, _selectedTrajectoryId, out var locationId, out var trajectoryId))
			{
				MainCarValues mainCarValues = RallyCarsDatasSo.GetRallyCarData(locationId, trajectoryId, _allCarsInfo)?.MainCarValues;
				if (!(mainCarValues == null))
				{
					CarSelectorPanel.SetClassText(mainCarValues.Class);
					CarSelectorPanel.SetCarIcon(mainCarValues.CarID);
					CarSelectorPanel.SetCarClassImage((int)mainCarValues.Gearbox);
				}
			}
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			if (ScenesHelper.IsMenuSceneActive())
			{
				_managerUI.ShowSelectRacigModeWindow();
				Close();
			}
			else
			{
				_rallyController.ExitToMainMenu();
			}
		}

		private void MapAmbienceOn()
		{
			if (!_rainPaused)
			{
				_rainSound.Pause();
				_rainPaused = true;
			}
			_Car_Parking.Scripts.Audio.Audio.SetAmbienceVolume(_Car_Parking.Scripts.Audio.Audio.AmbienceVolume);
			AudioWrapper.SetState("State_Track_Select", "In_Prefab");
			AudioWrapper.PostEvent("Play_Amb_Rally_Track_Select", base.gameObject);
		}

		private void MapAmbienceOff()
		{
			if (_rainPaused)
			{
				_rainSound.Resume();
				_rainPaused = false;
			}
			AudioWrapper.PostEvent("Stop_Amb_Rally_Track_Select", base.gameObject);
			AudioWrapper.SetState("State_Track_Select", "Out_Prefab");
			_Car_Parking.Scripts.Audio.Audio.SetAmbienceVolume(_Car_Parking.Scripts.Audio.Audio.AmbienceVolume);
		}

		private void OnDisable()
		{
			MapAmbienceOff();
		}

		private void ShowRallyInfoWindow()
		{
			OpenAndTrackWindow(isInfo: true, this.GetCancellationTokenOnDestroy()).Forget();
		}

		private async UniTask OpenAndTrackWindow(bool isInfo, CancellationToken ct)
		{
			if (isInfo)
			{
				PlayerPrefs.SetInt("InfoButtonClicked", 1);
				InfoButtonAnimator.StopPulse();
				_trackedWindow = await _managerUI.ShowRallyInfoWindow().AttachExternalCancellation(ct);
			}
			else
			{
				_trackedWindow = await _managerUI.ShowRallySettingsWindow().AttachExternalCancellation(ct);
			}
			_trackedWindow.OnDestroyAsObservable().Subscribe((Unit _) =>
			{
				_trackedWindow = null;
			});
		}

		private void ShowSettingsWindow()
		{
			OpenAndTrackWindow(isInfo: false, this.GetCancellationTokenOnDestroy()).Forget();
		}
	}
}
