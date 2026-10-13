using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Scripts.Garage;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Racing.CameraManagement.SpecificTypes;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.OfflineMode;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.WindowSystem;

namespace Racing.CameraManagement
{
	public class CameraManager : MonoBehaviour
	{
		[FormerlySerializedAs("mainController")]
		public CoreGameManager CoreGameManager;

		[SerializeField]
		private BannerShowController BannerShowController;

		private const float ToMainCamTransitDelay = 3f;

		private const float SpeedDollyCartForSetupPlayer = 2f;

		private const string PersoncustomizationcompletedPrefKey = "PersonCustomizationCompleted";

		private readonly Dictionary<RacingCameraType, ICameraHandler> _allCameras = new Dictionary<RacingCameraType, ICameraHandler>();

		private ICameraHandler _currentCamera;

		private OptionalDependency<CinemachineBrain> _cinemachineBrainDep;

		private CinemachineBrain _cinemachineBrain;

		private CinemachineBlendDefinition? _nextBlend;

		private ScreenFader _screenFader;

		private ISlotService _slotService;

		private IGarageSelectorService _garageSelectorService;

		private IOfflineModeSystem _offlineModeSystem;

		private bool _isDestroyed;

		public GameObject firstCam;

		public CinemachineDollyCartsWrapper[] AllDollyCartsByType;

		public CinemachineDollyCart[] AllDollyCartsToSellCar;

		public AnimationCurve speedCurve;

		private IPrefabFactory _prefabFactory;

		private CinemachineCore.GetBlendOverrideDelegate _defaultBlendDelegate;

		public GarageMainCameraHandler garageCam;

		private ManagerUI _managerUI;

		[SerializeField]
		private List<PresetCameraWrapper> AllCameraPresetsByGarage = new List<PresetCameraWrapper>();

		private GaragePositionsConfig _garagePositionsConfig;

		public Transform targetToLookAt;

		public CinemachineDollyCart[] AllDollyCartsToSetupPlayer;

		private OptionalDependency<MenuControl> _menuControlDep;

		private MenuControl _menuControl;

		private bool firstStarting;

		private Transform tempFirstToLook;

		private Vector3 targetPosFirstToLook;

		private CinemachineDollyCart[] CurrentCinemachineDollyCarts => AllDollyCartsByType[_garageSelectorService.CurrentGarageIndex].CinemachineDollyCarts;

		public RacingCameraType CurrentCameraType
		{
			get
			{
				if (_currentCamera == null)
				{
					return RacingCameraType.None;
				}
				return _currentCamera.VirtualCameraType;
			}
		}

		public ICameraHandler CurrentCamera => _currentCamera;

		public CinemachineBrain CurrentBrain => _cinemachineBrain;

		public CinemachineBlendDefinition CutBlend { get; } = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0f);

		public bool HasCurrentCamera => _currentCamera != null;

		public float CurrentCameraZoom => _currentCamera.ZoomDistance;

		[field: SerializeField]
		public PersonShopCameraHandler PersonShopCam { get; private set; }

		public CameraOverridePreset[] AllCameraPresets => AllCameraPresetsByGarage[_garageSelectorService.CurrentGarageIndex].Presets;

		public CustomMoveCameraToTarget CustomMoveCameraToTarget { get; private set; }

		public CinemachineDollyCart CurrentDollyCart { get; private set; }

		[Inject]
		public void Construct(ScreenFader screenFader, ISlotService slotService, IGarageSelectorService garageSelectorService, IOfflineModeSystem offlineModeSystem, ManagerUI managerUI, GaragePositionsConfig garagePositionsConfig, IPrefabFactory prefabFactory, OptionalDependency<CinemachineBrain> cinemachineBrain, OptionalDependency<MenuControl> menuControl)
		{
			_screenFader = screenFader;
			_cinemachineBrainDep = cinemachineBrain;
			_slotService = slotService;
			_garageSelectorService = garageSelectorService;
			_offlineModeSystem = offlineModeSystem;
			_managerUI = managerUI;
			_garagePositionsConfig = garagePositionsConfig;
			_prefabFactory = prefabFactory;
			_menuControlDep = menuControl;
		}

		private void Awake()
		{
			AwakeAsync().Forget();
		}

		private async UniTaskVoid AwakeAsync()
		{
			CancellationToken token = this.GetCancellationTokenOnDestroy();
			_cinemachineBrain = _cinemachineBrainDep?.Value;
			_menuControl = _menuControlDep?.Value;
			if (!garageCam)
			{
				garageCam = GetComponentInChildren<GarageMainCameraHandler>();
			}
			if (_cinemachineBrain != null && _prefabFactory != null)
			{
				CustomMoveCameraToTarget = _prefabFactory.Create<CustomMoveCameraToTarget>(new object[1] { _cinemachineBrain });
			}
			await UniTask.WaitUntil(() => V.DriverPerson, PlayerLoopTiming.Update, token);
			if (!this)
			{
				return;
			}
			if ((bool)firstCam && firstCam.activeSelf)
			{
				if (PlayerPrefs.GetInt("PersonCustomizationCompleted", 0) != 1 && (_slotService == null || !_slotService.HaveCarInAnyGarage) && (_offlineModeSystem == null || _offlineModeSystem.Disabled))
				{
					StartMoveSetupPersonCamera().Forget();
				}
				else if (_slotService != null && !_slotService.HaveCarInAnyGarage)
				{
					StartMoveVeryFirstCam().Forget();
				}
			}
			await UniTask.Yield(PlayerLoopTiming.Update, token);
			if ((bool)this)
			{
				_defaultBlendDelegate = CinemachineCore.GetBlendOverride;
				CinemachineCore.GetBlendOverride = GetBlendOverride;
				if (_slotService.HaveCarInAnyGarage)
				{
					ActivateCamera(RacingCameraType.GarageLookAround, base.transform);
				}
			}
		}

		public async UniTaskVoid StartMoveVeryFirstCam()
		{
			if (_isDestroyed || !this || !firstCam)
			{
				return;
			}
			CinemachineDollyCart dollyCart = CurrentCinemachineDollyCarts[UnityEngine.Random.Range(0, CurrentCinemachineDollyCarts.Length)];
			bool enableFirstStart = !_slotService.HaveCarInAnyGarage && _offlineModeSystem.Disabled;
			firstCam.SetActive(value: true);
			if (enableFirstStart)
			{
				dollyCart = AllDollyCartsToSellCar[UnityEngine.Random.Range(0, AllDollyCartsToSellCar.Length)];
			}
			else
			{
				firstStarting = true;
				await UniTask.DelayFrame(1);
				if (_isDestroyed)
				{
					return;
				}
				_cinemachineBrain.enabled = true;
				await UniTask.DelayFrame(1);
				if (_isDestroyed)
				{
					return;
				}
				if (_cinemachineBrain.enabled)
				{
					_cinemachineBrain.enabled = false;
				}
			}
			SetDolly(dollyCart);
			if (!enableFirstStart)
			{
				tempFirstToLook = new GameObject("tempFirstToLook").transform;
				tempFirstToLook.position = dollyCart.m_Path.EvaluatePosition(2f);
				targetPosFirstToLook = tempFirstToLook.position;
				firstCam.GetComponent<CinemachineVirtualCamera>().LookAt = tempFirstToLook;
				firstCam.transform.LookAt(tempFirstToLook);
				V.CoreGameManager.transform.LookAt(tempFirstToLook);
				if (_isDestroyed)
				{
					return;
				}
				if ((bool)_menuControl)
				{
					_menuControl.ShowMainMenu();
				}
				while (CoreGameManager.menuControl.firstLoading)
				{
					await UniTask.Delay(100);
					if (_isDestroyed)
					{
						return;
					}
				}
			}
			dollyCart.enabled = true;
			_cinemachineBrain.enabled = true;
			await UniTask.Yield();
			_managerUI.CloseBootLoadingWindow();
			float speedK = 20f;
			if (enableFirstStart)
			{
				speedK = 5f;
			}
			await UniTask.Delay(100);
			if (_isDestroyed)
			{
				return;
			}
			float speedByTouch = 1f;
			while (dollyCart.m_Position < dollyCart.m_Path.PathLength)
			{
				float num = dollyCart.m_Position / dollyCart.m_Path.PathLength;
				dollyCart.m_Speed = speedCurve.Evaluate(num) * speedK * speedByTouch;
				if ((bool)tempFirstToLook)
				{
					if (num > 0.4f && (bool)V.DriverPerson)
					{
						targetPosFirstToLook = V.DriverPerson.transform.position;
					}
					else
					{
						targetPosFirstToLook = dollyCart.m_Path.EvaluatePosition(dollyCart.m_Position + 2f);
					}
				}
				if (num > 0.8f && !enableFirstStart)
				{
					break;
				}
				await UniTask.Delay(100);
				if (_isDestroyed)
				{
					return;
				}
				if (Input.touchCount > 0)
				{
					speedByTouch++;
				}
				if ((bool)V.DriverPerson)
				{
					Vector2 vector = new Vector2(V.DriverPerson.transform.position.x, V.DriverPerson.transform.position.z);
					if (Vector2.Distance(garageCam ? new Vector2(garageCam.transform.position.x, garageCam.transform.position.z) : vector, vector) < 5f)
					{
						break;
					}
				}
			}
			if (!enableFirstStart)
			{
				if ((bool)firstCam)
				{
					firstCam.SetActive(value: false);
				}
				if ((bool)garageCam)
				{
					if ((bool)V.DriverPerson)
					{
						garageCam.edgeTracker.transform.position = V.DriverPerson.transform.position;
						garageCam.edgeTrackerTargetPos = V.DriverPerson.transform.position;
					}
					garageCam.edgeReached = false;
					garageCam.UpdateBodyFollowExternal();
				}
			}
			if ((bool)tempFirstToLook)
			{
				UnityEngine.Object.Destroy(tempFirstToLook.gameObject);
			}
			firstStarting = false;
			BannerShowController?.Initialize();
		}

		private async UniTask StartMoveSetupPersonCamera()
		{
			if (_isDestroyed || !this || !firstCam)
			{
				return;
			}
			CurrentDollyCart = AllDollyCartsToSetupPlayer[UnityEngine.Random.Range(0, AllDollyCartsToSetupPlayer.Length)];
			CurrentDollyCart.m_Speed = 2f;
			CinemachineVirtualCamera vCam = firstCam.GetComponent<CinemachineVirtualCamera>();
			vCam.GetCinemachineComponent<CinemachineHardLockToTarget>().m_Damping = 0f;
			SetDolly(CurrentDollyCart);
			firstCam.SetActive(value: true);
			CurrentDollyCart.enabled = true;
			_cinemachineBrain.enabled = true;
			_managerUI.CloseWaitingWindow();
			_managerUI.CloseBootLoadingWindow();
			await UniTask.Yield();
			if (_isDestroyed)
			{
				return;
			}
			targetToLookAt = new GameObject("targetToLookAt").transform;
			targetToLookAt.position = _garagePositionsConfig.GaragePositionsDatas[_garageSelectorService.CurrentGarageIndex].PositionTargetForCamera;
			CinemachineComponentBase[] components = vCam.GetComponents<CinemachineComponentBase>();
			foreach (CinemachineComponentBase cinemachineComponentBase in components)
			{
				if (cinemachineComponentBase.Stage == CinemachineCore.Stage.Aim)
				{
					UnityEngine.Object.Destroy(cinemachineComponentBase);
				}
			}
			if (!vCam.GetCinemachineComponent<CinemachineHardLookAt>())
			{
				vCam.AddCinemachineComponent<CinemachineHardLookAt>();
			}
			firstCam.GetComponent<CinemachineVirtualCamera>().LookAt = targetToLookAt;
			while (CurrentDollyCart.m_Position < CurrentDollyCart.m_Path.PathLength)
			{
				await UniTask.Delay(100);
				if (_isDestroyed)
				{
					return;
				}
			}
			firstCam.GetComponent<CinemachineVirtualCamera>().LookAt = null;
			UnityEngine.Object.Destroy(targetToLookAt.gameObject);
			CurrentDollyCart = null;
		}

		private void Update()
		{
			if ((bool)tempFirstToLook)
			{
				tempFirstToLook.transform.position = Vector3.Lerp(tempFirstToLook.transform.position, targetPosFirstToLook, Time.deltaTime * 3f);
			}
		}

		private void SetDolly(CinemachineDollyCart dollyCart)
		{
			firstCam.GetComponent<CinemachineVirtualCamera>().Follow = dollyCart.transform;
			firstCam.transform.rotation = dollyCart.transform.rotation;
			dollyCart.transform.position = dollyCart.m_Path.EvaluatePosition(0f);
			firstCam.transform.position = dollyCart.m_Path.EvaluatePosition(0f);
			CoreGameManager.transform.position = dollyCart.m_Path.EvaluatePosition(0f);
			CoreGameManager.transform.LookAt(dollyCart.m_Path.EvaluatePosition(1f));
			dollyCart.m_Position = 0f;
		}

		private void OnDestroy()
		{
			CinemachineCore.GetBlendOverride = _defaultBlendDelegate;
			_isDestroyed = true;
			CustomMoveCameraToTarget?.StopCustomMoveCam();
		}

		public void RegisterCamera(ICameraHandler handler)
		{
			_allCameras.TryAdd(handler.VirtualCameraType, handler);
		}

		public void UnregisterCamera(ICameraHandler registrar)
		{
			if (_allCameras.ContainsKey(registrar.VirtualCameraType))
			{
				_allCameras.Remove(registrar.VirtualCameraType);
			}
		}

		public ICameraHandler TryGetCamera(RacingCameraType racingCameraType)
		{
			return _allCameras.GetValueOrDefault(racingCameraType);
		}

		public void ActivateCamera(RacingCameraType racingCameraType, Transform target = null, CinemachineBlendDefinition? blendMode = null, Action<CinemachineVirtualCameraBase> onLiveCallback = null, bool useFade = false, bool stopCustomMove = true)
		{
			_currentCamera?.SetActive(isActive: false, target);
			if (racingCameraType == RacingCameraType.None || !_allCameras.ContainsKey(racingCameraType))
			{
				if ((bool)_cinemachineBrain)
				{
					_cinemachineBrain.enabled = false;
				}
				return;
			}
			racingCameraType = ((racingCameraType == RacingCameraType.DynoCar) ? RacingCameraType.GarageLookAround : racingCameraType);
			if (!firstStarting)
			{
				_cinemachineBrain.enabled = true;
			}
			ICameraHandler cameraHandler = _allCameras[racingCameraType];
			_nextBlend = blendMode ?? cameraHandler.VirtualCameraBlend;
			if (useFade)
			{
				_screenFader.FadeOut((_nextBlend.Value.BlendTime > 0f) ? (_nextBlend.Value.BlendTime + 0.2f) : 1f);
			}
			_currentCamera = cameraHandler;
			_currentCamera.SetActive(isActive: true, target, onLiveCallback);
			if (stopCustomMove)
			{
				CustomMoveCameraToTarget?.StopCustomMoveCam();
			}
		}

		public ICameraHandler GetCurrentCamera()
		{
			return _currentCamera;
		}

		public void ChangeTarget(Transform target)
		{
			_currentCamera?.ChangeTarget(target);
		}

		public void SetCameraRecenter(bool isActive)
		{
			_currentCamera?.SetRecenter(isActive);
		}

		public void InvokeCameraZoom()
		{
			_currentCamera?.CameraZoom();
		}

		public void InvokeCameraZoom(float delta)
		{
			_currentCamera?.CameraZoom(delta);
		}

		public void ForceSetCameraZoom(float value)
		{
			_currentCamera?.SetZoomValue(value);
		}

		private CinemachineBlendDefinition GetBlendOverride(ICinemachineCamera fromVcam, ICinemachineCamera toVcam, CinemachineBlendDefinition defaultBlend, MonoBehaviour owner)
		{
			if (firstCam == fromVcam.VirtualCameraGameObject)
			{
				return defaultBlend;
			}
			if (!_nextBlend.HasValue)
			{
				if ((bool)_cinemachineBrain)
				{
					return _cinemachineBrain.m_DefaultBlend;
				}
				return CutBlend;
			}
			CinemachineBlendDefinition value = _nextBlend.Value;
			_nextBlend = null;
			return value;
		}
	}
}
