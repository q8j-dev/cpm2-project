using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.DailyRewards
{
	public class DailyRewardsWindow : WindowScript
	{
		private const int DaysInWeek = 7;

		[SerializeField]
		private CanvasGroup MainPanel = default;

		[SerializeField]
		private DailyRewardItem DailyRewardItemSimplePrefab = default;

		[SerializeField]
		private DailyRewardItem DailyRewardItemDay7Prefab = default;

		[SerializeField]
		private ScrollRect ScrollRect = default;

		[SerializeField]
		private TMP_Text WeekText = default;

		[SerializeField]
		private ScrollRectItemsCenterController ScrollRectItemsCenterController = default;

		[SerializeField]
		private DailyRewardsWindowNextPrevWeek ButtonWeekNext = default;

		[SerializeField]
		private DailyRewardsWindowNextPrevWeek ButtonWeekPrevius = default;

		[SerializeField]
		private TabIconsController WeekTabs = default;

		private IDailyRewardsServer _dailyRewardsServer;

		private CancellationTokenSource _cancellationTokenSourceUpdateUI;

		private IPrefabFactory _prefabFactory;

		private DailyRewardAnalytic _dailyRewardAnalytic;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

		private int CurrentWeek => Mathf.Abs(ScrollRectItemsCenterController.ItemCenterIndex) / 7;

		private int WeekMax => ScrollRect.content.childCount / 7;

		[Inject]
		public void Construct(IDailyRewardsServer dailyRewardsServer, IPrefabFactory prefabFactory, DailyRewardAnalytic dailyRewardAnalytic)
		{
			_dailyRewardsServer = dailyRewardsServer;
			_prefabFactory = prefabFactory;
			_dailyRewardAnalytic = dailyRewardAnalytic;
		}

		protected override void OnEnable()
		{
			UpdateUI();
			ScrollRectItemsCenterController.OnItemCenterChanged += OnItemCenterChangedCallback;
			ButtonWeekPrevius.OnClicked += OnButtonWeekPreviusClickedCallback;
			ButtonWeekNext.OnClicked += OnButtonWeekNextClickedCallback;
			base.OnEnable();
		}

		private void OnDisable()
		{
			ScrollRectItemsCenterController.OnItemCenterChanged -= OnItemCenterChangedCallback;
			ButtonWeekPrevius.OnClicked -= OnButtonWeekPreviusClickedCallback;
			ButtonWeekNext.OnClicked -= OnButtonWeekNextClickedCallback;
		}

		protected override void OnDestroy()
		{
			_managerUI.CloseWaitingWindow();
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceUpdateUI);
			base.OnDestroy();
		}

		private void OnItemCenterChangedCallback(GameObject itemCenter)
		{
			UpdateWeekText();
			UpdateWeekLeftRightButtons();
		}

		private void OnButtonWeekPreviusClickedCallback(DailyRewardsWindowNextPrevWeek button)
		{
			MoveToWeek(CurrentWeek - 1);
		}

		private void OnButtonWeekNextClickedCallback(DailyRewardsWindowNextPrevWeek button)
		{
			MoveToWeek(CurrentWeek + 1);
		}

		private void MoveToWeek(int week)
		{
			ScrollRectItemsCenterController.MoveToItemCenter(week * 7 + 3);
		}

		private void UpdateUI()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceUpdateUI);
			_cancellationTokenSourceUpdateUI = new CancellationTokenSource();
			UpdateUIAsync(_cancellationTokenSourceUpdateUI.Token).Forget();
		}

		private async UniTaskVoid UpdateUIAsync(CancellationToken cancellationToken)
		{
			MainPanel.alpha = 0f;
			await _managerUI.ShowWaitingWindow(set: true, backButtonState: true);
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
			int amountCheck = 5;
			while (amountCheck-- > 0 && _dailyRewardsServer.Count == 0)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(2.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			}
			IReadOnlyList<DailyRewardData> rewards = _dailyRewardsServer.Rewards;
			if (rewards.Count == 0)
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("DailyRewards.Message.ServerIsNotAvailable"), 3f, NotificationType.Error).Forget();
			}
			_managerUI.CloseWaitingWindow();
			int currentStreak = rewards.Count((DailyRewardData r) => r.Status != DailyRewardData.StatusType.Closed);
			_dailyRewardAnalytic?.LogShown(currentStreak);
			ScrollRect.content.RemoveAllChild();
			int lastActiveRewardIndex = 0;
			DailyRewardItem dailyRewardItemNeighbour = null;
			for (int num = 0; num < rewards.Count; num++)
			{
				DailyRewardItem dailyRewardItem = _prefabFactory.Instantiate(GetPrefab(rewards[num].Day), ScrollRect.content);
				dailyRewardItem.LineRightUI.SetActive(num < rewards.Count - 1);
				dailyRewardItem.Init(rewards[num], dailyRewardItemNeighbour);
				dailyRewardItem.OnClicked += OnDailyRewardItemClickedCallback;
				if (rewards[num].Status == DailyRewardData.StatusType.Opened || rewards[num].Status == DailyRewardData.StatusType.Rewarded)
				{
					lastActiveRewardIndex = num;
				}
				dailyRewardItemNeighbour = dailyRewardItem;
			}
			WeekTabs.Init(WeekMax);
			await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
			if (!cancellationToken.IsCancellationRequested)
			{
				if (rewards.Count > 0)
				{
					MainPanel.alpha = 1f;
				}
				ScrollRectItemsCenterController.ItemCenterIndex = lastActiveRewardIndex;
				OnItemCenterChangedCallback(ScrollRectItemsCenterController.ItemCenter);
			}
		}

		private void UpdateWeekText()
		{
			WeekText.text = string.Format(new LocalizedString("Mainmenuwindow.Rewards.Week"), CurrentWeek + 1);
		}

		private void UpdateWeekLeftRightButtons()
		{
			ButtonWeekPrevius.UpdateText(CurrentWeek - 1);
			ButtonWeekNext.UpdateText(CurrentWeek + 1);
			ButtonWeekPrevius.SetActive(CurrentWeek > 0);
			ButtonWeekNext.SetActive(CurrentWeek + 1 < WeekMax);
			WeekTabs.SetActive(CurrentWeek);
		}

		private void OnDailyRewardItemClickedCallback(DailyRewardItem item)
		{
			OnDailyRewardItemClickedCallbackAsync(item).Forget();
		}

		private async UniTask OnDailyRewardItemClickedCallbackAsync(DailyRewardItem item)
		{
			DailyRewardData dailyRewardData = item.DailyRewardData;
			if (dailyRewardData == null || dailyRewardData.Status != DailyRewardData.StatusType.Opened)
			{
				return;
			}
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.SmallClickSwitch, base.gameObject);
			SuspendUiNavigation();
			UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
			WindowScript windowScript = await _managerUI.ShowWindow("DailyRewardsAwardWindow", alwaysOnTop: true, show: true, null, fadeBlack: false, enableBackButton: true, cleanScreen: false, item.DailyRewardData.Day.ToString() ?? "");
			if ((bool)windowScript)
			{
				windowScript.OnDestroyAsObservable().Subscribe((Unit _) =>
				{
					RestoreUiNavigation();
				});
			}
		}

		private DailyRewardItem GetPrefab(int day)
		{
			if (day % 7 == 6)
			{
				return DailyRewardItemDay7Prefab;
			}
			return DailyRewardItemSimplePrefab;
		}
	}
}
