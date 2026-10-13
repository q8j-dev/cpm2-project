using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;

public class InteractableRaceMapHandler : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	private RectTransform _mapRawImageRT;

	private Camera _targetCamera;

	public float panSpeed = 1f;

	public float zoomSpeed = 20f;

	public float minZoom = 30f;

	public float maxZoom = 65f;

	public float minUIElementShowValue = 70f;

	public float maxUIElementShowValue = 80f;

	private Vector3 cubeMin;

	private Vector3 cubeMax;

	private Vector2 lastTouchPos1;

	private Vector2 lastTouchPos2;

	public Transform cubeTransform;

	[SerializeField]
	private RawImage MapImage = default;

	[SerializeField]
	private float SmoothTime = 0.2f;

	[SerializeField]
	private float CameraSens = 1f;

	private Vector3 velocity = Vector3.zero;

	private Vector3 targetPosition;

	private bool isMoved;

	public RacingUIMap RacingUIMap;

	private Transform[] _trackedObjects = new Transform[0];

	private bool _isTouching;

	private bool _rtReady;

	private EventRegistry _eventRegistry;

	protected RegistryHandle<FloatVariable> _zoomHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.Zoom);

	private RegistryHandle<Vector3Variable> _mapMoveHandle = new RegistryHandle<Vector3Variable>(EventRegistryIDs.MapMove);

	private RegistryHandle<Vector2ValueEvent> _mapDragMoveHandle = new RegistryHandle<Vector2ValueEvent>(EventRegistryIDs.MapDragMoveValueEvent);

	private RegistryHandle<BoolEvent> _onMapMoveHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnMapMove);

	public float zoomSpeedAlter = 10f;

	private const float MinLookFactor = 0.15f;

	[SerializeField]
	private Vector2 panSpeedAlter = new Vector2(655f, 910f);

	private bool _dragActive;

	private bool _dragBlocked;

	private Vector3 _dragAnchorWorld;

	public void InitMapElements(Transform[] needTrackedRectTransforms, RoadMapElement[] roadMapElements)
	{
		for (int i = 0; i < RacingUIMap.CurrentRoadElements.Length; i++)
		{
			RacingUIMap.DisableRoadElements(i);
		}
		_trackedObjects = needTrackedRectTransforms;
		RacingUIMap.CurrentTrackedObjects = needTrackedRectTransforms;
		RacingUIMap.CurrentRoadElements = roadMapElements;
	}

	private void Start()
	{
		RacingUIMap.ObjectRoot.gameObject.SetActive(value: true);
		cubeTransform = RacingUIMap.Cube;
		Vector3 vector = cubeTransform.localScale / 2f;
		cubeMin = cubeTransform.position - vector;
		cubeMax = cubeTransform.position + vector;
		_mapRawImageRT = MapImage.GetComponent<RectTransform>();
		MapImage.texture = RacingUIMap.RenderObjectStage((int)MapImage.rectTransform.rect.width, (int)MapImage.rectTransform.rect.height);
		_targetCamera = RacingUIMap.GetCamera();
		_rtReady = true;
		MapAmbienceOn();
		RacingUIMap.TryGetEventRegistry(out _eventRegistry);
		_onMapMoveHandle.Get(_eventRegistry).Subscribe(OnMapMoveStartedChanged);
		_mapDragMoveHandle.Get(_eventRegistry).Subscribe(OnRaceMapDragMove);
	}

	private void OnRectTransformDimensionsChange()
	{
		if (_rtReady && !(MapImage == null) && !(RacingUIMap == null))
		{
			int num = (int)MapImage.rectTransform.rect.width;
			int num2 = (int)MapImage.rectTransform.rect.height;
			if (num >= 1 && num2 >= 1)
			{
				MapImage.texture = RacingUIMap.RenderObjectStage(num, num2);
			}
		}
	}

	private void OnDestroy()
	{
		RacingUIMap.Cleanup();
		MapAmbienceOff();
		_onMapMoveHandle.Get(_eventRegistry)?.Unsubscribe(OnMapMoveStartedChanged);
		_mapDragMoveHandle.Get(_eventRegistry)?.Unsubscribe(OnRaceMapDragMove);
	}

	private void MapAmbienceOn()
	{
		Audio.SetAmbienceVolume(1f);
		AudioWrapper.SetState("State_Track_Select", "In_Prefab");
		AudioWrapper.PostEvent("Play_Amb_Track_Select", base.gameObject);
	}

	private void MapAmbienceOff()
	{
		AudioWrapper.PostEvent("Stop_Amb_Track_Select", base.gameObject);
		AudioWrapper.SetState("State_Track_Select", "Out_Prefab");
		if (ScenesHelper.IsLapRacingSceneActive())
		{
			Audio.SetAmbienceVolume(0f);
		}
	}

	private void Update()
	{
		HandleMapCameraZoom();
		HandleMapCameraAlter();
	}

	private void HandleMapCameraAlter()
	{
		if (!_dragActive || _dragBlocked)
		{
			Vector3 value = _mapMoveHandle.Get(_eventRegistry).Value;
			Vector2 a = new Vector2(value.x, value.y);
			if (a.sqrMagnitude > 1E-07f)
			{
				RacingUIMap.StopSnaping();
				isMoved = true;
				float value2 = (_targetCamera.fieldOfView - minZoom) / (maxZoom - minZoom);
				float num = Mathf.Lerp(0.15f, 1f, Mathf.Clamp01(value2));
				a = Vector2.Scale(a, panSpeedAlter) * num;
				HandleCameraMovement(-a);
			}
			else
			{
				StopCameraMovement();
			}
		}
	}

	private void HandleMapCameraZoom()
	{
		float num = _zoomHandle.Get(_eventRegistry).Value * zoomSpeedAlter;
		_targetCamera.fieldOfView = Mathf.Clamp(_targetCamera.fieldOfView - num, minZoom, maxZoom);
	}

	private void OnMapMoveStartedChanged(bool started)
	{
		if (!started)
		{
			_dragActive = false;
			_dragBlocked = false;
		}
	}

	private void OnRaceMapDragMove(Vector2 screenPos)
	{
		if (!_rtReady || !_targetCamera)
		{
			return;
		}
		Vector3 world;
		if (!_dragActive)
		{
			_dragActive = true;
			_dragBlocked = !TryScreenToRaceMapWorld(screenPos, requireInside: true, out _dragAnchorWorld);
			if (!_dragBlocked)
			{
				RacingUIMap.StopSnaping();
				isMoved = false;
				velocity = Vector3.zero;
				targetPosition = _targetCamera.transform.position;
			}
		}
		else if (!_dragBlocked && TryScreenToRaceMapWorld(screenPos, requireInside: false, out world))
		{
			Vector3 position = _targetCamera.transform.position;
			position.x += _dragAnchorWorld.x - world.x;
			position.z += _dragAnchorWorld.z - world.z;
			_targetCamera.transform.position = position;
			ClampCameraPosition();
			targetPosition = _targetCamera.transform.position;
		}
	}

	private void TryClickRaceMapIcon(PointerEventData eventData)
	{
		if (_rtReady && eventData.button == PointerEventData.InputButton.Left)
		{
			EventSystem current = EventSystem.current;
			float num = (current ? ((float)current.pixelDragThreshold) : 10f);
			if (!((eventData.position - eventData.pressPosition).sqrMagnitude > num * num) && TryScreenToRaceMapViewport(eventData.position, requireInside: true, out var viewport))
			{
				RacingUIMap.CheckTouch(new Vector2(viewport.x * (float)Screen.width, viewport.y * (float)Screen.height));
			}
		}
	}

	private bool TryScreenToRaceMapViewport(Vector2 screenPos, bool requireInside, out Vector3 viewport)
	{
		viewport = default;
		if (!MapImage || !_mapRawImageRT)
		{
			return false;
		}
		Canvas canvas = MapImage.canvas;
		Camera cam = (((bool)canvas && canvas.renderMode != RenderMode.ScreenSpaceOverlay) ? canvas.worldCamera : null);
		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_mapRawImageRT, screenPos, cam, out var localPoint))
		{
			return false;
		}
		Rect rect = _mapRawImageRT.rect;
		if (requireInside && !rect.Contains(localPoint))
		{
			return false;
		}
		viewport = new Vector3((localPoint.x - rect.x) / rect.width, (localPoint.y - rect.y) / rect.height, 0f);
		return true;
	}

	private bool TryScreenToRaceMapWorld(Vector2 screenPos, bool requireInside, out Vector3 world)
	{
		world = default;
		if (!cubeTransform || !TryScreenToRaceMapViewport(screenPos, requireInside, out var viewport))
		{
			return false;
		}
		Ray ray = _targetCamera.ViewportPointToRay(viewport);
		if (!new Plane(Vector3.up, new Vector3(0f, cubeTransform.position.y, 0f)).Raycast(ray, out var enter))
		{
			return false;
		}
		world = ray.GetPoint(enter);
		return true;
	}

	private void Zooming()
	{
		Touch touch = Input.GetTouch(0);
		Touch touch2 = Input.GetTouch(1);
		if (touch2.phase == TouchPhase.Began || touch.phase == TouchPhase.Began)
		{
			lastTouchPos1 = touch.position;
			lastTouchPos2 = touch2.position;
			RacingUIMap.StopSnaping();
		}
		Vector2 position = touch.position;
		Vector2 position2 = touch2.position;
		float num = Vector2.Distance(position, position2);
		float num2 = (Vector2.Distance(lastTouchPos1, lastTouchPos2) - num) * zoomSpeed * Time.unscaledDeltaTime;
		_targetCamera.fieldOfView = Mathf.Clamp(_targetCamera.fieldOfView + num2, minZoom, maxZoom);
		lastTouchPos1 = position;
		lastTouchPos2 = position2;
	}

	private void CameraMoved()
	{
		Touch touch = Input.GetTouch(0);
		Vector2 delta = touch.deltaPosition * panSpeed;
		if (touch.phase == TouchPhase.Moved)
		{
			RacingUIMap.StopSnaping();
			isMoved = true;
		}
		HandleCameraMovement(delta);
	}

	private void HandleCameraMovement(Vector2 delta)
	{
		targetPosition = _targetCamera.transform.position + new Vector3(0f - delta.x, 0f, 0f - delta.y) * CameraSens;
		SmoothDampCamera();
		ClampCameraPosition();
	}

	private void StopCameraMovement()
	{
		if (isMoved)
		{
			velocity = Vector3.Lerp(velocity, Vector3.zero, Time.unscaledDeltaTime);
			SmoothDampCamera();
		}
		if (velocity.magnitude < 0.2f || (bool)RacingUIMap.target)
		{
			velocity = Vector3.zero;
			isMoved = false;
		}
		ClampCameraPosition();
	}

	private void SmoothDampCamera()
	{
		_targetCamera.transform.position = Vector3.SmoothDamp(_targetCamera.transform.position, targetPosition, ref velocity, SmoothTime, float.PositiveInfinity, Time.unscaledDeltaTime);
	}

	private void ClampCameraPosition()
	{
		Vector3 position = _targetCamera.transform.position;
		if (!(position.x >= cubeMin.x) || !(position.x <= cubeMax.x) || !(position.z >= cubeMin.z) || !(position.z <= cubeMax.z))
		{
			position.x = Mathf.Clamp(position.x, cubeMin.x, cubeMax.x);
			position.z = Mathf.Clamp(position.z, cubeMin.z, cubeMax.z);
			_targetCamera.transform.position = position;
		}
	}

	public void Reset()
	{
		_trackedObjects = null;
		RacingUIMap.StopSnaping();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		_isTouching = true;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		_isTouching = false;
		TryClickRaceMapIcon(eventData);
	}
}
