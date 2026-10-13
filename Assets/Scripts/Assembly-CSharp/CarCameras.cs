using System;
using System.Collections.Generic;
using CPM.CameraManagement.Obstruction;
using CPM.Scripts.VinylDrawer;
using CPMEngine;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

[RequireComponent(typeof(CarCamerasController))]
public class CarCameras : MonoBehaviour
{
	public enum Cameras
	{
		SmoothLookAt = 0,
		MouseOrbit = 1,
		FixedTo = 2,
		Map = 3,
		CraftTable = 4,
		InsideCarForAccessories = 5
	}

	public enum SteamLook
	{
		Default = 0,
		Fixed = 1,
		Free = 2
	}

	private const string AxisNameX = "Mouse X";

	private const string AxisNameY = "Mouse Y";

	private const string MouseScrollWheel = "Mouse ScrollWheel";

	public Powertrain dr;

	public Axles axles;

	[FormerlySerializedAs("rigidbody")]
	public Rigidbody carRigidbody;

	private Camera _cam;

	public Cameras mycamera;

	public float distance = 10f;

	public float height = 2.5f;

	[HideInInspector]
	public float DefaultHeight;

	[HideInInspector]
	public float FpvDefaultPitch;

	public float yawAngle;

	[Range(-90f, 90f)]
	public float rotationAngle;

	public float pitchAngle = -2.5f;

	public float rotationDamping = 3f;

	public float heightDamping = 2f;

	public float distanceDamping = 1f;

	private const float CollisionPullInSpeed = 25f;

	private const float CollisionPushOutSpeed = 4f;

	private const float CollisionDistanceSnap = 50f;

	private float _smoothCollisionDistance;

	[HideInInspector]
	public bool dampFixedCamera;

	[HideInInspector]
	public bool mouseOrbitFixedCamera;

	[HideInInspector]
	public bool driverView;

	public float perspectiveZoomSpeed = 0.01f;

	private float _xSpeed = 10f;

	private float _ySpeed = 10f;

	private float _yMinLimit = -20f;

	private float _yMaxLimit = 80f;

	private float _mouseDamping = 10f;

	public float SmoothFollowAdd = 4.5f;

	[HideInInspector]
	public float distanceMin = 4f;

	[HideInInspector]
	public float distanceMax = 30f;

	public bool isDistanceDamping;

	[HideInInspector]
	public float x;

	[HideInInspector]
	public float y;

	public float currentYawRotationAngle;

	public float wantedYawRotationAngle;

	public float currentDistance;

	[HideInInspector]
	public SteamLook SteamLookMode;

	[HideInInspector]
	public float freeOrbitYaw;

	[SerializeField]
	[Header("Steam speed camera (FOV + pullback)")]
	private SteamSpeedCameraConfig _speedFovConfig;

	[HideInInspector]
	public float ZoomFov = 50f;

	private float _speedDistanceBonus;

	private float _currentHeight;

	private float _wantedHeight;

	private float _deltaPitchAngle;

	private Vector3 _deltaMovement;

	private Vector3 _oldVelocity;

	private Vector3 _deltaVelocity;

	private Vector3 _velocity;

	private Vector3 _accel;

	private float _centrifugalAccel;

	[HideInInspector]
	public Transform myTransform;

	[HideInInspector]
	public Transform mtarget;

	public Quaternion rotation;

	public float xAngle;

	public float yAngle;

	private float xAngTemp;

	private float yAngTemp;

	private Vector3 _firstpoint;

	private Vector3 _firstpoin2;

	private Vector3 _secondpoint;

	private float _xAngTemp2;

	private float _yAngTemp2;

	public bool isbegan;

	private float _yVelocity;

	private float timer1;

	private float _lastVelo;

	public float DistDamp;

	public float Koff;

	public float Acceleration;

	public bool RaycastHitted;

	public Vector3 RaycastHitDirection;

	public float RaycastDistance;

	public float pitchAngleOffset;

	public bool ShakeZ;

	[Tooltip("Layers treated as solid obstructions between the follow camera and the car. Falls back to a sensible default (Default/Road/Tunnel/Terrain/ParkingCubes/Garage) if left as \"Nothing\".")]
	[Header("Camera Obstruction")]
	public LayerMask cameraObstructionMask;

	[Tooltip("Radius of the sphere cast used to detect obstructions. Big enough that corners of geometry aren't missed by a thin ray, small enough not to snag on walls the camera merely drives past.")]
	public float obstructionSphereRadius = 0.15f;

	[Tooltip("Extra distance kept between the camera and a detected obstruction.")]
	public float obstructionMargin = 0.3f;

	[Tooltip("The camera never gets closer to the car than this, even when pushed in by an obstruction.")]
	public float minObstructedDistance = 1.5f;

	private static readonly string[] DefaultObstructionLayerNames = new string[6] { "Default", "Road", "Tunnel", "Terrain", "ParkingCubes", "Garage" };

	[Tooltip("Sudden pull-ins larger than this (metres per frame) wait one frame for confirmation before the camera acts on them. Filters single stray hits without damping real walls.")]
	public float obstructionSuddenDropThreshold = 1f;

	[Range(0f, 1f)]
	[Tooltip("Share of its normal height above the car the camera keeps when an obstruction presses it all the way in. Keeps the view from dropping level with the car.")]
	public float obstructedMinHeightFraction = 0.3f;

	private readonly RaycastHit[] _obstructionHitsBuffer = new RaycastHit[16];

	private readonly List<ObstructionCandidate> _obstructionCandidates = new List<ObstructionCandidate>(16);

	private float _lastAppliedObstructionDistance = float.PositiveInfinity;

	private bool _obstructionDropPending;

	public bool StopShaking;

	public float TargetZ;

	private IDisposable _stopShakeDisposable;

	public float ShakeZSpeed = 5f;

	private float _lastZ;

	public float RangeShakeZ = 5f;

	public bool JumpHeight;

	public float HeightAdd;

	public float TargetHeightAdd;

	public float HeightShakeSpeed = 5f;

	public float HeightShakeRange = 0.2f;

	private GameObject _currentCar;

	private int _count1;

	private int _count2;

	public Transform posTargetFollow;

	public Transform targetMainMenu;

	public float speedFollow;

	public bool FollowPosRot;

	public Transform rotTargetFollow;

	public Vector3 targetPositon;

	public Vector3 targetPositionOfRotation;

	public bool makeRayCast = true;

	private float _startTimeZShake;

	private float _startTimeHeightShake;

	private Vector3 _targetPos;

	public Vector3 addEuler;

	public float minDistacne = 3.5f;

	public float maxDistance = 8f;

	public bool changingTarget;

	public Transform veloTarget;

	public Quaternion VeloTargetQuaternion;

	public float veloFollowSpeed = 5f;

	public bool camChangedByTouch;

	public bool lerpPosition;

	public bool beganTouching;

	private float _lerpPositionSpeed;

	private const float SecondsForceCamera = 4f;

	private MenuLevelParent _menuLevelParent;

	private LevelScript _levelScript;

	private OptionalDependency<LevelScript> _levelScriptDep;

	private FreeDriveProvider _freeDriveProvider;

	private Transform _target;

	private Transform _realTarget;

	private Vector3 _spectatorPrevPos;

	private bool _spectatorPrevPosValid;

	private Vector3 _spectatorVelocity;

	public bool IsRotating;

	public float WidthPercent { get; set; }

	[field: SerializeField]
	private bool ForceForwardVelocity { get; set; }

	public Transform Target
	{
		get
		{
			return _target;
		}
		set
		{
			_target = value;
			_realTarget = _target;
			if ((bool)_realTarget && _realTarget.gameObject.CompareTag("Car"))
			{
				Transform transform = _realTarget.Find("CarCommonCameraTarget");
				if ((bool)transform)
				{
					_realTarget = transform;
				}
			}
		}
	}

	private bool IsSpeedFovCam
	{
		get
		{
			if ((bool)_cam && !FlagsStorage.OutOfCar && (bool)V.Car && V.CurrentCamIndex != 0)
			{
				return !driverView;
			}
			return false;
		}
	}

	public bool SpeedFovControlsFov
	{
		get
		{
			if (IsSpeedFovCam)
			{
				if (!(_speedFovConfig == null))
				{
					return _speedFovConfig.EnableFov;
				}
				return true;
			}
			return false;
		}
	}

	[Inject]
	public void Construct(MenuLevelParent menuLevelParent, FreeDriveProvider freeDriveProvider, OptionalDependency<LevelScript> levelScript)
	{
		_freeDriveProvider = freeDriveProvider;
		_levelScriptDep = levelScript;
		_menuLevelParent = menuLevelParent;
	}

	private void Awake()
	{
		_levelScript = _levelScriptDep?.Value;
		_freeDriveProvider.CarCameras = this;
	}

	private void OnDestroy()
	{
		_freeDriveProvider.CarCameras = null;
	}

	private void Start()
	{
		if (ScenesHelper.IsParkingLevelActive())
		{
			InitStart();
		}
		ChangeForceCameraForSecond();
	}

	public void InitStart()
	{
		DistDamp = 2f;
		_smoothCollisionDistance = distance;
		makeRayCast = true;
		_cam = GetComponent<Camera>();
		targetMainMenu = new GameObject("targetFollowCam").transform;
		posTargetFollow = new GameObject("posTargetFollow").transform;
		rotTargetFollow = new GameObject("rotTargetFollow").transform;
		veloTarget = new GameObject("veloTarget").transform;
		myTransform = base.transform;
		mtarget = Target;
		if ((bool)mtarget)
		{
			mtarget.TryGetComponent<Rigidbody>(out carRigidbody);
			mtarget.TryGetComponent<Axles>(out axles);
			mtarget.TryGetComponent<Powertrain>(out dr);
		}
	}

	private void RaycastForDistance()
	{
		if (makeRayCast && mycamera == Cameras.SmoothLookAt && !FlagsStorage.FixedCameraAndLookAt && FlagsStorage.CamFollowCar && (bool)Target && !FlagsStorage.MoveByTouch && !FlagsStorage.FixedCameraAndLookAt && FlagsStorage.CamFollowCar && (bool)Target && !FlagsStorage.MoveByTouch)
		{
			MakeRaycast();
		}
	}

	private void MakeRaycast()
	{
		Vector3 position = _realTarget.position;
		RaycastHitDirection = position - targetMainMenu.transform.position;
		EnsureObstructionMaskInitialized();
		int num = Physics.SphereCastNonAlloc(position, obstructionSphereRadius, -RaycastHitDirection.normalized, _obstructionHitsBuffer, distance, cameraObstructionMask.value, QueryTriggerInteraction.Ignore);
		Transform transform = (_realTarget ? _realTarget.root : null);
		_obstructionCandidates.Clear();
		for (int i = 0; i < num; i++)
		{
			RaycastHit raycastHit = _obstructionHitsBuffer[i];
			Transform transform2 = raycastHit.transform;
			if ((bool)transform2)
			{
				Transform root = transform2.root;
				bool isIgnored = ((bool)transform && root == transform) || root.CompareTag("Car") || root.CompareTag("Trailer") || root.CompareTag("Draggable") || transform2.CompareTag("Car") || transform2.CompareTag("Trailer") || transform2.CompareTag("Draggable");
				_obstructionCandidates.Add(new ObstructionCandidate(raycastHit.distance, isIgnored));
			}
		}
		float? num2 = CameraObstructionResolver.ResolveDistance(_obstructionCandidates, obstructionMargin, minObstructedDistance);
		if (num2.HasValue && CameraObstructionResolver.AcceptSuddenDrop(_lastAppliedObstructionDistance, num2.Value, obstructionSuddenDropThreshold, ref _obstructionDropPending))
		{
			RaycastHitted = true;
			RaycastDistance = num2.Value;
		}
		else
		{
			RaycastHitted = false;
			RaycastDistance = distance;
			if (!num2.HasValue)
			{
				_obstructionDropPending = false;
			}
		}
		_lastAppliedObstructionDistance = RaycastDistance;
	}

	private void EnsureObstructionMaskInitialized()
	{
		if (cameraObstructionMask.value == 0)
		{
			cameraObstructionMask = LayerMask.GetMask(DefaultObstructionLayerNames);
		}
	}

	private void MakeRaycast2()
	{
		Vector3 position = _realTarget.position;
		Vector3 vector = position - myTransform.position;
		LayerMask layerMask = ~(1 << LayerMask.NameToLayer("short distance"));
		if (Physics.Raycast(position, -vector, out var hitInfo, distance, layerMask.value, QueryTriggerInteraction.Ignore))
		{
			if ((!hitInfo.transform.CompareTag("Car") && !hitInfo.transform.CompareTag("Draggable") && hitInfo.transform.gameObject.layer != 27) || V.MenuIndex == 4)
			{
				RaycastHitted = true;
				RaycastDistance = hitInfo.distance - 0.3f;
				if (RaycastDistance < 1.5f)
				{
					RaycastDistance = 1.5f;
				}
			}
			else
			{
				RaycastDistance = distance;
				RaycastHitted = false;
			}
		}
		else
		{
			RaycastDistance = distance;
			RaycastHitted = false;
		}
	}

	private void LateUpdate()
	{
		if (_menuLevelParent.isDroneEnabled)
		{
			return;
		}
		ApplySpeedFov();
		ClampPositions();
		if (FollowPosRot)
		{
			if ((bool)rotTargetFollow && (bool)posTargetFollow)
			{
				base.transform.LookAt(rotTargetFollow);
				base.transform.position = posTargetFollow.position;
				float d = 0.5f;
				if ((bool)_levelScript && (bool)_levelScript.LastDroppedLoad && FlagsStorage.LoadCollidedInvoked)
				{
					targetPositon = _levelScript.LastDroppedLoad.position;
					targetPositionOfRotation = _levelScript.LastDroppedLoad.position;
					d = 2f;
				}
				if (VectorExtensions.IsDistanceMoreThan(posTargetFollow.position, targetPositon, d))
				{
					posTargetFollow.position = Vector3.MoveTowards(posTargetFollow.position, targetPositon, 0.2f);
				}
				rotTargetFollow.position = Vector3.MoveTowards(rotTargetFollow.position, targetPositionOfRotation, 0.2f);
				_cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, 45f, 0.01f);
			}
			return;
		}
		if (!_realTarget || FlagsStorage.OutOfCar || mycamera == Cameras.MouseOrbit)
		{
			return;
		}
		Vector3 rigidBodyVelocityForSmoothLookAt;
		float num5;
		int num10;
		if (mycamera == Cameras.SmoothLookAt)
		{
			if (!FlagsStorage.FixedCameraAndLookAt)
			{
				if (!FlagsStorage.CamFollowCar)
				{
					return;
				}
				if (FlagsStorage.MoveByTouch)
				{
					float num = yAngle;
					float num2 = xAngle;
					float z = base.transform.eulerAngles.z;
					z = Mathf.Clamp(z, -2f, 2f);
					rotation = Quaternion.Lerp(targetMainMenu.rotation, Quaternion.Euler(num, num2, z), Time.deltaTime * 10f);
					Vector3 vector = _realTarget.position;
					if (changingTarget)
					{
						vector = _realTarget.position + new Vector3(0f, 2f, 0f);
					}
					distanceDamping = 200f;
					MakeRaycast2();
					if (RaycastHitted && !V.CoreGameManager.menuControl.selectingPersonNow)
					{
						currentDistance = RaycastDistance - 0.1f;
					}
					else
					{
						currentDistance = Mathf.Lerp(currentDistance, distance, distanceDamping * Time.deltaTime);
					}
					Vector3 vector2 = new Vector3(0f, 0f, 0f - currentDistance);
					Vector3 position = rotation * vector2 + vector;
					if (!changingTarget)
					{
						targetMainMenu.rotation = rotation;
					}
					targetMainMenu.position = position;
					Quaternion b = Quaternion.Euler(targetMainMenu.rotation.eulerAngles + addEuler);
					base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, Time.deltaTime * speedFollow);
					float num3 = speedFollow;
					if (changingTarget)
					{
						num3 = 1.5f;
					}
					else if (num3 > 1f && num3 != 10f)
					{
						num3 = Vector3.Distance(base.transform.position, targetMainMenu.position);
					}
					float num4 = Vector3.Distance(base.transform.position, targetMainMenu.position);
					if (num4 > 100f)
					{
						base.transform.position = targetMainMenu.position;
						return;
					}
					if (num4 < 5f)
					{
						changingTarget = false;
					}
					base.transform.position = Vector3.Lerp(base.transform.position, targetMainMenu.position, Time.deltaTime * num3);
					return;
				}
				if (!carRigidbody)
				{
					V.Car.TryGetComponent<Rigidbody>(out carRigidbody);
				}
				if (!axles)
				{
					V.Car.TryGetComponent<Axles>(out axles);
				}
				rigidBodyVelocityForSmoothLookAt = GetRigidBodyVelocityForSmoothLookAt();
				num5 = rigidBodyVelocityForSmoothLookAt.magnitude * 0.02f;
				if ((double)num5 > 0.3)
				{
					num5 = 0.3f;
				}
				bool flag = false;
				if (flag)
				{
					ShakeZSpeed = 2f;
					RangeShakeZ = 3f;
					HeightShakeRange = 0.2f;
					HeightShakeSpeed = 1f;
				}
				if (JumpHeight | flag)
				{
					HeightShaking();
				}
				veloTarget.position = _realTarget.position;
				if (rigidBodyVelocityForSmoothLookAt.magnitude > 1f && V.CurrentCamIndex == 2)
				{
					Vector3 normalized = rigidBodyVelocityForSmoothLookAt.normalized;
					Vector3 forward = _realTarget.transform.position + 1f * normalized - _realTarget.position;
					VeloTargetQuaternion = Quaternion.LookRotation(forward, Vector3.up);
					veloTarget.transform.rotation = Quaternion.Lerp(veloTarget.transform.rotation, VeloTargetQuaternion, Time.deltaTime * veloFollowSpeed);
				}
				switch (SteamLookMode)
				{
				case SteamLook.Fixed:
					wantedYawRotationAngle = _realTarget.eulerAngles.y;
					break;
				case SteamLook.Free:
					wantedYawRotationAngle = freeOrbitYaw;
					break;
				default:
					if (V.CurrentCamIndex == 2)
					{
						wantedYawRotationAngle = VeloTargetQuaternion.eulerAngles.y + yawAngle;
					}
					else
					{
						wantedYawRotationAngle = _realTarget.eulerAngles.y + yawAngle;
					}
					break;
				}
				currentYawRotationAngle = myTransform.eulerAngles.y;
				float num6 = 0f;
				float num7 = Mathf.Abs(_realTarget.eulerAngles.x);
				if (num7 < 30f)
				{
					num6 = num7 / 10f;
				}
				_wantedHeight = _realTarget.position.y + height + HeightAdd + num6;
				if (FlagsStorage.InGarageFree && _wantedHeight > RaycastDistance && !FlagsStorage.IsInGarageTunnel)
				{
					_wantedHeight = RaycastDistance;
				}
				_currentHeight = myTransform.position.y;
				currentYawRotationAngle = Mathf.LerpAngle(currentYawRotationAngle, wantedYawRotationAngle, _mouseDamping * Time.deltaTime);
				_currentHeight = Mathf.Lerp(_currentHeight, _wantedHeight, _mouseDamping * Time.deltaTime);
				if (Vector3.Distance(myTransform.position, _realTarget.position) > 60f)
				{
					_currentHeight = _wantedHeight;
					currentYawRotationAngle = wantedYawRotationAngle;
					lerpPosition = false;
				}
				float num8 = (RaycastHitted ? RaycastDistance : distance);
				if (!RaycastHitted && Mathf.Abs(_smoothCollisionDistance - num8) > 50f)
				{
					_smoothCollisionDistance = num8;
				}
				float num9 = ((num8 < _smoothCollisionDistance) ? 25f : 4f);
				_smoothCollisionDistance = Mathf.Lerp(_smoothCollisionDistance, num8, num9 * Time.deltaTime);
				if (RaycastHitted)
				{
					if (!(distance < 0.3f))
					{
						if (FlagsStorage.InGarageFree && _wantedHeight > RaycastDistance)
						{
							num10 = ((!FlagsStorage.IsInGarageTunnel) ? 1 : 0);
							if (num10 != 0)
							{
								goto IL_07c2;
							}
						}
						else
						{
							num10 = 0;
						}
						goto IL_07ce;
					}
					num10 = 1;
					goto IL_07c2;
				}
				Quaternion quaternion = Quaternion.Euler(0f, currentYawRotationAngle, 0f);
				targetMainMenu.position = _realTarget.position;
				if (FlagsStorage.SmoothFollow)
				{
					DampDistance();
				}
				if (isDistanceDamping)
				{
					currentDistance = Mathf.Lerp(currentDistance, distance, distanceDamping * Time.deltaTime);
					if (Mathf.Abs(currentDistance - distance) > 100f)
					{
						currentDistance = distance;
					}
					targetMainMenu.position -= quaternion * Vector3.forward * (currentDistance + num5 + _speedDistanceBonus);
				}
				else
				{
					targetMainMenu.position -= quaternion * Vector3.forward * (_smoothCollisionDistance + num5 + _speedDistanceBonus);
				}
				targetMainMenu.position = new Vector3(targetMainMenu.position.x, _currentHeight, targetMainMenu.position.z);
				goto IL_09f4;
			}
			base.transform.LookAt(_realTarget);
			if (rotationAngle != 0f)
			{
				Vector3 eulerAngles = myTransform.transform.rotation.eulerAngles;
				myTransform.transform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z + rotationAngle);
			}
			return;
		}
		if (mycamera == Cameras.FixedTo)
		{
			rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.Euler(y + _realTarget.eulerAngles.x + pitchAngle + _deltaPitchAngle, x + _realTarget.eulerAngles.y + yawAngle, _realTarget.eulerAngles.z), Time.deltaTime * _mouseDamping);
			myTransform.rotation = rotation;
			if (dampFixedCamera)
			{
				Vector3 position2 = new Vector3(_realTarget.position.x, _realTarget.position.y + height, _realTarget.position.z) - myTransform.forward * distance - (_deltaMovement.z * _realTarget.forward + _deltaMovement.y * _realTarget.up + _deltaMovement.x * _realTarget.right);
				myTransform.position = position2;
			}
			else
			{
				myTransform.eulerAngles = new Vector3(_realTarget.eulerAngles.x + pitchAngle, _realTarget.eulerAngles.y + yawAngle, _realTarget.eulerAngles.z);
				myTransform.position = new Vector3(_realTarget.position.x, _realTarget.position.y + height, _realTarget.position.z) - myTransform.forward * distance;
			}
		}
		else
		{
			_ = mycamera;
			_ = 4;
		}
		return;
		IL_09f4:
		targetMainMenu.LookAt(new Vector3(_realTarget.position.x, _realTarget.position.y + height + pitchAngle, _realTarget.position.z));
		if (lerpPosition)
		{
			_lerpPositionSpeed = (rigidBodyVelocityForSmoothLookAt.magnitude + 0.5f) * 0.3f + _lerpPositionSpeed;
			myTransform.position = Vector3.Lerp(base.transform.position, targetMainMenu.position, Time.deltaTime * _lerpPositionSpeed);
			float num11 = Vector3.Distance(myTransform.position, targetMainMenu.position);
			if (num11 < 0.01f || num11 > 30f)
			{
				lerpPosition = false;
			}
		}
		else
		{
			_lerpPositionSpeed = 0f;
			myTransform.position = targetMainMenu.position;
		}
		myTransform.transform.rotation = targetMainMenu.rotation;
		Vector3 eulerAngles2 = myTransform.transform.rotation.eulerAngles;
		myTransform.transform.rotation = Quaternion.Euler(eulerAngles2.x, eulerAngles2.y, eulerAngles2.z + rotationAngle);
		CheckWheelForShake();
		RaycastForDistance();
		return;
		IL_07c2:
		_wantedHeight = RaycastDistance;
		goto IL_07ce;
		IL_07ce:
		Quaternion quaternion2 = Quaternion.Euler(0f, currentYawRotationAngle, 0f);
		targetMainMenu.position = _realTarget.position;
		targetMainMenu.position -= quaternion2 * Vector3.forward * _smoothCollisionDistance;
		float num12 = _wantedHeight;
		if (num10 == 0)
		{
			float freeDistance = (isDistanceDamping ? currentDistance : distance) + num5;
			num12 = _realTarget.position.y + CameraObstructionResolver.ScaleObstructedHeight(_wantedHeight - _realTarget.position.y, RaycastDistance, freeDistance, obstructedMinHeightFraction);
		}
		targetMainMenu.position = new Vector3(targetMainMenu.position.x, num12, targetMainMenu.position.z);
		goto IL_09f4;
	}

	private void HeightShaking()
	{
		if (Mathf.Abs(TargetHeightAdd - HeightAdd) < 0.02f)
		{
			TargetHeightAdd = UnityEngine.Random.Range(0f - HeightShakeRange, HeightShakeRange);
			_startTimeHeightShake = Time.time;
		}
		float t = (Time.time - _startTimeHeightShake) / HeightShakeSpeed;
		HeightAdd = Mathf.Lerp(HeightAdd, TargetHeightAdd, t);
	}

	private void ShakeByZ()
	{
		Vector3 eulerAngles = myTransform.eulerAngles;
		eulerAngles.z = _lastZ;
		float t = (Time.time - _startTimeZShake) / ShakeZSpeed;
		eulerAngles.z = Mathf.Lerp(eulerAngles.z, TargetZ, t);
		_lastZ = eulerAngles.z;
		if (Mathf.Abs(eulerAngles.z - TargetZ) < 0.1f)
		{
			if (StopShaking)
			{
				ShakeZ = false;
				StopShaking = false;
			}
			if (!StopShaking)
			{
				if (TargetZ == 0f)
				{
					if (eulerAngles.z < 0f)
					{
						TargetZ = UnityEngine.Random.Range(0f, RangeShakeZ);
					}
					else
					{
						TargetZ = UnityEngine.Random.Range(0f, 0f - RangeShakeZ);
					}
				}
				else
				{
					TargetZ = 0f;
				}
				_startTimeZShake = Time.time;
			}
		}
		if (StopShaking)
		{
			TargetZ = 0f;
		}
		myTransform.eulerAngles = eulerAngles;
	}

	public void SetShake(float speed, float range, float time)
	{
		ShakeZSpeed = speed;
		RangeShakeZ = range;
		HeightShakeSpeed = 5f;
		HeightShakeRange = 0.2f;
		_stopShakeDisposable?.Dispose();
		_stopShakeDisposable = Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe((long _) =>
		{
			StopShake();
		}).AddTo(this);
	}

	private void StopShake()
	{
		StopShaking = true;
		ShakeZ = false;
	}

	private void CheckWheelForShake()
	{
	}

	private void dampFieldOfView()
	{
		if ((bool)V.Car)
		{
			Powertrain component = V.Car.GetComponent<Powertrain>();
			float b = 55f + component.SpeedMps / 4f;
			_cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, b, Time.deltaTime * 3f);
		}
	}

	private void ApplySpeedFov()
	{
		if (!IsSpeedFovCam)
		{
			_speedDistanceBonus = 0f;
			return;
		}
		bool num = _speedFovConfig == null || _speedFovConfig.EnableFov;
		bool flag = _speedFovConfig == null || _speedFovConfig.EnablePullback;
		float num2 = (_speedFovConfig ? _speedFovConfig.MaxFovAdd : 25f);
		float num3 = (_speedFovConfig ? _speedFovConfig.StartSpeedKmh : 60f);
		float num4 = (_speedFovConfig ? _speedFovConfig.SpeedAtMaxKmh : 200f);
		float num5 = (_speedFovConfig ? _speedFovConfig.MaxPullback : 1.5f);
		float num6 = (_speedFovConfig ? _speedFovConfig.PullbackLerpSpeed : 3f);
		float num7 = Mathf.Clamp01((Mathf.Abs(dr ? dr.SpeedMps : 0f) * 3.6f - num3) / Mathf.Max(1f, num4 - num3));
		if (num)
		{
			float fieldOfView = Mathf.Min(ZoomFov + num2 * num7, 90f);
			_cam.fieldOfView = fieldOfView;
		}
		_speedDistanceBonus = (flag ? Mathf.Lerp(_speedDistanceBonus, num5 * num7, Time.deltaTime * num6) : 0f);
	}

	private void DampDistance()
	{
		if (!dr)
		{
			V.Car.TryGetComponent<Powertrain>(out dr);
		}
		Acceleration = ((dr.SpeedMps < 3f) ? 0f : (Koff * (dr.SpeedMps - _lastVelo) / (Time.deltaTime + Mathf.Epsilon)));
		_lastVelo = dr.SpeedMps;
		float value = SmoothFollowAdd + Acceleration;
		value = Mathf.Clamp(value, SmoothFollowAdd - 1.5f, SmoothFollowAdd + 3f);
		distance = Mathf.Lerp(distance, value, Time.deltaTime / DistDamp);
	}

	private void Update()
	{
		if (_menuLevelParent.isDroneEnabled || !IsRotating || !FlagsStorage.MoveByTouch || FlagsStorage.IgnoreTouchesOnMenu || FlagsStorage.OutOfCar)
		{
			return;
		}
		if (isbegan && V.MenuIndex != 10)
		{
			if (timer1 < 0.5f)
			{
				timer1 = Time.deltaTime + timer1;
			}
		}
		else if (timer1 > 0f)
		{
			timer1 -= Time.deltaTime;
		}
		if (Input.touchCount == 2)
		{
			Zooming();
			isbegan = false;
			_secondpoint = _firstpoint;
			xAngTemp = _xAngTemp2;
			yAngTemp = _yAngTemp2;
			return;
		}
		if (Input.touchCount == 1)
		{
			if (Input.GetTouch(0).phase == TouchPhase.Began)
			{
				_firstpoint = Input.GetTouch(0).position;
				WidthPercent = _firstpoint.x / (float)Screen.width * 100f;
				xAngTemp = xAngle;
				yAngTemp = yAngle;
				rotationDamping = 2f;
			}
			if (Input.GetTouch(0).phase == TouchPhase.Moved && isbegan)
			{
				_firstpoin2 = _firstpoint;
				_xAngTemp2 = xAngTemp;
				_yAngTemp2 = yAngTemp;
				_secondpoint = Input.GetTouch(0).position;
				xAngle = xAngTemp + (_secondpoint.x - _firstpoint.x) * 180f / (float)Screen.width * 2f;
				yAngle = yAngTemp - (_secondpoint.y - _firstpoint.y) * 90f / (float)Screen.height * 2f;
				if (yAngle >= 180f)
				{
					yAngle = 180f;
				}
				if (yAngle < 2f)
				{
					yAngle = 2f;
				}
			}
		}
		if (Input.touchCount == 0)
		{
			isbegan = false;
		}
	}

	private void FixedUpdate()
	{
		if (dampFixedCamera)
		{
			_oldVelocity = _velocity;
			_velocity = mtarget.InverseTransformDirection(carRigidbody.velocity);
			_deltaVelocity = _velocity - _oldVelocity;
			_accel = _deltaVelocity / Time.deltaTime + _centrifugalAccel * Vector3.right;
			_deltaMovement = _accel * (Time.deltaTime * Time.deltaTime * 5f);
			_deltaMovement.z = Mathf.Clamp(_deltaMovement.z, -0.2f, 0.2f);
			_deltaMovement.y = Mathf.Clamp(_deltaMovement.y, -0.1f, 0.1f);
			_deltaMovement.x = Mathf.Clamp(_deltaMovement.x, -0.01f, 0.01f);
			_deltaPitchAngle = _deltaMovement.y * 20f - _deltaMovement.z * 20f;
			_deltaPitchAngle = Mathf.Clamp(_deltaPitchAngle, -5f, 5f);
		}
	}

	private void Zooming()
	{
		Touch touch = Input.GetTouch(0);
		Touch touch2 = Input.GetTouch(1);
		Vector2 vector = touch.position - touch.deltaPosition;
		Vector2 vector2 = touch2.position - touch2.deltaPosition;
		float magnitude = (vector - vector2).magnitude;
		float magnitude2 = (touch.position - touch2.position).magnitude;
		float num = magnitude - magnitude2;
		distance += num * perspectiveZoomSpeed * 0.1f;
		distanceDamping = 3f;
		float min = 3.5f;
		float max = 8f;
		if (VinylsEditor.HasVinyl)
		{
			min = 0.5f;
			max = 4f;
		}
		distance = Mathf.Clamp(distance, min, max);
	}

	private static float ClampAngle(float yawAngle, float min, float max)
	{
		if (yawAngle < -360f)
		{
			yawAngle += 360f;
		}
		else if (yawAngle > 360f)
		{
			yawAngle -= 360f;
		}
		return Mathf.Clamp(yawAngle, min, max);
	}

	public void NewCarEntered(GameObject car)
	{
		car.TryGetComponent<Rigidbody>(out carRigidbody);
		car.TryGetComponent<Axles>(out axles);
		car.TryGetComponent<Powertrain>(out dr);
	}

	private void ClampPositions()
	{
		Vector3 position = base.transform.position;
		position.x = Mathf.Clamp(position.x, -20000f, 20000f);
		position.z = Mathf.Clamp(position.z, -20000f, 20000f);
		position.y = Mathf.Clamp(position.y, -500f, 500f);
		base.transform.position = position;
		if (base.transform.rotation == new Quaternion(0f, 0f, 0f, 0f))
		{
			base.transform.rotation = Quaternion.identity;
		}
	}

	public void ChangeForceCameraForSecond()
	{
		ForceForwardVelocity = true;
		if ((bool)_realTarget)
		{
			freeOrbitYaw = _realTarget.eulerAngles.y;
		}
		Observable.Timer(TimeSpan.FromSeconds(4.0)).Subscribe((long _) =>
		{
			ForceForwardVelocity = false;
			FlagsStorage.IsInGarageTunnel = false;
		}).AddTo(this);
	}

	private Vector3 GetRigidBodyVelocityForSmoothLookAt()
	{
		if (FlagsStorage.IsSpectatorMode && (bool)_realTarget)
		{
			Vector3 position = _realTarget.position;
			if (_spectatorPrevPosValid && Time.deltaTime > 0f)
			{
				_spectatorVelocity = (position - _spectatorPrevPos) / Time.deltaTime;
			}
			_spectatorPrevPos = position;
			_spectatorPrevPosValid = true;
			return _spectatorVelocity;
		}
		_spectatorPrevPosValid = false;
		if (ForceForwardVelocity && (bool)dr)
		{
			Vector3 vector = carRigidbody.transform.forward * 2f;
			if (dr.Gear != -1)
			{
				return vector;
			}
			return -vector;
		}
		return carRigidbody.velocity;
	}
}
