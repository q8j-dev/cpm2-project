using System;
using System.Collections.Generic;
using System.Threading;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.PlayerProperty.Business
{
	public class BusinessProfitControl : IBusinessProfitControl, IInitializable, IDisposable
	{
		private const string FastFoodBuildingMoneyHas = "FastFoodBuildingMoneyHas";

		private readonly IDictionary<int, ObscuredInt> _profits = new Dictionary<int, ObscuredInt>();

		private readonly IBusinessService _businessService;

		private readonly IEarnMoneyService _earnMoneyService;

		private readonly FreeDriveControl _freeDriveControl;

		private readonly CurrencyManagerController _currencyManagerController;

		private CancellationTokenSource _cts;

		[Inject]
		public BusinessProfitControl(IBusinessService businessService, IEarnMoneyService earnMoneyService, FreeDriveControl freeDriveControl, CurrencyManagerController currencyManagerController)
		{
			_businessService = businessService;
			_earnMoneyService = earnMoneyService;
			_freeDriveControl = freeDriveControl;
			_currencyManagerController = currencyManagerController;
		}

		public void Initialize()
		{
			_cts = new CancellationTokenSource();
			for (int i = 0; i < 10; i++)
			{
				if (_businessService.HasBought(i))
				{
					_profits[i] = ObscuredPrefs.GetInt(GetProfitKey(i));
				}
			}
			UpdateAsync(_cts.Token).Forget();
			_freeDriveControl.OnFastFoodBuildBought += OnStatusChanged;
		}

		public void Dispose()
		{
			_freeDriveControl.OnFastFoodBuildBought -= OnStatusChanged;
			CommonExtensions.CancelAndDisposeToken(ref _cts);
		}

		public int GetProfit(int businessId)
		{
			if (!_profits.TryGetValue(businessId, out var value))
			{
				return 0;
			}
			return value;
		}

		public void ClearProfit(int businessId)
		{
			if (_profits.ContainsKey(businessId))
			{
				_profits[businessId] = 0;
			}
		}

		public void Save()
		{
			foreach (int key in _profits.Keys)
			{
				ObscuredPrefs.SetInt(GetProfitKey(key), _profits[key]);
			}
		}

		public void OnMoneyCollect(int buildingId)
		{
			int profit = GetProfit(buildingId);
			_currencyManagerController.AddMoney(new AdvancedInt(profit));
			ClearProfit(buildingId);
			Save();
			ManagerUIHelper.SetMessage("+" + profit, 3f);
		}

		private string GetProfitKey(int businessId)
		{
			return "FastFoodBuildingMoneyHas" + businessId;
		}

		private async UniTask UpdateAsync(CancellationToken cancellationToken)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(3.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			while (!cancellationToken.IsCancellationRequested)
			{
				await UniTask.Delay(TimeSpan.FromSeconds((float)_earnMoneyService.BusinessProfitIntervalInSec), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
				foreach (int item in new List<int>(_profits.Keys))
				{
					if ((int)_profits[item] < (int)_earnMoneyService.BusinessProfitMaxPerSession)
					{
						IDictionary<int, ObscuredInt> profits = _profits;
						int key = item;
						profits[key] = (int)profits[key] + 1;
					}
				}
			}
		}

		private void OnStatusChanged(int id, bool on)
		{
			if (on && !_profits.ContainsKey(id))
			{
				_profits[id] = 0;
				Save();
			}
		}
	}
}
