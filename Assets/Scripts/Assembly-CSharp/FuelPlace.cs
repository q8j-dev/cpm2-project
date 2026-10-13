using System;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Providers;

public class FuelPlace : MonoBehaviour
{
	public GasPistolMove gasPistolMove;

	public GameObject FuelIcon;

	public MeshRenderer mr1;

	public GameObject fuelIconMAP;

	private MeshRenderer mr2;

	private IDisposable _carSpeedCheck;

	private GlobalCollectionsProvider _collectionsProvider;

	private const int MaxCarSpeedToShowWindow = 2;

	[Inject]
	public void Construct(GlobalCollectionsProvider collectionsProvider)
	{
		_collectionsProvider = collectionsProvider;
	}

	private void Awake()
	{
		if ((bool)fuelIconMAP)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(fuelIconMAP);
			gameObject.transform.position = base.transform.position;
			gameObject.transform.eulerAngles = new Vector3(90f, 0f, 0f);
			_collectionsProvider.RegisterMapIcon(gameObject.transform);
		}
	}

	private void Start()
	{
		mr2 = FuelIcon.GetComponent<MeshRenderer>();
	}

	public void OnFuelExit()
	{
		_carSpeedCheck?.Dispose();
		gasPistolMove.StopFillFuel(full: false);
		Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
		{
			EnableAfterShow();
		}).AddTo(this);
	}

	public void OnFuelExitMultplayerCar()
	{
		gasPistolMove.StopFillFuelMultiplayerCar();
		Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
		{
			EnableAfterShow();
		}).AddTo(this);
	}

	public void OnFuelEnter(GameObject car, bool multiplayerCar)
	{
		mr1.enabled = false;
		mr2.enabled = false;
		if (!car.TryGetComponent<VehicleData>(out var carData))
		{
			return;
		}
		_carSpeedCheck?.Dispose();
		_carSpeedCheck = Observable.Timer(TimeSpan.FromSeconds(0.5)).Repeat().Subscribe((long _) =>
		{
			if (carData.GetPowertrain.SpeedKmh < 2f)
			{
				OnCarEnter(carData, multiplayerCar);
			}
		})
			.AddTo(this);
	}

	private void OnCarEnter(VehicleData vehicleData, bool multiplayerCar)
	{
		_carSpeedCheck?.Dispose();
		if (FlagsStorage.IsPhotoMode)
		{
			return;
		}
		if (!multiplayerCar)
		{
			if (!FlagsStorage.IsPassenger && !FlagsStorage.OutOfCar)
			{
				gasPistolMove.StartFillFuel();
			}
			else if (vehicleData.VehicleMembersController.IsPassenger(V.DriverPerson))
			{
				gasPistolMove.StartFillFuelMultiPlayerCar(vehicleData);
			}
		}
		else
		{
			FuelTankEntity fuelTank = vehicleData.CarChildsReferences.FuelTank;
			if (fuelTank.TankCapacity - fuelTank.CurrentFuel > 1f)
			{
				gasPistolMove.StartFillFuelMultiPlayerCar(vehicleData);
			}
		}
	}

	private void EnableAfterShow()
	{
		mr1.enabled = true;
		mr2.enabled = true;
	}
}
