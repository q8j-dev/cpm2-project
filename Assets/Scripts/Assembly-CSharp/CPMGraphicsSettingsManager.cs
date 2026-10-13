using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.GraphicsSettings;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.PhotoMode;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Steam.Perf;
using _Car_Parking.Scripts.Steam.Traffic;

public class CPMGraphicsSettingsManager : MonoBehaviour
{
	public static CPMGraphicsSettingsManager Instance;

	[HideInInspector]
	public CPMGraphicsSettingsData _graphicsSettings;

	public Texture2D[] ColorCorrectionLUTS;

	public UniversalRendererData UrpRenderer;

	public ScriptableRendererFeature Decals;

	public ScriptableRendererFeature blur;

	public ScriptableRendererFeature Sky;

	public ScriptableRendererFeature fog;

	public UniversalRenderPipelineAsset mainPipeLine;

	public Texture2D BlurFallBackTex;

	private Camera _mainCam;

	private ReflectionProbe _realtimeProbe;

	private string _configPath;

	private const string ConfigVersion = "0.9.5";

	private const int HighFpsDefault = 240;

	public CPMGraphicsSettingsDefaultPresets m_Presets;

	public VolumeProfile m_DefaultVolume;

	public List<ReflectionProbe> ReflectionProbes = new List<ReflectionProbe>();

	private SunAndSkyController SunController;

	public LayerMask RealtimeReflectionLayerMask;

	private int _quality;

	private int _displayMode;

	private FullScreenMode _fullScreenMode;

	private int[] _resolution;

	private int _displayIndex;

	private CoreGameManager _coreGameManager;

	private LevelSceneControl _levelSceneControl;

	private LevelsProvider _levelsProvider;

	private Camera ReflectionCamera;

	private RenderTexture ReflectionCube;

	private int ReflectionCubeCycles;

	private bool UseShaderRealtimeReflection;

	public bool RenderReflections;

	public Material SkyboxFallBack;

	private float TimeOfDayExposure;

	private float TimeOfDayWHiteBallance;

	[HideInInspector]
	public float AcesAdjustment;

	private const int LowMemoryRamIos = 4100;

	private const int VeryLowMemoryRamIos = 2100;

	private const int LayerFarClipIndex = 10;

	private const float MinPropsDist = 15f;

	private const float MaxPropsDist = 120f;

	private const float MinLodBias = 0.4f;

	[HideInInspector]
	public readonly Dictionary<int, GraphicsQuality> _graphicsSettingsByIndex = new Dictionary<int, GraphicsQuality>
	{
		{
			0,
			GraphicsQuality.Fast
		},
		{
			1,
			GraphicsQuality.Medium
		},
		{
			2,
			GraphicsQuality.High
		},
		{
			3,
			GraphicsQuality.Ultra
		},
		{
			4,
			GraphicsQuality.PCUltra
		}
	};

	public List<Resolution> _availableResolutions = new List<Resolution>();

	public const int UltraPresetIndex = 3;

	private bool FirstLaunch = true;

	private RenderDistanceController _renderDistanceController = new RenderDistanceController();

	private CamerasRenderDistance _camerasRenderDistance = new CamerasRenderDistance();

	private RenderPropsDistanceController _renderPropsDistanceController = new RenderPropsDistanceController();

	private PhotoModeConfig _config;

	private PerformanceDetector _performanceDetector;

	private static readonly List<DisplayInfo> _displaysBuffer = new List<DisplayInfo>();

	private const string TyreDeformKeyword = "_TYREDEFORM";

	public static readonly int ExposureWB = Shader.PropertyToID("_GlobalExposureWB");

	public RenderDistanceController RenderDistanceController => _renderDistanceController;

	public CamerasRenderDistance CamerasRenderDistance => _camerasRenderDistance;

	public RenderPropsDistanceController RenderPropsDistanceController => _renderPropsDistanceController;

	public int Quality
	{
		get
		{
			return _quality;
		}
		private set
		{
			_quality = value;
			OnQualityChanged?.Invoke(value);
		}
	}

	public FullScreenMode DisplayMode
	{
		get
		{
			return _fullScreenMode;
		}
		set
		{
			_fullScreenMode = value;
			ApplyDisplayMode();
		}
	}

	public int DisplayIndex
	{
		get
		{
			return _displayIndex;
		}
		set
		{
			_displayIndex = value;
			ApplyDisplay();
		}
	}

	public int[] Resolution
	{
		get
		{
			return _resolution;
		}
		set
		{
			_resolution = value;
			ApplyResolution();
		}
	}

	public float RenderScale
	{
		get
		{
			return _graphicsSettings.m_RenderScale;
		}
		set
		{
			_graphicsSettings.m_RenderScale = value;
			ApplyRenderScale();
		}
	}

	public CPMGraphicsSettings.AAQuality AAQuality
	{
		get
		{
			return _graphicsSettings.m_AAQuality;
		}
		set
		{
			_graphicsSettings.m_AAQuality = value;
			ApplyAA();
			ApplyPostProcessing();
		}
	}

	public CPMGraphicsSettings.TextureResolution TextureResolution
	{
		get
		{
			return _graphicsSettings.m_TextureResolution;
		}
		set
		{
			_graphicsSettings.m_TextureResolution = value;
			ApplyTextureResolution();
		}
	}

	public int MaxFramerate
	{
		get
		{
			return _graphicsSettings.m_MaxFramerate;
		}
		set
		{
			_graphicsSettings.m_MaxFramerate = value;
			ApplyMaxFrameRate();
		}
	}

	public bool VSync
	{
		get
		{
			return _graphicsSettings.m_VSync;
		}
		set
		{
			_graphicsSettings.m_VSync = value;
			ApplyVSync();
		}
	}

	public bool BatterySaveMode
	{
		get
		{
			return _graphicsSettings.BatterySaveMode;
		}
		set
		{
			_graphicsSettings.BatterySaveMode = value;
			ApplyBatterySaveMode();
		}
	}

	public float CameraRenderDistance => _renderDistanceController.Distance;

	public float ObjectsLodQuality
	{
		get
		{
			return _graphicsSettings.m_ObjectsLodQuality;
		}
		set
		{
			_graphicsSettings.m_ObjectsLodQuality = value;
			ApplyObjectsLodQuality();
		}
	}

	public float NightAmbientLightCompensation
	{
		get
		{
			return _graphicsSettings.m_NightAmbientLightingCompensation;
		}
		set
		{
			_graphicsSettings.m_NightAmbientLightingCompensation = value;
			ApplyNightAmbientLightCompensation();
		}
	}

	public bool ConfigLoaded => _graphicsSettings != null;

	public float Cloudiness
	{
		get
		{
			return _graphicsSettings.m_Cloudiness;
		}
		set
		{
			_graphicsSettings.m_Cloudiness = value;
			ApplyCloudiness();
		}
	}

	public CPMGraphicsSettings.ShadersQuality ShadingQuality
	{
		get
		{
			return _graphicsSettings.m_ShadingQuality;
		}
		set
		{
			_graphicsSettings.m_ShadingQuality = value;
			ApplyShadingQuality();
			ApplyOpaqueTexture();
		}
	}

	public CPMGraphicsSettings.ShadowsQuality ShadowsQuality
	{
		get
		{
			return _graphicsSettings.m_ShadowsQuality;
		}
		set
		{
			_graphicsSettings.m_ShadowsQuality = value;
			ApplyShadowsQuality();
			ApplyCameraShadowsQuality();
		}
	}

	public bool UseSky
	{
		get
		{
			return _graphicsSettings.m_UseSky;
		}
		set
		{
			_graphicsSettings.m_UseSky = value;
			Sky.SetActive(_graphicsSettings.m_UseSky);
		}
	}

	public bool UseBlur
	{
		get
		{
			return _graphicsSettings.m_UseBlur;
		}
		set
		{
			_graphicsSettings.m_UseBlur = value;
			ApplyBlur();
			ApplyPostProcessing();
		}
	}

	public bool UseTyreDeformation
	{
		get
		{
			return _graphicsSettings.m_UseTyreDeformation;
		}
		set
		{
			if (_graphicsSettings.m_UseTyreDeformation != value)
			{
				_graphicsSettings.m_UseTyreDeformation = value;
				ApplyTyreDeformation();
				OnTyreDeformationChanged?.Invoke(value);
				ApplyTyreDeformation();
			}
		}
	}

	public CPMGraphicsSettings.RainQuality RainQuality
	{
		get
		{
			return _graphicsSettings.m_RainQuality;
		}
		set
		{
			_graphicsSettings.m_RainQuality = value;
			ApplyRainQuality();
		}
	}

	public bool UseDecalShadows
	{
		get
		{
			return _graphicsSettings.m_UseDecalShadows;
		}
		set
		{
			_graphicsSettings.m_UseDecalShadows = value;
			ApplyCarGroundShadow();
		}
	}

	public bool UseLensFlares
	{
		get
		{
			return _graphicsSettings.m_UseLensFlares;
		}
		set
		{
			_graphicsSettings.m_UseLensFlares = value;
			ApplyLensFlares();
		}
	}

	public bool UseStaticReflections
	{
		get
		{
			return _graphicsSettings.m_UseStaticReflectons;
		}
		set
		{
			_graphicsSettings.m_UseStaticReflectons = value;
			ApplyStaticReflections();
		}
	}

	public float ClipDistance => _renderPropsDistanceController.Distances.Item1;

	public float CarDetailDistance
	{
		get
		{
			return _graphicsSettings.CarDetailDistance;
		}
		set
		{
			_graphicsSettings.CarDetailDistance = value;
			ApplyDetailDistance();
		}
	}

	public CPMGraphicsSettings.AdditionalEffects AdditionalEffects
	{
		get
		{
			return _graphicsSettings.m_AdditionalEffects;
		}
		set
		{
			_graphicsSettings.m_AdditionalEffects = value;
			ApplyAdditionalEffects();
		}
	}

	public CPMGraphicsSettings.ColorCorrectionLut UseColorCorrectionLut
	{
		get
		{
			return _graphicsSettings.m_ColorCorrectionLut;
		}
		set
		{
			_graphicsSettings.m_ColorCorrectionLut = value;
			ApplyColorCorrection();
			ApplyPostProcessing();
		}
	}

	public bool UseVignette
	{
		get
		{
			return _graphicsSettings.m_UseVignette;
		}
		set
		{
			_graphicsSettings.m_UseVignette = value;
			ApplyVignette();
			ApplyPostProcessing();
		}
	}

	public bool UseMotionBlur
	{
		get
		{
			return _graphicsSettings.m_UseMotionBlur;
		}
		set
		{
			_graphicsSettings.m_UseMotionBlur = value;
			ApplyMotionBlur();
			ApplyPostProcessing();
		}
	}

	public TonemappingMode Tonemapping
	{
		get
		{
			return _graphicsSettings.m_Tonemapping;
		}
		set
		{
			_graphicsSettings.m_Tonemapping = value;
			ApplyTonemapping();
			ApplyPostProcessing();
		}
	}

	public bool UseBloom
	{
		get
		{
			return _graphicsSettings.m_UseBloom;
		}
		set
		{
			_graphicsSettings.m_UseBloom = value;
			ApplyBloom();
			ApplyPostProcessing();
		}
	}

	public CPMGraphicsSettings.FogQuality FogQuality
	{
		get
		{
			return _graphicsSettings.m_FogQuality;
		}
		set
		{
			_graphicsSettings.m_FogQuality = value;
			ApplyFog();
			ApplyFogQuality();
		}
	}

	public float EvCompensation
	{
		get
		{
			return _graphicsSettings.m_EVCompensation;
		}
		set
		{
			_graphicsSettings.m_EVCompensation = value;
			ApplyExposure();
		}
	}

	public float WBCompensation
	{
		get
		{
			return _graphicsSettings.m_WBCompensation;
		}
		set
		{
			_graphicsSettings.m_WBCompensation = value;
			ApplyWhiteBallance();
		}
	}

	public CPMGraphicsSettings.RealtimeRefections RealtimeReflections
	{
		get
		{
			return _graphicsSettings.m_RealtimeReflections;
		}
		set
		{
			_graphicsSettings.m_RealtimeReflections = value;
			ApplyRealtimeReflections();
		}
	}

	public event Action<int> OnQualityChanged;

	public event Action<bool> OnTyreDeformationChanged;

	[Inject]
	public void Construct(LevelsProvider levelsProvider, PhotoModeConfig config)
	{
		_config = config;
		_levelsProvider = levelsProvider;
	}

	public void SetGraphicsOnStart(Camera mainCam, CoreGameManager coreGameManager, LevelSceneControl levelSceneControl, LevelsProvider levelsProvider)
	{
		_levelsProvider = levelsProvider;
		_levelSceneControl = levelSceneControl;
		_coreGameManager = coreGameManager;
		_mainCam = mainCam;
		SetGraphics();
		FillResolutions();
		Observable.TimerFrame(30).Subscribe((long _) =>
		{
			SetGraphics();
		}).AddTo(this);
		QualitySettings.streamingMipmapsActive = false;
		ApplySteamMipMapBias(-0.5f);
	}

	private void ApplySteamMipMapBias(float bias)
	{
		Texture[] array = Resources.FindObjectsOfTypeAll<Texture>();
		foreach (Texture texture in array)
		{
			if (!(texture == null))
			{
				if (texture is Texture2D texture2D)
				{
					texture2D.mipMapBias = bias;
				}
				else if (texture is Cubemap cubemap)
				{
					cubemap.mipMapBias = bias;
				}
			}
		}
	}

	public void SetDynamicCloudiness(bool isDynamic)
	{
	}

	public void SetGraphics()
	{
		if (!_mainCam)
		{
			_mainCam = Camera.main;
		}
		SunController = UnityEngine.Object.FindObjectOfType<SunAndSkyController>();
		if ((bool)_mainCam)
		{
			_realtimeProbe = _mainCam.GetComponentInChildren<ReflectionProbe>();
		}
		if (_graphicsSettings != null)
		{
			ApplyAA();
			ApplyRealtimeReflections();
			ApplyRenderDistance();
			ApplyClipDistance();
			ApplyDetailDistance();
			ApplyAdditionalEffects();
			ApplyRainQuality();
			ApplyStaticReflections();
			ApplyCameraShadowsQuality();
			ApplyPostProcessing();
			ApplyExposure();
			ApplyWhiteBallance();
			ApplyNightAmbientLightCompensation();
			ApplyFog();
			ApplyFogQuality();
			SetDecalAndSaveConfig(active: true);
			ApplyTyreDeformation();
		}
	}

	private void SetDecalAndSaveConfig(bool active)
	{
		if (!UseDecalShadows)
		{
			UseDecalShadows = active;
			SaveConfig();
		}
	}

	private CPMGraphicsSettingsData GetTemplateFromPreset(int presetIndex = 0)
	{
		Quality = presetIndex;
		CPMGraphicsSettingsData cPMGraphicsSettingsData = ((m_Presets != null && m_Presets.m_Presets.Count > presetIndex) ? (m_Presets.m_Presets[presetIndex].m_Preset.Clone() as CPMGraphicsSettingsData) : new CPMGraphicsSettingsData());
		if (cPMGraphicsSettingsData != null)
		{
			cPMGraphicsSettingsData.m_UseMotionBlur = false;
		}
		return cPMGraphicsSettingsData;
	}

	public CPMGraphicsSettings.AAQuality GetPresetAAQuality()
	{
		int quality = Quality;
		if (m_Presets == null || quality < 0 || quality >= m_Presets.m_Presets.Count)
		{
			return CPMGraphicsSettings.AAQuality.FXAA;
		}
		CPMGraphicsSettingsData preset = m_Presets.m_Presets[quality].m_Preset;
		if (preset.m_AAQuality == CPMGraphicsSettings.AAQuality.Off)
		{
			return CPMGraphicsSettings.AAQuality.FXAA;
		}
		return preset.m_AAQuality;
	}

	public void ChangeDynamicBatching()
	{
		mainPipeLine.supportsDynamicBatching = !mainPipeLine.supportsDynamicBatching;
		Debug.Log($"supportsDynamicBatching: {mainPipeLine.supportsDynamicBatching}");
	}

	public void ChangeSrpBatching()
	{
		mainPipeLine.useSRPBatcher = !mainPipeLine.useSRPBatcher;
		GraphicsSettings.useScriptableRenderPipelineBatching = mainPipeLine.useSRPBatcher;
		Debug.Log($"useSRPBatcher: {mainPipeLine.useSRPBatcher}");
	}

	private void CreateConfigPath()
	{
		if (string.IsNullOrEmpty(_configPath))
		{
			_configPath = Path.Combine(Application.persistentDataPath, "GraphicsSettings.json");
		}
	}

	private void LoadConfig()
	{
		CreateConfigPath();
		if (File.Exists(_configPath))
		{
			string text = File.ReadAllText(_configPath);
			CPMGraphicsSettingsConfig cPMGraphicsSettingsConfig = JsonUtility.FromJson<CPMGraphicsSettingsConfig>(text);
			Quality = cPMGraphicsSettingsConfig.Quality;
			bool flag = false;
			if (cPMGraphicsSettingsConfig.m_ConfigVersion != "0.9.5")
			{
				SetGraphicsSettingsData(GetTemplateFromPreset(Mathf.Clamp(cPMGraphicsSettingsConfig.Quality, 0, 4)));
				FirstLaunch = true;
			}
			else
			{
				SetGraphicsSettingsData(cPMGraphicsSettingsConfig.m_SettingsData);
				FirstLaunch = false;
				if (!text.Contains("m_UseTyreDeformation"))
				{
					_graphicsSettings.m_UseTyreDeformation = Quality >= 3;
				}
				if (!text.Contains("m_RainQuality"))
				{
					_graphicsSettings.m_RainQuality = RainQualityForPreset(Quality);
				}
				if (!Enum.IsDefined(typeof(CPMGraphicsSettings.FogQuality), _graphicsSettings.m_FogQuality))
				{
					_graphicsSettings.m_FogQuality = CPMGraphicsSettings.FogQuality.On;
					flag = true;
				}
				if (!text.Contains("m_HighFpsDefaultsApplied"))
				{
					_graphicsSettings.m_MaxFramerate = 240;
					_graphicsSettings.m_VSync = true;
					_graphicsSettings.m_HighFpsDefaultsApplied = true;
					flag = true;
				}
			}
			_fullScreenMode = cPMGraphicsSettingsConfig.FullScreenMode;
			int[] resolution = cPMGraphicsSettingsConfig.Resolution;
			if (resolution != null && resolution.Length == 2)
			{
				_resolution = cPMGraphicsSettingsConfig.Resolution;
			}
			_displayIndex = cPMGraphicsSettingsConfig.DisplayIndex;
			ApplyDisplay();
			if (flag)
			{
				SaveConfig();
			}
		}
		else
		{
			FirstLaunch = true;
			SetGraphicsSettingsData(GetTemplateFromPreset());
			SaveConfig();
		}
	}

	public void SaveConfig()
	{
		CreateConfigPath();
		CPMGraphicsSettingsConfig cPMGraphicsSettingsConfig = new CPMGraphicsSettingsConfig();
		cPMGraphicsSettingsConfig.m_ConfigVersion = "0.9.5";
		cPMGraphicsSettingsConfig.Quality = Quality;
		cPMGraphicsSettingsConfig.m_SettingsData = _graphicsSettings;
		cPMGraphicsSettingsConfig.FullScreenMode = _fullScreenMode;
		cPMGraphicsSettingsConfig.Resolution = _resolution;
		cPMGraphicsSettingsConfig.DisplayIndex = _displayIndex;
		File.WriteAllText(_configPath, JsonUtility.ToJson(cPMGraphicsSettingsConfig));
	}

	private void CalculateGraphicPresetByDevicePerformance()
	{
		int num = 0;
		if (_performanceDetector == null)
		{
			_performanceDetector = new PerformanceDetector();
		}
		num = _performanceDetector.DetectPerformanceTier();
		PlayerPrefs.SetString("graphics_quality", _graphicsSettingsByIndex[num].ToString());
		Debug.Log($"Auto Graphics Preset: {num}. Ram: {SystemInfo.systemMemorySize} MB");
		ApplyPreset(num);
		PlayerPrefs.SetInt("AutoGraphicsPresetChecked", 1);
		SetGraphicsSettingsData(GetTemplateFromPreset(num));
		SteamTrafficLimits.SetDefaultsForQuality(num);
		int num2 = SteamTrafficLimits.DefaultForQuality(num);
		PlayerPrefs.SetInt("CarQuantity", num2);
		Debug.Log($"Auto Car Traffic Car Quantity set: {num2}");
		ApplyResolution();
		SaveConfig();
	}

	public void ApplyPreset(int index = 0)
	{
		float cloudiness = _graphicsSettings?.m_Cloudiness ?? 0f;
		SetGraphicsSettingsData(GetTemplateFromPreset(index));
		_graphicsSettings.m_Cloudiness = cloudiness;
		Quality = index;
		ApplyAllSettings();
		ForceRefreshGlobalShaderProperties().Forget();
		SaveConfig();
	}

	private async UniTask ForceRefreshGlobalShaderProperties()
	{
		if ((bool)SunController)
		{
			SunController.ForceUpdateAllGlobals();
		}
		await UniTask.DelayFrame(25);
		if ((bool)SunController)
		{
			SunController.ForceUpdateAllGlobals();
		}
	}

	private void Awake()
	{
		if (!Instance)
		{
			Instance = this;
		}
	}

	private void Start()
	{
		_mainCam = Camera.main;
		if (PlayerPrefs.GetInt("AutoGraphicsPresetChecked", 0) != 1)
		{
			CalculateGraphicPresetByDevicePerformance();
		}
		else
		{
			LoadConfig();
			ApplyAllSettings();
		}
		RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
		_renderDistanceController.OnDistanceChanged += OnRenderDistanceChangedCallback;
		_renderPropsDistanceController.OnDistanceChanged += OnRenderPropsDistanceChangedCallback;
		new PhotoModeSavesReseter(_config).ResetAllSaves();
	}

	private void OnDestroy()
	{
		RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
		_renderDistanceController.OnDistanceChanged -= OnRenderDistanceChangedCallback;
		_renderPropsDistanceController.OnDistanceChanged -= OnRenderPropsDistanceChangedCallback;
		_performanceDetector = null;
		if ((bool)ReflectionCamera)
		{
			UnityEngine.Object.Destroy(ReflectionCamera.gameObject);
		}
		if ((bool)ReflectionCube)
		{
			UnityEngine.Object.Destroy(ReflectionCube);
		}
		Shader.DisableKeyword("_CARREFLECTION");
	}

	private void ApplyAllSettings()
	{
		if (_graphicsSettings != null)
		{
			ApplyAdditionalEffects();
			ApplyRealtimeReflections();
			ApplyRenderScale();
			ApplyTextureResolution();
			ApplyMaxFrameRate();
			ApplyVSync();
			ApplyRenderDistance();
			ApplyDetailDistance();
			ApplyObjectsLodQuality();
			ApplyShadingQuality();
			ApplyTyreDeformation();
			ApplyOpaqueTexture();
			ApplyRainQuality();
			ApplyTyreDeformation();
			ApplyCameraShadowsQuality();
			ApplyAA();
			ApplyBlur();
			ApplyCarGroundShadow();
			ApplyLensFlares();
			ApplyStaticReflections();
			ApplyClipDistance();
			ApplyPostProcessing();
			ApplyTonemapping();
			ApplyBloom();
			ApplyVignette();
			ApplyMotionBlur();
			ApplyColorCorrection();
			ApplyFog();
			ApplyFogQuality();
			ApplyNightAmbientLightCompensation();
		}
	}

	private void ApplyDisplayMode()
	{
		Screen.fullScreenMode = _fullScreenMode;
	}

	public List<DisplayInfo> GetDisplays()
	{
		Screen.GetDisplayLayout(_displaysBuffer);
		return _displaysBuffer;
	}

	private void ApplyDisplay()
	{
		Screen.GetDisplayLayout(_displaysBuffer);
		if (_displayIndex < 0 || _displayIndex >= _displaysBuffer.Count)
		{
			_displayIndex = 0;
			return;
		}
		DisplayInfo display = _displaysBuffer[_displayIndex];
		if (!Screen.mainWindowDisplayInfo.Equals(display))
		{
			Vector2Int position = new Vector2Int((display.width - Screen.width) / 2, (display.height - Screen.height) / 2);
			Screen.MoveMainWindowTo(in display, position);
		}
	}

	private void FillResolutions()
	{
		_availableResolutions.Clear();
		_availableResolutions.AddRange(GetTopResolutions(5));
		int[] resolution = _resolution;
		if ((resolution == null || resolution.Length != 2) && _availableResolutions.Count > 0)
		{
			_resolution = new int[2]
			{
				_availableResolutions.Last().width,
				_availableResolutions.Last().height
			};
		}
	}

	public List<Resolution> GetTopResolutions(int count)
	{
		Resolution[] resolutions = Screen.resolutions;
		if (resolutions.Length == 0)
		{
			return new List<Resolution>();
		}
		Resolution resolution = (from r in resolutions
			orderby r.width, r.height
			select r).Last();
		float targetAspectRatio = (float)resolution.width / (float)resolution.height;
		List<Resolution> list = (from r in resolutions
			where Mathf.Abs((float)r.width / (float)r.height - targetAspectRatio) < 0.02f
			group r by new { r.width, r.height } into g
			select g.First() into r
			orderby r.width, r.height
			select r).ToList();
		int count2 = Mathf.Max(0, list.Count - count);
		return list.Skip(count2).ToList();
	}

	private void ApplyResolution()
	{
		int[] resolution = _resolution;
		if (resolution != null && resolution.Length == 2)
		{
			Screen.SetResolution(_resolution[0], _resolution[1], _fullScreenMode);
			ScreenResolutionEvents.RaiseResolutionApplied();
		}
	}

	public void ApplyRenderScale()
	{
		UnityGraphicsBullshit.RenderScale = _graphicsSettings.m_RenderScale;
		UnityGraphicsBullshit.OpaqueDownsampling = ((!(_graphicsSettings.m_RenderScale <= 0.75f)) ? Downsampling._2xBilinear : Downsampling.None);
	}

	private void ApplyAA()
	{
		UniversalAdditionalCameraData universalAdditionalCameraData = null;
		if ((bool)_mainCam)
		{
			universalAdditionalCameraData = _mainCam.GetComponent<UniversalAdditionalCameraData>();
		}
		Shader.DisableKeyword("_TEMPORALFOG");
		switch (_graphicsSettings.m_AAQuality)
		{
		case CPMGraphicsSettings.AAQuality.Off:
			if ((bool)universalAdditionalCameraData)
			{
				universalAdditionalCameraData.antialiasing = AntialiasingMode.None;
			}
			UnityGraphicsBullshit.Msaa = MsaaQuality.Disabled;
			break;
		case CPMGraphicsSettings.AAQuality.MSAA2x:
			if ((bool)universalAdditionalCameraData)
			{
				universalAdditionalCameraData.antialiasing = AntialiasingMode.None;
			}
			UnityGraphicsBullshit.Msaa = MsaaQuality._2x;
			break;
		case CPMGraphicsSettings.AAQuality.MSAA4x:
			if ((bool)universalAdditionalCameraData)
			{
				universalAdditionalCameraData.antialiasing = AntialiasingMode.None;
			}
			UnityGraphicsBullshit.Msaa = MsaaQuality._4x;
			break;
		case CPMGraphicsSettings.AAQuality.MSAA8x:
			if ((bool)universalAdditionalCameraData)
			{
				universalAdditionalCameraData.antialiasing = AntialiasingMode.None;
			}
			UnityGraphicsBullshit.Msaa = MsaaQuality._8x;
			break;
		case CPMGraphicsSettings.AAQuality.FXAA:
			if ((bool)universalAdditionalCameraData)
			{
				universalAdditionalCameraData.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
			}
			UnityGraphicsBullshit.Msaa = MsaaQuality.Disabled;
			break;
		case CPMGraphicsSettings.AAQuality.SMAA:
			if ((bool)universalAdditionalCameraData)
			{
				universalAdditionalCameraData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
			}
			UnityGraphicsBullshit.Msaa = MsaaQuality.Disabled;
			break;
		case CPMGraphicsSettings.AAQuality.TAA:
			if ((bool)universalAdditionalCameraData)
			{
				Shader.EnableKeyword("_TEMPORALFOG");
				universalAdditionalCameraData.antialiasing = AntialiasingMode.TemporalAntiAliasing;
				universalAdditionalCameraData.taaSettings.quality = TemporalAAQuality.Medium;
			}
			UnityGraphicsBullshit.Msaa = MsaaQuality.Disabled;
			break;
		}
		if (!(universalAdditionalCameraData != null))
		{
			return;
		}
		UniversalAdditionalCameraData[] array = Resources.FindObjectsOfTypeAll<UniversalAdditionalCameraData>();
		foreach (UniversalAdditionalCameraData universalAdditionalCameraData2 in array)
		{
			if (!(universalAdditionalCameraData2 == null) && !(universalAdditionalCameraData2 == universalAdditionalCameraData))
			{
				universalAdditionalCameraData2.antialiasing = universalAdditionalCameraData.antialiasing;
				universalAdditionalCameraData2.antialiasingQuality = universalAdditionalCameraData.antialiasingQuality;
			}
		}
	}

	private void ApplyTextureResolution()
	{
		_ = _graphicsSettings.m_TextureResolution;
		int valueOnRamSettingsIosOnly = RamDevice.GetValueOnRamSettingsIosOnly(2, 1, 0);
		QualitySettings.globalTextureMipmapLimit = 0;
		Debug.Log($"TextureMipmapLimit set graphics manager {QualitySettings.globalTextureMipmapLimit}");
		QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
	}

	public void ApplyMaxFrameRate()
	{
		Application.targetFrameRate = _graphicsSettings.m_MaxFramerate;
		if (MaxFramerate > 30 && BatterySaveMode)
		{
			BatterySaveMode = false;
		}
	}

	public void ApplyVSync()
	{
		QualitySettings.vSyncCount = (_graphicsSettings.m_VSync ? 1 : 0);
	}

	private void ApplyBatterySaveMode()
	{
		if (BatterySaveMode && MaxFramerate > 30)
		{
			_graphicsSettings.m_MaxFramerate = 30;
		}
	}

	private void ApplyRenderDistance()
	{
		_camerasRenderDistance.UpdateCamerasDistance(_renderDistanceController.Distance);
		ApplyShadowsQuality();
	}

	private void ApplyObjectsLodQuality()
	{
		QualitySettings.lodBias = _graphicsSettings.m_ObjectsLodQuality;
		QualitySettings.lodBias = SteamPerfTweaks.SteamLodBias(_graphicsSettings.m_SteamLodBias, Quality);
	}

	private void ApplyNightAmbientLightCompensation()
	{
		if ((bool)SunController)
		{
			SunController.SetAmbientNightCompensation(_graphicsSettings.m_NightAmbientLightingCompensation);
		}
	}

	private void ApplyShadingQuality()
	{
		Shader.DisableKeyword("_QUALITY_POTATO");
		Shader.DisableKeyword("_QUALITY_ULTRA");
		switch (_graphicsSettings.m_ShadingQuality)
		{
		case CPMGraphicsSettings.ShadersQuality.Potato:
			Shader.EnableKeyword("_QUALITY_POTATO");
			Shader.DisableKeyword("_BENTNORMAL");
			break;
		case CPMGraphicsSettings.ShadersQuality.High:
			Shader.DisableKeyword("_BENTNORMAL");
			break;
		case CPMGraphicsSettings.ShadersQuality.Ultra:
			Shader.EnableKeyword("_QUALITY_ULTRA");
			Shader.EnableKeyword("_BENTNORMAL");
			break;
		}
		RainWetnessDriver.ApplyQualityPreset(_graphicsSettings.m_ShadingQuality);
	}

	private void ApplyOpaqueTexture()
	{
		switch (_graphicsSettings.m_ShadingQuality)
		{
		case CPMGraphicsSettings.ShadersQuality.Potato:
			UnityGraphicsBullshit.RequireOpaqueTexture = false;
			break;
		case CPMGraphicsSettings.ShadersQuality.High:
			UnityGraphicsBullshit.RequireOpaqueTexture = true;
			break;
		case CPMGraphicsSettings.ShadersQuality.Ultra:
			UnityGraphicsBullshit.RequireOpaqueTexture = true;
			break;
		}
	}

	public void ApplyShadowsQuality()
	{
		switch (_graphicsSettings.m_ShadowsQuality)
		{
		case CPMGraphicsSettings.ShadowsQuality.Off:
			QualitySettings.shadowmaskMode = ShadowmaskMode.Shadowmask;
			UnityGraphicsBullshit.MainLightCastShadows = true;
			UnityGraphicsBullshit.MainLightShadowResolution = ShadowResolution._256;
			UnityGraphicsBullshit.ShadowCascadeCount = 1;
			break;
		case CPMGraphicsSettings.ShadowsQuality.Low:
			QualitySettings.shadowmaskMode = ShadowmaskMode.DistanceShadowmask;
			UnityGraphicsBullshit.MainLightCastShadows = true;
			UnityGraphicsBullshit.MainLightShadowResolution = ShadowResolution._512;
			UnityGraphicsBullshit.ShadowCascadeCount = 2;
			UnityGraphicsBullshit.ShadowDistance = 30f;
			UnityGraphicsBullshit.Cascade2Split = 1f / UnityGraphicsBullshit.ShadowDistance * 7f;
			break;
		case CPMGraphicsSettings.ShadowsQuality.Mid:
			QualitySettings.shadowmaskMode = ShadowmaskMode.DistanceShadowmask;
			UnityGraphicsBullshit.MainLightCastShadows = true;
			UnityGraphicsBullshit.MainLightShadowResolution = ShadowResolution._1024;
			UnityGraphicsBullshit.ShadowCascadeCount = 2;
			UnityGraphicsBullshit.ShadowDistance = 30f;
			UnityGraphicsBullshit.Cascade2Split = 1f / UnityGraphicsBullshit.ShadowDistance * 7f;
			break;
		case CPMGraphicsSettings.ShadowsQuality.High:
			QualitySettings.shadowmaskMode = ShadowmaskMode.DistanceShadowmask;
			UnityGraphicsBullshit.MainLightShadowResolution = ShadowResolution._2048;
			UnityGraphicsBullshit.MainLightCastShadows = true;
			UnityGraphicsBullshit.ShadowCascadeCount = 2;
			UnityGraphicsBullshit.ShadowDistance = 100f;
			UnityGraphicsBullshit.Cascade2Split = 1f / UnityGraphicsBullshit.ShadowDistance * 7f;
			break;
		case CPMGraphicsSettings.ShadowsQuality.Ultra:
			QualitySettings.shadowmaskMode = ShadowmaskMode.DistanceShadowmask;
			UnityGraphicsBullshit.MainLightShadowResolution = ShadowResolution._4096;
			UnityGraphicsBullshit.MainLightCastShadows = true;
			UnityGraphicsBullshit.ShadowCascadeCount = 4;
			UnityGraphicsBullshit.ShadowDistance = Mathf.Min(CameraRenderDistance, 250f);
			UnityGraphicsBullshit.Cascade4Split = new Vector3(1f / UnityGraphicsBullshit.ShadowDistance * 7f, 1f / UnityGraphicsBullshit.ShadowDistance * 40f, 1f / UnityGraphicsBullshit.ShadowDistance * 120f);
			break;
		}
	}

	public void ApplyCameraShadowsQuality()
	{
		if ((bool)_mainCam)
		{
			_mainCam.GetUniversalAdditionalCameraData().renderShadows = _graphicsSettings.m_ShadowsQuality != CPMGraphicsSettings.ShadowsQuality.Off;
		}
		SettingDirectionalLight().Forget();
	}

	private async UniTask SettingDirectionalLight()
	{
		await UniTask.DelayFrame(20);
		Light sun = RenderSettings.sun;
		if ((bool)sun)
		{
			sun.shadowNearPlane = 10f;
		}
		GameObject gameObject = GameObject.Find("Directional Light");
		if ((bool)gameObject)
		{
			Light component = gameObject.GetComponent<Light>();
			component.shadows = ((_graphicsSettings.m_ShadowsQuality != CPMGraphicsSettings.ShadowsQuality.Off) ? LightShadows.Hard : LightShadows.None);
			if ((bool)gameObject.GetComponent<DirectionalLightShadowSwitch>())
			{
				component.enabled = _graphicsSettings.m_ShadowsQuality != CPMGraphicsSettings.ShadowsQuality.Off;
			}
		}
	}

	private void ApplyBlur()
	{
		if ((bool)blur)
		{
			blur.SetActive(_graphicsSettings.m_UseBlur);
			if (!_graphicsSettings.m_UseBlur)
			{
				Shader.SetGlobalTexture("_GlobalUniversalBlurTexture", Texture2D.blackTexture);
			}
		}
	}

	public void ApplyTyreDeformation()
	{
		if (_graphicsSettings.m_UseTyreDeformation)
		{
			Shader.EnableKeyword("_TYREDEFORM");
		}
		else
		{
			Shader.DisableKeyword("_TYREDEFORM");
		}
	}

	private void ApplyRainQuality()
	{
		RainWetnessDriver.ApplyRainQuality(_graphicsSettings.m_RainQuality);
	}

	public static CPMGraphicsSettings.RainQuality RainQualityForPreset(int presetIndex)
	{
		if (presetIndex <= 0)
		{
			return CPMGraphicsSettings.RainQuality.Off;
		}
		if (presetIndex == 1)
		{
			return CPMGraphicsSettings.RainQuality.Minimal;
		}
		return CPMGraphicsSettings.RainQuality.All;
	}

	private void ApplyCarGroundShadow()
	{
		Decals.SetActive(_graphicsSettings.m_UseDecalShadows);
	}

	public void ApplyLensFlares()
	{
		UnityGraphicsBullshit.SupportDataDrivenLensFlare = _graphicsSettings.m_UseLensFlares;
	}

	public void PerformFirstLaunchFixes()
	{
		if (FirstLaunch)
		{
			FirstLaunch = false;
			SaveConfig();
			ResetStaticReflectionDelayedAsync(this.GetCancellationTokenOnDestroy()).Forget();
		}
	}

	private async UniTaskVoid ResetStaticReflectionDelayedAsync(CancellationToken ct = default(CancellationToken))
	{
		_ = 1;
		try
		{
			bool useReflections = UseStaticReflections;
			UseStaticReflections = false;
			await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			UseStaticReflections = useReflections;
			await UniTask.Yield(ct);
		}
		catch (OperationCanceledException)
		{
		}
	}

	public void ApplyStaticReflections()
	{
		for (int i = 0; i < ReflectionProbes.Count; i++)
		{
			if (ReflectionProbes[i] != null)
			{
				ReflectionProbes[i].enabled = _graphicsSettings.m_UseStaticReflectons;
			}
		}
	}

	private void ApplyClipDistance()
	{
		float[] array = new float[32];
		(float, float) distances = _renderPropsDistanceController.Distances;
		array[25] = distances.Item1;
		array[14] = distances.Item2;
		if ((bool)_mainCam)
		{
			_mainCam.layerCullDistances = array;
		}
	}

	private void ApplyDetailDistance()
	{
		ObscuredPrefs.SetFloat("detailDistance", _graphicsSettings.CarDetailDistance);
		V.DetailDistance = _graphicsSettings.CarDetailDistance;
	}

	private void ApplyAdditionalEffects()
	{
		switch (_graphicsSettings.m_AdditionalEffects)
		{
		case CPMGraphicsSettings.AdditionalEffects.Low:
			if (SceneManager.GetActiveScene().buildIndex == 1)
			{
				_coreGameManager.GaragLight.SetActive(value: false);
			}
			if ((bool)_levelsProvider.LevelScript && (bool)_levelsProvider.LevelScript.ParkingLights)
			{
				_levelsProvider.LevelScript.ParkingLights.SetActive(value: false);
			}
			if ((bool)V.SceneControl)
			{
				V.SceneControl.OnQualityChanged();
				V.SceneControl.ForHighQualityEnable(enable: false);
			}
			if ((bool)_levelSceneControl)
			{
				_levelSceneControl.EnableLowQuality();
			}
			break;
		case CPMGraphicsSettings.AdditionalEffects.Mid:
			if (SceneManager.GetActiveScene().buildIndex == 1)
			{
				_coreGameManager.GaragLight.SetActive(value: false);
			}
			if ((bool)_levelsProvider.LevelScript && (bool)_levelsProvider.LevelScript.ParkingLights)
			{
				_levelsProvider.LevelScript.ParkingLights.SetActive(value: true);
			}
			if ((bool)V.SceneControl)
			{
				V.SceneControl.OnQualityChanged();
				V.SceneControl.ForHighQualityEnable(enable: false);
			}
			if ((bool)_levelSceneControl)
			{
				_levelSceneControl.EnableLowQuality();
			}
			break;
		case CPMGraphicsSettings.AdditionalEffects.High:
			if (SceneManager.GetActiveScene().buildIndex == 1)
			{
				_coreGameManager.GaragLight.SetActive(value: true);
			}
			if ((bool)_levelsProvider.LevelScript && (bool)_levelsProvider.LevelScript.ParkingLights)
			{
				_levelsProvider.LevelScript.ParkingLights.SetActive(value: true);
			}
			if ((bool)V.SceneControl)
			{
				V.SceneControl.OnQualityChanged();
				V.SceneControl.ForHighQualityEnable(enable: true);
			}
			if ((bool)_levelSceneControl)
			{
				_levelSceneControl.EnableHighQuality();
			}
			break;
		}
		GlobalEventSystem.InvokeGraphicsChanged(_graphicsSettings.m_AdditionalEffects);
	}

	private void ApplyPostProcessing()
	{
		if ((bool)_mainCam)
		{
			UniversalAdditionalCameraData universalAdditionalCameraData = _mainCam.GetUniversalAdditionalCameraData();
			if (_graphicsSettings.m_Tonemapping != TonemappingMode.None || _graphicsSettings.m_UseBloom || _graphicsSettings.m_UseVignette || _graphicsSettings.m_UseMotionBlur || _graphicsSettings.m_ColorCorrectionLut != CPMGraphicsSettings.ColorCorrectionLut.Off || _graphicsSettings.m_AAQuality != CPMGraphicsSettings.AAQuality.Off || _graphicsSettings.m_UseBlur)
			{
				universalAdditionalCameraData.renderPostProcessing = true;
				universalAdditionalCameraData.stopNaN = true;
				UnityGraphicsBullshit.SupportsHDR = true;
			}
			else
			{
				universalAdditionalCameraData.renderPostProcessing = false;
				universalAdditionalCameraData.stopNaN = false;
				UnityGraphicsBullshit.SupportsHDR = false;
			}
		}
	}

	private void ApplyColorCorrection()
	{
		if (m_DefaultVolume != null && m_DefaultVolume.TryGet<ColorLookup>(out var component))
		{
			if (_graphicsSettings.m_ColorCorrectionLut != CPMGraphicsSettings.ColorCorrectionLut.Off)
			{
				component.active = true;
				component.contribution.value = 1f;
				component.texture.value = ColorCorrectionLUTS[(int)(_graphicsSettings.m_ColorCorrectionLut - 1)];
			}
			else
			{
				component.active = false;
			}
		}
	}

	private void ApplyVignette()
	{
		if (m_DefaultVolume != null && m_DefaultVolume.TryGet<Vignette>(out var component))
		{
			component.active = _graphicsSettings.m_UseVignette;
		}
	}

	private void ApplyMotionBlur()
	{
		if (m_DefaultVolume != null && m_DefaultVolume.TryGet<MotionBlur>(out var component))
		{
			component.active = _graphicsSettings.m_UseMotionBlur;
		}
	}

	private void ApplyTonemapping()
	{
		if (m_DefaultVolume != null)
		{
			if (m_DefaultVolume.TryGet<Tonemapping>(out var component))
			{
				component.active = _graphicsSettings.m_Tonemapping != TonemappingMode.None;
				component.mode.value = _graphicsSettings.m_Tonemapping;
			}
			if (m_DefaultVolume.TryGet<ColorAdjustments>(out var component2))
			{
				bool flag = _graphicsSettings.m_Tonemapping == TonemappingMode.ACES;
				component2.contrast.value = (flag ? (-12f) : 0f);
				AcesAdjustment = (flag ? 0.3f : 0f);
			}
		}
		ApplyExposure();
		ApplyWhiteBallance();
	}

	private void ApplyCloudiness(bool immediate = false)
	{
		if (SunController != null)
		{
			if (!immediate)
			{
				SunController.CloudSequenceSlider = _graphicsSettings.m_Cloudiness;
			}
			else
			{
				SunController.ApplyCloudSequenceImmediate(_graphicsSettings.m_Cloudiness);
			}
		}
		else
		{
			Shader.SetGlobalFloat("_GlobalOvercast", 0f);
		}
	}

	public void SyncCloudiness(float value)
	{
		if (_graphicsSettings != null)
		{
			_graphicsSettings.m_Cloudiness = value;
		}
	}

	private void ApplyBloom()
	{
		if (m_DefaultVolume != null && m_DefaultVolume.TryGet<Bloom>(out var component))
		{
			component.active = _graphicsSettings.m_UseBloom;
		}
	}

	private void ApplyFog()
	{
		Shader.SetGlobalTexture("_SkyboxProbe", ReflectionProbe.defaultTexture);
	}

	private void ApplyFogQuality()
	{
		fog.SetActive(_graphicsSettings.m_FogQuality != CPMGraphicsSettings.FogQuality.Off);
		switch (_graphicsSettings.m_FogQuality)
		{
		case CPMGraphicsSettings.FogQuality.Off:
			Shader.DisableKeyword("_FOG_QUALITY_MEDIUM");
			Shader.DisableKeyword("_FOG_QUALITY_HIGH");
			break;
		case CPMGraphicsSettings.FogQuality.On:
			Shader.EnableKeyword("_FOG_QUALITY_MEDIUM");
			Shader.EnableKeyword("_FOG_QUALITY_HIGH");
			break;
		}
	}

	private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
	{
		if (camera == ReflectionCamera)
		{
			QualitySettings.maximumLODLevel = 2;
			Shader.DisableKeyword("_QUALITY_ULTRA");
			Shader.EnableKeyword("_QUALITY_POTATO");
			Shader.DisableKeyword("_CARREFLECTION");
			Shader.DisableKeyword("_BENTNORMAL");
			Shader.DisableKeyword("_FOG_QUALITY_MEDIUM");
			Shader.DisableKeyword("_FOG_QUALITY_HIGH");
		}
		else
		{
			QualitySettings.maximumLODLevel = 0;
			ApplyShadingQuality();
			ApplyFogQuality();
			if (UseShaderRealtimeReflection && ReflectionCubeCycles > 6)
			{
				Shader.EnableKeyword("_CARREFLECTION");
			}
			else
			{
				Shader.DisableKeyword("_CARREFLECTION");
			}
		}
	}

	public void ApplyExposure()
	{
		TimeOfDayExposure = ((FlagsStorage.InGarageMenu || FlagsStorage.InGarageFree || FlagsStorage.IsLevelScene) ? 0f : TimeOfDayExposure);
		SetExternalExposure(TimeOfDayExposure);
	}

	private void UpdateGlobalExposureWbCorrection()
	{
		Shader.SetGlobalVector(ExposureWB, new Vector4(Mathf.Pow(2f, TimeOfDayExposure + AcesAdjustment + _graphicsSettings.m_EVCompensation), TimeOfDayWHiteBallance, 0f, 0f));
	}

	public void SetExternalExposure(float value)
	{
		TimeOfDayExposure = value;
		if (m_DefaultVolume != null && m_DefaultVolume.TryGet<ColorAdjustments>(out var component))
		{
			component.active = _graphicsSettings.m_Tonemapping != TonemappingMode.None;
			component.postExposure.value = TimeOfDayExposure + AcesAdjustment + _graphicsSettings.m_EVCompensation;
		}
		UpdateGlobalExposureWbCorrection();
	}

	public void ApplyWhiteBallance()
	{
		TimeOfDayWHiteBallance = ((FlagsStorage.InGarageMenu || FlagsStorage.InGarageFree || FlagsStorage.IsLevelScene) ? 0f : TimeOfDayWHiteBallance);
		SetExternalWhiteBallance(TimeOfDayWHiteBallance);
		UpdateGlobalExposureWbCorrection();
	}

	public void SetExternalWhiteBallance(float value)
	{
		TimeOfDayWHiteBallance = value;
		if (m_DefaultVolume != null && m_DefaultVolume.TryGet<WhiteBalance>(out var component))
		{
			component.active = _graphicsSettings.m_Tonemapping != TonemappingMode.None;
			component.temperature.value = TimeOfDayWHiteBallance + _graphicsSettings.m_WBCompensation;
		}
	}

	public void ApplyRealtimeReflections()
	{
		if (FlagsStorage.InGarageMenu || FlagsStorage.InGarageFree || _graphicsSettings.m_RealtimeReflections == CPMGraphicsSettings.RealtimeRefections.Off)
		{
			if ((bool)ReflectionCube)
			{
				UnityEngine.Object.DestroyImmediate(ReflectionCube);
			}
			if ((bool)ReflectionCamera)
			{
				UnityEngine.Object.DestroyImmediate(ReflectionCamera.gameObject);
			}
			RenderReflections = false;
			UseShaderRealtimeReflection = false;
			return;
		}
		if (!ReflectionCube)
		{
			ReflectionCube = new RenderTexture(256, 256, 16);
			ReflectionCube.format = RenderTextureFormat.DefaultHDR;
			ReflectionCube.useMipMap = true;
			ReflectionCube.useDynamicScale = false;
			ReflectionCube.dimension = TextureDimension.Cube;
			ReflectionCube.hideFlags = HideFlags.HideAndDontSave;
			Shader.SetGlobalTexture("_ReflectionCube", ReflectionCube);
			ReflectionCubeCycles = 0;
			UseShaderRealtimeReflection = true;
		}
		if (!ReflectionCamera)
		{
			GameObject gameObject = new GameObject("ReflectionCamera", typeof(Camera));
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			gameObject.transform.position = base.transform.position;
			gameObject.transform.rotation = Quaternion.identity;
			ReflectionCamera = gameObject.GetComponent<Camera>();
			UniversalAdditionalCameraData universalAdditionalCameraData = ReflectionCamera.GetUniversalAdditionalCameraData();
			universalAdditionalCameraData.renderPostProcessing = false;
			universalAdditionalCameraData.renderShadows = true;
			universalAdditionalCameraData.SetRenderer(3);
			universalAdditionalCameraData.stopNaN = true;
			universalAdditionalCameraData.requiresDepthOption = CameraOverrideOption.Off;
			universalAdditionalCameraData.requiresColorOption = CameraOverrideOption.Off;
			ReflectionCamera.nearClipPlane = 0.5f;
			ReflectionCamera.farClipPlane = 50f;
			ReflectionCamera.cullingMask = RealtimeReflectionLayerMask;
			ReflectionCamera.enabled = false;
			if ((bool)SunController)
			{
				gameObject.AddComponent<Skybox>().material = SkyboxFallBack;
			}
		}
		RenderReflections = true;
	}

	private void LateUpdate()
	{
		if (!RenderReflections)
		{
			return;
		}
		int faceMask = 63;
		if (_graphicsSettings.m_RealtimeReflections == CPMGraphicsSettings.RealtimeRefections.Medium)
		{
			if (!SteamReflectionCubeThrottle.TryGetFaceMask(Quality, out var faceMask2))
			{
				return;
			}
			faceMask = faceMask2;
			ReflectionCubeCycles = Mathf.Min(ReflectionCubeCycles + 1, 7);
		}
		else
		{
			ReflectionCubeCycles = 7;
		}
		Transform transform;
		if ((bool)V.Car)
		{
			transform = V.Car.transform;
		}
		else
		{
			if (!_mainCam)
			{
				return;
			}
			transform = _mainCam.transform;
		}
		ReflectionCamera.transform.position = transform.position;
		ReflectionCamera.RenderToCubemap(ReflectionCube, faceMask);
	}

	private void SetGraphicsSettingsData(CPMGraphicsSettingsData graphicsSettings)
	{
		_graphicsSettings = graphicsSettings;
		_renderDistanceController.SetSettings(GraphicsDistanceController.PriorityType.Default, _graphicsSettings.m_CameraRenderDistance);
		_renderPropsDistanceController.SetSettings(GraphicsDistanceController.PriorityType.Default, _graphicsSettings.ClipDistance);
		_graphicsSettings.m_ObjectsLodQuality = ClipDistanceToLodBias(_graphicsSettings.ClipDistance);
	}

	private void OnRenderDistanceChangedCallback(float distance)
	{
		_graphicsSettings.m_CameraRenderDistance = _renderDistanceController.GetDistanceSettings(GraphicsDistanceController.PriorityType.Default);
		ApplyRenderDistance();
	}

	private void OnRenderPropsDistanceChangedCallback(float distance01, float distance02)
	{
		_graphicsSettings.ClipDistance = _renderPropsDistanceController.GetDistanceSettings(GraphicsDistanceController.PriorityType.Default);
		_graphicsSettings.m_ObjectsLodQuality = ClipDistanceToLodBias(_graphicsSettings.ClipDistance);
		ApplyObjectsLodQuality();
		ApplyClipDistance();
	}

	private static float ClipDistanceToLodBias(float clipDistance)
	{
		float t = Mathf.InverseLerp(15f, 120f, clipDistance);
		return Mathf.Lerp(0.4f, 1f, t);
	}
}
