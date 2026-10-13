using CPM.Scripts.Garage;
using CPMEngine;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.Infrastructure;

public class DynoRunController : MonoBehaviour
{
	public Transform dynoSpawnPoint;

	public DynoWindow dynoWindow;

	public dynoSceneParent dynoSceneParent;

	public Powertrain Powertrain;

	public VehicleData VehicleData;

	public Transform carTransform;

	[SerializeField]
	private float _wheelDirection = 1f;

	[SerializeField]
	private Vector3 RunVector;

	[SerializeField]
	[Space]
	private Animator fanAnimator;

	[SerializeField]
	private GameObject fanCollider;

	[Space]
	[SerializeField]
	private GameObject PlatformForShort;

	[SerializeField]
	private GameObject PlatformForLong;

	[Header("Camera to graphs")]
	[SerializeField]
	private Transform DynoStandGraphsCamera;

	private static readonly int FanOn = Animator.StringToHash("FanOn");

	private static readonly uint PlayDynoSound = AudioWrapper.GetId("Play_DynoStand_Roll");

	private static readonly uint StopDynoSound = AudioWrapper.GetId("Stop_DynoStand_Roll");

	private static readonly uint DynoRTPC = AudioWrapper.GetId("RTPC_Dyno_Wheel_Speed");

	private bool _isDynoRun;

	private Axles _axles;

	private Vector3 _initCarPos;

	private Quaternion _initCarRot;

	private int _oldTrc;

	private int _oldEsp;

	private GarageController _garageController;

	private GaragePositionsConfig _garageConfig;

	private IGarageSelectorService _selectorService;

	private CarInputManager _carInputManager;

	private Transform _wheelPoint;

	private float _maxTq;

	[Inject]
	public void Construct(GarageController garageController, GaragePositionsConfig garageConfig, IGarageSelectorService selectorService, CarInputManager carInputManager)
	{
		_garageController = garageController;
		_garageConfig = garageConfig;
		_selectorService = selectorService;
		_carInputManager = carInputManager;
	}

	private void Awake()
	{
		if (V.CoreGameManager.menuControl != null)
		{
			V.CoreGameManager.menuControl.DynoRunController = this;
		}
		dynoSceneParent.SetActive(active: false);
		_wheelPoint = new GameObject("WheelPoint").transform;
		_wheelPoint.SetParent(dynoSpawnPoint.parent);
		_wheelPoint.SetPositionAndRotation(dynoSpawnPoint.position, dynoSpawnPoint.rotation);
	}

	private async UniTaskVoid RunDynoStand()
	{
		Vector3 carDefaultPosition = carTransform.position;
		while (_isDynoRun)
		{
			MoveAxis();
			SetRtpcValue();
			SetRotation();
			carTransform.position = new Vector3(carDefaultPosition.x, carTransform.position.y, carDefaultPosition.z);
			await UniTask.WaitForFixedUpdate();
		}
	}

	public int GetClassPoint()
	{
		return VehicleData.GetVehicleEntityMono.CarEntity.GetValue().Points;
	}

	public void OnDynoOpen()
	{
		dynoSceneParent.SetActive(active: true);
		if ((bool)fanCollider)
		{
			fanCollider.GetComponent<BoxCollider>().enabled = false;
		}
		GlobalEventSystem.InvokeTesterCollisionIgnore(TesterType.Dyno, 1f);
		carTransform = V.LastUsedCar.transform;
		V.LastUsedCar.TryGetComponent<Axles>(out _axles);
		V.LastUsedCar.TryGetComponent<Powertrain>(out Powertrain);
		V.LastUsedCar.TryGetComponent<VehicleData>(out VehicleData);
		SetCarForDynoRun(V.LastUsedCar);
		base.enabled = true;
		AudioWrapper.PostEvent(PlayDynoSound, dynoSpawnPoint.gameObject);
		SoundController component = V.LastUsedCar.GetComponent<SoundController>();
		component.TurnOnSkidSound(state: false);
		component.TurnOnCrashSound(state: false);
		if (dynoSceneParent == null || _axles == null)
		{
			return;
		}
		_isDynoRun = true;
		FlagsStorage.HideSkidSmoke = true;
		RunDynoStand().Forget();
		dynoSceneParent.graphFill.SetFillState(toRun: true);
		if ((bool)fanAnimator)
		{
			fanAnimator.SetBool(FanOn, value: true);
		}
		if ((bool)PlatformForShort && (bool)PlatformForLong)
		{
			int num;
			if ((bool)Powertrain && CarIdStorage.LongsForDyno.Contains(VehicleData.CarId))
			{
				GearboxType gearType = Powertrain.GearType;
				num = ((gearType == GearboxType.GearAwd || gearType == GearboxType.Gear4Wd) ? 1 : 0);
			}
			else
			{
				num = 0;
			}
			bool flag = (byte)num != 0;
			PlatformForShort.SetActive(!flag);
			PlatformForLong.SetActive(flag);
		}
	}

	private Vector3 CarPos(GameObject car)
	{
		Vector3 result = VectorExtensions.GetPosByWheel(car, dynoSpawnPoint.position);
		if ((bool)Powertrain && (bool)_wheelPoint)
		{
			GarageData garageData = _garageConfig.GaragePositionsDatas[_selectorService.CurrentGarageIndex];
			Vector3 localPosition = _wheelPoint.localPosition;
			localPosition.x = ((Powertrain.GearType == GearboxType.GearFwd) ? garageData.FWDLocalPosDynoStand.x : garageData.RWDLocalPosDynoStand.x);
			if (_axles.Rear1Axle.Wheels.Count > 0)
			{
				localPosition.x = garageData.SWDLocalPosDynoStand.x;
			}
			localPosition.y = result.y;
			_wheelPoint.localPosition = localPosition;
			if ((bool)_axles)
			{
				float num = 0f;
				if (Powertrain.GearType == GearboxType.GearFwd)
				{
					num = VehicleData.transform.OffsetAlongForward(_axles.FrontAxle.LeftWheel.transform);
					result = _wheelPoint.position - _wheelPoint.transform.right * _wheelDirection * Mathf.Abs(num);
				}
				else
				{
					num = VehicleData.transform.OffsetAlongForward(_axles.RearAxle.LeftWheel.transform);
					result = _wheelPoint.position + _wheelPoint.transform.right * _wheelDirection * Mathf.Abs(num);
				}
				if (_axles.Rear1Axle.Wheels.Count > 0)
				{
					VehicleData.transform.OffsetAlongForward(_axles.FrontAxle.LeftWheel.transform);
					result = _wheelPoint.position - _wheelPoint.transform.right * _wheelDirection * Mathf.Abs(num);
				}
			}
		}
		return result;
	}

	private void SetCarForDynoRun(GameObject car)
	{
		_initCarPos = car.transform.position;
		_initCarRot = car.transform.rotation;
		_carInputManager.IgnoreSteerControl(state: true);
		_carInputManager.ResetSteerAndWheelsPosition();
		_oldTrc = _carInputManager.CarOptions.TrcLevel;
		_oldEsp = _carInputManager.CarOptions.EspLevel;
		_carInputManager.SetTrcLevel(0, update: false);
		_carInputManager.SetEspLevel(0, update: false);
		if (car.TryGetComponent<VehicleData>(out VehicleData))
		{
			VehicleData.racingState = RacingState.DynoRun;
		}
		Powertrain.SetAlignCar(state: true);
		Powertrain.SetDynorunMode(value: true);
		if (Powertrain.Gear == 0)
		{
			Powertrain.Gear = 1;
		}
		_garageController.EnableCarPhysics(VehicleData, isEnabled: true);
		car.transform.SetPositionAndRotation(CarPos(car), dynoSpawnPoint.transform.transform.rotation);
	}

	private void SetRtpcValue()
	{
		float value = math.remap(Powertrain.EngineIdleRpm, Powertrain.RedlineRpm, 0f, 1f, Powertrain.CurrentRpm);
		value = Mathf.Clamp(value, 0f, 1f);
		AudioWrapper.SetRTPCValue(DynoRTPC, value, dynoSpawnPoint.gameObject);
	}

	private void SetRotation()
	{
		if (!(dynoSceneParent == null))
		{
			Vector3 eulerAngles = carTransform.eulerAngles;
			eulerAngles.y = dynoSceneParent.point.transform.eulerAngles.y;
			carTransform.eulerAngles = eulerAngles;
		}
	}

	private void MoveAxis()
	{
		float rollAngle = _axles.FrontAxle.Wheels[0].Wheel.RollAngle;
		float rollAngle2 = _axles.RearAxle.Wheels[0].Wheel.RollAngle;
		Transform[] frontAxles = dynoSceneParent.frontAxles;
		foreach (Transform transform in frontAxles)
		{
			if ((bool)transform)
			{
				transform.localRotation = Quaternion.AngleAxis((0f - rollAngle) * 57.29578f, Vector3.forward);
			}
		}
		frontAxles = dynoSceneParent.rearAxles;
		foreach (Transform transform2 in frontAxles)
		{
			if ((bool)transform2)
			{
				transform2.localRotation = Quaternion.AngleAxis((0f - rollAngle2) * 57.29578f, Vector3.forward);
			}
		}
		if (!(_carInputManager.ThrottleInput < 0.1f) || !(Powertrain.CurrentRpm < Powertrain.EngineIdleRpm + 150f))
		{
			return;
		}
		foreach (WheelProvider allWheel in _axles.AllWheels)
		{
			if (allWheel.Wheel.IsPowered && allWheel.Wheel.AngularVelocity > 1f)
			{
				allWheel.Wheel.AngularVelocity = 0f;
			}
		}
	}

	public void OnDynoClosed(bool getToMenu = true)
	{
		dynoSceneParent.graphFill.ClearChart();
		_isDynoRun = false;
		FlagsStorage.HideSkidSmoke = PlayerPrefs.GetInt("hideSkidSmoke", 0) == 1;
		GlobalEventSystem.InvokeTesterCollisionIgnore(TesterType.Dyno, 1f);
		dynoSceneParent.graphFill.SetFillState(toRun: false);
		AudioWrapper.PostEvent(StopDynoSound, dynoSpawnPoint.gameObject);
		SoundController component = V.Car.GetComponent<SoundController>();
		component.TurnOnSkidSound(state: true);
		component.TurnOnCrashSound(state: true);
		if (getToMenu)
		{
			Powertrain.ResetTransmission();
			_garageController.ResetCarToSavedPosition();
		}
		Powertrain.SetAlignCar(state: false);
		Powertrain.SetDynorunMode(value: false);
		if (!Powertrain.IsAutomaticGearbox)
		{
			Powertrain.Gear = 0;
		}
		_carInputManager.SetTrcLevel(_oldTrc, update: false);
		_carInputManager.SetEspLevel(_oldEsp, update: false);
		VehicleData.racingState = RacingState.None;
		if ((bool)fanAnimator)
		{
			fanAnimator.SetBool(FanOn, value: false);
		}
		if ((bool)fanCollider)
		{
			fanCollider.GetComponent<BoxCollider>().enabled = true;
		}
		base.enabled = false;
		dynoSceneParent.SetActive(active: false);
		_carInputManager.IgnoreSteerControl(state: false);
	}

	private void OnDestroy()
	{
		if (_isDynoRun)
		{
			AudioWrapper.PostEvent(StopDynoSound, dynoSpawnPoint.gameObject);
		}
		_isDynoRun = false;
		FlagsStorage.HideSkidSmoke = PlayerPrefs.GetInt("hideSkidSmoke", 0) == 1;
	}
}
