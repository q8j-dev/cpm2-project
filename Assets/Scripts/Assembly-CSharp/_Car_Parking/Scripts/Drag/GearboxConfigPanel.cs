using System.Collections.Generic;
using System.Linq;
using CPM.Database;
using CPMEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.Drag
{
	public class GearboxConfigPanel : MonoBehaviour, IBackableWindow
	{
		[SerializeField]
		private GameObject GearsPanel = default;

		[SerializeField]
		[Header("Buttons")]
		private Button SaveButton = default;

		[SerializeField]
		private Button DefaultButton = default;

		[SerializeField]
		private Button CancelButton = default;

		[SerializeField]
		[Header("Sliders")]
		private SliderValueHandler EndRatioSlider = default;

		[SerializeField]
		private SliderValueHandler TorqueSplitSlider = default;

		[SerializeField]
		private TextMeshProUGUI TorqueSplitTitle = default;

		[Header("Gears")]
		[SerializeField]
		private SliderWithCountPrefix SliderWithCountPrefixPrefab = default;

		[SerializeField]
		private RectTransform GearsParent = default;

		[SerializeField]
		[Header("Toggles")]
		private Toggle GearsToggle = default;

		[SerializeField]
		private Toggle LsdToggle = default;

		[SerializeField]
		private GameObject LineTitle = default;

		[Header("LSDPanel")]
		[SerializeField]
		private GameObject LsdPanel = default;

		[SerializeField]
		private GameObject LeftLSDPanel = default;

		[SerializeField]
		private GameObject RightLSDPanel = default;

		[SerializeField]
		private SliderValueHandler FrontAxselerationBiasSlider = default;

		[SerializeField]
		private SliderValueHandler FrontBrakeBiasSlider = default;

		[SerializeField]
		private SliderValueHandler FrontMaxBiasSlider = default;

		[SerializeField]
		private SliderValueHandler FrontBiasSlider = default;

		[SerializeField]
		private SliderValueHandler FrontDifferentialLockSlider = default;

		[SerializeField]
		private SliderValueHandler RearAxselerationBiasSlider = default;

		[SerializeField]
		private SliderValueHandler RearBrakeBiasSlider = default;

		[SerializeField]
		private SliderValueHandler RearMaxBiasSlider = default;

		[SerializeField]
		private SliderValueHandler RearBiasSlider = default;

		[SerializeField]
		private SliderValueHandler RearDifferentialLockSlider = default;

		private List<SliderWithCountPrefix> _cachedSliders = new List<SliderWithCountPrefix>();

		private const int FirstGearIndex = 2;

		private const float GearboxMinValue = 0.4f;

		private const float GearboxMaxValue = 6f;

		private const float EndRationMinValue = 1f;

		private const float EndRationMaxValue = 6f;

		private const float TorqueSplitMinValue = 10f;

		private const float TorqueSplitMaxValue = 90f;

		private const float MaxPercent = 100f;

		private VehicleEntityProvider _vehicleEntityProvider;

		private float[] _gearRatio;

		private float _finalDrive;

		private float _frontDiffLock;

		private float _rearDiffLock;

		private LSDType _frontLsdType;

		private LSDType _rearLsdType;

		private float _fAccelerationBias;

		private float _fBrakeBias;

		private float _fMaxBias;

		private float _rAccelerationBias;

		private float _rBrakeBias;

		private float _rMaxBias;

		private float _torqueSplit;

		private bool _awd;

		private IPrefabFactory _prefabFactory;

		private ICarTuningFactory _carTuningFactory;

		private ManagerUI _managerUI;

		private ICarInfoService _carInfoService;

		private CarInfo _carInfo;

		private bool _isGearPanelOpen;

		private GearBoxPart _defaultValues;

		private const string KeyLockOnPower = "Gearboxuiwindow.AccelerationBias";

		private const string KeyLockOnBraking = "Gearboxuiwindow.BrakeBIas";

		private const string KeyLockMax = "Gearboxuiwindow.MaxBias";

		private const string KeyLock = "Gearboxuiwindow.Bias";

		[Inject]
		public void Construct(IPrefabFactory prefabFactory, ManagerUI managerUI, ICarInfoService carInfoService, AllParts allParts, ICarTuningFactory carTuningFactory)
		{
			_prefabFactory = prefabFactory;
			_managerUI = managerUI;
			_carInfoService = carInfoService;
			_carTuningFactory = carTuningFactory;
		}

		public void InitBackButton()
		{
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			Cancel();
		}

		private void Start()
		{
			_vehicleEntityProvider = V.Car.GetComponent<VehicleEntityProvider>();
			_defaultValues = _vehicleEntityProvider.CarEntity.GetGearBox;
			Init();
			InitBackButton();
		}

		private void OnEnable()
		{
			_managerUI.OnBackButtonActivated(obj: false);
			Debug.Log("enabled this thing", this);
		}

		private void Init(bool reInit = false)
		{
			_carInfo = _carInfoService.GetFromSlot(V.SlotID);
			TransmissionValues getGearboxValues = _vehicleEntityProvider.CarEntity.GetGearboxValues;
			LSDValues value = _vehicleEntityProvider.CarEntity.FrontLsd.GetValue();
			LSDValues value2 = _vehicleEntityProvider.CarEntity.RearLsd.GetValue();
			_awd = getGearboxValues.Type == 4;
			_gearRatio = getGearboxValues.GearRatio;
			_finalDrive = getGearboxValues.FinalDrive;
			_torqueSplit = getGearboxValues.TorqueSplit;
			_fAccelerationBias = value.AccelBias;
			_fBrakeBias = value.BrakeBias;
			_fMaxBias = value.MaxBias;
			_frontDiffLock = value.LockPercent;
			_frontLsdType = value.Type;
			_rAccelerationBias = value2.AccelBias;
			_rBrakeBias = value2.BrakeBias;
			_rMaxBias = value2.MaxBias;
			_rearDiffLock = value2.LockPercent;
			_rearLsdType = value2.Type;
			if (!reInit)
			{
				float[] val = _gearRatio.Skip(2).ToArray();
				SetSliderValues();
				InitGears(_gearRatio, _cachedSliders.Count > 0);
				ReUseGears(val);
				SetLsdSliderValues();
				InitSliders();
				InitLSD();
			}
			else
			{
				float[] val2 = _gearRatio.Skip(2).ToArray();
				ReUseGears(val2);
				ReUseLSD();
				ReUseSliders();
			}
			OpenPanel(GearsToggle.isOn);
			InitButtons();
		}

		private void ReUseSliders()
		{
			EndRatioSlider.SetValue(_finalDrive);
			if (_awd)
			{
				TorqueSplitSlider.SetValue(_torqueSplit * 100f);
			}
		}

		private void ReUseGears(float[] val)
		{
			for (int i = 0; i < _cachedSliders.Count; i++)
			{
				_cachedSliders[i].SetValue(val[i]);
			}
		}

		private void ResetLSD()
		{
			_carInfo.TuningData.DynamicTuningSettings.FrontAccelBias = -1f;
			_vehicleEntityProvider.CarEntity.Engine.DynamicTuningSettings.FrontAccelBias = -1f;
			_carInfo.TuningData.DynamicTuningSettings.RearAccelBias = -1f;
			_vehicleEntityProvider.CarEntity.Engine.DynamicTuningSettings.RearAccelBias = -1f;
			_carInfo.TuningData.DynamicTuningSettings.FrontBrakeBias = -1f;
			_vehicleEntityProvider.CarEntity.Engine.DynamicTuningSettings.FrontBrakeBias = -1f;
			_carInfo.TuningData.DynamicTuningSettings.RearBrakeBias = -1f;
			_vehicleEntityProvider.CarEntity.Engine.DynamicTuningSettings.RearBrakeBias = -1f;
			_carInfo.TuningData.DynamicTuningSettings.FrontMaxBias = -1f;
			_vehicleEntityProvider.CarEntity.Engine.DynamicTuningSettings.FrontMaxBias = -1f;
			_carInfo.TuningData.DynamicTuningSettings.RearMaxBias = -1f;
			_vehicleEntityProvider.CarEntity.Engine.DynamicTuningSettings.RearMaxBias = -1f;
			_carInfo.TuningData.DynamicTuningSettings.FrontLsdLock = -1f;
			_vehicleEntityProvider.CarEntity.Engine.DynamicTuningSettings.FrontLsdLock = -1f;
			_carInfo.TuningData.DynamicTuningSettings.RearLsdLock = -1f;
			_vehicleEntityProvider.CarEntity.Engine.DynamicTuningSettings.RearLsdLock = -1f;
			_vehicleEntityProvider.CarEntity.FrontLsd.DynamicTuningSettings = new DynamicTuningSettings();
			_vehicleEntityProvider.CarEntity.RearLsd.DynamicTuningSettings = new DynamicTuningSettings();
		}

		private void OpenPanel(bool isGearsPanel)
		{
			_isGearPanelOpen = isGearsPanel;
			GearsPanel.SetActive(isGearsPanel);
			LsdPanel.SetActive(!isGearsPanel);
		}

		private void OnLsdTogglePressed(bool state)
		{
			if (state)
			{
				OpenPanel(isGearsPanel: false);
			}
		}

		private void OnGearsTogglePressed(bool state)
		{
			if (state)
			{
				OpenPanel(isGearsPanel: true);
			}
		}

		private void InitLSD()
		{
			DisableLsdPanelsAndSliders();
			TransmissionValues value = _vehicleEntityProvider.CarEntity.GearBox.GetValue();
			LSDValues frontLsd = _vehicleEntityProvider.VehicleEngineResult.FrontLsd;
			LSDValues rearLsd = _vehicleEntityProvider.VehicleEngineResult.RearLsd;
			switch ((GearboxType)value.Type)
			{
			case GearboxType.GearAwd:
			case GearboxType.Gear4Wd:
			case GearboxType.Gear4Wdr1:
			case GearboxType.Gear4Wdr2:
			case GearboxType.Gear6Wd:
			case GearboxType.Gear6Wdr2:
			case GearboxType.Gear8Wd:
				ParseLsdPanel(frontLsd, isFront: true);
				ParseLsdPanel(rearLsd, isFront: false);
				break;
			case GearboxType.GearFwd:
				ParseLsdPanel(frontLsd, isFront: true);
				break;
			case GearboxType.GearRwd:
				ParseLsdPanel(rearLsd, isFront: false);
				break;
			default:
				ParseLsdPanel(frontLsd, isFront: true);
				break;
			}
			bool flag = !LeftLSDPanel.activeSelf && !RightLSDPanel.activeSelf;
			LineTitle.SetActive(!flag);
			LsdToggle.gameObject.SetActive(!flag);
		}

		private void DisableLsdPanelsAndSliders()
		{
			LeftLSDPanel.SetActive(value: false);
			RightLSDPanel.SetActive(value: false);
			FrontBiasSlider.transform.parent.gameObject.SetActive(value: false);
			FrontBiasSlider.ShowHeaderText(state: false);
			FrontAxselerationBiasSlider.transform.parent.gameObject.SetActive(value: false);
			FrontAxselerationBiasSlider.ShowHeaderText(state: false);
			FrontBrakeBiasSlider.transform.parent.gameObject.SetActive(value: false);
			FrontBrakeBiasSlider.ShowHeaderText(state: false);
			FrontMaxBiasSlider.transform.parent.gameObject.SetActive(value: false);
			FrontMaxBiasSlider.ShowHeaderText(state: false);
			RearBiasSlider.transform.parent.gameObject.SetActive(value: false);
			RearBiasSlider.ShowHeaderText(state: false);
			RearAxselerationBiasSlider.transform.parent.gameObject.SetActive(value: false);
			RearAxselerationBiasSlider.ShowHeaderText(state: false);
			RearBrakeBiasSlider.transform.parent.gameObject.SetActive(value: false);
			RearBrakeBiasSlider.ShowHeaderText(state: false);
			RearMaxBiasSlider.transform.parent.gameObject.SetActive(value: false);
			RearMaxBiasSlider.ShowHeaderText(state: false);
			FrontDifferentialLockSlider.transform.parent.gameObject.SetActive(value: false);
			FrontDifferentialLockSlider.ShowHeaderText(state: false);
			RearDifferentialLockSlider.transform.parent.gameObject.SetActive(value: false);
			RearDifferentialLockSlider.ShowHeaderText(state: false);
		}

		private void ParseLsdPanel(LSDValues lsdValues, bool isFront)
		{
			LSDType type = lsdValues.Type;
			bool flag = type == LSDType.Open || type == LSDType.Locked;
			if (isFront)
			{
				LeftLSDPanel.SetActive(!flag);
			}
			else
			{
				RightLSDPanel.SetActive(!flag);
			}
			if (flag)
			{
				return;
			}
			switch (lsdValues.Type)
			{
			case LSDType.ActiveM:
				if (isFront)
				{
					FrontDifferentialLockSlider.transform.parent.gameObject.SetActive(value: true);
					FrontDifferentialLockSlider.ShowHeaderText(state: true);
				}
				else
				{
					RearDifferentialLockSlider.transform.parent.gameObject.SetActive(value: true);
					RearDifferentialLockSlider.ShowHeaderText(state: true);
				}
				break;
			case LSDType.Electronic:
				if (isFront)
				{
					FrontMaxBiasSlider.transform.parent.gameObject.SetActive(value: true);
					FrontMaxBiasSlider.ShowHeaderText(state: true);
					FrontDifferentialLockSlider.transform.parent.gameObject.SetActive(value: true);
					FrontDifferentialLockSlider.ShowHeaderText(state: true);
				}
				else
				{
					RearMaxBiasSlider.transform.parent.gameObject.SetActive(value: true);
					RearMaxBiasSlider.ShowHeaderText(state: true);
					RearDifferentialLockSlider.transform.parent.gameObject.SetActive(value: true);
					RearDifferentialLockSlider.ShowHeaderText(state: true);
				}
				break;
			case LSDType.Way1:
				if (isFront)
				{
					FrontAxselerationBiasSlider.transform.parent.gameObject.SetActive(value: true);
					FrontAxselerationBiasSlider.ShowHeaderText(state: true);
					FrontDifferentialLockSlider.transform.parent.gameObject.SetActive(value: true);
					FrontDifferentialLockSlider.ShowHeaderText(state: true);
				}
				else
				{
					RearAxselerationBiasSlider.transform.parent.gameObject.SetActive(value: true);
					RearAxselerationBiasSlider.ShowHeaderText(state: true);
					RearDifferentialLockSlider.transform.parent.gameObject.SetActive(value: true);
					RearDifferentialLockSlider.ShowHeaderText(state: true);
				}
				break;
			case LSDType.Way2:
				if (isFront)
				{
					FrontBiasSlider.transform.parent.gameObject.SetActive(value: true);
					FrontBiasSlider.ShowHeaderText(state: true);
					FrontDifferentialLockSlider.transform.parent.gameObject.SetActive(value: true);
					FrontDifferentialLockSlider.ShowHeaderText(state: true);
				}
				else
				{
					RearBiasSlider.transform.parent.gameObject.SetActive(value: true);
					RearBiasSlider.ShowHeaderText(state: true);
					RearDifferentialLockSlider.transform.parent.gameObject.SetActive(value: true);
					RearDifferentialLockSlider.ShowHeaderText(state: true);
				}
				break;
			case LSDType.Way15:
				if (isFront)
				{
					FrontAxselerationBiasSlider.transform.parent.gameObject.SetActive(value: true);
					FrontAxselerationBiasSlider.ShowHeaderText(state: true);
					FrontBrakeBiasSlider.transform.parent.gameObject.SetActive(value: true);
					FrontBrakeBiasSlider.ShowHeaderText(state: true);
					FrontDifferentialLockSlider.transform.parent.gameObject.SetActive(value: true);
					FrontDifferentialLockSlider.ShowHeaderText(state: true);
				}
				else
				{
					RearAxselerationBiasSlider.transform.parent.gameObject.SetActive(value: true);
					RearAxselerationBiasSlider.ShowHeaderText(state: true);
					RearBrakeBiasSlider.transform.parent.gameObject.SetActive(value: true);
					RearBrakeBiasSlider.ShowHeaderText(state: true);
					RearDifferentialLockSlider.transform.parent.gameObject.SetActive(value: true);
					RearDifferentialLockSlider.ShowHeaderText(state: true);
				}
				break;
			case LSDType.Torsen:
				if (isFront)
				{
					FrontDifferentialLockSlider.transform.parent.gameObject.SetActive(value: true);
					FrontDifferentialLockSlider.ShowHeaderText(state: true);
				}
				else
				{
					RearDifferentialLockSlider.transform.parent.gameObject.SetActive(value: true);
					RearDifferentialLockSlider.ShowHeaderText(state: true);
				}
				break;
			case LSDType.Open:
			case LSDType.Locked:
				break;
			}
		}

		private void InitButtons()
		{
			SaveButton.onClick.RemoveAllListeners();
			SaveButton.onClick.AddListener(Save);
			CancelButton.onClick.RemoveAllListeners();
			CancelButton.onClick.AddListener(Cancel);
			DefaultButton.onClick.RemoveAllListeners();
			DefaultButton.onClick.AddListener(SetDefault);
			GearsToggle.onValueChanged.RemoveAllListeners();
			GearsToggle.onValueChanged.AddListener(OnGearsTogglePressed);
			LsdToggle.onValueChanged.RemoveAllListeners();
			LsdToggle.onValueChanged.AddListener(OnLsdTogglePressed);
		}

		private void RebuildLayout()
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(GearsParent);
		}

		private void SetLsdSliderValues()
		{
			(float, float) accelBiasRange = LSDValues.GetAccelBiasRange(_frontLsdType);
			(float, float) accelBiasRange2 = LSDValues.GetAccelBiasRange(_rearLsdType);
			(float, float) brakeBiasRange = LSDValues.GetBrakeBiasRange(_frontLsdType);
			(float, float) brakeBiasRange2 = LSDValues.GetBrakeBiasRange(_rearLsdType);
			(float, float) maxBiasRange = LSDValues.GetMaxBiasRange(_frontLsdType);
			(float, float) maxBiasRange2 = LSDValues.GetMaxBiasRange(_rearLsdType);
			(float, float) lockPercentRange = LSDValues.GetLockPercentRange(_frontLsdType);
			(float, float) lockPercentRange2 = LSDValues.GetLockPercentRange(_rearLsdType);
			_fAccelerationBias = Mathf.Clamp(_fAccelerationBias, accelBiasRange.Item1, accelBiasRange.Item2);
			_rAccelerationBias = Mathf.Clamp(_rAccelerationBias, accelBiasRange2.Item1, accelBiasRange2.Item2);
			_fBrakeBias = Mathf.Clamp(_fBrakeBias, brakeBiasRange.Item1, brakeBiasRange.Item2);
			_rBrakeBias = Mathf.Clamp(_rBrakeBias, brakeBiasRange2.Item1, brakeBiasRange2.Item2);
			_fMaxBias = Mathf.Clamp(_fMaxBias, maxBiasRange.Item1, maxBiasRange.Item2);
			_rMaxBias = Mathf.Clamp(_rMaxBias, maxBiasRange2.Item1, maxBiasRange2.Item2);
			_frontDiffLock = Mathf.Clamp(_frontDiffLock, lockPercentRange.Item1, lockPercentRange.Item2);
			_rearDiffLock = Mathf.Clamp(_rearDiffLock, lockPercentRange2.Item1, lockPercentRange2.Item2);
			FrontBiasSlider.Init(BiasToPercent(accelBiasRange.Item1), BiasToPercent(accelBiasRange.Item2), 1f, isRoundValue: true, removeSpecialListener: true);
			FrontBiasSlider.SetValue(BiasToPercent(_fAccelerationBias));
			RearBiasSlider.Init(BiasToPercent(accelBiasRange2.Item1), BiasToPercent(accelBiasRange2.Item2), 1f, isRoundValue: true, removeSpecialListener: true);
			RearBiasSlider.SetValue(BiasToPercent(_rAccelerationBias));
			FrontAxselerationBiasSlider.Init(BiasToPercent(accelBiasRange.Item1), BiasToPercent(accelBiasRange.Item2), 1f, isRoundValue: true, removeSpecialListener: true);
			FrontAxselerationBiasSlider.SetValue(BiasToPercent(_fAccelerationBias));
			RearAxselerationBiasSlider.Init(BiasToPercent(accelBiasRange2.Item1), BiasToPercent(accelBiasRange2.Item2), 1f, isRoundValue: true, removeSpecialListener: true);
			RearAxselerationBiasSlider.SetValue(BiasToPercent(_rAccelerationBias));
			FrontBrakeBiasSlider.Init(BiasToPercent(brakeBiasRange.Item1), BiasToPercent(brakeBiasRange.Item2), 1f, isRoundValue: true, removeSpecialListener: true);
			FrontBrakeBiasSlider.SetValue(BiasToPercent(_fBrakeBias));
			RearBrakeBiasSlider.Init(BiasToPercent(brakeBiasRange2.Item1), BiasToPercent(brakeBiasRange2.Item2), 1f, isRoundValue: true, removeSpecialListener: true);
			RearBrakeBiasSlider.SetValue(BiasToPercent(_rBrakeBias));
			FrontMaxBiasSlider.Init(BiasToPercent(maxBiasRange.Item1), BiasToPercent(maxBiasRange.Item2), 1f, isRoundValue: true, removeSpecialListener: true);
			FrontMaxBiasSlider.SetValue(BiasToPercent(_fMaxBias));
			RearMaxBiasSlider.Init(BiasToPercent(maxBiasRange2.Item1), BiasToPercent(maxBiasRange2.Item2), 1f, isRoundValue: true, removeSpecialListener: true);
			RearMaxBiasSlider.SetValue(BiasToPercent(_rMaxBias));
			FrontDifferentialLockSlider.Init(lockPercentRange.Item1, lockPercentRange.Item2, 1f, isRoundValue: true);
			FrontDifferentialLockSlider.SetValue(_frontDiffLock);
			RearDifferentialLockSlider.Init(lockPercentRange2.Item1, lockPercentRange2.Item2, 1f, isRoundValue: true);
			RearDifferentialLockSlider.SetValue(_rearDiffLock);
			SetLsdSliderHeaders();
		}

		private static float BiasToPercent(float bias)
		{
			return LSDValues.LockPercentFromBias(bias);
		}

		private static float PercentToBias(float percent)
		{
			return LSDValues.BiasFromLockPercent(percent);
		}

		private static string Header(string key, string fallback)
		{
			string text = LocalizationManager.Localize(key);
			if (!(text == key))
			{
				return text;
			}
			return fallback;
		}

		private void SetLsdSliderHeaders()
		{
			string headerText = Header("Gearboxuiwindow.AccelerationBias", "Блокировка под тягой, %");
			string headerText2 = Header("Gearboxuiwindow.BrakeBIas", "Блокировка на торможении, %");
			string headerText3 = Header("Gearboxuiwindow.MaxBias", "Макс. блокировка, %");
			string headerText4 = Header("Gearboxuiwindow.Bias", "Блокировка, %");
			FrontAxselerationBiasSlider.SetHeaderText(headerText);
			RearAxselerationBiasSlider.SetHeaderText(headerText);
			FrontBrakeBiasSlider.SetHeaderText(headerText2);
			RearBrakeBiasSlider.SetHeaderText(headerText2);
			FrontMaxBiasSlider.SetHeaderText(headerText3);
			RearMaxBiasSlider.SetHeaderText(headerText3);
			FrontBiasSlider.SetHeaderText(headerText4);
			RearBiasSlider.SetHeaderText(headerText4);
		}

		private void ReUseLSD()
		{
			FrontBiasSlider.SetValue(BiasToPercent(_fAccelerationBias));
			RearBiasSlider.SetValue(BiasToPercent(_rAccelerationBias));
			FrontAxselerationBiasSlider.SetValue(BiasToPercent(_fAccelerationBias));
			RearAxselerationBiasSlider.SetValue(BiasToPercent(_rAccelerationBias));
			FrontBrakeBiasSlider.SetValue(BiasToPercent(_fBrakeBias));
			RearBrakeBiasSlider.SetValue(BiasToPercent(_rBrakeBias));
			FrontMaxBiasSlider.SetValue(BiasToPercent(_fMaxBias));
			RearMaxBiasSlider.SetValue(BiasToPercent(_rMaxBias));
			FrontDifferentialLockSlider.SetValue(_frontDiffLock);
			RearDifferentialLockSlider.SetValue(_rearDiffLock);
		}

		private void SetSliderValues()
		{
			float minValue = _defaultValues.GetParams.FinalDrive * 0.85f;
			float maxValue = _defaultValues.GetParams.FinalDrive * 1.15f;
			EndRatioSlider.Init(minValue, maxValue, 0.001f, isRoundValue: false, removeSpecialListener: true, 3);
			EndRatioSlider.SetValue(_finalDrive);
			if (_awd)
			{
				TorqueSplitSlider.Init(10f, 90f, 1f, isRoundValue: true);
				TorqueSplitSlider.SetValue(_torqueSplit * 100f);
			}
			TorqueSplitSlider.transform.parent.gameObject.SetActive(_awd);
			TorqueSplitTitle.SetActive(_awd);
		}

		private void InitSliders()
		{
			EndRatioSlider.AddListener(OnEndRationChange);
			TorqueSplitSlider.AddListener(OnTorqueSplitValueChange);
			FrontBiasSlider.AddListener(OnFrontBiasChange);
			RearBiasSlider.AddListener(OnRearBiasChange);
			FrontAxselerationBiasSlider.AddListener(OnFrontAxcelBiasChange);
			RearAxselerationBiasSlider.AddListener(OnRearAxcelBiasChange);
			FrontBrakeBiasSlider.AddListener(OnFrontBrakeBiasChange);
			RearBrakeBiasSlider.AddListener(OnRearBrakeBiasChange);
			FrontMaxBiasSlider.AddListener(OnFrontMaxBiasChange);
			RearMaxBiasSlider.AddListener(OnRearMaxBiasChange);
			FrontDifferentialLockSlider.AddListener(OnFrontDiffLockValueChange);
			RearDifferentialLockSlider.AddListener(OnRearDiffLockValueChange);
		}

		private void InitGears(float[] values, bool reuse = false)
		{
			foreach (SliderWithCountPrefix cachedSlider in _cachedSliders)
			{
				cachedSlider.gameObject.SetActive(value: false);
			}
			for (int i = 2; i < values.Length; i++)
			{
				SliderWithCountPrefix sliderWithCountPrefix = (reuse ? _cachedSliders[i - 2] : _prefabFactory.Instantiate(SliderWithCountPrefixPrefab, GearsParent));
				if (!reuse)
				{
					_cachedSliders.Add(sliderWithCountPrefix);
				}
				_cachedSliders[i - 2].gameObject.SetActive(value: true);
				float minVal = _defaultValues.GetParams.GearRatio[i] * 0.9f;
				float maxValue = _defaultValues.GetParams.GearRatio[i] * 1.1f;
				sliderWithCountPrefix.Init(i, (i - 2 + 1).ToString(), OnGearValueChange, minVal, maxValue, values[i], 0.001f);
			}
			RebuildLayout();
		}

		private void Save()
		{
			if (_isGearPanelOpen)
			{
				for (int i = 2; i < _gearRatio.Length; i++)
				{
					_carInfo.TuningData.DynamicTuningSettings.Gears[i - 2] = _gearRatio[i];
				}
				_carInfo.TuningData.DynamicTuningSettings.FinalDrive = _finalDrive;
				_vehicleEntityProvider.CarEntity.Engine.DynamicTuningSettings.TorqueSplit = _torqueSplit;
			}
			else
			{
				_carInfo.TuningData.DynamicTuningSettings.FrontAccelBias = _fAccelerationBias;
				_vehicleEntityProvider.CarEntity.Engine.DynamicTuningSettings.FrontAccelBias = _fAccelerationBias;
				_carInfo.TuningData.DynamicTuningSettings.RearAccelBias = _rAccelerationBias;
				_vehicleEntityProvider.CarEntity.Engine.DynamicTuningSettings.RearAccelBias = _rAccelerationBias;
				_carInfo.TuningData.DynamicTuningSettings.FrontBrakeBias = _fBrakeBias;
				_vehicleEntityProvider.CarEntity.Engine.DynamicTuningSettings.FrontBrakeBias = _fBrakeBias;
				_carInfo.TuningData.DynamicTuningSettings.RearBrakeBias = _rBrakeBias;
				_vehicleEntityProvider.CarEntity.Engine.DynamicTuningSettings.RearBrakeBias = _rBrakeBias;
				_carInfo.TuningData.DynamicTuningSettings.FrontMaxBias = _fMaxBias;
				_vehicleEntityProvider.CarEntity.Engine.DynamicTuningSettings.FrontMaxBias = _fMaxBias;
				_carInfo.TuningData.DynamicTuningSettings.RearMaxBias = _rMaxBias;
				_vehicleEntityProvider.CarEntity.Engine.DynamicTuningSettings.RearMaxBias = _rMaxBias;
				_carInfo.TuningData.DynamicTuningSettings.FrontLsdLock = _frontDiffLock;
				_vehicleEntityProvider.CarEntity.Engine.DynamicTuningSettings.FrontLsdLock = _frontDiffLock;
				_carInfo.TuningData.DynamicTuningSettings.RearLsdLock = _rearDiffLock;
				_vehicleEntityProvider.CarEntity.Engine.DynamicTuningSettings.RearLsdLock = _rearDiffLock;
			}
			_vehicleEntityProvider.CarEntity.RecalculateEngine(_vehicleEntityProvider, _carTuningFactory, _carInfo);
			_managerUI.ShowNotificationWindow(new LocalizedString("GearboxConfigPanel.GearboxConfigurationChanged!"), 3f).Forget();
			_managerUI.OnBackButtonActivated(obj: true);
			base.gameObject.SetActive(value: false);
		}

		private void Cancel()
		{
			Init(reInit: true);
			_managerUI.OnBackButtonActivated(obj: true);
			base.gameObject.SetActive(value: false);
		}

		private void SetDefault()
		{
			if (_isGearPanelOpen)
			{
				_vehicleEntityProvider.CarEntity.Engine.DynamicTuningSettings.Gears = Enumerable.Repeat(-1f, 14).ToArray();
				_vehicleEntityProvider.CarEntity.Engine.DynamicTuningSettings.FinalDrive = -1f;
				_vehicleEntityProvider.CarEntity.Engine.DynamicTuningSettings.TorqueSplit = -1f;
				OpenPanel(isGearsPanel: true);
				List<float> list = new List<float>();
				for (int i = 2; i < _defaultValues.GetParams.GearRatio.Length; i++)
				{
					list.Add(_defaultValues.GetParams.GearRatio[i]);
				}
				float[] val = list.ToArray();
				ReUseGears(val);
			}
			else
			{
				OpenPanel(isGearsPanel: false);
				ResetLSD();
			}
			Init(reInit: true);
			_vehicleEntityProvider.CarEntity.RecalculateEngine(_vehicleEntityProvider, _carTuningFactory, _carInfo);
			_managerUI.ShowNotificationWindow(new LocalizedString("GearboxConfigPanel.Defaultgearboxconfigurationinstalled!"), 3f).Forget();
			_managerUI.OnBackButtonActivated(obj: true);
			base.gameObject.SetActive(value: false);
		}

		private void OnGearValueChange(int gearIndex, float value)
		{
			_gearRatio[gearIndex] = value;
		}

		private void OnEndRationChange(float value)
		{
			_finalDrive = value;
		}

		private void OnFrontBiasChange(float value)
		{
			_fAccelerationBias = PercentToBias(value);
		}

		private void OnFrontMaxBiasChange(float value)
		{
			_fMaxBias = PercentToBias(value);
		}

		private void OnFrontBrakeBiasChange(float value)
		{
			_fBrakeBias = PercentToBias(value);
		}

		private void OnFrontAxcelBiasChange(float value)
		{
			_fAccelerationBias = PercentToBias(value);
		}

		private void OnRearBiasChange(float value)
		{
			_rAccelerationBias = PercentToBias(value);
		}

		private void OnRearMaxBiasChange(float value)
		{
			_rMaxBias = PercentToBias(value);
		}

		private void OnRearBrakeBiasChange(float value)
		{
			_rBrakeBias = PercentToBias(value);
		}

		private void OnRearAxcelBiasChange(float value)
		{
			_rAccelerationBias = PercentToBias(value);
		}

		private void OnFrontDiffLockValueChange(float value)
		{
			_frontDiffLock = value;
		}

		private void OnRearDiffLockValueChange(float value)
		{
			_rearDiffLock = value;
		}

		private void OnTorqueSplitValueChange(float value)
		{
			_torqueSplit = value / 100f;
		}
	}
}
