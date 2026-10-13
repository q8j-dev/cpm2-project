using System;
using System.Globalization;
using CPM.UI.BoostGauge;
using CPM.UI.TachoGauge;
using CPMEngine;
using TMPro;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.UIElement;

public class SpeedometerUI : MonoBehaviour
{
	public GameObject Dash;

	[Header("Car Status")]
	public Image CheckEngineIndicator;

	public Image AbsIcon;

	public Image TcsIcon;

	public Image EspIcon;

	[Header("Turbo")]
	public RectTransform turboMeter;

	private bool _showTurboGauge = true;

	[Header("Boost Gauge")]
	public Transform BoostTicksRoot;

	public GameObject BoostDigitPrefab;

	public GameObject BoostTickPrefab;

	public Image BoostFillPositive;

	public Image BoostFillNegative;

	public float BoostDigitsRadius = 123f;

	public float BoostLineRadius = 123f;

	public float BoostEndLineRadius = 123f;

	public float BoostLineWidth = 2f;

	public float BoostShortLineLength = 10f;

	public float BoostLongLineLength = 18f;

	public int BoostScaleMin = 2;

	public float BoostVacuumMax = 0.8f;

	private float _lerpedBoost;

	[Header("Fuel")]
	public TextMeshProUGUI FuelText;

	public float MinFuelAngle = 95f;

	public float MaxFuelAngle = -98f;

	private FuelTankEntity _fuelTankEntity;

	public RectTransform FuelArrowNeedle;

	private float _lerpedDashFuel;

	private float _actualFuelMeterNeedleAngle;

	[Header("Speed")]
	public TextMeshProUGUI speedoLabel;

	public TextMeshProUGUI speedo;

	public TextMeshProUGUI MileageLabel;

	public TextMeshProUGUI gear;

	[Header("Tacho")]
	public GameObject Tachometer;

	public RectTransform TachoMeterNeedle;

	public float TachoStartAngle = 155f;

	public float TachoEndAngle = -97f;

	private float _valueRpm;

	private float _coloredValue;

	private float _lerpedRpm;

	public GameObject DigitPrefab;

	public GameObject LinePrefab;

	public Transform TicksParent;

	public int NumberOfRpm = 8;

	public float DigitsRadius = 123f;

	public float LineRadius = 123f;

	public float LineWidth = 2f;

	public float LineLength = 17f;

	public float StartAngle = 244f;

	public float EndAngle = 136f;

	[Header("Tacho Ring (7→3 arc)")]
	public Image TachoRingRpmFill;

	public Image TachoRingRedZone;

	[Range(0f, 1f)]
	public float TachoArcSpan = 2f / 3f;

	private float _lastDistanceTraveled;

	private float _tachoAngle;

	private float _fillAmountTacho;

	private float maxRpmNeedle = 1f;

	private float redlineRPMNeedle;

	public TextMeshProUGUI WaterTempText;

	public SpeedometerTireView TireView;

	private Axles _cachedAxles;

	private Powertrain _cachedPowertrain;

	private Powertrain _boundPowertrain;

	private MultiPlayerPosControl _mppc;

	private VehicleData _cachedVehicleData;

	private GameObject _person;

	private GameObject _car;

	private bool _isInited;

	private float _minRpmColor;

	private float _speedInt;

	private bool _isTestMode;

	private float _lastRpm;

	public float accelerationRpm = 0.1f;

	public Color shadowColor;

	public Image shadowUp;

	public Image shadowDown;

	public int frameUpdate = 5;

	private int _frameCount;

	private float _acc;

	public float alphaKoff = 1f;

	public float speedCoolDown = 5f;

	private IDisposable _speedSub;

	private InputItemsProvider _inputItemsProvider;

	private const string GeneratedTickName = "GenTacho";

	private FuelTankEntity FuelTankEntity
	{
		get
		{
			if ((bool)_fuelTankEntity)
			{
				return _fuelTankEntity;
			}
			if ((bool)GetVehicleData && (bool)GetVehicleData.CarChildsReferences.FuelTank)
			{
				_fuelTankEntity = GetVehicleData.CarChildsReferences.FuelTank;
			}
			return _fuelTankEntity;
		}
	}

	private Powertrain GetPowertrain
	{
		get
		{
			if ((bool)_cachedPowertrain)
			{
				return _cachedPowertrain;
			}
			if ((bool)V.Car && V.Car.TryGetComponent<Powertrain>(out var component))
			{
				_cachedPowertrain = component;
			}
			return _cachedPowertrain;
		}
	}

	private Axles GetAxles
	{
		get
		{
			if ((bool)_cachedAxles)
			{
				return _cachedAxles;
			}
			if ((bool)V.Car && V.Car.TryGetComponent<Axles>(out var component))
			{
				_cachedAxles = component;
			}
			return _cachedAxles;
		}
	}

	private VehicleData GetVehicleData
	{
		get
		{
			if ((bool)_cachedVehicleData)
			{
				return _cachedVehicleData;
			}
			if ((bool)V.Car && V.Car.TryGetComponent<VehicleData>(out var component))
			{
				_cachedVehicleData = component;
			}
			return _cachedVehicleData;
		}
	}

	[Inject]
	public void Construct(InputItemsProvider inputItemsProvider)
	{
		_inputItemsProvider = inputItemsProvider;
		_inputItemsProvider.SpeedometerUI = this;
	}

	private void Awake()
	{
		ResetAssistIcons();
		if ((bool)V.Car)
		{
			V.Car.TryGetComponent<MultiPlayerPosControl>(out _mppc);
		}
		FlagsStorage.Kph = PlayerPrefs.GetInt("kph", 1) == 1;
		SetKph();
		shadowDown = null;
		shadowUp = null;
		if (shadowDown != null)
		{
			shadowDown.gameObject.SetActive(value: false);
		}
		if (shadowUp != null)
		{
			shadowUp.gameObject.SetActive(value: false);
		}
	}

	private void OnDestroy()
	{
		if (_inputItemsProvider != null && _inputItemsProvider.SpeedometerUI == this)
		{
			_inputItemsProvider.SpeedometerUI = null;
		}
		Unbind();
	}

	public void ReInit()
	{
		FlagsStorage.Kph = PlayerPrefs.GetInt("kph", 1) == 1;
		SetKph();
	}

	public void SetKph()
	{
		if ((bool)speedoLabel)
		{
			speedoLabel.text = Utils.CurrentSpeedMetric.ToUpper();
		}
	}

	public void SetNewCar(GameObject newCar)
	{
		_car = newCar;
		if ((bool)_car)
		{
			_car.TryGetComponent<Powertrain>(out _cachedPowertrain);
			_car.TryGetComponent<Axles>(out _cachedAxles);
			_cachedVehicleData = null;
			_fuelTankEntity = null;
			Init();
		}
	}

	private void OnEnable()
	{
		V.OnCarEntered += CheckButtons;
		SubscribeOnEnable();
		CheckEngine();
		if ((bool)TireView)
		{
			TireView.gameObject.SetActive(value: true);
		}
	}

	private void CheckEngine()
	{
		if ((bool)GetPowertrain && (bool)CheckEngineIndicator)
		{
			CheckEngineIndicator.SetActive(GetPowertrain.CheckEngineStatus);
		}
	}

	private void OnDisable()
	{
		V.OnCarEntered -= CheckButtons;
		_speedInt = 0f;
		Unbind();
		_isInited = false;
		ResetAssistIcons();
	}

	private void ResetAssistIcons()
	{
		if ((bool)AbsIcon)
		{
			AbsIcon.SetActive(active: false);
		}
		if ((bool)EspIcon)
		{
			EspIcon.SetActive(active: false);
		}
		if ((bool)TcsIcon)
		{
			TcsIcon.SetActive(active: false);
		}
	}

	private void SubscribeOnEnable()
	{
		Bind(GetPowertrain);
	}

	private void Bind(Powertrain powertrain)
	{
		if ((bool)powertrain && !(_boundPowertrain == powertrain))
		{
			Unbind();
			_speedInt = 0f;
			powertrain.OnCheckEngineChanged = (Action)Delegate.Combine(powertrain.OnCheckEngineChanged, new Action(CheckEngine));
			powertrain.OnGearChanged += SetGearText;
			powertrain.OnTrcStateChanged += UpdateTrcIcon;
			powertrain.OnEspStateChanged += UpdateEspIcon;
			powertrain.OnAbsStateChanged += UpdateAbsIcon;
			if ((bool)speedo)
			{
				_speedSub = powertrain.SpeedKmhReactive.Subscribe(UpdateSpeed).AddTo(this);
			}
			_boundPowertrain = powertrain;
		}
	}

	private void Unbind()
	{
		if ((bool)_boundPowertrain)
		{
			Powertrain boundPowertrain = _boundPowertrain;
			boundPowertrain.OnCheckEngineChanged = (Action)Delegate.Remove(boundPowertrain.OnCheckEngineChanged, new Action(CheckEngine));
			_boundPowertrain.OnGearChanged -= SetGearText;
			_boundPowertrain.OnTrcStateChanged -= UpdateTrcIcon;
			_boundPowertrain.OnEspStateChanged -= UpdateEspIcon;
			_boundPowertrain.OnAbsStateChanged -= UpdateAbsIcon;
		}
		_speedSub?.Dispose();
		_speedSub = null;
		_boundPowertrain = null;
	}

	private void Init()
	{
		if ((bool)GetPowertrain && (bool)GetAxles && (bool)GetVehicleData)
		{
			CheckBoostGauge();
			float maxRpm = ((GetPowertrain.MaxRpm > GetPowertrain.RedlineRpm) ? GetPowertrain.MaxRpm : GetPowertrain.RedlineRpm);
			NumberOfRpm = TachometerGaugeMath.MajorStepCount(maxRpm);
			maxRpmNeedle = NumberOfRpm * 1000;
			_minRpmColor = GetPowertrain.EngineIdleRpm;
			redlineRPMNeedle = GetPowertrain.RedlineRpm;
			if ((bool)TachoRingRedZone)
			{
				float num = ((maxRpmNeedle > 0f) ? Mathf.Clamp01(redlineRPMNeedle / maxRpmNeedle) : 1f);
				TachoRingRedZone.fillAmount = (1f - num) * TachoArcSpan;
			}
			GenerateRpmDigits();
			GenerateBoostDigits();
			SetMileageLabel(FlagsStorage.Kph);
			Bind(GetPowertrain);
			_isInited = true;
			_lerpedRpm = _valueRpm;
			SetGearText();
			CheckEngine();
		}
	}

	[ContextMenu("Generate")]
	private void GenerateRpmDigits()
	{
		if (!Tachometer || !DigitPrefab || !LinePrefab)
		{
			return;
		}
		Transform transform = (TicksParent ? TicksParent : Tachometer.transform);
		Color value = (TachoRingRedZone ? TachoRingRedZone.color : Color.red);
		float num = (((bool)TachoRingRedZone && TachoArcSpan > 0f) ? (1f - Mathf.Clamp01(TachoRingRedZone.fillAmount / TachoArcSpan)) : 2f);
		for (int num2 = transform.childCount - 1; num2 >= 0; num2--)
		{
			GameObject gameObject = transform.GetChild(num2).gameObject;
			if (!(gameObject.name != "GenTacho") || (bool)gameObject.GetComponent<TextMeshProUGUI>())
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
		int num3 = TachometerGaugeMath.QuarterIndexStep((float)NumberOfRpm * 1000f);
		int num4 = TachometerGaugeMath.DigitStepThousands((float)NumberOfRpm * 1000f);
		for (int i = 0; i <= NumberOfRpm * 4; i += num3)
		{
			if (TachometerGaugeMath.IsMajor(i) && i / 4 % num4 == 0)
			{
				float angleRad = TachometerGaugeMath.TickAngleRad((float)i * 0.25f, StartAngle, EndAngle, NumberOfRpm);
				float num5 = TachometerGaugeMath.TickWidth(i, LineWidth);
				if (i == 0 || i == NumberOfRpm * 4)
				{
					num5 *= 2f;
				}
				float radius = TachometerGaugeMath.TickCenterRadius(i, LineRadius, LineLength, DigitsRadius);
				float length = TachometerGaugeMath.TickLength(i, LineRadius, LineLength, DigitsRadius);
				float num6 = ((NumberOfRpm > 0) ? ((float)i / ((float)NumberOfRpm * 4f)) : 0f);
				CreateTachoLine(transform, angleRad, num5, radius, length, (num6 >= num) ? new Color?(value) : ((Color?)null));
				if (TachometerGaugeMath.IsMajor(i))
				{
					CreateTachoDigit(transform, angleRad, (i / 4).ToString());
				}
			}
		}
	}

	private void CreateTachoLine(Transform parent, float angleRad, float width, float radius, float length, Color? color)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(LinePrefab, parent);
		gameObject.name = "GenTacho";
		RectTransform component = gameObject.GetComponent<RectTransform>();
		Vector2 anchorMin = (component.anchorMax = new Vector2(0.5f, 0.5f));
		component.anchorMin = anchorMin;
		component.pivot = new Vector2(0.5f, 0.5f);
		component.anchoredPosition = new Vector2(Mathf.Cos(angleRad) * radius, Mathf.Sin(angleRad) * radius);
		component.sizeDelta = new Vector2(width, length);
		component.localRotation = Quaternion.Euler(0f, 0f, angleRad * 57.29578f + 90f);
		if (color.HasValue && gameObject.TryGetComponent<Image>(out var component2))
		{
			component2.color = color.Value;
		}
	}

	private void CreateTachoDigit(Transform parent, float angleRad, string label)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(DigitPrefab, parent);
		gameObject.name = "GenTacho";
		RectTransform component = gameObject.GetComponent<RectTransform>();
		float num = TachometerGaugeMath.DigitCenterRadius(DigitsRadius, LineRadius, component.sizeDelta.y);
		component.anchoredPosition = new Vector2(Mathf.Cos(angleRad) * num, Mathf.Sin(angleRad) * num);
		TextMeshProUGUI textMeshProUGUI = gameObject.GetComponent<TextMeshProUGUI>();
		if (!textMeshProUGUI)
		{
			textMeshProUGUI = gameObject.GetComponentInChildren<TextMeshProUGUI>();
		}
		if ((bool)textMeshProUGUI)
		{
			textMeshProUGUI.text = label;
		}
	}

	[ContextMenu("Generate Boost")]
	private void GenerateBoostDigits()
	{
		if (!BoostTicksRoot || !BoostDigitPrefab || !BoostTickPrefab)
		{
			return;
		}
		float maxTurboPressure = 0f;
		if ((bool)GetPowertrain)
		{
			GetPowertrain.GetEngineTurboParams(out maxTurboPressure, out var _, out var _, out var _);
		}
		bool flag = false;
		if (maxTurboPressure <= 0f && !flag)
		{
			return;
		}
		for (int num = BoostTicksRoot.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(BoostTicksRoot.GetChild(num).gameObject);
		}
		int num2 = BoostGaugeMath.ScaleMax(maxTurboPressure, BoostScaleMin);
		float length = BoostLongLineLength + (BoostEndLineRadius - BoostLineRadius);
		float angleDeg = BoostGaugeMath.NegativeArcAngleDeg(-1f);
		CreateBoostLine(angleDeg, length, BoostEndLineRadius);
		CreateBoostDigit(angleDeg, "-1");
		for (int i = 0; i <= num2; i++)
		{
			float angleDeg2 = BoostGaugeMath.PositiveArcAngleDeg(i, num2);
			if (i == num2)
			{
				CreateBoostLine(angleDeg2, length, BoostEndLineRadius);
			}
			else
			{
				CreateBoostLine(angleDeg2, BoostLongLineLength, BoostLineRadius);
			}
			CreateBoostDigit(angleDeg2, i.ToString());
		}
	}

	private void CreateBoostLine(float angleDeg, float length, float radius)
	{
		float f = angleDeg * ((float)Math.PI / 180f);
		RectTransform component = UnityEngine.Object.Instantiate(BoostTickPrefab, BoostTicksRoot).GetComponent<RectTransform>();
		component.anchoredPosition = new Vector2(Mathf.Cos(f) * radius, Mathf.Sin(f) * radius);
		component.sizeDelta = new Vector2(BoostLineWidth, length);
		component.localRotation = Quaternion.Euler(0f, 0f, angleDeg + 90f);
	}

	private void CreateBoostDigit(float angleDeg, string label)
	{
		float f = angleDeg * ((float)Math.PI / 180f);
		GameObject gameObject = UnityEngine.Object.Instantiate(BoostDigitPrefab, BoostTicksRoot);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		Vector2 anchorMin = (component.anchorMax = new Vector2(0.5f, 0.5f));
		component.anchorMin = anchorMin;
		component.pivot = new Vector2(0.5f, 0.5f);
		TextMeshProUGUI textMeshProUGUI = gameObject.GetComponent<TextMeshProUGUI>();
		if (!textMeshProUGUI)
		{
			textMeshProUGUI = gameObject.GetComponentInChildren<TextMeshProUGUI>();
		}
		float x = 0f;
		if ((bool)textMeshProUGUI)
		{
			textMeshProUGUI.text = label;
			textMeshProUGUI.horizontalAlignment = HorizontalAlignmentOptions.Geometry;
			textMeshProUGUI.verticalAlignment = VerticalAlignmentOptions.Geometry;
			textMeshProUGUI.ForceMeshUpdate(ignoreActiveState: true, forceTextReparsing: true);
			TMP_TextInfo textInfo = textMeshProUGUI.textInfo;
			float num = float.MaxValue;
			float num2 = float.MinValue;
			int num3 = 0;
			while (textInfo != null && num3 < textInfo.characterCount)
			{
				TMP_CharacterInfo tMP_CharacterInfo = textInfo.characterInfo[num3];
				if (tMP_CharacterInfo.isVisible && tMP_CharacterInfo.character != '-')
				{
					num = Mathf.Min(num, tMP_CharacterInfo.bottomLeft.x);
					num2 = Mathf.Max(num2, tMP_CharacterInfo.topRight.x);
				}
				num3++;
			}
			if (num <= num2)
			{
				x = (num + num2) * 0.5f;
			}
		}
		component.anchoredPosition = new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * BoostDigitsRadius - new Vector2(x, 0f);
	}

	private void SetGearText()
	{
		if ((bool)gear)
		{
			gear.text = GetPowertrain.GetSpeedometerGearName;
		}
	}

	private void CheckButtons(GameObject person, VehicleData vehicleData)
	{
		if ((bool)V.Car && V.Car != _car)
		{
			SetNewCar(V.Car);
		}
	}

	private void LateUpdate()
	{
		if (!_isInited)
		{
			Init();
		}
	}

	private void ShowEngineTemp()
	{
		if ((bool)WaterTempText)
		{
			WaterTempText.text = "Water: " + Utils.RoundTo(GetPowertrain.WaterTemp, 0).ToString(CultureInfo.InvariantCulture) + "°C";
		}
	}

	private void ShowFuel()
	{
		if ((bool)FuelTankEntity && (bool)FuelArrowNeedle && !(FuelTankEntity.TankCapacity <= 0f))
		{
			FuelTankEntity fuelTankEntity = FuelTankEntity;
			float num = 5f;
			float t = Mathf.Clamp01(Time.deltaTime * num);
			_lerpedDashFuel = Utils.GetFixedVariable(Mathf.Lerp(_lerpedDashFuel, fuelTankEntity.CurrentFuel, t));
			if (fuelTankEntity.TankCapacity > 0f)
			{
				_actualFuelMeterNeedleAngle = math.remap(0f, fuelTankEntity.TankCapacity, MinFuelAngle, MaxFuelAngle, Mathf.Clamp(_lerpedDashFuel, 0f, fuelTankEntity.TankCapacity));
			}
			FuelArrowNeedle.rotation = ((!float.IsNaN(_actualFuelMeterNeedleAngle) && !float.IsInfinity(_actualFuelMeterNeedleAngle)) ? Quaternion.Euler(0f, 0f, _actualFuelMeterNeedleAngle) : Quaternion.Euler(0f, 0f, MinFuelAngle));
		}
	}

	private void CheckBoostGauge()
	{
		GetPowertrain.GetEngineTurboParams(out var _, out var _, out var _, out var turboType);
		_showTurboGauge = turboType != TurboType.Supercharger && turboType != TurboType.NA;
		if ((bool)turboMeter)
		{
			turboMeter.gameObject.SetActive(_showTurboGauge);
		}
	}

	private void ShowBoost()
	{
		if (!GetPowertrain || !turboMeter || !_showTurboGauge || (!BoostFillPositive && !BoostFillNegative))
		{
			return;
		}
		GetPowertrain.GetEngineTurboParams(out var maxTurboPressure, out var curTurboPressure, out var _, out var _);
		if (!(maxTurboPressure <= 0f))
		{
			float b;
			if (GetPowertrain.IsEngineWorks)
			{
				float num = Mathf.Clamp01(GetPowertrain.Throttle);
				float num2 = (1f - num) * BoostVacuumMax;
				b = curTurboPressure - num2;
			}
			else
			{
				b = 0f;
			}
			_lerpedBoost = Utils.GetFixedVariable(Mathf.Lerp(_lerpedBoost, b, Time.deltaTime * 10f));
			if ((bool)BoostFillPositive)
			{
				BoostFillPositive.fillAmount = BoostGaugeMath.PositiveFillAmount(_lerpedBoost, BoostGaugeMath.ScaleMax(maxTurboPressure, BoostScaleMin));
			}
			if ((bool)BoostFillNegative)
			{
				BoostFillNegative.fillAmount = BoostGaugeMath.NegativeFillAmount(_lerpedBoost);
			}
		}
	}

	public void ShowDash()
	{
		Dash.SetActive(value: true);
	}

	public void SetTestMode(bool state)
	{
		_isTestMode = state;
	}

	private void UpdateSpeed(float newSpeed)
	{
		if ((!FlagsStorage.OutOfCar && !FlagsStorage.IsPassenger) || _isTestMode)
		{
			float num = (FlagsStorage.Kph ? newSpeed : (newSpeed / 1.6f));
			float num2 = 0.3f;
			float maxDelta = Mathf.Abs(num - _speedInt) * (Time.deltaTime / num2);
			_speedInt = Mathf.MoveTowards(_speedInt, num, maxDelta);
			string text = $"{Mathf.FloorToInt(_speedInt)}";
			speedo.text = text;
			if (Mathf.Abs(GetPowertrain.GetMileage - _lastDistanceTraveled) > 0.1f && (bool)MileageLabel)
			{
				SetMileageLabel(FlagsStorage.Kph);
				_lastDistanceTraveled = GetPowertrain.GetMileage;
			}
		}
	}

	public void SetMileageLabel(bool isKpm)
	{
		if ((bool)GetPowertrain && (bool)MileageLabel)
		{
			float km = (isKpm ? GetPowertrain.GetMileage : Utils.Km2Mile(GetPowertrain.GetMileage));
			MileageLabel.text = Utils.FormatOdometer(km);
		}
	}

	private void UpdateAbsIcon(bool state)
	{
		if ((bool)AbsIcon)
		{
			AbsIcon.SetActive(state);
		}
	}

	private void UpdateTrcIcon(bool state)
	{
		if ((bool)TcsIcon)
		{
			TcsIcon.SetActive(state);
		}
	}

	private void UpdateEspIcon(bool state)
	{
		if ((bool)EspIcon)
		{
			EspIcon.SetActive(state);
		}
	}

	private void UpdateRpmGauge()
	{
		_valueRpm = ((FlagsStorage.IsMultiFree && FlagsStorage.IsPassenger) ? _mppc.LerpedRPM : GetPowertrain.CurrentRpm);
		if (!GetPowertrain.IsEngineWorks && GetPowertrain.Starter <= 0f)
		{
			_valueRpm = 0f;
		}
		_frameCount++;
		if (_frameCount > frameUpdate && (bool)shadowUp && (bool)shadowDown)
		{
			_acc = (_valueRpm * 0.01f - _lastRpm * 0.01f) / Time.deltaTime * accelerationRpm;
			shadowUp.gameObject.SetActive(value: true);
			shadowDown.gameObject.SetActive(value: true);
			if (_acc > 0f)
			{
				shadowUp.gameObject.SetActive(value: true);
				shadowColor.a = Mathf.Abs(_acc) * alphaKoff;
				shadowUp.color = shadowColor;
				Color color = shadowDown.color;
				if (color.a > 0f)
				{
					color.a = Mathf.Lerp(color.a, 0f, Time.deltaTime * speedCoolDown);
					shadowDown.color = color;
				}
			}
			else
			{
				shadowDown.gameObject.SetActive(value: true);
				shadowColor.a = Mathf.Abs(_acc) * alphaKoff;
				shadowDown.color = shadowColor;
				Color color2 = shadowUp.color;
				if (color2.a > 0f)
				{
					color2.a = Mathf.Lerp(color2.a, 0f, Time.deltaTime * speedCoolDown);
					shadowUp.color = color2;
				}
			}
			_frameCount = 0;
			_lastRpm = _valueRpm;
		}
		_lerpedRpm = Utils.GetFixedVariable(Mathf.Lerp(_lerpedRpm, _valueRpm, Time.deltaTime * 5f));
		_tachoAngle = math.remap(0f, maxRpmNeedle, TachoStartAngle, TachoEndAngle, _lerpedRpm);
		_coloredValue = math.remap(_minRpmColor, redlineRPMNeedle, 0f, 0.5f, _valueRpm);
		TachoMeterNeedle.localEulerAngles = Vector3.forward * _tachoAngle;
		if ((bool)TachoRingRpmFill)
		{
			float num = ((maxRpmNeedle > 0f) ? Mathf.Clamp01(_lerpedRpm / maxRpmNeedle) : 0f);
			TachoRingRpmFill.fillAmount = num * TachoArcSpan;
			TachoRingRpmFill.color = ((_coloredValue > 0.465f) ? new Color(0.96f, 0.051f, 0.055f, 1f) : new Color(1f, 1f, 1f, _coloredValue));
		}
	}

	private void Update()
	{
		if ((bool)GetPowertrain)
		{
			ShowBoost();
			ShowFuel();
			UpdateRpmGauge();
		}
	}

	public void UpdateCar()
	{
		SubscribeOnEnable();
	}
}
