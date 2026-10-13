using System;
using CPM.Account;
using CPM.DailyTasks;
using CPM.Database;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Wallet;

public class CurrencyManagerController
{
	private const int DefaultMoneyValue = 25000;

	private const int MaxMoney = 50000000;

	public const string CoinPrefsKey = "coins1";

	public const string MoneyPrefsKey = "money";

	public static CurrencyManagerController Instance;

	public static Action<float> MoneyChanged;

	private IWalletService _databaseChangeService;

	private IAccountController _accountController;

	private IDatabaseSaveLoadService _databaseSaveLoadService;

	private int _lastPushedMoney = -1;

	private bool WaitingInProgress { get; set; }

	private bool SpendCoinsInProgress { get; set; }

	public static ObscuredInt Coins
	{
		get
		{
			GetterCoinsManager.Get4LocalPrefs().Get(out int value);
			return value;
		}
	}

	public static ObscuredInt Money => (int)ObscuredPrefs.GetFloat("money", 25000f);

	public event Action CoinsChanged;

	public event Action NotEnoughMoney;

	public event Action NotEnoughCoins;

	public event Action NoInternetConnection;

	public event Action<bool, bool> ShowWaitingWindow;

	public static event VisualHandler OnMoneyChanged;

	[Inject]
	public void Construct(DailyTasksManager dailyTasksManager, IWalletService walletDatabaseChangeService, IAccountController accountController, IDatabaseSaveLoadService databaseSaveLoadService)
	{
		_databaseSaveLoadService = databaseSaveLoadService;
		_accountController = accountController;
		_databaseChangeService = walletDatabaseChangeService;
		dailyTasksManager.SetResourcesController(this);
		Instance = this;
	}

	public static void SetMoneyPrefs(AdvancedInt value)
	{
		value.Get(out int value2);
		ObscuredPrefs.SetFloat("money", value2);
	}

	public void SetMoney(AdvancedInt value)
	{
		value.Get(out int value2);
		value2 = Mathf.Abs(value2);
		value2 = Mathf.Clamp(value2, 0, 50000000);
		SetMoneyPrefs(new AdvancedInt(value2));
		OnMoneyChanged?.Invoke(this);
		UpdateLabelsEvents();
	}

	public static void SetCoins(AdvancedInt value, GetterCoinsManager.Source source = GetterCoinsManager.Source.Default)
	{
		GetterCoinsManager.Set2LocalPrefs(value, source);
	}

	public void AddMoney(AdvancedInt value)
	{
		if (value < 0)
		{
			value = new AdvancedInt(0);
		}
		AdvancedInt advancedInt = new AdvancedInt(Money) + value;
		advancedInt.Clamp(0, 50000000);
		SetMoneyPrefs(advancedInt);
		UpdateLabelsEvents();
	}

	public async UniTask<bool> SyncMoneyFromDatabase()
	{
		WalletOperationResult walletOperationResult = await _databaseChangeService.SyncMoneyFromDatabase();
		if (walletOperationResult.IsSuccess())
		{
			SetMoney(new AdvancedInt(walletOperationResult.Value));
			CoinsChanged?.Invoke();
		}
		return walletOperationResult.IsSuccess();
	}

	public async UniTask<bool> UpdateCoinsFromDatabase(GetterCoinsManager.Source source = GetterCoinsManager.Source.Default)
	{
		if (!_accountController.Authorized)
		{
			return false;
		}
		WalletOperationResult walletOperationResult = await _databaseChangeService.SyncCoinsFromDatabase();
		bool num = walletOperationResult.IsSuccess();
		if (num)
		{
			SetCoins(new AdvancedInt(walletOperationResult.Value), source);
			CoinsChanged?.Invoke();
		}
		return num;
	}

	public AdvancedBoolean IsEnoughCoins(int price, bool withMessage = false)
	{
		bool num = (int)Coins >= price;
		if (!num & withMessage)
		{
			NotEnoughCoins?.Invoke();
		}
		return new AdvancedBoolean(num);
	}

	public AdvancedBoolean IsEnoughMoney(int price, bool withMessage = false)
	{
		bool num = (int)Money >= price;
		if (!num & withMessage)
		{
			NotEnoughMoney?.Invoke();
		}
		return new AdvancedBoolean(num);
	}

	public AdvancedBoolean IsMoneyEnough(int price)
	{
		float f = (int)Money;
		f = Mathf.Abs(f);
		price = Mathf.Abs(price);
		return new AdvancedBoolean((float)price <= f);
	}

	private async UniTask<AdvancedBoolean> TrySpendCoinsOperation(SpendCoinsType spendCoinsType, AdvancedInt price, Action success, Action fail = null)
	{
		price.Get(out int intPrice);
		if (intPrice == 0)
		{
			success?.Invoke();
			return new AdvancedBoolean(v: true);
		}
		if ((int)Coins < intPrice)
		{
			fail?.Invoke();
			NotEnoughCoins?.Invoke();
			return new AdvancedBoolean(v: false);
		}
		if (!InternetHelper.HasInternetConnection())
		{
			Debug.LogError("No internet connection");
			fail?.Invoke();
			NoInternetConnection?.Invoke();
			return new AdvancedBoolean(v: false);
		}
		ShowWaitingWindowMethod();
		if (await _accountController.CheckShouldLogOutOnThisDevice(CheckLoggedDeviceInFreeType.TrySpendCoins))
		{
			return new AdvancedBoolean(v: false);
		}
		WalletOperationResult walletOperationResult = await _databaseChangeService.TrySpendCoins(spendCoinsType, intPrice);
		bool num = walletOperationResult.IsSuccess();
		if (num)
		{
			success?.Invoke();
			SetCoins(new AdvancedInt(walletOperationResult.Value));
			CoinsChanged?.Invoke();
		}
		else
		{
			fail?.Invoke();
			NotEnoughCoins?.Invoke();
		}
		CloseWaitingWindowMethod();
		return new AdvancedBoolean(num);
	}

	public async UniTask<AdvancedBoolean> TryCoinsSpend(SpendCoinsType spendCoinsType, AdvancedInt price, Action success, Action fail = null, bool allowZero = true)
	{
		if (SpendCoinsInProgress)
		{
			Debug.LogError("Can't spend coins. SpendCoinsInProgress.");
			fail?.Invoke();
			return new AdvancedBoolean(v: false);
		}
		SpendCoinsInProgress = true;
		bool result = false;
		try
		{
			if (!allowZero && price <= 0)
			{
				fail?.Invoke();
				return new AdvancedBoolean(v: false);
			}
			(await TrySpendCoinsOperation(spendCoinsType, price, success, fail)).Get(out result);
		}
		catch (Exception ex)
		{
			Debug.LogError("SpendCoins Error:" + ex);
		}
		finally
		{
			SpendCoinsInProgress = false;
		}
		return new AdvancedBoolean(result);
	}

	private void PushMoneyAfterSpend()
	{
		if (_accountController != null && _accountController.Authorized)
		{
			int num = Money;
			if (num != _lastPushedMoney)
			{
				_lastPushedMoney = num;
				_databaseSaveLoadService.SendWalletData(new WalletData(num)).Forget();
			}
		}
	}

	public void TryMoneySpend(out bool result, AdvancedInt price, bool showShop, bool spendAnyway, Action success = null, Action fail = null)
	{
		int num = Money;
		price.Get(out int value);
		if (value < 0)
		{
			value = 100000000;
		}
		int num2 = num;
		num2 -= value;
		num2 = Mathf.Clamp(num2, 0, 50000000);
		result = value <= num;
		if (result)
		{
			success?.Invoke();
			SetMoney(new AdvancedInt(num2));
			PlayerDatas.PriceSoldLogic(new AdvancedInt(value));
			PushMoneyAfterSpend();
		}
		else if (spendAnyway)
		{
			success?.Invoke();
			SetMoney(new AdvancedInt(num2));
			PlayerDatas.PriceSoldLogic(new AdvancedInt(num));
			PushMoneyAfterSpend();
			result = true;
		}
		else
		{
			if (showShop)
			{
				NotEnoughMoney?.Invoke();
			}
			fail?.Invoke();
		}
	}

	public void TryMoneySpend(out bool result, AdvancedInt price, Action success = null, Action fail = null, bool sendNotEnoughMoneyIfNeed = true)
	{
		int num = Money;
		price.Get(out int value);
		if (value < 0)
		{
			value = 100000000;
		}
		AdvancedInt advancedInt = new AdvancedInt(num);
		advancedInt -= value;
		advancedInt.Clamp(0, 50000000);
		result = value <= num;
		if (result)
		{
			success?.Invoke();
			SetMoney(advancedInt);
			PlayerDatas.PriceSoldLogic(new AdvancedInt(value));
			PushMoneyAfterSpend();
		}
		else
		{
			fail?.Invoke();
			if (sendNotEnoughMoneyIfNeed)
			{
				NotEnoughMoney?.Invoke();
			}
		}
	}

	public void ShowWaitingWindowMethod(bool showBackButton = true)
	{
		if (WaitingInProgress)
		{
			Debug.LogWarning("Waiting already in progress");
			return;
		}
		WaitingInProgress = true;
		ShowWaitingWindow?.Invoke(arg1: true, showBackButton);
	}

	public void CloseWaitingWindowMethod()
	{
		if (!WaitingInProgress)
		{
			Debug.LogWarning("Waiting is closed");
		}
		WaitingInProgress = false;
		ShowWaitingWindow?.Invoke(arg1: false, arg2: true);
	}

	public void AddCoin(int coins)
	{
	}

	public void ShowNotEnoughCoinsWindow()
	{
		NotEnoughCoins?.Invoke();
	}

	public void ShowNotEnoughMoneyWindow()
	{
		NotEnoughMoney?.Invoke();
	}

	public void UpdateLabelsEvents()
	{
		MoneyChanged?.Invoke((int)Money);
		try
		{
			OnMoneyChanged?.Invoke(this);
		}
		catch (Exception message)
		{
			Debug.LogWarning(message);
		}
	}

	public static void UpdateCoinsVisuals()
	{
		Instance.CoinsChanged?.Invoke();
	}

	public void UpdateCoinsAnimated()
	{
		CoinsChanged?.Invoke();
	}
}
