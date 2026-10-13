using System;
using CPMEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Other;
using _Car_Parking.Scripts.Other.Helpers;

public class SteeringWheel : MonoBehaviour
{
	[SerializeField]
	private SteeringWheelHandPositions _steeringWheelLeftHandPositions;

	[SerializeField]
	private SteeringWheelHandPositions _steeringWheelRightHandPositions;

	[SerializeField]
	private bool _isGyroscopicStabilization;

	[SerializeField]
	private Transform _stabilizedObject;

	[SerializeField]
	private float _stabilizedSmoothSpeed = 20f;

	public bool rotateAroundY;

	public float Steering;

	public bool negative;

	private float _maxSteeringAngle = 90f;

	private float _z;

	private float _yVelocity;

	private Axles _axles;

	private MultiPlayerPosControl _mppc;

	private Powertrain _powertrain;

	private VehicleData _menuVehicleParent;

	private Action _onCarGraphicLoadedHandler;

	public bool HaveHandPositions
	{
		get
		{
			if ((bool)_steeringWheelLeftHandPositions)
			{
				return _steeringWheelRightHandPositions;
			}
			return false;
		}
	}

	public void Set(SteeringWheelHandPositions steeringWheelLeftHandPositions, SteeringWheelHandPositions steeringWheelRightHandPositions)
	{
		_steeringWheelLeftHandPositions = steeringWheelLeftHandPositions;
		_steeringWheelRightHandPositions = steeringWheelRightHandPositions;
	}

	public void Init(VehicleData car)
	{
		if ((bool)car)
		{
			if (car.TryGetComponent<DriverController>(out var component))
			{
				component.SetHandWheelToSteeringWheel(car.Driver);
			}
			car.TryGetComponent<MultiPlayerPosControl>(out _mppc);
			car.TryGetComponent<Axles>(out _axles);
			car.TryGetComponent<Powertrain>(out _powertrain);
			if (FlagsStorage.IsMainMenu && !car.IsCarGraphicLoaded)
			{
				car.OnCarGraphicLoaded += CarDataLoaded;
			}
		}
		void CarDataLoaded()
		{
			car.OnCarGraphicLoaded -= CarDataLoaded;
			SmoothLoadCarInMenu.MeshRendererInChildrenEnabler(base.gameObject, enable: true);
		}
	}

	public void InitHands(HandIKTarget leftHandIkTarget, HandIKTarget rightHandIkTarget)
	{
		if ((bool)leftHandIkTarget)
		{
			_steeringWheelLeftHandPositions.Init(leftHandIkTarget);
		}
		if ((bool)rightHandIkTarget)
		{
			_steeringWheelRightHandPositions.Init(rightHandIkTarget);
		}
	}

	public void TryInitPanelStabilization(Transform stabilizedObject)
	{
		if ((bool)stabilizedObject)
		{
			_isGyroscopicStabilization = true;
			_stabilizedObject = stabilizedObject;
		}
	}

	public void NullMppcRef()
	{
		_mppc = null;
	}

	private void OnMppcDestroyed(Unit _)
	{
		_mppc = null;
	}

	private void Start()
	{
		foreach (Transform item in base.transform)
		{
			item.gameObject.layer = LayerViaName.CarBody;
		}
		if ((bool)_mppc)
		{
			_mppc.OnDestroyAsObservable().Subscribe(OnMppcDestroyed).AddTo(this);
		}
		if (FlagsStorage.IsMainMenu)
		{
			_menuVehicleParent = GetComponentInParent<VehicleData>();
			if ((bool)_menuVehicleParent && !_menuVehicleParent.IsCarGraphicLoaded)
			{
				_onCarGraphicLoadedHandler = OnCarGraphicLoaded;
				_menuVehicleParent.OnCarGraphicLoaded += _onCarGraphicLoadedHandler;
			}
		}
	}

	private void Update()
	{
		if ((bool)_axles && (bool)_powertrain)
		{
			Steering = 0f - _powertrain.GetSteer();
			if (float.IsNaN(Steering) || float.IsInfinity(Steering))
			{
				Steering = 0f;
			}
			if ((bool)_mppc && _powertrain.OnLineCar)
			{
				Steering = Mathf.Lerp(Steering, _mppc.Steering, Time.deltaTime * 2f);
			}
			if (!_mppc && !_powertrain)
			{
				CommonExtensions.NullIfDestroyed(ref _mppc);
			}
			if (negative)
			{
				Steering = 0f - Steering;
			}
			float value = Steering * _maxSteeringAngle;
			value = Mathf.Clamp(value, 0f - _maxSteeringAngle, _maxSteeringAngle);
			if (Math.Abs(value - _z) > 90f)
			{
				_z = value;
			}
			if (float.IsNaN(_z) || float.IsInfinity(_z) || float.IsNaN(_yVelocity) || float.IsInfinity(_yVelocity))
			{
				_z = value;
				_yVelocity = 0f;
			}
			_z = DampWheelAngle(_z, value, ref _yVelocity, Time.deltaTime);
			Steering = Mathf.Clamp(_z / _maxSteeringAngle, -1f, 1f);
			if (float.IsNaN(Steering) || float.IsInfinity(Steering))
			{
				Steering = 0f;
			}
			if (rotateAroundY)
			{
				base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, _z);
			}
			else
			{
				base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, Utils.GetFixedVariable(_z) + 180f);
			}
			if ((bool)_steeringWheelLeftHandPositions)
			{
				_steeringWheelLeftHandPositions.UpdatePosition(Steering);
			}
			if ((bool)_steeringWheelRightHandPositions)
			{
				_steeringWheelRightHandPositions.UpdatePosition(0f - Steering);
			}
			if (_isGyroscopicStabilization && (bool)_stabilizedObject)
			{
				Quaternion b = Quaternion.Euler(base.transform.eulerAngles.x, base.transform.eulerAngles.y, 0f);
				_stabilizedObject.rotation = Quaternion.Lerp(_stabilizedObject.rotation, b, _stabilizedSmoothSpeed * Time.deltaTime);
			}
		}
	}

	private static float DampWheelAngle(float current, float target, ref float velocity, float deltaTime)
	{
		if (deltaTime <= 0f)
		{
			return current;
		}
		return Mathf.SmoothDamp(current, target, ref velocity, 0.1f, float.PositiveInfinity, deltaTime);
	}

	private void OnCarGraphicLoaded()
	{
		if ((bool)_menuVehicleParent && _onCarGraphicLoadedHandler != null)
		{
			_menuVehicleParent.OnCarGraphicLoaded -= _onCarGraphicLoadedHandler;
		}
		_onCarGraphicLoadedHandler = null;
		_menuVehicleParent = null;
		SmoothLoadCarInMenu.MeshRendererInChildrenEnabler(base.gameObject, enable: true);
	}

	private void OnDestroy()
	{
		if ((bool)_menuVehicleParent && _onCarGraphicLoadedHandler != null)
		{
			_menuVehicleParent.OnCarGraphicLoaded -= _onCarGraphicLoadedHandler;
		}
		_onCarGraphicLoadedHandler = null;
		_menuVehicleParent = null;
		_mppc = null;
	}

	private SteeringWheelHandPositions CreateSteeringWheelHandPositions(Vector3 position, Quaternion rotation, string name)
	{
		GameObject obj = new GameObject(name);
		obj.transform.SetParent(base.transform);
		obj.transform.position = position;
		obj.transform.rotation = rotation;
		return obj.AddComponent<SteeringWheelHandPositions>();
	}
}
