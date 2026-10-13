using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Free_drive;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.EventSystem
{
	public class ActivityZoneTrigger : MonoBehaviour, IEventZone
	{
		public const int CreateObjectsPerFrameCount = 10;

		public SceneType SceneTypeForConfig;

		public int IndexForConfig;

		public const string ColliderName = "Collider";

		private int _zoneIndex;

		public float TargetPassTime;

		public float TargetPenaltyTime;

		public EventZoneMapElement MapModel;

		public int MaxCollectiblesCount;

		private SceneType _sceneType;

		private bool _isMusicStart;

		public bool UseRandomPosition;

		[SerializeField]
		private Transform[] CollectablePlaces;

		[SerializeField]
		private EventZoneCollectible ZoneCollectiblePrefab;

		[SerializeField]
		private EventZoneCollectiblesPosDatas EventZoneCollectiblesConfig;

		private List<(Vector3, Quaternion)> _collectablePlaces = new List<(Vector3, Quaternion)>();

		private bool _started;

		private List<EventZoneCollectible> _cachedCollectables = new List<EventZoneCollectible>();

		private TimedActivityZoneController _timedActivityZoneController;

		private IPrefabFactory _prefabFactory;

		private OptionalDependency<TimedActivityZoneController> _timedActivityZoneControllerDep;

		private OptionalDependency<MapCamera> _mapCameraDep;

		private IDisposable _soundEventObservable;

		private bool _isStarted;

		private int _collectablesCount;

		private FreeDriveProvider _freeDriveProvider;

		private MapCamera _mapCamera;

		private int _currentGiggleStep;

		private bool _isDestroyed;

		private bool _isInZone;

		private CollectiblePoolContainer _collectiblePoolContainer;

		private float _zoneRadius;

		private IDisposable _checkZone;

		private VehicleData _vehicleData;

		public int ZoneIndex => _zoneIndex;

		public bool CantPlay
		{
			get
			{
				if (MapModel.ZoneState != ZoneState.Started)
				{
					return MapModel.ZoneState == ZoneState.Cooldown;
				}
				return true;
			}
		}

		public int CollectiblesCount => _collectablesCount;

		public SceneType SceneType => _sceneType;

		[Inject]
		[UnityEngine.Scripting.Preserve]
		public void Construct(IPrefabFactory prefabFactory, FreeDriveProvider freeDriveProvider, CollectiblePoolContainer collectiblePoolContainer, OptionalDependency<TimedActivityZoneController> timedActivityZoneController, OptionalDependency<MapCamera> mapCamera)
		{
			_timedActivityZoneControllerDep = timedActivityZoneController;
			_freeDriveProvider = freeDriveProvider;
			_prefabFactory = prefabFactory;
			_mapCameraDep = mapCamera;
			_collectiblePoolContainer = collectiblePoolContainer;
		}

		private void Awake()
		{
			_timedActivityZoneController = _timedActivityZoneControllerDep?.Value;
			_mapCamera = _mapCameraDep?.Value;
		}

		private void Start()
		{
			if (!FlagsStorage.IsLevelScene)
			{
				Vector3 lossyScale = base.transform.lossyScale;
				_zoneRadius = Mathf.Max(lossyScale.x, lossyScale.z) * 0.5f;
				if ((bool)_freeDriveProvider.FirstCarMultiPlayer)
				{
					_vehicleData = _freeDriveProvider.FirstCarMultiPlayer.GetComponent<VehicleData>();
				}
				_checkZone = Observable.Interval(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
				{
					CheckZone();
				}).AddTo(this);
				_timedActivityZoneController.RegisterZone(this);
				DelayedInit().Forget();
			}
		}

		private async UniTaskVoid DelayedInit()
		{
			await UniTask.Yield();
			if (!_isDestroyed)
			{
				EventZoneMapElement mapModel = MapModel;
				mapModel.OnCooldownEnd = (Action)Delegate.Combine(mapModel.OnCooldownEnd, new Action(OnCooldownEnd));
				if (!MapModel)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		private void OnCooldownEnd()
		{
			MapModel.ChangeState(ZoneState.None);
			if (_isInZone)
			{
				PrepareZone();
				ActivateZone(isPrepared: true);
				SetEventSwitchState("state_event_zone_filter", "in_zone");
				SetZoneAction();
			}
		}

		private void OnZoneEnter()
		{
			if ((bool)_vehicleData && !CantPlay && (!_vehicleData.Driver || !(_vehicleData.Driver != V.DriverPerson)))
			{
				SetEventSwitchState("state_event_zone_filter", "in_zone");
				SetZoneAction();
				ActivateZone();
			}
		}

		private void OnZoneExit()
		{
			if ((bool)_vehicleData && (!_vehicleData.Driver || !(_vehicleData.Driver != V.DriverPerson)))
			{
				SetEventSwitchState("state_event_zone_filter", "out_zone");
				_timedActivityZoneController.ShowZoneParticle(state: false);
				if (MapModel.ZoneState != ZoneState.Started)
				{
					_isMusicStart = false;
					DeactivateCollectables();
				}
				else
				{
					_started = false;
					MapModel.ChangeState(ZoneState.Pause);
					_timedActivityZoneController.StartPenaltyTimer();
				}
			}
		}

		private void ActivateZone(bool isPrepared = false)
		{
			if (!_isMusicStart)
			{
				if (!isPrepared)
				{
					PrepareZone();
				}
				_isMusicStart = true;
				_timedActivityZoneController.OnZoneEntered();
				_timedActivityZoneController.LogStart(MaxCollectiblesCount, SceneType.ToString(), ZoneIndex.ToString());
			}
		}

		private void CheckZone()
		{
			if ((bool)_freeDriveProvider.FirstCarMultiPlayer && !_vehicleData)
			{
				_vehicleData = _freeDriveProvider.FirstCarMultiPlayer.GetComponent<VehicleData>();
			}
			if ((bool)_vehicleData)
			{
				float num = Vector3.Distance(_freeDriveProvider.FirstCarMultiPlayer.transform.position, base.transform.position);
				if (!_isInZone && num <= _zoneRadius)
				{
					_isInZone = true;
					OnZoneEnter();
				}
				else if (_isInZone && num >= _zoneRadius + 1f)
				{
					_isInZone = false;
					OnZoneExit();
				}
			}
		}

		public void PrepareZone()
		{
			DeactivateCollectables();
			ShufflePositions();
			CreateCollectables();
			MapModel.ChangeState(ZoneState.Prepared);
		}

		private void StartNewAction()
		{
			_timedActivityZoneController.ShowZoneParticle(state: true);
			ChangeCollectablesState(state: true, showMapIcon: true).Forget();
			_timedActivityZoneController.SubscribeZone(this);
			_started = true;
		}

		private void ResumeOldAction()
		{
			_timedActivityZoneController.ShowZoneParticle(state: true);
			_timedActivityZoneController.ContinueActivity();
			MapModel.ChangeState(ZoneState.Started);
		}

		private void OnActionCooldown()
		{
		}

		private void SetZoneAction()
		{
			switch (MapModel.ZoneState)
			{
			case ZoneState.None:
				_timedActivityZoneController.ShowZoneParticle(state: true);
				break;
			case ZoneState.Prepared:
				_timedActivityZoneController.ShowZoneParticle(state: true);
				break;
			case ZoneState.Started:
				StartNewAction();
				break;
			case ZoneState.Pause:
				ResumeOldAction();
				break;
			case ZoneState.Cooldown:
				OnActionCooldown();
				break;
			}
		}

		public void OnCollect()
		{
			_collectablesCount++;
			if (_collectablesCount == 1)
			{
				MapModel.ChangeState(ZoneState.Started);
				SetZoneAction();
			}
			if (_collectablesCount == MaxCollectiblesCount)
			{
				MapModel.ChangeState(ZoneState.Cooldown);
				_timedActivityZoneController.FinishActivity();
			}
			_timedActivityZoneController.SendLogProgress();
		}

		private void ShufflePositions()
		{
			for (int i = 0; i < _collectablePlaces.Count - 1; i++)
			{
				int num = UnityEngine.Random.Range(i, _collectablePlaces.Count);
				List<(Vector3, Quaternion)> collectablePlaces = _collectablePlaces;
				int index = i;
				List<(Vector3, Quaternion)> collectablePlaces2 = _collectablePlaces;
				int index2 = num;
				(Vector3, Quaternion) tuple = _collectablePlaces[num];
				(Vector3, Quaternion) tuple2 = _collectablePlaces[i];
				(Vector3, Quaternion) tuple3 = (collectablePlaces[index] = tuple);
				tuple3 = (collectablePlaces2[index2] = tuple2);
			}
		}

		public void CreateCollectables()
		{
			CreateCollectiblesAsync().Forget();
		}

		private async UniTaskVoid CreateCollectiblesAsync()
		{
			new List<IMovableCollectible>();
			List<IMovableCollectible> list = await _collectiblePoolContainer.GetCollectiblePool(MaxCollectiblesCount);
			if (!_isDestroyed)
			{
				for (int i = 0; i < list.Count; i++)
				{
					EventZoneCollectible eventZoneCollectible = (EventZoneCollectible)list[i];
					list[i].MoveCollectible(_collectablePlaces[i].Item1, _collectablePlaces[i].Item2);
					eventZoneCollectible.Init(this, i);
					_cachedCollectables.Add(eventZoneCollectible);
					eventZoneCollectible.gameObject.SetActive(value: false);
				}
				if (_freeDriveProvider != null && (bool)_freeDriveProvider.FreeDriveControl)
				{
					_freeDriveProvider.FreeDriveControl.HideBigMap();
				}
				ChangeCollectablesState(state: true, showMapIcon: false).Forget();
			}
		}

		private async UniTaskVoid ChangeCollectablesState(bool state, bool showMapIcon)
		{
			for (int i = 0; i < _cachedCollectables.Count; i++)
			{
				if (i <= MaxCollectiblesCount)
				{
					_cachedCollectables[i].gameObject.SetActive(state);
					_cachedCollectables[i].SetIconState(showMapIcon);
					await UniTask.Delay(100);
					if (_isDestroyed)
					{
						break;
					}
				}
			}
		}

		public void DeactivateCollectables()
		{
			for (int i = 0; i < _cachedCollectables.Count; i++)
			{
				if ((bool)_cachedCollectables[i])
				{
					_cachedCollectables[i].ChangeState(state: false);
				}
			}
			_cachedCollectables.Clear();
		}

		private void SetPositions(ZoneConfigData zoneConfigData)
		{
			for (int i = 0; i < zoneConfigData.Zones.Count; i++)
			{
				_collectablePlaces.Add((zoneConfigData.Zones[i], zoneConfigData.Rotation[i]));
			}
		}

		public void Init(EventType eventID, int zoneIndex, SceneType sceneType)
		{
			_sceneType = sceneType;
			_zoneIndex = zoneIndex;
			ZoneConfigData positions = EventZoneCollectiblesConfig.GetPositions(_zoneIndex, _sceneType);
			if (positions != null)
			{
				MaxCollectiblesCount = (positions.IsRandomPosition ? positions.TargetCount : positions.ActiveCollectiblesCount);
				SetTimes(positions);
				SetPositions(positions);
				SetZone(positions);
			}
		}

		private void SetZone(ZoneConfigData zoneConfigData)
		{
			base.transform.localPosition = base.transform.parent.InverseTransformPoint(zoneConfigData.ZonePosition);
			base.gameObject.transform.localScale = zoneConfigData.ZoneScale;
		}

		private void SetTimes(ZoneConfigData zoneConfigData)
		{
			TargetPassTime = zoneConfigData.TotalPassTime;
			TargetPenaltyTime = zoneConfigData.TotalPenaltyTime;
		}

		private void SetEventSwitchState(string switchName, string state)
		{
			AudioWrapper.SetState(switchName, state);
		}

		public async UniTaskVoid DelayedResetCollectibles()
		{
			await UniTask.Delay(TimeSpan.FromSeconds(2.0));
			if (!_isDestroyed)
			{
				ResetCollectibles();
			}
		}

		private void ResetCollectibles()
		{
			_isMusicStart = false;
			_collectablesCount = 0;
			DeactivateCollectables();
		}

		private void OnDestroy()
		{
			_checkZone?.Dispose();
			_isDestroyed = true;
			if ((bool)MapModel)
			{
				EventZoneMapElement mapModel = MapModel;
				mapModel.OnCooldownEnd = (Action)Delegate.Remove(mapModel.OnCooldownEnd, new Action(OnCooldownEnd));
			}
		}
	}
}
