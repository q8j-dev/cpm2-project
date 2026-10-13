using System;
using System.Collections.Generic;
using CPMEngine;
using CarInteractionAnimationsScripts;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MVPFree.MapIcons;
using MVPFree.MapIcons.Enum;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.Views;
using Service;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Free_drive;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.GraphicsSettings;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam.Perf;

public class MapCamera : MonoBehaviour
{
	public const float MinZoomOrtSize = 400f;

	private const int MapLimitZ = 2500;

	private const int MapLimitXMin = -15000;

	private const int MapLimitXMax = 1200;

	private const int BigMapLimitZMin = -6123;

	private const int BigMapLimitZMax = 5930;

	private const int BigMapLimitXMin = -17118;

	private const int BigMapLimitXMax = 4330;

	private const float ClampLengthOffset = 20f;

	private const float RaycastDistance = 600f;

	private const float SphereCastRadiusMin = 5f;

	private const float SphereCastRadiusMax = 100f;

	private const float SphereCastStepsCount = 10f;

	private const float NavMeshSamplePositionDistance = 10f;

	private const int NavMeshSamplePositionAreaMask = 16;

	private static readonly Vector3 OffsetNavMeshCollider = new Vector3(0f, 50f, 0f);

	public readonly float OrtSizeForSmallMapIcons = 300f;

	private Transform tr;

	public Camera cam;

	public bool followPerson = true;

	public float delay = 0.1f;

	public RenderTexture rtSmall;

	public RenderTexture rtBig;

	public RenderTexture rtService;

	public bool serviceMapIsOpen;

	public bool beganTouching;

	private Vector3 direction;

	private Vector3? prev;

	public RawImage mapTexture;

	public RawImage mapTextureSmall;

	public RawImage serviceMap;

	public float maxOrtSize = 3100f;

	private GameObject _allMapMeshCollider;

	private UniTask _cameraRenderOp;

	private bool _isCameraRendering;

	private bool _initStarted;

	private WaitForSeconds _camMoveDelay;

	public Material carPhone_mat;

	[SerializeField]
	private GameObject gpsButton;

	[SerializeField]
	private MapCameraConfig mapCameraConfig;

	private bool _wasDestroyed;

	[SerializeField]
	private GameObject LapRaceMapMeshColliderParent;

	[SerializeField]
	private GameObject LapRaceRoadsNavMesh;

	private MapCamera _mapCamera;

	private OptionalDependency<RacingCameraController> _racingCameraControllerDep;

	private MapControl _mapControl;

	public bool StopMapcamera;

	private MapIconsModel _mapIconsModel;

	private Tween _moveCameraTween;

	private PersonAnimationsController _personAnimationsController;

	private EventRegistry _eventRegistry;

	private static readonly Vector2 MultiplierPan = new Vector2(20f, 40f);

	private const float AlterInputZoomSensitivity = 300f;

	private const float MapPanSmoothLerp = 12f;

	private Vector3 _smoothedMapMove;

	private readonly Color _debugColor = new Color(0.69f, 0.84f, 0f, 1f);

	private RegistryHandle<FloatVariable> _zoomHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.Zoom);

	private RegistryHandle<Vector3Variable> _mapMoveHandle = new RegistryHandle<Vector3Variable>(EventRegistryIDs.MapMove);

	private RegistryHandle<ScriptableEvent> _toggleFilterOnMapHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleFilterOnMap);

	private RegistryHandle<ScriptableEvent> _toggleShowHideMapFiltersHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleShowHideMapFilters);

	private RegistryHandle<ScriptableEvent> _markOnMapHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.MarkOnMap);

	private RegistryHandle<ScriptableEvent> _hideShowAllFiltersHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.HideShowAllFilters);

	private RegistryHandle<BoolEvent> _onMapMoveHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnMapMove);

	private RegistryHandle<Vector2ValueEvent> _mapDragMoveHandle = new RegistryHandle<Vector2ValueEvent>(EventRegistryIDs.MapDragMoveValueEvent);

	private bool _mouseDragActive;

	private bool _mouseDragBlocked;

	private Vector3 _dragAnchorWorld;

	private SteeringWheelValueEvent _steeringWheelValueEvent;

	private UiNavigationValueEvent _uiNavigationValueEvent;

	private MapIconsStarter _mapIconsStarter;

	private OptionalDependency<MapIconsStarter> _mapIconsStarterDep;

	private CarInputManager _carInputManager;

	private Powertrain _currentPowertrain;

	private readonly Dictionary<GameObject, IconDescriptionItemView> _filterButtonToItem = new Dictionary<GameObject, IconDescriptionItemView>();

	private DesktopControlsSettingsSO _controlsSettings;

	private InputMapSetEvent _inputMapSetEvent;

	public int intervalRender = 100;

	private MenuLevelParent _menuLevelParent;

	private GlobalCollectionsProvider _collectionsProvider;

	private FreeDriveProvider _freeDriveProvider;

	public bool ShowingBigMap { get; set; }

	private bool WheelConnected
	{
		get
		{
			if ((bool)_steeringWheelValueEvent)
			{
				return _steeringWheelValueEvent.StoredValue;
			}
			return false;
		}
	}

	public event Action<RenderTexture> OnSmallRenderTextureCreatedEvent;

	[Inject]
	public void Construct(MapControl mapControl, MenuLevelParent menuLevelParent, GlobalCollectionsProvider collectionsProvider, FreeDriveProvider freeDriveProvider, MapIconsModel mapIconsModel, EventRegistry eventRegistry, SteeringWheelValueEvent steeringWheelValueEvent, UiNavigationValueEvent uiNavigationValueEvent, CarInputManager carInputManager, InputMapSetEvent inputMapSetEvent, OptionalDependency<DesktopControlsSettingsSO> settingsDep, OptionalDependency<MapIconsStarter> mapIconsStarter, OptionalDependency<RacingCameraController> racingCameraController)
	{
		_freeDriveProvider = freeDriveProvider;
		_collectionsProvider = collectionsProvider;
		_menuLevelParent = menuLevelParent;
		_racingCameraControllerDep = racingCameraController;
		_mapControl = mapControl;
		_mapIconsModel = mapIconsModel;
		_eventRegistry = eventRegistry;
		_steeringWheelValueEvent = steeringWheelValueEvent;
		_uiNavigationValueEvent = uiNavigationValueEvent;
		_mapIconsStarterDep = mapIconsStarter;
		_carInputManager = carInputManager;
		_inputMapSetEvent = inputMapSetEvent;
		if (settingsDep.HasValue)
		{
			_controlsSettings = settingsDep.Value;
		}
	}

	private void Awake()
	{
		if (_mapIconsStarterDep.HasValue)
		{
			_mapIconsStarter = _mapIconsStarterDep.Value;
		}
		intervalRender = 0;
		delay = 0f;
		InitStart();
	}

	private void SubscribeToActions()
	{
		_markOnMapHandle.Get(_eventRegistry).Subscribe(OnMarkOnMapPressed);
		_toggleShowHideMapFiltersHandle.Get(_eventRegistry).Subscribe(OnToggleExpandFilters);
		_toggleFilterOnMapHandle.Get(_eventRegistry).Subscribe(OnVisibleButtonSelectedFilter);
		_hideShowAllFiltersHandle.Get(_eventRegistry).Subscribe(HideShowAllFilters);
		_uiNavigationValueEvent.Subscribe(DeselectIconView);
		V.OnOutOfCarChanged = (Action<bool>)Delegate.Remove(V.OnOutOfCarChanged, new Action<bool>(TrySetPowertrain));
		V.OnOutOfCarChanged = (Action<bool>)Delegate.Combine(V.OnOutOfCarChanged, new Action<bool>(TrySetPowertrain));
		SubscribeDragEvents();
	}

	private void UnsubscribeToActions()
	{
		_markOnMapHandle.Get(_eventRegistry)?.Unsubscribe(OnMarkOnMapPressed);
		_toggleShowHideMapFiltersHandle.Get(_eventRegistry)?.Unsubscribe(OnToggleExpandFilters);
		_toggleFilterOnMapHandle.Get(_eventRegistry)?.Unsubscribe(OnVisibleButtonSelectedFilter);
		_hideShowAllFiltersHandle.Get(_eventRegistry)?.Unsubscribe(HideShowAllFilters);
		V.OnOutOfCarChanged = (Action<bool>)Delegate.Remove(V.OnOutOfCarChanged, new Action<bool>(TrySetPowertrain));
		UnsubscribeDragEvents();
		UiInputSetup.DisableUiNavigation(_mapIconsStarter.IconsDescriptionView.ContentParent);
	}

	private void SubscribeDragEvents()
	{
		_onMapMoveHandle.Get(_eventRegistry)?.Unsubscribe(OnMapMoveStartedChanged);
		_onMapMoveHandle.Get(_eventRegistry)?.Subscribe(OnMapMoveStartedChanged);
		_mapDragMoveHandle.Get(_eventRegistry)?.Unsubscribe(OnMapDragMove);
		_mapDragMoveHandle.Get(_eventRegistry)?.Subscribe(OnMapDragMove);
		if ((bool)_inputMapSetEvent)
		{
			_inputMapSetEvent.Unsubscribe(OnInputGroupChanged);
			_inputMapSetEvent.Subscribe(OnInputGroupChanged);
		}
		_mouseDragActive = false;
		_mouseDragBlocked = false;
	}

	private void UnsubscribeDragEvents()
	{
		if ((bool)_inputMapSetEvent)
		{
			_inputMapSetEvent.Unsubscribe(OnInputGroupChanged);
		}
		_onMapMoveHandle.Get(_eventRegistry)?.Unsubscribe(OnMapMoveStartedChanged);
		_mapDragMoveHandle.Get(_eventRegistry)?.Unsubscribe(OnMapDragMove);
		_mouseDragActive = false;
		_mouseDragBlocked = false;
	}

	private void OnInputGroupChanged(InputGroupsEnum group)
	{
		if ((group == InputGroupsEnum.Map || group == InputGroupsEnum.RacingMap) && (ShowingBigMap || serviceMapIsOpen))
		{
			SubscribeDragEvents();
		}
	}

	private void OnMapMoveStartedChanged(bool started)
	{
		if (!started)
		{
			_mouseDragActive = false;
			_mouseDragBlocked = false;
		}
	}

	private void OnMapDragMove(Vector2 screenPos)
	{
		if ((!ShowingBigMap && !serviceMapIsOpen) || (_mapIconsModel.IsOpenedIconsDescription && WheelConnected))
		{
			return;
		}
		Vector3 world;
		if (!_mouseDragActive)
		{
			_mouseDragActive = true;
			_mouseDragBlocked = _mapIconsModel.IsScrollingIconsDescription || IsPointerOverHintsPanel() || !TryScreenToMapWorld(screenPos, requireInside: true, out _dragAnchorWorld);
			if (!_mouseDragBlocked)
			{
				_moveCameraTween?.Kill();
				_smoothedMapMove = Vector3.zero;
			}
		}
		else if (!_mouseDragBlocked && TryScreenToMapWorld(screenPos, requireInside: false, out world))
		{
			_moveCameraTween?.Kill();
			Vector3 localPosition = tr.localPosition;
			localPosition.x += _dragAnchorWorld.x - world.x;
			localPosition.z += _dragAnchorWorld.z - world.z;
			tr.localPosition = GetCorrectedPositionForBigMap(localPosition);
		}
	}

	private bool TryScreenToMapWorld(Vector2 screenPos, bool requireInside, out Vector3 world)
	{
		world = default;
		RawImage rawImage = (serviceMapIsOpen ? serviceMap : mapTexture);
		if (!rawImage)
		{
			return false;
		}
		RectTransform rectTransform = rawImage.rectTransform;
		Canvas canvas = rawImage.canvas;
		Camera camera = (((bool)canvas && canvas.renderMode != RenderMode.ScreenSpaceOverlay) ? canvas.worldCamera : null);
		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPos, camera, out var localPoint))
		{
			return false;
		}
		Rect rect = rectTransform.rect;
		if (requireInside && !rect.Contains(localPoint))
		{
			return false;
		}
		Vector2 vector = new Vector2((localPoint.x - rect.x) / rect.width, (localPoint.y - rect.y) / rect.height);
		float orthographicSize = cam.orthographicSize;
		Vector3 localPosition = tr.localPosition;
		world = new Vector3(localPosition.x + (vector.x - 0.5f) * 2f * orthographicSize * cam.aspect, 0f, localPosition.z + (vector.y - 0.5f) * 2f * orthographicSize);
		return true;
	}

	private void CreateButtonMappingToFilters()
	{
		foreach (var (_, iconDescriptionItemView2) in _mapIconsStarter.IconsDescriptionView.IconDescriptionItems)
		{
			_filterButtonToItem.TryAdd(iconDescriptionItemView2.gameObject, iconDescriptionItemView2);
		}
	}

	private void TrySetPowertrain(bool isOut)
	{
		if (!isOut)
		{
			if (!V.Car)
			{
				_currentPowertrain = null;
			}
			else if (!V.Car.TryGetComponent<Powertrain>(out _currentPowertrain) && (bool)_carInputManager && (bool)_carInputManager.GetPowertrain)
			{
				_currentPowertrain = _carInputManager.GetPowertrain;
			}
		}
	}

	private void DeselectIconView(UiNavigationEnum target)
	{
		if (target != UiNavigationEnum.Keys)
		{
			EventSystem.current.SetSelectedGameObject(null, null);
		}
	}

	private void HideShowAllFilters()
	{
		if (_mapIconsModel != null)
		{
			bool allIconsHided = _mapIconsModel.IconStatusModel.AllIconsHided;
			_mapIconsModel.IconStatusModel.SetAllVisible(allIconsHided);
		}
	}

	private void OnToggleExpandFilters()
	{
		if (_mapIconsStarter == null || _mapIconsStarter.IconsDescriptionView == null || _mapIconsModel.IsMovingIconsDescription)
		{
			return;
		}
		IconsDescriptionView iconsDescriptionView = _mapIconsStarter.IconsDescriptionView;
		bool num = !_mapIconsModel.IsOpenedIconsDescription;
		iconsDescriptionView.ExpandButton.onClick.Invoke();
		if (num)
		{
			UiInputSetup.EnableUiNavigation(_mapIconsStarter.IconsDescriptionView.ContentParent);
			if (_mapIconsStarter.IconsDescriptionView.IconDescriptionItems.Count > 0 && (bool)EventSystem.current)
			{
				EventSystem.current.SetSelectedGameObject(_mapIconsStarter.IconsDescriptionView.IconDescriptionItems[IconType.CarBlack].gameObject);
			}
			UISelectionCursor.ShowCursor();
		}
		else
		{
			if ((bool)EventSystem.current)
			{
				EventSystem.current.SetSelectedGameObject(null);
			}
			UISelectionCursor.HideCursor();
			UiInputSetup.DisableUiNavigation(_mapIconsStarter.IconsDescriptionView.ContentParent);
		}
	}

	private void OnMarkOnMapPressed()
	{
		EventSystem current = EventSystem.current;
		if (!current || !current.currentSelectedGameObject || !_filterButtonToItem.ContainsKey(current.currentSelectedGameObject))
		{
			Navigate();
		}
	}

	private void OnVisibleButtonSelectedFilter()
	{
		if (!(_mapIconsStarter == null) && !(_mapIconsStarter.IconsDescriptionView == null))
		{
			EventSystem current = EventSystem.current;
			if ((bool)current && (bool)current.currentSelectedGameObject && _filterButtonToItem.TryGetValue(current.currentSelectedGameObject, out var value) && (bool)value)
			{
				value.TriggerVisibleButton();
			}
		}
	}

	private void OnDestroy()
	{
		UnsubscribeDragEvents();
		_freeDriveProvider.MapCamera = null;
		_wasDestroyed = true;
		_collectionsProvider.UnregisterRenderTexture(rtBig);
		_collectionsProvider.UnregisterRenderTexture(rtService);
		_collectionsProvider.UnregisterRenderTexture(rtSmall);
		CommonExtensions.DeleteRenderTex(rtBig);
		rtBig = null;
		CommonExtensions.DeleteRenderTex(rtService);
		rtService = null;
		CommonExtensions.DeleteRenderTex(rtSmall);
		rtSmall = null;
		_moveCameraTween?.Kill();
	}

	public void InitStart()
	{
		if (!_initStarted)
		{
			_initStarted = true;
			_camMoveDelay = new WaitForSeconds(delay);
			_freeDriveProvider.MapCamera = this;
			tr = GetComponent<Transform>();
			cam = GetComponent<Camera>();
			cam.enabled = false;
			RunControlCameraRenderingAsync();
			Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(delay)).Subscribe((long _) =>
			{
				RenderCamera();
			}).AddTo(this);
			HideBigMap();
			if (ScenesHelper.IsLapRacingSceneActive())
			{
				_allMapMeshCollider = LapRaceMapMeshColliderParent;
				GPSTracker.allroadsNavMesh = LapRaceRoadsNavMesh;
			}
			else
			{
				_allMapMeshCollider = GameObject.Find("Global point/ALL_ROADS_FOR_EXPORT4/All_map_MeshCollider");
				GPSTracker.allroadsNavMesh = GameObject.Find("Global point/ALL_ROADS_FOR_EXPORT4/Navmesh");
			}
			if (GPSTracker.GPSButton == null)
			{
				GPSTracker.GPSButton = gpsButton;
			}
		}
	}

	private void RunControlCameraRenderingAsync()
	{
		_isCameraRendering = true;
		if (_cameraRenderOp.Status != UniTaskStatus.Pending)
		{
			_cameraRenderOp = ControlCameraRenderingAsync();
		}
	}

	private async UniTask ControlCameraRenderingAsync()
	{
		while (_isCameraRendering && !_wasDestroyed)
		{
			cam.enabled = true;
			await UniTask.WaitForFixedUpdate();
			if (_wasDestroyed || !_isCameraRendering)
			{
				break;
			}
			cam.enabled = false;
			await UniTask.Delay(SteamMinimapThrottle.RenderInterval(intervalRender));
		}
	}

	public void Navigate(Vector3 position)
	{
		if (Global.TaskManager.Task == null)
		{
			if (!GPSTracker.IsDrawing)
			{
				GPSTracker.CancelGPSDraw();
			}
			position.y = cam.transform.position.y;
			position = GetCorrectedPosition(position);
			GPSTracker.DrawGPS(position - V.GlobalOffset, GPSTracker.GPS.MAP, this);
		}
	}

	public void Navigate()
	{
		if (Global.TaskManager.Task == null)
		{
			if (!GPSTracker.IsDrawing)
			{
				Navigate(cam.transform.position);
			}
			else
			{
				GPSTracker.CancelGPSDraw();
			}
		}
	}

	public Vector3 GetCorrectedPosition(Vector3 pos)
	{
		if ((bool)_allMapMeshCollider)
		{
			_allMapMeshCollider.SetActive(value: true);
		}
		Ray ray = new Ray(pos, Vector3.down);
		bool flag = Physics.Raycast(ray, out var hitInfo, 600f, LayerMask.GetMask("Map"));
		if (!flag)
		{
			int mask = LayerMask.GetMask("Map");
			for (int i = 0; (float)i < 10f; i++)
			{
				float radius = Mathf.Lerp(5f, 100f, Mathf.InverseLerp(0f, 9f, i));
				if (Physics.SphereCast(ray, radius, out hitInfo, 600f, mask))
				{
					flag = true;
					break;
				}
			}
		}
		if (flag && NavMesh.SamplePosition(hitInfo.point + OffsetNavMeshCollider, out var hit, 10f, 16))
		{
			hitInfo.point = hit.position;
			pos = hitInfo.point;
		}
		if ((bool)_allMapMeshCollider)
		{
			_allMapMeshCollider.SetActive(value: false);
		}
		return pos;
	}

	private void TryGpsToPosition(Vector3 pos)
	{
		Ray ray = new Ray(pos, Vector3.down);
		bool flag = Physics.Raycast(ray, out var hitInfo, 600f, LayerMask.GetMask("Map"));
		if (!flag)
		{
			int mask = LayerMask.GetMask("Map");
			for (int i = 0; (float)i < 10f; i++)
			{
				float radius = Mathf.Lerp(5f, 100f, Mathf.InverseLerp(0f, 9f, i));
				if (Physics.SphereCast(ray, radius, out hitInfo, 600f, mask))
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			if (NavMesh.SamplePosition(hitInfo.point + OffsetNavMeshCollider, out var hit, 10f, 16))
			{
				hitInfo.point = hit.position;
				hitInfo.point -= OffsetNavMeshCollider;
			}
			GPSTracker.DrawGPS(hitInfo.point - V.GlobalOffset + OffsetNavMeshCollider, GPSTracker.GPS.MAP, this);
		}
	}

	public void DrawNextCheckPoint(LapCheckpointTrigger lapCheckpointTrigger)
	{
		GPSTracker.DrawGPS(lapCheckpointTrigger.transform.position - V.GlobalOffset + OffsetNavMeshCollider, GPSTracker.GPS.MAP, this);
	}

	private void RenderCamera()
	{
		if (!StopMapcamera && followPerson && (bool)V.DriverPerson)
		{
			if (!_personAnimationsController && V.DriverPerson.TryGetComponent<DriverActionController>(out var component))
			{
				_personAnimationsController = component.PersonAnimationsController;
			}
			bool flag = false;
			if ((bool)_personAnimationsController && _personAnimationsController.SitGetOutCarSystem != null && _personAnimationsController.SitGetOutCarSystem.SitGetOutData != null)
			{
				flag = _personAnimationsController.SitGetOutCarSystem.SitGetOutData.IsSittingIntoCarAnimate || _personAnimationsController.SitGetOutCarSystem.SitGetOutData.IsGettingOutOfTheCarAnimate;
			}
			Vector3 position = V.DriverPerson.transform.position;
			position.y = 300f;
			tr.position = position;
			tr.LookAt(V.DriverPerson.transform);
			Vector3 eulerAngles = base.transform.eulerAngles;
			if (_menuLevelParent.isDroneEnabled | flag)
			{
				eulerAngles.y = _freeDriveProvider.CarCameras.transform.eulerAngles.y;
			}
			else if (FlagsStorage.OutOfCar)
			{
				eulerAngles.y = V.DriverPerson.transform.eulerAngles.y;
			}
			else if ((bool)V.Car)
			{
				eulerAngles.y = V.Car.transform.eulerAngles.y;
			}
			base.transform.eulerAngles = eulerAngles;
		}
	}

	public void SetSmallMap()
	{
		cam.targetTexture = rtSmall;
	}

	public void ShowBigMap(bool showMapTexture = true)
	{
		_isCameraRendering = false;
		cam.enabled = true;
		mapTexture.gameObject.SetActive(showMapTexture);
		serviceMapIsOpen = false;
		followPerson = false;
		cam.orthographicSize = 500f;
		tr.localPosition = GetCorrectedPositionForBigMap(tr.localPosition);
		ZoomIcon();
		base.transform.eulerAngles = new Vector3(90f, 0f, 0f);
		FlagsStorage.IsEnabledJoy = false;
		ShowingBigMap = true;
		if (!rtBig)
		{
			int num = 1000;
			int num2 = 500;
			num = RamDevice.GetValueOnRamSettings(Screen.width / 2, Screen.width / 2, Screen.width);
			num2 = RamDevice.GetValueOnRamSettings(Screen.height / 2, Screen.height / 2, Screen.height);
			rtBig = new RenderTexture(num, num2, 0, RenderTextureFormat.ARGB32);
			rtBig.name = "BigMap";
			_collectionsProvider.RegisterRenderTexture(rtBig);
		}
		cam.targetTexture = rtBig;
		mapTexture.texture = rtBig;
		Audio.SetPauseSoundFilter(on: true);
		SubscribeToActions();
		CreateButtonMappingToFilters();
	}

	public void ShowServiceMap()
	{
		_isCameraRendering = false;
		cam.enabled = true;
		FlagsStorage.IsEnabledJoy = false;
		serviceMapIsOpen = true;
		followPerson = false;
		cam.orthographicSize = 500f;
		if (_mapIconsModel != null)
		{
			_mapIconsModel.OpenServiceMap(enabled: true);
		}
		Vector3 localPosition = tr.localPosition;
		float num = maxOrtSize - cam.orthographicSize;
		localPosition.x = Mathf.Clamp(localPosition.x, -15000f - num, 1200f + num);
		localPosition.z = Mathf.Clamp(localPosition.z, -2500f - num, 2500f + num);
		tr.localPosition = localPosition;
		ZoomIcon();
		base.transform.eulerAngles = new Vector3(90f, 0f, 0f);
		if (rtService == null)
		{
			float num2 = 1000f;
			float num3 = num2 * serviceMap.rectTransform.rect.width / serviceMap.rectTransform.rect.height;
			rtService = new RenderTexture((int)num3, (int)num2, 0, RenderTextureFormat.ARGB32);
			rtService.name = "ServiceMap";
			_collectionsProvider.RegisterRenderTexture(rtService);
		}
		cam.targetTexture = rtService;
		serviceMap.texture = rtService;
		SubscribeDragEvents();
	}

	public void HideBigMap()
	{
		RunControlCameraRenderingAsync();
		serviceMapIsOpen = false;
		followPerson = true;
		cam.orthographicSize = 150f;
		FlagsStorage.IsEnabledJoy = true;
		zoomIcon(OrtSizeForSmallMapIcons);
		ShowingBigMap = false;
		if (rtSmall == null)
		{
			int num = 1024;
			rtSmall = new RenderTexture(num, num, 0, RenderTextureFormat.ARGB32);
			rtSmall.name = "SmallMap";
			_collectionsProvider.RegisterRenderTexture(rtSmall);
			OnSmallRenderTextureCreatedEvent?.Invoke(rtSmall);
		}
		if (V.Car != null && V.Car.TryGetComponent<VehicleData>(out var component) && component.Phone != null)
		{
			Debug.Log("Got Phone");
			carPhone_mat = component.Phone.transform.Find("Display").GetComponent<Renderer>().sharedMaterial;
		}
		if (carPhone_mat != null)
		{
			carPhone_mat.mainTexture = rtSmall;
		}
		cam.targetTexture = rtSmall;
		if ((bool)mapTextureSmall)
		{
			mapTextureSmall.texture = rtSmall;
		}
		if ((bool)mapTexture)
		{
			mapTexture.gameObject.SetActive(value: false);
		}
		if (_mapIconsModel != null)
		{
			_mapIconsModel.OpenServiceMap(enabled: false);
		}
		UnsubscribeToActions();
	}

	public void EnableDisableSmallMap(bool enable)
	{
		mapTextureSmall.transform.parent.parent.gameObject.SetActive(enable);
	}

	public void EnableDisableInteriorMap(bool enable)
	{
	}

	public void MoveCameraByVelocity(Vector3 pos, float velocity, float maxMoveDuration)
	{
		pos.y = base.transform.position.y;
		float num = Vector3.Distance(pos, base.transform.position);
		velocity *= cam.orthographicSize / 400f;
		MoveCamera(pos, Mathf.Min(maxMoveDuration, num / velocity));
	}

	public void MoveCamera(Vector3 pos, float duration)
	{
		_moveCameraTween?.Kill();
		pos.y = base.transform.position.y;
		_moveCameraTween = base.transform.DOMove(pos, duration).SetEase(Ease.OutSine).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
	}

	public void ZoomIcon()
	{
		float orthographicSize = cam.orthographicSize;
		zoomIcon(orthographicSize);
	}

	public void zoomIcon(float ortSize)
	{
		foreach (Transform allMapIcon in _collectionsProvider.GetAllMapIcons())
		{
			if ((bool)allMapIcon)
			{
				ZoomIcon(allMapIcon, ortSize);
			}
		}
		if ((bool)_freeDriveProvider.MapControl && (bool)_freeDriveProvider.MapControl.treasure)
		{
			ZoomIcon(_freeDriveProvider.MapControl.treasure, ortSize);
		}
	}

	private void ZoomIcon(Transform icon, float ortSize)
	{
		float num = Mathf.Lerp(60f, 1000f, ortSize / maxOrtSize);
		icon.localScale = ((Math.Abs(icon.localScale.z - 2f) < 0.001f) ? new Vector3(6f * num, 4f * num, 4f) : new Vector3(0.3f * num, 0.3f * num, 1f));
		if (icon.TryGetComponent<IconMapView>(out var component))
		{
			component.ApplyMapRootScale(ShowingBigMap || serviceMapIsOpen);
		}
	}

	private void ChangeMapSizeWithSpeed()
	{
		if (serviceMapIsOpen || ShowingBigMap || FlagsStorage.OutOfCar || !V.Car)
		{
			_freeDriveProvider.MapControl.ClampLenthMax = 140f;
			return;
		}
		if (!_currentPowertrain)
		{
			TrySetPowertrain(FlagsStorage.OutOfCar);
		}
		if ((bool)_currentPowertrain)
		{
			float speedMps = _currentPowertrain.SpeedMps;
			if ((bool)mapCameraConfig)
			{
				cam.orthographicSize = mapCameraConfig.OrthographicSizeByMps.GetValue(speedMps);
			}
			else
			{
				speedMps = ((!(speedMps < 50f)) ? Mathf.Min(speedMps - 50f, 50f) : 0f);
				cam.orthographicSize = 150f + speedMps;
				cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 150f, 200f);
			}
			_freeDriveProvider.MapControl.ClampLenthMax = cam.orthographicSize - 20f;
		}
	}

	private void Update()
	{
		if (!StopMapcamera)
		{
			ChangeMapSizeWithSpeed();
			if (SteamMinimapThrottle.ShouldZoomIcons(cam.orthographicSize))
			{
				ZoomIcon();
			}
			if (ShowingBigMap || serviceMapIsOpen)
			{
				ApplyAlterInputMapMovement();
			}
		}
	}

	private bool IsPointerOverHintsPanel()
	{
		if (_mapIconsStarter == null || _mapIconsStarter.IconsDescriptionView == null || Mouse.current == null)
		{
			return false;
		}
		RectTransform scrollViewRect = _mapIconsStarter.IconsDescriptionView.ScrollViewRect;
		if (scrollViewRect == null || !scrollViewRect.gameObject.activeInHierarchy)
		{
			return false;
		}
		Canvas componentInParent = scrollViewRect.GetComponentInParent<Canvas>();
		Camera camera = (componentInParent ? componentInParent.worldCamera : null);
		return RectTransformUtility.RectangleContainsScreenPoint(scrollViewRect, Mouse.current.position.ReadValue(), camera);
	}

	private void ApplyAlterInputMapMovement()
	{
		if ((_mapIconsModel.IsOpenedIconsDescription && WheelConnected) || _mapIconsModel.IsScrollingIconsDescription || IsPointerOverHintsPanel())
		{
			return;
		}
		float value = _zoomHandle.Get(_eventRegistry).Value;
		if (Mathf.Abs(value) > 0.0001f)
		{
			cam.orthographicSize -= value * 300f;
			cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 400f, maxOrtSize);
			ZoomIcon();
		}
		if (_mouseDragActive && !_mouseDragBlocked)
		{
			_smoothedMapMove = Vector3.zero;
			return;
		}
		Vector3 value2 = _mapMoveHandle.Get(_eventRegistry).Value;
		value2 = Vector2.Scale(MultiplierPan, value2);
		_smoothedMapMove = Vector3.Lerp(_smoothedMapMove, value2, 12f);
		if (_smoothedMapMove.sqrMagnitude > 0.0001f)
		{
			_moveCameraTween?.Kill();
			float num = cam.orthographicSize / maxOrtSize * 8f;
			Vector3 localPosition = tr.localPosition;
			localPosition.x += _smoothedMapMove.x * num;
			localPosition.z += _smoothedMapMove.y * num;
			localPosition = GetCorrectedPositionForBigMap(localPosition);
			tr.localPosition = localPosition;
		}
	}

	private Vector3 GetCorrectedPositionForBigMap(Vector3 pos)
	{
		pos.x = Mathf.Clamp(pos.x, -17118f, 4330f);
		pos.z = Mathf.Clamp(pos.z, -6123f, 5930f);
		return pos;
	}
}
