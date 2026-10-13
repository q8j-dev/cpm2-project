using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Addler.Runtime.Core.LifetimeBinding;
using Cysharp.Threading.Tasks;
using Gley.TrafficSystem.Internal;
using Gley.UrbanSystem.Internal;
using MVPFree.MapIcons.Enum;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.Views;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.Gley;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam.Perf;
using _Car_Parking.Scripts.Steam.Traffic;

namespace Gley.TrafficSystem
{
	[HelpURL("https://gley.gitbook.io/mobile-traffic-system-v3/setup-guide/initializing-asset")]
	public class TrafficComponent : MonoBehaviour
	{
		private sealed class SteamLod
		{
			public LODGroup Group;

			public Renderer[] All;

			public Renderer[] Far;
		}

		[Header("Required Settings")]
		[Tooltip("Player is used to instantiate vehicles out of view")]
		public Transform player;

		[Tooltip("Max number of possible vehicles. Cannot be increased during game-play")]
		public int nrOfVehicles = 1;

		[Tooltip("Square located at this distance from the player are actively update. Ex: if set is to 2 -> intersections will update on a 2 square distance from the player")]
		[Header("Spawning")]
		[Header("Optional Settings")]
		public int activeSquareLevels = 1;

		[Tooltip("Minimum distance from the player where a vehicle can be instantiated. (If -1 the system will automatically determine this value)")]
		public float minDistanceToAdd = -1f;

		[Tooltip("Distance from the player where a vehicle can be removed. (If -1 the system will automatically determine this value)")]
		public float distanceToRemove = -1f;

		[Tooltip("How long yellow light is on. (If -1 the value from the intersection component will be used)")]
		[Header("Intersection")]
		public float yellowLightTime = -1f;

		[Tooltip("How long green light is on. (If -1 the value from the intersection component will be used)")]
		public float greenLightTime = -1f;

		[Tooltip("Nr of vehicles instantiated around the player from the start. Set it to something < nrOfVehicles for low density right at the start. (If -1 all vehicles will be instantiated from the beginning)")]
		[Header("Density")]
		public int initialActiveVehicles = -1;

		[Tooltip("Set high priority on roads for higher traffic density(ex highways). See priority setup")]
		public bool useWaypointPriority;

		[Tooltip("List of colors to randomly assign to traffic vehicles. Leave empty to keep prefab colors.")]
		[Header("Car Colors")]
		public List<Color> colorPool;

		[Header("Lights")]
		[Tooltip("Set the initial state of the car lights")]
		public bool lightsOn;

		[SerializeField]
		[Tooltip("The number of known waypoints for the vehicle")]
		[Header("Waypoints")]
		private int _defaultPathLength = 5;

		[Tooltip("Area to disable from the start if cars are not allowed to spawn there")]
		public Area disableWaypointsArea;

		public Vector3 GridCorner;

		public Vector3[] OriginWaypoints;

		public Vector3[,] OriginWaypointsGrid;

		private MapIconsModel _mapIconsModel;

		private GlobalCollectionsProvider _collectionsProvider;

		private FreeDriveProvider _freeDriveProvider;

		private SoundBankLoadService _soundBankService;

		private bool _npcTrafficBankHeld;

		public const int DefaultDensityValue = 50;

		private const string UI_MAPICON = "UI_mapiconcar";

		private readonly List<IconMapView> _trafficMapIcons = new List<IconMapView>();

		private AsyncOperationHandle<GameObject> _mapIconPrefabHandle;

		private FPSDisplay2 _fpsDisplay;

		private int _currentDensity;

		private float _smoothFps = 30f;

		private const float FPS_REMOVE_THRESHOLD = 28f;

		private const float FPS_CRITICAL_THRESHOLD = 20f;

		private const float FPS_RESTORE_THRESHOLD = 36f;

		private bool _isInitedGley_Data;

		private bool _isInitedGleyLightsOnly_Data;

		private bool _initRequested;

		private GameObject _trafficLoader;

		private AsyncOperationHandle<VehiclePool> _poolHandle;

		private SceneControl _sceneControl;

		private const string GLEY_DATA = "Gley_Final";

		private const string GLEY_DATA_SEMAPHORES = "Gley_Final_Lights";

		private const string GLEY_DATA_CAR_POOL = "VehiclePool";

		public TrafficWaypointsData TrafficWaypointsData;

		public GridData GreedData;

		private const double SteamPrewarmBudgetMs = 4.0;

		public static float SteamMapIconScale = 0.5f;

		public static float SteamMapIconScaleMinimap = 1f;

		private readonly Dictionary<GameObject, Stack<GameObject>> _steamPrewarmed = new Dictionary<GameObject, Stack<GameObject>>();

		private GameObject _steamPrewarmHolder;

		private bool _steamPrewarmStarted;

		private bool _steamPrewarmDone;

		private const float SteamLodDetailHeight = 0.06f;

		private const float SteamLodCullHeight = 0.01f;

		private static readonly string[] SteamLodNearOnly = new string[4] { "driver", "passanger", "passenger", "mirror" };

		private readonly List<SteamLod> _steamLods = new List<SteamLod>();

		private bool _steamLodCullApplied;

		private const float SteamLodDetailDistance = 60f;

		private const float SteamLodCullDistance = 150f;

		private const float SteamShadowOffDistance = 120f;

		private const float SteamShadowOnDistance = 100f;

		private readonly Dictionary<int, Renderer[]> _steamShadowCasters = new Dictionary<int, Renderer[]>();

		private readonly HashSet<int> _steamShadowsOff = new HashSet<int>();

		private Transform _steamShadowEye;

		private const float SteamTriggerMinSpeed = 20f;

		private const float SteamTriggerLengthStep = 0.1f;

		private const float SteamStuckCheckInterval = 0.5f;

		private const float SteamStuckSpeed = 0.5f;

		private const float SteamDeadlockSeconds = 8f;

		private const float SteamVisibleDeadlockSeconds = 20f;

		private const float SteamHeadStoppedSeconds = 15f;

		private const float SteamHeadAtStopSeconds = 30f;

		private const int SteamMaxChain = 8;

		private float[] _steamStuckTime;

		private float[] _steamStoppedTime;

		private readonly List<int> _steamDeadlockMembers = new List<int>();

		private bool _steamWatchdogRunning;

		[Inject]
		public void Construct(OptionalDependency<MapIconsModel> mapIconsModel, OptionalDependency<GlobalCollectionsProvider> collectionsProvider, OptionalDependency<FreeDriveProvider> freeDriveProvider, SceneControl sceneControl, SoundBankLoadService soundBankService)
		{
			_mapIconsModel = mapIconsModel.Value;
			_collectionsProvider = collectionsProvider.Value;
			_freeDriveProvider = freeDriveProvider.Value;
			_sceneControl = sceneControl;
			_soundBankService = soundBankService;
		}

		public async UniTaskVoid Init(Transform playerTransform)
		{
			if (!_initRequested)
			{
				_initRequested = true;
				minDistanceToAdd = 250f;
				distanceToRemove = 800f;
				activeSquareLevels = Mathf.Max(activeSquareLevels, 2);
				SteamTrafficLimits.EnsureDefaultsApplied();
				nrOfVehicles = 100;
				initialActiveVehicles = Mathf.Min(PlayerPrefs.GetInt("CarQuantity", 50), 30);
				await InitTrafficLightsWithData();
				MoveElements.GlobalPointMoved -= GlobalPointMoved;
				MoveElements.GlobalPointMoved += GlobalPointMoved;
				GlobalPointMoved();
				SteamFitActiveSquaresToSpawnDistance();
				TrafficOptions options = new TrafficOptions
				{
					ActiveSquaresLevel = activeSquareLevels,
					DisableWaypointsArea = disableWaypointsArea,
					DistanceToRemove = distanceToRemove,
					GreenLightTime = greenLightTime,
					InitialDensity = initialActiveVehicles,
					LightsOn = lightsOn,
					MinDistanceToAdd = minDistanceToAdd,
					UseWaypointPriority = useWaypointPriority,
					YellowLightTime = yellowLightTime,
					DefaultPathLength = _defaultPathLength,
					ColorPool = colorPool
				};
				if (_poolHandle.IsValid())
				{
					Addressables.Release(_poolHandle);
				}
				_poolHandle = Addressables.LoadAssetAsync<VehiclePool>("VehiclePool");
				VehiclePool vehiclePool = await _poolHandle;
				await SteamPrewarmVehiclesAsync(this.GetCancellationTokenOnDestroy());
				options.InstantiateVehicle = SteamTakePrewarmedVehicle;
				options.ModifyTriggerSize = SteamTriggerSizeModifier;
				API.Initialize(playerTransform, nrOfVehicles, vehiclePool, options);
				SteamReleasePrewarm();
				CreateTrafficMapIcons().Forget();
				_currentDensity = PlayerPrefs.GetInt("CarQuantity", 50);
				_currentDensity = Mathf.Min(_currentDensity, nrOfVehicles);
				SteamStartStuckWatchdog();
				API.SetTrafficDensity(_currentDensity);
			}
		}

		private void SteamFitActiveSquaresToSpawnDistance()
		{
			if ((bool)GreedData && GreedData.GridCellSize > 0 && !(minDistanceToAdd <= 0f))
			{
				int num = Mathf.CeilToInt(minDistanceToAdd / (float)GreedData.GridCellSize) + 1;
				if (num > activeSquareLevels)
				{
					activeSquareLevels = num;
				}
			}
		}

		private async UniTask InitTrafficLightsWithData()
		{
			if (!_isInitedGley_Data)
			{
				_isInitedGley_Data = true;
				SetNpcTrafficBank(enabled: true);
				if ((bool)_trafficLoader)
				{
					_trafficLoader.GetComponent<TrafficLoader>().RemoveLights();
					UnityEngine.Object.Destroy(_trafficLoader);
				}
				_trafficLoader = UnityEngine.Object.Instantiate(await Addressables.LoadAssetAsync<GameObject>("Gley_Final").BindTo(base.gameObject));
				TrafficLoader component = _trafficLoader.GetComponent<TrafficLoader>();
				component.Init(_sceneControl);
				component.StartSemaphoreCycle();
				TrafficWaypointsData = _trafficLoader.GetComponentInChildren<TrafficWaypointsData>();
				GreedData = _trafficLoader.GetComponentInChildren<GridData>();
				V.CoreGameManager.ClearAssets();
			}
		}

		public async UniTaskVoid InitTrafficLightsOnly()
		{
			if (!_isInitedGleyLightsOnly_Data)
			{
				_isInitedGleyLightsOnly_Data = true;
				if ((bool)_trafficLoader)
				{
					_trafficLoader.GetComponent<TrafficLoader>().RemoveLights();
					UnityEngine.Object.Destroy(_trafficLoader);
				}
				_trafficLoader = UnityEngine.Object.Instantiate(await Addressables.LoadAssetAsync<GameObject>("Gley_Final_Lights").BindTo(base.gameObject));
				TrafficLoader component = _trafficLoader.GetComponent<TrafficLoader>();
				component.Init(_sceneControl);
				component.StartSemaphoreCycle();
				V.CoreGameManager.ClearAssets();
			}
		}

		private void EvaluateFpsDensity()
		{
			if (_fpsDisplay == null)
			{
				Camera main = Camera.main;
				if (main != null)
				{
					_fpsDisplay = main.GetComponent<FPSDisplay2>();
				}
				if (_fpsDisplay == null)
				{
					return;
				}
			}
			int num = PlayerPrefs.GetInt("CarQuantity", 10);
			if (num <= 0)
			{
				return;
			}
			float currentFPS = _fpsDisplay.CurrentFPS;
			if (!float.IsNaN(currentFPS) && !float.IsInfinity(currentFPS) && !(currentFPS <= 0f))
			{
				_smoothFps = _smoothFps * 0.5f + currentFPS * 0.5f;
				int currentDensity = _currentDensity;
				if (_smoothFps < 20f)
				{
					_currentDensity -= 3;
				}
				else if (_smoothFps < 28f)
				{
					_currentDensity--;
				}
				else if (_smoothFps > 36f)
				{
					_currentDensity++;
				}
				_currentDensity = Mathf.Clamp(_currentDensity, 0, num);
				if (_currentDensity != currentDensity)
				{
					API.SetTrafficDensity(_currentDensity);
				}
			}
		}

		private void GlobalPointMoved()
		{
			bool flag = OriginWaypoints == null || OriginWaypoints.Length == 0;
			if (flag)
			{
				OriginWaypoints = new Vector3[TrafficWaypointsData.AllTrafficWaypoints.Length];
				OriginWaypointsGrid = new Vector3[GreedData.Grid.Length, GreedData.Grid[0].Row.Length];
				GridCorner = GreedData.GridCorner;
			}
			for (int i = 0; i < TrafficWaypointsData.AllTrafficWaypoints.Length; i++)
			{
				if (flag)
				{
					OriginWaypoints[i] = TrafficWaypointsData.AllTrafficWaypoints[i].Position;
				}
				TrafficWaypointsData.AllTrafficWaypoints[i].Position = OriginWaypoints[i] + V.GlobalOffset;
			}
			for (int j = 0; j < GreedData.Grid.Length; j++)
			{
				for (int k = 0; k < GreedData.Grid[j].Row.Length; k++)
				{
					if (flag)
					{
						OriginWaypointsGrid[j, k] = GreedData.Grid[j].Row[k].CellProperties.Center;
					}
					GreedData.Grid[j].Row[k].CellProperties.Center = OriginWaypointsGrid[j, k] + V.GlobalOffset;
				}
			}
			if (flag)
			{
				GridCorner = GreedData.GridCorner;
			}
			GreedData.GridCorner = GridCorner + V.GlobalOffset;
			if (!flag)
			{
				TrafficManager instance = TrafficManager.Instance;
				if (instance != null)
				{
					instance.RefreshVehiclePositionsAfterGlobalOffset();
				}
			}
		}

		private async UniTaskVoid CreateTrafficMapIcons()
		{
			if (_mapIconsModel == null || _collectionsProvider == null)
			{
				return;
			}
			VehicleComponent[] vehicles = API.GetAllVehicles();
			if (vehicles == null)
			{
				return;
			}
			_mapIconPrefabHandle = Addressables.LoadAssetAsync<GameObject>("UI_mapiconcar");
			GameObject gameObject = await _mapIconPrefabHandle;
			if (gameObject == null)
			{
				return;
			}
			VehicleComponent[] array = vehicles;
			foreach (VehicleComponent vehicleComponent in array)
			{
				if (vehicleComponent == null)
				{
					continue;
				}
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
				IconMapView component = gameObject2.GetComponent<IconMapView>();
				if (!component)
				{
					UnityEngine.Object.Destroy(gameObject2);
					continue;
				}
				_trafficMapIcons.Add(component);
				_mapIconsModel.IconsToDisableWhenZoomCam.Add(component);
				_mapIconsModel.AddMapIconModel(component, IconType.CarBlack);
				_collectionsProvider.RegisterMapIcon(component.transform);
				component.transform.parent = vehicleComponent.transform;
				component.transform.localPosition = Vector3.zero;
				component.transform.localEulerAngles = new Vector3(90f, -90f, 0f);
				component.BigMapRootScale = SteamMapIconScale;
				component.MiniMapRootScale = SteamMapIconScaleMinimap;
				component.ApplyMapRootScale(bigMap: false);
				SpriteRenderer spriteRenderer = component.SpriteRenderer;
				Color color = spriteRenderer.color;
				if (!FlagsStorage.IsMultiFree && vehicleComponent.meshRenderer != null)
				{
					color = vehicleComponent.meshRenderer.material.color;
				}
				spriteRenderer.color = color;
			}
			if ((bool)_freeDriveProvider?.MapCamera)
			{
				_freeDriveProvider.MapCamera.ZoomIcon();
			}
		}

		public void SetNpcTrafficBank(bool enabled)
		{
			if (enabled != _npcTrafficBankHeld)
			{
				_npcTrafficBankHeld = enabled;
				if (enabled)
				{
					_soundBankService?.TryLoadBank(SoundBankType.CpmNpcTraffic);
				}
				else
				{
					_soundBankService?.TryUnloadBank(SoundBankType.CpmNpcTraffic);
				}
			}
		}

		private void OnDestroy()
		{
			CancelInvoke("EvaluateFpsDensity");
			MoveElements.GlobalPointMoved -= GlobalPointMoved;
			SetNpcTrafficBank(enabled: false);
			if (_mapIconsModel != null)
			{
				foreach (IconMapView trafficMapIcon in _trafficMapIcons)
				{
					if (!(trafficMapIcon == null))
					{
						_mapIconsModel.IconsToDisableWhenZoomCam.Remove(trafficMapIcon);
						_mapIconsModel.RemoveMapIconModel(trafficMapIcon);
						_collectionsProvider?.UnregisterMapIcon(trafficMapIcon.transform);
						UnityEngine.Object.Destroy(trafficMapIcon.gameObject);
					}
				}
			}
			_trafficMapIcons.Clear();
			if (_mapIconPrefabHandle.IsValid())
			{
				Addressables.Release(_mapIconPrefabHandle);
			}
			if (_poolHandle.IsValid())
			{
				Addressables.Release(_poolHandle);
			}
			if ((bool)_trafficLoader)
			{
				UnityEngine.Object.Destroy(_trafficLoader);
				_trafficLoader = null;
			}
			TrafficWaypointsData = null;
			GreedData = null;
			OriginWaypoints = null;
			OriginWaypointsGrid = null;
		}

		public async UniTask SteamPrewarmVehiclesAsync(CancellationToken cancellationToken)
		{
			if (_steamPrewarmStarted)
			{
				await UniTask.WaitUntil(() => _steamPrewarmDone, PlayerLoopTiming.Update, cancellationToken);
				return;
			}
			_steamPrewarmStarted = true;
			try
			{
				VehiclePool vehiclePool = await Addressables.LoadAssetAsync<VehiclePool>("VehiclePool").BindTo(base.gameObject).WithCancellation(cancellationToken);
				if (!vehiclePool || vehiclePool.trafficCars == null || vehiclePool.trafficCars.Length == 0)
				{
					return;
				}
				_steamPrewarmHolder = new GameObject("[SteamTrafficPrewarm]");
				_steamPrewarmHolder.SetActive(value: false);
				_steamPrewarmHolder.transform.SetParent(base.transform, worldPositionStays: false);
				Stopwatch stopwatch = Stopwatch.StartNew();
				foreach (GameObject item in SteamVehicleOrder(vehiclePool, 100))
				{
					if ((bool)item)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(item, Vector3.zero, Quaternion.identity, _steamPrewarmHolder.transform);
						SteamSetupLod(gameObject);
						if (!_steamPrewarmed.TryGetValue(item, out var value))
						{
							value = (_steamPrewarmed[item] = new Stack<GameObject>());
						}
						value.Push(gameObject);
						if (!(stopwatch.Elapsed.TotalMilliseconds < 4.0))
						{
							await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
							stopwatch.Restart();
						}
					}
				}
			}
			finally
			{
				_steamPrewarmDone = true;
			}
		}

		private void SteamSetupLod(GameObject instance)
		{
			if (!instance || (bool)instance.GetComponentInChildren<LODGroup>(includeInactive: true))
			{
				return;
			}
			Renderer[] componentsInChildren = instance.GetComponentsInChildren<Renderer>(includeInactive: true);
			if (componentsInChildren.Length == 0)
			{
				return;
			}
			List<Renderer> list = new List<Renderer>(componentsInChildren.Length);
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				if (!SteamIsNearOnly(renderer.name))
				{
					list.Add(renderer);
				}
			}
			LODGroup lODGroup = instance.AddComponent<LODGroup>();
			SteamLod steamLod = new SteamLod
			{
				Group = lODGroup,
				All = componentsInChildren,
				Far = ((list.Count == componentsInChildren.Length) ? componentsInChildren : list.ToArray())
			};
			_steamLods.Add(steamLod);
			SteamApplyLod(steamLod, _steamLodCullApplied);
			lODGroup.RecalculateBounds();
		}

		private void SteamSyncLodMode()
		{
			bool flag = SteamPerfFlags.IsOn("trafficCull");
			if (flag == _steamLodCullApplied)
			{
				return;
			}
			_steamLodCullApplied = flag;
			foreach (SteamLod steamLod in _steamLods)
			{
				SteamApplyLod(steamLod, flag);
			}
		}

		private static void SteamApplyLod(SteamLod entry, bool cullFar)
		{
			if (!entry.Group)
			{
				return;
			}
			float num = 0.06f;
			float screenRelativeTransitionHeight = 0.01f;
			if (cullFar)
			{
				Camera main = Camera.main;
				float num2 = (main ? main.fieldOfView : 60f);
				float num3 = entry.Group.size * Mathf.Max(QualitySettings.lodBias, 0.01f) / (2f * Mathf.Tan(num2 * 0.5f * ((float)Math.PI / 180f)));
				if (num3 > 0f)
				{
					num = Mathf.Clamp(num3 / 60f, 0.002f, 0.5f);
					screenRelativeTransitionHeight = Mathf.Clamp(num3 / 150f, 0.001f, num * 0.9f);
				}
			}
			entry.Group.SetLODs(new LOD[2]
			{
				new LOD(num, entry.All),
				new LOD(screenRelativeTransitionHeight, entry.Far)
			});
		}

		private void SteamUpdateFarShadows(VehicleComponent[] vehicles)
		{
			if (!SteamPerfFlags.IsOn("trafficShadow"))
			{
				return;
			}
			if (!_steamShadowEye)
			{
				Camera main = Camera.main;
				if (!main)
				{
					return;
				}
				_steamShadowEye = main.transform;
			}
			Vector3 position = _steamShadowEye.position;
			float num = 14400f;
			float num2 = 10000f;
			foreach (VehicleComponent vehicleComponent in vehicles)
			{
				if (!vehicleComponent || !vehicleComponent.gameObject.activeSelf)
				{
					continue;
				}
				int instanceID = vehicleComponent.gameObject.GetInstanceID();
				bool flag = _steamShadowsOff.Contains(instanceID);
				float sqrMagnitude = (vehicleComponent.transform.position - position).sqrMagnitude;
				bool flag2 = (flag ? (sqrMagnitude > num2) : (sqrMagnitude > num));
				if (flag2 == flag)
				{
					continue;
				}
				Renderer[] array = SteamShadowCasters(vehicleComponent.gameObject);
				foreach (Renderer renderer in array)
				{
					if ((bool)renderer)
					{
						renderer.shadowCastingMode = ((!flag2) ? ShadowCastingMode.On : ShadowCastingMode.Off);
					}
				}
				if (flag2)
				{
					_steamShadowsOff.Add(instanceID);
				}
				else
				{
					_steamShadowsOff.Remove(instanceID);
				}
			}
		}

		private Renderer[] SteamShadowCasters(GameObject instance)
		{
			int instanceID = instance.GetInstanceID();
			if (_steamShadowCasters.TryGetValue(instanceID, out var value))
			{
				return value;
			}
			List<Renderer> list = new List<Renderer>();
			Renderer[] componentsInChildren = instance.GetComponentsInChildren<Renderer>(includeInactive: true);
			foreach (Renderer renderer in componentsInChildren)
			{
				if (renderer.shadowCastingMode == ShadowCastingMode.On)
				{
					list.Add(renderer);
				}
			}
			return _steamShadowCasters[instanceID] = list.ToArray();
		}

		private static bool SteamIsNearOnly(string name)
		{
			string[] steamLodNearOnly = SteamLodNearOnly;
			foreach (string value in steamLodNearOnly)
			{
				if (name.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					return true;
				}
			}
			return false;
		}

		private GameObject SteamTakePrewarmedVehicle(GameObject prefab, Transform parent)
		{
			if (!prefab || !_steamPrewarmed.TryGetValue(prefab, out var value) || value.Count == 0)
			{
				return null;
			}
			GameObject gameObject = value.Pop();
			if (!gameObject)
			{
				return null;
			}
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			gameObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
			return gameObject;
		}

		private void SteamReleasePrewarm()
		{
			_steamPrewarmed.Clear();
			if ((bool)_steamPrewarmHolder)
			{
				UnityEngine.Object.Destroy(_steamPrewarmHolder);
			}
			_steamPrewarmHolder = null;
		}

		private static void SteamTriggerSizeModifier(float currentSpeed, BoxCollider frontCollider, float maxSpeed, float minTriggerLength, float maxTriggerLength)
		{
			float num = ((currentSpeed < 20f) ? minTriggerLength : ((!(currentSpeed >= maxSpeed)) ? (minTriggerLength + (currentSpeed - 20f) * ((maxTriggerLength - minTriggerLength) / (maxSpeed - 20f))) : maxTriggerLength));
			Vector3 size = frontCollider.size;
			bool flag = num <= minTriggerLength || num >= maxTriggerLength;
			if (!(Mathf.Abs(size.z - num) < (flag ? 0.001f : 0.1f)))
			{
				size.z = num;
				frontCollider.size = size;
				Vector3 center = frontCollider.center;
				center.z = num / 2f;
				frontCollider.center = center;
			}
		}

		private void SteamStartStuckWatchdog()
		{
			if (!_steamWatchdogRunning)
			{
				_steamWatchdogRunning = true;
				SteamStuckWatchdogLoop(this.GetCancellationTokenOnDestroy()).Forget();
			}
		}

		private async UniTaskVoid SteamStuckWatchdogLoop(CancellationToken cancellationToken)
		{
			try
			{
				while (true)
				{
					await UniTask.Delay(TimeSpan.FromSeconds(0.5), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
					if (TrafficManager.Exists && TrafficManager.Instance.Initialized && TrafficManager.Instance.gameObject.activeInHierarchy)
					{
						VehicleComponent[] allVehicles = API.GetAllVehicles();
						if (allVehicles != null && allVehicles.Length != 0)
						{
							SteamSyncLodMode();
							SteamUpdateFarShadows(allVehicles);
							SteamUpdateStuckTimes(allVehicles);
							SteamBreakDeadlocks(allVehicles);
							SteamRemoveStalledHeads(allVehicles);
						}
					}
				}
			}
			catch (OperationCanceledException)
			{
			}
			finally
			{
				_steamWatchdogRunning = false;
			}
		}

		private void SteamUpdateStuckTimes(VehicleComponent[] vehicles)
		{
			if (_steamStuckTime == null || _steamStuckTime.Length != vehicles.Length)
			{
				_steamStuckTime = new float[vehicles.Length];
				_steamStoppedTime = new float[vehicles.Length];
			}
			for (int i = 0; i < vehicles.Length; i++)
			{
				VehicleComponent vehicleComponent = vehicles[i];
				bool flag = (bool)vehicleComponent && vehicleComponent.gameObject.activeSelf && !vehicleComponent.Ignored && vehicleComponent.GetCurrentSpeedMS() < 0.5f;
				_steamStoppedTime[i] = (flag ? (_steamStoppedTime[i] + 0.5f) : 0f);
				_steamStuckTime[i] = ((flag && (bool)SteamBlockingVehicle(vehicleComponent)) ? (_steamStuckTime[i] + 0.5f) : 0f);
			}
		}

		private static VehicleComponent SteamBlockingVehicle(VehicleComponent vehicle)
		{
			Obstacle closestObstacle = vehicle.MovementInfo.ClosestObstacle;
			if (closestObstacle.Collider == null || closestObstacle.ObstacleType != ObstacleTypes.TrafficVehicle)
			{
				return null;
			}
			if (closestObstacle.VehicleScript is VehicleComponent result)
			{
				return result;
			}
			Rigidbody attachedRigidbody = closestObstacle.Collider.attachedRigidbody;
			if (!attachedRigidbody)
			{
				return null;
			}
			return attachedRigidbody.GetComponent<VehicleComponent>();
		}

		private void SteamBreakDeadlocks(VehicleComponent[] vehicles)
		{
			for (int i = 0; i < vehicles.Length; i++)
			{
				if (_steamStuckTime[i] < 8f)
				{
					continue;
				}
				_steamDeadlockMembers.Clear();
				_steamDeadlockMembers.Add(i);
				bool flag = false;
				int num = i;
				for (int j = 0; j < 8; j++)
				{
					VehicleComponent vehicleComponent = SteamBlockingVehicle(vehicles[num]);
					if (!vehicleComponent)
					{
						break;
					}
					int listIndex = vehicleComponent.ListIndex;
					if (listIndex < 0 || listIndex >= vehicles.Length || _steamStuckTime[listIndex] <= 0f)
					{
						break;
					}
					if (listIndex == i)
					{
						flag = true;
						break;
					}
					if (_steamDeadlockMembers.Contains(listIndex))
					{
						break;
					}
					_steamDeadlockMembers.Add(listIndex);
					num = listIndex;
				}
				if (!flag)
				{
					continue;
				}
				int num2 = SteamPickDeadlockVictim(vehicles);
				if (num2 < 0)
				{
					continue;
				}
				API.RemoveVehicle(num2);
				foreach (int steamDeadlockMember in _steamDeadlockMembers)
				{
					_steamStuckTime[steamDeadlockMember] = 0f;
				}
				_steamStoppedTime[num2] = 0f;
			}
		}

		private int SteamPickDeadlockVictim(VehicleComponent[] vehicles)
		{
			int num = -1;
			int num2 = -1;
			float num3 = float.MaxValue;
			foreach (int steamDeadlockMember in _steamDeadlockMembers)
			{
				num3 = Mathf.Min(num3, _steamStuckTime[steamDeadlockMember]);
				if (num2 < 0 || _steamStoppedTime[steamDeadlockMember] > _steamStoppedTime[num2])
				{
					num2 = steamDeadlockMember;
				}
				if (vehicles[steamDeadlockMember].CanBeRemoved() && (num < 0 || _steamStoppedTime[steamDeadlockMember] > _steamStoppedTime[num]))
				{
					num = steamDeadlockMember;
				}
			}
			if (num >= 0)
			{
				return num;
			}
			if (!(num3 >= 20f))
			{
				return -1;
			}
			return num2;
		}

		private void SteamRemoveStalledHeads(VehicleComponent[] vehicles)
		{
			for (int i = 0; i < vehicles.Length; i++)
			{
				if (!(_steamStoppedTime[i] < 15f) && !(_steamStuckTime[i] > 0f))
				{
					MovementInfo movementInfo = vehicles[i].MovementInfo;
					Obstacle closestObstacle = movementInfo.ClosestObstacle;
					if ((!(closestObstacle.Collider != null) || closestObstacle.ObstacleType != ObstacleTypes.Player) && ((!movementInfo.HasStopWaypoints() && !movementInfo.HasGiveWayWaypoints()) || !(_steamStoppedTime[i] < 30f)))
					{
						API.RemoveVehicle(i);
						_steamStoppedTime[i] = 0f;
					}
				}
			}
		}

		private static List<GameObject> SteamVehicleOrder(VehiclePool pool, int nrOfVehicles)
		{
			CarType[] trafficCars = pool.trafficCars;
			List<GameObject> list = new List<GameObject>(nrOfVehicles);
			int num = Mathf.Min(trafficCars.Length, nrOfVehicles);
			for (int i = 0; i < num; i++)
			{
				list.Add(trafficCars[i].VehiclePrefab);
			}
			int num2 = nrOfVehicles - num;
			if (num2 <= 0)
			{
				return list;
			}
			float num3 = 0f;
			List<float> list2 = new List<float>(trafficCars.Length);
			CarType[] array = trafficCars;
			foreach (CarType carType in array)
			{
				num3 += (float)carType.Percent;
				list2.Add(num3);
			}
			float num4 = num3 / (float)num2;
			int num5 = 0;
			for (int k = 0; k < num2; k++)
			{
				while ((float)(k + 1) * num4 > list2[num5])
				{
					num5++;
					if (num5 >= trafficCars.Length)
					{
						num5 = trafficCars.Length - 1;
						break;
					}
				}
				list.Add(trafficCars[num5].VehiclePrefab);
			}
			return list;
		}
	}
}
