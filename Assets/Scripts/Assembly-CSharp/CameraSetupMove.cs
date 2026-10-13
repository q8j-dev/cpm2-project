using UnityEngine;
using UnityEngine.UI.Extensions;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;

public class CameraSetupMove : MonoBehaviour
{
	private const float HorizontalMin = -10f;

	private const float HorizontalMax = 10f;

	private const float VerticalMax = 2f;

	private const float VerticalMin = -0.3f;

	private MainPerson _mainPerson;

	private RacingCameraController _racingCameraController;

	[SerializeField]
	private RacingCarControllerUI racingCarControllerUI;

	[SerializeField]
	private GameObject trackedObject;

	private CoreGameManager _coreGameManager;

	private MenuLevelParent _menuLevelParent;

	private CanvasGroup _canvasGroup;

	private bool _isChangingHeight;

	private bool _isChangingPosition;

	private EventRegistry _eventRegistry;

	private InputMapSetEvent _inputMapSetEvent;

	private RegistryHandle<ScriptableEvent> _cancelHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.Cancel);

	private RegistryHandle<BoolEvent> _onCameraHeightHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnCameraHeight);

	private RegistryHandle<BoolEvent> _onCameraMoveHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnCameraMove);

	private RegistryHandle<Vector2Variable> _cameraSetupMoveHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.CameraSetupMove);

	private RegistryHandle<FloatVariable> _cameraSetupHeightHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.CameraSetupHeight);

	[Inject]
	public void Construct(OptionalDependency<RacingCameraController> racingCameraControllerDep, OptionalDependency<MenuLevelParent> menuLevelParentDep, EventRegistry eventRegistry, InputMapSetEvent inputMapSetEvent, CoreGameManager coreGameManager)
	{
		_racingCameraController = racingCameraControllerDep?.Value;
		_menuLevelParent = menuLevelParentDep?.Value;
		_eventRegistry = eventRegistry;
		_inputMapSetEvent = inputMapSetEvent;
		_coreGameManager = coreGameManager;
	}

	private void Start()
	{
		if (!trackedObject)
		{
			trackedObject = ResolveInsideCamElements();
		}
		if (!trackedObject || _inputMapSetEvent == null)
		{
			base.enabled = false;
			return;
		}
		_canvasGroup = trackedObject.GetOrAddComponent<CanvasGroup>();
		ToggleCameraSetup(isOn: false);
		_inputMapSetEvent.Unsubscribe(CheckForCameraSetupActivation);
		_inputMapSetEvent.Subscribe(CheckForCameraSetupActivation);
	}

	private GameObject ResolveInsideCamElements()
	{
		if ((bool)racingCarControllerUI && (bool)racingCarControllerUI.InsideCamElements)
		{
			return racingCarControllerUI.InsideCamElements;
		}
		if ((bool)_menuLevelParent && (bool)_menuLevelParent.InsideCamElements)
		{
			return _menuLevelParent.InsideCamElements;
		}
		return null;
	}

	private void OnDisable()
	{
		if ((bool)_inputMapSetEvent)
		{
			_inputMapSetEvent.Unsubscribe(CheckForCameraSetupActivation);
		}
	}

	private void CheckForCameraSetupActivation(InputGroupsEnum group)
	{
		if (group == InputGroupsEnum.CameraSetup && trackedObject.activeSelf)
		{
			ToggleCameraSetup(isOn: true);
		}
	}

	private void StopCameraSetup()
	{
		_inputMapSetEvent.SetPreviousRootGroup();
		ToggleCameraSetup(isOn: false);
		FlagsStorage.MovingInsideCamPos = false;
	}

	private void ToggleCameraSetup(bool isOn)
	{
		if ((bool)_canvasGroup)
		{
			_canvasGroup.alpha = (isOn ? 1 : 0);
			_canvasGroup.blocksRaycasts = isOn;
			_canvasGroup.interactable = isOn;
		}
		if (isOn)
		{
			_onCameraHeightHandle.Get(_eventRegistry).Subscribe(StartStopHeight);
			_onCameraMoveHandle.Get(_eventRegistry).Subscribe(StartStopMove);
			_cancelHandle.Get(_eventRegistry).Subscribe(StopCameraSetup);
		}
		else
		{
			_onCameraHeightHandle.Get(_eventRegistry)?.Unsubscribe(StartStopHeight);
			_onCameraMoveHandle.Get(_eventRegistry)?.Unsubscribe(StartStopMove);
			_cancelHandle.Get(_eventRegistry)?.Unsubscribe(StopCameraSetup);
		}
	}

	private void Update()
	{
		if ((bool)trackedObject && (bool)V.Car)
		{
			FpvCameraMoveMethod();
			if (trackedObject.activeInHierarchy)
			{
				TrySwitchVisibleHeadPerson(_coreGameManager.transform);
			}
		}
	}

	private void StartStopHeight(bool isStarted)
	{
		_isChangingHeight = isStarted;
		FlagsStorage.MovingInsideCamPos = true;
	}

	private void StartStopMove(bool isStarted)
	{
		_isChangingPosition = isStarted;
		FlagsStorage.MovingInsideCamPos = true;
	}

	private void FpvCameraMoveMethod()
	{
		if (V.Car.TryGetComponent<DriverController>(out var component) && (bool)component.camInsideView)
		{
			Transform transform = component.camInsideView.transform;
			Vector3 localPosition = transform.localPosition;
			if (_isChangingHeight)
			{
				float value = _cameraSetupHeightHandle.Get(_eventRegistry).Value;
				localPosition += new Vector3(0f, value, 0f);
			}
			if (_isChangingPosition)
			{
				Vector2 value2 = _cameraSetupMoveHandle.Get(_eventRegistry).Value;
				localPosition += V.Car.transform.InverseTransformDirection(transform.right) * value2.x + V.Car.transform.InverseTransformDirection(transform.forward) * value2.y;
			}
			_racingCameraController?.MarkCameraSettingsChanged();
			localPosition.y = Mathf.Clamp(localPosition.y, -0.3f, 2f);
			localPosition.x = Mathf.Clamp(localPosition.x, -10f, 10f);
			localPosition.z = Mathf.Clamp(localPosition.z, -10f, 10f);
			transform.localPosition = localPosition;
			component.camPosInitialInsideView = localPosition;
		}
	}

	private void TrySwitchVisibleHeadPerson(Transform cameraTransform)
	{
		if ((bool)_mainPerson || ((bool)V.DriverPerson && V.DriverPerson.TryGetComponent<MainPerson>(out _mainPerson)))
		{
			_mainPerson.TrySwitchVisibleHeadPerson(cameraTransform);
		}
	}
}
