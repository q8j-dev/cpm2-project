using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.Scopes;

public class RacingUIMap : MonoBehaviour
{
	public Transform ObjectRoot;

	private RenderTexture RT;

	public Camera RTCam;

	public StageLightingPreset LightingPreset;

	public Transform Cube;

	public Vector3 CameraDefaultPosition;

	private float WindowWidth = 1f;

	private float WindowHeigth = 1f;

	private IPrefabFactory _prefabFactory;

	private float? _targetFOV;

	[SerializeField]
	private float[] TracksFOV = default;

	public Transform[] RacingTrackedObjects;

	public Transform[] CircuitTrackedObjects;

	public GameObject CircuitPoints;

	public GameObject RaceModePoints;

	public GameObject RacePlace;

	public Transform target;

	public float smoothTime = 1f;

	private Vector3 velocity = Vector3.zero;

	public RoadMapElement[] RacingRoadElements;

	public RoadMapElement[] CircuitRoadElements;

	private bool snapping;

	public float TargetValue;

	public float TargetOffsetX;

	public float TargetOffsetZ;

	private Camera _mainCamera;

	private EventRegistry _eventRegistry;

	private DesktopControlsSettingsSO _controlsSettings;

	public Transform[] CurrentTrackedObjects { get; set; }

	public RoadMapElement[] CurrentRoadElements { get; set; } = new RoadMapElement[0];

	public CircuitRaceSelectMapWindow CircuitRaceSelectMapWindow { get; set; }

	public RacingModesWindow RacingModesWindow { get; set; }

	public bool TryGetEventRegistry(out EventRegistry eventRegistry)
	{
		if (!_eventRegistry)
		{
			eventRegistry = null;
			return false;
		}
		eventRegistry = _eventRegistry;
		return true;
	}

	public bool TryGetControlsSettings(out DesktopControlsSettingsSO controlsSettings)
	{
		controlsSettings = _controlsSettings;
		return _controlsSettings;
	}

	public Camera GetCamera()
	{
		return RTCam;
	}

	[Inject]
	public void Construct(IPrefabFactory prefabFactory, Camera camera, EventRegistry eventRegistry, OptionalDependency<DesktopControlsSettingsSO> settingsDep)
	{
		_prefabFactory = prefabFactory;
		_mainCamera = camera;
		_eventRegistry = eventRegistry;
		if (settingsDep.HasValue)
		{
			_controlsSettings = settingsDep.Value;
		}
	}

	public void Reset()
	{
		StopSnaping();
	}

	public void DisableRoadElements(int index)
	{
		CurrentRoadElements[index].SetObjectState(state: false);
	}

	public void SetVisibleRoad(int index)
	{
		if (index <= CurrentRoadElements.Length - 1)
		{
			for (int i = 0; i < CurrentRoadElements.Length; i++)
			{
				DisableRoadElements(i);
			}
			CurrentRoadElements[index].SetObjectState(state: true);
		}
	}

	private void CreateRT(int Width, int Height)
	{
		RT = new RenderTexture(Width, Height, 16, RenderTextureFormat.ARGBHalf);
		RT.useDynamicScale = false;
	}

	public RenderTexture RenderObjectStage(int Width, int Height)
	{
		if ((bool)RT)
		{
			if (RT.width != Width || RT.height != Height)
			{
				RTCam.targetTexture = null;
				CommonExtensions.DeleteRenderTex(RT);
				CreateRT(Width, Height);
			}
		}
		else
		{
			CreateRT(Width, Height);
		}
		WindowWidth = Width;
		WindowHeigth = Height;
		RTCam.targetTexture = RT;
		RTCam.enabled = true;
		RTCam.Render();
		return RT;
	}

	public void Cleanup()
	{
		if ((bool)RTCam)
		{
			RTCam.targetTexture = null;
			if ((bool)RT)
			{
				CommonExtensions.DeleteRenderTex(RT);
			}
			RTCam.enabled = false;
		}
	}

	private void Update()
	{
		if (snapping && target != null)
		{
			Vector3 pos = new Vector3(target.position.x + TargetOffsetX, RTCam.transform.position.y, target.position.z + TargetOffsetZ);
			pos = ClampToCubeXZ(pos);
			RTCam.transform.position = Vector3.SmoothDamp(RTCam.transform.position, pos, ref velocity, smoothTime, float.PositiveInfinity, Time.unscaledDeltaTime);
			if (Vector3.Distance(RTCam.transform.position, pos) < TargetValue)
			{
				snapping = false;
				target = null;
				velocity = Vector3.zero;
			}
		}
		if (_targetFOV.HasValue)
		{
			RTCam.fieldOfView = Mathf.Lerp(RTCam.fieldOfView, _targetFOV.Value, Time.unscaledDeltaTime * smoothTime);
			if (Mathf.Abs(RTCam.fieldOfView - _targetFOV.Value) < 0.1f)
			{
				RTCam.fieldOfView = _targetFOV.Value;
				_targetFOV = null;
			}
		}
	}

	public void CheckTouch(Vector2 position)
	{
		if ((!CircuitRaceSelectMapWindow && !RacingModesWindow) || !RT)
		{
			return;
		}
		position.x *= (float)RT.width / (float)Screen.width;
		position.y *= (float)RT.height / (float)Screen.height;
		if (!Physics.Raycast(RTCam.ScreenPointToRay(position), out var hitInfo) || !hitInfo.collider.TryGetComponent<MapIconBehaviour>(out var component))
		{
			return;
		}
		RacingTypeIcon racingTypeIcon = component.RacingTypeIcon;
		if ((bool)CircuitRaceSelectMapWindow && racingTypeIcon != RacingTypeIcon.Drag && racingTypeIcon != RacingTypeIcon.Races)
		{
			CircuitRaceSelectMapWindow.OnMapSelected((int)racingTypeIcon);
		}
		else if ((bool)RacingModesWindow)
		{
			switch (racingTypeIcon)
			{
			case RacingTypeIcon.Drag:
				RacingModesWindow.SelectRacingModeToggle(RacingMode.DragRacing);
				break;
			case RacingTypeIcon.Races:
				RacingModesWindow.SelectRacingModeToggle(RacingMode.RacingTracks);
				break;
			case RacingTypeIcon.Rally:
				RacingModesWindow.SelectRacingModeToggle(RacingMode.Rally);
				break;
			}
		}
	}

	public void SnapToTarget(int index, bool withFOV = false)
	{
		target = CurrentTrackedObjects[index];
		snapping = true;
		if (withFOV && TracksFOV.Length != 0)
		{
			_targetFOV = TracksFOV[index];
		}
	}

	public void StopSnaping()
	{
		target = null;
		_targetFOV = null;
		snapping = false;
	}

	private Vector3 ClampToCubeXZ(Vector3 pos)
	{
		if (!Cube)
		{
			return pos;
		}
		Vector3 vector = Cube.localScale / 2f;
		Vector3 vector2 = Cube.position - vector;
		Vector3 vector3 = Cube.position + vector;
		pos.x = Mathf.Clamp(pos.x, vector2.x, vector3.x);
		pos.z = Mathf.Clamp(pos.z, vector2.z, vector3.z);
		return pos;
	}

	public void EnableMainCamera(bool state)
	{
		_mainCamera.enabled = state;
	}
}
