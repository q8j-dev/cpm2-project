using System.Collections.Generic;
using CPMEngine;
using SWS;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

public class TrafficCarControl : MonoBehaviour
{
	public int TrafficNumber;

	public float targetSpeed;

	private List<GameObject> _trafficCarsLevels = new List<GameObject>();

	private TargetControll _targetControl;

	private int i;

	private PlayerDatas.CarLoadingUtility _carLoadingUtility;

	private Sort2 _sort2;

	private OptionalDependency<Sort2> _sort2Dep;

	private LevelScript _levelScript;

	private LevelsProvider _levelsProvider;

	private CPMVehicleCore _cpmVehicleCore;

	public Vector3 CarToFollowPos { get; private set; }

	public GameObject CarToFollow { get; private set; }

	[Inject]
	public void Construct(PlayerDatas.CarLoadingUtility carLoadingUtility, LevelScript levelScript, LevelsProvider levelsProvider, CPMVehicleCore cpmVehicleCore, OptionalDependency<Sort2> sort2)
	{
		_levelsProvider = levelsProvider;
		_levelScript = levelScript;
		_sort2Dep = sort2;
		_carLoadingUtility = carLoadingUtility;
		_cpmVehicleCore = cpmVehicleCore;
	}

	private void Awake()
	{
		_sort2 = _sort2Dep?.Value;
		_levelScript.TrafficCarControl = this;
		_targetControl = GetComponent<TargetControll>();
	}

	public void CreateLevelTraffic()
	{
		if (!(_levelsProvider.CurrentLevelData == null))
		{
			if (_levelsProvider.CurrentLevelData.SceneForLoad == 5)
			{
				CreatTrafficCarsFor6_7();
			}
			else if (_levelsProvider.CurrentLevelData.SceneForLoad == 6)
			{
				CreatTrafficCarsFor12();
			}
			else if (_levelsProvider.CurrentLevelData.ID == 40)
			{
				CreatTrafficCarsFor13();
			}
			else if (_levelsProvider.CurrentLevelData.ID == 55)
			{
				CreatTrafficCarsForFastDriving();
			}
			else if (_levelsProvider.CurrentLevelData.ID == 85 || _levelsProvider.CurrentLevelData.ID == 86)
			{
				CreateTrafficCarsForScene14();
			}
			else if (!FlagsStorage.IsMultiFree && !FlagsStorage.IsMainMenu)
			{
				CreatAllTraffic();
			}
		}
	}

	private void CreatTrafficCarsForFastDriving()
	{
		CreateTrafficCar(0, 50);
		CreateTrafficCar(0, 100);
		CreateTrafficCar(0, 140);
		CreateTrafficCar(1, 50);
		CreateTrafficCar(1, 120);
	}

	private void CreateTrafficCarsForScene14()
	{
		if ((bool)_sort2)
		{
			List<Vector3> list = new List<Vector3>();
			for (int i = 890; i < 1040; i++)
			{
				list.Add(_sort2.row4TrafficArrayV[i]);
			}
			Vector3[] targets = list.ToArray();
			CreateTrafficCar(0, 60, targets, 1);
			CreateTrafficCar(0, 90, targets, 1);
			CreateTrafficCar(0, 40, targets, 1);
			CreateTrafficCar(0, 50, targets, 1);
			CreateTrafficCar(0, 70, targets, 1);
		}
	}

	private void CreatTrafficCarsFor6_7()
	{
		for (int i = 0; i < TrafficNumber; i++)
		{
			CreateTrafficCar(0, 4 * i);
		}
	}

	private void CreatTrafficCarsFor8()
	{
		for (int i = 0; i < TrafficNumber; i++)
		{
			CreateTrafficCar(i, 2);
		}
		CreateTrafficCar(0, 122);
	}

	private void CreatTrafficCarsFor9()
	{
		for (int i = 0; i < TrafficNumber; i++)
		{
			CreateTrafficCar(i, 2);
		}
		CreateTrafficCar(0, 270);
		CreateTrafficCar(0, 68);
	}

	private void CreatTrafficCarsFor12()
	{
		CreateTrafficCar(0, 1);
		CreateTrafficCar(0, 5);
		CreateTrafficCar(1, 1);
		CreateTrafficCar(2, 3);
		CreateTrafficCar(3, 2);
	}

	private void CreatTrafficCarsFor13()
	{
		for (int i = 0; i < 9; i++)
		{
			CreateTrafficCarWaypoints(0, 5 * i);
		}
		for (int j = 0; j < 3; j++)
		{
			CreateTrafficCarWaypoints(3, 8 * j + 10);
		}
	}

	private void CreatAllTraffic()
	{
		for (int i = 0; i < TrafficNumber; i++)
		{
			CreateTrafficCar(i, 2);
		}
	}

	public void CreateChasingCar(int carId)
	{
		int num = 0;
		int num2 = 3;
		Vector3[] array = _targetControl.targets1V3[num];
		Vector3 pos = array[num2];
		pos.y++;
		CarToFollow = _carLoadingUtility.LoadRacingCarLevel(carId, pos, array, num2);
		CarToFollowPos = CarToFollow.transform.position;
		CarToFollow.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
		if (CarToFollow.TryGetComponent<CarAIController>(out var component))
		{
			component.targets = array;
			component.TargetSpeed = 50f;
			component.CurrentTarIndex = num2 + 1;
			component.Sign = 1;
			component.CurrentWay = num;
			component.AiType = AiType.LevelRacingCar;
			component.StopCar = true;
			GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("Trigger"), CarToFollow.transform);
			gameObject.transform.localPosition = new Vector3(0f, 0f, 5.5f);
			gameObject.transform.localScale = new Vector3(2f, 1f, 7f);
			gameObject.GetComponent<TriggerScriptTrafficCar>().CarAIController = component;
			if (CarToFollow.TryGetComponent<VehicleData>(out var component2))
			{
				component2.trigerScriptTraffic = gameObject.GetComponent<TriggerScriptTrafficCar>();
				component2.CarMat.color = Color.red;
			}
		}
		PlayerDatas.CreateSkid(CarToFollow);
	}

	private void CreateTrafficCar(int i, int initPosIndex)
	{
		if (i >= 0 && i < _targetControl.targets1V3.Length)
		{
			Vector3[] targets = _targetControl.targets1V3[i];
			CreateTrafficCar(i, initPosIndex, targets, 1);
		}
	}

	private void CreateTrafficCar(int i, int initPosIndex, int sign)
	{
		Vector3[] targets = _targetControl.targets1V3[i];
		CreateTrafficCar(i, initPosIndex, targets, sign);
	}

	private void CreateTrafficCar(int i, int initPosIndex, Vector3[] targets, int sign)
	{
		Vector3 pos = targets[initPosIndex] + Vector3.up * 0.5f;
		pos.y++;
		GameObject gameObject = TrafficCarsService.LoadTrafficCar(Random.Range(1, 11), dir: targets[initPosIndex + 1] - targets[initPosIndex], pos: pos, levelsProvider: _levelsProvider);
		if ((bool)gameObject)
		{
			if (gameObject.TryGetComponent<Powertrain>(out var component))
			{
				component.SetTrafficToWorld(_cpmVehicleCore);
			}
			if (gameObject.TryGetComponent<CarAIController>(out var component2))
			{
				component2.targets = targets;
				component2.TargetSpeed = targetSpeed - 2f;
				component2.CurrentTarIndex = initPosIndex + 1;
				component2.Sign = sign;
				component2.CurrentWay = i;
				component2.AiType = AiType.TrafficCar;
			}
			PlayerDatas.CreateSkid(gameObject);
			_trafficCarsLevels.Add(gameObject);
			if (FlagsStorage.IsMainMenu)
			{
				CreateHeadLights(gameObject);
			}
		}
	}

	private void CreateHeadLights(GameObject car)
	{
	}

	public void DestroyAllTrafficCar()
	{
		CommonExtensions.ClearGOCollection(_trafficCarsLevels);
	}

	private void CreateTrafficCarWaypoints(int i, int initPosIndex)
	{
		GameObject gameObject = TrafficCarsService.LoadTrafficCarForWaypointMove(Random.Range(1, 11), Vector3.zero);
		if ((bool)gameObject)
		{
			splineMove component = gameObject.GetComponent<splineMove>();
			component.SetPath(_targetControl.pathWaypoints[i]);
			gameObject.GetComponent<TrafficWaypointControl>().targetSpeed = Random.Range(36, 50);
			component.loopType = splineMove.LoopType.loop;
			component.GoToWaypoint(initPosIndex);
		}
	}
}
