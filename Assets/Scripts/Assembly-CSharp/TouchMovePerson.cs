using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Person.Locomotion;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.WorldInteraction;

public class TouchMovePerson : MonoBehaviour
{
	private Rigidbody _rigidbody;

	private PersonLocomotion _locomotion;

	public bool PlayerOutOfCar;

	public float CamPosLerpSpeed;

	public float CamRotLerpSpeed;

	private TouchField _touchField;

	private NewMoveController _playerController;

	private CoreGameManager _coreGameManager;

	private WorldInteractionController _worldInteraction;

	private const float RotationSpeed = 6f;

	public Vector3 CameraPivot;

	public float angleMax = 45f;

	public float angleMaxUp = -10f;

	public float CameraDistance;

	public float CameraDistanceGlobal;

	protected float InputRotationX;

	protected float InputRotationY;

	public Vector3 CharacterPivot;

	protected Vector3 LookDirection;

	private bool _raycastHitted;

	private float _joystickX;

	private float _joystickY;

	private float _timeElapsed;

	public float lerpDuration = 10f;

	public float speedKof = 2f;

	private int _internalCounter;

	private bool _isOperational;

	private string _statusMessage;

	private ObscuredFloat _distanceTravel;

	private double _hiddenValue;

	private long _uniqueIdentifier;

	private float _vertical;

	private float _horizontal;

	public bool disableCam;

	private ObscuredFloat _mileagePassenger;

	private ulong _calculatedChecksum;

	private char _identifierChar;

	private MenuLevelParent _menuLevelParent;

	private InputItemsProvider _controlItemsProvider;

	private FreeDriveProvider _freeDriveProvider;

	private DriverActionController driverActionController;

	private bool _canMove;

	private CarInputManager _carInputManager;

	private EventRegistry _eventRegistry;

	private DesktopControlsSettingsSO _controlsSettings;

	private RegistryHandle<FloatVariable> _personRunHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.PersonRun);

	private RegistryHandle<ScriptableEvent> _personCrouchHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.PersonCrouch);

	private RegistryHandle<ScriptableEvent> _personJumpHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.PersonJump);

	private RegistryHandle<ScriptableEvent> _personWalkRunToggleHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.PersonWalkRunToggle);

	private RegistryHandle<Vector2Variable> _lookFreeHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.LookFree);

	private RegistryHandle<Vector2Variable> _movePersonHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.MovePerson);

	private RegistryHandle<FloatVariable> _personRotateRightLeftHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.PersonRotateRightLeft);

	private const float RunHoldThreshold = 0.5f;

	private bool _crouchLatched;

	private bool _jumpLatched;

	private bool _walkToggleLatched;

	private Vector3 target;

	private Vector3 prevPos;

	private bool sendRadnomPos = true;

	private Vector3 offset;

	private int frameSkip;

	public Vector3 posOnRagdollStart;

	public bool UpdateCamera { get; set; } = true;

	public bool IsKeyboardMovement { get; private set; }

	public Vector2 SoapMove => _movePersonHandle.Get(_eventRegistry).Value;

	public bool RunHeld => _personRunHandle.Get(_eventRegistry).Value > 0.5f;

	public float CameraYaw => InputRotationX;

	private Vector2 ScalePersonLook(Vector2 look)
	{
		if (!_controlsSettings)
		{
			return look;
		}
		Vector2 lookScale = _controlsSettings.GetLookScale(CameraSettingsContext.Person);
		look.x *= lookScale.x;
		look.y *= lookScale.y;
		return look;
	}

	[Inject]
	public void Construct(MenuLevelParent menuLevelParent, InputItemsProvider controlItemsProvider, FreeDriveProvider freeDriveProvider, CarInputManager carInputManager, EventRegistry eventRegistry, OptionalDependency<DesktopControlsSettingsSO> settingsDep)
	{
		_freeDriveProvider = freeDriveProvider;
		_controlItemsProvider = controlItemsProvider;
		_menuLevelParent = menuLevelParent;
		_carInputManager = carInputManager;
		_eventRegistry = eventRegistry;
		if (settingsDep.HasValue)
		{
			_controlsSettings = settingsDep.Value;
		}
	}

	public bool ConsumeCrouch()
	{
		if (!_crouchLatched)
		{
			return false;
		}
		_crouchLatched = false;
		return true;
	}

	public bool ConsumeJump()
	{
		if (!_jumpLatched)
		{
			return false;
		}
		_jumpLatched = false;
		return true;
	}

	public bool ConsumeWalkToggle()
	{
		if (!_walkToggleLatched)
		{
			return false;
		}
		_walkToggleLatched = false;
		return true;
	}

	private void OnEnable()
	{
		_personCrouchHandle.Get(_eventRegistry)?.Subscribe(OnPersonCrouch);
		_personJumpHandle.Get(_eventRegistry)?.Subscribe(OnPersonJump);
		_personWalkRunToggleHandle.Get(_eventRegistry)?.Subscribe(OnPersonWalkRunToggle);
	}

	private void OnDisable()
	{
		_personCrouchHandle.Get(_eventRegistry)?.Unsubscribe(OnPersonCrouch);
		_personJumpHandle.Get(_eventRegistry)?.Unsubscribe(OnPersonJump);
		_personWalkRunToggleHandle.Get(_eventRegistry)?.Unsubscribe(OnPersonWalkRunToggle);
		_crouchLatched = false;
		_jumpLatched = false;
		_walkToggleLatched = false;
	}

	private void OnPersonCrouch()
	{
		_crouchLatched = true;
	}

	private void OnPersonJump()
	{
		_jumpLatched = true;
	}

	private void OnPersonWalkRunToggle()
	{
		_walkToggleLatched = true;
	}

	private void Start()
	{
		TryGetComponent<Rigidbody>(out _rigidbody);
		TryGetComponent<PersonLocomotion>(out _locomotion);
		CameraDistanceGlobal = CameraDistance;
		if (V.DriverPerson != base.gameObject)
		{
			base.enabled = false;
		}
		else
		{
			_playerController = base.gameObject.GetComponent<NewMoveController>();
			int num = PlayerPrefs.GetInt("lastPersonCamIndex", 0);
			if (ScenesHelper.IsMenuSceneActive() && (num == 1 || num == 2))
			{
				num = 3;
			}
			CameraPivot = _playerController.CameraPositionArray[num];
			CameraDistance = _playerController.CameraDistanceArray[num];
			CameraDistanceGlobal = base.gameObject.GetComponent<TouchMovePerson>().CameraDistance;
			if (num == 3)
			{
				angleMax = 60f;
				angleMaxUp = -30f;
				CamPosLerpSpeed = 1000f;
			}
			else
			{
				CamPosLerpSpeed = 7f;
				angleMax = 45f;
				angleMaxUp = -10f;
			}
			_playerController.CurrentCameraPosition = num;
		}
		_touchField = _controlItemsProvider.TouchField;
		_coreGameManager = V.CoreGameManager;
		_playerController = base.gameObject.GetComponent<NewMoveController>();
		_worldInteraction = base.gameObject.GetComponent<WorldInteractionController>();
		if ((bool)_touchField)
		{
			_touchField.UseFixedUpdate = true;
		}
	}

	private void FixedUpdate()
	{
		if (!_canMove || !_coreGameManager)
		{
			return;
		}
		if (!PlayerOutOfCar)
		{
			PlayerOutOfCar = true;
			if (_coreGameManager.TryGetComponent<PersonCameraControll>(out var component))
			{
				component.enabled = false;
			}
		}
		Vector3 vector = Quaternion.AngleAxis(InputRotationX, Vector3.up) * Vector3.forward;
		Vector3 vector2 = Quaternion.AngleAxis(InputRotationX + 90f, Vector3.up) * Vector3.forward;
		Vector3 vector3 = vector * _vertical + vector2 * _horizontal;
		LookDirection = Quaternion.AngleAxis(InputRotationY, vector2) * vector;
		_playerController.Input.RunX = vector3.x;
		_playerController.Input.RunZ = vector3.z;
		_playerController.Input.LookX = LookDirection.x;
		_playerController.Input.LookZ = LookDirection.z;
		CharacterPivot = Quaternion.AngleAxis(InputRotationX, Vector3.up) * CameraPivot;
		RaycastCheck();
	}

	private void Update()
	{
		_canMove = FlagsStorage.OutOfCar && base.gameObject == V.DriverPerson;
		if (FlagsStorage.InGarageMenu && _canMove)
		{
			_canMove = V.CoreGameManager.HasFirstCar() || FlagsStorage.FreeWalk;
		}
		if (FlagsStorage.OutOfCar && FlagsStorage.OutOfCar && base.gameObject == V.DriverPerson)
		{
			if (!driverActionController)
			{
				base.gameObject.TryGetComponent<DriverActionController>(out driverActionController);
			}
			Vector2 value = _lookFreeHandle.Get(_eventRegistry).Value;
			value = ScalePersonLook(value);
			InputRotationX += value.x;
			float value2 = _personRotateRightLeftHandle.Get(_eventRegistry).Value;
			InputRotationX += value2;
			Vector2 value3 = _movePersonHandle.Get(_eventRegistry).Value;
			InputRotationX %= 360f;
			InputRotationY = Mathf.Clamp(InputRotationY - value.y, angleMaxUp, angleMax);
			_vertical = value3.y;
			_horizontal = value3.x;
			Vector3 vector = Quaternion.AngleAxis(InputRotationX, Vector3.up) * Vector3.forward;
			Vector3 axis = Quaternion.AngleAxis(InputRotationX + 90f, Vector3.up) * Vector3.forward;
			LookDirection = Quaternion.AngleAxis(InputRotationY, axis) * vector;
			CharacterPivot = Quaternion.AngleAxis(InputRotationX, Vector3.up) * CameraPivot;
			CameraPosRot();
		}
	}

	private void RaycastCheck()
	{
		if (CameraDistanceGlobal == 0f)
		{
			return;
		}
		Vector3 position = base.transform.position;
		Vector3 origin = position + new Vector3(0f, 2f, 0f);
		Vector3 vector = -_coreGameManager.transform.position + position;
		LayerMask layerMask = -1;
		if (Physics.Raycast(origin, -vector, out var hitInfo, CameraDistanceGlobal, layerMask.value, QueryTriggerInteraction.Ignore))
		{
			if (hitInfo.transform.gameObject.layer != 17)
			{
				_raycastHitted = true;
				CameraDistance = hitInfo.distance - 0.4f;
			}
		}
		else if (_raycastHitted)
		{
			_raycastHitted = false;
			CameraDistance = CameraDistanceGlobal;
		}
	}

	public void SnapCameraBehind(Quaternion facing)
	{
		if (UpdateCamera)
		{
			if (!_coreGameManager)
			{
				_coreGameManager = V.CoreGameManager;
			}
			if ((bool)_coreGameManager)
			{
				InputRotationX = facing.eulerAngles.y;
				InputRotationY = 0f;
				Vector3 vector = Quaternion.AngleAxis(InputRotationX, Vector3.up) * Vector3.forward;
				Vector3 axis = Quaternion.AngleAxis(InputRotationX + 90f, Vector3.up) * Vector3.forward;
				LookDirection = Quaternion.AngleAxis(InputRotationY, axis) * vector;
				CharacterPivot = Quaternion.AngleAxis(InputRotationX, Vector3.up) * CameraPivot;
				Vector3 vector2 = base.transform.position + CharacterPivot;
				_coreGameManager.transform.position = vector2 - LookDirection * CameraDistance;
				_coreGameManager.transform.rotation = Quaternion.LookRotation(LookDirection, Vector3.up);
			}
		}
	}

	private void CameraPosRot()
	{
		if (!UpdateCamera || _menuLevelParent.isDroneEnabled)
		{
			return;
		}
		if (_coreGameManager == null)
		{
			_coreGameManager = V.CoreGameManager;
		}
		if (((bool)_freeDriveProvider.MapCamera && _freeDriveProvider.MapCamera.ShowingBigMap) || (bool)_coreGameManager.dealerWindow)
		{
			return;
		}
		if ((bool)driverActionController.ragdollCpm && driverActionController.ragdollCpm.isRagdoll)
		{
			if (sendRadnomPos)
			{
				if (frameSkip < 30 && driverActionController.ragdollCpm.StartedInCar)
				{
					_coreGameManager.transform.localPosition = posOnRagdollStart;
					_coreGameManager.transform.LookAt(base.gameObject.transform);
					frameSkip++;
					return;
				}
				sendRadnomPos = false;
			}
			if (sendRadnomPos)
			{
				return;
			}
			Vector3 vector = ((driverActionController.ragdollCpm.hips != null) ? driverActionController.ragdollCpm.hips.position : base.transform.position);
			Vector3 vector2 = new Vector3(LookDirection.x, 0f, LookDirection.z);
			if (vector2 == Vector3.zero)
			{
				Vector3 normalized = (_coreGameManager.transform.position - vector).normalized;
				vector2 = new Vector3(normalized.x, 0f, normalized.z);
				if (vector2 == Vector3.zero)
				{
					vector2 = Vector3.forward;
				}
			}
			vector2.Normalize();
			Vector3 b = vector - vector2 * CameraDistance + new Vector3(0f, 4f, 0f);
			_coreGameManager.transform.position = Vector3.Lerp(_coreGameManager.transform.position, b, Time.deltaTime * 2f);
			_coreGameManager.transform.LookAt(vector);
		}
		else
		{
			sendRadnomPos = true;
			frameSkip = 0;
			posOnRagdollStart = _coreGameManager.transform.localPosition;
			Vector3 vector3 = (_locomotion ? _locomotion.VisualOffset : Vector3.zero);
			Vector3 vector4 = base.transform.position + vector3 + CharacterPivot;
			if ((bool)_worldInteraction && _worldInteraction.TryGetCameraFocus(out var focus))
			{
				vector4 = focus;
			}
			target = vector4 - LookDirection * CameraDistance;
			_coreGameManager.transform.position = Vector3.Lerp(_coreGameManager.transform.position, target, Time.deltaTime * CamPosLerpSpeed);
			if (Vector3.Distance(_coreGameManager.transform.position, target) > 100f)
			{
				_coreGameManager.transform.position = target;
			}
			Quaternion b2 = ((LookDirection != Vector3.zero) ? Quaternion.LookRotation(LookDirection, Vector3.up) : new Quaternion(0f, 0f, 0f, 1f));
			_coreGameManager.transform.rotation = Quaternion.Lerp(_coreGameManager.transform.rotation, b2, Time.deltaTime * CamRotLerpSpeed);
		}
	}
}
