using CPMEngine;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.NewUI;

public class ShockAbsorberCarControlUIPart : PhoneUISubPanel
{
	private const float VibrateIntensity = 0.2f;

	private const float VibrationDuration = 0.05f;

	[Header("ComfortTexts")]
	[SerializeField]
	private TextMeshProUGUI ComfortFrontBumpValueText;

	[SerializeField]
	private TextMeshProUGUI ComfortFrontReboundValueText;

	[SerializeField]
	private TextMeshProUGUI ComfortRearBumpValueText;

	[SerializeField]
	private TextMeshProUGUI ComfortRearReboundValueText;

	[SerializeField]
	[Header("SportTexts")]
	private TextMeshProUGUI SportFrontBumpValueText;

	[SerializeField]
	private TextMeshProUGUI SportFrontReboundValueText;

	[SerializeField]
	private TextMeshProUGUI SportRearBumpValueText;

	[SerializeField]
	private TextMeshProUGUI SportRearReboundValueText;

	[SerializeField]
	[Header("CustomTexts")]
	private TextMeshProUGUI CustomFrontBumpValueText;

	[SerializeField]
	private TextMeshProUGUI CustomFrontReboundValueText;

	[SerializeField]
	private TextMeshProUGUI CustomRearBumpValueText;

	[SerializeField]
	private TextMeshProUGUI CustomRearReboundValueText;

	[Header("PresetsToggles")]
	[SerializeField]
	private Toggle ComfortToggle;

	[SerializeField]
	private Toggle SportToggle;

	[SerializeField]
	private Toggle CustomToggle;

	private SuspensionState _currentSuspensionState;

	private CarControlUIPanel _carControlUIPanel;

	private VehicleStockTuningData _vehicleStockTuningData;

	private float4 _cachedBumpReboundValues;

	private float4 _comfortBumpReboundValues;

	private float4 _sportBumpReboundValues;

	public void PublishDatas(CarControlUIPanel carControlUIPanel)
	{
		_carControlUIPanel = carControlUIPanel;
		_vehicleStockTuningData = CarTuningFactory.GetDefaultCarTuningDataConfig(_carControlUIPanel.GetVehicleData.CarId);
	}

	private void Start()
	{
		SetListeners();
	}

	public override void Show()
	{
		base.Show();
		SetCarShockAbsorbersPresets();
		SetTexts();
	}

	private void SetListeners()
	{
		CustomToggle.onValueChanged.AddListener(SetCustom);
		ComfortToggle.onValueChanged.AddListener(SetComfort);
		SportToggle.onValueChanged.AddListener(SetSport);
	}

	private void SetTexts()
	{
		_ = _carControlUIPanel.GetVehicleEntityProvider.CarEntity.FrontWheel.SuspensionData;
		_ = _carControlUIPanel.GetVehicleEntityProvider.CarEntity.RearWheel.SuspensionData;
		SetBumpReboundTexts(_cachedBumpReboundValues, ComfortFrontBumpValueText, ComfortFrontReboundValueText, ComfortRearBumpValueText, ComfortRearReboundValueText);
		SetBumpReboundTexts(_sportBumpReboundValues, SportFrontBumpValueText, SportFrontReboundValueText, SportRearBumpValueText, SportRearReboundValueText);
		SetBumpReboundTexts(_comfortBumpReboundValues, CustomFrontBumpValueText, CustomFrontReboundValueText, CustomRearBumpValueText, CustomRearReboundValueText);
	}

	private void SetCarShockAbsorbersPresets()
	{
		_currentSuspensionState = _carControlUIPanel.GetVehicleEntityProvider.CarEntity.FrontWheel.SuspensionData.SuspensionState;
		VehicleEntityProvider getVehicleEntityProvider = _carControlUIPanel.GetVehicleEntityProvider;
		_cachedBumpReboundValues = Utils.GetSuspensionValues(getVehicleEntityProvider.CarEntity.FrontWheel.SuspensionData, getVehicleEntityProvider.CarEntity.RearWheel.SuspensionData, _vehicleStockTuningData.Tuning.FrontWheelData.SuspensionData, _vehicleStockTuningData.Tuning.RearWheelData.SuspensionData, SuspensionState.Custom);
		_comfortBumpReboundValues = Utils.GetSuspensionValues(getVehicleEntityProvider.CarEntity.FrontWheel.SuspensionData, getVehicleEntityProvider.CarEntity.RearWheel.SuspensionData, _vehicleStockTuningData.Tuning.FrontWheelData.SuspensionData, _vehicleStockTuningData.Tuning.RearWheelData.SuspensionData, SuspensionState.Comfort);
		_sportBumpReboundValues = Utils.GetSuspensionValues(getVehicleEntityProvider.CarEntity.FrontWheel.SuspensionData, getVehicleEntityProvider.CarEntity.RearWheel.SuspensionData, _vehicleStockTuningData.Tuning.FrontWheelData.SuspensionData, _vehicleStockTuningData.Tuning.RearWheelData.SuspensionData, SuspensionState.Sport);
		ActivateToggles();
	}

	private void ActivateToggles()
	{
		ComfortToggle.SetIsOnWithoutNotify(_currentSuspensionState == SuspensionState.Comfort);
		SportToggle.SetIsOnWithoutNotify(_currentSuspensionState == SuspensionState.Sport);
		CustomToggle.SetIsOnWithoutNotify(_currentSuspensionState == SuspensionState.Custom);
	}

	private void SetBumpReboundTexts(float4 values, TextMeshProUGUI frontBump, TextMeshProUGUI frontRebound, TextMeshProUGUI rearBump, TextMeshProUGUI rearRebound)
	{
		frontBump.text = $"{Utils.N2KN(values.x):N}";
		frontRebound.text = $"{Utils.N2KN(values.y):N}";
		rearBump.text = $"{Utils.N2KN(values.z):N}";
		rearRebound.text = $"{Utils.N2KN(values.w):N}";
	}

	private void SetCustom(bool state)
	{
		if (state)
		{
			_currentSuspensionState = SuspensionState.Custom;
			UpdateShockAbsorbers(_currentSuspensionState);
			Utils.Vibrate(0.05f, 0.2f);
		}
	}

	private void SetComfort(bool state)
	{
		if (state)
		{
			_currentSuspensionState = SuspensionState.Comfort;
			UpdateShockAbsorbers(_currentSuspensionState);
			Utils.Vibrate(0.05f, 0.2f);
		}
	}

	private void SetSport(bool state)
	{
		if (state)
		{
			_currentSuspensionState = SuspensionState.Sport;
			UpdateShockAbsorbers(_currentSuspensionState);
			Utils.Vibrate(0.05f, 0.2f);
		}
	}

	private void UpdateShockAbsorbers(SuspensionState state)
	{
		_carControlUIPanel.GetVehicleEntityProvider.UpdateSuspensionState(state);
	}
}
