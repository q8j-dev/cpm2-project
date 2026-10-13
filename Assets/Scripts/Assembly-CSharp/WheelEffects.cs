using System;
using CPMEngine;
using UnityEngine;
using _Car_Parking;
using _Car_Parking.Scripts.Car.Configs;
using _Car_Parking.Scripts.Car.Physics.Vfx;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SkidSmokeParticle;

public class WheelEffects : MonoBehaviour
{
	private const int FastSpeed = 30;

	private const float SlipVeloThreshold = 5f;

	private const float VelocityDivider = 300f;

	private const float CountVelocityDivider = 0.5f;

	private const float CountScaleFactor = 0.75f;

	private const int MaxParticleCount = 8;

	private const float BaseLifetime = 3.5f;

	private const float LifetimeScaleFactor = 0.2f;

	private const float SmokeAlphaFullTemp = 320f;

	private const float SmokeIgniteFullTemp = 200f;

	private const float SmokeMaxLaunchSpeed = 8f;

	private const float SmokeEntrainment = 0.15f;

	private const float SmokeSpreadDegrees = 25f;

	private const float SmokeMinLifetime = 0.5f;

	private const float BaseSize = 1f;

	private const int ParticleLimit = 2500;

	private const float TempDivider = 350f;

	private const int SkipFrames = 2;

	private const float MaxMultiplayerSmokeTime = 10f;

	private const float MaxMultiplayerSandSmokeTime = 3f;

	private const float EnterMultiplayerSmokeThresholdTime = 1.5f;

	private const float MaxEnterMultiplayerSmokeThresholdTime = 5f;

	private const float WheelNotSpinningLinearSpeedThreshold = 0.5f;

	private const float ThrottleEmitThreshold = 0.1f;

	private const float SandSmokeMinSpeed = 1f;

	private const float SandSmokeMaxFrequencySpeed = 15f;

	private const int SandSmokeMinFrameInterval = 6;

	private const int SandSmokeMaxFrameInterval = 13;

	private const float SandSmokeMinSizeFactor = 0.5f;

	private const float PuddleSplashMinSpeed = 2f;

	private const float PuddleSplashMaxSpeed = 25f;

	private const float PuddleSplashMinPuddleK = 0.02f;

	private const int PuddleSplashFrameSkip = 3;

	private const float PuddleSplashMinSizeMul = 0.3f;

	private const float PuddleSplashMinFillSizeMul = 0.15f;

	private const int PuddleSplashMaxCount = 3;

	private WheelProvider _wheel;

	private IWheel _wheelInterface;

	private Powertrain _chassis;

	private float _slipSkidAmount;

	private Vector3 _lastSkidPosition;

	private int _lastSkid;

	private FreeDriveProvider _freeDriveProvider;

	private Vector3 _prevPos;

	private int _skipFrame;

	private int _calcCount;

	private int _prevCalcCount;

	private float _skidSmokeTime;

	private float _radius;

	private float _width;

	private ParticleSystem.EmitParams _param;

	private Skidmarks _skidmarks;

	private ParticleSystem _cachedSandSmokeParticle;

	private Vector3 _cachedPatchCenter;

	private bool _isInited;

	private Vector3 _lerpedHitPointSkid;

	private bool _updatedHitPoint;

	private float _smokeMultiplayerCurrentTime;

	private bool _markSlipSkids;

	private bool _markDigSkids;

	private Surface _cachedVfxSurface = (Surface)(-1);

	private WheelSurfaceVfxRuntime.RuntimeEntry[] _cachedVfxEntries;

	private static int _smokeQualityApplied = -1;

	public float SmokeTimeMultiplayerCar { get; set; }

	private ParticleSystem SandSmokeParticle
	{
		get
		{
			if (!_cachedSandSmokeParticle && (bool)V.CoreGameManager && (bool)V.CoreGameManager.sandSmoke)
			{
				_cachedSandSmokeParticle = V.CoreGameManager.sandSmoke;
			}
			return _cachedSandSmokeParticle;
		}
	}

	private bool IsInitSkidmarks
	{
		get
		{
			if ((bool)_skidmarks)
			{
				return _skidmarks.skidmarks != null;
			}
			return false;
		}
	}

	private void ApplySmokeQualityGate()
	{
		int quality = CPMGraphicsSettingsManager.Instance.Quality;
		if (quality != _smokeQualityApplied)
		{
			_smokeQualityApplied = quality;
			ParticleSystem.NoiseModule noise = _wheel.SkidSmokeParticle.noise;
			noise.enabled = quality >= 2;
		}
	}

	public void SetWheel(float radius, float width)
	{
		_radius = radius;
		_width = width;
	}

	public void SetSkidmarks(Skidmarks skidmarks)
	{
		_skidmarks = skidmarks;
	}

	public void Init(WheelProvider wheel, Powertrain chassis)
	{
		_wheel = wheel;
		if ((bool)_wheel)
		{
			if (!_isInited)
			{
				_wheel.OnUpdateWheelEvent -= HandleOnUpdateWheelEvent;
				_wheel.OnUpdateWheelEvent += HandleOnUpdateWheelEvent;
				_isInited = true;
			}
			_wheelInterface = wheel.Wheel;
		}
		_chassis = chassis;
	}

	private void GenerateSkidmarks()
	{
		if (IsInitSkidmarks && !FlagsStorage.HideSkidMarks && _wheel.GetMarkPatch(out var left, out var right, out var center, out var normal))
		{
			float width = Vector3.Distance(left, right);
			GenerateSlipSkidMarks(center, normal, width);
			GenerateDigSkidmarks(center, normal, width);
			_lastSkidPosition = center;
			if (!_markSlipSkids && !_markDigSkids)
			{
				_lastSkid = -1;
			}
		}
	}

	private void GenerateDigSkidmarks(Vector3 center, Vector3 normal, float width)
	{
		Surface surfaceType = _wheelInterface.CurrentPhysMat.SurfaceType;
		if (surfaceType == Surface.Concrete || surfaceType == Surface.Kerb || surfaceType == Surface.Glue || surfaceType == Surface.Dryasphalt || surfaceType == Surface.Wetasphalt || CPMGraphicsSettingsManager.Instance.Quality < 2 || !(Mathf.Abs(_wheelInterface.AngularVelocity) > 0.5f))
		{
			return;
		}
		if ((_lastSkidPosition - center).sqrMagnitude > 25f)
		{
			_lastSkid = -1;
			return;
		}
		Surface surfaceType2 = _wheelInterface.CurrentPhysMat.SurfaceType;
		float digOpacity = SkidmarkColorContainer.GetDigOpacity(surfaceType2);
		if (digOpacity <= 0f)
		{
			_lastSkid = -1;
		}
		if ((bool)_skidmarks)
		{
			_lastSkid = _skidmarks.AddSkidMark(center, normal, digOpacity, _lastSkid, width, surfaceType2);
		}
		_markDigSkids = true;
	}

	private void GenerateSlipSkidMarks(Vector3 center, Vector3 normal, float width)
	{
		_slipSkidAmount = Mathf.Clamp((_wheelInterface.SlipAngleVolume + _wheelInterface.SlipRatioVolume) / 5f, 0f, 0.6f);
		if (!(_wheelInterface.SlipAngleVolume > 1.4f) && !(_wheelInterface.SlipRatioVolume > 0.7f))
		{
			return;
		}
		if ((_lastSkidPosition - center).sqrMagnitude > 25f)
		{
			_lastSkid = -1;
			return;
		}
		Surface surfaceType = _wheelInterface.CurrentPhysMat.SurfaceType;
		float opacity = Mathf.Clamp((surfaceType == Surface.Snow) ? (_slipSkidAmount * 1.5f) : _slipSkidAmount, SkidmarkColorContainer.GetSlipFloor(surfaceType), SkidmarkColorContainer.GetSlipCeiling(surfaceType));
		float num;
		switch (surfaceType)
		{
		case Surface.OutOfRoad:
		case Surface.Damp:
		case Surface.DirtRoad:
		case Surface.Offroad:
		case Surface.Grass:
		case Surface.Sand:
		case Surface.Gravel:
			num = width * 1.1f;
			break;
		case Surface.Snow:
			num = width * 1.05f;
			break;
		default:
			num = width;
			break;
		}
		float markWidth = num;
		_lastSkid = _skidmarks.AddSkidMark(center, normal, opacity, _lastSkid, markWidth, surfaceType);
		_markSlipSkids = true;
	}

	private void GenerateSkidmarksOnline()
	{
		if (IsInitSkidmarks && !FlagsStorage.HideSkidMarks && (bool)_wheel.ModelTransform)
		{
			Transform modelTransform = _wheel.ModelTransform;
			Vector3 up = base.transform.up;
			Vector3 vector = modelTransform.position - _radius * 0.92f * up;
			_lerpedHitPointSkid = Vector3.MoveTowards(vector, vector, _chassis.SpeedMpsOnline * Time.deltaTime);
			GenerateOnlineSlipSkidMarks(vector, up);
			GenerateOnlineDigSkidMarks(_lerpedHitPointSkid, up);
			_lastSkidPosition = vector;
			if (!_markSlipSkids && !_markDigSkids)
			{
				_lastSkid = -1;
			}
		}
	}

	private void GenerateOnlineDigSkidMarks(Vector3 center, Vector3 normal)
	{
		Surface surfaceType = _wheelInterface.CurrentPhysMat.SurfaceType;
		if (surfaceType == Surface.Concrete || surfaceType == Surface.Kerb || surfaceType == Surface.Glue || surfaceType == Surface.Dryasphalt || surfaceType == Surface.Wetasphalt || surfaceType == Surface.None || CPMGraphicsSettingsManager.Instance.Quality < 2 || !(Mathf.Abs(_wheelInterface.AngularVelocity) > 0.5f))
		{
			return;
		}
		if ((_lastSkidPosition - center).sqrMagnitude > 25f)
		{
			_lastSkid = -1;
			return;
		}
		Surface surfaceType2 = _wheelInterface.CurrentPhysMat.SurfaceType;
		float digOpacity = SkidmarkColorContainer.GetDigOpacity(surfaceType2);
		if (digOpacity <= 0f)
		{
			_lastSkid = -1;
		}
		_lastSkid = _skidmarks.AddSkidMark(center, normal, digOpacity, _lastSkid, _width, surfaceType2);
		_markDigSkids = true;
	}

	private void GenerateOnlineSlipSkidMarks(Vector3 center, Vector3 normal)
	{
		float num = (_wheel.IsSkidmarkOnline ? 0.011f : (-0.011f));
		if (!_wheel.IsFront)
		{
			num *= 2f;
		}
		_slipSkidAmount = Mathf.Clamp(_slipSkidAmount + num + _chassis.Brake * 0.75f, 0f, 0.6f);
		if (_slipSkidAmount < 0.011f)
		{
			return;
		}
		if ((_lastSkidPosition - center).sqrMagnitude > 25f)
		{
			_lastSkid = -1;
			return;
		}
		Surface surfaceType = _wheelInterface.CurrentPhysMat.SurfaceType;
		float value;
		switch (surfaceType)
		{
		case Surface.None:
			return;
		default:
			value = _slipSkidAmount;
			break;
		case Surface.Snow:
			value = _slipSkidAmount * 1.5f;
			break;
		}
		float opacity = Mathf.Clamp(value, SkidmarkColorContainer.GetSlipFloor(surfaceType), SkidmarkColorContainer.GetSlipCeiling(surfaceType));
		float num2;
		switch (surfaceType)
		{
		case Surface.OutOfRoad:
		case Surface.Damp:
		case Surface.DirtRoad:
		case Surface.Offroad:
		case Surface.Grass:
		case Surface.Sand:
		case Surface.Gravel:
			num2 = _width * 1.1f;
			break;
		case Surface.Snow:
			num2 = _width * 1.05f;
			break;
		default:
			num2 = _width;
			break;
		}
		float markWidth = num2;
		_lastSkid = _skidmarks.AddSkidMark(center, normal, opacity, _lastSkid, markWidth, surfaceType);
		_markSlipSkids = true;
	}

	private void GenerateWheelSmoke()
	{
		_calcCount = 0;
		Vector3 left = _chassis.Velocity;
		_skipFrame = ((left.magnitude >= 30f) ? 1 : 2);
		if (FlagsStorage.HideSkidSmoke)
		{
			return;
		}
		if (_freeDriveProvider != null && (bool)_freeDriveProvider.FreeDriveControl && _freeDriveProvider.FreeDriveControl.IsToRoadButtonCkicked)
		{
			_prevPos = base.transform.position;
			return;
		}
		CPMEngine.PhysicMaterial currentPhysMat;
		if ((bool)_wheel.SkidSmokeParticle && Time.frameCount % _skipFrame == 0 && SkidSmokeCounter.SkidSmokeParticleCount <= 2500)
		{
			ApplySmokeQualityGate();
			_wheel.GetPatch(out left, out var _, out var center, out var _);
			if (_wheelInterface.GetSmoke())
			{
				currentPhysMat = _wheelInterface.CurrentPhysMat;
				if (currentPhysMat != null && currentPhysMat.SkidSmoke && !WeatherPhysics.IsWetForEffects)
				{
					if (_wheel.SurfaceTemperature > 160f)
					{
						Vector3 vector = base.transform.position - _prevPos;
						float num = Mathf.InverseLerp(160f, 200f, _wheel.SurfaceTemperature);
						_calcCount = Mathf.Max(2, Mathf.CeilToInt((float)CalcCount(vector.sqrMagnitude) * num));
						float num2 = CalcLife();
						float num3 = UnityEngine.Random.Range(-0.2f, 0.2f);
						_param.startLifetime = Mathf.Max(num2 + num3, 0.5f);
						float num4 = Mathf.Lerp(0.2f, 1f, Mathf.InverseLerp(160f, 320f, _wheel.SurfaceTemperature)) * Mathf.Lerp(0.35f, 1f, num);
						Color skidSmokeColor = _wheel.SkidSmokeColor;
						skidSmokeColor.a = num4;
						_param.startColor = skidSmokeColor;
						_param.startSize = (1f + num3) * Mathf.Lerp(0.55f, 1f, num);
						Vector3 smokeVelocity = _wheelInterface.SmokeVelocity;
						Vector3 vector2 = new Vector3(smokeVelocity.x, 0f, smokeVelocity.z);
						float magnitude = vector2.magnitude;
						if (magnitude > 0.001f)
						{
							vector2 *= 8f * (1f - Mathf.Exp((0f - magnitude) / 8f)) / magnitude;
						}
						Vector3 velocity = _chassis.Velocity;
						vector2 += new Vector3(velocity.x, 0f, velocity.z) * 0.15f;
						vector2 = Quaternion.AngleAxis(UnityEngine.Random.Range(-25f, 25f), Vector3.up) * vector2;
						vector2.y = UnityEngine.Random.Range(0.3f, 1.2f);
						_param.velocity = vector2;
						Vector3 vector3 = ((_calcCount <= 1) ? Vector3.zero : (vector / _calcCount));
						Vector3 vector4 = Vector3.up * (0.35f * _param.startSize);
						for (int i = 0; i < _calcCount; i++)
						{
							float num5 = UnityEngine.Random.Range(-0.3f, 0.3f);
							_param.position = center - vector3 * i + base.transform.right * num5 + vector4 - V.GlobalOffset;
							_wheel.SkidSmokeParticle.Emit(_param, 1);
						}
						if (CPMGraphicsSettingsManager.Instance.Quality >= 2)
						{
							Color color = skidSmokeColor;
							color.a = Mathf.Min(1f, num4 * 1.5f);
							_param.startColor = color;
							_param.startLifetime = UnityEngine.Random.Range(0.25f, 0.45f);
							_param.startSize = (0.7f + num3 * 0.5f) * Mathf.Lerp(0.55f, 1f, num);
							_param.velocity = new Vector3(velocity.x, 0f, velocity.z) * 0.9f + vector2 * 0.2f;
							_param.position = center + base.transform.right * UnityEngine.Random.Range(-0.15f, 0.15f) + Vector3.up * (0.35f * _param.startSize) - V.GlobalOffset;
							_wheel.SkidSmokeParticle.Emit(_param, 2);
							_param.startColor = skidSmokeColor;
							_param.startLifetime = Mathf.Max(num2 + num3, 0.5f);
							_param.startSize = (1f + num3) * Mathf.Lerp(0.55f, 1f, num);
						}
						if (FlagsStorage.SingeDragRacingMode && _chassis.Velocity.magnitude < 1f)
						{
							_param.velocity = -_chassis.transform.forward * (_param.startLifetime * 2f);
							_param.position = center - V.GlobalOffset;
							_wheel.SkidSmokeParticle.Emit(_param, 1);
						}
					}
					goto IL_069b;
				}
			}
			int prevCalcCount = _prevCalcCount;
			if (prevCalcCount > 0 && prevCalcCount <= 2 && _calcCount == 0)
			{
				Vector3 forward = _chassis.transform.forward;
				_param.startColor = _wheel.SkidSmokeColor;
				_param.position = center + forward * 0.2f - V.GlobalOffset;
				_wheel.SkidSmokeParticle.Emit(_param, 1);
				_param.position = center - forward * 0.2f - V.GlobalOffset;
				_wheel.SkidSmokeParticle.Emit(_param, 1);
			}
		}
		goto IL_069b;
		IL_069b:
		if (!SandSmokeParticle)
		{
			return;
		}
		float num6 = _wheelInterface.AngularVelocity * _radius;
		currentPhysMat = _wheelInterface.CurrentPhysMat;
		if (currentPhysMat != null && currentPhysMat.SandSmoke && _wheelInterface.CurrentPhysMat.SurfaceType != Surface.Gravel && !WeatherPhysics.IsWetForEffects && num6 > 1f && Time.frameCount % CalcSandSmokeFrameInterval(num6) == 0)
		{
			float num7 = Mathf.Lerp(0.5f, 1f, CalcSandSmokeSpeedFactor(num6));
			Vector3 vector5 = base.transform.position - _prevPos;
			int num8 = CalcCount(vector5.sqrMagnitude);
			Vector3 vector6 = ((num8 <= 1) ? Vector3.zero : (vector5 / num8));
			ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
			{
				startLifetime = UnityEngine.Random.Range(2f, 3f),
				startSize = (1f + UnityEngine.Random.Range(-0.2f, 0.2f)) * num7
			};
			if (_wheelInterface.CurrentPhysMat.SurfaceType == Surface.Snow)
			{
				emitParams.startColor = new Color(0.93f, 0.96f, 1f, 0.85f);
			}
			for (int j = 0; j < num8; j++)
			{
				float num9 = UnityEngine.Random.Range(-0.3f, 0.3f);
				emitParams.position = _cachedPatchCenter + vector6 * j + base.transform.right * num9 - V.GlobalOffset;
				SandSmokeParticle.Emit(emitParams, 1);
			}
			if (_wheelInterface.SmokeVelocity.magnitude - 5f > 10f)
			{
				emitParams.startSize = (1f + UnityEngine.Random.Range(-0.2f, 0.2f)) * num7;
				emitParams.position = _cachedPatchCenter + new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f)) - V.GlobalOffset;
				SandSmokeParticle.Emit(emitParams, 2);
			}
		}
	}

	private int CalcCount(float velocity)
	{
		float num = 1f + velocity / 0.5f;
		return Mathf.Min((int)(1f + 0.75f * num), 8);
	}

	private float CalcLife()
	{
		float num = _wheel.SurfaceTemperature / 350f;
		float sqrMagnitude = _chassis.Velocity.sqrMagnitude;
		float num2 = 1f + sqrMagnitude / 300f;
		return 3.5f * num - 0.2f * num2;
	}

	private float CalcSandSmokeSpeedFactor(float wheelLinearSpeed)
	{
		return Mathf.InverseLerp(1f, 15f, wheelLinearSpeed);
	}

	private int CalcSandSmokeFrameInterval(float wheelLinearSpeed)
	{
		float t = CalcSandSmokeSpeedFactor(wheelLinearSpeed);
		return Mathf.Max(Mathf.RoundToInt(Mathf.Lerp(13f, 6f, t)), 6);
	}

	private void GenerateWheelSmokeOnline(float deltaTimeRatio)
	{
		if (!_wheel.SkidSmokeParticle || !_wheel.ModelTransform || FlagsStorage.HideSkidSmoke)
		{
			return;
		}
		Transform modelTransform = _wheel.ModelTransform;
		float width = _width;
		Vector3 vector = modelTransform.position - _radius * modelTransform.up;
		CPMEngine.PhysicMaterial currentPhysMat;
		if (_wheel.IsSkidmarkOnline)
		{
			currentPhysMat = _wheelInterface.CurrentPhysMat;
			if (currentPhysMat != null && currentPhysMat.SkidSmoke && !WeatherPhysics.IsWetForEffects)
			{
				if (_smokeMultiplayerCurrentTime < 5f)
				{
					_smokeMultiplayerCurrentTime += deltaTimeRatio;
				}
				if (_smokeMultiplayerCurrentTime < 1.5f)
				{
					return;
				}
				SmokeTimeMultiplayerCar += deltaTimeRatio;
				SmokeTimeMultiplayerCar = Mathf.Clamp(SmokeTimeMultiplayerCar, 0f, 10f);
				if (SkidSmokeCounter.SkidSmokeParticleCount > 2500)
				{
					return;
				}
				if (Time.frameCount % 2 == 0)
				{
					_param.position = vector + new Vector3(0f, _radius, 0f) + new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f)) - V.GlobalOffset;
					float num = Mathf.Clamp(SmokeTimeMultiplayerCar, 1f, 4f);
					float startLifetime = UnityEngine.Random.Range(0.42f, 0.7f) * num;
					float num2 = UnityEngine.Random.Range(-0.2f, 0.2f);
					_param.startSize = 1f + num2;
					_param.startLifetime = startLifetime;
					Color skidSmokeColor = _wheel.SkidSmokeColor;
					skidSmokeColor.a = Mathf.Lerp(0.2f, 1f, Mathf.Clamp01(SmokeTimeMultiplayerCar / 6f));
					_param.startColor = skidSmokeColor;
					float smokeTimeMultiplayerCar = SmokeTimeMultiplayerCar;
					if (smokeTimeMultiplayerCar >= 0f)
					{
						if (smokeTimeMultiplayerCar <= 6f)
						{
							_wheel.SkidSmokeParticle.Emit(_param, 1);
						}
						else
						{
							if (!_wheel.IsLeft)
							{
								_param.position = vector + new Vector3(0f, _radius * 2f, 0f) + modelTransform.right * (width * 0.5f) + new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f)) - V.GlobalOffset;
							}
							else
							{
								_param.position = vector + new Vector3(0f, _radius * 2f, 0f) - modelTransform.right * (width * 0.5f) + new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f)) - V.GlobalOffset;
							}
							_wheel.SkidSmokeParticle.Emit(_param, 2);
						}
					}
				}
			}
		}
		if (!SandSmokeParticle)
		{
			return;
		}
		float num3 = _wheelInterface.AngularVelocity * _radius;
		currentPhysMat = _wheelInterface.CurrentPhysMat;
		if (currentPhysMat != null && currentPhysMat.SandSmoke && (bool)SandSmokeParticle && _wheelInterface.CurrentPhysMat.SurfaceType != Surface.Gravel && !WeatherPhysics.IsWetForEffects && num3 > 1f && Time.frameCount % CalcSandSmokeFrameInterval(num3) == 0)
		{
			float num4 = Mathf.Lerp(0.5f, 1f, CalcSandSmokeSpeedFactor(num3));
			ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
			{
				position = vector + new Vector3(0f, _radius, 0f) + new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f)) - V.GlobalOffset,
				startLifetime = UnityEngine.Random.Range(1.5f, 3f),
				startSize = (1f + UnityEngine.Random.Range(-0.2f, 0.2f)) * num4
			};
			if (_wheelInterface.CurrentPhysMat.SurfaceType == Surface.Snow)
			{
				emitParams.startColor = new Color(0.93f, 0.96f, 1f, 0.85f);
			}
			SandSmokeParticle.Emit(emitParams, 1);
			SmokeTimeMultiplayerCar += deltaTimeRatio;
			SmokeTimeMultiplayerCar = Mathf.Clamp(SmokeTimeMultiplayerCar, 0f, 3f);
			if (SmokeTimeMultiplayerCar > 2f)
			{
				Vector3 vector2 = (_wheel.IsLeft ? (-modelTransform.right * (width * 0.5f)) : (modelTransform.right * (width * 0.5f)));
				emitParams.startSize = (1f + UnityEngine.Random.Range(-0.2f, 0.2f)) * num4;
				emitParams.position = vector + new Vector3(0f, _radius * 2f, 0f) + vector2 + new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f)) - V.GlobalOffset;
				SandSmokeParticle.Emit(emitParams, 2);
			}
		}
		else
		{
			if (_smokeMultiplayerCurrentTime > 0f)
			{
				_smokeMultiplayerCurrentTime -= deltaTimeRatio;
			}
			if (!(SmokeTimeMultiplayerCar <= 0f))
			{
				SmokeTimeMultiplayerCar -= deltaTimeRatio;
			}
		}
	}

	private void GenerateSurfaceVfx(Vector3 patchOrigin, Vector3 hubOrigin, bool isOnline)
	{
		if (FlagsStorage.HideWheelDirt)
		{
			return;
		}
		WheelSurfaceVfxRuntime wheelSurfaceVfxRuntime = V.WheelSurfaceVfxService?.Runtime;
		if (wheelSurfaceVfxRuntime == null || wheelSurfaceVfxRuntime.BySurface == null)
		{
			return;
		}
		CPMEngine.PhysicMaterial currentPhysMat = _wheelInterface.CurrentPhysMat;
		if (currentPhysMat == null)
		{
			return;
		}
		Surface surfaceType = currentPhysMat.SurfaceType;
		if (surfaceType == Surface.None)
		{
			return;
		}
		if (surfaceType != _cachedVfxSurface)
		{
			_cachedVfxSurface = surfaceType;
			int num = (int)surfaceType;
			_cachedVfxEntries = ((num >= 0 && num < wheelSurfaceVfxRuntime.BySurface.Length) ? wheelSurfaceVfxRuntime.BySurface[num] : null);
		}
		WheelSurfaceVfxRuntime.RuntimeEntry[] cachedVfxEntries = _cachedVfxEntries;
		if (cachedVfxEntries == null || cachedVfxEntries.Length == 0 || (isOnline && SkidSmokeCounter.SkidSmokeParticleCount > 2500))
		{
			return;
		}
		float num2 = Mathf.Abs(_wheelInterface.AngularVelocity * _radius);
		float slipRatioVolume = _wheelInterface.SlipRatioVolume;
		float slipAngleVolume = _wheelInterface.SlipAngleVolume;
		bool flag = ((!isOnline) ? _chassis.IsDriftingNow : (slipAngleVolume > 1.4f || _chassis.Handbrake > 0.5f));
		bool isFront = _wheel.IsFront;
		bool flag2 = isOnline && CPMGraphicsSettingsManager.Instance.Quality < 2;
		int frameCount = Time.frameCount;
		foreach (WheelSurfaceVfxRuntime.RuntimeEntry runtimeEntry in cachedVfxEntries)
		{
			WheelSurfaceVfxEntry source = runtimeEntry.Source;
			if ((flag2 && !source.EmitForOnlineCarsAtLowQuality) || (source.FrameSkip > 1 && frameCount % source.FrameSkip != 0) || num2 < source.MinWheelLinearSpeed || (source.MinSlipRatio > 0f && slipRatioVolume < source.MinSlipRatio) || (source.MinSlipAngle > 0f && slipAngleVolume < source.MinSlipAngle) || (source.RequireThrottle && _chassis.Throttle < 0.1f) || (flag && (!source.SkipEmitOnDriftWithoutWheelSpin || num2 < 0.5f) && ((isFront && !source.EmitOnFrontDuringDrift) || (!isFront && !source.EmitOnRearDuringDrift))) || !runtimeEntry.Instance)
			{
				continue;
			}
			if (source.EmitOrigin == WheelEmitOrigin.WheelHub)
			{
				EmitTangentialAroundHub(runtimeEntry, hubOrigin);
				continue;
			}
			ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
			{
				position = patchOrigin
			};
			emitParams.startColor = runtimeEntry.TintColor;
			if (runtimeEntry.StartSize > 0f)
			{
				emitParams.startSize = runtimeEntry.StartSize;
			}
			if (source.UseWheelSmokeVelocityForParticles && !isOnline)
			{
				EmitWithShapedVelocity(runtimeEntry, source, emitParams);
			}
			else
			{
				runtimeEntry.Instance.Emit(emitParams, source.EmitCount);
			}
		}
	}

	private void EmitWithShapedVelocity(WheelSurfaceVfxRuntime.RuntimeEntry runtimeEntry, WheelSurfaceVfxEntry entry, ParticleSystem.EmitParams emitParams)
	{
		Vector3 smokeVelocity = _wheelInterface.SmokeVelocity;
		Vector3 vector = new Vector3(smokeVelocity.x, 0f, smokeVelocity.z);
		float num = vector.magnitude * entry.LaunchSpeedMultiplier;
		if (entry.MaxLaunchSpeed > 0f && num > 0f)
		{
			num = entry.MaxLaunchSpeed * (1f - Mathf.Exp((0f - num) / entry.MaxLaunchSpeed));
		}
		Vector3 vector2 = ((num > 0.001f) ? (vector / vector.magnitude) : Vector3.zero);
		float num2 = num * Mathf.Tan(entry.EmitUpwardAngleDegrees * ((float)Math.PI / 180f));
		Vector3 position = emitParams.position;
		int emitCount = entry.EmitCount;
		for (int i = 0; i < emitCount; i++)
		{
			float num3 = ((entry.SpreadAngleDegrees > 0f) ? UnityEngine.Random.Range(0f - entry.SpreadAngleDegrees, entry.SpreadAngleDegrees) : 0f);
			Vector3 vector3 = ((num3 != 0f) ? (Quaternion.AngleAxis(num3, Vector3.up) * vector2) : vector2);
			float num4 = ((entry.SpeedRandomness > 0f) ? (1f + UnityEngine.Random.Range(0f - entry.SpeedRandomness, entry.SpeedRandomness)) : 1f);
			emitParams.velocity = (vector3 * num + Vector3.up * num2) * num4;
			if (entry.SpreadAngleDegrees > 0f)
			{
				emitParams.position = position + base.transform.right * UnityEngine.Random.Range(-0.1f, 0.1f) + base.transform.forward * UnityEngine.Random.Range(-0.1f, 0.1f);
			}
			runtimeEntry.Instance.Emit(emitParams, 1);
		}
	}

	private void EmitTangentialAroundHub(WheelSurfaceVfxRuntime.RuntimeEntry runtimeEntry, Vector3 hubOrigin)
	{
		if (!_wheel.ModelTransform)
		{
			return;
		}
		WheelSurfaceVfxEntry source = runtimeEntry.Source;
		Transform modelTransform = _wheel.ModelTransform;
		Vector3 right = modelTransform.right;
		Vector3 forward = modelTransform.forward;
		Vector3 up = modelTransform.up;
		float num = _radius * source.ParticleAngularSpeed;
		float num2 = (_wheel.IsLeft ? (-1f) : 1f);
		Vector3 vector = right * (num2 * source.OuterSideOffset);
		int emitCount = source.EmitCount;
		for (int i = 0; i < emitCount; i++)
		{
			float f = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
			float num3 = Mathf.Cos(f);
			float num4 = Mathf.Sin(f);
			Vector3 vector2 = forward * num3 + up * num4;
			Vector3 vector3 = Vector3.Cross(right, vector2);
			ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
			{
				position = hubOrigin + vector2 * _radius + vector,
				velocity = vector3 * num,
				startColor = runtimeEntry.TintColor
			};
			if (runtimeEntry.StartSize > 0f)
			{
				emitParams.startSize = runtimeEntry.StartSize;
			}
			runtimeEntry.Instance.Emit(emitParams, 1);
		}
	}

	private void DirtTyreEffect()
	{
		if (_wheelInterface.CurrentPhysMat != null)
		{
			switch (_wheelInterface.CurrentPhysMat.SurfaceType)
			{
			case Surface.Grass:
				_wheel.DirtAmount += Time.deltaTime * Mathf.Abs(_wheelInterface.AngularVelocity / 1500f);
				break;
			case Surface.OutOfRoad:
			case Surface.Damp:
			case Surface.Sand:
			case Surface.Gravel:
				_wheel.DirtAmount += Time.deltaTime * Mathf.Abs(_wheelInterface.AngularVelocity / 1200f);
				break;
			case Surface.Glue:
				_wheel.DirtAmount += Time.deltaTime * Mathf.Abs(_wheelInterface.AngularVelocity / 3000f);
				break;
			case Surface.DirtRoad:
			case Surface.Offroad:
			case Surface.Snow:
				_wheel.DirtAmount += Time.deltaTime * Mathf.Abs(_wheelInterface.AngularVelocity / 900f);
				break;
			case Surface.Winterroad:
			case Surface.Ice:
				_wheel.DirtAmount += Time.deltaTime * Mathf.Abs(_wheelInterface.AngularVelocity / 3000f);
				break;
			default:
				_wheel.DirtAmount -= Time.deltaTime * Mathf.Abs(_wheelInterface.AngularVelocity / 2000f);
				break;
			case Surface.None:
				break;
			}
			_wheel.DirtAmount = Mathf.Clamp(Utils.GetFixedVariable(_wheel.DirtAmount), 0f, 3f);
		}
	}

	private void GeneratePuddleSplash(Vector3 patchCenter, float carSpeed)
	{
		if (FlagsStorage.HideWheelDirt || !RainWetnessDriver.HasActiveDriver || carSpeed < 2f)
		{
			return;
		}
		CPMEngine.PhysicMaterial currentPhysMat = _wheelInterface.CurrentPhysMat;
		if (currentPhysMat == null)
		{
			return;
		}
		switch (currentPhysMat.SurfaceType)
		{
		case Surface.Dryasphalt:
		case Surface.Wetasphalt:
		case Surface.OutOfRoad:
		case Surface.Damp:
		case Surface.DirtRoad:
		case Surface.Offroad:
		case Surface.Grass:
		case Surface.Indoor:
		case Surface.Sand:
		case Surface.Wood:
		case Surface.Glue:
		case Surface.Kerb:
		case Surface.Concrete:
		case Surface.Gravel:
		{
			int num = ((!_wheel.IsFront) ? 1 : 0) + ((!_wheel.IsLeft) ? 1 : 0);
			if ((Time.frameCount + num) % 3 != 0)
			{
				break;
			}
			ParticleSystem splashPS = RainWetnessDriver.GetSplashPS();
			if ((bool)splashPS && !(RainWetnessDriver.SamplePuddleAt(patchCenter) < 0.02f))
			{
				float currentPuddleLevel = RainWetnessDriver.CurrentPuddleLevel;
				float num2 = Mathf.InverseLerp(2f, 25f, carSpeed);
				float num3 = Mathf.Lerp(0.3f, 1f, num2) * Mathf.Lerp(0.15f, 1f, currentPuddleLevel);
				int num4 = 1 + Mathf.RoundToInt(num2 * currentPuddleLevel * 2f);
				Vector3 vector = base.transform.position - _prevPos;
				Vector3 vector2 = ((num4 <= 1) ? Vector3.zero : (vector / num4));
				ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
				{
					startLifetime = UnityEngine.Random.Range(0.5f, 1.5f),
					startSize = (1f + UnityEngine.Random.Range(-0.2f, 0.2f)) * num3
				};
				for (int i = 0; i < num4; i++)
				{
					emitParams.position = patchCenter + vector2 * i + base.transform.right * UnityEngine.Random.Range((0f - _width) * 0.5f, _width * 0.5f);
					splashPS.Emit(emitParams, 1);
				}
			}
			break;
		}
		}
	}

	private void OnEnable()
	{
		if (_isInited && (bool)_wheel)
		{
			_wheel.OnUpdateWheelEvent -= HandleOnUpdateWheelEvent;
			_wheel.OnUpdateWheelEvent += HandleOnUpdateWheelEvent;
		}
	}

	private void OnDisable()
	{
		if (_isInited || (bool)_wheel)
		{
			_wheel.OnUpdateWheelEvent -= HandleOnUpdateWheelEvent;
		}
	}

	private void OnDestroy()
	{
		if ((bool)_wheel)
		{
			_wheel.OnUpdateWheelEvent -= HandleOnUpdateWheelEvent;
		}
	}

	private void HandleOnUpdateWheelEvent(WheelProvider wheel)
	{
		if (!wheel.Wheel.Enabled)
		{
			return;
		}
		if (Time.timeScale == 0f)
		{
			_lastSkid = -1;
			return;
		}
		_markSlipSkids = false;
		_markDigSkids = false;
		if (_chassis.OnLineCar)
		{
			GenerateSkidmarksOnline();
			GenerateWheelSmokeOnline(Time.deltaTime);
			if ((bool)_wheel.ModelTransform)
			{
				Transform modelTransform = _wheel.ModelTransform;
				Vector3 patchOrigin = modelTransform.position - _radius * modelTransform.up;
				Vector3 position = modelTransform.position;
				GenerateSurfaceVfx(patchOrigin, position, isOnline: true);
			}
		}
		else
		{
			Vector3 velocity = _chassis.Velocity;
			Vector3 vector = (RainConeRendererFeature.CarVelocityXZ = new Vector3(velocity.x, 0f, velocity.z));
			wheel.GetMarkPatch(out var _, out var _, out _cachedPatchCenter, out var _);
			GenerateSkidmarks();
			GenerateWheelSmoke();
			Vector3 hubOrigin = (_wheel.ModelTransform ? _wheel.ModelTransform.position : _cachedPatchCenter);
			GenerateSurfaceVfx(_cachedPatchCenter, hubOrigin, isOnline: false);
			GeneratePuddleSplash(_cachedPatchCenter, vector.magnitude);
		}
		DirtTyreEffect();
		_prevPos = base.transform.position;
		_prevCalcCount = _calcCount;
	}
}
