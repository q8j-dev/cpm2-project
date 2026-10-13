using System;
using System.Collections.Generic;
using CPM.Account;
using CPM.Connection;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.DailyRewards
{
	public abstract class DailyRewardsServerBase : IDailyRewardsServer, IDeleteUserData
	{
		private const float DelayToCheckHash = 3f;

		private List<DailyRewardData> _rewards = new List<DailyRewardData>();

		private readonly CurrencyManagerController _currencyManagerController;

		private readonly IConnectionManager _connectionManager;

		public IReadOnlyList<DailyRewardData> Rewards => _rewards;

		public int Count => _rewards.Count;

		public event Action OnDailyRewardsChanged;

		public DailyRewardsServerBase()
		{
		}

		public DailyRewardsServerBase(IConnectionManager connectionManager, CurrencyManagerController currencyManagerController, IDeleterUserDataGetter deleterUserDataGetter)
		{
			_connectionManager = connectionManager;
			_currencyManagerController = currencyManagerController;
			deleterUserDataGetter.Add(this);
		}

		public abstract UniTask<CombinedPrice> GetCarPriceAsync(int day);

		public abstract UniTask<bool> SyncRewardsAsync();

		protected abstract UniTask<FunctionCallResult<AwardRewardResponse>> AwardRewardRequestAsync(DailyRewardData dailyRewardData, string accountID);

		protected abstract UniTask<FunctionCallResult<ExchangeRewardCarResponse>> SellCarRequestAsync(int day);

		public void DeleteData()
		{
			Clear();
		}

		public void Clear()
		{
			_rewards.Clear();
		}

		public void SetRewards(List<DailyRewardData> rewards)
		{
			if (rewards == null)
			{
				rewards = new List<DailyRewardData>();
			}
			_rewards = rewards;
			OnDailyRewardsChanged?.Invoke();
		}

		public async UniTask<FunctionStatus> AwardRewardAsync(DailyRewardData dailyRewardData, ISlotService slotService, string accountID)
		{
			if (dailyRewardData == null || dailyRewardData.Status != DailyRewardData.StatusType.Opened)
			{
				return FunctionStatus.UnknownLocal;
			}
			if (!(await CheckInternetConnectionAndHash()))
			{
				return FunctionStatus.UnknownLocal;
			}
			if (dailyRewardData.Type == DailyRewardData.TypeReward.Car && !slotService.TryGetEmptySlot(out var _))
			{
				return FunctionStatus.AwardRewardNotEnoughSlots;
			}
			FunctionCallResult<AwardRewardResponse> functionCallResult = await AwardRewardRequestAsync(dailyRewardData, accountID);
			if (functionCallResult.Status == FunctionStatus.Success && functionCallResult.Data == null)
			{
				functionCallResult = FunctionCallResult<AwardRewardResponse>.Fail(FunctionStatus.UnknownLocal);
			}
			if (functionCallResult.Status == FunctionStatus.Success)
			{
				if (functionCallResult.Data.NewMoney.HasValue)
				{
					int value = functionCallResult.Data.NewMoney.Value;
					_currencyManagerController.SetMoney(new AdvancedInt(value));
				}
				if (functionCallResult.Data.NewCoins.HasValue)
				{
					CurrencyManagerController.SetCoins(new AdvancedInt(functionCallResult.Data.NewCoins.Value));
					CurrencyManagerController.UpdateCoinsVisuals();
				}
				if (functionCallResult.Data.NewCar.HasValue && functionCallResult.Data.NewCar >= 0)
				{
					if (!slotService.TryGetEmptySlot(out var res2))
					{
						return FunctionStatus.AwardRewardNotEnoughSlots;
					}
					dailyRewardData.Car = functionCallResult.Data.NewCar.Value;
					if (V.CoreGameManager.AddPurchasedCarToSlot(res2, functionCallResult.Data.NewCar.Value, null, null, functionCallResult.Data.CarInstanceId) == null)
					{
						return FunctionStatus.UnknownError;
					}
					V.CoreGameManager.LoadBoughtCars();
					_ = RateAppHelper.RateAppAsync();
				}
				dailyRewardData.Status = DailyRewardData.StatusType.Rewarded;
			}
			dailyRewardData.OnDataChangedSend();
			return functionCallResult.Status;
		}

		public async UniTask<FunctionStatus> ExchangeRewardCarAsync(int day)
		{
			if (!(await CheckInternetConnectionAndHash()))
			{
				return FunctionStatus.UnknownLocal;
			}
			FunctionCallResult<ExchangeRewardCarResponse> functionCallResult = await SellCarRequestAsync(day);
			if (functionCallResult.IsFail)
			{
				return functionCallResult.Status;
			}
			if (functionCallResult.IsSuccess && functionCallResult.Data != null)
			{
				int money = 0;
				if (functionCallResult.Data.NewMoney.HasValue)
				{
					int value = functionCallResult.Data.NewMoney.Value;
					money = value - (int)CurrencyManagerController.Money;
					_currencyManagerController.SetMoney(new AdvancedInt(value));
				}
				int coin = 0;
				if (functionCallResult.Data.NewCoins.HasValue)
				{
					int value2 = functionCallResult.Data.NewCoins.Value;
					coin = value2 - (int)CurrencyManagerController.Coins;
					CurrencyManagerController.SetCoins(new AdvancedInt(value2));
					CurrencyManagerController.UpdateCoinsVisuals();
				}
				if (day >= 0 && day < _rewards.Count)
				{
					_rewards[day].Status = DailyRewardData.StatusType.Rewarded;
					_rewards[day].Money = money;
					_rewards[day].Coin = coin;
					_rewards[day].Car = -1;
					_rewards[day].OnDataChangedSend();
				}
			}
			return functionCallResult.Status;
		}

		private async UniTask<bool> CheckInternetConnectionAndHash()
		{
			if (!InternetHelper.HasInternetConnection())
			{
				return false;
			}
			_connectionManager.CorrectHash.Get(out var value);
			if (!value)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(3.0));
				_connectionManager.CorrectHash.Get(out value);
			}
			if (!value)
			{
				return false;
			}
			return true;
		}
	}
}
