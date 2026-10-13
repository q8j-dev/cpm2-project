using System;
using System.Collections.Generic;
using System.Diagnostics;
using Gley.UrbanSystem.Internal;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Jobs;
using _Car_Parking.Scripts.Steam.Perf;
using _Car_Parking.Scripts.Steam.Traffic;

namespace Gley.TrafficSystem.Internal
{
	internal class TrafficManager : MonoBehaviour
	{
		private TransformAccessArray _vehicleTrigger;

		private TransformAccessArray _suspensionConnectPoints;

		private TransformAccessArray _wheelsGraphics;

		private NativeArray<float3> _activeCameraPositions;

		private NativeArray<VehicleTypes> _vehicleType;

		private Rigidbody[] _vehicleRigidbody;

		private Dictionary<int, Rigidbody> _trailerRigidbody;

		private NativeArray<float3> _vehicleDownDirection;

		private NativeArray<float3> _vehicleForwardDirection;

		private NativeArray<float3> _vehicleRightDirection;

		private NativeArray<float3> _trailerRightDirection;

		private NativeArray<float3> _vehicleTargetWaypointPosition;

		private NativeArray<float3> _vehicleNextWaypointPosition;

		private NativeArray<float3> _vehicleStopPosition;

		private NativeArray<float3> _vehiclePosition;

		private NativeArray<float3> _vehicleGroundDirection;

		private NativeArray<float3> _vehicleForwardForce;

		private NativeArray<float3> _vehicleLaterlForce;

		private NativeArray<float3> _trailerForwardForce;

		private NativeArray<float3> _vehicleVelocity;

		private NativeArray<float3> _trailerVelocity;

		private NativeArray<float> _wheelSpringForce;

		private NativeArray<float> _vehicleMaxSteer;

		private NativeArray<float> _vehicleRotationAngle;

		private NativeArray<float> _vehiclePowerStep;

		private NativeArray<float> _vehicleBrakeStep;

		private NativeArray<float> _vehicleDrag;

		private NativeArray<float> _massDifference;

		private NativeArray<float> _trailerDrag;

		private NativeArray<float> _vehicleMaxAllowedSpeed;

		private NativeArray<float> _vehicleTargetSpeed;

		private NativeArray<float> _vehicleWheelDistance;

		private NativeArray<float> _vehicleSteeringStep;

		private NativeArray<float> _vehicleDistanceToStop;

		private NativeArray<float> _accelerationPercent;

		private NativeArray<float> _steerPercent;

		private NativeArray<float> _brakePercent;

		private NativeArray<float> _requiredBrakePower;

		private NativeArray<float> _sideLeaningFactor;

		private NativeArray<float> _forwardLeaningFactor;

		private NativeArray<bool> _stopTargetReached;

		private NativeArray<int> _vehicleStartWheelIndex;

		private NativeArray<int> _vehicleEndWheelIndex;

		private NativeArray<int> _vehicleNrOfWheels;

		private NativeArray<int> _vehicleListIndex;

		private NativeArray<int> _vehicleCurrentGear;

		private NativeArray<int> _vehicleTargetGear;

		private NativeArray<int> _trailerNrWheels;

		private NativeArray<bool> _vehicleReadyToRemove;

		private NativeArray<bool> _vehicleIsBraking;

		private NativeArray<bool> _vehicleNeedWaypoint;

		private NativeArray<bool> _vehicleNotDrivable;

		private NativeArray<bool> _excludedVehicles;

		private NativeArray<bool> _addExcludedVehicle;

		private NativeArray<RaycastHit> _wheelRaycatsResult;

		private NativeArray<RaycastCommand> _wheelRaycastCommand;

		private NativeArray<float3> _wheelSuspensionPosition;

		private NativeArray<float3> _wheelGroundPosition;

		private NativeArray<float3> _wheelVelocity;

		private NativeArray<float3> _wheelRightDirection;

		private NativeArray<float3> _wheelNormalDirection;

		private NativeArray<float3> _wheelSuspensionForce;

		private NativeArray<float> _wheelRotation;

		private NativeArray<float> _wheelRadius;

		private NativeArray<float> _wheelRaycatsDistance;

		private NativeArray<float> _wheelMaxSuspension;

		private NativeArray<float> _wheelSpringStiffness;

		private NativeArray<int> _groundedWheels;

		private NativeArray<int> _wheelAssociatedCar;

		private NativeArray<bool> _wheelCanSteer;

		private NativeArray<bool> _excludedWheels;

		private NativeArray<float> _turnAngle;

		private NativeArray<float> _raycastLengths;

		private NativeArray<float> _wCircumferences;

		private UpdateWheelJob _updateWheelJob;

		private UpdateTriggerJob _updateTriggerJob;

		private DriveJob _driveJob;

		private WheelJob _wheelJob;

		private JobHandle _raycastJobHandle;

		private JobHandle _updateWheelJobHandle;

		private JobHandle _updateTriggerJobHandle;

		private JobHandle _driveJobHandle;

		private JobHandle _wheelJobHandle;

		private Transform[] _activeCameras;

		private LayerMask _roadLayers;

		private Vector3 _forward;

		private Vector3 _up;

		private float _distanceToRemove;

		private float _minDistanceToAdd;

		private int _nrOfVehicles;

		private int _nrOfJobs;

		private int _indexToRemove;

		private int _totalWheels;

		private int _activeSquaresLevel;

		private int _activeCameraIndex;

		private bool _initialized;

		private static TrafficManager _instance;

		private AllVehiclesData _allVehiclesData;

		private DensityManager _densityManager;

		private SoundManager _soundManager;

		private VehicleAI _vehicleAI;

		private BehaviourManager _behaviourManager;

		private PositionValidator _positionValidator;

		private WaypointSelector _waypointSelector;

		private TrafficWaypointsData _trafficWaypointsData;

		private TrafficModules _trafficModules;

		private IntersectionManager _intersectionManager;

		private PathFindingManager _pathFindingManager;

		private IntersectionsData _intersectionsData;

		private ActiveCellsManager _activeCellsManager;

		private GridData _gridData;

		private DisabledWaypointsManager _disabledWaypointsManager;

		private PathFindingData _pathFindingData;

		private TimeManager _timeManager;

		private PlayerWaypointsManager _playerWaypointsManager;

		private DebugManager _debugManager;

		private IBehaviourImplementation _behaviourImplementation;

		private BehaviourResult _behaviourResult;

		private const int SteamRemoveChecksDivisor = 30;

		private static readonly ProfilerMarker SteamMarkWheelRead = new ProfilerMarker("Gley.Fixed.WheelRead");

		private static readonly ProfilerMarker SteamMarkVehicles = new ProfilerMarker("Gley.Fixed.Vehicles");

		private static readonly ProfilerMarker SteamMarkBehaviour = new ProfilerMarker("Gley.Fixed.Behaviour");

		private static readonly ProfilerMarker SteamMarkRaycasts = new ProfilerMarker("Gley.Fixed.Raycasts");

		private static readonly ProfilerMarker SteamMarkJobs = new ProfilerMarker("Gley.Fixed.Jobs");

		private static readonly ProfilerMarker SteamMarkForces = new ProfilerMarker("Gley.Fixed.Forces");

		private static readonly ProfilerMarker SteamMarkAI = new ProfilerMarker("Gley.Fixed.AI");

		private static readonly ProfilerMarker SteamMarkUpdateWheels = new ProfilerMarker("Gley.Update.Wheels");

		private static readonly ProfilerMarker SteamMarkUpdateDensity = new ProfilerMarker("Gley.Update.Density");

		private static readonly ProfilerMarker SteamMarkUpdateManagers = new ProfilerMarker("Gley.Update.Managers");

		private const float SteamBehaviourFarDistance = 100f;

		private int _steamFixedStep;

		private const int SteamWheelJobBatch = 32;

		private NativeArray<float3> _steamBodyVelocity;

		private NativeArray<float3> _steamBodyAngularVelocity;

		private NativeArray<float3> _steamBodyCenterOfMass;

		public static float SteamMaxForwardLean = 0.15f;

		public static bool Exists => _instance != null;

		public bool Initialized => _initialized;

		public static TrafficManager Instance
		{
			get
			{
				if (_instance == null && MonoBehaviourUtilities.TryGetSceneScript(out ExecutionResult<TrafficWaypointsData> result))
				{
					_instance = result.Value.gameObject.AddComponent<TrafficManager>();
				}
				return _instance;
			}
		}

		public AllVehiclesData AllVehiclesData
		{
			get
			{
				if (_allVehiclesData != null)
				{
					return _allVehiclesData;
				}
				return ReturnError<AllVehiclesData>();
			}
		}

		public DensityManager DensityManager
		{
			get
			{
				if (_densityManager != null)
				{
					return _densityManager;
				}
				return ReturnError<DensityManager>();
			}
		}

		public SoundManager SoundManager
		{
			get
			{
				if (_soundManager != null)
				{
					return _soundManager;
				}
				return ReturnError<SoundManager>();
			}
		}

		public VehicleAI VehicleAI
		{
			get
			{
				if (_vehicleAI != null)
				{
					return _vehicleAI;
				}
				return ReturnError<VehicleAI>();
			}
		}

		public BehaviourManager BehaviourManager
		{
			get
			{
				if (_behaviourManager != null)
				{
					return _behaviourManager;
				}
				return ReturnError<BehaviourManager>();
			}
		}

		public PositionValidator PositionValidator
		{
			get
			{
				if (_positionValidator != null)
				{
					return _positionValidator;
				}
				return ReturnError<PositionValidator>();
			}
		}

		public WaypointSelector WaypointSelector
		{
			get
			{
				if (_waypointSelector != null)
				{
					return _waypointSelector;
				}
				return ReturnError<WaypointSelector>();
			}
		}

		public TrafficWaypointsData TrafficWaypointsData
		{
			get
			{
				if (_trafficWaypointsData != null)
				{
					return _trafficWaypointsData;
				}
				return ReturnError<TrafficWaypointsData>();
			}
		}

		public TrafficModules TrafficModules
		{
			get
			{
				if (_trafficModules != null)
				{
					return _trafficModules;
				}
				return ReturnError<TrafficModules>();
			}
		}

		public IntersectionManager IntersectionManager
		{
			get
			{
				if (_intersectionManager != null)
				{
					return _intersectionManager;
				}
				return ReturnError<IntersectionManager>();
			}
		}

		public PathFindingManager PathFindingManager
		{
			get
			{
				if (TrafficModules.PathFinding)
				{
					if (_pathFindingManager != null)
					{
						return _pathFindingManager;
					}
					UnityEngine.Debug.LogError(TrafficSystemErrors.NullPathFindingData);
				}
				else
				{
					UnityEngine.Debug.LogError(TrafficSystemErrors.NoPathFindingWaypoints);
				}
				return null;
			}
		}

		public IntersectionsData IntersectionsData
		{
			get
			{
				if (_intersectionsData != null)
				{
					return _intersectionsData;
				}
				return ReturnError<IntersectionsData>();
			}
		}

		public ActiveCellsManager ActiveCellsManager
		{
			get
			{
				if (_activeCellsManager != null)
				{
					return _activeCellsManager;
				}
				return ReturnError<ActiveCellsManager>();
			}
		}

		public GridData GridData
		{
			get
			{
				if (_gridData != null)
				{
					return _gridData;
				}
				return ReturnError<GridData>();
			}
		}

		public DisabledWaypointsManager DisabledWaypointsManager
		{
			get
			{
				if (_disabledWaypointsManager != null)
				{
					return _disabledWaypointsManager;
				}
				return ReturnError<DisabledWaypointsManager>();
			}
		}

		public PathFindingData PathFindingData
		{
			get
			{
				if (_pathFindingData != null)
				{
					return _pathFindingData;
				}
				return ReturnError<PathFindingData>();
			}
		}

		public TimeManager TimeManager
		{
			get
			{
				if (_timeManager != null)
				{
					return _timeManager;
				}
				return ReturnError<TimeManager>();
			}
		}

		public PlayerWaypointsManager PlayerWaypointsManager
		{
			get
			{
				if (_playerWaypointsManager != null)
				{
					return _playerWaypointsManager;
				}
				return ReturnError<PlayerWaypointsManager>();
			}
		}

		public DebugManager DebugManager
		{
			get
			{
				if (_debugManager != null)
				{
					return _debugManager;
				}
				return ReturnError<DebugManager>();
			}
		}

		public IBehaviourImplementation BehaviourImplementation
		{
			get
			{
				if (_behaviourImplementation != null)
				{
					return _behaviourImplementation;
				}
				return ReturnError<IBehaviourImplementation>();
			}
		}

		private T ReturnError<T>()
		{
			StackTrace stackTrace = new StackTrace();
			string empty = string.Empty;
			if (stackTrace.FrameCount >= 3)
			{
				empty = stackTrace.GetFrame(1).GetMethod().Name;
			}
			UnityEngine.Debug.LogError(TrafficSystemErrors.PropertyError(empty));
			return default;
		}

		public void Initialize(Transform[] activeCameras, int nrOfVehicles, VehiclePool vehiclePool, TrafficOptions trafficOptions)
		{
			LayerSetup layerSetup = Resources.Load<LayerSetup>("LayerSetupData");
			ExecutionResult<GridData> result;
			if (layerSetup == null)
			{
				UnityEngine.Debug.LogError(TrafficSystemErrors.LayersNotConfigured);
			}
			else if (MonoBehaviourUtilities.TryGetSceneScript(out result))
			{
				if (result.Value.IsValid(out var error))
				{
					_gridData = result.Value;
					if (MonoBehaviourUtilities.TryGetSceneScript(out ExecutionResult<TrafficWaypointsData> result2))
					{
						if (result2.Value.IsValid(out var error2))
						{
							_trafficWaypointsData = result2.Value.Initialize();
							if (MonoBehaviourUtilities.TryGetSceneScript(out ExecutionResult<IntersectionsData> result3))
							{
								if (result3.Value.IsValid(out var error3))
								{
									_intersectionsData = result3.Value;
									base.gameObject.AddComponent<CoroutineManager>();
									IPedestrianWaypointsDataHandler pedestrianWaypointsDataHandler = new DummyPedestrianWaypointsDataHandler();
									if (MonoBehaviourUtilities.TryGetObjectScript(TrafficSystemConstants.PlayHolder, out ExecutionResult<TrafficModules> result4))
									{
										_trafficModules = result4.Value;
										if (TrafficModules.PathFinding)
										{
											if (!MonoBehaviourUtilities.TryGetObjectScript(TrafficSystemConstants.PlayHolder, out ExecutionResult<PathFindingData> result5))
											{
												UnityEngine.Debug.LogError(result5.Error);
												return;
											}
											if (!result5.Value.IsValid(out var error4))
											{
												UnityEngine.Debug.LogError(error4);
												return;
											}
											_pathFindingData = result5.Value;
										}
										if (vehiclePool.trafficCars.Length == 0)
										{
											UnityEngine.Debug.LogError(TrafficSystemErrors.NoVehiclesAvailable);
											return;
										}
										if (nrOfVehicles <= 0)
										{
											UnityEngine.Debug.LogError(TrafficSystemErrors.InvalidNrOfVehicles);
											return;
										}
										_nrOfVehicles = nrOfVehicles;
										_activeCameras = activeCameras;
										_activeSquaresLevel = trafficOptions.ActiveSquaresLevel;
										_roadLayers = layerSetup.roadLayers;
										_up = Vector3.up;
										_soundManager = new SoundManager(trafficOptions.MasterVolume);
										_allVehiclesData = new AllVehiclesData(base.transform, vehiclePool, nrOfVehicles, layerSetup.buildingsLayers, layerSetup.obstaclesLayers, layerSetup.playerLayers, layerSetup.roadLayers, trafficOptions.LightsOn, trafficOptions.ModifyTriggerSize, TrafficWaypointsData, trafficOptions.MinOffset, trafficOptions.MaxOffset, trafficOptions.ColorPool, trafficOptions.InstantiateVehicle);
										_totalWheels = AllVehiclesData.GetTotalWheels();
										_wheelSuspensionPosition = new NativeArray<float3>(_totalWheels, Allocator.Persistent);
										_wheelVelocity = new NativeArray<float3>(_totalWheels, Allocator.Persistent);
										_wheelGroundPosition = new NativeArray<float3>(_totalWheels, Allocator.Persistent);
										_wheelNormalDirection = new NativeArray<float3>(_totalWheels, Allocator.Persistent);
										_wheelRightDirection = new NativeArray<float3>(_totalWheels, Allocator.Persistent);
										_wheelRaycatsDistance = new NativeArray<float>(_totalWheels, Allocator.Persistent);
										_wheelRadius = new NativeArray<float>(_totalWheels, Allocator.Persistent);
										_wheelAssociatedCar = new NativeArray<int>(_totalWheels, Allocator.Persistent);
										_wheelCanSteer = new NativeArray<bool>(_totalWheels, Allocator.Persistent);
										_wheelSuspensionForce = new NativeArray<float3>(_totalWheels, Allocator.Persistent);
										_wheelMaxSuspension = new NativeArray<float>(_totalWheels, Allocator.Persistent);
										_wheelSpringStiffness = new NativeArray<float>(_totalWheels, Allocator.Persistent);
										_wheelRaycatsResult = new NativeArray<RaycastHit>(_totalWheels, Allocator.Persistent);
										_wheelRaycastCommand = new NativeArray<RaycastCommand>(_totalWheels, Allocator.Persistent);
										_wheelSpringForce = new NativeArray<float>(_totalWheels, Allocator.Persistent);
										_excludedWheels = new NativeArray<bool>(_totalWheels, Allocator.Persistent);
										_groundedWheels = new NativeArray<int>(_totalWheels, Allocator.Persistent);
										_vehicleTrigger = new TransformAccessArray(nrOfVehicles);
										_vehicleType = new NativeArray<VehicleTypes>(nrOfVehicles, Allocator.Persistent);
										_vehicleForwardForce = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
										_vehicleLaterlForce = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
										_trailerForwardForce = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
										_vehiclePosition = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
										_vehicleGroundDirection = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
										_vehicleDownDirection = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
										_vehicleRightDirection = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
										_trailerRightDirection = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
										_vehicleForwardDirection = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
										_vehicleTargetWaypointPosition = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
										_vehicleNextWaypointPosition = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
										_vehicleStopPosition = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
										_vehicleVelocity = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
										_trailerVelocity = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
										_wheelRotation = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
										_turnAngle = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
										_vehicleDrag = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
										_massDifference = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
										_trailerDrag = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
										_vehicleSteeringStep = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
										_vehicleDistanceToStop = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
										_accelerationPercent = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
										_steerPercent = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
										_brakePercent = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
										_requiredBrakePower = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
										_vehicleMaxAllowedSpeed = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
										_vehicleTargetSpeed = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
										_vehicleWheelDistance = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
										_vehiclePowerStep = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
										_vehicleBrakeStep = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
										_raycastLengths = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
										_wCircumferences = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
										_vehicleRotationAngle = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
										_vehicleMaxSteer = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
										_sideLeaningFactor = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
										_forwardLeaningFactor = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
										_vehicleListIndex = new NativeArray<int>(nrOfVehicles, Allocator.Persistent);
										_vehicleEndWheelIndex = new NativeArray<int>(nrOfVehicles, Allocator.Persistent);
										_vehicleStartWheelIndex = new NativeArray<int>(nrOfVehicles, Allocator.Persistent);
										_vehicleNrOfWheels = new NativeArray<int>(nrOfVehicles, Allocator.Persistent);
										_trailerNrWheels = new NativeArray<int>(nrOfVehicles, Allocator.Persistent);
										_vehicleReadyToRemove = new NativeArray<bool>(nrOfVehicles, Allocator.Persistent);
										_vehicleNeedWaypoint = new NativeArray<bool>(nrOfVehicles, Allocator.Persistent);
										_vehicleIsBraking = new NativeArray<bool>(nrOfVehicles, Allocator.Persistent);
										_vehicleNotDrivable = new NativeArray<bool>(nrOfVehicles, Allocator.Persistent);
										_excludedVehicles = new NativeArray<bool>(nrOfVehicles, Allocator.Persistent);
										_addExcludedVehicle = new NativeArray<bool>(nrOfVehicles, Allocator.Persistent);
										_vehicleCurrentGear = new NativeArray<int>(nrOfVehicles, Allocator.Persistent);
										_vehicleTargetGear = new NativeArray<int>(nrOfVehicles, Allocator.Persistent);
										_stopTargetReached = new NativeArray<bool>(nrOfVehicles, Allocator.Persistent);
										_vehicleRigidbody = new Rigidbody[nrOfVehicles];
										_trailerRigidbody = new Dictionary<int, Rigidbody>();
										_activeCameraPositions = new NativeArray<float3>(activeCameras.Length, Allocator.Persistent);
										for (int i = 0; i < _activeCameraPositions.Length; i++)
										{
											_activeCameraPositions[i] = activeCameras[i].position;
										}
										if (trafficOptions.DistanceToRemove < 0f)
										{
											float num = GridData.GridCellSize;
											trafficOptions.DistanceToRemove = 2f * num + num / 2f;
										}
										if (trafficOptions.MinDistanceToAdd < 0f)
										{
											float num2 = GridData.GridCellSize;
											trafficOptions.MinDistanceToAdd = num2 + num2 / 2f;
										}
										_distanceToRemove = trafficOptions.DistanceToRemove * trafficOptions.DistanceToRemove;
										_minDistanceToAdd = trafficOptions.MinDistanceToAdd;
										_timeManager = new TimeManager();
										bool debugDensity = false;
										_positionValidator = new PositionValidator(_activeCameras, layerSetup.trafficLayers, layerSetup.playerLayers, layerSetup.buildingsLayers, _minDistanceToAdd, debugDensity);
										_waypointSelector = new WaypointSelector(GridData, trafficOptions.SpawnWaypointSelector, TrafficWaypointsData);
										_behaviourManager = new BehaviourManager(nrOfVehicles, trafficOptions.VehicleBehaviours, TrafficWaypointsData, AllVehiclesData);
										if (trafficOptions.BehaviourImplementation == null)
										{
											_behaviourImplementation = new DefaultBehaviourImplementation().Initialize(AllVehiclesData.AllVehicles, trafficOptions.DefaultPathLength);
										}
										else
										{
											_behaviourImplementation = trafficOptions.BehaviourImplementation.Initialize(AllVehiclesData.AllVehicles, trafficOptions.DefaultPathLength);
										}
										_playerWaypointsManager = new PlayerWaypointsManager();
										_vehicleAI = new VehicleAI(AllVehiclesData, TrafficWaypointsData, SoundManager, TimeManager, trafficOptions.DefaultPathLength, PlayerWaypointsManager);
										Transform[] array = new Transform[_totalWheels];
										Transform[] array2 = new Transform[_totalWheels];
										int num3 = 0;
										for (int j = 0; j < nrOfVehicles; j++)
										{
											VehicleComponent vehicleComponent = AllVehiclesData.AllVehicles[j];
											_vehicleTrigger.Add(vehicleComponent.frontTrigger);
											_vehicleRigidbody[j] = vehicleComponent.rb;
											_vehicleSteeringStep[j] = vehicleComponent.SteerStep;
											_vehicleWheelDistance[j] = vehicleComponent.wheelDistance;
											_accelerationPercent[j] = 1f;
											_brakePercent[j] = 1f;
											_requiredBrakePower[j] = 0f;
											_steerPercent[j] = -2.1474836E+09f;
											_vehicleDrag[j] = vehicleComponent.rb.drag;
											_raycastLengths[j] = vehicleComponent.GetRayCastLength();
											_wCircumferences[j] = vehicleComponent.GetWheelCircumference();
											_vehicleMaxSteer[j] = vehicleComponent.MaxSteer;
											_vehicleStartWheelIndex[j] = num3;
											_vehicleNrOfWheels[j] = vehicleComponent.GetNrOfWheels();
											_vehicleEndWheelIndex[j] = _vehicleStartWheelIndex[j] + _vehicleNrOfWheels[j];
											_trailerNrWheels[j] = vehicleComponent.GetTrailerWheels();
											_sideLeaningFactor[j] = 1f - vehicleComponent.SideLeaningFactor;
											_forwardLeaningFactor[j] = 1f - vehicleComponent.ForwardLeaningFactor;
											for (int k = 0; k < _vehicleNrOfWheels[j]; k++)
											{
												array[num3] = vehicleComponent.allWheels[k].wheelTransform;
												array2[num3] = vehicleComponent.allWheels[k].wheelTransform.GetChild(0);
												_wheelCanSteer[num3] = vehicleComponent.allWheels[k].wheelPosition == Wheel.WheelPosition.Front;
												_wheelRadius[num3] = vehicleComponent.allWheels[k].wheelRadius;
												_wheelMaxSuspension[num3] = vehicleComponent.allWheels[k].maxSuspension;
												_wheelSpringStiffness[num3] = vehicleComponent.SpringStiffness;
												_wheelAssociatedCar[num3] = j;
												_wheelSpringForce[num3] = vehicleComponent.SpringForce;
												num3++;
											}
											if (vehicleComponent.trailer != null)
											{
												TrailerComponent trailer = vehicleComponent.trailer;
												for (int l = 0; l < vehicleComponent.trailer.GetNrOfWheels(); l++)
												{
													array[num3] = trailer.allWheels[l].wheelTransform;
													array2[num3] = trailer.allWheels[l].wheelTransform.GetChild(0);
													_wheelCanSteer[num3] = false;
													_wheelRadius[num3] = trailer.allWheels[l].wheelRadius;
													_wheelMaxSuspension[num3] = trailer.allWheels[l].maxSuspension;
													_wheelSpringStiffness[num3] = trailer.GetSpringStiffness();
													_wheelAssociatedCar[num3] = j;
													_wheelSpringForce[num3] = trailer.GetSpringForce();
													num3++;
												}
												_vehicleEndWheelIndex[j] += trailer.GetNrOfWheels();
												_trailerDrag[j] = trailer.rb.drag;
												_massDifference[j] = trailer.rb.mass / vehicleComponent.rb.mass * (trailer.joint.connectedMassScale / trailer.joint.massScale);
												_trailerRigidbody.Add(j, trailer.rb);
											}
											_vehicleListIndex[j] = vehicleComponent.ListIndex;
											_vehicleType[j] = vehicleComponent.VehicleType;
										}
										_suspensionConnectPoints = new TransformAccessArray(array);
										_wheelsGraphics = new TransformAccessArray(array2);
										if (SystemInfo.processorCount != 0)
										{
											_nrOfJobs = _totalWheels / SystemInfo.processorCount + 1;
										}
										else
										{
											_nrOfJobs = nrOfVehicles / 4;
										}
										Events.OnChangeDestination = (Events.ChangeDestination)Delegate.Combine(Events.OnChangeDestination, new Events.ChangeDestination(DestinationChanged));
										Events.OnVehicleActivated = (Events.VehicleActivated)Delegate.Combine(Events.OnVehicleActivated, new Events.VehicleActivated(NewVehicleAdded));
										_disabledWaypointsManager = new DisabledWaypointsManager(TrafficWaypointsData, WaypointSelector, trafficOptions.DisableWaypointsArea);
										_densityManager = new DensityManager(AllVehiclesData, TrafficWaypointsData, _positionValidator, _activeCameraPositions, nrOfVehicles, activeCameras[0].position, activeCameras[0].forward, _activeSquaresLevel, trafficOptions.UseWaypointPriority, trafficOptions.InitialDensity, debugDensity, WaypointSelector);
										_intersectionManager = new IntersectionManager(IntersectionsData, TrafficWaypointsData, pedestrianWaypointsDataHandler, trafficOptions.TrafficLightsBehaviour, trafficOptions.GreenLightTime, trafficOptions.YellowLightTime, TimeManager);
										_activeCellsManager = new ActiveCellsManager(_activeCameraPositions, GridData, trafficOptions.ActiveSquaresLevel);
										if (TrafficModules.PathFinding)
										{
											_pathFindingManager = new PathFindingManager(GridData, _pathFindingData);
										}
										_initialized = true;
									}
									else
									{
										UnityEngine.Debug.LogError(result4.Error);
									}
								}
								else
								{
									UnityEngine.Debug.LogError(error3);
								}
							}
							else
							{
								UnityEngine.Debug.LogError(result3.Error);
							}
						}
						else
						{
							UnityEngine.Debug.LogError(error2);
						}
					}
					else
					{
						UnityEngine.Debug.LogError(result2.Error);
					}
				}
				else
				{
					UnityEngine.Debug.LogError(error);
				}
			}
			else
			{
				UnityEngine.Debug.LogError(result.Error);
			}
		}

		public void InitializeBehaviourImplementation(params object[] parameters)
		{
			_behaviourImplementation.Initialize(parameters);
		}

		private void FixedUpdate()
		{
			if (!_initialized)
			{
				return;
			}
			_steamFixedStep++;
			if (SteamPerfFlags.IsOn("trafficPhysics"))
			{
				SteamReadSuspension();
			}
			else
			{
				for (int i = 0; i < _totalWheels; i++)
				{
					if (!_excludedWheels[i])
					{
						_wheelSuspensionPosition[i] = _suspensionConnectPoints[i].position;
						_wheelVelocity[i] = _vehicleRigidbody[_wheelAssociatedCar[i]].GetPointVelocity(_wheelSuspensionPosition[i]);
					}
				}
			}
			for (int j = 0; j < _nrOfVehicles; j++)
			{
				if (!_excludedVehicles[j] && !_vehicleRigidbody[j].IsSleeping())
				{
					if (_trailerNrWheels[j] > 0)
					{
						_trailerVelocity[j] = _trailerRigidbody[j].velocity;
						_trailerRightDirection[j] = _trailerRigidbody[j].transform.right;
					}
					_vehicleVelocity[j] = _vehicleRigidbody[j].velocity;
					_vehicleDownDirection[j] = -AllVehiclesData.GetVehicle(j).GetFrontAxleUpVector();
					_forward = AllVehiclesData.GetVehicle(j).GetFrontAxleForwardVector();
					_vehicleForwardDirection[j] = _forward;
					_vehicleRightDirection[j] = AllVehiclesData.GetVehicle(j).GetFrontAxleRightVector();
					_vehiclePosition[j] = AllVehiclesData.GetVehicle(j).GetFrontAxlePosition();
					_vehicleGroundDirection[j] = float3.zero;
					_groundedWheels[j] = 0;
					if (!SteamSkipBehaviourThisStep(j))
					{
						_behaviourResult = BehaviourManager.ExecuteBehaviour(AllVehiclesData.AllVehicles[j].MovementInfo, j, _requiredBrakePower[j], _stopTargetReached[j], _vehicleCurrentGear[j], AllVehiclesData.AllVehicles[j].FrontTrigger.position, _vehicleStopPosition[j]);
						_vehicleMaxAllowedSpeed[j] = _behaviourResult.MaxAllowedSpeed;
						_brakePercent[j] = _behaviourResult.BrakePercent;
						_vehicleStopPosition[j] = _behaviourResult.SpeedInPoint.StopPosition;
						_vehicleDistanceToStop[j] = _behaviourResult.SpeedInPoint.DistanceToStop;
						_vehicleTargetGear[j] = _behaviourResult.TargetGear;
						_steerPercent[j] = _behaviourResult.SteerPercent;
						_vehicleTargetSpeed[j] = _behaviourResult.SpeedInPoint.TargetSpeed;
					}
				}
			}
			for (int k = 0; k < _totalWheels; k++)
			{
				if (!_excludedWheels[k] && !_vehicleRigidbody[_wheelAssociatedCar[k]].IsSleeping())
				{
					_wheelRaycastCommand[k] = new RaycastCommand(_wheelSuspensionPosition[k], _vehicleDownDirection[_wheelAssociatedCar[k]], new QueryParameters(_roadLayers), _raycastLengths[_wheelAssociatedCar[k]]);
				}
			}
			_raycastJobHandle = RaycastCommand.ScheduleBatch(_wheelRaycastCommand, _wheelRaycatsResult, _nrOfJobs);
			_raycastJobHandle.Complete();
			for (int l = 0; l < _totalWheels; l++)
			{
				if (!_excludedWheels[l] && !_vehicleRigidbody[_wheelAssociatedCar[l]].IsSleeping())
				{
					_wheelRaycatsDistance[l] = _wheelRaycatsResult[l].distance;
					_wheelNormalDirection[l] = _wheelRaycatsResult[l].normal;
					_wheelGroundPosition[l] = _wheelRaycatsResult[l].point;
					if (_wheelRaycatsDistance[l] != 0f)
					{
						_vehicleGroundDirection[_wheelAssociatedCar[l]] += _wheelNormalDirection[l];
						_groundedWheels[_wheelAssociatedCar[l]]++;
					}
				}
			}
			_wheelJob = new WheelJob
			{
				WheelSuspensionForce = _wheelSuspensionForce,
				SpringForces = _wheelSpringForce,
				WheelMaxSuspension = _wheelMaxSuspension,
				WheelRayCastDistance = _wheelRaycatsDistance,
				WheelRadius = _wheelRadius,
				WheelNormalDirection = _wheelNormalDirection,
				WheelVelocity = _wheelVelocity,
				SpringStiffness = _wheelSpringStiffness,
				ExcludedWheels = _excludedWheels
			};
			_driveJob = new DriveJob
			{
				WheelCircumferences = _wCircumferences,
				CarVelocity = _vehicleVelocity,
				FixedDeltaTime = Time.fixedDeltaTime,
				TargetWaypointPosition = _vehicleTargetWaypointPosition,
				NextWaypointPosition = _vehicleNextWaypointPosition,
				StopPosition = _vehicleStopPosition,
				AllBotsPosition = _vehiclePosition,
				MaxSteer = _vehicleMaxSteer,
				ForwardDirection = _vehicleForwardDirection,
				WorldUp = _up,
				WheelRotation = _wheelRotation,
				TurnAngle = _turnAngle,
				VehicleRotationAngle = _vehicleRotationAngle,
				ReadyToRemove = _vehicleReadyToRemove,
				NeedsWaypoint = _vehicleNeedWaypoint,
				DistanceToRemove = _distanceToRemove,
				CameraPositions = _activeCameraPositions,
				ForwardForce = _vehicleForwardForce,
				LateralForce = _vehicleLaterlForce,
				RightDirection = _vehicleRightDirection,
				PowerStep = _vehiclePowerStep,
				BrakeStep = _vehicleBrakeStep,
				IsBraking = _vehicleIsBraking,
				Drag = _vehicleDrag,
				MaximumAllowedSpeed = _vehicleMaxAllowedSpeed,
				CurrentGear = _vehicleCurrentGear,
				TargetGear = _vehicleTargetGear,
				GroundDirection = _vehicleGroundDirection,
				SteeringStep = _vehicleSteeringStep,
				WheelDistance = _vehicleWheelDistance,
				NrOfWheels = _vehicleNrOfWheels,
				TrailerVelocity = _trailerVelocity,
				TrailerForce = _trailerForwardForce,
				TrailerRightDirection = _trailerRightDirection,
				TrailerNrOfWheels = _trailerNrWheels,
				MassDifference = _massDifference,
				TrailerDrag = _trailerDrag,
				DistanceToStop = _vehicleDistanceToStop,
				ExcludedVehicles = _excludedVehicles,
				GroundedWheels = _groundedWheels,
				AccelerationPercent = _accelerationPercent,
				BrakePercent = _brakePercent,
				RequiredBrakePower = _requiredBrakePower,
				SteerPercent = _steerPercent,
				TargetReached = _stopTargetReached,
				TargetSpeed = _vehicleTargetSpeed
			};
			_wheelJobHandle = IJobParallelForExtensions.Schedule(_wheelJob, _totalWheels, _nrOfJobs);
			_driveJobHandle = IJobParallelForExtensions.Schedule(_driveJob, _nrOfVehicles, _nrOfJobs);
			_wheelJobHandle.Complete();
			_driveJobHandle.Complete();
			_wheelSuspensionForce = _wheelJob.WheelSuspensionForce;
			_wheelRotation = _driveJob.WheelRotation;
			_turnAngle = _driveJob.TurnAngle;
			_vehicleRotationAngle = _driveJob.VehicleRotationAngle;
			_vehicleReadyToRemove = _driveJob.ReadyToRemove;
			_vehicleNeedWaypoint = _driveJob.NeedsWaypoint;
			_vehicleForwardForce = _driveJob.ForwardForce;
			_vehicleLaterlForce = _driveJob.LateralForce;
			_vehicleIsBraking = _driveJob.IsBraking;
			_vehicleCurrentGear = _driveJob.CurrentGear;
			_trailerForwardForce = _driveJob.TrailerForce;
			for (int m = 0; m < _nrOfVehicles; m++)
			{
				if (_excludedVehicles[m] || _vehicleRigidbody[m].IsSleeping())
				{
					continue;
				}
				int num;
				if (SteamPerfFlags.IsOn("trafficPhysics"))
				{
					num = SteamApplyWheelForces(m);
				}
				else
				{
					num = 0;
					for (int n = _vehicleStartWheelIndex[m]; n < _vehicleEndWheelIndex[m] - _trailerNrWheels[m]; n++)
					{
						if (_wheelRaycatsDistance[n] != 0f)
						{
							num++;
							_vehicleRigidbody[m].AddForceAtPosition(_wheelSuspensionForce[n], _wheelGroundPosition[n]);
							_vehicleRigidbody[m].AddForceAtPosition(_vehicleLaterlForce[m], new Vector3(_wheelSuspensionPosition[n].x, _wheelGroundPosition[n].y + (_wheelSuspensionPosition[n].y - _wheelGroundPosition[n].y) * _sideLeaningFactor[m], _wheelSuspensionPosition[n].z), ForceMode.Acceleration);
							if (!_vehicleNotDrivable[m])
							{
								_vehicleRigidbody[m].AddForceAtPosition(_vehicleForwardForce[m], new Vector3(_wheelSuspensionPosition[n].x, _wheelGroundPosition[n].y + (_wheelSuspensionPosition[n].y - _wheelGroundPosition[n].y) * _forwardLeaningFactor[m], _wheelSuspensionPosition[n].z), ForceMode.Acceleration);
							}
						}
						else
						{
							_vehicleRigidbody[m].AddForceAtPosition(Physics.gravity * _vehicleRigidbody[m].mass / (_vehicleEndWheelIndex[m] - _vehicleStartWheelIndex[m]), _wheelSuspensionPosition[n]);
						}
					}
				}
				if (_trailerNrWheels[m] > 0)
				{
					for (int num2 = _vehicleEndWheelIndex[m] - _trailerNrWheels[m]; num2 < _vehicleEndWheelIndex[m]; num2++)
					{
						if (_wheelRaycatsDistance[num2] != 0f)
						{
							_trailerRigidbody[m].AddForceAtPosition(_wheelSuspensionForce[num2], _wheelGroundPosition[num2]);
							_trailerRigidbody[m].AddForceAtPosition(_trailerForwardForce[m], _wheelSuspensionPosition[num2], ForceMode.VelocityChange);
							if (_vehicleIsBraking[m])
							{
								_trailerRigidbody[m].AddForceAtPosition(_vehicleForwardForce[m] / _trailerNrWheels[m], _wheelSuspensionPosition[num2], ForceMode.Acceleration);
							}
						}
					}
				}
				if (!_vehicleNotDrivable[m])
				{
					Vector3 angularVelocity = _vehicleRigidbody[m].angularVelocity;
					angularVelocity.y = 0f;
					_vehicleRigidbody[m].angularVelocity = angularVelocity;
					if (num != 0)
					{
						_vehicleRigidbody[m].MoveRotation(_vehicleRigidbody[m].rotation * Quaternion.Euler(0f, _vehicleRotationAngle[m], 0f));
					}
					VehicleAI.Drive(m, _vehicleCurrentGear[m], _vehicleNeedWaypoint[m], AllVehiclesData.AllVehicles[m].VehicleType);
					_allVehiclesData.AllVehicles[m].ApplyAdditionalForces(_turnAngle[m]);
				}
			}
		}

		private bool SteamSkipBehaviourThisStep(int vehicleIndex)
		{
			if (((_steamFixedStep + vehicleIndex) & 1) == 0 || _activeCameraPositions.Length == 0)
			{
				return false;
			}
			return math.distancesq(_vehiclePosition[vehicleIndex], _activeCameraPositions[0]) > 10000f;
		}

		private void SteamReadSuspension()
		{
			if (!_steamBodyVelocity.IsCreated)
			{
				_steamBodyVelocity = new NativeArray<float3>(_nrOfVehicles, Allocator.Persistent);
				_steamBodyAngularVelocity = new NativeArray<float3>(_nrOfVehicles, Allocator.Persistent);
				_steamBodyCenterOfMass = new NativeArray<float3>(_nrOfVehicles, Allocator.Persistent);
			}
			for (int i = 0; i < _nrOfVehicles; i++)
			{
				Rigidbody rigidbody = _vehicleRigidbody[i];
				_steamBodyVelocity[i] = rigidbody.velocity;
				_steamBodyAngularVelocity[i] = rigidbody.angularVelocity;
				_steamBodyCenterOfMass[i] = rigidbody.worldCenterOfMass;
			}
			new SteamReadSuspensionJob
			{
				ExcludedWheels = _excludedWheels,
				WheelVehicle = _wheelAssociatedCar,
				BodyVelocity = _steamBodyVelocity,
				BodyAngularVelocity = _steamBodyAngularVelocity,
				BodyCenterOfMass = _steamBodyCenterOfMass,
				Position = _wheelSuspensionPosition,
				PointVelocity = _wheelVelocity
			}.ScheduleReadOnly(_suspensionConnectPoints, 32).Complete();
		}

		private void SteamDisposeSuspension()
		{
			if (_steamBodyVelocity.IsCreated)
			{
				_steamBodyVelocity.Dispose();
			}
			if (_steamBodyAngularVelocity.IsCreated)
			{
				_steamBodyAngularVelocity.Dispose();
			}
			if (_steamBodyCenterOfMass.IsCreated)
			{
				_steamBodyCenterOfMass.Dispose();
			}
		}

		private static float SteamForwardLean(float stored)
		{
			if (!SteamPerfFlags.IsOn("trafficLean"))
			{
				return stored;
			}
			return Mathf.Max(stored, 1f - SteamMaxForwardLean);
		}

		private int SteamApplyWheelForces(int i)
		{
			Rigidbody rigidbody = _vehicleRigidbody[i];
			Vector3 worldCenterOfMass = rigidbody.worldCenterOfMass;
			float mass = rigidbody.mass;
			int num = _vehicleStartWheelIndex[i];
			int num2 = _vehicleEndWheelIndex[i];
			int num3 = num2 - _trailerNrWheels[i];
			Vector3 vector = _vehicleLaterlForce[i];
			Vector3 vector2 = _vehicleForwardForce[i];
			bool flag = !_vehicleNotDrivable[i];
			float num4 = _sideLeaningFactor[i];
			float num5 = SteamForwardLean(_forwardLeaningFactor[i]);
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			Vector3 zero3 = Vector3.zero;
			Vector3 zero4 = Vector3.zero;
			int num6 = 0;
			for (int j = num; j < num3; j++)
			{
				Vector3 vector3 = _wheelSuspensionPosition[j];
				if (_wheelRaycatsDistance[j] != 0f)
				{
					num6++;
					Vector3 vector4 = _wheelGroundPosition[j];
					Vector3 vector5 = _wheelSuspensionForce[j];
					zero += vector5;
					zero2 += Vector3.Cross(vector4 - worldCenterOfMass, vector5);
					float num7 = vector3.y - vector4.y;
					Vector3 vector6 = new Vector3(vector3.x, vector4.y + num7 * num4, vector3.z);
					zero3 += vector;
					zero4 += Vector3.Cross(vector6 - worldCenterOfMass, vector);
					if (flag)
					{
						Vector3 vector7 = new Vector3(vector3.x, vector4.y + num7 * num5, vector3.z);
						zero3 += vector2;
						zero4 += Vector3.Cross(vector7 - worldCenterOfMass, vector2);
					}
				}
				else
				{
					Vector3 vector8 = Physics.gravity * mass / (num2 - num);
					zero += vector8;
					zero2 += Vector3.Cross(vector3 - worldCenterOfMass, vector8);
				}
			}
			rigidbody.AddForce(zero + zero3 * mass, ForceMode.Force);
			rigidbody.AddTorque(zero2, ForceMode.Force);
			rigidbody.AddTorque(zero4, ForceMode.Acceleration);
			return num6;
		}

		private void SteamTryRemoveNext()
		{
			_indexToRemove++;
			if (_indexToRemove == _nrOfVehicles)
			{
				_indexToRemove = 0;
			}
			if (_vehicleReadyToRemove[_indexToRemove] && (!_excludedVehicles[_indexToRemove] || _addExcludedVehicle[_indexToRemove]) && _vehicleRigidbody[_indexToRemove].gameObject.activeSelf && AllVehiclesData.AllVehicles[_vehicleListIndex[_indexToRemove]].CanBeRemoved())
			{
				_vehicleReadyToRemove[_indexToRemove] = false;
				DensityManager.RemoveVehicle(_indexToRemove, force: false);
			}
		}

		private void Update()
		{
			if (_initialized)
			{
				TimeManager.UpdateTime();
				for (int i = 0; i < _nrOfVehicles; i++)
				{
					AllVehiclesData.AllVehicles[i].SetBrakeLights(_vehicleIsBraking[i]);
				}
				new SteamReadWheelPoseJob
				{
					ExcludedWheels = _excludedWheels,
					Position = _wheelSuspensionPosition,
					Right = _wheelRightDirection
				}.ScheduleReadOnly(_suspensionConnectPoints, 32).Complete();
				_updateWheelJob = new UpdateWheelJob
				{
					WheelsOrigin = _wheelSuspensionPosition,
					DownDirection = _vehicleDownDirection,
					WheelRotation = _wheelRotation,
					TurnAngle = _turnAngle,
					WheelRadius = _wheelRadius,
					MaxSuspension = _wheelMaxSuspension,
					RayCastDistance = _wheelRaycatsDistance,
					NrOfVehicles = _nrOfVehicles,
					CanSteer = _wheelCanSteer,
					VehicleIndex = _wheelAssociatedCar,
					ExcludedWheels = _excludedWheels
				};
				_updateWheelJobHandle = _updateWheelJob.Schedule(_wheelsGraphics);
				_updateWheelJobHandle.Complete();
				_updateTriggerJob = new UpdateTriggerJob
				{
					TurnAngle = _turnAngle,
					ExcludedVehicles = _excludedVehicles
				};
				_updateTriggerJobHandle = _updateTriggerJob.Schedule(_vehicleTrigger);
				_updateTriggerJobHandle.Complete();
				_indexToRemove++;
				if (_indexToRemove == _nrOfVehicles)
				{
					_indexToRemove = 0;
				}
				_activeCameraIndex = UnityEngine.Random.Range(0, _activeCameraPositions.Length);
				DensityManager.UpdateVehicleDensity(_activeCameras[_activeCameraIndex].position, _activeCameras[_activeCameraIndex].forward, _activeCameraPositions[_activeCameraIndex]);
				if (_vehicleReadyToRemove[_indexToRemove] && (!_excludedVehicles[_indexToRemove] || (_excludedVehicles[_indexToRemove] && _addExcludedVehicle[_indexToRemove])) && _vehicleRigidbody[_indexToRemove].gameObject.activeSelf && AllVehiclesData.AllVehicles[_vehicleListIndex[_indexToRemove]].CanBeRemoved())
				{
					_vehicleReadyToRemove[_indexToRemove] = false;
					DensityManager.RemoveVehicle(_indexToRemove, force: false);
				}
				for (int num = _nrOfVehicles / 30; num > 0; num--)
				{
					SteamTryRemoveNext();
				}
				for (int j = 0; j < _activeCameras.Length; j++)
				{
					_activeCameraPositions[j] = _activeCameras[j].transform.position;
				}
				IntersectionManager.UpdateIntersections(TimeManager.RealTimeSinceStartup);
				ActiveCellsManager.UpdateGrid(_activeSquaresLevel, _activeCameraPositions);
			}
		}

		public void UpdateCamera(Transform[] activeCameras)
		{
			if (_initialized)
			{
				if (activeCameras.Length != _activeCameraPositions.Length)
				{
					_activeCameraPositions = new NativeArray<float3>(activeCameras.Length, Allocator.Persistent);
				}
				_activeCameras = activeCameras;
				DensityManager.UpdateCameraPositions(activeCameras);
			}
		}

		public float GetSteeringAngle(int vehicleIndex)
		{
			if (!_initialized)
			{
				return 0f;
			}
			if (vehicleIndex >= 0 && vehicleIndex < _nrOfVehicles)
			{
				return _turnAngle[vehicleIndex];
			}
			UnityEngine.Debug.LogError($"Vehicle index {vehicleIndex} is invalid. It should be between 0 and {_nrOfVehicles}");
			return 0f;
		}

		public void SetActiveSquaresLevel(int activeSquaresLevel)
		{
			if (_initialized)
			{
				_activeSquaresLevel = activeSquaresLevel;
				DensityManager.UpdateActiveSquaresLevel(activeSquaresLevel);
			}
		}

		public void StopVehicleDriving(GameObject vehicle)
		{
			if (_initialized)
			{
				int vehicleIndex = AllVehiclesData.GetVehicleIndex(vehicle);
				if (vehicleIndex != -1)
				{
					_vehicleNotDrivable[vehicleIndex] = true;
				}
			}
		}

		public void ResumeVehicleDriving(GameObject vehicle)
		{
			if (_initialized)
			{
				int vehicleIndex = AllVehiclesData.GetVehicleIndex(vehicle);
				if (vehicleIndex != -1)
				{
					_vehicleNotDrivable[vehicleIndex] = false;
					API.AddWaypointAndClear(API.GetClosestWaypointInDirection(vehicle.transform.position, vehicle.transform.forward).ListIndex, vehicleIndex);
				}
			}
		}

		public void InstantiateVehicleWithPath(Vector3 position, VehicleTypes vehicleType, Vector3 destination, UnityAction<VehicleComponent, int> completeMethod)
		{
			List<int> path = PathFindingManager.GetPath(position, destination, vehicleType);
			if (path != null)
			{
				DensityManager.RequestVehicleAtPosition(position, vehicleType, completeMethod, path);
			}
		}

		public void SetDestination(int vehicleIndex, Vector3 position)
		{
			if (PathFindingManager != null)
			{
				List<int> pathToDestination = PathFindingManager.GetPathToDestination(vehicleIndex, AllVehiclesData.AllVehicles[vehicleIndex].MovementInfo.GetWaypointIndex(0), position, AllVehiclesData.AllVehicles[vehicleIndex].VehicleType);
				if (pathToDestination != null)
				{
					AllVehiclesData.AllVehicles[vehicleIndex].MovementInfo.SetPath(pathToDestination);
				}
			}
		}

		public void ExcludeVehicleFromSystem(int vehicleIndex)
		{
			if (vehicleIndex >= 0 && vehicleIndex < _nrOfVehicles)
			{
				_excludedVehicles[vehicleIndex] = true;
				for (int i = 0; i < _wheelAssociatedCar.Length; i++)
				{
					if (_wheelAssociatedCar[i] == vehicleIndex)
					{
						_excludedWheels[i] = true;
					}
				}
			}
			else
			{
				UnityEngine.Debug.LogError($"Vehicle index {vehicleIndex} is invalid. It should be between 0 and {_nrOfVehicles}");
			}
		}

		public void RestoreRemovedVehicleToSystem(int vehicleIndex)
		{
			if (vehicleIndex >= 0 && vehicleIndex < _nrOfVehicles)
			{
				_addExcludedVehicle[vehicleIndex] = true;
			}
			else
			{
				UnityEngine.Debug.LogError($"Vehicle index {vehicleIndex} is invalid. It should be between 0 and {_nrOfVehicles}");
			}
		}

		private void NewVehicleAdded(int vehicleIndex, int waypointIndex)
		{
			ref NativeArray<float3> vehicleTargetWaypointPosition = ref _vehicleTargetWaypointPosition;
			float3 value = (_vehicleStopPosition[vehicleIndex] = TrafficWaypointsData.AllTrafficWaypoints[waypointIndex].Position);
			vehicleTargetWaypointPosition[vehicleIndex] = value;
			_vehicleNextWaypointPosition[vehicleIndex] = TrafficWaypointsData.AllTrafficWaypoints[waypointIndex].Position;
			_vehicleStopPosition[vehicleIndex] = TrafficSystemConstants.DEFAULT_POSITION;
			_vehiclePowerStep[vehicleIndex] = AllVehiclesData.AllVehicles[vehicleIndex].PowerStep;
			_vehicleBrakeStep[vehicleIndex] = AllVehiclesData.AllVehicles[vehicleIndex].BrakeStep;
			_vehicleIsBraking[vehicleIndex] = false;
			_vehicleNeedWaypoint[vehicleIndex] = false;
			_vehicleNotDrivable[vehicleIndex] = false;
			_addExcludedVehicle[vehicleIndex] = false;
			if (_excludedVehicles[vehicleIndex])
			{
				_excludedVehicles[vehicleIndex] = false;
				for (int i = 0; i < _wheelAssociatedCar.Length; i++)
				{
					if (_wheelAssociatedCar[i] == vehicleIndex)
					{
						_excludedWheels[i] = false;
					}
				}
			}
			_vehicleCurrentGear[vehicleIndex] = 1;
			_vehicleTargetGear[vehicleIndex] = 1;
			_turnAngle[vehicleIndex] = 0f;
			Vector3 velocity = AllVehiclesData.AllVehicles[vehicleIndex].GetForwardVector() * AllVehiclesData.AllVehicles[vehicleIndex].MovementInfo.GetFirstWaypointSpeed() / 2f;
			_vehicleRigidbody[vehicleIndex].velocity = velocity;
			if (_trailerNrWheels[vehicleIndex] != 0)
			{
				_trailerRigidbody[vehicleIndex].velocity = _vehicleRigidbody[vehicleIndex].velocity;
			}
		}

		private void DestinationChanged(int vehicleIndex, Vector3 newPosition)
		{
			_vehicleNeedWaypoint[vehicleIndex] = false;
			_vehicleTargetWaypointPosition[vehicleIndex] = newPosition;
			_vehicleNextWaypointPosition[vehicleIndex] = AllVehiclesData.GetVehicle(vehicleIndex).MovementInfo.GetPosition(1);
		}

		public void RefreshVehiclePositionsAfterGlobalOffset()
		{
			if (_allVehiclesData == null)
			{
				return;
			}
			VehicleComponent[] allVehicles = _allVehiclesData.AllVehicles;
			for (int i = 0; i < allVehicles.Length; i++)
			{
				if (!(allVehicles[i] == null) && allVehicles[i].gameObject.activeSelf)
				{
					MovementInfo movementInfo = allVehicles[i].MovementInfo;
					if (movementInfo != null && movementInfo.PathLength != 0)
					{
						movementInfo.RefreshPositionsFromWaypointData(TrafficWaypointsData);
						_vehicleTargetWaypointPosition[i] = movementInfo.GetPosition(0);
						_vehicleNextWaypointPosition[i] = movementInfo.GetPosition(1);
					}
				}
			}
		}

		private void OnDestroy()
		{
			try
			{
				_wheelSpringForce.Dispose();
				_raycastLengths.Dispose();
				_wCircumferences.Dispose();
				_wheelRadius.Dispose();
				_vehicleVelocity.Dispose();
				_trailerVelocity.Dispose();
				_vehicleMaxSteer.Dispose();
				_suspensionConnectPoints.Dispose();
				SteamDisposeSuspension();
				_wheelsGraphics.Dispose();
				_wheelGroundPosition.Dispose();
				_wheelVelocity.Dispose();
				_wheelRotation.Dispose();
				_turnAngle.Dispose();
				_wheelRaycatsResult.Dispose();
				_wheelRaycastCommand.Dispose();
				_wheelCanSteer.Dispose();
				_wheelAssociatedCar.Dispose();
				_vehicleEndWheelIndex.Dispose();
				_vehicleStartWheelIndex.Dispose();
				_vehicleNrOfWheels.Dispose();
				_trailerNrWheels.Dispose();
				_vehicleDownDirection.Dispose();
				_vehicleForwardDirection.Dispose();
				_vehicleRotationAngle.Dispose();
				_vehicleStopPosition.Dispose();
				_vehicleRightDirection.Dispose();
				_vehicleTargetWaypointPosition.Dispose();
				_vehicleNextWaypointPosition.Dispose();
				_vehiclePosition.Dispose();
				_vehicleGroundDirection.Dispose();
				_vehicleReadyToRemove.Dispose();
				_vehicleListIndex.Dispose();
				_vehicleNeedWaypoint.Dispose();
				_wheelRaycatsDistance.Dispose();
				_wheelRightDirection.Dispose();
				_wheelNormalDirection.Dispose();
				_wheelMaxSuspension.Dispose();
				_wheelSuspensionForce.Dispose();
				_vehicleForwardForce.Dispose();
				_vehicleLaterlForce.Dispose();
				_vehicleSteeringStep.Dispose();
				_vehicleCurrentGear.Dispose();
				_vehicleTargetGear.Dispose();
				_vehicleDrag.Dispose();
				_vehicleWheelDistance.Dispose();
				_vehiclePowerStep.Dispose();
				_vehicleBrakeStep.Dispose();
				_vehicleTrigger.Dispose();
				_vehicleType.Dispose();
				_vehicleIsBraking.Dispose();
				_vehicleNotDrivable.Dispose();
				_activeCameraPositions.Dispose();
				_trailerForwardForce.Dispose();
				_trailerRightDirection.Dispose();
				_trailerDrag.Dispose();
				_massDifference.Dispose();
				_wheelSuspensionPosition.Dispose();
				_vehicleDistanceToStop.Dispose();
				_wheelSpringStiffness.Dispose();
				_excludedVehicles.Dispose();
				_excludedWheels.Dispose();
				_addExcludedVehicle.Dispose();
				_groundedWheels.Dispose();
				_accelerationPercent.Dispose();
				_brakePercent.Dispose();
				_requiredBrakePower.Dispose();
				_steerPercent.Dispose();
				_vehicleMaxAllowedSpeed.Dispose();
				_stopTargetReached.Dispose();
				_vehicleTargetSpeed.Dispose();
				_sideLeaningFactor.Dispose();
				_forwardLeaningFactor.Dispose();
			}
			catch
			{
			}
			Events.OnChangeDestination = (Events.ChangeDestination)Delegate.Remove(Events.OnChangeDestination, new Events.ChangeDestination(DestinationChanged));
			Events.OnVehicleActivated = (Events.VehicleActivated)Delegate.Remove(Events.OnVehicleActivated, new Events.VehicleActivated(NewVehicleAdded));
			DestroyableManager.Instance.DestroyAll();
			_instance = null;
		}
	}
}
