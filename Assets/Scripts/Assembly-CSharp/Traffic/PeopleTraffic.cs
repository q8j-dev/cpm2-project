using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Steam.Pedestrians;
using _Car_Parking.Scripts.Steam.Perf;
using _Car_Parking.Scripts.Steam.Traffic;

namespace Traffic
{
	public class PeopleTraffic : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Write name of folder containing people prefabs for people traffic")]
		private string _peopleResourceFolderName;

		[SerializeField]
		[Tooltip("Scriptable object containing people traffic points")]
		private SpawnPointsScriptableObject _trafficPointsSO;

		[Tooltip("How frequently to check if 2 person can conversate. For performance reasons set big value (around 1)")]
		[SerializeField]
		private float _checkPeopleFreq = 0.5f;

		[SerializeField]
		[Tooltip("Distance between 2 person that may conversate")]
		private float _peopleTalkDistance = 2f;

		[SerializeField]
		[Tooltip("Radius within to spawn person")]
		private float _spawnRadius = 1f;

		[Tooltip("Radius to check if people close on spawn")]
		[SerializeField]
		private float _checkClosePeopleRadius = 4f;

		[SerializeField]
		[Tooltip("Layers where may spawn people")]
		private LayerMask _spawnLayers;

		private readonly List<Behaviours> _allPeople = new List<Behaviours>();

		private readonly List<NavMeshSurface> _surfaces = new List<NavMeshSurface>(10);

		private readonly List<PointWithIndex> _closestPoints = new List<PointWithIndex>();

		private readonly Queue<Behaviours> _trafficDistanceCheckSchedule = new Queue<Behaviours>();

		private const float StalledCheckSeconds = 2f;

		private float _stalledCheckTimer;

		private readonly Queue<Behaviours> _peopleToSpawn = new Queue<Behaviours>();

		private int[] _trafficContainingSceneIndexes = new int[0];

		private List<Vector3> _walkPoints = new List<Vector3>();

		private GameObject[] _peoplePrefabs;

		private float _checkTimeOut;

		private Camera _camera;

		private bool _isTrafficSceneLoaded;

		private CancellationTokenSource _tokenSourcePlaceTraffic;

		private bool _isPlacingTraffic;

		private readonly WaypointSpatialGrid _waypointGrid = new WaypointSpatialGrid();

		private readonly List<Vector3> _withinPointsBuffer = new List<Vector3>();

		[HideInInspector]
		public ScenePoints points;

		[HideInInspector]
		public List<Behaviours> visibleAgents;

		[HideInInspector]
		public bool isSideCarTrafficEnabled = true;

		public const int TrafficNavAreaId = 8;

		public const float MaxDensity = 40f;

		public const float TrafficSpawnRadius = 150f;

		public const float VisibleSpawnDistance = 90f;

		public const int DefaultDensity = 50;

		private const int PlaceAttempts = 5;

		private int _densitySetting;

		private const int SteamDistanceChecksDivisor = 30;

		private const int SteamPlacePerFrame = 20;

		private const double SteamSpawnBudgetMs = 3.0;

		private const int SteamWeightedSlots = 100;

		private bool _steamInitiating;

		private CharactersService _steamCharacters;

		private SteamPedestrianFactory _steamPedestrianFactory;

		private UniTaskCompletionSource _steamPedestriansBuild;

		private GameObject[] _steamPedestrianPrefabs;

		private NavMeshPath _steamPath;

		public static float SteamDodgeChance = 1f;

		public static float SteamDodgeMinSpeed = 7f;

		public static float SteamDodgeCorridor = 2.2f;

		public static float SteamDodgeLookAhead = 1.1f;

		private const int SteamDodgeCheckFrames = 4;

		private const float SteamDodgeMaxDistance = 14f;

		private readonly List<Behaviours> _steamDodging = new List<Behaviours>();

		private Rigidbody _steamDodgeCarBody;

		private GameObject _steamDodgeCar;

		private const float SteamSameLookDistance = 25f;

		private readonly List<int> _steamPrefabDeck = new List<int>();

		public static PeopleTraffic Instance { get; private set; }

		public int CurrentDensity
		{
			get
			{
				return _densitySetting;
			}
			set
			{
				_densitySetting = value;
			}
		}

		private void Awake()
		{
			PeopleTraffic peopleTraffic = UnityEngine.Object.FindObjectOfType<PeopleTraffic>();
			if (peopleTraffic != null && peopleTraffic != this)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			Instance = this;
			isSideCarTrafficEnabled = PlayerPrefs.GetInt("sideCarTraffic") == 1;
			SteamTrafficLimits.EnsureDefaultsApplied();
			CurrentDensity = PlayerPrefs.GetInt("peopleTrafficDensity", 50);
			CurrentDensity = Mathf.Min(CurrentDensity, 100);
			if ((bool)_trafficPointsSO)
			{
				_trafficContainingSceneIndexes = new int[_trafficPointsSO.peopleTrafficPoints.Count];
				for (int i = 0; i < _trafficContainingSceneIndexes.Length; i++)
				{
					_trafficContainingSceneIndexes[i] = _trafficPointsSO.peopleTrafficPoints[i].sceneIndex;
				}
			}
		}

		private void OnEnable()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
			SceneManager.sceneUnloaded += OnSceneUnloaded;
			MoveElements.GlobalPointMoved += RecalculateWayPoints;
		}

		private void OnDisable()
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
			SceneManager.sceneUnloaded -= OnSceneUnloaded;
			MoveElements.GlobalPointMoved -= RecalculateWayPoints;
			CancelTokenPlaceTraffic();
		}

		private void OnDestroy()
		{
			_allPeople.Clear();
			_surfaces.Clear();
			_trafficDistanceCheckSchedule.Clear();
			_peopleToSpawn.Clear();
			visibleAgents?.Clear();
			visibleAgents = null;
			Instance = null;
			CancelTokenPlaceTraffic();
		}

		private void Update()
		{
			if ((CurrentDensity == 0 && _allPeople.Count == 0) || !_isTrafficSceneLoaded)
			{
				return;
			}
			CheckTrafficStalled();
			int num = 1 + _trafficDistanceCheckSchedule.Count / 30;
			for (int i = 0; i < num; i++)
			{
				if (_trafficDistanceCheckSchedule.Count <= 0)
				{
					break;
				}
				Behaviours behaviours = _trafficDistanceCheckSchedule.Dequeue();
				if ((bool)behaviours)
				{
					if (behaviours.CheckIsFar() || SteamShouldRespawn(behaviours))
					{
						behaviours.DeactivateBehaviours();
						behaviours.gameObject.SetActive(value: false);
						_peopleToSpawn.Enqueue(behaviours);
						RunPlaceTraffic().Forget();
					}
					else
					{
						_trafficDistanceCheckSchedule.Enqueue(behaviours);
					}
				}
			}
			SteamUpdateDodge();
			if (Behaviours.SteamAlwaysWalk)
			{
				return;
			}
			_checkTimeOut -= Time.deltaTime;
			if (!(_checkTimeOut <= 0f))
			{
				return;
			}
			_checkTimeOut = _checkPeopleFreq;
			if (visibleAgents.Count <= 1)
			{
				return;
			}
			Behaviours behaviours2 = visibleAgents[UnityEngine.Random.Range(0, visibleAgents.Count)];
			if (behaviours2.isInConversation)
			{
				return;
			}
			for (int j = 0; j < visibleAgents.Count; j++)
			{
				if (!(visibleAgents[j] == behaviours2) && !visibleAgents[j].isInConversation)
				{
					Vector3 position = visibleAgents[j].transform.position;
					Vector3 position2 = behaviours2.transform.position;
					if ((position - position2).sqrMagnitude <= _peopleTalkDistance * _peopleTalkDistance)
					{
						visibleAgents[j].StartListen(position2);
						behaviours2.StartTalk(position);
						break;
					}
				}
			}
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			OnSceneLoadedInternal(scene, mode).Forget();
		}

		private async UniTaskVoid OnSceneLoadedInternal(Scene scene, LoadSceneMode mode)
		{
			if (_camera == null)
			{
				while (V.CoreGameManager == null)
				{
					await UniTask.Delay(TimeSpan.FromMilliseconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, base.destroyCancellationToken);
				}
				_camera = V.CoreGameManager.GetComponent<Camera>();
			}
			if (!_trafficContainingSceneIndexes.Contains(scene.buildIndex))
			{
				return;
			}
			UpdateTrafficPoints(scene);
			await UniTask.Delay(TimeSpan.FromMilliseconds(5000.0), ignoreTimeScale: false, PlayerLoopTiming.Update, base.destroyCancellationToken);
			if (!(this == null))
			{
				if (CurrentDensity == 0)
				{
					_isTrafficSceneLoaded = true;
					DisableTraffic();
					return;
				}
				visibleAgents = new List<Behaviours>(CurrentDensity);
				_checkTimeOut = _checkPeopleFreq;
				_isTrafficSceneLoaded = true;
				SetSurfacesEnabled(isEnabled: true);
				ResetCancelToken();
				StartTrafficWhenNavMeshReady().Forget();
			}
		}

		private void UpdateTrafficPoints(Scene scene)
		{
			RebuildTrafficPoints(scene.buildIndex);
		}

		private void RebuildTrafficPoints(int triggeredBySceneIndex, int excludeSceneIndex = -1)
		{
			Scene[] allLoadedScenes = ScenesHelper.GetAllLoadedScenes();
			List<Vector3> list = new List<Vector3>();
			for (int i = 0; i < _trafficPointsSO.peopleTrafficPoints.Count; i++)
			{
				int sceneIndex = _trafficPointsSO.peopleTrafficPoints[i].sceneIndex;
				if (sceneIndex == excludeSceneIndex)
				{
					continue;
				}
				Scene[] array = allLoadedScenes;
				foreach (Scene scene in array)
				{
					if (scene.buildIndex == sceneIndex)
					{
						list.AddRange(_trafficPointsSO.peopleTrafficPoints[i].pointPositions);
						break;
					}
				}
			}
			_walkPoints = list;
			points = new ScenePoints(triggeredBySceneIndex);
			points.pointPositions = new Vector3[list.Count];
			for (int k = 0; k < list.Count; k++)
			{
				points.pointPositions[k] = list[k] + V.GlobalOffset;
			}
			_waypointGrid.Build(points.pointPositions, 150f);
		}

		private void OnSceneUnloaded(Scene scene)
		{
			if (!_trafficContainingSceneIndexes.Contains(scene.buildIndex))
			{
				return;
			}
			Scene[] array = (from sc in ScenesHelper.GetAllLoadedScenes()
				where sc != scene
				select sc).ToArray();
			for (int num = 0; num < array.Length; num++)
			{
				Scene scene2 = array[num];
				if (_trafficContainingSceneIndexes.Contains(scene2.buildIndex))
				{
					RebuildTrafficPoints(scene2.buildIndex, scene.buildIndex);
					return;
				}
			}
			_isTrafficSceneLoaded = false;
			DisableTraffic();
		}

		public static void SubscribeNavMeshSurface(NavMeshSurface navMeshSurface)
		{
			if (!navMeshSurface)
			{
				return;
			}
			if (Instance != null && Instance._isTrafficSceneLoaded)
			{
				AttachSurface(navMeshSurface);
				return;
			}
			Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				if (Instance == null)
				{
					UnityEngine.Debug.LogError("People traffic Instance = null");
				}
				else if ((bool)navMeshSurface && navMeshSurface.gameObject.activeInHierarchy)
				{
					AttachSurface(navMeshSurface, 4f);
				}
			}).AddTo(navMeshSurface);
		}

		private static void AttachSurface(NavMeshSurface navMeshSurface, float delaySeconds = 0f)
		{
			if (!Instance._surfaces.Contains(navMeshSurface))
			{
				Instance._surfaces.Add(navMeshSurface);
			}
			if (Instance.CurrentDensity == 0)
			{
				navMeshSurface.enabled = false;
				return;
			}
			if (delaySeconds <= 0f)
			{
				navMeshSurface.enabled = true;
				return;
			}
			Observable.Timer(TimeSpan.FromSeconds(delaySeconds)).Subscribe((long _) =>
			{
				if ((bool)navMeshSurface)
				{
					navMeshSurface.enabled = true;
				}
			}).AddTo(Instance);
		}

		public static void UnsubscribeNavMeshSurface(NavMeshSurface navMeshSurface)
		{
			if ((bool)navMeshSurface)
			{
				navMeshSurface.enabled = false;
				if (Instance != null)
				{
					Instance._surfaces.Remove(navMeshSurface);
				}
			}
		}

		private void ResetCancelToken()
		{
			CancelTokenPlaceTraffic();
		}

		private void InitiateTraffic()
		{
			SteamInitiateTrafficGradually();
		}

		private void RunTraffic()
		{
			for (int i = 0; i < CurrentDensity; i++)
			{
				_peopleToSpawn.Enqueue(_allPeople[i]);
			}
			RunPlaceTraffic().Forget();
		}

		private void CheckTrafficStalled()
		{
			if (CurrentDensity <= 0 || _allPeople.Count <= 0 || _trafficDistanceCheckSchedule.Count != 0 || _peopleToSpawn.Count != 0)
			{
				_stalledCheckTimer = 0f;
				return;
			}
			_stalledCheckTimer += Time.deltaTime;
			if (!(_stalledCheckTimer < 2f))
			{
				_stalledCheckTimer = 0f;
				StartTrafficWhenNavMeshReady().Forget();
			}
		}

		private void DisableTraffic()
		{
			CancelTokenPlaceTraffic();
			_trafficDistanceCheckSchedule.Clear();
			for (int i = 0; i < _allPeople.Count; i++)
			{
				_allPeople[i].DeactivateBehaviours();
				_allPeople[i].gameObject.SetActive(value: false);
			}
		}

		public void ChangeTrafficDensity(int newDensity)
		{
			CurrentDensity = newDensity;
			if (_isTrafficSceneLoaded)
			{
				if (CurrentDensity > 0)
				{
					SetSurfacesEnabled(isEnabled: true);
					ResetCancelToken();
					StartTrafficWhenNavMeshReady().Forget();
				}
				else
				{
					DisableTraffic();
					SetSurfacesEnabled(isEnabled: false);
				}
			}
		}

		private void SetSurfacesEnabled(bool isEnabled)
		{
			for (int i = 0; i < _surfaces.Count; i++)
			{
				if ((bool)_surfaces[i])
				{
					_surfaces[i].enabled = isEnabled;
				}
			}
		}

		private async UniTaskVoid StartTrafficWhenNavMeshReady()
		{
			if (await WaitForNavMeshReady(base.destroyCancellationToken) && !(this == null) && _isTrafficSceneLoaded && CurrentDensity > 0)
			{
				if (visibleAgents == null)
				{
					visibleAgents = new List<Behaviours>(CurrentDensity);
				}
				if (_allPeople.Count < CurrentDensity)
				{
					InitiateTraffic();
				}
				else
				{
					RunTraffic();
				}
			}
		}

		private async UniTask<bool> WaitForNavMeshReady(CancellationToken cancellationToken)
		{
			float deadline = Time.realtimeSinceStartup + 15f;
			while (Time.realtimeSinceStartup < deadline)
			{
				if (IsNavMeshReady())
				{
					return true;
				}
				if (await UniTask.Delay(TimeSpan.FromMilliseconds(100.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow())
				{
					return false;
				}
			}
			return IsNavMeshReady();
		}

		private bool IsNavMeshReady()
		{
			NavMeshHit hit;
			return NavMesh.SamplePosition(V.DriverPerson ? V.DriverPerson.transform.position : base.transform.position, out hit, 150f, 8);
		}

		private Vector3 GetRandomPositionInCircle(Vector3 center)
		{
			for (int i = 0; i < 10; i++)
			{
				Vector2 vector = UnityEngine.Random.insideUnitCircle * _spawnRadius;
				if (Physics.Raycast(center + new Vector3(vector.x, 1.5f, vector.y), Vector3.down, out var hitInfo, 3f, _spawnLayers) && NavMesh.SamplePosition(hitInfo.point, out var hit, 2f, 8))
				{
					return hit.position;
				}
			}
			return center;
		}

		private void LoadPeopleResources()
		{
			if (!SteamTryLoadPedestrians())
			{
				_peoplePrefabs = Resources.LoadAll<GameObject>(_peopleResourceFolderName);
			}
		}

		private async UniTaskVoid RunPlaceTraffic()
		{
			if (_isPlacingTraffic)
			{
				return;
			}
			_isPlacingTraffic = true;
			CancellationTokenSource cts = (_tokenSourcePlaceTraffic = CancellationTokenSource.CreateLinkedTokenSource(base.destroyCancellationToken));
			try
			{
				await PlaceTrafficAsync(cts.Token);
			}
			catch (OperationCanceledException)
			{
			}
			finally
			{
				if (_tokenSourcePlaceTraffic == cts)
				{
					_tokenSourcePlaceTraffic = null;
					_isPlacingTraffic = false;
				}
				cts.Dispose();
			}
		}

		private async UniTask PlaceTrafficAsync(CancellationToken cancellationToken)
		{
			while (!TryGetClosestPoints())
			{
				await UniTask.Delay(TimeSpan.FromMilliseconds(2000.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}
			}
			for (int attempt = 0; attempt < 5; attempt++)
			{
				if (_peopleToSpawn.Count <= 0)
				{
					break;
				}
				if (attempt > 0 && (await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow() || !TryGetClosestPoints()))
				{
					break;
				}
				int countToSpawn = _peopleToSpawn.Count;
				for (int i = 0; i < countToSpawn; i++)
				{
					if (_peopleToSpawn.Count == 0)
					{
						break;
					}
					PointWithIndex pointWithIndex = _closestPoints[UnityEngine.Random.Range(0, _closestPoints.Count)];
					Vector3 randomPositionInCircle = GetRandomPositionInCircle(pointWithIndex.point);
					PlacePersonOnPosition(randomPositionInCircle, pointWithIndex.index);
					if ((i + 1) % 20 == 0 && i + 1 < countToSpawn)
					{
						await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
					}
				}
			}
		}

		private void PlacePersonOnPosition(Vector3 pos, int pointIndex)
		{
			Behaviours behaviours = _peopleToSpawn.Dequeue();
			if (_allPeople.Count > CurrentDensity)
			{
				_allPeople.Remove(behaviours);
				UnityEngine.Object.Destroy(behaviours.gameObject);
				if (_allPeople.Count != 0)
				{
					return;
				}
				{
					foreach (NavMeshSurface surface in _surfaces)
					{
						surface.enabled = false;
					}
					return;
				}
			}
			if (!NavMesh.SamplePosition(pos, out var _, 1f, 8))
			{
				_peopleToSpawn.Enqueue(behaviours);
				return;
			}
			if (!SteamCanReachWaypoint(pos, pointIndex))
			{
				_peopleToSpawn.Enqueue(behaviours);
				return;
			}
			if (SteamHasSameLookNearby(behaviours, pos))
			{
				_peopleToSpawn.Enqueue(behaviours);
				return;
			}
			behaviours.transform.position = pos;
			behaviours.currentPointIndex = pointIndex;
			int num = 0;
			foreach (Behaviours allPerson in _allPeople)
			{
				if ((allPerson.transform.position - pos).sqrMagnitude < _checkClosePeopleRadius * _checkClosePeopleRadius)
				{
					num++;
				}
			}
			if (num < 5)
			{
				behaviours.ActivateBehaviours();
				behaviours.gameObject.SetActive(value: true);
			}
			_trafficDistanceCheckSchedule.Enqueue(behaviours);
		}

		private bool TryGetClosestPoints()
		{
			_closestPoints.Clear();
			if (V.DriverPerson != null)
			{
				_waypointGrid.QueryRing(V.DriverPerson.transform.position, 90f, 150f, _closestPoints);
			}
			return _closestPoints.Count > 0;
		}

		private void InstantiatePerson()
		{
			if (_peoplePrefabs == null)
			{
				LoadPeopleResources();
			}
			if (_peoplePrefabs == null || _peoplePrefabs.Length == 0)
			{
				return;
			}
			GameObject gameObject = _peoplePrefabs[SteamNextPrefabIndex()];
			if ((bool)gameObject)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, base.transform);
				Behaviours component = gameObject2.GetComponent<Behaviours>();
				if (!component)
				{
					UnityEngine.Object.Destroy(gameObject2);
					return;
				}
				gameObject2.SetActive(value: true);
				component.SteamLookId = gameObject.GetInstanceID();
				component.SteamRandomizeInstance();
				_allPeople.Add(component);
				component.DeactivateBehaviours();
				component.gameObject.SetActive(value: false);
				_peopleToSpawn.Enqueue(component);
			}
		}

		private void RecalculateWayPoints()
		{
			for (int i = 0; i < points.pointPositions.Length; i++)
			{
				points.pointPositions[i] = _walkPoints[i] + V.GlobalOffset;
			}
			_waypointGrid.Build(points.pointPositions, 150f);
		}

		public void UpdateSideCarTrafficState(bool isActive)
		{
			isSideCarTrafficEnabled = isActive;
		}

		public bool TryGetRandomClosestPoint(out Vector3 point)
		{
			_withinPointsBuffer.Clear();
			if (V.DriverPerson != null)
			{
				_waypointGrid.QueryWithin(V.DriverPerson.transform.position, 150f, _withinPointsBuffer);
			}
			if (_withinPointsBuffer.Count > 0)
			{
				point = _withinPointsBuffer[UnityEngine.Random.Range(0, _withinPointsBuffer.Count)];
				return true;
			}
			point = Vector3.zero;
			return false;
		}

		private void CancelTokenPlaceTraffic()
		{
			_tokenSourcePlaceTraffic?.Cancel();
			_tokenSourcePlaceTraffic?.Dispose();
			_tokenSourcePlaceTraffic = null;
			_isPlacingTraffic = false;
		}

		public void SteamProvideCharacters(CharactersService characters)
		{
			if (_steamCharacters == null)
			{
				_steamCharacters = characters;
			}
		}

		public UniTask SteamBuildPedestriansAsync()
		{
			if (_steamPedestriansBuild == null)
			{
				_steamPedestriansBuild = new UniTaskCompletionSource();
				SteamBuildPedestriansInternalAsync(_steamPedestriansBuild).Forget();
			}
			return _steamPedestriansBuild.Task;
		}

		private async UniTaskVoid SteamBuildPedestriansInternalAsync(UniTaskCompletionSource completion)
		{
			try
			{
				SteamPedestrianConfig config = Resources.Load<SteamPedestrianConfig>("Configs/SteamPedestrianConfig");
				if (_steamCharacters != null && (bool)config)
				{
					_steamPedestrianFactory = new SteamPedestrianFactory(_steamCharacters, base.transform);
					_steamPedestrianPrefabs = SteamWeightedPrefabs(await _steamPedestrianFactory.BuildAsync(config, Environment.TickCount, base.destroyCancellationToken), config.Weights);
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception)
			{
			}
			finally
			{
				completion.TrySetResult();
			}
		}

		private static GameObject[] SteamWeightedPrefabs(List<SteamPedestrianTemplate> templates, SteamPedestrianConfig.RarityWeights weights)
		{
			Dictionary<PedestrianRarity, List<GameObject>> dictionary = new Dictionary<PedestrianRarity, List<GameObject>>();
			foreach (SteamPedestrianTemplate template in templates)
			{
				if (!dictionary.TryGetValue(template.Rarity, out var value))
				{
					value = (dictionary[template.Rarity] = new List<GameObject>());
				}
				value.Add(template.Prefab);
			}
			float num = 0f;
			foreach (PedestrianRarity key in dictionary.Keys)
			{
				num += Mathf.Max(0f, weights.Get(key));
			}
			List<GameObject> list2 = new List<GameObject>(100);
			foreach (KeyValuePair<PedestrianRarity, List<GameObject>> item in dictionary)
			{
				float num2 = Mathf.Max(0f, weights.Get(item.Key));
				if (!(num2 <= 0f))
				{
					int num3 = Mathf.Max(item.Value.Count, Mathf.RoundToInt(100f * num2 / num));
					for (int i = 0; i < num3; i++)
					{
						list2.Add(item.Value[i % item.Value.Count]);
					}
				}
			}
			if (list2.Count == 0)
			{
				foreach (SteamPedestrianTemplate template2 in templates)
				{
					list2.Add(template2.Prefab);
				}
			}
			return list2.ToArray();
		}

		private bool SteamTryLoadPedestrians()
		{
			if (_steamPedestrianPrefabs == null || _steamPedestrianPrefabs.Length == 0)
			{
				return false;
			}
			_peoplePrefabs = _steamPedestrianPrefabs;
			return true;
		}

		public async UniTask SteamRebuildCrowdAsync()
		{
			int density = CurrentDensity;
			ChangeTrafficDensity(0);
			foreach (Behaviours allPerson in _allPeople)
			{
				if ((bool)allPerson)
				{
					UnityEngine.Object.Destroy(allPerson.gameObject);
				}
			}
			_allPeople.Clear();
			_peopleToSpawn.Clear();
			_trafficDistanceCheckSchedule.Clear();
			visibleAgents?.Clear();
			_steamPedestrianFactory?.Dispose();
			_steamPedestrianFactory = null;
			_steamPedestrianPrefabs = null;
			_steamPedestriansBuild = null;
			_peoplePrefabs = null;
			await SteamBuildPedestriansAsync();
			ChangeTrafficDensity(density);
		}

		private void SteamUpdateDodge()
		{
			for (int num = _steamDodging.Count - 1; num >= 0; num--)
			{
				Behaviours behaviours = _steamDodging[num];
				if (!behaviours || !behaviours.SteamStepDodge())
				{
					_steamDodging.RemoveAt(num);
				}
			}
			if (Time.frameCount % 4 != 0 || !SteamPerfFlags.IsOn("pedDodge"))
			{
				return;
			}
			GameObject car = V.Car;
			if (!car)
			{
				return;
			}
			if (car != _steamDodgeCar)
			{
				_steamDodgeCar = car;
				_steamDodgeCarBody = car.GetComponent<Rigidbody>();
			}
			if (!_steamDodgeCarBody)
			{
				return;
			}
			Vector3 velocity = _steamDodgeCarBody.velocity;
			float magnitude = velocity.magnitude;
			if (magnitude < SteamDodgeMinSpeed)
			{
				return;
			}
			Vector3 position = car.transform.position;
			Vector3 vector = velocity / magnitude;
			float num2 = Mathf.Min(magnitude * SteamDodgeLookAhead, 14f);
			float num3 = 196f;
			foreach (Behaviours allPerson in _allPeople)
			{
				if (!allPerson || !allPerson.gameObject.activeSelf || !allPerson.SteamDodgeReady)
				{
					continue;
				}
				Vector3 vector2 = allPerson.transform.position - position;
				if (vector2.sqrMagnitude > num3)
				{
					continue;
				}
				float num4 = Vector3.Dot(vector2, vector);
				if (num4 <= 0f || num4 > num2)
				{
					continue;
				}
				Vector3 vector3 = vector2 - vector * num4;
				vector3.y = 0f;
				if (!(vector3.magnitude > SteamDodgeCorridor))
				{
					if (UnityEngine.Random.value > SteamDodgeChance)
					{
						allPerson.SteamSkipDodge();
						continue;
					}
					Vector3 escapeDir = ((vector3.sqrMagnitude > 0.04f) ? vector3.normalized : (Vector3.Cross(Vector3.up, vector) * ((UnityEngine.Random.value < 0.5f) ? 1f : (-1f))));
					allPerson.SteamStartDodge(escapeDir);
					_steamDodging.Add(allPerson);
				}
			}
		}

		public void SteamRefreshAvoidance()
		{
			foreach (Behaviours allPerson in _allPeople)
			{
				if ((bool)allPerson)
				{
					allPerson.SteamApplyAvoidance();
				}
			}
		}

		private int SteamNextPrefabIndex()
		{
			if (_steamPrefabDeck.Count == 0)
			{
				for (int i = 0; i < _peoplePrefabs.Length; i++)
				{
					_steamPrefabDeck.Add(i);
				}
				for (int num = _steamPrefabDeck.Count - 1; num > 0; num--)
				{
					int num2 = UnityEngine.Random.Range(0, num + 1);
					List<int> steamPrefabDeck = _steamPrefabDeck;
					int index = num;
					List<int> steamPrefabDeck2 = _steamPrefabDeck;
					int index2 = num2;
					int num3 = _steamPrefabDeck[num2];
					int num4 = _steamPrefabDeck[num];
					int num5 = (steamPrefabDeck[index] = num3);
					num5 = (steamPrefabDeck2[index2] = num4);
				}
			}
			int index3 = _steamPrefabDeck.Count - 1;
			int result = _steamPrefabDeck[index3];
			_steamPrefabDeck.RemoveAt(index3);
			return result;
		}

		private bool SteamHasSameLookNearby(Behaviours person, Vector3 pos)
		{
			if (person.SteamLookId < 0 || _peopleToSpawn.Count == 0)
			{
				return false;
			}
			float num = 625f;
			foreach (Behaviours allPerson in _allPeople)
			{
				if (!(allPerson == person) && (bool)allPerson && allPerson.gameObject.activeSelf && allPerson.SteamLookId == person.SteamLookId && (allPerson.transform.position - pos).sqrMagnitude < num)
				{
					return true;
				}
			}
			return false;
		}

		private bool SteamCanReachWaypoint(Vector3 position, int pointIndex)
		{
			Vector3[] pointPositions = points.pointPositions;
			if (pointPositions == null || pointIndex < 0 || pointIndex >= pointPositions.Length)
			{
				return true;
			}
			if (_steamPath == null)
			{
				_steamPath = new NavMeshPath();
			}
			if (NavMesh.CalculatePath(position, pointPositions[pointIndex], 8, _steamPath))
			{
				return _steamPath.status == NavMeshPathStatus.PathComplete;
			}
			return false;
		}

		private static bool SteamShouldRespawn(Behaviours person)
		{
			if (person.SteamStranded)
			{
				return !person.IsVisible;
			}
			return false;
		}

		private void SteamInitiateTrafficGradually()
		{
			if (!_steamInitiating)
			{
				SteamInitiateTrafficAsync().Forget();
			}
		}

		private async UniTaskVoid SteamInitiateTrafficAsync()
		{
			_steamInitiating = true;
			try
			{
				await SteamBuildPedestriansAsync();
				Stopwatch stopwatch = Stopwatch.StartNew();
				while (_isTrafficSceneLoaded && _allPeople.Count < CurrentDensity)
				{
					int count = _allPeople.Count;
					InstantiatePerson();
					if (_allPeople.Count == count)
					{
						break;
					}
					if (!(stopwatch.Elapsed.TotalMilliseconds < 3.0))
					{
						RunPlaceTraffic().Forget();
						await UniTask.Yield(PlayerLoopTiming.Update, base.destroyCancellationToken);
						stopwatch.Restart();
					}
				}
				if (_peopleToSpawn.Count > 0)
				{
					RunPlaceTraffic().Forget();
				}
				if (_allPeople.Count >= CurrentDensity)
				{
					_peoplePrefabs = null;
				}
			}
			catch (OperationCanceledException)
			{
			}
			finally
			{
				_steamInitiating = false;
			}
		}
	}
}
