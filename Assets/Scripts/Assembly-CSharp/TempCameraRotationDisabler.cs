using System;
using Racing.CameraManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;

public class TempCameraRotationDisabler : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	public static bool IsTouching;

	public float TouchSensitivity_x = 10f;

	public float TouchSensitivity_y = 10f;

	public static Action OnPointerDownAction;

	public static Action OnPointerUpAction;

	private bool _isTouching;

	private CameraManager _cameraManager;

	private FreeDriveProvider _freeDriveProvider;

	private EventRegistry _eventRegistry;

	private RegistryHandle<BoolEvent> _onLookHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnLook);

	[SerializeField]
	private float ScrollZoomStep = 70f;

	[Inject]
	public void Construct(CameraManager cameraManager, FreeDriveProvider freeDriveProvider, EventRegistry eventRegistry)
	{
		_freeDriveProvider = freeDriveProvider;
		_cameraManager = cameraManager;
		_eventRegistry = eventRegistry;
	}

	private void Start()
	{
		_onLookHandle.Get(_eventRegistry).Subscribe(ToggleLook);
	}

	private void OnDestroy()
	{
		if ((bool)_freeDriveProvider.CarCameras)
		{
			_freeDriveProvider.CarCameras.IsRotating = true;
		}
		_onLookHandle.Get(_eventRegistry)?.Unsubscribe(ToggleLook);
	}

	private void ToggleLook(bool isStarted)
	{
		if (isStarted)
		{
			OnPointerDownAction?.Invoke();
		}
		else
		{
			OnPointerUpAction?.Invoke();
		}
		if ((bool)_freeDriveProvider.CarCameras)
		{
			_freeDriveProvider.CarCameras.IsRotating = isStarted;
		}
		IsTouching = (_isTouching = isStarted);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
	}

	public void OnPointerUp(PointerEventData eventData)
	{
	}
}
