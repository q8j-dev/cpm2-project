using System;
using System.Linq;
using Cinemachine;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.UIElement;

namespace Racing.CameraManagement.SpecificTypes
{
	public class GarageMainCameraHandler : VirtualCameraHandler
	{
		public Transform bodyFollow;

		public Transform edgeTracker;

		[SerializeField]
		private CinemachineCollider cinemachineCollider;

		[SerializeField]
		private float initZoomDistance = 3f;

		[SerializeField]
		private float speedXAxis = 1f;

		[SerializeField]
		private float speedYAxis = 0.5f;

		[SerializeField]
		private float speedFollow;

		[SerializeField]
		private float speedRotation = 2f;

		[SerializeField]
		private float speedZ = 1f;

		[SerializeField]
		private float lerpedZ;

		[SerializeField]
		private float visualTuningYOffset = -0.5f;

		[SerializeField]
		private CamControlTest camControlTest;

		private const string CarLayerName = "car";

		private const string PersonLayerName = "DriverPerson";

		private CinemachineOrbitalTransposer _transposer;

		private CameraSurfaceTracker _surfaceTracker;

		private CinemachineComposer _composer;

		private Transform _mainCamTr;

		private bool _movingToNewCar;

		private bool _rotatingByTouch;

		private GameObject car;

		private LayerMask masks;

		private LayerMask masksWithoutCar;

		private Vector3 targetRot;

		private float timeStartTouch;

		private Vector3 eulerAngles;

		private LayerMask cameraMasks;

		public Vector3 edgeTrackerTargetPos;

		public Vector3 offsetForEdge;

		public bool edgeReached;

		private float garageDistanceCollider = 1f;

		private float distanceMag = 1f;

		private Vector3 _rotationOffset;

		private InputItemsProvider _controlItemsProvider;

		private IDisposable _enabler;

		private UITouchHelper _uiTouchHelper;

		private const int EdgeTrackThresholdDistance = 40;

		private const int DefaultPresetIndex = 0;

		private const int SuspensionPresetIndex = 1;

		private const int DynoPresetIndex = 2;

		private const int VisualTuningPresetIndex = 3;

		private const int SellCarPresetIndex = 4;

		private const int SmokeCarPresetIndex = 5;

		private const int FreeWalkIndex = 6;

		private const float FullAngle = 360f;

		private const float MinClamp = 0f;

		private const float MaxClamp = 70f;

		private CarCamerasParametersConfig _chaseConfig;

		private AlterChaseCameraConfig _chaseFramingConfig;

		public Collider collider;

		private bool firstSellCarOpen = true;

		private float _currentMinClamp { get; set; }

		private float _currentMaxClamp { get; set; }

		public override bool ChangeTargetOnActivate => false;

		public bool LockCamera { get; set; }

		public GameObject GetCar => car;

		protected override float DebugTrackedYaw => targetRot.y;

		protected override bool DebugIsPitchAtBottom => targetRot.x >= _currentMaxClamp - 0.1f;

		protected override Vector2 _multiplier => new Vector2(8.96f, 4.48f);

		protected override CameraSettingsContext SettingsContext => CameraSettingsContext.Garage;

		protected override float GamepadLookScale => 0.28f;

		[Inject]
		public void Construct(UITouchHelper uiTouchHelper, InputItemsProvider controlItemsProvider)
		{
			_controlItemsProvider = controlItemsProvider;
			_uiTouchHelper = uiTouchHelper;
		}

		protected override void Awake()
		{
			base.Awake();
			cameraMasks = LayerMask.GetMask("car") | LayerMask.GetMask("DriverPerson");
			_surfaceTracker = new CameraSurfaceTracker(cameraMasks);
			_transposer = _camera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
			_composer = _camera.GetCinemachineComponent<CinemachineComposer>();
			masksWithoutCar = cinemachineCollider.m_CollideAgainst;
			masks = cinemachineCollider.m_CollideAgainst;
			targetRot = bodyFollow.transform.eulerAngles;
			eulerAngles = bodyFollow.transform.eulerAngles;
			_zoomDistance = initZoomDistance;
			if ((bool)camControlTest)
			{
				camControlTest.a.SetValueWithoutNotify(speedXAxis);
				camControlTest.b.SetValueWithoutNotify(speedYAxis);
				camControlTest.c.SetValueWithoutNotify(speedFollow * 0.01f);
				camControlTest.d.SetValueWithoutNotify(speedRotation * 0.01f);
				camControlTest.e.SetValueWithoutNotify(speedZ * 0.01f);
			}
			_rotationOffset = new Vector3(8f, -111f, 0f);
			if ((bool)_chaseConfig && (bool)_chaseFramingConfig)
			{
				_chaseConfig.GetChaseParams(1, isTrack: false, isTall: false, out var distance, out var _, out var pitch);
				_rotationOffset.x = _chaseFramingConfig.TiltDeg(distance, pitch) - _chaseFramingConfig.GarageTiltUpDeg;
			}
			if ((bool)_camera && (bool)_chaseFramingConfig)
			{
				_camera.m_Lens.FieldOfView = _chaseFramingConfig.BaseFov;
			}
		}

		[Inject]
		public void ConstructAlterChaseConfig(CarCamerasParametersConfig chaseConfig, AlterChaseCameraConfig chaseFramingConfig)
		{
			_chaseConfig = chaseConfig;
			_chaseFramingConfig = chaseFramingConfig;
		}

		private void ApplyChaseDefaultZoomAlter()
		{
			if ((bool)_chaseConfig && (bool)_chaseFramingConfig && (bool)car && (bool)edgeTracker && !FlagsStorage.OnVisualTuning && !car.GetComponent<SellCarValues>())
			{
				Transform transform = AlterChaseCameraConfig.FindAnchor(car.transform);
				if ((bool)transform)
				{
					Vector3 vector = transform.position - edgeTracker.position;
					vector.y = 0f;
					float magnitude = vector.magnitude;
					_zoomDistance = _chaseFramingConfig.MiddleDistance + _chaseFramingConfig.GarageDistanceAdd - garageDistanceCollider - magnitude;
					_zoomDistance = Mathf.Clamp(_zoomDistance, MinZoomDistance, MaxZoomDistance);
				}
			}
		}

		[ContextMenu("Debug/Rotate Full Circle")]
		private void DebugRotateFullCircleMenu()
		{
			DebugRotateFullCircle();
		}

		[ContextMenu("Debug/Rotate Top To Bottom")]
		private void DebugRotateTopToBottomMenu()
		{
			DebugRotateTopToBottom();
		}

		protected override void DebugMoveToPitchTop()
		{
			SetClampX();
			targetRot.x = _currentMinClamp;
			eulerAngles.x = _currentMinClamp;
			if ((bool)bodyFollow)
			{
				Vector3 vector = bodyFollow.eulerAngles;
				vector.x = _currentMinClamp;
				bodyFollow.eulerAngles = vector;
			}
		}

		protected override void ApplyLook(Vector2 look)
		{
			SetClampX();
			targetRot.y += look.x;
			targetRot.x -= look.y;
			targetRot.x = Mathf.Clamp(targetRot.x, _currentMinClamp, _currentMaxClamp);
			targetRot.z = 0f;
		}

		private void OnEnable()
		{
			_onLookHandle.Get(_eventRegistry).Subscribe(ToggleRotation);
		}

		private void OnDisable()
		{
			_onLookHandle.Get(_eventRegistry)?.Unsubscribe(ToggleRotation);
		}

		private void ToggleRotation(bool isStarted)
		{
			if (isStarted)
			{
				_rotatingByTouch = true;
				timeStartTouch = Time.time;
			}
			else
			{
				_rotatingByTouch = false;
			}
		}

		private void Start()
		{
			_mainCamTr = _cameraManager.CurrentBrain.transform;
		}

		public void ResetCamToDefault()
		{
			CameraOverridePreset cameraOverridePreset = _cameraManager.AllCameraPresets.First((CameraOverridePreset p) => p.Index == 0);
			if (!(cameraOverridePreset == null))
			{
				cameraOverridePreset.ApplyCineColliderSettings(cinemachineCollider);
				SetPosRotFromPreset(cameraOverridePreset);
			}
		}

		public void ResetCameraForSuspension()
		{
			CameraOverridePreset cameraOverridePreset = _cameraManager.AllCameraPresets.First((CameraOverridePreset p) => p.Index == 1);
			_zoomDistance = initZoomDistance;
			if (cameraOverridePreset == null)
			{
				Transform obj = bodyFollow.transform;
				Vector3 position = (edgeTracker.transform.position = (edgeTrackerTargetPos = new Vector3(6.5f, 0.8f, 26f)));
				obj.position = position;
				bodyFollow.transform.eulerAngles = (targetRot = (eulerAngles = new Vector3(0f, 150f, 0f)));
			}
			else
			{
				SetPosRotFromPreset(cameraOverridePreset);
			}
		}

		public void ResetCameraForFree()
		{
			_zoomDistance = initZoomDistance;
			CameraOverridePreset cameraOverridePreset = _cameraManager.AllCameraPresets.First((CameraOverridePreset p) => p.Index == 6);
			if ((bool)cameraOverridePreset)
			{
				SetPosRotFromPreset(cameraOverridePreset);
			}
		}

		public void ResetCamForDyno()
		{
			_zoomDistance = initZoomDistance;
			CameraOverridePreset cameraOverridePreset = _cameraManager.AllCameraPresets.First((CameraOverridePreset p) => p.Index == 2);
			if (!(cameraOverridePreset == null))
			{
				SetPosRotFromPreset(cameraOverridePreset);
				cameraOverridePreset.ApplyCineColliderSettings(cinemachineCollider);
			}
		}

		public void ResetCameraForVisualTuning()
		{
			_zoomDistance = initZoomDistance;
			CameraOverridePreset cameraOverridePreset = _cameraManager.AllCameraPresets.First((CameraOverridePreset p) => p.Index == 3);
			if (!(cameraOverridePreset == null))
			{
				SetPosRotFromPreset(cameraOverridePreset);
				cameraOverridePreset.ApplyCineColliderSettings(cinemachineCollider);
			}
		}

		public void ResetCameraForSellCar()
		{
			_zoomDistance = initZoomDistance;
			CameraOverridePreset cameraOverridePreset = _cameraManager.AllCameraPresets.First((CameraOverridePreset p) => p.Index == 4);
			if (!(cameraOverridePreset == null))
			{
				_zoomDistance = cameraOverridePreset.ZoomDistance;
				SetPosRotFromPreset(cameraOverridePreset);
				cameraOverridePreset.ApplyCineColliderSettings(cinemachineCollider);
			}
		}

		private void SetPosRotFromPreset(CameraOverridePreset preset)
		{
			if (!(collider == null) && !(bodyFollow == null) && !(edgeTracker == null))
			{
				Bounds bounds = collider.bounds;
				Vector3 localOffset = preset.GetLocalOffset(bounds);
				Vector3 vector = collider.transform.TransformPoint(localOffset);
				if ((bool)bodyFollow && (bool)edgeTracker)
				{
					Transform obj = bodyFollow;
					Vector3 position = (edgeTracker.position = (edgeTrackerTargetPos = vector));
					obj.position = position;
				}
				Quaternion quaternion = Quaternion.LookRotation(bounds.center - vector);
				if ((bool)bodyFollow)
				{
					bodyFollow.eulerAngles = (targetRot = (eulerAngles = quaternion.eulerAngles));
				}
			}
		}

		public void ResetCameraSmokeColor()
		{
			CameraOverridePreset cameraOverridePreset = _cameraManager.AllCameraPresets.First((CameraOverridePreset p) => p.Index == 5);
			if ((bool)cameraOverridePreset)
			{
				_zoomDistance = cameraOverridePreset.ZoomDistance;
				SetPosRotFromPreset(cameraOverridePreset);
			}
		}

		public void UpdateBodyFollowExternal()
		{
			targetRot = bodyFollow.transform.eulerAngles;
			eulerAngles = bodyFollow.eulerAngles;
		}

		public override void SetActive(bool isActive, Transform target, Action<CinemachineVirtualCameraBase> onLiveCallback = null)
		{
			_ = _isActive;
			base.SetActive(isActive, target, onLiveCallback);
			_enabler?.Dispose();
			if (isActive)
			{
				cinemachineCollider.enabled = true;
				_rotatingByTouch = true;
				if ((bool)target && target.gameObject != car)
				{
					OnCarChanged(target.gameObject, car);
				}
			}
		}

		protected override void LateUpdate()
		{
			if (_isActive)
			{
				MoveFollowBody();
				MoveByTouchAlter();
				UpdateZoom();
			}
		}

		private void MoveFollowBody()
		{
			_composer.m_TrackedObjectOffset.y = (FlagsStorage.OnVisualTuning ? visualTuningYOffset : 0f);
			if (!bodyFollow || !collider)
			{
				return;
			}
			_ = car != V.LastUsedCar;
			if ((_rotatingByTouch || !edgeReached) && _surfaceTracker.TryGetSurfacePoint(_mainCamTr.position, collider, out var surfacePoint))
			{
				edgeTrackerTargetPos = surfacePoint;
			}
			float num = Vector3.Distance(edgeTracker.transform.position, edgeTrackerTargetPos);
			if (num > 40f && _rotatingByTouch)
			{
				edgeTracker.transform.position = edgeTrackerTargetPos;
			}
			float b = _zoomDistance + garageDistanceCollider;
			if (_movingToNewCar)
			{
				lerpedZ = Mathf.Lerp(lerpedZ, b, Time.deltaTime);
			}
			else
			{
				lerpedZ = Mathf.Lerp(lerpedZ, b, Time.deltaTime * speedZ);
			}
			_transposer.m_FollowOffset.z = lerpedZ;
			if (edgeReached)
			{
				edgeReached = num < 0.5f;
			}
			else
			{
				float num2 = Mathf.Clamp(num, 0.5f, 3f);
				if (!_movingToNewCar)
				{
					num2 *= speedFollow;
				}
				edgeTracker.transform.position = Vector3.Lerp(edgeTracker.transform.position, edgeTrackerTargetPos + offsetForEdge, Time.deltaTime * num2);
				garageDistanceCollider = Vector3.Distance(edgeTracker.transform.position, collider.transform.position) * distanceMag;
				if (num < 0.2f)
				{
					edgeReached = true;
					if (_movingToNewCar)
					{
						OnNewCarReached();
					}
				}
			}
			bodyFollow.transform.position = edgeTracker.transform.position;
		}

		public void OnCarChanged(GameObject newCar, GameObject oldCar)
		{
			edgeReached = false;
			_movingToNewCar = true;
			SellCarValues component = newCar.gameObject.GetComponent<SellCarValues>();
			VehicleData component2 = newCar.GetComponent<VehicleData>();
			if (!component2 && (bool)V.Car)
			{
				component2 = V.Car.GetComponent<VehicleData>();
			}
			if ((bool)component)
			{
				collider = component.Collider;
				if (!firstSellCarOpen)
				{
					targetRot = _rotationOffset;
					eulerAngles = targetRot;
					bodyFollow.transform.eulerAngles = eulerAngles;
				}
				firstSellCarOpen = true;
				_composer.m_ScreenY = 0.35f;
			}
			else if ((bool)component2 || (bool)V.DriverPerson)
			{
				bool num = !oldCar;
				offsetForEdge = Vector3.zero;
				_composer.m_ScreenY = 0.5f;
				collider = (component2 ? component2.Collider : V.DriverPerson.GetComponent<Collider>());
				if (num)
				{
					targetRot = Quaternion.LookRotation(V.DriverPerson ? V.DriverPerson.transform.forward : newCar.transform.forward).eulerAngles + _rotationOffset;
					eulerAngles = targetRot;
					bodyFollow.transform.eulerAngles = eulerAngles;
				}
				firstSellCarOpen = false;
			}
			if (oldCar == null)
			{
				_rotatingByTouch = true;
			}
			else
			{
				Vector3 vector = newCar.transform.position - oldCar.transform.position;
				edgeTrackerTargetPos += vector;
			}
			cinemachineCollider.m_CollideAgainst = masksWithoutCar;
			car = newCar;
		}

		private void OnNewCarReached()
		{
			_movingToNewCar = false;
			cinemachineCollider.enabled = true;
			cinemachineCollider.m_CollideAgainst = masks;
			ApplyChaseDefaultZoomAlter();
		}

		private void MoveByTouchAlter()
		{
			if (!UITouched() && !LockCamera && (bool)bodyFollow && (bool)collider)
			{
				if (_rotatingByTouch)
				{
					UpdateLook();
				}
				float num = Vector3.Distance(bodyFollow.transform.position, edgeTrackerTargetPos);
				float num2 = (3f + num) * speedRotation;
				eulerAngles = new Vector3(Mathf.Lerp(eulerAngles.x, targetRot.x, Time.deltaTime * num2), Mathf.Lerp(eulerAngles.y, targetRot.y, Time.deltaTime * num2), Mathf.Lerp(eulerAngles.z, targetRot.z, Time.deltaTime * num2));
				bodyFollow.eulerAngles = eulerAngles;
			}
		}

		private void MoveBodyByTouch()
		{
			if (UITouched() || LockCamera || !bodyFollow || !collider)
			{
				return;
			}
			Transform transform = bodyFollow;
			if (!TempCameraRotationDisabler.IsTouching || _uiTouchHelper.HaveTouch)
			{
				return;
			}
			if (Input.touchCount == 1)
			{
				if (Input.touches[0].phase == TouchPhase.Moved)
				{
					_rotatingByTouch = true;
				}
				if (_rotatingByTouch)
				{
					timeStartTouch = Time.time;
					Vector2 deltaPosition = Input.GetTouch(0).deltaPosition;
					if (deltaPosition.sqrMagnitude > 1f)
					{
						SetClampX();
						targetRot.y += speedXAxis * deltaPosition.x;
						targetRot.x -= speedYAxis * deltaPosition.y;
						targetRot.x = Mathf.Clamp(targetRot.x, _currentMinClamp, _currentMaxClamp);
						targetRot.z = 0f;
					}
				}
			}
			if (_rotatingByTouch && Time.time - timeStartTouch > 3f)
			{
				_rotatingByTouch = false;
			}
			float num = Vector3.Distance(bodyFollow.transform.position, edgeTrackerTargetPos);
			float num2 = (3f + num) * speedRotation;
			eulerAngles = new Vector3(Mathf.Lerp(eulerAngles.x, targetRot.x, Time.deltaTime * num2), Mathf.Lerp(eulerAngles.y, targetRot.y, Time.deltaTime * num2), Mathf.Lerp(eulerAngles.z, targetRot.z, Time.deltaTime * num2));
			transform.eulerAngles = eulerAngles;
		}

		private void SetClampX()
		{
			_currentMinClamp = ((targetRot.x >= 350f) ? 360f : 0f);
			_currentMaxClamp = ((targetRot.x >= 350f) ? 430f : 70f);
		}

		private bool UITouched()
		{
			ThrottleSliderControl throttleSliderControl = _controlItemsProvider.ThrottleSliderControl;
			BrakeSliderControl brakeSliderControl = _controlItemsProvider.BrakeSliderControl;
			if (!throttleSliderControl || !brakeSliderControl)
			{
				return false;
			}
			if (!brakeSliderControl.IsBrakePressed.Value)
			{
				return throttleSliderControl.IsThrottlePressed.Value;
			}
			return true;
		}
	}
}
