using CPM.PlayerData;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP_Registry;

public class GetFuelWindow : WindowScript
{
	public GameObject HideWhenLoadingRewarded;

	public GameObject RewardedLoading;

	public GameObject NotEnoughMoneyWindow;

	public GameObject mainContent;

	private ObscuredInt fuelPrice;

	private PlayerDataStorage playerDataStorage;

	private CurrencyManagerController _currencyManagerController;

	private FreeDriveProvider _freeDriveProvider;

	private LevelsProvider _levelsProvider;

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

	[Inject]
	public void Construct(PlayerDataStorage playerDataStorage, CurrencyManagerController currencyManagerController, FreeDriveProvider freeDriveProvider, LevelsProvider levelsProvider)
	{
		_levelsProvider = levelsProvider;
		_freeDriveProvider = freeDriveProvider;
		_currencyManagerController = currencyManagerController;
		this.playerDataStorage = playerDataStorage;
	}

	protected override void Start()
	{
		HideWhenLoadingRewarded.SetActive(value: true);
		RewardedLoading.SetActive(value: false);
		fuelPrice = PlayerPrefs.GetInt("buyFuelCount", 1) * 1000;
		fuelPrice = Mathf.Clamp(fuelPrice, 0, 10000);
		base.Start();
	}

	public void BuyFuel()
	{
		GameObject car = V.Car;
		if (car == null)
		{
			car = V.LastUsedCar;
		}
		if ((bool)car)
		{
			AdvancedInt price = new AdvancedInt(fuelPrice);
			_currencyManagerController.TryMoneySpend(out var _, price, () =>
			{
				CoreGameManager coreGameManager = V.CoreGameManager;
				ObscuredInt obscuredInt = fuelPrice;
				coreGameManager.SetMessage("-" + obscuredInt.ToString(), 2f);
				car.GetComponent<VehicleData>().CarChildsReferences.FuelTank.FullFuelImmediately();
				_levelsProvider.LevelScript.Resume();
				int num = PlayerPrefs.GetInt("buyFuelCount", 1);
				num++;
				PlayerPrefs.SetInt("buyFuelCount", num);
				fuelPrice = num * 1000;
				_freeDriveProvider.FreeDriveControl.HideActionButton();
			});
		}
	}

	public void WatchAndGetFuel()
	{
		HideWhenLoadingRewarded.SetActive(value: false);
		RewardedLoading.SetActive(value: true);
	}

	public void OnOpenGetFuel()
	{
		HideWhenLoadingRewarded.SetActive(value: true);
		RewardedLoading.SetActive(value: false);
		NotEnoughMoneyWindow.SetActive(value: false);
		mainContent.SetActive(value: true);
	}

	public void ShowNotEnoughWindow()
	{
		HideWhenLoadingRewarded.SetActive(value: true);
		RewardedLoading.SetActive(value: false);
		mainContent.SetActive(value: false);
		NotEnoughMoneyWindow.SetActive(value: true);
	}

	public void OnRewardedFinished()
	{
		GameObject gameObject = V.Car;
		if (gameObject == null)
		{
			gameObject = V.LastUsedCar;
		}
		if ((bool)gameObject && gameObject.TryGetComponent<VehicleData>(out var component) && (bool)component.CarChildsReferences.FuelTank)
		{
			component.CarChildsReferences.FuelTank.FullFuelImmediately();
		}
		_levelsProvider.LevelScript.Resume();
		_managerUI.HideActionButton();
	}

	public void OnRewardedClosed()
	{
		_freeDriveProvider.FreeDriveControl.ShowGetFuelMenu();
	}
}
