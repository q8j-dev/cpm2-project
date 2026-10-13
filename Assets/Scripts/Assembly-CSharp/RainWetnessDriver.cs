using System;
using CPMEngine;
using Car_Parking.Scripts.Drag.CarCompositions;
using GraphicsRND.Rain.PuddleRipples;
using UnityEngine;
using UnityEngine.Rendering;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Providers;

[DisallowMultipleComponent]
[ExecuteAlways]
public sealed class RainWetnessDriver : MonoBehaviour
{
	private const float RainOffEpsilon = 0.001f;

	[Header("Rain (Single Control)")]
	[Range(0f, 1f)]
	public float rainIntensity;

	[Range(0f, 1f)]
	[Header("Material Response")]
	public float kDark = 0.3f;

	[Range(0f, 1f)]
	public float kSmoothBoost = 1f;

	[Header("Intensity Smoothing (seconds)")]
	[Min(0f)]
	public float intensitySmoothTime = 1.5f;

	[Header("Wetness Timing (seconds)")]
	[Range(0f, 1f)]
	public float wetnessThreshold = 0.1f;

	[Min(0.0001f)]
	public float wetIncreaseTime = 2f;

	[Min(0.0001f)]
	public float wetDryTime = 90f;

	[Tooltip("Drying clock for the drops and wiper marks on the car only — glass and paint shed water long before the world does, so this runs far shorter than wetDryTime.")]
	[Min(0.0001f)]
	public float carDryTime = 4f;

	[Min(0.0001f)]
	public float filmSmoothingTime = 0.6f;

	[Min(0.0001f)]
	public float puddleSmoothingTime = 0.9f;

	[Range(0f, 1f)]
	public float puddleIntensityInfluence = 1f;

	[Tooltip("Puddle level below which the puddle look fades out. PuddleWetPower < 1 keeps _PuddleKT high at tiny levels, so without this the drying tail leaves puddle-shaped gloss behind.")]
	[Range(0f, 1f)]
	public float puddleDryTailStart = 0.25f;

	[Header("Profile (ScriptableObject)")]
	public WetParamsProfile profile;

	[Header("Wet Surface Noise")]
	public Texture2D wetNoiseMap;

	[Header("Puddle Splash VFX")]
	public ParticleSystem puddleSplashParticle;

	[Header("Drop Textures")]
	[Tooltip("Static still-drops texture, pushed as the _StillDrops global.")]
	public Texture2D stillDrops;

	[Tooltip("Animated drop/streak CRTs. Not globals — HardSurfaceRainOpaque references them directly; the driver just ticks their Update().")]
	public RenderTexture rainDrops;

	public RenderTexture rainStreaks;

	[Min(0.05f)]
	[Tooltip("Duration of a single blade sweep across the glass.")]
	[Header("Windshield Wipers")]
	public float wiperWipeDuration = 1f;

	[Min(0f)]
	[Tooltip("Pause with the blade parked between sweeps (the intermittent delay).")]
	public float wiperDwellDuration = 0.5f;

	[Tooltip("Random +/- variation added to each dwell so strokes don't feel mechanical.")]
	[Min(0f)]
	public float wiperDwellJitter = 0.15f;

	[Tooltip("How long rain takes to build back up to full after a wipe.")]
	[Min(0.05f)]
	public float wiperRefillDuration = 1.5f;

	[Tooltip("Residual rain left on the glass right after the blade passes.")]
	[Range(0f, 1f)]
	public float wiperRefillMin = 0.1f;

	[Tooltip("Wipe easing power — blade accelerates then decelerates (>1 = stronger ease).")]
	[Range(1f, 5f)]
	public float wiperWipePower = 2f;

	[Tooltip("Refill easing power — fast re-appearance then slow approach to fully wet.")]
	[Range(1f, 5f)]
	public float wiperRefillPower = 2.5f;

	[Range(0f, 1f)]
	[Tooltip("Wipers start once visible rain on the glass (_RainIntensity) rises above this.")]
	public float wiperRainThreshold = 0.05f;

	[Range(0f, 1f)]
	[Tooltip("Strength of the cockpit rain drops on cars with no wipers set up — they get no sweep, so the drops are toned down instead of hidden.")]
	public float noWiperFirstPersonRainBlend = 0.5f;

	[Range(0f, 1f)]
	[Tooltip("Strength of the cockpit rain drops on cars with wipers — the blade already clears the glass, so the residual drops sit below full strength.")]
	public float wiperFirstPersonRainBlend = 0.7f;

	private VehicleData _playerVehicle;

	private GameObject _boundCar;

	private RacingCameraController _racingCameraController;

	private bool _racingCameraLookupDone;

	private int _fpFrame = -1;

	private bool _fpCached;

	private int _wetMaskFrame = -1;

	private float _wetMaskCached = 1f;

	private bool _wipersRunning;

	private WindowComposition.WiperSettings _wiperSettings;

	[Header("Render Features (Auto-Managed)")]
	[SerializeField]
	private RainConeRendererFeature _rainConeFeature;

	[SerializeField]
	private RainDropsCRTFeature _rainDropsCRTFeature;

	[Tooltip("Fullscreen pass compositing ScreenRainRT onto the screen; off outside rain.")]
	[SerializeField]
	private FullScreenPassRendererFeature _screenRainOverlayFeature;

	[SerializeField]
	[Tooltip("Camera rendering screen-drop particles into ScreenRainRT; off in potato and first-person view.")]
	[Header("Screen Rain Simulation (Auto-Managed)")]
	private Camera _screenRainSimCamera;

	[SerializeField]
	[Tooltip("Drop emitter under the sim camera; rate scales with rain intensity.")]
	private ParticleSystem _screenRainEmitter;

	private GameObject _screenRainSimGO;

	private bool _screenRainSimActive;

	private bool _screenRainSimApplied;

	private bool _screenRainOverlayActive;

	private bool _screenRainOverlayApplied;

	private float _screenRainBaseRate = -1f;

	private float _screenRainLastRate = -1f;

	private bool _featuresCurrentlyActive = true;

	private bool _featuresStateApplied;

	private static RainWetnessDriver _instance;

	private static readonly int ID_WetNoiseMap = Shader.PropertyToID("_WetNoiseMap");

	private static readonly int ID_StillDrops = Shader.PropertyToID("_StillDrops");

	private static readonly int ID_FilmWetness = Shader.PropertyToID("_FilmWetness");

	private static readonly int ID_CarFilmWetness = Shader.PropertyToID("_CarFilmWetness");

	private static readonly int ID_PuddleLevel = Shader.PropertyToID("_PuddleLevel");

	private static readonly int ID_RainIntensity = Shader.PropertyToID("_RainIntensity");

	private static readonly int ID_RainConeFade = Shader.PropertyToID("_RainConeFade");

	private static readonly int ID_RainDropsXY = Shader.PropertyToID("_RainDropsXY");

	private static readonly int ID_WindshieldRefractFP = Shader.PropertyToID("_WindshieldRefractFP");

	private static readonly int ID_RainWetnessGate = Shader.PropertyToID("_RainWetnessGate");

	private static readonly int ID_WetDark = Shader.PropertyToID("_WetDark");

	private static readonly int ID_WetSmoothBoost = Shader.PropertyToID("_WetSmoothBoost");

	private static readonly int ID_GeneralWetnessScale = Shader.PropertyToID("_GeneralWetnessScale");

	private static readonly int ID_PuddleExp = Shader.PropertyToID("_PuddleExp");

	private static readonly int ID_PuddleFill = Shader.PropertyToID("_PuddleFill");

	private static readonly int ID_PuddleEdgeSoftness = Shader.PropertyToID("_PuddleEdgeSoftness");

	private static readonly int ID_PuddleDarkExtra = Shader.PropertyToID("_PuddleDarkExtra");

	private static readonly int ID_PuddleNormalFlatten = Shader.PropertyToID("_PuddleNormalFlatten");

	private static readonly int ID_CoatMaskScale = Shader.PropertyToID("_CoatMaskScale");

	private static readonly int ID_CoatSmoothness = Shader.PropertyToID("_CoatSmoothness");

	private static readonly int ID_CoatFilmMul = Shader.PropertyToID("_CoatFilmMul");

	private static readonly int ID_CoatPuddleMul = Shader.PropertyToID("_CoatPuddleMul");

	private static readonly int ID_WetNormalBoostMax = Shader.PropertyToID("_WetNormalBoostMax");

	private static readonly int ID_NormalBoostEarlyK = Shader.PropertyToID("_NormalBoostEarlyK");

	private static readonly int ID_PuddleWetStart = Shader.PropertyToID("_PuddleWetStart");

	private static readonly int ID_PuddleWetPower = Shader.PropertyToID("_PuddleWetPower");

	private static readonly int ID_PuddleKT = Shader.PropertyToID("_PuddleKT");

	private static readonly int ID_SmoothFilmMul = Shader.PropertyToID("_SmoothFilmMul");

	private static readonly int ID_SmoothPuddleMul = Shader.PropertyToID("_SmoothPuddleMul");

	private static readonly int ID_PuddleSlopeStartCos = Shader.PropertyToID("_PuddleSlopeStartCos");

	private static readonly int ID_PuddleSlopeEndCos = Shader.PropertyToID("_PuddleSlopeEndCos");

	private static readonly int ID_PuddleSlopePow = Shader.PropertyToID("_PuddleSlopePow");

	private static GlobalKeyword _kwIsRainWetness;

	private static bool _kwWetInit;

	private static bool _kwWetOn;

	private float _wet;

	private float _film;

	private float _carFilm;

	private float _puddle;

	private float _puddleWet;

	private float _riSlew;

	private float _riSlewVel;

	private float _wetVel;

	private float _filmVel;

	private float _carFilmVel;

	private float _puddleVel;

	private float _puddleWetVel;

	private bool _atRest;

	private bool _potatoMode;

	private bool _rainQualityDirty;

	private static CPMGraphicsSettings.RainQuality _rainQuality = CPMGraphicsSettings.RainQuality.All;

	private float _occlude = 1f;

	private CustomRenderTexture _rainDropsCrt;

	private CustomRenderTexture _rainStreaksCrt;

	private float _wetIncTimeCl;

	private float _wetDryTimeCl;

	private float _carDryTimeCl;

	private float _filmSmoothCl;

	private float _puddleSmoothCl;

	private float _lastPushedPuddle = -1f;

	private float _cachedPuddleKT;

	private float _lastAllFilm = -1f;

	private float _lastAllCarFilm = -1f;

	private float _lastAllRiSlew = -1f;

	private float _lastAllPuddle = -1f;

	private float _lastAllOcclude = -1f;

	private float _lastWindshieldRefractFP = -1f;

	private WetParamsProfile _lastProfile;

	private int _lastProfileRevision = -1;

	private static int _puddleCacheFrame = -1;

	private static Vector3 _puddleCachePos;

	private static float _puddleCacheVal;

	private const float PuddleCacheRadiusSqr = 16f;

	public static bool HasActiveDriver => _instance != null;

	private static bool DropEffectsEnabled => _rainQuality != CPMGraphicsSettings.RainQuality.Off;

	private static bool SurfaceDropsEnabled => _rainQuality == CPMGraphicsSettings.RainQuality.All;

	public float WeatherIntensity => rainIntensity;

	public static float CurrentIntensity
	{
		get
		{
			if (!_instance)
			{
				return 0f;
			}
			return _instance.rainIntensity;
		}
	}

	public static float CurrentPuddleLevel
	{
		get
		{
			if (!_instance)
			{
				return 0f;
			}
			return _instance._puddle;
		}
	}

	private static RainSound RainSoundService
	{
		get
		{
			if (!V.CoreGameManager)
			{
				return null;
			}
			return V.CoreGameManager.RainSound;
		}
	}

	private static void SetWetnessKeyword(bool on)
	{
		if (!_kwWetInit)
		{
			_kwIsRainWetness = GlobalKeyword.Create("_ISRAINWETNESS");
		}
		else if (_kwWetOn == on)
		{
			return;
		}
		_kwWetInit = true;
		_kwWetOn = on;
		Shader.SetKeyword(in _kwIsRainWetness, on);
		Shader.SetGlobalFloat(ID_RainWetnessGate, on ? 1f : 0f);
	}

	private void OnEnable()
	{
		_instance = this;
		_wet = (_film = (_carFilm = (_puddle = (_puddleWet = 0f))));
		_riSlew = (_riSlewVel = 0f);
		_wetVel = (_filmVel = (_carFilmVel = (_puddleVel = (_puddleWetVel = 0f))));
		WeatherPhysics.Wetness = 0f;
		_occlude = 1f;
		_lastPushedPuddle = -1f;
		_lastAllFilm = (_lastAllCarFilm = (_lastAllRiSlew = (_lastAllPuddle = (_lastAllOcclude = -1f))));
		_lastWindshieldRefractFP = -1f;
		_atRest = false;
		_featuresCurrentlyActive = true;
		_featuresStateApplied = false;
		_potatoMode = Shader.IsKeywordEnabled("_QUALITY_POTATO");
		_screenRainSimGO = ((_screenRainSimCamera != null) ? _screenRainSimCamera.gameObject : null);
		_screenRainSimActive = false;
		_screenRainSimApplied = false;
		_screenRainOverlayActive = false;
		_screenRainOverlayApplied = false;
		_racingCameraController = null;
		_racingCameraLookupDone = false;
		Shader.SetGlobalFloat(ID_RainConeFade, 1f);
		CacheTimings();
		CacheWiperSettings();
		CacheAutomotiveCrts();
		PushStatic();
		PushDynamics();
		PushAll();
		_rainQualityDirty = true;
		ApplyPotatoSettings();
		SetWetnessKeyword(on: false);
		_lastProfile = profile;
		_lastProfileRevision = (profile ? profile.revision : (-1));
		if (Application.isPlaying)
		{
			V.OnCarEntered += OnPlayerCarEntered;
			V.OnCarExit += OnPlayerCarExit;
			BindPlayerCar(V.Car);
		}
	}

	private void OnDisable()
	{
		if (_instance == this)
		{
			_instance = null;
		}
		RainSoundService?.StopAll();
		WeatherPhysics.Wetness = 0f;
		V.OnCarEntered -= OnPlayerCarEntered;
		V.OnCarExit -= OnPlayerCarExit;
		_playerVehicle?.WindowComposition?.SetGlassDoubleSided(doubleSided: false);
		_playerVehicle = null;
		_boundCar = null;
		_wipersRunning = false;
		_screenRainOverlayFeature?.SetActive(active: false);
		if (Application.isPlaying && _screenRainSimGO != null)
		{
			if (_screenRainSimGO.activeSelf)
			{
				_screenRainSimGO.SetActive(value: false);
				ClearScreenRainRT();
			}
			if (_screenRainBaseRate >= 0f && _screenRainEmitter != null)
			{
				ParticleSystem.EmissionModule emission = _screenRainEmitter.emission;
				emission.rateOverTimeMultiplier = _screenRainBaseRate;
				emission.enabled = true;
				_screenRainBaseRate = -1f;
				_screenRainLastRate = -1f;
			}
		}
	}

	private void OnPlayerCarEntered(GameObject person, VehicleData vehicleData)
	{
		_playerVehicle = vehicleData;
		_boundCar = ((vehicleData != null) ? vehicleData.gameObject : V.Car);
	}

	private void OnPlayerCarExit(GameObject person, VehicleData vehicleData)
	{
		if (_wipersRunning)
		{
			_playerVehicle?.WindowComposition?.StopWipers();
		}
		_playerVehicle?.WindowComposition?.SetGlassDoubleSided(doubleSided: false);
		_playerVehicle = null;
		_boundCar = null;
		_wipersRunning = false;
	}

	private void BindPlayerCar(GameObject car)
	{
		_boundCar = car;
		_playerVehicle = ((car != null) ? car.GetComponent<VehicleData>() : null);
	}

	private void DriveWipers(float dt, bool rainOnGlass)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (V.Car != _boundCar)
		{
			BindPlayerCar(V.Car);
		}
		bool flag = IsFirstPersonCameraActive();
		float num = (flag ? 1f : 0f);
		if (num != _lastWindshieldRefractFP)
		{
			_lastWindshieldRefractFP = num;
			Shader.SetGlobalFloat(ID_WindshieldRefractFP, num);
		}
		WindowComposition windowComposition = ((_playerVehicle != null) ? _playerVehicle.WindowComposition : null);
		if (windowComposition == null)
		{
			_wipersRunning = false;
			return;
		}
		bool hasPhysicalWipers = windowComposition.HasPhysicalWipers;
		windowComposition.SetGlassDoubleSided(_kwWetOn & flag);
		windowComposition.SetFirstPersonRainBlend(hasPhysicalWipers ? wiperFirstPersonRainBlend : noWiperFirstPersonRainBlend);
		if (!hasPhysicalWipers)
		{
			windowComposition.ClearWipers();
			_wipersRunning = false;
		}
		else if (rainOnGlass)
		{
			windowComposition.TickWipers(dt, in _wiperSettings);
			_wipersRunning = true;
		}
		else if (_wipersRunning && windowComposition.TickWiperStop(dt, in _wiperSettings))
		{
			_wipersRunning = false;
		}
	}

	private void OnValidate()
	{
		_lastPushedPuddle = -1f;
		_lastAllFilm = (_lastAllCarFilm = (_lastAllRiSlew = (_lastAllPuddle = (_lastAllOcclude = -1f))));
		CacheTimings();
		CacheWiperSettings();
		CacheAutomotiveCrts();
		PushStatic();
		PushDynamics();
		PushAll();
		_lastProfile = profile;
		_lastProfileRevision = (profile ? profile.revision : (-1));
	}

	private void CacheTimings()
	{
		_wetIncTimeCl = Mathf.Max(0.0001f, wetIncreaseTime);
		_wetDryTimeCl = Mathf.Max(0.0001f, wetDryTime);
		_carDryTimeCl = Mathf.Max(0.0001f, carDryTime);
		_filmSmoothCl = Mathf.Max(0.0001f, filmSmoothingTime);
		_puddleSmoothCl = Mathf.Max(0.0001f, puddleSmoothingTime);
	}

	private void CacheWiperSettings()
	{
		_wiperSettings = new WindowComposition.WiperSettings(wiperWipeDuration, wiperDwellDuration, wiperDwellJitter, wiperRefillDuration, wiperRefillMin, wiperWipePower, wiperRefillPower);
	}

	private void SetRainFeaturesActive(bool active)
	{
		if (!_featuresStateApplied || _featuresCurrentlyActive != active)
		{
			_featuresStateApplied = true;
			_featuresCurrentlyActive = active;
			_rainConeFeature?.SetActive(active && DropEffectsEnabled);
			_rainDropsCRTFeature?.SetActive(active && !_potatoMode && DropEffectsEnabled);
		}
	}

	public static bool ShouldEnableScreenRainSimulation(bool rainActive, bool potatoMode, bool firstPersonView)
	{
		if (rainActive && !potatoMode && !firstPersonView)
		{
			return SurfaceDropsEnabled;
		}
		return false;
	}

	private void CacheRacingCamera()
	{
		if (!_racingCameraLookupDone && !(V.CoreGameManager == null))
		{
			_racingCameraLookupDone = true;
			V.CoreGameManager.TryGetComponent<RacingCameraController>(out _racingCameraController);
		}
	}

	private bool IsFirstPersonCameraActive()
	{
		if (Application.isPlaying && _fpFrame == Time.frameCount)
		{
			return _fpCached;
		}
		_fpCached = ComputeFirstPersonCameraActive();
		_fpFrame = Time.frameCount;
		return _fpCached;
	}

	private float CarWetMask()
	{
		if (!Application.isPlaying || _boundCar == null)
		{
			return 1f;
		}
		if (_wetMaskFrame == Time.frameCount)
		{
			return _wetMaskCached;
		}
		_wetMaskCached = RainDryZoneManager.WetMaskAt(_boundCar.transform.position);
		_wetMaskFrame = Time.frameCount;
		return _wetMaskCached;
	}

	private bool ComputeFirstPersonCameraActive()
	{
		CacheRacingCamera();
		RacingProvider racingProvider = ((_playerVehicle != null) ? _playerVehicle.RacingProvider : null);
		DragRacingCamera dragRacingCamera = racingProvider?.DragRacingController?.dragRacingCamera;
		if (dragRacingCamera != null && dragRacingCamera.isActiveAndEnabled)
		{
			return dragRacingCamera.currentCamera == DragRacingCamera.CameraType.driverView;
		}
		RacingCameraController racingCameraController = racingProvider?.RacingCameraController ?? _racingCameraController;
		if (racingCameraController != null && racingCameraController.isActiveAndEnabled)
		{
			return racingCameraController.currentCamera == DragRacingCamera.CameraType.driverView;
		}
		CarCamerasController carCamerasController = ((V.CoreGameManager != null) ? V.CoreGameManager.carCamerasController : null);
		if (carCamerasController != null)
		{
			return carCamerasController.IsFirstPersonView;
		}
		return false;
	}

	private void UpdateScreenRainSim(bool rainActive)
	{
		bool flag = _screenRainSimCamera != null;
		CarCamerasController carCamerasController = ((V.CoreGameManager != null) ? V.CoreGameManager.carCamerasController : null);
		bool flag2 = IsFirstPersonCameraActive() || (carCamerasController != null && carCamerasController.IsFirstPersonView);
		bool firstPersonView = (((rainActive && !_potatoMode) & flag) && Application.isPlaying) & flag2;
		bool flag3 = flag && ShouldEnableScreenRainSimulation(rainActive, _potatoMode, firstPersonView);
		if (!_screenRainOverlayApplied || _screenRainOverlayActive != flag3)
		{
			_screenRainOverlayApplied = true;
			_screenRainOverlayActive = flag3;
			_screenRainOverlayFeature?.SetActive(flag3);
		}
		if (!Application.isPlaying || _screenRainSimGO == null)
		{
			return;
		}
		if (!_screenRainSimApplied || _screenRainSimActive != flag3)
		{
			_screenRainSimApplied = true;
			_screenRainSimActive = flag3;
			_screenRainSimGO.SetActive(flag3);
			if (!flag3)
			{
				ClearScreenRainRT();
			}
		}
		if (!flag3 || !(_screenRainEmitter != null))
		{
			return;
		}
		float num = _riSlew * CarWetMask();
		if (Mathf.Abs(num - _screenRainLastRate) > 0.02f || (num <= 0f && _screenRainLastRate > 0f))
		{
			_screenRainLastRate = num;
			ParticleSystem.EmissionModule emission = _screenRainEmitter.emission;
			if (_screenRainBaseRate < 0f)
			{
				_screenRainBaseRate = emission.rateOverTimeMultiplier;
			}
			if (emission.enabled = num > 0f)
			{
				emission.rateOverTimeMultiplier = _screenRainBaseRate * num;
			}
		}
	}

	private void ClearScreenRainRT()
	{
		RenderTexture renderTexture = ((_screenRainSimCamera != null) ? _screenRainSimCamera.targetTexture : null);
		if (!(renderTexture == null))
		{
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = renderTexture;
			GL.Clear(clearDepth: false, clearColor: true, Color.clear);
			RenderTexture.active = active;
		}
	}

	private void ApplyPotatoSettings()
	{
		if (_rainConeFeature != null)
		{
			_rainConeFeature.settings.RequestDepthTexture = !_potatoMode;
		}
		if (_featuresCurrentlyActive)
		{
			_rainDropsCRTFeature?.SetActive(!_potatoMode && DropEffectsEnabled);
		}
	}

	public static void ApplyRainQuality(CPMGraphicsSettings.RainQuality quality)
	{
		_rainQuality = quality;
		if (_instance != null)
		{
			_instance._rainQualityDirty = true;
		}
	}

	private void ApplyRainQualityChange()
	{
		_rainQualityDirty = false;
		_featuresStateApplied = false;
		_screenRainSimApplied = false;
		_screenRainOverlayApplied = false;
		ApplyPotatoSettings();
		if (!DropEffectsEnabled)
		{
			NeutralizeRippleTexture();
		}
	}

	private void NeutralizeRippleTexture()
	{
		Shader.SetGlobalTexture((_rainDropsCRTFeature != null) ? Shader.PropertyToID(_rainDropsCRTFeature.settings.globalName) : ID_RainDropsXY, Texture2D.linearGrayTexture);
	}

	public static void ApplyQualityPreset(CPMGraphicsSettings.ShadersQuality quality)
	{
		if (!(_instance == null))
		{
			_instance._potatoMode = quality == CPMGraphicsSettings.ShadersQuality.Potato;
			_instance.ApplyPotatoSettings();
		}
	}

	private void Update()
	{
		if (profile != _lastProfile || ((bool)profile && profile.revision != _lastProfileRevision))
		{
			PushStatic();
			_lastProfile = profile;
			_lastProfileRevision = (profile ? profile.revision : (-1));
		}
		float num = Time.deltaTime;
		if (num <= 0f)
		{
			num = 0.0001f;
		}
		float num2 = Mathf.Clamp01(rainIntensity);
		if (num2 < 0.001f)
		{
			num2 = 0f;
		}
		if (_rainQualityDirty)
		{
			ApplyRainQualityChange();
		}
		bool flag = _riSlew >= 0.001f || num2 > 0f;
		SetRainFeaturesActive(flag);
		UpdateScreenRainSim(flag);
		RainSoundService?.Tick(flag && DropEffectsEnabled, _riSlew);
		if (_riSlew < 0.0001f && num2 < 0.0001f && _wet < 0.0001f && _film < 0.0001f && _carFilm < 0.0001f && _puddle < 0.0001f && _puddleWet < 0.0001f)
		{
			if (!_atRest)
			{
				_wet = (_film = (_carFilm = (_puddle = (_puddleWet = (_riSlew = 0f)))));
				_wetVel = (_filmVel = (_carFilmVel = (_puddleVel = (_puddleWetVel = (_riSlewVel = 0f)))));
				WeatherPhysics.Wetness = 0f;
				_lastAllFilm = (_lastAllCarFilm = (_lastAllRiSlew = (_lastAllPuddle = (_lastAllOcclude = -1f))));
				_lastPushedPuddle = -1f;
				PushAll();
				PushDynamics();
				SetWetnessKeyword(on: false);
				_atRest = true;
			}
			DriveWipers(num, rainOnGlass: false);
		}
		else
		{
			_atRest = false;
			SetWetnessKeyword(on: true);
			_riSlew = ((intensitySmoothTime > 0f) ? SmoothDampUnit(_riSlew, num2, ref _riSlewVel, intensitySmoothTime, num) : num2);
			float num3 = ((_riSlew > wetnessThreshold) ? 1f : 0f);
			_wet = SmoothDampUnit(_wet, num3, ref _wetVel, (num3 > _wet) ? _wetIncTimeCl : _wetDryTimeCl, num);
			float num4 = CarWetMask();
			WeatherPhysics.Wetness = _wet * num4;
			_film = SmoothDampUnit(_film, _wet, ref _filmVel, _filmSmoothCl, num);
			float num5 = _riSlew * num4;
			_carFilm = SmoothDampUnit(_carFilm, num5, ref _carFilmVel, (num5 > _carFilm) ? _wetIncTimeCl : _carDryTimeCl, num);
			float num6 = num3 * Mathf.Lerp(1f, _riSlew, puddleIntensityInfluence);
			_puddleWet = SmoothDampUnit(_puddleWet, num6, ref _puddleWetVel, (num6 > _puddleWet) ? _wetIncTimeCl : _wetDryTimeCl, num);
			_puddle = SmoothDampUnit(_puddle, _puddleWet, ref _puddleVel, _puddleSmoothCl, num);
			PushAll();
			PushDynamics();
			TickAutomotiveCrts();
			DriveWipers(num, _riSlew * num4 > wiperRainThreshold);
		}
	}

	private void CacheAutomotiveCrts()
	{
		_rainDropsCrt = rainDrops as CustomRenderTexture;
		_rainStreaksCrt = rainStreaks as CustomRenderTexture;
		if (_rainDropsCrt != null)
		{
			_rainDropsCrt.updateMode = CustomRenderTextureUpdateMode.OnDemand;
		}
		if (_rainStreaksCrt != null)
		{
			_rainStreaksCrt.updateMode = CustomRenderTextureUpdateMode.OnDemand;
		}
	}

	private void TickAutomotiveCrts()
	{
		if (!(_riSlew <= 0.0001f) || !(_carFilm <= 0.0001f))
		{
			if (_rainDropsCrt != null)
			{
				_rainDropsCrt.Update();
			}
			if (_rainStreaksCrt != null)
			{
				_rainStreaksCrt.Update();
			}
		}
	}

	private void PushAll()
	{
		if (!(Mathf.Abs(_film - _lastAllFilm) < 0.0005f) || !(Mathf.Abs(_carFilm - _lastAllCarFilm) < 0.0005f) || !(Mathf.Abs(_riSlew - _lastAllRiSlew) < 0.0005f) || !(Mathf.Abs(_puddle - _lastAllPuddle) < 0.0005f))
		{
			_lastAllFilm = _film;
			_lastAllCarFilm = _carFilm;
			_lastAllRiSlew = _riSlew;
			_lastAllPuddle = _puddle;
			Shader.SetGlobalFloat(ID_FilmWetness, _film);
			Shader.SetGlobalFloat(ID_CarFilmWetness, _carFilm);
			Shader.SetGlobalFloat(ID_PuddleLevel, _puddle);
			Shader.SetGlobalFloat(ID_RainIntensity, _riSlew);
		}
	}

	private void PushStatic()
	{
		if ((bool)profile)
		{
			if (wetNoiseMap != null)
			{
				Shader.SetGlobalTexture(ID_WetNoiseMap, wetNoiseMap);
			}
			if (stillDrops != null)
			{
				Shader.SetGlobalTexture(ID_StillDrops, stillDrops);
			}
			Shader.SetGlobalFloat(ID_WetDark, kDark);
			Shader.SetGlobalFloat(ID_WetSmoothBoost, kSmoothBoost);
			Shader.SetGlobalFloat(ID_GeneralWetnessScale, profile.GeneralWetnessScale);
			Shader.SetGlobalFloat(ID_PuddleExp, profile.PuddleExp);
			Shader.SetGlobalFloat(ID_PuddleEdgeSoftness, profile.PuddleEdgeSoftness);
			Shader.SetGlobalFloat(ID_PuddleDarkExtra, profile.PuddleDarkExtra);
			Shader.SetGlobalFloat(ID_PuddleNormalFlatten, profile.PuddleNormalFlatten);
			Shader.SetGlobalFloat(ID_CoatMaskScale, profile.CoatMaskScale);
			Shader.SetGlobalFloat(ID_CoatSmoothness, profile.CoatSmoothness);
			Shader.SetGlobalFloat(ID_CoatFilmMul, profile.CoatFilmMul);
			Shader.SetGlobalFloat(ID_CoatPuddleMul, profile.CoatPuddleMul);
			Shader.SetGlobalFloat(ID_WetNormalBoostMax, profile.NormalBoostMax);
			Shader.SetGlobalFloat(ID_NormalBoostEarlyK, profile.NormalBoostEarlyK);
			Shader.SetGlobalFloat(ID_PuddleWetStart, profile.PuddleWetStart);
			Shader.SetGlobalFloat(ID_PuddleWetPower, profile.PuddleWetPower);
			Shader.SetGlobalFloat(ID_SmoothFilmMul, profile.SmoothFilmMul);
			Shader.SetGlobalFloat(ID_SmoothPuddleMul, profile.SmoothPuddleMul);
			RainDryZoneManager.EdgeFeather = profile.DryZoneFeather;
			RainDryZoneManager.EdgeGamma = profile.DryZoneEdgeGamma;
			RainDryZoneManager.EdgeWobble = profile.DryZoneEdgeWobble;
			RainDryZoneManager.WobbleFrequency = profile.DryZoneWobbleFreq;
			float num = Mathf.Cos(profile.PuddleSlopeStartDeg * ((float)Math.PI / 180f));
			float num2 = Mathf.Cos(profile.PuddleSlopeEndDeg * ((float)Math.PI / 180f));
			if (num2 > num)
			{
				float num3 = num;
				num = num2;
				num2 = num3;
			}
			Shader.SetGlobalFloat(ID_PuddleSlopeStartCos, num);
			Shader.SetGlobalFloat(ID_PuddleSlopeEndCos, num2);
			Shader.SetGlobalFloat(ID_PuddleSlopePow, Mathf.Max(0.5f, profile.PuddleSlopePow));
		}
	}

	private void PushDynamics()
	{
		if ((bool)profile && !(Mathf.Abs(_puddle - _lastPushedPuddle) < 0.0005f))
		{
			_lastPushedPuddle = _puddle;
			float num = Mathf.Clamp01(_puddle);
			Shader.SetGlobalFloat(ID_PuddleFill, profile.PuddleFill * Mathf.Pow(num, profile.PuddleWetPower));
			float f = Mathf.Clamp01((num - profile.PuddleWetStart) / Mathf.Max(1E-06f, 1f - profile.PuddleWetStart));
			_cachedPuddleKT = Mathf.Pow(f, Mathf.Max(0.001f, profile.PuddleWetPower));
			if (puddleDryTailStart > 0f)
			{
				float num2 = Mathf.Clamp01(num / puddleDryTailStart);
				_cachedPuddleKT *= num2 * num2 * (3f - 2f * num2);
			}
			Shader.SetGlobalFloat(ID_PuddleKT, _cachedPuddleKT);
		}
	}

	private static float SmoothDampUnit(float cur, float target, ref float vel, float smoothTime, float dt)
	{
		smoothTime = Mathf.Max(0.0001f, smoothTime);
		float num = 2f / smoothTime;
		float num2 = num * dt;
		float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
		float num4 = cur - target;
		float num5 = (vel + num * num4) * dt;
		vel = (vel - num * num5) * num3;
		float num6 = target + (num4 + num5) * num3;
		if (num6 < 0f)
		{
			num6 = 0f;
			vel = 0f;
		}
		else if (num6 > 1f)
		{
			num6 = 1f;
			vel = 0f;
		}
		return num6;
	}

	public void SetWeatherIntensity(float intensity)
	{
		rainIntensity = Mathf.Clamp01(intensity);
	}

	public void RequestRain(float targetIntensity)
	{
		SetWeatherIntensity(targetIntensity);
	}

	public void StopRain()
	{
		SetWeatherIntensity(0f);
	}

	public static void SetIntensity(float intensity)
	{
		SunAndSkyController sunAndSkyController = GlobalEventSystem.RequestSunController();
		if (sunAndSkyController != null)
		{
			sunAndSkyController.SetRainIntensity(intensity);
		}
		else
		{
			_instance?.SetWeatherIntensity(intensity);
		}
	}

	public static void ClearForGarage()
	{
		RainSoundService?.StopAll();
		Shader.SetGlobalFloat(ID_RainIntensity, 0f);
		Shader.SetGlobalFloat(ID_FilmWetness, 0f);
		Shader.SetGlobalFloat(ID_CarFilmWetness, 0f);
		Shader.SetGlobalFloat(ID_PuddleLevel, 0f);
		Shader.SetGlobalFloat(ID_PuddleKT, 0f);
		Shader.SetGlobalFloat(ID_PuddleFill, 0f);
		SetWetnessKeyword(on: false);
		WeatherPhysics.Wetness = 0f;
		RainWetnessDriver instance = _instance;
		if (!(instance == null))
		{
			bool flag = instance.rainIntensity > 0f;
			SetIntensity(0f);
			instance._wet = (instance._film = (instance._carFilm = (instance._puddle = (instance._puddleWet = 0f))));
			instance._riSlew = (instance._riSlewVel = 0f);
			instance._wetVel = (instance._filmVel = (instance._carFilmVel = (instance._puddleVel = (instance._puddleWetVel = 0f))));
			instance._lastAllFilm = (instance._lastAllCarFilm = (instance._lastAllRiSlew = (instance._lastAllPuddle = (instance._lastAllOcclude = -1f))));
			instance._lastPushedPuddle = -1f;
			instance.PushAll();
			instance.PushDynamics();
			if (flag)
			{
				CPMGraphicsSettingsManager.Instance?.SyncCloudiness(0f);
			}
		}
	}

	public static float SamplePuddleAt(Vector3 worldPos)
	{
		RainWetnessDriver instance = _instance;
		if (instance == null || instance.wetNoiseMap == null || !instance.wetNoiseMap.isReadable || instance._cachedPuddleKT < 1E-05f)
		{
			return 0f;
		}
		float num = RainDryZoneManager.WetMaskAt(worldPos);
		if (num < 0.05f)
		{
			return 0f;
		}
		Vector3 globalOffset = V.GlobalOffset;
		float num2 = (globalOffset.x - worldPos.x) * 0.05f;
		float num3 = (globalOffset.z - worldPos.z) * 0.05f;
		num2 -= Mathf.Floor(num2);
		num3 -= Mathf.Floor(num3);
		float t = Mathf.Pow(Mathf.Clamp01(instance.wetNoiseMap.GetPixelBilinear(num2, num3).r), instance.profile.PuddleExp);
		float num4 = Mathf.SmoothStep(0f, instance.profile.PuddleEdgeSoftness, t);
		return Mathf.Clamp01(instance._cachedPuddleKT * num4) * num;
	}

	public static float SamplePuddleAtCached(Vector3 worldPos)
	{
		int frameCount = Time.frameCount;
		if (frameCount == _puddleCacheFrame && (worldPos - _puddleCachePos).sqrMagnitude <= 16f)
		{
			return _puddleCacheVal;
		}
		_puddleCacheFrame = frameCount;
		_puddleCachePos = worldPos;
		_puddleCacheVal = SamplePuddleAt(worldPos);
		return _puddleCacheVal;
	}

	public static ParticleSystem GetSplashPS()
	{
		if (!DropEffectsEnabled)
		{
			return null;
		}
		return _instance?.puddleSplashParticle;
	}

	public static void SetRainActive(bool active)
	{
		SetIntensity(active ? 0.6f : 0f);
	}
}
