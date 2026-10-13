using System;
using System.Collections.Generic;
using System.Linq;
using CPMEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.UIElement.HUD;

public class LightController : MonoBehaviour
{
	public static readonly uint Play_headlights_close = AudioWrapper.GetId("Play_pop_up_headlights_close");

	public static readonly uint Play_headlights_open = AudioWrapper.GetId("Play_pop_up_headlights_open");

	private const string TurnLightsAutoToggleConfigPath = "Configs/CarTurnLightsAutoTogglerConfig";

	private const string PlayTurnIndicator = "Play_indicator";

	private const string StopTurnIndicator = "Stop_indicator";

	public const float IndicatorAlpha = 0.8f;

	public GameObject[] HFtweens;

	public GameObject[] HRtweens;

	public LightSideType LightSide;

	public LightSetting LightSetting;

	private LightConfig _lightConfig;

	public GameObject HLLeft;

	public GameObject HLRight;

	public GameObject HLLeftLight;

	public GameObject HLRightLight;

	private GameObject HeadLightsParent;

	public ReactiveProperty<TurnSignalState> IndicatorState;

	public TurnSignalState currentState;

	public int HState;

	private Shader _addictive;

	private Color _indicatorHaloMainColor;

	private Color _indicatorMainColor;

	private Color _lerpedColorBrake1;

	private Color _lerpedColorBrake2;

	private Color _lerpedColorRear;

	public Material LeftIndicatorMat2;

	public Material LeftIndicatorMat3;

	public Material LeftIndicatorMat4;

	public Material LeftIndicatorMat5;

	public Material RightIndicatorMat2;

	public Material RightIndicatorMat3;

	public Material RightIndicatorMat4;

	public Material RightIndicatorMat5;

	public Material IndicatorDiodMat;

	public Material turnRightDiodMat;

	public Material turnLeftDiodMat;

	public Color indicatorColorDiod;

	public Color lerpedColorDiod;

	private Color _brakeColorRed;

	private Color _brakeColorTransParentRed;

	private Color rearColorTransparent;

	public Material BrakeMat1;

	public List<Material> BrakeMat2 = new List<Material>();

	public Material rearMat;

	public Material HeadlightsMats;

	public List<Material> rearLightsMats = new List<Material>();

	public Material lightsMat;

	public Material lightsMat2;

	public Material reverseMat;

	public Color indicatorColorGray = Color.gray;

	public Color indicatorTransparent;

	private Color rearWhite;

	public int TintId;

	public Transform lights;

	public Color indicatorColor;

	public bool BrakeIsLight;

	private int _intensity;

	public Color indicatorColorYellow = Color.yellow;

	[SerializeField]
	private Color[] _turnOnColorLights;

	public bool lightState;

	private bool _farLightsOn;

	public bool littleBrake;

	private bool _turnLeftOn;

	private bool _turnRightOn;

	public float diodsIntesity = 2f;

	public float[] lightsIntensity;

	public float[] lightsIntensity2;

	public float[] frontLightIntensity;

	public float[] rearBreakIntensity;

	public float[] rearLightIntensity;

	public float[] reverseIntensity;

	public float[] turnIntensity;

	public float[] turnIntensity2;

	public float[] turnIntensity3;

	private float _intensityBrakeLight = 0.5f;

	private float _intensityRearLight = 0.5f;

	private VehicleData _vehicleData;

	private Powertrain _powertrain;

	private Axles _axles;

	private CarTurnLightsAutoToggler carTurnLightsAutoToggler;

	private MultiPlayerPosControl mppc;

	private CarChildsReferences _carChildsReferences;

	private MeshRenderer[] _leftIndicators;

	private MeshRenderer[] _rightIndicators;

	private MeshRenderer[] _brakes;

	private MeshRenderer[] _rear;

	private Material[] _leftHaloMat;

	private Material[] _rightHaloMat;

	private Color[] _leftHaloClr;

	private Color[] _rightHaloClr;

	public Light mainLight;

	private static readonly int OpenAnimatorInt = Animator.StringToHash("open");

	private IDisposable _disposableTurnOffAllIndicators;

	private bool _isIndicatorSoundPlaying;

	private bool _isDestroyed;

	private IDisposable _turnSignalSubscription;

	private IDisposable _shortFlashDisposable;

	private MeshCollider _collider;

	private MenuLevelParent _menuLevelParent;

	private HDHeadlighsController _hdHeadlighsController;

	private SoundBankLoadService _soundBankService;

	private bool _headlightsBankHeld;

	private bool _hasNewLightsController;

	private static readonly float[] BodyKitLightsIntensityFloor = new float[3] { 0f, 2f, 3f };

	private HDHeadlighsController GetHdHeadlighsController
	{
		get
		{
			if ((bool)_hdHeadlighsController)
			{
				return _hdHeadlighsController;
			}
			TryGetComponent<HDHeadlighsController>(out _hdHeadlighsController);
			return _hdHeadlighsController;
		}
	}

	private int CurrentLightsIndex
	{
		get
		{
			if (!lightState)
			{
				return 0;
			}
			if (!_farLightsOn)
			{
				return 1;
			}
			return 2;
		}
	}

	public event Action<bool> OnSwitchLights;

	public void NullMppcRef()
	{
		mppc = null;
	}

	public LightController(Color rearColorTransparent)
	{
		this.rearColorTransparent = rearColorTransparent;
	}

	[Inject]
	public void Construct(MenuLevelParent menuLevelParent)
	{
		_menuLevelParent = menuLevelParent;
	}

	private void Awake()
	{
		_lightConfig = Resources.Load<LightConfig>("Configs/LightConfig");
		SetBothLightSide();
		TryGetComponent<VehicleData>(out _vehicleData);
		TryGetComponent<Powertrain>(out _powertrain);
		TryGetComponent<MultiPlayerPosControl>(out mppc);
		TryGetComponent<Axles>(out _axles);
		TryGetComponent<CarChildsReferences>(out _carChildsReferences);
		_hasNewLightsController = GetHdHeadlighsController;
		CarTurnlightsAutoTurnOffConfig carTurnlightsAutoTurnOffConfig = Resources.Load<CarTurnlightsAutoTurnOffConfig>("Configs/CarTurnLightsAutoTogglerConfig");
		carTurnLightsAutoToggler = new CarTurnLightsAutoToggler(base.transform, this, OnTurnLightsAutoToggled, carTurnlightsAutoTurnOffConfig.data);
		_brakeColorRed = Color.red;
		CreateDefaultIntensity();
		if (_turnOnColorLights.IsNullOrEmptyCollection())
		{
			_turnOnColorLights = new Color[4];
			for (int i = 0; i < 4; i++)
			{
				_turnOnColorLights[i] = indicatorColorYellow;
			}
		}
	}

	public void SetupHDRenderers()
	{
		if (_hasNewLightsController)
		{
			GetHdHeadlighsController.SetupRenderers();
		}
	}

	private void SubscribeEngineListeners()
	{
		if ((bool)_powertrain)
		{
			_powertrain.OnGearChanged += ReverseLight;
			_powertrain.OnEngineStarted += TurnOnDaytimeRunningLights;
			_powertrain.OnEngineShutdown += TurnOffDaytimeRunningLights;
		}
	}

	private void UnSubscribeEngineListeners()
	{
		if ((bool)_powertrain)
		{
			_powertrain.OnGearChanged -= ReverseLight;
			_powertrain.OnEngineStarted -= TurnOnDaytimeRunningLights;
			_powertrain.OnEngineShutdown -= TurnOffDaytimeRunningLights;
		}
	}

	private void InitNewLights()
	{
		GetHdHeadlighsController.Init();
		_vehicleData.OnCarGraphicLoaded -= InitNewLights;
		if ((bool)_powertrain && _powertrain.IsEngineWorks)
		{
			TurnOnDaytimeRunningLights();
		}
	}

	public void RemoveLightsSide(LightSideType side)
	{
		LightSide &= ~side;
		LightSetting = _lightConfig.GetByType(LightSide);
	}

	public void SetBothLightSide()
	{
		LightSide = LightSideType.Both;
		LightSetting = _lightConfig.GetByType(LightSide);
	}

	public void SetupHeadLightsRenderers()
	{
		GetHdHeadlighsController.SetupRenderers();
	}

	private void OnDestroy()
	{
		if ((bool)_vehicleData)
		{
			_vehicleData.OnCarGraphicLoaded -= TryHoldHeadlightsBank;
		}
		if (_headlightsBankHeld)
		{
			_soundBankService?.TryUnloadBank(SoundBankType.CarHeadlights).Forget();
		}
		AudioWrapper.PostEvent("Stop_indicator", base.gameObject);
		CommonExtensions.DestroyMat(ref LeftIndicatorMat2);
		CommonExtensions.DestroyMat(ref LeftIndicatorMat3);
		CommonExtensions.DestroyMat(ref LeftIndicatorMat4);
		CommonExtensions.DestroyMat(ref LeftIndicatorMat5);
		CommonExtensions.DestroyMat(ref RightIndicatorMat2);
		CommonExtensions.DestroyMat(ref RightIndicatorMat3);
		CommonExtensions.DestroyMat(ref RightIndicatorMat4);
		CommonExtensions.DestroyMat(ref RightIndicatorMat5);
		CommonExtensions.DestroyMat(ref IndicatorDiodMat);
		CommonExtensions.DestroyMat(ref turnRightDiodMat);
		CommonExtensions.DestroyMat(ref turnLeftDiodMat);
		CommonExtensions.DestroyMat(ref BrakeMat1);
		CommonExtensions.DestroyMat(ref rearMat);
		CommonExtensions.DestroyMat(ref HeadlightsMats);
		CommonExtensions.DestroyMat(ref lightsMat);
		CommonExtensions.DestroyMat(ref lightsMat2);
		CommonExtensions.DestroyMat(ref reverseMat);
		IndicatorState?.Dispose();
		for (int i = 0; i < rearLightsMats.Count; i++)
		{
			if ((bool)rearLightsMats[i])
			{
				Material mat = rearLightsMats[i];
				CommonExtensions.DestroyMat(ref mat);
			}
		}
		for (int j = 0; j < BrakeMat2.Count; j++)
		{
			if ((bool)BrakeMat2[j])
			{
				Material mat2 = BrakeMat2[j];
				CommonExtensions.DestroyMat(ref mat2);
			}
		}
		if (_leftHaloMat != null)
		{
			for (int k = 0; k < _leftHaloMat.Length; k++)
			{
				if ((bool)_leftHaloMat[k])
				{
					CommonExtensions.DestroyMat(ref _leftHaloMat[k]);
				}
			}
		}
		if (_rightHaloMat != null)
		{
			for (int l = 0; l < _rightHaloMat.Length; l++)
			{
				if ((bool)_rightHaloMat[l])
				{
					CommonExtensions.DestroyMat(ref _rightHaloMat[l]);
				}
			}
		}
		_isDestroyed = true;
	}

	private void OnTurnLightsAutoToggled()
	{
		TurnOffAllIndicators();
		if ((bool)_vehicleData && _vehicleData.FreeDriveProvider != null && (bool)_vehicleData.FreeDriveProvider.QuickCarSettings)
		{
			_vehicleData.FreeDriveProvider.QuickCarSettings.OnTurnLightsAutoTurnOff();
		}
	}

	private void CreateDefaultIntensity()
	{
		if (lightsIntensity.Length == 0)
		{
			lightsIntensity = new float[3] { 0f, 1f, 2f };
		}
		if (lightsIntensity2.Length == 0)
		{
			lightsIntensity2 = new float[3] { 0f, 1f, 2f };
		}
		if (frontLightIntensity.Length == 0)
		{
			frontLightIntensity = new float[3] { 0f, 1f, 2f };
		}
		if (rearBreakIntensity.Length == 0)
		{
			rearBreakIntensity = new float[3] { 0f, 1f, 2f };
		}
		if (rearLightIntensity.Length == 0)
		{
			rearLightIntensity = new float[3] { 0f, 1f, 2f };
		}
		if (reverseIntensity.Length == 0)
		{
			reverseIntensity = new float[3] { 0f, 1f, 2f };
		}
		if (turnIntensity.Length == 0)
		{
			turnIntensity = new float[3] { 0.7f, 1f, 2f };
		}
		if (turnIntensity2.Length == 0)
		{
			turnIntensity2 = new float[3] { 0.7f, 1f, 2f };
		}
		if (turnIntensity3.Length == 0)
		{
			turnIntensity3 = new float[3] { 1f, 1f, 3f };
		}
	}

	private void Start()
	{
		IndicatorState = new ReactiveProperty<TurnSignalState>(currentState);
		IndicatorState.Subscribe((TurnSignalState value) =>
		{
			HandleTurnSignalState(value, 0.38);
		}).AddTo(this);
		if (_hasNewLightsController)
		{
			if (!_vehicleData.MainModel)
			{
				_vehicleData.OnCarGraphicLoaded += InitNewLights;
			}
			else
			{
				InitNewLights();
			}
		}
		_soundBankService = (_vehicleData ? _vehicleData.SoundBankService : null);
		if ((bool)_vehicleData && !_vehicleData.MainModel)
		{
			_vehicleData.OnCarGraphicLoaded += TryHoldHeadlightsBank;
		}
		else
		{
			TryHoldHeadlightsBank();
		}
		if ((bool)IndicatorDiodMat && (bool)_vehicleData.LightsComposition.DiodMat2)
		{
			indicatorColorDiod = _vehicleData.LightsComposition.DiodMat2.color;
		}
		_intensity = Shader.PropertyToID("_Intensity");
		TintId = Shader.PropertyToID("_TintColor");
		indicatorTransparent = indicatorColorYellow;
		indicatorTransparent.a = 0f;
		indicatorColorYellow.a = 0.1f;
		_brakeColorTransParentRed = _brakeColorRed;
		_brakeColorRed.a = 0.1f;
		_brakeColorTransParentRed.a = 0f;
		rearWhite = Color.white;
		rearWhite.a = 0.5f;
		rearColorTransparent = Color.yellow;
		rearColorTransparent.a = 0f;
		_addictive = Shader.Find("Legacy Shaders/Particles/Additive");
		_leftIndicators = new MeshRenderer[6];
		_rightIndicators = new MeshRenderer[6];
		_leftHaloClr = new Color[6];
		_rightHaloClr = new Color[6];
		_leftHaloMat = new Material[6];
		_rightHaloMat = new Material[6];
		HFtweens = new GameObject[2];
		HRtweens = new GameObject[2];
		_brakes = new MeshRenderer[2];
		_rear = new MeshRenderer[2];
		_collider = null;
		if ((bool)_vehicleData)
		{
			_collider = _vehicleData.Collider.GetComponent<MeshCollider>();
		}
		if (_collider == null && (bool)_carChildsReferences && (bool)_carChildsReferences.Collider)
		{
			_collider = _carChildsReferences.Collider.GetComponent<MeshCollider>();
		}
		if (!lights)
		{
			lights = base.gameObject.transform.GetChild(0);
		}
		FindInChildIndicators();
		if (!HeadLightsParent)
		{
			CreateHeadLights();
		}
		SetCarIndicatorColor();
		ResetAllLights();
		if ((bool)_axles)
		{
			_axles.IsBrakePressed.Subscribe(BrakeLights).AddTo(this);
		}
	}

	private void OnEnable()
	{
		SubscribeEngineListeners();
		if ((bool)_powertrain && _powertrain.IsEngineWorks)
		{
			TurnOnDaytimeRunningLights();
		}
		CPMGraphicsSettingsManager.Instance.OnQualityChanged += OnQualityChanged;
	}

	private void OnDisable()
	{
		UnSubscribeEngineListeners();
		TurnOffDaytimeRunningLights();
		CPMGraphicsSettingsManager.Instance.OnQualityChanged -= OnQualityChanged;
	}

	private void OnQualityChanged(int quality)
	{
		if ((bool)mainLight)
		{
			mainLight.cullingMask = -1;
		}
	}

	private void ReverseLight()
	{
		if (_powertrain.Gear == -1)
		{
			TurnOnReverseLights();
		}
		else
		{
			TurnOffReverseLights();
		}
	}

	private void SetCarIndicatorColor()
	{
		Color color = ((PlayerPrefs.GetInt("DarkButtons", 0) == 1) ? HudColorChanger.DarkColor : HudColorChanger.DefaultColor);
		color.a = 0.8f;
		indicatorColor = color;
	}

	private void CreateHeadLights()
	{
		if (!_collider)
		{
			return;
		}
		float z = _collider.bounds.size.z;
		HLLeft = UnityEngine.Object.Instantiate(Resources.Load("HeadLight2", typeof(GameObject)), new Vector3(0f, 0f, z / 2f), Quaternion.identity, base.transform) as GameObject;
		Vector3 localPosition = new Vector3(_collider.sharedMesh.bounds.size.x / 2f, 0f, z / 2f);
		HLLeft.transform.localPosition = localPosition;
		HLLeftLight = HLLeft.transform.GetChild(0).gameObject;
		Transform obj = HLLeftLight.transform;
		obj.localPosition = Vector3.zero;
		obj.localRotation = Quaternion.identity;
		obj.localScale = Vector3.one;
		if (obj.TryGetComponent<Light>(out var component))
		{
			component.spotAngle = LightSetting.OuterSpotAngle;
			component.range = LightSetting.LightDistance;
			component.intensity = 0.01f;
			bool flag = CPMGraphicsSettingsManager.Instance.ShadingQuality == CPMGraphicsSettings.ShadersQuality.Potato;
			component.cookie = (flag ? null : LightSetting.LowBeamCookie);
			component.innerSpotAngle = (flag ? 0f : LightSetting.InnerSpotAngle);
			if (ScenesHelper.IsParkingLevelActive())
			{
				component.cullingMask = -1;
			}
		}
		GameObject obj2 = new GameObject("HeadLightsParent");
		obj2.transform.parent = base.gameObject.transform;
		obj2.transform.localPosition = Vector3.zero;
		obj2.transform.localEulerAngles = Vector3.zero;
		HeadLightsParent = obj2;
		HLLeft.transform.parent = HeadLightsParent.transform;
		RemoveFrontLightCone();
	}

	private void PlayIndicatorSound(bool play)
	{
		if (!AudioWrapper.IsInitialized())
		{
			return;
		}
		if (play && base.gameObject == V.Car)
		{
			if (!_isIndicatorSoundPlaying)
			{
				AudioWrapper.PostEvent("Play_indicator", base.gameObject, 1u, CallBackIndicator, base.gameObject);
				_isIndicatorSoundPlaying = true;
			}
		}
		else if (_isIndicatorSoundPlaying)
		{
			AudioWrapper.PostEvent("Stop_indicator", base.gameObject);
		}
	}

	private void CallBackIndicator(object in_cookie, AkCallbackType callType, object in_info)
	{
		if (callType == AkCallbackType.AK_EndOfEvent)
		{
			_isIndicatorSoundPlaying = false;
		}
	}

	private void RemoveFrontLightCone()
	{
		if ((bool)HLLeft)
		{
			if (HLLeft.TryGetComponent<MeshFilter>(out var component))
			{
				UnityEngine.Object.Destroy(component);
			}
			if (HLLeft.TryGetComponent<MeshRenderer>(out var component2))
			{
				UnityEngine.Object.Destroy(component2);
			}
			if (HLLeft.TryGetComponent<Animator>(out var component3))
			{
				UnityEngine.Object.Destroy(component3);
			}
		}
	}

	private void SetQualityLight()
	{
		if ((bool)mainLight)
		{
			float currentFPS = _menuLevelParent.FPSDisplay2.CurrentFPS;
			if (!(currentFPS < 8f) && (currentFPS > 25f || (V.Car == base.gameObject && _menuLevelParent.FPSDisplay2.CurrentFPS > 15f)))
			{
				mainLight.enabled = true;
				mainLight.renderMode = LightRenderMode.ForcePixel;
			}
			else
			{
				mainLight.enabled = false;
			}
		}
	}

	public void turnOnNearLights()
	{
		lightState = true;
		_farLightsOn = false;
		SetQualityLight();
		if ((bool)HLLeft)
		{
			GameObject[] hRtweens = HRtweens;
			foreach (GameObject gameObject in hRtweens)
			{
				if ((bool)gameObject)
				{
					gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
				}
			}
			HLLeft.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			HLLeft.SetActive(value: true);
			HLLeftLight = HLLeft.transform.GetChild(0).gameObject;
			if (HLLeftLight.transform.TryGetComponent<Light>(out var component))
			{
				bool flag = CPMGraphicsSettingsManager.Instance.ShadingQuality == CPMGraphicsSettings.ShadersQuality.Potato;
				component.spotAngle = LightSetting.OuterSpotAngle;
				component.cookie = (flag ? null : LightSetting.LowBeamCookie);
				component.intensity = (flag ? (LightSetting.LightIntensity * 0.3f) : LightSetting.LightIntensity);
				component.innerSpotAngle = (flag ? 0f : LightSetting.InnerSpotAngle);
			}
		}
		if (_hasNewLightsController && GetHdHeadlighsController.isInitialized)
		{
			GetHdHeadlighsController.SwitchHightBeamLights(isOn: false);
			GetHdHeadlighsController.SwitchLowBeamLights(isOn: true);
		}
		else
		{
			if ((bool)HeadlightsMats && frontLightIntensity.Length > 1)
			{
				HeadlightsMats.SetFloat(_intensity, frontLightIntensity[1]);
			}
			if (rearLightsMats != null && rearLightIntensity.Length > 1)
			{
				foreach (Material rearLightsMat in rearLightsMats)
				{
					if ((bool)rearLightsMat)
					{
						rearLightsMat.SetFloat(_intensity, rearLightIntensity[1]);
					}
				}
			}
			if ((bool)lightsMat2 && lightsIntensity2.Length > 1)
			{
				lightsMat2.SetFloat(_intensity, lightsIntensity2[1]);
			}
		}
		if ((bool)lightsMat)
		{
			lightsMat.SetFloat(_intensity, BodyKitLightsIntensity(1));
		}
		TryOpenCloseLight(isOpen: true);
	}

	public void turnOnFarLights()
	{
		lightState = true;
		_farLightsOn = true;
		SetQualityLight();
		if (HFtweens != null && HRtweens != null)
		{
			GameObject[] hRtweens = HRtweens;
			foreach (GameObject gameObject in hRtweens)
			{
				if ((bool)gameObject && gameObject.TryGetComponent<MeshRenderer>(out var component))
				{
					component.material.color = Color.white;
				}
			}
			if ((bool)HLLeft)
			{
				HLLeft.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
				HLLeft.SetActive(value: true);
				HLLeftLight = HLLeft.transform.GetChild(0).gameObject;
				if (HLLeftLight.transform.TryGetComponent<Light>(out var component2))
				{
					bool flag = CPMGraphicsSettingsManager.Instance.ShadingQuality == CPMGraphicsSettings.ShadersQuality.Potato;
					component2.spotAngle = LightSetting.OuterSpotAngle;
					component2.cookie = (flag ? null : LightSetting.HighBeamCookie);
					component2.intensity = (flag ? (LightSetting.LightIntensity * 1.5f * 0.3f) : (LightSetting.LightIntensity * 1.5f));
					component2.innerSpotAngle = (flag ? 0f : LightSetting.InnerSpotAngle);
				}
			}
		}
		if (_hasNewLightsController && GetHdHeadlighsController.isInitialized)
		{
			GetHdHeadlighsController.SwitchHightBeamLights(isOn: true);
		}
		else if ((bool)HeadlightsMats && frontLightIntensity.Length > 2)
		{
			HeadlightsMats.SetFloat(_intensity, frontLightIntensity[2]);
		}
		if ((bool)lightsMat)
		{
			lightsMat.SetFloat(_intensity, BodyKitLightsIntensity(2));
		}
		TryOpenCloseLight(isOpen: true);
	}

	private void TryHoldHeadlightsBank()
	{
		if (!_headlightsBankHeld && _soundBankService != null && (bool)_vehicleData && (bool)_vehicleData.MainModel && (bool)_vehicleData.BodyLoadingComposition.BodyMarker && (((bool)_vehicleData.BodyLoadingComposition.BodyMarker.frontLightLeft && _vehicleData.BodyLoadingComposition.BodyMarker.frontLightLeft.TryGetComponent<Animator>(out var component)) || ((bool)_vehicleData.BodyLoadingComposition.BodyMarker.frontLightRight && _vehicleData.BodyLoadingComposition.BodyMarker.frontLightRight.TryGetComponent<Animator>(out component))))
		{
			_soundBankService.TryLoadBank(SoundBankType.CarHeadlights).Forget();
			_headlightsBankHeld = true;
		}
	}

	private void TryOpenCloseLight(bool isOpen)
	{
		if (!_vehicleData || !_vehicleData.MainModel)
		{
			return;
		}
		BodyMarker component = _vehicleData.MainModel.GetComponent<BodyMarker>();
		if (!component)
		{
			return;
		}
		GameObject[] array = new GameObject[2] { component.frontLightLeft, component.frontLightRight };
		bool flag = false;
		for (int i = 0; i < array.Length; i++)
		{
			if ((bool)array[i] && array[i].TryGetComponent<Animator>(out var component2) && component2.GetBool(OpenAnimatorInt) != isOpen)
			{
				component2.SetBool(OpenAnimatorInt, isOpen);
				if (!flag)
				{
					AudioWrapper.PostEvent(isOpen ? Play_headlights_open : Play_headlights_close, base.gameObject);
				}
				flag = true;
			}
		}
		OnSwitchLights?.Invoke(isOpen);
	}

	public void turnOffHeadLights()
	{
		lightState = false;
		_farLightsOn = false;
		GameObject[] hFtweens = HFtweens;
		foreach (GameObject gameObject in hFtweens)
		{
			if ((bool)gameObject)
			{
				gameObject.gameObject.SetActive(value: false);
			}
		}
		hFtweens = HRtweens;
		foreach (GameObject gameObject2 in hFtweens)
		{
			if ((bool)gameObject2)
			{
				gameObject2.GetComponent<MeshRenderer>().material.color = Color.black;
			}
		}
		if ((bool)HLLeft)
		{
			HLLeft.SetActive(value: false);
		}
		if (_hasNewLightsController && GetHdHeadlighsController.isInitialized)
		{
			GetHdHeadlighsController.SwitchHightBeamLights(isOn: false);
			GetHdHeadlighsController.SwitchLowBeamLights(isOn: false);
		}
		else
		{
			if ((bool)HeadlightsMats && frontLightIntensity.Length != 0)
			{
				HeadlightsMats.SetFloat(_intensity, frontLightIntensity[0]);
			}
			if (rearLightsMats != null && rearLightsMats.Count > 0)
			{
				foreach (Material rearLightsMat in rearLightsMats)
				{
					if ((bool)rearLightsMat && rearLightIntensity.Length != 0 && (bool)rearLightsMat)
					{
						rearLightsMat.SetFloat(_intensity, rearLightIntensity[0]);
					}
				}
			}
			if (BrakeMat2 != null && BrakeMat2.Count > 0)
			{
				foreach (Material item in BrakeMat2)
				{
					if ((bool)item && rearBreakIntensity.Length != 0 && (bool)item)
					{
						item.SetFloat(_intensity, rearBreakIntensity[0]);
					}
				}
			}
			if ((bool)reverseMat && reverseIntensity.Length != 0)
			{
				reverseMat.SetFloat(_intensity, reverseIntensity[0]);
			}
			if ((bool)LeftIndicatorMat2 && (bool)RightIndicatorMat2 && turnIntensity.Length != 0)
			{
				LeftIndicatorMat2.SetFloat(_intensity, turnIntensity[0]);
				RightIndicatorMat2.SetFloat(_intensity, turnIntensity[0]);
			}
			if ((bool)LeftIndicatorMat3 && (bool)RightIndicatorMat3 && turnIntensity2.Length != 0)
			{
				LeftIndicatorMat3.SetFloat(_intensity, turnIntensity2[0]);
				RightIndicatorMat3.SetFloat(_intensity, turnIntensity2[0]);
			}
			if ((bool)LeftIndicatorMat4 && (bool)RightIndicatorMat4 && turnIntensity3.Length != 0)
			{
				LeftIndicatorMat4.SetFloat(_intensity, turnIntensity3[0]);
				RightIndicatorMat4.SetFloat(_intensity, turnIntensity3[0]);
			}
			if ((bool)lightsMat2 && lightsIntensity2.Length != 0)
			{
				lightsMat2.SetFloat(_intensity, lightsIntensity2[0]);
			}
		}
		if ((bool)lightsMat)
		{
			lightsMat.SetFloat(_intensity, BodyKitLightsIntensity(0));
		}
		TryOpenCloseLight(isOpen: false);
	}

	public void FindInChildIndicators()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		foreach (Transform item in lights.transform)
		{
			switch (item.name)
			{
			case "LI":
			{
				_leftIndicators[num] = item.GetComponent<MeshRenderer>();
				_leftHaloClr[num] = (item.TryGetComponent<TurnLightHalo>(out var component2) ? component2.haloColor : indicatorColorYellow);
				if (!_leftHaloMat[num])
				{
					_leftHaloMat[num] = _leftIndicators[num].material;
					_leftHaloMat[num].shader = _addictive;
				}
				else
				{
					_leftIndicators[num].material = _leftHaloMat[num];
				}
				num++;
				break;
			}
			case "RI":
			{
				_rightIndicators[num2] = item.GetComponent<MeshRenderer>();
				_rightHaloClr[num2] = (item.TryGetComponent<TurnLightHalo>(out var component3) ? component3.haloColor : indicatorColorYellow);
				if (!_rightHaloMat[num2])
				{
					_rightHaloMat[num2] = _rightIndicators[num2].material;
					_rightHaloMat[num2].shader = _addictive;
				}
				else
				{
					_rightIndicators[num2].material = _rightHaloMat[num2];
				}
				num2++;
				break;
			}
			case "HF":
				HFtweens[num3] = item.gameObject;
				num3++;
				break;
			case "HR":
				HRtweens[num4] = item.gameObject;
				num4++;
				break;
			case "HeadLights":
			{
				HeadLightsParent = item.gameObject;
				HeadLightsParent.transform.localScale = Vector3.one;
				HeadLightsParent.SetActive(value: true);
				HLLeft = item.GetChild(0).gameObject;
				HLLeftLight = HLLeft.transform.GetChild(0).gameObject;
				Transform obj = HLLeftLight.transform;
				obj.localPosition = Vector3.zero;
				obj.localRotation = Quaternion.identity;
				obj.localScale = Vector3.one;
				if (obj.TryGetComponent<Light>(out var component4))
				{
					bool flag = CPMGraphicsSettingsManager.Instance.ShadingQuality == CPMGraphicsSettings.ShadersQuality.Potato;
					component4.spotAngle = LightSetting.OuterSpotAngle;
					component4.range = LightSetting.LightDistance;
					component4.intensity = (flag ? (LightSetting.LightIntensity * 0.3f) : LightSetting.LightIntensity);
					component4.cookie = (flag ? null : LightSetting.LowBeamCookie);
					component4.innerSpotAngle = (flag ? 0f : LightSetting.InnerSpotAngle);
					component4.cullingMask = -1;
				}
				HLRight = item.GetChild(1).gameObject;
				HLRightLight = HLRight.transform.GetChild(0).gameObject;
				Vector3 localPosition = (HLLeft.transform.localPosition + HLRight.transform.localPosition) / 2f;
				HeadLightsParent.transform.localPosition = new Vector3(HeadLightsParent.transform.localPosition.x, HeadLightsParent.transform.localPosition.y, 0f);
				if ((bool)_collider)
				{
					float z = _collider.sharedMesh.bounds.size.z / 2f + _collider.transform.localPosition.z;
					localPosition = new Vector3(localPosition.x, localPosition.y, z);
				}
				HLLeft.transform.localPosition = localPosition;
				HLLeft.transform.localScale = Vector3.one;
				mainLight = HLLeft.transform.GetChild(0).GetComponent<Light>();
				UnityEngine.Object.Destroy(HLRight);
				HLRight = null;
				HLRightLight = null;
				RemoveFrontLightCone();
				HLLeft.SetActive(value: true);
				break;
			}
			case "Brake":
			{
				MeshRenderer component = item.GetComponent<MeshRenderer>();
				if (component.material.shader == _addictive)
				{
					_brakes[num5] = component;
					if (!BrakeMat1)
					{
						BrakeMat1 = component.material;
					}
					else
					{
						component.material = BrakeMat1;
					}
					num6++;
					num5++;
				}
				break;
			}
			}
			SetActiveMeshRenderer(active: false, _brakes);
			SetActiveMeshRenderer(active: false, _rear);
		}
	}

	public void EmergencyLightOnForTime(float time)
	{
		if (IndicatorState.Value != TurnSignalState.Hazard)
		{
			_disposableTurnOffAllIndicators?.Dispose();
			_disposableTurnOffAllIndicators = Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe((long _) =>
			{
				TurnOffAllIndicators();
			}).AddTo(this);
		}
		EmergencyLightsOn();
	}

	public void EmergencyLightsOn()
	{
		if (IndicatorState.Value != TurnSignalState.Hazard)
		{
			ResetIndicatorsMaterialsColor();
			IndicatorState.Value = TurnSignalState.Hazard;
			if ((bool)GetHdHeadlighsController && GetHdHeadlighsController.isInitialized)
			{
				GetHdHeadlighsController.SwitchHazardLights(!GetHdHeadlighsController.GetHazardLights());
			}
			else
			{
				SetActiveMeshRenderer(active: true, _rightIndicators);
				SetActiveMeshRenderer(active: true, _leftIndicators);
			}
			PlayIndicatorSound(play: true);
		}
	}

	public void TurnOnLeftIndicators()
	{
		ResetIndicatorsMaterialsColor();
		IndicatorState.Value = TurnSignalState.Left;
		if (_hasNewLightsController)
		{
			GetHdHeadlighsController.SwitchLeftLights();
		}
		else
		{
			SetActiveMeshRenderer(active: false, _rightIndicators);
			SetActiveMeshRenderer(active: true, _leftIndicators);
		}
		PlayIndicatorSound(play: true);
		if ((bool)mppc && !mppc.isOnlineCar)
		{
			carTurnLightsAutoToggler.TurnLightToggledOn(-1);
		}
	}

	public void TurnOnRightIndicators()
	{
		ResetIndicatorsMaterialsColor();
		IndicatorState.Value = TurnSignalState.Right;
		if (_hasNewLightsController)
		{
			GetHdHeadlighsController.SwitchRightLights();
		}
		else
		{
			SetActiveMeshRenderer(active: true, _rightIndicators);
			SetActiveMeshRenderer(active: false, _leftIndicators);
		}
		PlayIndicatorSound(play: true);
		if ((bool)mppc && !mppc.isOnlineCar)
		{
			carTurnLightsAutoToggler.TurnLightToggledOn(1);
		}
	}

	public void TurnOnDaytimeRunningLights()
	{
		SwitchDaytimeRunningLights(state: true);
	}

	public void TurnOffDaytimeRunningLights()
	{
		SwitchDaytimeRunningLights(state: false);
	}

	private void SwitchDaytimeRunningLights(bool state)
	{
		if ((bool)GetHdHeadlighsController && GetHdHeadlighsController.isInitialized && !CarIdStorage.CarsNotUseDrl.Contains(_vehicleData.CarId))
		{
			GetHdHeadlighsController.SwitchDaytimeRunningLights(state);
		}
	}

	public void TurnOffAllIndicators()
	{
		ResetIndicatorsMaterialsColor();
		if ((bool)GetHdHeadlighsController)
		{
			if (GetHdHeadlighsController.isInitialized)
			{
				GetHdHeadlighsController.SwitchHazardLights(isOn: false);
			}
		}
		else
		{
			SetActiveMeshRenderer(active: false, _rightIndicators);
			SetActiveMeshRenderer(active: false, _leftIndicators);
		}
		IndicatorState.Value = TurnSignalState.Off;
		PlayIndicatorSound(play: false);
		if ((bool)mppc && !mppc.isOnlineCar)
		{
			carTurnLightsAutoToggler?.TurnLightToggledOff();
		}
		if ((bool)_menuLevelParent)
		{
			Color color = ((PlayerPrefs.GetInt("DarkButtons", 0) == 1) ? HudColorChanger.DarkColor : HudColorChanger.DefaultColor);
			color.a = 0.5f;
			if ((bool)_menuLevelParent.RightIndicatorArrowTC)
			{
				_menuLevelParent.RightIndicatorArrowTC.color = color;
			}
			if ((bool)_menuLevelParent.LeftIndicatorArrowTC)
			{
				_menuLevelParent.LeftIndicatorArrowTC.color = color;
			}
			if ((bool)_menuLevelParent.EmergencyTC)
			{
				_menuLevelParent.EmergencyTC.color = color;
			}
		}
	}

	public void TurnOnReverseLights()
	{
		if (_hasNewLightsController && GetHdHeadlighsController.isInitialized)
		{
			GetHdHeadlighsController.SwitchReverseLight(isOn: true);
			return;
		}
		SetActiveMeshRenderer(active: true, _rear);
		if ((bool)reverseMat)
		{
			float[] array = reverseIntensity;
			if (array != null && array.Length > 1)
			{
				reverseMat.SetFloat(_intensity, reverseIntensity[1]);
			}
		}
	}

	public void TurnOffReverseLights()
	{
		if (_hasNewLightsController && GetHdHeadlighsController.isInitialized)
		{
			GetHdHeadlighsController.SwitchReverseLight(isOn: false);
			return;
		}
		SetActiveMeshRenderer(active: false, _rear);
		if ((bool)reverseMat)
		{
			float[] array = reverseIntensity;
			if (array != null && array.Length > 0)
			{
				reverseMat.SetFloat(_intensity, reverseIntensity[0]);
			}
		}
	}

	private void SendBrakesState(bool breaking)
	{
		if (FlagsStorage.IsMultiFree && !FlagsStorage.InGarageFree && (bool)mppc && !mppc.isOnlineCar)
		{
			mppc.IsBreaking = breaking;
		}
	}

	private void BrakeLights(bool brakePressed)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (brakePressed)
		{
			if (_hasNewLightsController && GetHdHeadlighsController.isInitialized)
			{
				GetHdHeadlighsController.SwitchBrakeLights(isOn: true);
			}
			else
			{
				TurnOnBrakesLights().Subscribe().AddTo(this);
			}
			SendBrakesState(breaking: true);
		}
		else
		{
			if (_hasNewLightsController && GetHdHeadlighsController.isInitialized)
			{
				GetHdHeadlighsController.SwitchBrakeLights(isOn: false);
			}
			else
			{
				TurnOffBrakesLights().Subscribe().AddTo(this);
			}
			SendBrakesState(breaking: false);
		}
	}

	private IObservable<Unit> TurnOnBrakesLights()
	{
		float duration = 0.2f;
		return (from _ in Observable.EveryUpdate()
			select Time.deltaTime).Scan((float acc, float current) => acc + current).TakeWhile((float elapsedTime) => elapsedTime < duration).Do((float elapsedTime) =>
		{
			SetActiveMeshRenderer(active: true, _brakes);
			float t = elapsedTime / duration;
			_lerpedColorBrake1 = Color.Lerp(_lerpedColorBrake1, _brakeColorRed, t);
			_lerpedColorBrake2 = Color.Lerp(_lerpedColorBrake2, _brakeColorRed, t);
			if ((bool)BrakeMat1)
			{
				BrakeMat1.SetColor(TintId, _lerpedColorBrake1);
			}
			List<Material> brakeMat = BrakeMat2;
			if ((brakeMat != null && brakeMat.Count > 0) || littleBrake)
			{
				_intensityRearLight = Mathf.Lerp(_intensityRearLight, rearLightIntensity[2], t);
				if (rearLightsMats != null)
				{
					foreach (Material item in rearLightsMats.Where((Material mat) => mat))
					{
						item.SetFloat(_intensity, _intensityRearLight);
					}
				}
			}
			brakeMat = BrakeMat2;
			if (brakeMat != null && brakeMat.Count > 0)
			{
				_intensityBrakeLight = Mathf.Lerp(_intensityBrakeLight, rearBreakIntensity[1], t);
				foreach (Material item2 in BrakeMat2.Where((Material mat) => mat))
				{
					item2.SetFloat(_intensity, _intensityBrakeLight);
				}
			}
			if (!_turnLeftOn && (bool)LeftIndicatorMat5)
			{
				LeftIndicatorMat5.color = _turnOnColorLights[3];
				_intensityBrakeLight = Mathf.Lerp(_intensityBrakeLight, rearBreakIntensity[1], t);
				LeftIndicatorMat5.SetFloat(_intensity, _intensityBrakeLight);
			}
			if (!_turnRightOn && (bool)RightIndicatorMat5)
			{
				RightIndicatorMat5.color = _turnOnColorLights[3];
				_intensityBrakeLight = Mathf.Lerp(_intensityBrakeLight, rearBreakIntensity[1], t);
				RightIndicatorMat5.SetFloat(_intensity, _intensityBrakeLight);
			}
		})
			.AsUnitObservable()
			.Concat(Observable.Defer(Observable.ReturnUnit));
	}

	private IObservable<Unit> TurnOffBrakesLights()
	{
		float duration = 0.2f;
		return (from _ in Observable.EveryUpdate()
			select Time.deltaTime).Scan((float acc, float current) => acc + current).TakeWhile((float elapsedTime) => elapsedTime < duration).Do((float elapsedTime) =>
		{
			float t = elapsedTime / duration;
			_lerpedColorBrake1 = Color.Lerp(_lerpedColorBrake1, _brakeColorTransParentRed, t);
			_lerpedColorBrake2 = Color.Lerp(_lerpedColorBrake2, indicatorColorGray, t);
			if ((bool)BrakeMat1)
			{
				BrakeMat1.SetColor(TintId, _lerpedColorBrake1);
			}
			List<Material> brakeMat = BrakeMat2;
			if ((brakeMat != null && brakeMat.Count > 0) || littleBrake)
			{
				_intensityRearLight = Mathf.Lerp(_intensityRearLight, lightState ? rearLightIntensity[1] : rearLightIntensity[0], t);
				brakeMat = rearLightsMats;
				if (brakeMat != null && brakeMat.Count > 0)
				{
					foreach (Material item in rearLightsMats.Where((Material mat) => mat))
					{
						item.SetFloat(_intensity, _intensityRearLight);
					}
				}
			}
			brakeMat = BrakeMat2;
			if (brakeMat != null && brakeMat.Count > 0)
			{
				_intensityBrakeLight = Mathf.Lerp(_intensityBrakeLight, rearBreakIntensity[0], Time.deltaTime * 8f);
				foreach (Material item2 in BrakeMat2.Where((Material mat) => mat))
				{
					item2.SetFloat(_intensity, _intensityBrakeLight);
				}
			}
			if (!_turnLeftOn && (bool)LeftIndicatorMat5)
			{
				LeftIndicatorMat5.color = indicatorColorGray;
				LeftIndicatorMat5.SetFloat(_intensity, _intensityRearLight);
			}
			if (!_turnRightOn && (bool)RightIndicatorMat5)
			{
				RightIndicatorMat5.color = indicatorColorGray;
				RightIndicatorMat5.SetFloat(_intensity, _intensityRearLight);
			}
		})
			.AsUnitObservable()
			.Concat(Observable.Defer(() =>
			{
				SetActiveMeshRenderer(active: false, _brakes);
				return Observable.ReturnUnit();
			}));
	}

	public void TurnOnOffBrakesLightsForDebug(bool isOn)
	{
		if (isOn)
		{
			TurnOnBrakesLights().Subscribe().AddTo(this);
		}
		else
		{
			TurnOffBrakesLights().Subscribe().AddTo(this);
		}
	}

	private void HandleTurnSignalState(TurnSignalState state, double timer)
	{
		_turnSignalSubscription?.Dispose();
		if (_isDestroyed)
		{
			return;
		}
		CommonExtensions.NullIfDestroyed(ref mppc);
		switch (state)
		{
		case TurnSignalState.Left:
			currentState = TurnSignalState.Left;
			_turnSignalSubscription = (from x in Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(timer))
				select (x % 2 != 0L) ? 0f : 1f).Subscribe((float intensity) =>
			{
				if (!_hasNewLightsController)
				{
					for (int i = 0; i < _leftHaloMat.Length; i++)
					{
						if ((bool)_leftHaloMat[i])
						{
							_indicatorHaloMainColor = Color.Lerp(indicatorTransparent, _leftHaloClr[i], intensity);
							_leftHaloMat[i].SetColor(TintId, _indicatorHaloMainColor);
						}
					}
					_indicatorMainColor = Color.Lerp(indicatorColorGray, indicatorColorYellow, intensity);
					if ((bool)LeftIndicatorMat2)
					{
						_indicatorMainColor = Color.Lerp(indicatorColorGray, _turnOnColorLights[0], intensity);
						LeftIndicatorMat2.color = _indicatorMainColor;
						float value = Mathf.Lerp(turnIntensity[0], turnIntensity[2], intensity);
						LeftIndicatorMat2.SetFloat(_intensity, value);
					}
					if ((bool)LeftIndicatorMat3)
					{
						_indicatorMainColor = Color.Lerp(indicatorColorGray, _turnOnColorLights[1], intensity);
						LeftIndicatorMat3.color = _indicatorMainColor;
						float value2 = Mathf.Lerp(turnIntensity[0], turnIntensity2[2], intensity);
						LeftIndicatorMat3.SetFloat(_intensity, value2);
					}
					if ((bool)LeftIndicatorMat4)
					{
						_indicatorMainColor = Color.Lerp(indicatorColorGray, _turnOnColorLights[2], intensity);
						LeftIndicatorMat4.color = _indicatorMainColor;
						float value3 = Mathf.Lerp(turnIntensity[0], turnIntensity3[2], intensity);
						LeftIndicatorMat4.SetFloat(_intensity, value3);
					}
					if ((bool)LeftIndicatorMat5)
					{
						_indicatorMainColor = Color.Lerp(indicatorColorGray, _turnOnColorLights[3], intensity);
						LeftIndicatorMat5.color = _indicatorMainColor;
						float value4 = Mathf.Lerp(turnIntensity[0], turnIntensity3[2], intensity);
						LeftIndicatorMat5.SetFloat(_intensity, value4);
						_turnLeftOn = true;
					}
					if ((bool)turnLeftDiodMat)
					{
						turnLeftDiodMat.color = _indicatorMainColor;
						float value5 = Mathf.Lerp(turnIntensity[0], turnIntensity2[2], intensity);
						turnLeftDiodMat.SetFloat(_intensity, value5);
					}
				}
				_indicatorMainColor = Color.Lerp(indicatorColorYellow, indicatorColorGray, intensity);
				if (base.gameObject == V.Car)
				{
					indicatorColor.a = _indicatorMainColor.a + 0.3f;
					if ((bool)_menuLevelParent && (bool)_menuLevelParent.LeftIndicatorArrowTC)
					{
						_menuLevelParent.LeftIndicatorArrowTC.color = indicatorColor;
					}
				}
			}).AddTo(this);
			break;
		case TurnSignalState.Right:
			currentState = TurnSignalState.Right;
			_turnSignalSubscription = (from x in Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(timer))
				select (x % 2 != 0L) ? 0f : 1f).Subscribe((float intensity) =>
			{
				if (!_hasNewLightsController)
				{
					for (int i = 0; i < _rightHaloMat.Length; i++)
					{
						if ((bool)_rightHaloMat[i])
						{
							_indicatorHaloMainColor = Color.Lerp(indicatorTransparent, _rightHaloClr[i], intensity);
							_rightHaloMat[i].SetColor(TintId, _indicatorHaloMainColor);
						}
					}
					_indicatorMainColor = Color.Lerp(indicatorColorGray, indicatorColorYellow, intensity);
					if ((bool)RightIndicatorMat2)
					{
						_indicatorMainColor = Color.Lerp(indicatorColorGray, _turnOnColorLights[0], intensity);
						RightIndicatorMat2.color = _indicatorMainColor;
						float value = Mathf.Lerp(turnIntensity[0], turnIntensity[2], intensity);
						RightIndicatorMat2.SetFloat(_intensity, value);
					}
					if ((bool)RightIndicatorMat3)
					{
						_indicatorMainColor = Color.Lerp(indicatorColorGray, _turnOnColorLights[1], intensity);
						RightIndicatorMat3.color = _indicatorMainColor;
						float value2 = Mathf.Lerp(turnIntensity[0], turnIntensity2[2], intensity);
						RightIndicatorMat3.SetFloat(_intensity, value2);
					}
					if ((bool)RightIndicatorMat4)
					{
						_indicatorMainColor = Color.Lerp(indicatorColorGray, _turnOnColorLights[2], intensity);
						RightIndicatorMat4.color = _indicatorMainColor;
						float value3 = Mathf.Lerp(turnIntensity[0], turnIntensity3[2], intensity);
						RightIndicatorMat4.SetFloat(_intensity, value3);
					}
					if ((bool)RightIndicatorMat5)
					{
						_indicatorMainColor = Color.Lerp(indicatorColorGray, _turnOnColorLights[3], intensity);
						RightIndicatorMat5.color = _indicatorMainColor;
						float value4 = Mathf.Lerp(turnIntensity[0], turnIntensity3[2], intensity);
						RightIndicatorMat5.SetFloat(_intensity, value4);
						_turnRightOn = true;
					}
					if ((bool)turnRightDiodMat)
					{
						turnRightDiodMat.color = _indicatorMainColor;
						float value5 = Mathf.Lerp(turnIntensity[0], turnIntensity2[2], intensity);
						turnRightDiodMat.SetFloat(_intensity, value5);
					}
				}
				_indicatorMainColor = Color.Lerp(indicatorColorYellow, indicatorColorGray, intensity);
				if (base.gameObject == V.Car)
				{
					indicatorColor.a = _indicatorMainColor.a + 0.3f;
					if ((bool)_menuLevelParent && (bool)_menuLevelParent.LeftIndicatorArrowTC)
					{
						_menuLevelParent.RightIndicatorArrowTC.color = indicatorColor;
					}
				}
			}).AddTo(this);
			break;
		case TurnSignalState.Hazard:
			currentState = TurnSignalState.Hazard;
			_turnSignalSubscription = (from x in Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(timer))
				select (x % 2 != 0L) ? 0f : 1f).Subscribe((float lerpTime) =>
			{
				if (!_hasNewLightsController)
				{
					for (int i = 0; i < _leftHaloMat.Length; i++)
					{
						if ((bool)_leftHaloMat[i])
						{
							_indicatorHaloMainColor = Color.Lerp(indicatorTransparent, _leftHaloClr[i], lerpTime);
							_leftHaloMat[i].SetColor(TintId, _indicatorHaloMainColor);
						}
					}
					for (int j = 0; j < _rightHaloMat.Length; j++)
					{
						if ((bool)_rightHaloMat[j])
						{
							_indicatorHaloMainColor = Color.Lerp(indicatorTransparent, _rightHaloClr[j], lerpTime);
							_rightHaloMat[j].SetColor(TintId, _indicatorHaloMainColor);
						}
					}
					_indicatorMainColor = Color.Lerp(indicatorColorGray, indicatorColorYellow, lerpTime);
					if ((bool)LeftIndicatorMat2)
					{
						_indicatorMainColor = Color.Lerp(indicatorColorGray, _turnOnColorLights[0], lerpTime);
						LeftIndicatorMat2.color = _indicatorMainColor;
						float value = Mathf.Lerp(turnIntensity[0], turnIntensity[2], lerpTime);
						LeftIndicatorMat2.SetFloat(_intensity, value);
					}
					if ((bool)RightIndicatorMat2)
					{
						RightIndicatorMat2.color = _indicatorMainColor;
						float value2 = Mathf.Lerp(turnIntensity[0], turnIntensity[2], lerpTime);
						RightIndicatorMat2.SetFloat(_intensity, value2);
					}
					if ((bool)LeftIndicatorMat3)
					{
						_indicatorMainColor = Color.Lerp(indicatorColorGray, _turnOnColorLights[1], lerpTime);
						LeftIndicatorMat3.color = _indicatorMainColor;
						float value3 = Mathf.Lerp(turnIntensity[0], turnIntensity2[2], lerpTime);
						LeftIndicatorMat3.SetFloat(_intensity, value3);
					}
					if ((bool)RightIndicatorMat3)
					{
						RightIndicatorMat3.color = _indicatorMainColor;
						float value4 = Mathf.Lerp(turnIntensity[0], turnIntensity2[2], lerpTime);
						RightIndicatorMat3.SetFloat(_intensity, value4);
					}
					if ((bool)LeftIndicatorMat4)
					{
						_indicatorMainColor = Color.Lerp(indicatorColorGray, _turnOnColorLights[2], lerpTime);
						LeftIndicatorMat4.color = _indicatorMainColor;
						float value5 = Mathf.Lerp(turnIntensity[0], turnIntensity3[2], lerpTime);
						LeftIndicatorMat4.SetFloat(_intensity, value5);
					}
					if ((bool)RightIndicatorMat4)
					{
						RightIndicatorMat4.color = _indicatorMainColor;
						float value6 = Mathf.Lerp(turnIntensity[0], turnIntensity3[2], lerpTime);
						RightIndicatorMat4.SetFloat(_intensity, value6);
					}
					if ((bool)LeftIndicatorMat5)
					{
						_indicatorMainColor = Color.Lerp(indicatorColorGray, _turnOnColorLights[3], lerpTime);
						LeftIndicatorMat5.color = _indicatorMainColor;
						float value7 = Mathf.Lerp(turnIntensity[0], turnIntensity3[2], lerpTime);
						LeftIndicatorMat5.SetFloat(_intensity, value7);
						_turnLeftOn = true;
					}
					if ((bool)RightIndicatorMat5)
					{
						RightIndicatorMat5.color = _indicatorMainColor;
						float value8 = Mathf.Lerp(turnIntensity[0], turnIntensity3[2], lerpTime);
						RightIndicatorMat5.SetFloat(_intensity, value8);
						_turnRightOn = true;
					}
					if ((bool)IndicatorDiodMat)
					{
						IndicatorDiodMat.color = lerpedColorDiod;
						IndicatorDiodMat.SetFloat(_intensity, turnIntensity2[2]);
					}
					if ((bool)turnRightDiodMat)
					{
						turnRightDiodMat.color = _indicatorMainColor;
						float value9 = Mathf.Lerp(turnIntensity[0], turnIntensity2[2], lerpTime);
						turnRightDiodMat.SetFloat(_intensity, value9);
					}
					if ((bool)turnLeftDiodMat)
					{
						turnLeftDiodMat.color = _indicatorMainColor;
						float value10 = Mathf.Lerp(turnIntensity[0], turnIntensity2[2], lerpTime);
						turnLeftDiodMat.SetFloat(_intensity, value10);
					}
				}
				_indicatorMainColor = Color.Lerp(indicatorColorYellow, indicatorColorGray, lerpTime);
				if (!_isDestroyed && base.gameObject == V.Car)
				{
					indicatorColor.a = _indicatorMainColor.a + 0.3f;
					if ((bool)_menuLevelParent)
					{
						if ((bool)_menuLevelParent.RightIndicatorArrowTC)
						{
							_menuLevelParent.RightIndicatorArrowTC.color = indicatorColor;
						}
						if ((bool)_menuLevelParent.LeftIndicatorArrowTC)
						{
							_menuLevelParent.LeftIndicatorArrowTC.color = indicatorColor;
						}
						if ((bool)_menuLevelParent.EmergencyTC)
						{
							_menuLevelParent.EmergencyTC.color = indicatorColor;
						}
					}
				}
			}).AddTo(this);
			break;
		case TurnSignalState.Off:
			currentState = TurnSignalState.Off;
			break;
		default:
			throw new ArgumentOutOfRangeException("state", state, null);
		}
	}

	private void SetActiveMeshRenderer(bool active, MeshRenderer[] obj)
	{
		if (obj == null)
		{
			return;
		}
		foreach (MeshRenderer meshRenderer in obj)
		{
			if ((bool)meshRenderer)
			{
				meshRenderer.enabled = active;
			}
		}
	}

	public void ResetAllLights()
	{
		turnOffHeadLights();
		TurnOffAllIndicators();
		TurnOffBrakesLights().Subscribe().AddTo(this);
		TurnOffReverseLights();
	}

	public void ApplyLightStateToBodyKitMaterials()
	{
		if ((bool)lightsMat)
		{
			lightsMat.SetFloat(_intensity, BodyKitLightsIntensity(CurrentLightsIndex));
		}
	}

	private float BodyKitLightsIntensity(int index)
	{
		if (index <= 0)
		{
			return BodyKitLightsIntensityFloor[0];
		}
		float num = ((index < BodyKitLightsIntensityFloor.Length) ? BodyKitLightsIntensityFloor[index] : BodyKitLightsIntensityFloor[BodyKitLightsIntensityFloor.Length - 1]);
		if (index >= lightsIntensity.Length)
		{
			return num;
		}
		return Mathf.Max(lightsIntensity[index], num);
	}

	private void ResetIndicatorsMaterialsColor()
	{
		_indicatorHaloMainColor = indicatorTransparent;
		_indicatorMainColor = indicatorColorGray;
		lerpedColorDiod = indicatorColorDiod;
		if (_leftHaloMat != null)
		{
			Material[] leftHaloMat = _leftHaloMat;
			foreach (Material material in leftHaloMat)
			{
				if ((bool)material)
				{
					material.SetColor(TintId, indicatorTransparent);
				}
			}
		}
		if (_rightHaloMat != null)
		{
			Material[] leftHaloMat = _rightHaloMat;
			foreach (Material material2 in leftHaloMat)
			{
				if ((bool)material2)
				{
					material2.SetColor(TintId, indicatorTransparent);
				}
			}
		}
		if ((bool)LeftIndicatorMat2)
		{
			LeftIndicatorMat2.color = indicatorColorGray;
			LeftIndicatorMat2.SetFloat(_intensity, lightState ? turnIntensity[1] : turnIntensity[0]);
		}
		if ((bool)LeftIndicatorMat3)
		{
			LeftIndicatorMat3.color = indicatorColorGray;
			LeftIndicatorMat3.SetFloat(_intensity, lightState ? turnIntensity2[1] : turnIntensity2[0]);
		}
		if ((bool)LeftIndicatorMat4)
		{
			LeftIndicatorMat4.color = indicatorColorGray;
			LeftIndicatorMat4.SetFloat(_intensity, lightState ? turnIntensity3[1] : turnIntensity3[0]);
		}
		if ((bool)IndicatorDiodMat)
		{
			IndicatorDiodMat.color = indicatorColorDiod;
			IndicatorDiodMat.SetFloat(_intensity, 1f);
		}
		if ((bool)turnLeftDiodMat)
		{
			turnLeftDiodMat.color = indicatorColorDiod;
			turnLeftDiodMat.SetFloat(_intensity, 1f);
		}
		if ((bool)turnRightDiodMat)
		{
			turnRightDiodMat.color = indicatorColorDiod;
			turnRightDiodMat.SetFloat(_intensity, 1f);
		}
		if ((bool)RightIndicatorMat2)
		{
			RightIndicatorMat2.color = indicatorColorGray;
			RightIndicatorMat2.SetFloat(_intensity, lightState ? turnIntensity[1] : turnIntensity[0]);
		}
		if ((bool)RightIndicatorMat3)
		{
			RightIndicatorMat3.color = indicatorColorGray;
			RightIndicatorMat3.SetFloat(_intensity, lightState ? turnIntensity2[1] : turnIntensity2[0]);
		}
		if ((bool)RightIndicatorMat4)
		{
			RightIndicatorMat4.color = indicatorColorGray;
			RightIndicatorMat4.SetFloat(_intensity, lightState ? turnIntensity3[1] : turnIntensity3[0]);
		}
		if ((bool)LeftIndicatorMat5)
		{
			LeftIndicatorMat5.color = indicatorColorGray;
			LeftIndicatorMat5.SetFloat(_intensity, lightState ? turnIntensity3[1] : turnIntensity3[0]);
		}
		if ((bool)RightIndicatorMat5)
		{
			RightIndicatorMat5.color = indicatorColorGray;
			RightIndicatorMat5.SetFloat(_intensity, lightState ? turnIntensity3[1] : turnIntensity3[0]);
		}
		_turnLeftOn = false;
		_turnRightOn = false;
		if (base.gameObject == V.Car && (bool)_menuLevelParent)
		{
			indicatorColor.a = 1f;
			if ((bool)_menuLevelParent.LeftIndicatorArrowTC)
			{
				_menuLevelParent.LeftIndicatorArrowTC.color = indicatorColor;
			}
			if ((bool)_menuLevelParent.RightIndicatorArrowTC)
			{
				_menuLevelParent.RightIndicatorArrowTC.color = indicatorColor;
			}
			if ((bool)_menuLevelParent.EmergencyTC)
			{
				_menuLevelParent.EmergencyTC.color = indicatorColor;
			}
		}
	}

	public void ShortFlash()
	{
		turnOnFarLights();
		HState = 3;
		float num = UnityEngine.Random.Range(0.2f, 1f);
		_shortFlashDisposable?.Dispose();
		_shortFlashDisposable = Observable.Timer(TimeSpan.FromSeconds(num)).Subscribe((long _) =>
		{
			turnOffHeadLights();
			HState = 0;
		}).AddTo(this);
	}
}
