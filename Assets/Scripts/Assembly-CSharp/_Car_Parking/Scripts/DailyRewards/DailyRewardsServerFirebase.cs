using System;
using System.Collections.Generic;
using CPM.Account;
using CPM.Connection;
using CPM.Database;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Other;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.DailyRewards
{
	public class DailyRewardsServerFirebase : DailyRewardsServerBase, IDailyRewardsServer, IDeleteUserData
	{
		public const string GetRewardsFn = "GetRewards24_1";

		public const string AwardRewardFn = "AwardReward24_1";

		public const string CarRewardPriceFn = "GetCarPrice24_1";

		public const string SellCarFn = "ExchangeCarForMoney24_1";

		private const int TimeoutGetCarPriceAsync = 30;

		private const int TimeoutAwardRewardRequestAsync = 30;

		private const int TimeoutSellCarRequestAsync = 30;

		private const int TimeoutGetRewardsDataRequestAsync = 30;

		private readonly FirebaseWrappedFunctionFactory _factory;

		private IDatabaseSaveLoadService _databaseSaveLoadService;

		[Inject]
		public DailyRewardsServerFirebase(IConnectionManager connectionManager, CurrencyManagerController currencyManagerController, IDatabaseSaveLoadService databaseSaveLoadService, IDeleterUserDataGetter deleterUserDataGetter)
			: base(connectionManager, currencyManagerController, deleterUserDataGetter)
		{
			_factory = new FirebaseWrappedFunctionFactory();
			_databaseSaveLoadService = databaseSaveLoadService;
		}

		public override async UniTask<CombinedPrice> GetCarPriceAsync(int day)
		{
			try
			{
				FunctionCallResult<int> functionCallResult = await _factory.Create("GetCarPrice24_1", 30).CallAsyncResult<int>(day);
				if (functionCallResult.IsSuccess)
				{
					return new CombinedPrice(0, functionCallResult.Data);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message ?? "");
			}
			return new CombinedPrice(0, 0);
		}

		public override async UniTask<bool> SyncRewardsAsync()
		{
			FunctionCallResult<DailyRewardData[]> functionCallResult = await _factory.Create("GetRewards24_1", 30).CallAsyncResult<DailyRewardData[]>("");
			if (functionCallResult.IsSuccess && functionCallResult.Data != null)
			{
				for (int i = 0; i < functionCallResult.Data.Length; i++)
				{
					functionCallResult.Data[i].Day = i;
				}
				SetRewards(new List<DailyRewardData>(functionCallResult.Data));
			}
			return functionCallResult.IsSuccess;
		}

		protected override async UniTask<FunctionCallResult<AwardRewardResponse>> AwardRewardRequestAsync(DailyRewardData dailyRewardData, string accountID)
		{
			_ = 1;
			try
			{
				await _databaseSaveLoadService.SendWalletData(new WalletData(CurrencyManagerController.Money));
				FirebaseWrappedFunction firebaseWrappedFunction = _factory.Create("AwardReward24_1", 30);
				IDecoder decoder = new MemoryPackDecoder(accountID);
				return await firebaseWrappedFunction.CallAsyncResult<AwardRewardResponse>(decoder, dailyRewardData.Day, 1);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message ?? "");
			}
			return FunctionCallResult<AwardRewardResponse>.Fail(FunctionStatus.UnknownLocal);
		}

		protected override async UniTask<FunctionCallResult<ExchangeRewardCarResponse>> SellCarRequestAsync(int day)
		{
			_ = 1;
			try
			{
				await _databaseSaveLoadService.SendWalletData(new WalletData(CurrencyManagerController.Money));
				return await _factory.Create("ExchangeCarForMoney24_1", 30).CallAsyncResult<ExchangeRewardCarResponse>(day);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message ?? "");
			}
			return FunctionCallResult<ExchangeRewardCarResponse>.Fail(FunctionStatus.UnknownLocal);
		}
	}
}
