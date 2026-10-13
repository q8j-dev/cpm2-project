using System;
using System.Threading;
using CPM.Ghost;
using CPM.Leaderboard.UI;
using Cysharp.Threading.Tasks;
using RacingScripts.RaceControllers;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Racing_Scripts.RewardUI;

namespace _Car_Parking.Scripts.Racing_Scripts.Rally.Leaderboard
{
	public sealed class RallyLeaderboardWindow : WindowScript, IBackableWindow
	{
		[SerializeField]
		private RaceRewardItem[] RaceRewardItems = default;

		[SerializeField]
		private LeaderboardListView ListView = default;

		[SerializeField]
		private Button BackButton = default;

		[SerializeField]
		private Button StartButton = default;

		[SerializeField]
		private RallyRecordsUIPanel RecordsPanel = default;

		private RaceModeController _raceModeController;

		private RallyRecordsBank _bank;

		private GhostStorage _storage;

		private RallyController _rallyController;

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

		[UnityEngine.Scripting.Preserve]
		[Inject]
		public void Construct(RaceModeController raceModeController, RallyRecordsBank bank, GhostStorage storage)
		{
			_raceModeController = raceModeController;
			_bank = bank;
			_storage = storage;
		}

		public void InitBackButton()
		{
			_managerUI.SetCurrentWindow(this);
		}

		protected override void Start()
		{
			base.Start();
			StartButton.onClick.AddListener(StartRace);
			_rallyController = _raceModeController.GetComponentInChildren<RallyController>();
			if (_storage.TryGetBestMeta(RallyLeaderboardMapId.LocalKey(), out var meta))
			{
				SetRewards(meta.TimeMs).Forget();
			}
			if ((bool)BackButton)
			{
				BackButton.SetActive(active: false);
			}
			InitBackButton();
			LoadList();
			RecordsPanel.SetTargetTimes(_rallyController);
		}

		private async UniTaskVoid SetRewards(float result)
		{
			CancellationToken ct = this.GetCancellationTokenOnDestroy();
			try
			{
				RallyTimes[] times = _rallyController.RallySequenceData.RallyTimes;
				float ngFactor = _rallyController.RallyData?.EffectiveNgFactor ?? 1f;
				int count = Mathf.Min(RaceRewardItems.Length, times.Length);
				for (int i = 0; i < count; i++)
				{
					RallyTimes rallyTimes = times[i];
					bool flag = rallyTimes.TargetTime * ngFactor > result;
					RaceRewardItems[i].SetText(rallyTimes.Price, 0);
					RaceRewardItems[i].SetState(flag);
					if (flag)
					{
						RaceRewardItems[i].AnimateWin();
						await UniTask.Delay(TimeSpan.FromSeconds(0.5), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
					}
				}
			}
			catch (OperationCanceledException)
			{
			}
		}

		private void LoadList()
		{
			if ((bool)ListView && (bool)_raceModeController && _bank != null && (bool)_raceModeController.SpawnedObject)
			{
				RallyLeaderboardMapId.TryBuild(_raceModeController.SpawnedObject.GetComponent<RallyController>(), out var mapId);
				ListView.Init(mapId, _bank, LocalBest);
				ListView.Refresh();
			}
		}

		private void StartRace()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.ClickBackSwitch, base.gameObject);
			_rallyController.StartStage(force: true).Forget();
			Close();
		}

		public void Back()
		{
			_managerUI.ShowWindow("RallyMenuWindow").Forget();
			Close();
		}
	}
}
