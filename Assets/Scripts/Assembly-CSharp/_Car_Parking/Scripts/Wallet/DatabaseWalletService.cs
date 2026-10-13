using System;
using CPM.PlayerData;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using Newtonsoft.Json;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Scripts.Backend;
using _Car_Parking.Scripts.Other.Cypher;
using _Car_Parking.Scripts.PurchaseSystem;

namespace _Car_Parking.Scripts.Wallet
{
	public class DatabaseWalletService : IWalletService
	{
		[UnityEngine.Scripting.Preserve]
		private class MoneyResponse
		{
			[JsonProperty("code")]
			public int Code = default;

			[JsonProperty("data")]
			public string Data = default;
		}

		[UnityEngine.Scripting.Preserve]
		private class CircuitRaceRewardResponse
		{
			[JsonProperty("coins")]
			public int Coins = default;

			[JsonProperty("token")]
			public string Token = default;

			[UnityEngine.Scripting.Preserve]
			public CircuitRaceRewardResponse()
			{
			}
		}

		public const string SpendCoinsFn = "SpendCoins24_1";

		public const string PurchaseInAppItemFn = "PurchaseInAppItem24_1";

		public const string RestorePurchaseFn = "RestorePurchase24_1";

		public const string GetCoinsFn = "GetCoins24_1";

		public const string GetMoneyFn = "GetMoney24_1";

		public const string SetDragRacingFn = "progress-service/fb/v1/racing/drag/update-client";

		public const string SetCircuitRacingFn = "progress-service/fb/v1/racing/circuit/update-client";

		private const string PurchaseInAppItemSteamFn = "PurchaseInAppItemSteam24_1";

		private const string PurchaseInAppItemSteamSandboxFn = "PurchaseInAppItemSteamSandbox24_1";

		private FirebaseWrappedFunctionFactory _factory;

		private PlayerDataStorage _playerDataStorage;

		[Inject]
		public DatabaseWalletService(PlayerDataStorage playerDataStorage)
		{
			_playerDataStorage = playerDataStorage;
			_factory = new FirebaseWrappedFunctionFactory();
		}

		public async UniTask<WalletOperationResult> SyncCoinsFromDatabase()
		{
			IDecoder decoder = new AesDecoder();
			FunctionCallResult<int> functionCallResult = await _factory.Create("GetCoins24_1").CallAsyncResult<int>(decoder);
			if (functionCallResult.IsSuccess)
			{
				return new WalletOperationResult(1, functionCallResult.Data);
			}
			return new WalletOperationResult(0, 0);
		}

		public async UniTask<WalletOperationResult> SyncMoneyFromDatabase()
		{
			FunctionCallResult<MoneyResponse> functionCallResult = await _factory.Create("GetMoney24_1").CallAsyncResult<MoneyResponse>();
			if (functionCallResult.IsFail || functionCallResult.Data == null || string.IsNullOrEmpty(functionCallResult.Data.Data))
			{
				return new WalletOperationResult(0, 0);
			}
			try
			{
				Result<int, string> result = new AesDecoder().Decode<int>(functionCallResult.Data.Data);
				if (result.IsSuccess)
				{
					return new WalletOperationResult(1, result.Value);
				}
			}
			catch (Exception)
			{
			}
			return new WalletOperationResult(0, 0);
		}

		public async UniTask<WalletOperationResult> TrySpendCoins(SpendCoinsType spendCoinsType, ObscuredInt value)
		{
			string data = WalletCypher.Encode(((int)value).ToString(), value.ToString());
			FunctionCallResult<int> functionCallResult = await _factory.Create("SpendCoins24_1").CallAsyncResult<int>(new AesDecoder(), data);
			if (functionCallResult.IsFail)
			{
				return new WalletOperationResult(0, 0);
			}
			return new WalletOperationResult(1, functionCallResult.Data);
		}

		public async UniTask<WalletOperationResult> PurchaseItem(ItemData data)
		{
			return await OperationAsync(data, "PurchaseInAppItem24_1");
		}

		public async UniTask<WalletOperationResult> PurchaseItemSteam(ItemData data)
		{
			return await OperationAsync(data, "PurchaseInAppItemSteam24_1");
		}

		public async UniTask<WalletOperationResult> RestorePurchase(ItemData data)
		{
			return await OperationAsync(data, "RestorePurchase24_1");
		}

		private async UniTask<WalletOperationResult> OperationAsync(ItemData data, string functionName)
		{
			string data2 = WalletCypher.Encode(JsonConvert.SerializeObject(data), "1");
			FunctionCallResult<string[]> functionCallResult = await _factory.Create(functionName).CallAsyncResult<string[]>(data2);
			if (functionCallResult.IsFail)
			{
				return new WalletOperationResult(ServerPurchaseStatusMapper.ToWalletFailStatusCode(functionCallResult.Status), 0);
			}
			string[] data3 = functionCallResult.Data;
			int result = 0;
			if (data3 != null && data3.Length >= 1 && !string.IsNullOrEmpty(data3[0]))
			{
				int.TryParse(WalletCypher.Decode(data3[0]), out result);
			}
			WalletOperationResult walletOperationResult = new WalletOperationResult(1, result);
			if (data3 != null && data3.Length >= 2 && !string.IsNullOrEmpty(data3[1]))
			{
				walletOperationResult.Data = WalletCypher.Decode(data3[1]);
			}
			return walletOperationResult;
		}

		public async UniTask<WalletOperationResult> GetCoinForDrag(string data)
		{
			string data2 = WalletCypher.Encode(data, "1");
			FunctionCallResult<int> functionCallResult = await _factory.Create("progress-service/fb/v1/racing/drag/update-client").CallAsyncResult<int>(new AesDecoder(), data2);
			if (functionCallResult.IsFail)
			{
				return new WalletOperationResult(0, 0);
			}
			return new WalletOperationResult(1, functionCallResult.Data);
		}

		public async UniTask<WalletOperationResult> GetCircuitRaceReward(string data, string key)
		{
			string data2 = WalletCypher.EncodeWithCustomToken(data, key);
			FunctionCallResult<CircuitRaceRewardResponse> functionCallResult = await _factory.Create("progress-service/fb/v1/racing/circuit/update-client").CallAsyncResult<CircuitRaceRewardResponse>(data2);
			if (functionCallResult.IsFail || functionCallResult.Data == null)
			{
				return new WalletOperationResult(0, 0);
			}
			return new WalletOperationResult(1, functionCallResult.Data.Coins, functionCallResult.Data.Token);
		}
	}
}
