using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace CPMEngine
{
	public class WheelProvider : MonoBehaviour
	{
		private const int CountFramesToApplyRotation = 3;

		public bool DualTyres;

		public bool OpenWheels;

		public WheelIndex WheelIndex;

		[NonSerialized]
		public Surface WheelSurface;

		[NonSerialized]
		public TyreType TyreType;

		public Vector3 TopPos;

		private WheelProviderEventDelegate _onPreSimulationInvoker;

		private WheelProviderEventDelegate _onPostSimulationInvoker;

		private WheelProviderEventDelegate _onUpdateWheelsInvoker;

		private WheelProviderEventDelegate _onApplyDynoDataInvoker;

		[SerializeField]
		[FormerlySerializedAs("Temperature")]
		private float _temperature;

		private float _gripTemperature;

		private float _surfaceTemperature;

		public float DirtAmount;

		public float AngularMultiplayerVelocity;

		public ParticleSystem SkidSmokeParticle;

		public Color SkidSmokeColor = Color.white;

		private WheelModel _wheelModel;

		private Transform _cachedTransform;

		private bool _cachedOnGroundDown;

		private bool _cachedHasSmoke;

		private Vector3 _cachedPatchCenter;

		private Vector3 _cachedPatchNormal;

		private Vector3 _cachedPatchLeft;

		private Vector3 _cachedPatchRight;

		private Vector3 _cachedMarkCenter;

		private Vector3 _cachedMarkNormal;

		private Vector3 _cachedMarkLeft;

		private Vector3 _cachedMarkRight;

		private float _tyreVisualLateralOffset;

		private float2 _tyreParams = new float2(0.225f, 45f);

		private float2 _rimParams;

		private VehicleData _vehicleData;

		private SphereCollider _sphereCollider;

		private GameObject _model;

		private WheelSound _ws;

		private WheelEffects _we;

		private TCurveTyre _tempCurveTyre;

		private TemperatureTyresInfo _temperatureTyresInfo;

		private Vector3 _prevPosModel;

		private const float BaseCooling = 80f;

		private const float AirflowCooling = 0.8f;

		private const float RollingHeatCoeff = 0.08f;

		private const float SlipHeatSaturation = 12000f;

		private const float HeatCapacityRubber = 900f;

		private bool _deformPlanePushed;

		private float _rotSampleElapsed;

		private bool _rotSampleSeeded;

		private float _optimalTempLo = 70f;

		private float _optimalTempHi = 100f;

		public bool IsSkidmarkOnline
		{
			get
			{
				return Wheel.SkidOnline;
			}
			set
			{
				Wheel.SkidOnline = value;
			}
		}

		public bool IsLeft { get; private set; }

		public float Temperature
		{
			get
			{
				return _temperature;
			}
			set
			{
				_temperature = value;
				_gripTemperature = value;
				_surfaceTemperature = value;
			}
		}

		public float GripTemperature => _gripTemperature;

		public float SurfaceTemperature => _surfaceTemperature;

		public Transform ModelTransform
		{
			get
			{
				if (!Model)
				{
					return null;
				}
				return Model.transform;
			}
		}

		private Powertrain Chassis { get; set; }

		public PhysicMaterial CurrentPhysMat => Wheel?.CurrentPhysMat;

		public float SmokeTimeMultiplayerCar
		{
			get
			{
				if (!(_we != null))
				{
					return 0f;
				}
				return _we.SmokeTimeMultiplayerCar;
			}
		}

		public GameObject Model
		{
			get
			{
				return _model;
			}
			set
			{
				_model = value;
				if (_model.TryGetComponent<WheelModel>(out var component))
				{
					WheelModel = component;
					if (!WheelModel.Rim)
					{
						WheelModel.Rim = Model.GetComponentInChildren<MeshRenderer>().gameObject;
					}
					if (DualTyres)
					{
						WheelModel.DualTyre = true;
					}
					WheelModel.UseTyreDeformation(TyreDeformationAllowed);
					WheelModel.SetOpenWheelsEnabled(OpenWheels);
				}
			}
		}

		public Transform RimColliderTransform
		{
			get
			{
				if (!WheelModel)
				{
					return null;
				}
				return WheelModel.RimColliderTransform;
			}
		}

		public Collider RimCollider
		{
			get
			{
				if (!WheelModel)
				{
					return null;
				}
				return WheelModel.RimCollider;
			}
		}

		public WheelModel WheelModel
		{
			get
			{
				if (!_wheelModel)
				{
					_wheelModel = GetComponentInChildren<WheelModel>(includeInactive: true);
				}
				return _wheelModel;
			}
			set
			{
				_wheelModel = value;
			}
		}

		public Transform GetTransform
		{
			get
			{
				if (!_cachedTransform)
				{
					_cachedTransform = base.transform;
				}
				return _cachedTransform;
			}
		}

		public bool OnGroundDown => _cachedOnGroundDown;

		public bool HasSmoke => _cachedHasSmoke;

		public bool IsFront
		{
			get
			{
				WheelIndex wheelIndex = WheelIndex;
				return wheelIndex == WheelIndex.FrontLeft || wheelIndex == WheelIndex.FrontRight;
			}
		}

		public IWheel Wheel { get; private set; }

		public float AngularWheelSpeedByTransform { get; set; }

		public static bool TyreDeformationAllowed
		{
			get
			{
				if ((bool)CPMGraphicsSettingsManager.Instance)
				{
					return CPMGraphicsSettingsManager.Instance.UseTyreDeformation;
				}
				return false;
			}
		}

		public bool RotateModelByTransform { get; set; }

		public float GetRimSize => _rimParams.x;

		public float TemperatureGauge01
		{
			get
			{
				if (Temperature <= _optimalTempLo)
				{
					return 0.45f * Mathf.InverseLerp(25f, _optimalTempLo, Temperature);
				}
				if (Temperature <= _optimalTempHi)
				{
					return Mathf.Lerp(0.45f, 0.55f, Mathf.InverseLerp(_optimalTempLo, _optimalTempHi, Temperature));
				}
				return Mathf.Lerp(0.55f, 1f, Mathf.InverseLerp(_optimalTempHi, 250f, Temperature));
			}
		}

		public event WheelProviderEventDelegate OnApplyDynoDataEvent
		{
			add
			{
				_onApplyDynoDataInvoker = (WheelProviderEventDelegate)Delegate.Combine(_onApplyDynoDataInvoker, value);
			}
			remove
			{
				_onApplyDynoDataInvoker = (WheelProviderEventDelegate)Delegate.Remove(_onApplyDynoDataInvoker, value);
			}
		}

		public event WheelProviderEventDelegate OnPostSimulationEvent
		{
			add
			{
				_onPostSimulationInvoker = (WheelProviderEventDelegate)Delegate.Combine(_onPostSimulationInvoker, value);
			}
			remove
			{
				_onPostSimulationInvoker = (WheelProviderEventDelegate)Delegate.Remove(_onPostSimulationInvoker, value);
			}
		}

		public event WheelProviderEventDelegate OnPreSimulationEvent
		{
			add
			{
				_onPreSimulationInvoker = (WheelProviderEventDelegate)Delegate.Combine(_onPreSimulationInvoker, value);
			}
			remove
			{
				_onPreSimulationInvoker = (WheelProviderEventDelegate)Delegate.Remove(_onPreSimulationInvoker, value);
			}
		}

		public event WheelProviderEventDelegate OnUpdateWheelEvent
		{
			add
			{
				_onUpdateWheelsInvoker = (WheelProviderEventDelegate)Delegate.Combine(_onUpdateWheelsInvoker, value);
			}
			remove
			{
				_onUpdateWheelsInvoker = (WheelProviderEventDelegate)Delegate.Remove(_onUpdateWheelsInvoker, value);
			}
		}

		private void CacheTyreVisualLateralOffset()
		{
			_tyreVisualLateralOffset = 0f;
			WheelModel wheelModel = WheelModel;
			if ((bool)wheelModel)
			{
				Vector3 vector = wheelModel.transform.TransformPoint(new Vector3(wheelModel.OuterTyreCenterLocalX(), 0f, 0f));
				_tyreVisualLateralOffset = Vector3.Dot(vector - base.transform.position, base.transform.right);
			}
		}

		[Inject]
		public void Construct(TemperatureTyresInfo temperatureTyresInfo)
		{
			_temperatureTyresInfo = temperatureTyresInfo;
		}

		private float CalculateWheelRadius()
		{
			float num = _tyreParams.x * (_tyreParams.y / 100f);
			return _rimParams.x + num;
		}

		private void Awake()
		{
			WheelIndex wheelIndex = WheelIndex;
			if (wheelIndex == WheelIndex.FrontLeft || wheelIndex == WheelIndex.RearLeft || wheelIndex == WheelIndex.Rear1Left || wheelIndex == WheelIndex.Rear2Left)
			{
				IsLeft = true;
			}
			_gripTemperature = _temperature;
			_surfaceTemperature = _temperature;
		}

		public void InstantiateSkidmarks(Skidmarks s, ParticleSystem smoke)
		{
			if ((bool)s)
			{
				if ((bool)_we)
				{
					_we.SetSkidmarks(s);
				}
				SkidSmokeParticle = smoke;
			}
		}

		public void UpdateWheelVisual()
		{
			if ((bool)WheelModel)
			{
				WheelModel.SetSize(CalculateWheelRadius(), _tyreParams.x, _rimParams.x, _rimParams.y, isOnline: false);
				CacheTyreVisualLateralOffset();
			}
			if ((bool)_we)
			{
				_we.SetWheel(CalculateWheelRadius(), _tyreParams.x);
			}
			if ((bool)_sphereCollider)
			{
				_sphereCollider.radius = CalculateWheelRadius();
			}
		}

		public void UpdateTrailerWheelVisual()
		{
			if ((bool)WheelModel)
			{
				WheelModel.SetSize(CalculateWheelRadius(), _tyreParams.x, _rimParams.x, _rimParams.y, isOnline: false);
				CacheTyreVisualLateralOffset();
			}
			if ((bool)_we)
			{
				_we.SetWheel(CalculateWheelRadius(), _tyreParams.x);
			}
		}

		public bool GetMarkPatch(out Vector3 left, out Vector3 right, out Vector3 center, out Vector3 normal)
		{
			left = _cachedMarkLeft;
			right = _cachedMarkRight;
			center = _cachedMarkCenter;
			normal = _cachedMarkNormal;
			return _cachedOnGroundDown;
		}

		public bool GetPatch(out Vector3 left, out Vector3 right, out Vector3 center, out Vector3 normal)
		{
			left = _cachedPatchLeft;
			right = _cachedPatchRight;
			center = _cachedPatchCenter;
			normal = _cachedPatchNormal;
			return _cachedOnGroundDown;
		}

		public void Init(Powertrain powertrain, WheelIndex index, VehicleData vehicleData)
		{
			if ((bool)Chassis)
			{
				Chassis.OnPreSimulationEvent -= OnPreSimulationHandle;
				Chassis.OnPostSimulationEvent -= OnPostSimulationHandle;
				Chassis.OnUpdateWheelsEvent -= OnUpdateWheelsHandle;
				Chassis.OnApplyDynoDataEvent -= OnApplyDynoDataHandle;
				Wheel = null;
			}
			Chassis = powertrain;
			_vehicleData = vehicleData;
			WheelIndex = index;
			if ((bool)Chassis)
			{
				Chassis.OnPreSimulationEvent += OnPreSimulationHandle;
				Chassis.OnPostSimulationEvent += OnPostSimulationHandle;
				Chassis.OnUpdateWheelsEvent += OnUpdateWheelsHandle;
				SubscribeQualityChanges();
				Chassis.OnApplyDynoDataEvent += OnApplyDynoDataHandle;
				Wheel = Chassis.GetWheel(WheelIndex);
				Wheel.NeedsOpenWheelCollider = OpenWheels;
				if (OpenWheels && (bool)WheelModel)
				{
					WheelModel.SetOpenWheelsEnabled(openWheelsEnabled: true);
				}
			}
			if (!TryGetComponent<WheelSound>(out _ws))
			{
				_ws = base.gameObject.AddComponent<WheelSound>();
			}
			_ws.Init(Chassis, _vehicleData);
			if (TryGetComponent<SphereCollider>(out _sphereCollider))
			{
				_sphereCollider.isTrigger = true;
			}
			if (Chassis.TrafficMode)
			{
				if ((bool)Model)
				{
					CacheTyreVisualLateralOffset();
				}
				return;
			}
			if (!TryGetComponent<WheelEffects>(out _we))
			{
				_we = base.gameObject.AddComponent<WheelEffects>();
			}
			_we.Init(this, Chassis);
			if ((bool)Model)
			{
				CacheTyreVisualLateralOffset();
			}
		}

		private void OnDestroy()
		{
			if ((bool)Chassis)
			{
				Chassis.OnPreSimulationEvent -= OnPreSimulationHandle;
				Chassis.OnPostSimulationEvent -= OnPostSimulationHandle;
				Chassis.OnUpdateWheelsEvent -= OnUpdateWheelsHandle;
				Chassis.OnApplyDynoDataEvent -= OnApplyDynoDataHandle;
			}
			if ((bool)CPMGraphicsSettingsManager.Instance)
			{
				CPMGraphicsSettingsManager.Instance.OnQualityChanged -= OnGraphicsQualityChanged;
				CPMGraphicsSettingsManager.Instance.OnTyreDeformationChanged -= OnTyreDeformationChanged;
			}
			_onPreSimulationInvoker = null;
			_onPostSimulationInvoker = null;
			_onUpdateWheelsInvoker = null;
			_onApplyDynoDataInvoker = null;
		}

		private void SubscribeQualityChanges()
		{
			if ((bool)CPMGraphicsSettingsManager.Instance)
			{
				CPMGraphicsSettingsManager.Instance.OnQualityChanged -= OnGraphicsQualityChanged;
				CPMGraphicsSettingsManager.Instance.OnQualityChanged += OnGraphicsQualityChanged;
				CPMGraphicsSettingsManager.Instance.OnTyreDeformationChanged -= OnTyreDeformationChanged;
				CPMGraphicsSettingsManager.Instance.OnTyreDeformationChanged += OnTyreDeformationChanged;
			}
		}

		private void OnGraphicsQualityChanged(int quality)
		{
			RefreshTyreDeformation();
		}

		private void OnTyreDeformationChanged(bool enabled)
		{
			RefreshTyreDeformation();
		}

		private void RefreshTyreDeformation()
		{
			if ((bool)WheelModel)
			{
				bool tyreDeformationAllowed = TyreDeformationAllowed;
				if (!tyreDeformationAllowed)
				{
					_deformPlanePushed = false;
					WheelModel.UpdateTyreContactPlane(Vector3.zero, Vector3.up, Vector3.zero, Vector3.zero, isTouching: false);
				}
				WheelModel.UseTyreDeformation(tyreDeformationAllowed);
				if (tyreDeformationAllowed && (bool)Chassis && !Chassis.OnLineCar && Wheel != null)
				{
					PushContactPlane();
				}
			}
		}

		private void OnApplyDynoDataHandle(Powertrain powertrain)
		{
			_onApplyDynoDataInvoker?.Invoke(this);
		}

		private void OnPostSimulationHandle(Powertrain powertrain)
		{
			if (Chassis.OnLineCar)
			{
				return;
			}
			_cachedHasSmoke = Wheel.GetSmoke();
			_cachedOnGroundDown = Wheel.OnGroundDown;
			if (_cachedOnGroundDown && Wheel != null)
			{
				TopPos = Wheel.TopPos;
				Wheel.GetPatch(out _cachedPatchLeft, out _cachedPatchRight, out _cachedPatchCenter, out _cachedPatchNormal);
				Wheel.GetMarkPatch(out _cachedMarkLeft, out _cachedMarkRight, out _cachedMarkCenter, out _cachedMarkNormal);
				Vector3 vector = base.transform.right - _cachedMarkNormal * Vector3.Dot(_cachedMarkNormal, base.transform.right);
				if (vector.sqrMagnitude > 1E-06f)
				{
					vector.Normalize();
					float num = Vector3.Dot(_cachedMarkCenter - base.transform.position, base.transform.right);
					Vector3 vector2 = vector * (_tyreVisualLateralOffset - num);
					_cachedMarkLeft += vector2;
					_cachedMarkRight += vector2;
					_cachedMarkCenter += vector2;
				}
			}
			_onPostSimulationInvoker?.Invoke(this);
		}

		private void OnPreSimulationHandle(Powertrain powertrain)
		{
			_onPreSimulationInvoker?.Invoke(this);
		}

		public void SetTyreHealth(float health, TyreType tyreType)
		{
			float defaultValue = (_temperatureTyresInfo ? _temperatureTyresInfo.GetTreadThickness(tyreType) : 0.01f);
			Wheel?.SetTyreHealth(health, defaultValue);
		}

		private void OnUpdateWheelsHandle(Powertrain powertrain)
		{
			RotateModelByTransformMovement(Time.deltaTime);
			CacheTransform(powertrain);
			UpdateTyre();
			CalcWheelMovement();
			UpdateWarmUp();
			_onUpdateWheelsInvoker?.Invoke(this);
		}

		private void UpdateTyre()
		{
			if (Chassis.OnLineCar || !WheelModel || !Wheel.Enabled)
			{
				return;
			}
			WheelModel.UpdateTyreDynamic(Wheel.TyreHealth, DirtAmount);
			if (!TyreDeformationAllowed)
			{
				if (_deformPlanePushed)
				{
					_deformPlanePushed = false;
					WheelModel.UpdateTyreContactPlane(Vector3.zero, Vector3.up, Vector3.zero, Vector3.zero, isTouching: false);
				}
			}
			else
			{
				PushContactPlane();
			}
		}

		private void PushContactPlane()
		{
			_deformPlanePushed = true;
			bool isTouching = Wheel.GetPatch(out var _, out var right, out var cnt, out var n) && !Wheel.DiggerSurface;
			Wheel.GetDeform(out right, out var latDeformVec, out var longDeformVec, out var _);
			WheelModel.UpdateTyreContactPlane(cnt, n, latDeformVec, longDeformVec, isTouching);
		}

		private void CacheTransform(Powertrain powertrain)
		{
			WheelSurface = CurrentPhysMat.SurfaceType;
			if (Chassis.OnLineCar)
			{
				return;
			}
			if (_cachedOnGroundDown)
			{
				NanResetIfBad(ref _cachedMarkCenter);
				NanResetIfBad(ref _cachedMarkLeft);
				NanResetIfBad(ref _cachedMarkRight);
				NanResetIfBad(ref _cachedPatchCenter);
				NanResetIfBad(ref _cachedPatchLeft);
				NanResetIfBad(ref _cachedPatchRight);
				Vector3 pointVelocity = powertrain.Rigidbody.GetPointVelocity(_cachedMarkCenter);
				pointVelocity -= _cachedMarkNormal * Vector3.Dot(pointVelocity, _cachedMarkNormal);
				Vector3 vector = pointVelocity * (Time.time - Time.fixedTime);
				if (float.IsFinite(vector.x) && float.IsFinite(vector.y) && float.IsFinite(vector.z))
				{
					_cachedMarkCenter += vector;
					_cachedMarkLeft += vector;
					_cachedMarkRight += vector;
					_cachedPatchCenter += vector;
					_cachedPatchLeft += vector;
					_cachedPatchRight += vector;
				}
			}
			IsSkidmarkOnline = _cachedHasSmoke;
		}

		private static void NanResetIfBad(ref Vector3 v)
		{
			if (!float.IsFinite(v.x) || !float.IsFinite(v.y) || !float.IsFinite(v.z))
			{
				v = Vector3.zero;
			}
		}

		private void CalcWheelMovement()
		{
			if ((bool)ModelTransform && (bool)Chassis && Chassis.OnLineCar)
			{
				Wheel.AngularVelocity = ((!RotateModelByTransform) ? AngularMultiplayerVelocity : AngularWheelSpeedByTransform);
			}
		}

		private void RotateModelByTransformMovement(float dt)
		{
			if (!RotateModelByTransform || !Chassis)
			{
				return;
			}
			_rotSampleElapsed += dt;
			if (Time.frameCount % 3 == 0)
			{
				Transform transform = Chassis.transform;
				if (!_rotSampleSeeded)
				{
					_rotSampleSeeded = true;
					_rotSampleElapsed = 0f;
					_prevPosModel = transform.position;
				}
				else
				{
					float distance = Vector3.Dot(transform.position - _prevPosModel, transform.forward);
					AngularWheelSpeedByTransform = PhysMath.WheelSpinFromTravel(distance, _rotSampleElapsed, Wheel?.Radius ?? 0f);
					_rotSampleElapsed = 0f;
					_prevPosModel = transform.position;
				}
			}
		}

		public void SetRimSize(WheelDiameter d, RimWidth width)
		{
			_rimParams.x = Utils.Inch2Meter((float)d) * 0.5f;
			_rimParams.y = Utils.Inch2Meter((float)width * 0.1f);
		}

		public void SetTyre(TyreType tyreType, TyreWidth tyreWidth, TyreProfile tyreProfile, WheelDiameter rimDiameter)
		{
			_tyreParams.x = Utils.Mm2Meter(tyreWidth);
			_tyreParams.y = (float)tyreProfile;
			TyreType = tyreType;
			if ((bool)_temperatureTyresInfo)
			{
				_tempCurveTyre = _temperatureTyresInfo.GetTyreInfo(TyreType);
				CacheOptimalTempZone();
			}
			if ((bool)_we)
			{
				_we.SetWheel(CalculateWheelRadius(), _tyreParams.x);
			}
			if ((bool)_sphereCollider)
			{
				_sphereCollider.radius = CalculateWheelRadius();
			}
		}

		public void SetSpacers(float spacerWidth)
		{
			if ((bool)WheelModel)
			{
				WheelModel.SpacerWidth = spacerWidth;
			}
		}

		private void UpdateWarmUp()
		{
			if (Wheel != null && !Chassis.TrafficMode && !Chassis.OnLineCar)
			{
				float f = Wheel.AngularVelocity * Wheel.Radius - Wheel.ForwardVelocity;
				float sideVelocity = Wheel.SideVelocity;
				float num = Mathf.Abs(Wheel.ForwardVelocity);
				float num2 = Mathf.Abs(Wheel.LongtitudeForce) * Mathf.Abs(f);
				float num3 = Mathf.Abs(Wheel.LateralForce) * Mathf.Abs(sideVelocity);
				float num4 = num2 + num3;
				Wheel.CarDrifting = Chassis.IsSlidingForSmoke || Chassis.IsDriftingNow;
				float num5 = PhysMath.TyreLongSlidingShare(Wheel.AngularVelocity, Wheel.Radius, Wheel.ForwardVelocity);
				float num6 = (Wheel.CarDrifting ? PhysMath.TyreLatSlidingShare(Wheel.ForwardVelocity, Wheel.SideVelocity) : 0f);
				float heatPowerIn = num2 * num5 + num3 * num6;
				num4 = 12000f * (1f - Mathf.Exp((0f - num4) / 12000f));
				float heatPowerIn2 = num4 + 0.08f * Wheel.NormalForce * num;
				if (Wheel.IsPowered && Chassis.GearboxState == GearboxState.Burnout)
				{
					heatPowerIn2 = 200000f;
					heatPowerIn = 200000f;
				}
				float num7 = 80f + 0.8f * num;
				float num8 = WeatherPhysics.ApplyWetCooling(num7, WheelSurface);
				float heatCapacity = 0.5f * Wheel.Mass * 900f;
				float deltaTime = Time.deltaTime;
				_temperature = PhysMath.TyreThermalStep(_temperature, heatPowerIn2, num8, 25f, heatCapacity, 250f, deltaTime);
				_gripTemperature = PhysMath.TyreThermalStep(_gripTemperature, heatPowerIn2, num7, 25f, heatCapacity, 250f, deltaTime);
				float heatCapacity2 = 12f * Wheel.Mass;
				_surfaceTemperature = PhysMath.TyreSurfaceThermalStep(_surfaceTemperature, heatPowerIn, _temperature, num8, 25f, heatCapacity2, deltaTime);
				IWheel wheel = Wheel;
				TCurveTyre tempCurveTyre = _tempCurveTyre;
				wheel.TyreFriction = ((tempCurveTyre != null && tempCurveTyre.Curve != null) ? _tempCurveTyre.Curve.Evaluate(_gripTemperature) : 1f);
				Wheel.TyreTemperature = _temperature;
				Wheel.GripTemperature = _gripTemperature;
				Wheel.SurfaceTemperature = _surfaceTemperature;
			}
		}

		private void CacheOptimalTempZone()
		{
			_optimalTempLo = 70f;
			_optimalTempHi = 100f;
			if (_tempCurveTyre?.Curve == null || _tempCurveTyre.Curve.length == 0)
			{
				return;
			}
			Keyframe[] keys = _tempCurveTyre.Curve.keys;
			float num = float.MinValue;
			Keyframe[] array = keys;
			for (int i = 0; i < array.Length; i++)
			{
				Keyframe keyframe = array[i];
				if (keyframe.value > num)
				{
					num = keyframe.value;
				}
			}
			float num2 = float.MaxValue;
			float num3 = float.MinValue;
			array = keys;
			for (int i = 0; i < array.Length; i++)
			{
				Keyframe keyframe2 = array[i];
				if (keyframe2.value >= num - 0.005f)
				{
					if (keyframe2.time < num2)
					{
						num2 = keyframe2.time;
					}
					if (keyframe2.time > num3)
					{
						num3 = keyframe2.time;
					}
				}
			}
			_optimalTempLo = num2;
			_optimalTempHi = num3;
		}
	}
}
