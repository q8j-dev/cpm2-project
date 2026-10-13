using System;
using Obi;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Providers;

public class GasPistolMove : MonoBehaviour
{
	public bool move;

	public bool NowFilling;

	public GameObject FuelingCar;

	public Vector3 initPos;

	public Vector3 initEuler;

	public Vector3 targetDistace;

	public AllwaysLookAtCam lookAtCam;

	public Transform CarFuelPlace;

	private float moveSpeed = 2f;

	public bool nearCarFuelSlot;

	public bool CarClose;

	public Transform ropeBegin;

	public Transform ropeEnd;

	public GameObject multiplayerCar;

	private ObiRopeExtrudedRenderer obiRopeRenderer;

	private const string FuelEvent = "Play_SFX_Fuel";

	private const string FuelStopEvent = "Stop_SFX_Fuel";

	private const float ArrivedToFuelSlotDistance = 0.05f;

	private bool _isFuelPlaying;

	private FuelTankEntity _fuelWindowTank;

	private GlobalCollectionsProvider _collectionsProvider;

	private FreeDriveProvider _freeDriveProvider;

	private ObiSolverProvider _obiSolverProvider;

	private IDisposable _enableAfterDisposable;

	private IDisposable _checkFullFuelMultiplayerCarTick;

	private Vector3 HomePosition => initPos + V.GlobalOffset;

	private ObiSolver Solver => _obiSolverProvider.ObiSolver;

	private ObiFixedUpdater ObiFixed => _obiSolverProvider.ObiFixed;

	[Inject]
	private void Construct(GlobalCollectionsProvider collectionsProvider, FreeDriveProvider freeDriveProvider, ObiSolverProvider obiSolverProvider)
	{
		_freeDriveProvider = freeDriveProvider;
		_collectionsProvider = collectionsProvider;
		_obiSolverProvider = obiSolverProvider;
	}

	private void Start()
	{
		if (_collectionsProvider == null || _obiSolverProvider == null)
		{
			return;
		}
		obiRopeRenderer = V.SceneControl.MainRope.GetComponent<ObiRopeExtrudedRenderer>();
		initPos = base.transform.position - V.GlobalOffset;
		targetDistace = HomePosition;
		initEuler = base.transform.eulerAngles;
		Observable.Timer(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
		{
			CheckDistance();
		}).AddTo(this);
		lookAtCam.enabled = false;
		if (!FlagsStorage.SolverCheckStarted)
		{
			Observable.Timer(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				CheckAllGasPistols();
			}).AddTo(this);
			FlagsStorage.SolverCheckStarted = true;
		}
		_collectionsProvider.RegisterGasPistolMove(this);
		ropeBegin.transform.parent = base.transform.parent;
	}

	private void OnDestroy()
	{
		_collectionsProvider?.UnregisterGasPistolMove(this);
	}

	private void CheckAllGasPistols()
	{
		if (!Solver)
		{
			return;
		}
		foreach (GasPistolMove allGasPistolMove in _collectionsProvider.GetAllGasPistolMoves())
		{
			Solver.enabled = false;
			allGasPistolMove.enabled = allGasPistolMove.CarClose;
			if (allGasPistolMove.CarClose)
			{
				if (CPMGraphicsSettingsManager.Instance.Quality != 0)
				{
					Solver.enabled = true;
				}
				break;
			}
		}
		if ((bool)V.Car && Solver.enabled && V.Car.GetComponent<Rigidbody>().velocity.magnitude > 20f)
		{
			Solver.enabled = false;
		}
		ObiFixed.enabled = Solver.enabled;
		if (!Solver.enabled)
		{
			V.SceneControl.MainRope.gameObject.SetActive(!Solver.enabled);
		}
		V.SceneControl.MainRope.GetComponent<MeshRenderer>().enabled = Solver.enabled;
	}

	private void CheckDistance()
	{
		if (FlagsStorage.IsLevelScene)
		{
			return;
		}
		if ((bool)V.Car || (bool)multiplayerCar)
		{
			float num = Vector3.Distance(base.transform.position, targetDistace);
			move = num >= 0.03f;
			if (NowFilling)
			{
				nearCarFuelSlot = !move;
			}
		}
		if (Vector3.Distance(base.transform.position, V.CoreGameManager.transform.position) > 50f)
		{
			if (CarClose)
			{
				V.SceneControl.ropeEnd.transform.parent = V.GlobalPoint;
			}
			CarClose = false;
			return;
		}
		if (!CarClose)
		{
			obiRopeRenderer.enabled = false;
			V.SceneControl.ropeBegin.position = ropeBegin.transform.position;
			V.SceneControl.ropeEnd.position = ropeEnd.transform.position;
			V.SceneControl.ropeEnd.transform.parent = ropeEnd.transform.parent;
			_enableAfterDisposable = Observable.Timer(TimeSpan.FromSeconds(3.5)).Subscribe((long _) =>
			{
				EnableAfter();
			}).AddTo(this);
		}
		CarClose = true;
	}

	private void EnableAfter()
	{
		obiRopeRenderer.enabled = true;
	}

	public void StartFillFuel()
	{
		if (!(_freeDriveProvider.FirstCarMultiPlayer != V.Car) && V.Car.TryGetComponent<VehicleData>(out var component))
		{
			if ((bool)component.CarChildsReferences.FuelSlot)
			{
				CarFuelPlace = component.CarChildsReferences.FuelSlot;
			}
			lookAtCam.cam = V.Car.transform;
			lookAtCam.enabled = true;
			if ((bool)component.CarChildsReferences.FuelTank)
			{
				_fuelWindowTank = component.CarChildsReferences.FuelTank;
				component.CarChildsReferences.FuelTank.lastGasPistol = this;
			}
			NowFilling = true;
			FuelingCar = V.Car;
			multiplayerCar = null;
			_checkFullFuelMultiplayerCarTick?.Dispose();
			_checkFullFuelMultiplayerCarTick = null;
		}
	}

	private void TryOpenFuelWindow()
	{
		if ((bool)_fuelWindowTank && !(Vector3.Distance(base.transform.position, targetDistace) > 0.05f))
		{
			FuelTankEntity fuelWindowTank = _fuelWindowTank;
			_fuelWindowTank = null;
			fuelWindowTank.StartFullingFuel();
		}
	}

	public void StartFillFuelMultiPlayerCar(VehicleData vehicleData)
	{
		if (!NowFilling)
		{
			_checkFullFuelMultiplayerCarTick?.Dispose();
			_checkFullFuelMultiplayerCarTick = Observable.Timer(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				CheckFullFuelMultiplayerCar();
			}).AddTo(this);
			multiplayerCar = vehicleData.gameObject;
			if (vehicleData.CarChildsReferences.FuelSlot != null)
			{
				CarFuelPlace = vehicleData.CarChildsReferences.FuelSlot;
			}
			lookAtCam.cam = vehicleData.transform;
			lookAtCam.enabled = true;
			NowFilling = true;
			FuelingCar = vehicleData.gameObject;
		}
	}

	private void CheckFullFuelMultiplayerCar()
	{
		if (NowFilling && (bool)multiplayerCar && multiplayerCar.TryGetComponent<VehicleData>(out var component) && (bool)component.CarChildsReferences.FuelTank)
		{
			FuelTankEntity fuelTank = component.CarChildsReferences.FuelTank;
			if (fuelTank.TankCapacity - fuelTank.CurrentFuel < 1f)
			{
				StopFillFuelMultiplayerCar();
			}
		}
	}

	public void StopFillFuel(bool full)
	{
		_fuelWindowTank = null;
		if (!NowFilling)
		{
			return;
		}
		if (_checkFullFuelMultiplayerCarTick != null)
		{
			StopFillFuelMultiplayerCar();
			return;
		}
		GameObject fuelingCar = FuelingCar;
		targetDistace = HomePosition;
		base.transform.eulerAngles = initEuler;
		lookAtCam.enabled = false;
		NowFilling = false;
		AudioWrapper.PostEvent("Stop_SFX_Fuel", base.gameObject);
		if (!full && (bool)fuelingCar && fuelingCar.TryGetComponent<VehicleData>(out var component) && (bool)component.CarChildsReferences.FuelTank)
		{
			component.CarChildsReferences.FuelTank.StopFullingFuel();
		}
		FuelingCar = null;
	}

	public void StopFillFuelMultiplayerCar()
	{
		_fuelWindowTank = null;
		if (NowFilling)
		{
			targetDistace = HomePosition;
			base.transform.eulerAngles = initEuler;
			lookAtCam.enabled = false;
			NowFilling = false;
			FuelingCar = null;
			AudioWrapper.PostEvent("Stop_SFX_Fuel", base.gameObject);
			_isFuelPlaying = false;
		}
		_checkFullFuelMultiplayerCarTick?.Dispose();
		_checkFullFuelMultiplayerCarTick = null;
	}

	private void Update()
	{
		if (!move && !NowFilling)
		{
			return;
		}
		if (NowFilling && (bool)CarFuelPlace)
		{
			targetDistace = CarFuelPlace.position;
			base.transform.position = Vector3.MoveTowards(base.transform.position, targetDistace, moveSpeed * Time.deltaTime);
			TryOpenFuelWindow();
		}
		else
		{
			targetDistace = HomePosition;
			base.transform.position = Vector3.MoveTowards(base.transform.position, targetDistace, moveSpeed * Time.deltaTime);
		}
		if (NowFilling)
		{
			if (nearCarFuelSlot)
			{
				if (!_isFuelPlaying)
				{
					AudioWrapper.PostEvent("Play_SFX_Fuel", base.gameObject);
					_isFuelPlaying = true;
				}
			}
			else if (_isFuelPlaying)
			{
				AudioWrapper.PostEvent("Stop_SFX_Fuel", base.gameObject);
				_isFuelPlaying = false;
			}
		}
		else if (_isFuelPlaying)
		{
			AudioWrapper.PostEvent("Stop_SFX_Fuel", base.gameObject);
			_isFuelPlaying = false;
		}
	}
}
