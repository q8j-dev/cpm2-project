using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.DailyRewards
{
	public class DailyRewardItem : MonoBehaviour
	{
		[SerializeField]
		private Button Button;

		[SerializeField]
		private TMP_Text[] DayTexts;

		[SerializeField]
		private DailyRewardItemTypeView[] DailyRewardItemTypeViews;

		[SerializeField]
		private DailyRewardData.TypeReward[] AvailableRewards;

		[SerializeField]
		private GameObjectStateUpdater LineLeft;

		[SerializeField]
		private GameObjectStateUpdater LineRight;

		[SerializeField]
		private DailyRewardStatusGameObjects StatusGraphics;

		private DailyRewardItem _dailyRewardItemNeighbour;

		private DailyRewardData _dailyRewardData;

		private CancellationTokenSource _cancellationTokenSourceTryAwardReward;

		public DailyRewardData DailyRewardData => _dailyRewardData;

		public GameObjectStateUpdater LineRightUI => LineRight;

		public GameObjectStateUpdater LineLeftUI => LineLeft;

		public DailyRewardData.TypeReward[] AvailableTypeRewards => AvailableRewards;

		public event Action<DailyRewardItem> OnClicked;

		public void Init(DailyRewardData dailyRewardData, DailyRewardItem dailyRewardItemNeighbour)
		{
			_dailyRewardData = dailyRewardData;
			_dailyRewardItemNeighbour = dailyRewardItemNeighbour;
			UpdateUI();
		}

		private void Start()
		{
			if (_dailyRewardData != null)
			{
				_dailyRewardData.OnDataChanged += OnDailyRewardChangedCallback;
			}
		}

		private void OnEnable()
		{
			Button.onClick.AddListener(OnButtonClick);
		}

		private void OnDisable()
		{
			Button.onClick.RemoveListener(OnButtonClick);
		}

		private void OnDestroy()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceTryAwardReward);
			if (_dailyRewardData != null)
			{
				_dailyRewardData.OnDataChanged -= OnDailyRewardChangedCallback;
			}
		}

		private void OnButtonClick()
		{
			OnClicked?.Invoke(this);
		}

		private void OnDailyRewardChangedCallback(DailyRewardData dailyRewardData)
		{
			UpdateUI();
		}

		private void UpdateUI()
		{
			if (_dailyRewardData == null)
			{
				return;
			}
			LineLeft.SetActive(_dailyRewardData.Day > 0);
			bool flag = _dailyRewardData.Status != DailyRewardData.StatusType.Closed;
			if (flag && (bool)_dailyRewardItemNeighbour && _dailyRewardItemNeighbour.DailyRewardData.Status == DailyRewardData.StatusType.Closed)
			{
				flag = false;
			}
			LineLeft.UpdateState(flag);
			if ((bool)_dailyRewardItemNeighbour)
			{
				_dailyRewardItemNeighbour.LineRightUI.SetActive(_dailyRewardData.Day > 0);
				_dailyRewardItemNeighbour.LineRightUI.UpdateState(flag);
			}
			StatusGraphics.UpdateState(_dailyRewardData.Status);
			for (int i = 0; i < DailyRewardItemTypeViews.Length; i++)
			{
				DailyRewardItemTypeViews[i].gameObject.SetActive(_dailyRewardData.Type == DailyRewardItemTypeViews[i].TypeReward);
				if (_dailyRewardData.Type == DailyRewardItemTypeViews[i].TypeReward)
				{
					DailyRewardItemTypeViews[i].Init(_dailyRewardData);
				}
			}
			for (int j = 0; j < DayTexts.Length; j++)
			{
				DayTexts[j].text = $"{_dailyRewardData.Day + 1}";
			}
		}
	}
}
