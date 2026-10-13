using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CPM.Ghost;
using CPM.Leaderboard.UI;
using CPMEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.Racing_Scripts.Rally.Leaderboard;
using _Car_Parking.Scripts.Racing_Scripts.RewardUI;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.Rally
{
	public class RallyFinishWindow : WindowScript
	{
		[SerializeField]
		private TextMeshProUGUI TimeText = default;

		[SerializeField]
		private TextMeshProUGUI PenaltyText = default;

		[SerializeField]
		private Button RestartButton = default;

		[SerializeField]
		private Button StageButton = default;

		[SerializeField]
		private Button ServicePackButton = default;

		[SerializeField]
		private Button BackButton = default;

		[Header("DisqualifiedPanel")]
		[SerializeField]
		private GameObject DisqualifiedPanel = default;

		[SerializeField]
		private TextMeshProUGUI DisqualifiedText = default;

		[SerializeField]
		private RallyRecordsUIPanel RallyRecordsUIPanel = default;

		[SerializeField]
		private LeaderboardListView LeaderboardListView = default;

		[SerializeField]
		private RaceRewardItem[] RaceRewardItems = default;

		[SerializeField]
		private RallyPenaltyIconElement[] PenaltyIconElements = default;

		[SerializeField]
		private GameObject PenaltyPanel = default;

		[SerializeField]
		private TextMeshProUGUI PenaltyPanelText = default;

		private RallyCar _rallyCar;

		private RallyController _rallyController;

		private RallyRecordsBank _bank;

		private GhostStorage _storage;

		private IRacingRewardAdService _racingRewardAdService;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Garage;

		private long LocalBest
		{
			get
			{
				if (_storage == null || !_storage.TryGetBestMeta(RallyLeaderboardMapId.LocalKey(), out var meta))
				{
					return -1L;
				}
				return meta.TimeMs;
			}
		}

		[Inject]
		[UnityEngine.Scripting.Preserve]
		public void Construct(ManagerUI managerUI, RallyRecordsBank bank, GhostStorage storage, IRacingRewardAdService racingRewardAdService)
		{
			_managerUI = managerUI;
			_bank = bank;
			_storage = storage;
			_racingRewardAdService = racingRewardAdService;
		}

		public void Init(RallyController rallyController, RallyCar rallyCar, bool isDisqualified = false)
		{
			_rallyController = rallyController;
			_rallyCar = rallyCar;
			SetTexts();
			RallyRecordsUIPanel.SetTargetTimes(rallyController);
			if (!isDisqualified)
			{
				_ = SetRewards(_rallyCar.TotalTime);
			}
			if ((bool)DisqualifiedPanel)
			{
				DisqualifiedPanel.SetActive(isDisqualified);
			}
			if (!isDisqualified)
			{
				ShowLeaderboard();
			}
		}

		private void ShowLeaderboard()
		{
			if ((bool)LeaderboardListView && _bank != null)
			{
				RallyLeaderboardMapId.TryBuild(_rallyController, out var mapId);
				LeaderboardListView.Init(mapId, _bank, LocalBest);
				LeaderboardListView.RefreshFromCache();
			}
		}

		public void SetDisqualifiedReason(RallyDisqualifyType rallyDisqualifyType)
		{
			string text = new LocalizedString($"RallyMode.{rallyDisqualifyType}");
			if ((bool)DisqualifiedText)
			{
				DisqualifiedText.text = text;
			}
			PenaltyPanel.SetActive(value: false);
		}

		private void SetTexts()
		{
			RallyTimes[] rallyTimes = _rallyController.RallySequenceData.RallyTimes;
			int num = Mathf.Min(RaceRewardItems.Length, rallyTimes.Length);
			for (int i = 0; i < num; i++)
			{
				RallyTimes rallyTimes2 = rallyTimes[i];
				if ((bool)RaceRewardItems[i])
				{
					RaceRewardItems[i].SetText(rallyTimes2.Price, 0);
				}
			}
			TimeText.text = Utils.FormatTime(_rallyCar.TotalTime);
			PenaltyText.text = "+" + Utils.FormatTime(_rallyCar.Penalty);
			ShowPenaltyBreakdown();
		}

		private void ShowPenaltyBreakdown()
		{
			Dictionary<PenaltyType, float> dictionary = new Dictionary<PenaltyType, float>();
			foreach (PenaltyRecord item in _rallyCar.PenaltyLog)
			{
				dictionary.TryGetValue(item.Type, out var value);
				dictionary[item.Type] = value + item.Amount;
			}
			float num = 0f;
			RallyPenaltyIconElement[] penaltyIconElements = PenaltyIconElements;
			foreach (RallyPenaltyIconElement rallyPenaltyIconElement in penaltyIconElements)
			{
				if (dictionary.TryGetValue(rallyPenaltyIconElement.PenaltyType, out var value2))
				{
					rallyPenaltyIconElement.gameObject.SetActive(value: true);
					rallyPenaltyIconElement.SetText(value2);
					num += value2;
				}
				else
				{
					rallyPenaltyIconElement.gameObject.SetActive(value: false);
				}
			}
			if ((bool)PenaltyPanelText)
			{
				PenaltyPanelText.text = "+" + Utils.FormatTime(num);
			}
			if ((bool)PenaltyPanel)
			{
				PenaltyPanel.SetActive(num > 0f);
			}
		}

		private static string PenaltyTypeLabel(PenaltyType type)
		{
			return type switch
			{
				PenaltyType.OffRoad => "Off Road", 
				PenaltyType.MissedCheckpoint => "Missed Checkpoint", 
				PenaltyType.MarshalCollision => "Marshal Collision", 
				PenaltyType.CutMarker => "Cut Marker", 
				PenaltyType.PeopleCollision => "People Collision", 
				PenaltyType.ReturnToRoad => "Return to Road", 
				_ => "Other", 
			};
		}

		protected override void Start()
		{
			base.Start();
			RestartButton.onClick.AddListener(RestartGame);
			ServicePackButton.onClick.AddListener(GoToGarage);
			StageButton.onClick.AddListener(GoToStage);
			BackButton.onClick.AddListener(BackToMenu);
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.ShortNotifySwitchState, base.gameObject);
		}

		private void GoToGarage()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.SmallClickSwitch, base.gameObject);
			_rallyController.GoToGarage();
			Close();
		}

		private void GoToStage()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.SmallClickSwitch, base.gameObject);
			_rallyController.OpenMapWindow();
			Close();
		}

		private void BackToMenu()
		{
			_rallyController.ExitToMainMenu();
			Close();
		}

		private async UniTaskVoid SetRewards(float result)
		{
			CancellationToken ct = this.GetCancellationTokenOnDestroy();
			try
			{
				RallyTimes[] times = _rallyController.RallySequenceData.RallyTimes;
				float ngFactor = _rallyController.RallyData?.EffectiveNgFactor ?? 1f;
				int count = Mathf.Min(RaceRewardItems.Length, times.Length);
				int[] prices = new int[count];
				int wonMoney = 0;
				int wonPlaces = 0;
				for (int i = 0; i < count; i++)
				{
					RallyTimes rallyTimes = times[i];
					bool flag = rallyTimes.TargetTime * ngFactor > result;
					prices[i] = rallyTimes.Price;
					RaceRewardItems[i].SetText(rallyTimes.Price, 0);
					RaceRewardItems[i].SetState(flag);
					if (flag)
					{
						wonMoney += rallyTimes.Price;
						wonPlaces++;
						RaceRewardItems[i].AnimateWin();
						await UniTask.Delay(TimeSpan.FromSeconds(0.5), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
					}
				}
				if (wonMoney > 0)
				{
					await ShowRewardAdWindow(prices, wonPlaces);
				}
			}
			catch (OperationCanceledException)
			{
			}
		}

		private async UniTask ShowRewardAdWindow(int[] rewards, int wonPlaces)
		{
			if (_racingRewardAdService != null && _racingRewardAdService.CanOfferForNewRace() && await _managerUI.ShowWindow("RacingRewardAdWindow", alwaysOnTop: true, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false) is RacingRewardAdWindow racingRewardAdWindow)
			{
				racingRewardAdWindow.InitReward(RacingRewardMode.Rally, rewards.Reverse().ToArray(), wonPlaces);
			}
		}

		private void RestartGame()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.SmallClickSwitch, base.gameObject);
			_rallyController.Restart();
			Close();
		}
	}
}
