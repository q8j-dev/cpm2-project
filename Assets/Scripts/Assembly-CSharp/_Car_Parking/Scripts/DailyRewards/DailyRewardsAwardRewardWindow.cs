using System;
using System.Collections.Generic;
using CPM.PlayerData;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Firebase.Functions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.DailyRewards
{
	public class DailyRewardsAwardRewardWindow : WindowScript, IBackableWindow
	{
		[Serializable]
		private class CarPanelData
		{
			public const float AnimationDuration = 0.3f;

			public GameObject ButtonsRoot;

			public Button ButtonExchange;

			public Button ButtonPlaceToSlot;

			public Button ButtonGoToGarage;

			public CombinedPriceView CombinedPriceView;

			public Transform IconRoot;

			public Button ButtonExchangeTapAnywhere;

			public GameObject CancelExchangeCarRoot;

			public Button ButtonCancelExchange;

			[NonSerialized]
			public CombinedPrice CarPrice;
		}

		[SerializeField]
		private GameObject TapAnywhereRoot;

		[SerializeField]
		private Button TapAnywhereButton;

		[SerializeField]
		private CombinedPriceView CombinedPriceView;

		[SerializeField]
		private RawImageLoadAsync CarImage;

		[SerializeField]
		private DailyRewardTypeGameObjects DailyRewardTypeGameObjects;

		[SerializeField]
		private CarPanelData CarPanel;

		[SerializeField]
		private TMP_Text[] DayTexts;

		private IDailyRewardsServer _dailyRewardsServer;

		private DailyRewardData _dailyRewardData;

		private ISlotService _slotService;

		private PlayerDataStorage _playerDataStorage;

		private DailyRewardAnalytic _dailyRewardAnalytic;

		private static readonly Dictionary<int, bool> _hadFreeSlotByDay = new Dictionary<int, bool>();

		private EventRegistry _eventRegistry;

		private RegistryHandle<ScriptableEvent> _acceptHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.Accept);

		[Inject]
		public void Construct(IDailyRewardsServer dailyRewardsServer, ISlotService slotService, PlayerDataStorage playerDataStorage, DailyRewardAnalytic dailyRewardAnalytic, EventRegistry eventRegistry)
		{
			_dailyRewardsServer = dailyRewardsServer;
			_slotService = slotService;
			_playerDataStorage = playerDataStorage;
			_dailyRewardAnalytic = dailyRewardAnalytic;
			_eventRegistry = eventRegistry;
		}

		public void InitBackButton()
		{
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			OnTapAnywhereButtonClick();
		}

		public override void SetAdditionalData(string data)
		{
			base.SetAdditionalData(data);
			if (int.TryParse(data, out var result))
			{
				InitUI(result).Forget();
			}
			else
			{
				InitUI(-1).Forget();
			}
		}

		protected override void OnEnable()
		{
			TapAnywhereButton.onClick.AddListener(OnTapAnywhereButtonClick);
			base.OnEnable();
			InitBackButton();
			_acceptHandle.Get(_eventRegistry).Subscribe(OnTapAnywhereButtonClick);
		}

		private void OnDisable()
		{
			TapAnywhereButton.onClick.RemoveListener(OnTapAnywhereButtonClick);
			RemoveCarExchangeButtonsListeners();
			RemoveCarButtonsListeners();
			_acceptHandle.Get(_eventRegistry)?.Unsubscribe(OnTapAnywhereButtonClick);
		}

		private void AddCarButtonsListeners()
		{
			CarPanel.ButtonExchange.onClick.AddListener(OnCarExchangedButtonClick);
			CarPanel.ButtonGoToGarage.onClick.AddListener(OnGoToGarageButtonClick);
			CarPanel.ButtonPlaceToSlot.onClick.AddListener(OnPlaceToSlotButtonClick);
		}

		private void RemoveCarButtonsListeners()
		{
			CarPanel.ButtonExchange.onClick.RemoveListener(OnCarExchangedButtonClick);
			CarPanel.ButtonGoToGarage.onClick.RemoveListener(OnGoToGarageButtonClick);
			CarPanel.ButtonPlaceToSlot.onClick.RemoveListener(OnPlaceToSlotButtonClick);
		}

		private void AddCarExchangeButtonsListeners()
		{
			CarPanel.ButtonExchangeTapAnywhere.onClick.AddListener(OnTapExchangeAnywhereButtonClickCallback);
			CarPanel.ButtonCancelExchange.onClick.AddListener(OnTapCancelExchangeButtonClickCallback);
		}

		private void RemoveCarExchangeButtonsListeners()
		{
			CarPanel.ButtonExchangeTapAnywhere.onClick.RemoveListener(OnTapExchangeAnywhereButtonClickCallback);
			CarPanel.ButtonCancelExchange.onClick.RemoveListener(OnTapCancelExchangeButtonClickCallback);
		}

		private void OnTapAnywhereButtonClick()
		{
			if (_dailyRewardData != null)
			{
				TapAnywhereButton.onClick.RemoveListener(OnTapAnywhereButtonClick);
				if (_dailyRewardData.Type == DailyRewardData.TypeReward.Car)
				{
					InitCarUIAsync().Forget();
				}
				else
				{
					TryAwardRewardAsync().Forget();
				}
			}
		}

		private void OnCarExchangedButtonClick()
		{
			RemoveCarButtonsListeners();
			CarExchangeUIInit();
		}

		private void OnGoToGarageButtonClick()
		{
			RemoveCarButtonsListeners();
			_managerUI.ShowNotificationWindow(new LocalizedString("DailyRewards.Message.ClearSlot"), 3f, NotificationType.Error);
			_managerUI.ShowGarageWindow();
			Close();
		}

		private void OnPlaceToSlotButtonClick()
		{
			RemoveCarButtonsListeners();
			TryAwardRewardAsync().Forget();
		}

		private void OnTapExchangeAnywhereButtonClickCallback()
		{
			RemoveCarExchangeButtonsListeners();
			SellCarAsync().Forget();
		}

		private void OnTapCancelExchangeButtonClickCallback()
		{
			CarExchangeCancel();
		}

		private async UniTaskVoid TryAwardRewardAsync()
		{
			DailyRewardData.TypeReward typeBeforeClaim = _dailyRewardData.Type;
			int moneyBeforeClaim = _dailyRewardData.Money;
			int coinBeforeClaim = _dailyRewardData.Coin;
			int dayNumber = _dailyRewardData.Day + 1;
			await _managerUI.ShowWaitingWindow(set: true, backButtonState: true);
			FunctionStatus functionStatus = await _dailyRewardsServer.AwardRewardAsync(_dailyRewardData, _slotService, _playerDataStorage.AccountID);
			switch (functionStatus)
			{
			case FunctionStatus.AwardRewardNotEnoughSlots:
				_managerUI.ShowNotificationWindow(new LocalizedString("DailyRewards.Message.ClearSlot"), 3f, NotificationType.Error).Forget();
				break;
			default:
				ShowServerIsNotAvailableMessage();
				break;
			case FunctionStatus.Success:
				break;
			}
			_managerUI.CloseWaitingWindow();
			if (functionStatus == FunctionStatus.Success)
			{
				if (typeBeforeClaim == DailyRewardData.TypeReward.Car)
				{
					bool hadFreeSlot = !_hadFreeSlotByDay.TryGetValue(_dailyRewardData.Day, out var value) || value;
					_hadFreeSlotByDay.Remove(_dailyRewardData.Day);
					_dailyRewardAnalytic?.LogPlaced(dayNumber, typeBeforeClaim, _dailyRewardData.Car, hadFreeSlot);
					_managerUI.ShowNotificationWindow(new LocalizedString("DailyRewards.Message.CarPlased"), 3f).Forget();
				}
				else
				{
					_dailyRewardAnalytic?.LogClaimed(dayNumber, typeBeforeClaim, moneyBeforeClaim, coinBeforeClaim);
				}
			}
			Close();
		}

		private async UniTaskVoid InitUI(int day)
		{
			IReadOnlyList<DailyRewardData> rewards = _dailyRewardsServer.Rewards;
			if (day < 0 || day >= rewards.Count)
			{
				ShowServerIsNotAvailableMessage();
				Close();
				return;
			}
			_dailyRewardData = rewards[day];
			CombinedPriceView.gameObject.SetActive(_dailyRewardData.Type == DailyRewardData.TypeReward.Coin || _dailyRewardData.Type == DailyRewardData.TypeReward.Money);
			CombinedPriceView.Init(new CombinedPrice(_dailyRewardData.Coin, _dailyRewardData.Money));
			DailyRewardTypeGameObjects.UpdateState(_dailyRewardData.Type);
			if (_dailyRewardData.Type == DailyRewardData.TypeReward.Car)
			{
				CarImage.Load(PathGetterHelper.GetCarPreviewPath(_dailyRewardData.Car));
			}
			for (int i = 0; i < DayTexts.Length; i++)
			{
				DayTexts[i].text = $"{day + 1}";
			}
		}

		private async UniTaskVoid InitCarUIAsync()
		{
			_managerUI.ShowWaitingWindow(set: true, backButtonState: true);
			TapAnywhereRoot.SetActive(value: false);
			CarPanel.CarPrice = null;
			CarPanelData carPanel = CarPanel;
			carPanel.CarPrice = await _dailyRewardsServer.GetCarPriceAsync(_dailyRewardData.Day);
			_managerUI.CloseWaitingWindow();
			if (CarPanel.CarPrice == null || ((int)CarPanel.CarPrice.Coin == 0 && (int)CarPanel.CarPrice.Money == 0))
			{
				ShowServerIsNotAvailableMessage();
				Close();
			}
			else
			{
				ShowCarUI();
			}
		}

		private void ShowCarUI()
		{
			AddCarButtonsListeners();
			CarPanel.ButtonsRoot.SetActive(value: true);
			CarPanel.CombinedPriceView.Init(CarPanel.CarPrice);
			bool flag = _slotService.TryGetEmptySlot(out var _);
			if (_dailyRewardData != null && !_hadFreeSlotByDay.ContainsKey(_dailyRewardData.Day))
			{
				_hadFreeSlotByDay[_dailyRewardData.Day] = flag;
			}
			if (flag)
			{
				CarPanel.ButtonPlaceToSlot.gameObject.SetActive(value: true);
				CarPanel.ButtonGoToGarage.gameObject.SetActive(value: false);
			}
			else
			{
				CarPanel.ButtonPlaceToSlot.gameObject.SetActive(value: false);
				CarPanel.ButtonGoToGarage.gameObject.SetActive(value: true);
				_managerUI.ShowNotificationWindow(new LocalizedString("DailyRewards.Message.ClearSlot"), 3f, NotificationType.Error);
			}
		}

		private async UniTaskVoid SellCarAsync()
		{
			int carIdBeforeExchange = _dailyRewardData.Car;
			int dayNumber = _dailyRewardData.Day + 1;
			_managerUI.ShowWaitingWindow(set: true, backButtonState: true);
			FunctionStatus num = await _dailyRewardsServer.ExchangeRewardCarAsync(_dailyRewardData.Day);
			if (num != FunctionStatus.Success)
			{
				ShowServerIsNotAvailableMessage();
			}
			_managerUI.CloseWaitingWindow();
			if (num == FunctionStatus.Success)
			{
				bool hadFreeSlot = (_hadFreeSlotByDay.TryGetValue(_dailyRewardData.Day, out var value) ? value : _slotService.TryGetEmptySlot(out var _));
				_hadFreeSlotByDay.Remove(_dailyRewardData.Day);
				_dailyRewardAnalytic?.LogExchanged(dayNumber, carIdBeforeExchange, hadFreeSlot, _dailyRewardData.Money);
			}
			Close();
		}

		private void CarExchangeUIInit()
		{
			CarPanel.ButtonsRoot.SetActive(value: false);
			Sequence s = DOTween.Sequence();
			s.Append(CarPanel.IconRoot.DOScaleX(0f, 0.3f));
			s.AppendCallback(() =>
			{
				CombinedPriceView.gameObject.SetActive(value: true);
				CombinedPriceView.Init(CarPanel.CarPrice);
				DailyRewardTypeGameObjects.UpdateState(((int)CarPanel.CarPrice.Coin > 0) ? DailyRewardData.TypeReward.Coin : DailyRewardData.TypeReward.Money);
			});
			s.Append(CarPanel.IconRoot.DOScaleX(1f, 0.3f));
			s.AppendCallback(() =>
			{
				CarPanel.ButtonExchangeTapAnywhere.gameObject.SetActive(value: true);
				CarPanel.CancelExchangeCarRoot.SetActive(value: true);
				AddCarExchangeButtonsListeners();
			});
		}

		private void CarExchangeCancel()
		{
			CarPanel.ButtonExchangeTapAnywhere.gameObject.SetActive(value: false);
			RemoveCarExchangeButtonsListeners();
			Sequence s = DOTween.Sequence();
			s.Append(CarPanel.IconRoot.DOScaleX(0f, 0.3f));
			s.AppendCallback(() =>
			{
				CombinedPriceView.gameObject.SetActive(value: false);
				DailyRewardTypeGameObjects.UpdateState(_dailyRewardData.Type);
				ShowCarUI();
				RemoveCarButtonsListeners();
				CarPanel.ButtonsRoot.SetActive(value: false);
			});
			s.Append(CarPanel.IconRoot.DOScaleX(1f, 0.3f));
			s.AppendCallback(() =>
			{
				AddCarButtonsListeners();
				CarPanel.ButtonsRoot.SetActive(value: true);
			});
		}

		private void ShowServerIsNotAvailableMessage()
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("DailyRewards.Message.ServerIsNotAvailable"), 3f, NotificationType.Error);
		}
	}
}
