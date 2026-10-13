using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;

namespace Services.Drone
{
	public class FreeCameraController : MonoBehaviour
	{
		public const float MaxDistanceToHostSquare = 4900f;

		private const float MinZoomDist = 10f;

		private const float MaxZoomDist = 75f;

		private const float DistanceFromWall = 2f;

		private const float MultiplayerFor2Garage = 1.5f;

		private const string WallTag = "GarageWalls";

		private bool _accelerated;

		private int _speedModeIndex;

		private Vector3 _direction = Vector3.zero;

		private Vector3 _oldDirection = Vector3.zero;

		private Camera _camera;

		private float _distance = 10f;

		private UITouch _uiHandler;

		private UITouch _cameraMove;

		private float _zoomDist;

		private float _xAngle;

		private float _yAngle;

		private Vector3 _firstPoint = Vector3.zero;

		private Vector3 _firstPoint2;

		private Vector3 _secondPoint;

		private float _abs;

		private bool _pressedBoth;

		private Vector3 _angle = Vector3.zero;

		private bool _lookAt;

		private bool _isUITouched;

		private float _timerDelay;

		private FreeCameraConfig _config;

		private float _eulerZ;

		private MenuLevelParent _menuLevelParent;

		private CarCameras _carCameras;

		private FreeDriveProvider _freeDriveProvider;

		public float VerticalInput;

		public float CameraTilt;

		public FixedJoystick CircleJoystick { get; set; }

		public FixedJoystick VerticalJoystick { get; set; }

		public FixedJoystick ZoomJoystick { get; set; }

		public FixedJoystick DiagonalJoystick { get; set; }

		public bool IsCirclePressed { get; set; }

		public bool IsVerticalPressed { get; set; }

		public bool IsZoomPressed { get; set; }

		public bool IsDiagonalPressed { get; set; }

		private bool IsDroneActive
		{
			get
			{
				if (_menuLevelParent.isDroneEnabled)
				{
					return !_freeDriveProvider.MapCamera.ShowingBigMap;
				}
				return false;
			}
		}

		private Vector3 HostPosition
		{
			get
			{
				if (FlagsStorage.OutOfCar && (bool)V.DriverPerson)
				{
					return V.DriverPerson.transform.position;
				}
				if ((bool)V.Car)
				{
					return V.Car.transform.position;
				}
				return base.transform.position;
			}
		}

		[Inject]
		public void Construct(FreeCameraConfig config, MenuLevelParent menuLevelParent, CarCameras carCameras, FreeDriveProvider freeDriveProvider)
		{
			_freeDriveProvider = freeDriveProvider;
			_carCameras = carCameras;
			_menuLevelParent = menuLevelParent;
			_config = config;
		}

		private void Start()
		{
			FlagsStorage.MoveByTouch = true;
			_camera = GetComponent<Camera>();
		}

		private void OnEnable()
		{
			V.OnInGarageFreeChanged = (Action<bool>)Delegate.Combine(V.OnInGarageFreeChanged, new Action<bool>(InGarageFreeChangedCallback));
		}

		private void OnDisable()
		{
			V.OnInGarageFreeChanged = (Action<bool>)Delegate.Remove(V.OnInGarageFreeChanged, new Action<bool>(InGarageFreeChangedCallback));
		}

		private void OnCollisionEnter(Collision collision)
		{
			_direction /= 2f;
		}

		private void Update()
		{
			if (!IsDroneActive)
			{
				return;
			}
			bool flag = IsCirclePressed || IsVerticalPressed;
			_pressedBoth = IsCirclePressed && IsVerticalPressed;
			if (!FlagsStorage.IgnoreTouchesOnMenu)
			{
				switch (Input.touchCount)
				{
				case 1:
					if (flag || IsZoomPressed || IsDiagonalPressed)
					{
						ref int? touchID = ref _uiHandler.touchID;
						int valueOrDefault = touchID.GetValueOrDefault();
						if (!touchID.HasValue)
						{
							valueOrDefault = 0;
							touchID = valueOrDefault;
						}
						_cameraMove.touchID = null;
					}
					else
					{
						_uiHandler.touchID = null;
						ref int? touchID = ref _cameraMove.touchID;
						int valueOrDefault = touchID.GetValueOrDefault();
						if (!touchID.HasValue)
						{
							valueOrDefault = 0;
							touchID = valueOrDefault;
						}
					}
					break;
				case 2:
					if (_uiHandler.touchID.HasValue)
					{
						if ((IsCirclePressed && !IsVerticalPressed && !IsZoomPressed && !IsDiagonalPressed) || (!IsCirclePressed && IsVerticalPressed && !IsZoomPressed && !IsDiagonalPressed) || (!IsCirclePressed && !IsVerticalPressed && IsZoomPressed && !IsDiagonalPressed) || (!IsCirclePressed && !IsVerticalPressed && !IsZoomPressed && IsDiagonalPressed))
						{
							_cameraMove.touchID = (_uiHandler.touchID + 1) % 2;
						}
						else
						{
							_cameraMove.touchID = null;
						}
					}
					else if (_cameraMove.touchID.HasValue)
					{
						if (flag)
						{
							_uiHandler.touchID = (_cameraMove.touchID + 1) % 2;
						}
						else
						{
							_uiHandler.touchID = null;
						}
					}
					break;
				default:
					_uiHandler.touchID = null;
					_cameraMove.touchID = null;
					break;
				case 3:
					break;
				}
			}
			if (flag || IsDiagonalPressed)
			{
				_direction = Vector3.zero;
				if (Mathf.Abs(_direction.z) < _config.MaxSpeed && CircleJoystick.Vertical != 0f)
				{
					_direction += Vector3.forward * (_config.GetAcceleration(_speedModeIndex) * CircleJoystick.Vertical);
					_accelerated = true;
				}
				if (Mathf.Abs(_direction.x) < _config.MaxSpeed && CircleJoystick.Horizontal != 0f)
				{
					_direction += Vector3.right * (_config.GetAcceleration(_speedModeIndex) * CircleJoystick.Horizontal);
					_accelerated = true;
				}
				if ((bool)VerticalJoystick)
				{
					VerticalInput = VerticalJoystick.Vertical;
				}
				if (Mathf.Abs(_direction.y) < _config.MaxSpeed && VerticalInput != 0f)
				{
					_direction += Vector3.up * (_config.GetVerticalSpeed(_speedModeIndex) * VerticalInput);
					_accelerated = true;
				}
			}
			else if (IsDroneActive && !_uiHandler.touchID.HasValue)
			{
				if (((bool)CircleJoystick && CircleJoystick.Direction.magnitude > 0.5f) || ((bool)VerticalJoystick && VerticalJoystick.Direction.magnitude > 0.5f))
				{
					return;
				}
				if (Input.touchCount == 2 && !IsZoomPressed)
				{
					Zoom();
				}
				_accelerated = false;
			}
			if (IsZoomPressed)
			{
				if ((bool)ZoomJoystick && ZoomJoystick.Vertical != 0f)
				{
					_zoomDist = Mathf.Clamp(_camera.fieldOfView - ZoomJoystick.Vertical * _config.PerspectiveZoomSpeed, 10f, 75f);
				}
				_camera.fieldOfView = _zoomDist;
			}
			if (IsDiagonalPressed)
			{
				Vector3 eulerAngles = base.transform.eulerAngles;
				_eulerZ = CameraTilt;
				if ((bool)DiagonalJoystick)
				{
					_eulerZ = eulerAngles.z + DiagonalJoystick.Horizontal * _config.RotatioSpeed;
				}
				base.transform.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, _eulerZ);
			}
		}

		public void ResetDiagonalRot()
		{
			_eulerZ = 0f;
			CameraTilt = 0f;
			base.transform.eulerAngles = new Vector3(base.transform.eulerAngles.x, base.transform.eulerAngles.y, _eulerZ);
		}

		private void LateUpdate()
		{
			if (!IsDroneActive)
			{
				return;
			}
			Vector3 position = base.transform.position;
			Raycast();
			CheckForUITouch();
			CameraMove();
			Move(_direction);
			if (FlagsStorage.InGarageFree)
			{
				_timerDelay += Time.deltaTime;
				if (_timerDelay > 0.5f)
				{
					_timerDelay = 0f;
					HandleCameraCollision();
				}
			}
			float num = Extensions.DistanceSquare(base.transform.position, HostPosition);
			if (num > 4900f && num > Extensions.DistanceSquare(position, HostPosition))
			{
				base.transform.position = position;
			}
		}

		public void MoveExternalClamped(Vector3 delta)
		{
			Vector3 vector = base.transform.position + delta;
			float num = Extensions.DistanceSquare(vector, HostPosition);
			if (!(num > 4900f) || !(num > Extensions.DistanceSquare(base.transform.position, HostPosition)))
			{
				base.transform.position = vector;
			}
		}

		public void LookAt()
		{
			_lookAt = !_lookAt;
			_eulerZ = 0f;
		}

		public void SetSport()
		{
			_speedModeIndex++;
		}

		private void Raycast()
		{
			if (Physics.Raycast(base.transform.position, _direction, out var hitInfo, 15f, -1, QueryTriggerInteraction.Ignore) && hitInfo.distance < 1f)
			{
				_direction = Vector3.zero;
			}
		}

		private void CameraMove()
		{
			if (_isUITouched)
			{
				return;
			}
			RotateCamera();
			if (_pressedBoth || Input.touchCount < _cameraMove.touchID + 1 || !_cameraMove.touchID.HasValue)
			{
				_firstPoint = Vector3.zero;
				return;
			}
			Touch touch = Input.GetTouch(_cameraMove.touchID.GetValueOrDefault());
			if (touch.phase == TouchPhase.Began)
			{
				Began(touch);
			}
			else if (touch.phase == TouchPhase.Moved)
			{
				if (_firstPoint == Vector3.zero)
				{
					Began(touch);
				}
				else
				{
					_secondPoint = touch.position;
				}
			}
			else if (touch.phase == TouchPhase.Ended)
			{
				_secondPoint = Vector3.zero;
				_firstPoint = Vector3.zero;
			}
		}

		private void CheckForUITouch()
		{
			if (_cameraMove.touchID.HasValue && Input.touchCount > _cameraMove.touchID)
			{
				if (Input.GetTouch(_cameraMove.touchID.GetValueOrDefault()).phase == TouchPhase.Began && (bool)EventSystem.current && EventSystem.current.IsPointerOverGameObject())
				{
					_isUITouched = true;
				}
			}
			else
			{
				_isUITouched = false;
			}
		}

		private void InGarageFreeChangedCallback(bool inGarageFree)
		{
			if (IsDroneActive)
			{
				GarageUpdatedAsync(this.GetCancellationTokenOnDestroy()).Forget();
			}
		}

		private async UniTaskVoid GarageUpdatedAsync(CancellationToken ct = default(CancellationToken))
		{
			try
			{
				for (int i = 0; i < 10; i++)
				{
					await UniTask.Yield(ct);
					ResetDiagonalRot();
					base.transform.position = V.Car.transform.position + V.Car.transform.forward * -3f + Vector3.up * 1.5f;
					base.transform.rotation = V.Car.transform.rotation;
				}
			}
			catch (OperationCanceledException)
			{
			}
		}

		private void RotateCamera()
		{
			if (_firstPoint != Vector3.zero && _secondPoint != Vector3.zero)
			{
				_xAngle = (_secondPoint.x - _firstPoint.x) * 90f / (float)Screen.width * 2f;
				_yAngle = (_secondPoint.y - _firstPoint.y) * 45f / (float)Screen.height * 2f;
				_yAngle = ((_yAngle > 180f) ? (_yAngle - 360f) : _yAngle);
				if (_yAngle >= 60f)
				{
					_yAngle = 60f;
				}
				if (_yAngle < -60f)
				{
					_yAngle = -60f;
				}
				float rotationSpeed = _config.GetRotationSpeed(_speedModeIndex);
				_angle = new Vector2(0f - _yAngle, _xAngle) * rotationSpeed;
			}
			else
			{
				_angle /= 1.2f;
			}
			if (_lookAt)
			{
				base.transform.LookAt(V.DriverPerson.transform.position + Vector3.up);
				Vector3 eulerAngles = base.transform.rotation.eulerAngles;
				eulerAngles.z = _eulerZ;
				base.transform.rotation = Quaternion.Euler(eulerAngles);
				return;
			}
			Quaternion localRotation = base.transform.localRotation;
			float z = localRotation.eulerAngles.z;
			Quaternion quaternion = Quaternion.Euler(_angle * Time.deltaTime);
			localRotation *= quaternion;
			float num = localRotation.eulerAngles.x;
			if (num >= 0f && num <= 90f)
			{
				num = Mathf.Clamp(num, 0f, 90f - _config.AddValue);
			}
			else if (num >= 270f && num <= 360f)
			{
				num = Mathf.Clamp(num, 270f + _config.AddValue, 360f);
			}
			localRotation = Quaternion.Euler(num, localRotation.eulerAngles.y, z);
			base.transform.localRotation = localRotation;
		}

		private void Began(Touch touch)
		{
			_firstPoint = touch.position;
			_carCameras.WidthPercent = _firstPoint.x / (float)Screen.width * 100f;
		}

		private void Move(Vector3 direction)
		{
			if (direction == Vector3.zero)
			{
				return;
			}
			_abs = Mathf.Max(Mathf.Abs(_direction.x), Mathf.Abs(_direction.y), Mathf.Abs(_direction.z));
			Transform transform = base.transform;
			if (!_accelerated)
			{
				if (_abs < 1f)
				{
					_direction = Vector3.zero;
					return;
				}
				if (_direction.x > 0f)
				{
					_direction -= Vector3.right;
				}
				else if (_direction.x < 0f)
				{
					_direction += Vector3.right;
				}
				if (_direction.y > 0f)
				{
					_direction -= Vector3.up;
				}
				else if (_direction.y < 0f)
				{
					_direction += Vector3.up;
				}
				if (_direction.z > 0f)
				{
					_direction -= Vector3.forward;
				}
				else if (_direction.z < 0f)
				{
					_direction += Vector3.forward;
				}
			}
			float y = direction.y;
			float y2 = transform.position.y;
			direction.y = 0f;
			base.transform.Translate(direction * Time.deltaTime, Space.Self);
			base.transform.position = new Vector3(base.transform.position.x, y2, base.transform.position.z);
			base.transform.Translate(new Vector3(0f, y, 0f) * Time.deltaTime, Space.World);
			Vector3 eulerAngles = transform.rotation.eulerAngles;
			if (_lookAt)
			{
				base.transform.LookAt(V.DriverPerson.transform.position + Vector3.up);
				Vector3 eulerAngles2 = base.transform.rotation.eulerAngles;
				eulerAngles2.z = _eulerZ;
				base.transform.rotation = Quaternion.Euler(eulerAngles2);
			}
			else
			{
				base.transform.rotation = Quaternion.Euler(eulerAngles.x - (_oldDirection.z - direction.z) / _config.GetAcceleration(_speedModeIndex) * _config.SwingSpeed, eulerAngles.y, eulerAngles.z + (_oldDirection.x - direction.x) / _config.GetAcceleration(_speedModeIndex) * _config.SwingSpeed);
			}
			_oldDirection = direction;
		}

		private void HandleCameraCollision()
		{
			Vector3 vector = V.DriverPerson.transform.position + new Vector3(0f, 2f, 0f);
			Vector3 normalized = (base.transform.position - vector).normalized;
			float num = Vector3.Distance(vector, base.transform.position);
			RaycastHit[] source = Physics.RaycastAll(new Ray(vector, normalized), num + 0.5f);
			if (source.Any((RaycastHit x) => x.collider.CompareTag("GarageWalls")))
			{
				RaycastHit raycastHit = source.First((RaycastHit x) => x.collider.CompareTag("GarageWalls"));
				base.transform.position = raycastHit.point + raycastHit.normal * ((!ScenesHelper.IsSceneActive(42)) ? 2f : 3f);
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawRay(base.transform.position, base.transform.rotation * _direction);
		}

		private void Zoom()
		{
			if (!_isUITouched)
			{
				_firstPoint = Vector3.zero;
				Touch touch = Input.GetTouch(0);
				Touch touch2 = Input.GetTouch(1);
				Vector2 vector = touch.position - touch.deltaPosition;
				Vector2 vector2 = touch2.position - touch2.deltaPosition;
				float magnitude = (vector - vector2).magnitude;
				float magnitude2 = (touch.position - touch2.position).magnitude;
				float num = magnitude - magnitude2;
				float min = 10f;
				float max = 75f;
				_distance = Mathf.Clamp(_camera.fieldOfView + num * _config.PerspectiveZoomSpeed, min, max);
				_camera.fieldOfView = _distance;
			}
		}

		public void OnOpen()
		{
			_speedModeIndex = 0;
			_lookAt = false;
		}
	}
}
