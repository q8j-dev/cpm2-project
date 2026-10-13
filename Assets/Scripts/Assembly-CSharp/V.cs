using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Purchasing;
using _Car_Parking.Scripts.Car.Physics.Vfx;
using _Car_Parking.Scripts.Racing_Scripts;

public static class V
{
	public enum placeToSpawn
	{
		MainCity = 0,
		Highway = 1,
		Desert = 2,
		Mountain = 3,
		Racing = 4,
		City2 = 5,
		Offroad = 6,
		House = 7,
		GarageEntrance = 8,
		JapanSerpentine = 9,
		JapanSuburban = 10,
		Island = 11,
		TropicalIsland = 12,
		Nurburgring = 13,
		Italy = 14,
		StraightRoad = 15
	}

	public static OneVinylData[] AllVinyls;

	public static placeToSpawn PlaceToSpawn = placeToSpawn.MainCity;

	public static placeToSpawn CurrentEnvironment = placeToSpawn.MainCity;

	public static RacingModeType CurrentRaceModeType;

	public static Action<bool> OnOutOfCarChanged;

	public static Action<bool> OnIsPassengerChanged;

	public static Action<bool> OnInGarageFreeChanged;

	public static Action<bool> OnPhotoModeChanged;

	public static Action OnSkyboxChanged;

	public static Action OnCarLiveryEnd;

	public static GameObject Car { get; set; }

	public static GameObject DriverPerson { get; set; }

	public static Transform GlobalPoint { get; set; }

	public static CoreGameManager CoreGameManager { get; set; }

	public static SceneControl SceneControl { get; set; }

	public static WheelSurfaceVfxService WheelSurfaceVfxService { get; set; }

	public static AnalyticWheres AnalyticWheres { get; set; }

	public static IStoreController StoreController { get; set; }

	public static AnimationCurve HealtGrip { get; set; }

	public static AnimationCurve TemperatureCooling { get; set; }

	public static AnimationCurve TemperatureHeating { get; set; }

	public static AnimationCurve TemperatureGrip { get; set; }

	public static GameObject LastUsedCar { get; set; }

	public static GameObject SandSmoke { get; set; }

	public static GameObject SkidSmoke { get; set; }

	public static GameObject SparkPrefab { get; set; }

	public static GameObject DamagedExhaustParticle { get; set; }

	public static GameObject ShootStrongExhaustParticle { get; set; }

	public static GameObject ShootWeakExhaustParticle { get; set; }

	public static GameObject BurbleExhaustParticle { get; set; }

	public static Vector3 GlobalOffset { get; set; }

	public static Font[] Fonts { get; set; }

	public static List<Material> FontMaterials { get; set; }

	public static int Steering { get; set; } = 0;

	public static int MenuIndex { get; set; }

	public static int PrevIndexOfCam { get; set; }

	public static int CurrentCamIndex { get; set; }

	public static int CurrentIndexArrayPositionCar { get; set; }

	public static int DirectionCar { get; set; }

	public static int PassedDay { get; set; }

	public static int RoomPassword { get; set; }

	public static int UnloadResourceCounter { get; set; }

	public static int PersonCamIndex { get; set; } = 0;

	public static ObscuredInt CarId { get; set; } = -1;

	public static ObscuredInt SlotID { get; set; }

	public static Transform PlayerPointLOD { get; set; }

	public static float SWsensitivity { get; set; }

	public static float LastSendMileage { get; set; }

	public static float DetailDistance { get; set; } = 40f;

	public static float CarPosLerpRate { get; set; } = 0.2f;

	public static string RoomNameToJoin { get; set; } = "";

	public static event Action<GameObject, VehicleData> OnCarEntered;

	public static event Action<GameObject, VehicleData> OnCarExit;

	public static event Action<MultiPlayerPosControl> OnNewCarCreated;

	public static event Action<MultiPersonControl> OnNewPersonCreated;

	public static event Action<Player> OnPlayerConnected;

	public static event Action<Player> OnPlayerDisconnected;

	public static event Action OnSceneLoaded;

	public static void SceneLoaded()
	{
		OnSceneLoaded?.Invoke();
	}

	public static void ResetOnSceneLoaded()
	{
		OnSceneLoaded = null;
	}

	public static void PlayerConnected(Player player)
	{
		OnPlayerConnected?.Invoke(player);
	}

	public static void PlayerDisconnected(Player player)
	{
		OnPlayerDisconnected?.Invoke(player);
	}

	public static void RaiseCarEnterEvent(GameObject person, VehicleData vehicleData)
	{
		OnCarEntered?.Invoke(person, vehicleData);
	}

	public static void RaiseCarExitEvent(GameObject person, VehicleData vehicleData)
	{
		OnCarExit?.Invoke(person, vehicleData);
	}

	public static void RaiseNewCarCreated(MultiPlayerPosControl mppc)
	{
		OnNewCarCreated?.Invoke(mppc);
	}

	public static void RaiseNewPersonCreated(MultiPersonControl person)
	{
		OnNewPersonCreated?.Invoke(person);
	}

	public static void TryClearV()
	{
		DriverPerson = null;
		LastUsedCar = null;
		SceneControl = null;
		SandSmoke = null;
		SkidSmoke = null;
		AnalyticWheres = null;
		SparkPrefab = null;
		DamagedExhaustParticle = null;
		ShootStrongExhaustParticle = null;
		ShootWeakExhaustParticle = null;
		BurbleExhaustParticle = null;
		GlobalPoint = null;
		PlayerPointLOD = null;
		OnOutOfCarChanged = null;
		OnIsPassengerChanged = null;
		GPSTracker.ResetStatics();
		PlayerDatas.UnloadUnusedAssets("V.ResetStatics");
	}
}
