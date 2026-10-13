using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement;

public class CoinPurchaseWindow : MonoBehaviour
{
	public GameObject coinPurchaseWindowFree;

	private CurrencyManagerController _currencyManagerController;

	private ParkingMenuUI _parkingMenuUI;

	private OptionalDependency<ParkingMenuUI> _parkingMenuUIDep;

	private LevelsProvider _levelsProvider;

	private FreeDriveProvider _freeDriveProvider;

	public static event VisualHandler OnCoinsChanged;

	[Inject]
	public void Construct(CurrencyManagerController currencyManagerController, LevelsProvider levelsProvider, FreeDriveProvider freeDriveProvider, OptionalDependency<ParkingMenuUI> parkingMenuUI)
	{
		_freeDriveProvider = freeDriveProvider;
		_levelsProvider = levelsProvider;
		_parkingMenuUIDep = parkingMenuUI;
		_currencyManagerController = currencyManagerController;
	}

	private void Awake()
	{
		_parkingMenuUI = _parkingMenuUIDep?.Value;
		CurrencyManagerController.SetCoins(new AdvancedInt(Mathf.Min(CurrencyManagerController.Coins, 500000)));
		_freeDriveProvider.CoinPurchaseWindow = this;
		UpdateCoinLabels();
		if ((bool)coinPurchaseWindowFree)
		{
			coinPurchaseWindowFree.SetActive(value: false);
		}
		_currencyManagerController.CoinsChanged += UpdateCoinLabels;
	}

	private void OnDestroy()
	{
		_freeDriveProvider.CoinPurchaseWindow = null;
		_currencyManagerController.CoinsChanged -= UpdateCoinLabels;
		OnCoinsChanged = null;
	}

	public void OpenCoinPurchaseWindow(string labelText)
	{
		if (FlagsStorage.IsFreeDriveMode)
		{
			if ((bool)_parkingMenuUI)
			{
				_parkingMenuUI.Hide();
			}
			_levelsProvider.LevelScript.CleanScreen();
			coinPurchaseWindowFree.SetActive(value: true);
		}
		else if (FlagsStorage.IsMainMenu)
		{
			V.CoreGameManager.menuControl.ShowCoinPurchase();
		}
		UpdateCoinPrice();
	}

	public void CloseCoinPurchaseWindowFree()
	{
		if (FlagsStorage.IsFreeDriveMode)
		{
			if ((bool)_parkingMenuUI)
			{
				_parkingMenuUI.Show();
			}
			_levelsProvider.LevelScript.UnCleanScreen();
			coinPurchaseWindowFree.SetActive(value: false);
		}
	}

	public void UpdateCoinPrice()
	{
		UpdateCoinLabels();
	}

	public void UpdateCoinLabels()
	{
		try
		{
			OnCoinsChanged?.Invoke(this);
		}
		catch
		{
		}
	}

	public void OnPurchaseDone()
	{
		UpdateCoinLabels();
	}
}
