using System.Linq;
using CPM.Database;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI.VisualTuning;
using _Car_Parking.Scripts.UIElement;

public class BrakesViusalTuningPanel : MonoBehaviour
{
	private BrakeTuningConfig _brakeTuningConfig;

	[SerializeField]
	private VisualTuningWindow VisualTuningWindow = default;

	[SerializeField]
	[FormerlySerializedAs("visualTuningElementPrefab")]
	private VisualItemElement VisualItemElementPrefab = default;

	[SerializeField]
	private TextMeshProUGUI SideText = default;

	[SerializeField]
	private TextMeshProUGUI typeText = default;

	[SerializeField]
	private SliderValueHandler BrakesDiameterSlider = default;

	[SerializeField]
	private Button WheelAxleLeftButton = default;

	[SerializeField]
	private Button WheelAxleRightButton = default;

	[SerializeField]
	private Button BrakeTypeLeftButton = default;

	[SerializeField]
	private Button BrakeTypeRightButton = default;

	[SerializeField]
	private CalipersTuningPanel CalipersVisualTuningPanel = default;

	public WheelTuningAxleType CurrentAxleType;

	private IWheelsDataService _wheelsDataService;

	private int _maxWheelAxleType = 3;

	private CarInfo _carInfo;

	private ICarInfoService _carInfoService;

	private IVisualTuningViewModelComplexOne _visualTuningViewModelComplexOne;

	private IVisualTuningViewModelComplexParams _visualTuningViewModelComplexParams;

	private const float MinBrakeDiskDiam = 190f;

	[Inject]
	public void Construct(ICarInfoService carInfoService, BrakeTuningConfig brakeTuningConfig, IVisualTuningViewModelComplexOne visualTuningViewModelComplexOne, IWheelsDataService wheelsDataService, IVisualTuningViewModelComplexParams visualTuningViewModelComplexParams)
	{
		_wheelsDataService = wheelsDataService;
		_carInfoService = carInfoService;
		_brakeTuningConfig = brakeTuningConfig;
		_visualTuningViewModelComplexOne = visualTuningViewModelComplexOne;
		_visualTuningViewModelComplexParams = visualTuningViewModelComplexParams;
	}

	private void OnEnable()
	{
		VisualTuningWindow.SetCurrentVisualTuningTypes(VisualTuningTypes.Brakes);
		WheelAxleLeftButton.onClick.AddListener(() =>
		{
			ChangeSide(isLeft: true);
		});
		WheelAxleRightButton.onClick.AddListener(() =>
		{
			ChangeSide(isLeft: false);
		});
		Init();
	}

	public void Start()
	{
		BrakeTypeLeftButton.onClick.AddListener(GoToCalipers);
		BrakeTypeRightButton.onClick.AddListener(GoToCalipers);
	}

	private void SetSliderValues(SliderValueHandler sliderValueHandler, float minVal, float maxValue, float step, bool roundValue = false, bool removeSpecialListener = false)
	{
		sliderValueHandler.Init(minVal, maxValue, step, roundValue, removeSpecialListener);
	}

	public void Init()
	{
		_carInfo = _carInfoService.GetFromSlot(V.SlotID);
		ChangeType();
		BrakesDiameterSlider.SetValueWithoutNotify(_carInfo.TuningData.FrontWheelData.BrakesData.Diameter);
		CurrentAxleType = WheelTuningAxleType.Both;
		CalculateMaxParams();
		CurrentAxleType = WheelTuningAxleType.Front;
		ChangeSide(isLeft: true);
		BrakesDiameterSlider.AddListener(OnBrakeDiskDiameterChanged);
	}

	public void CalculateMaxParams()
	{
		float num;
		if (CurrentAxleType == WheelTuningAxleType.Both)
		{
			float a = Mathf.Floor((float)_carInfo.TuningData.FrontWheelData.wheelSize * 25.4f * 0.95f);
			float b = Mathf.Floor((float)_carInfo.TuningData.RearWheelData.wheelSize * 25.4f * 0.95f);
			num = Mathf.Min(a, b);
			_carInfo.TuningData.FrontWheelData.BrakesData.Diameter = Mathf.Clamp(_carInfo.TuningData.FrontWheelData.BrakesData.Diameter, 190f, num);
			_carInfo.TuningData.RearWheelData.BrakesData.Diameter = Mathf.Clamp(_carInfo.TuningData.RearWheelData.BrakesData.Diameter, 190f, num);
		}
		else
		{
			num = Mathf.Floor((CurrentAxleType == WheelTuningAxleType.Front) ? ((float)_carInfo.TuningData.FrontWheelData.wheelSize * 25.4f * 0.95f) : ((float)_carInfo.TuningData.RearWheelData.wheelSize * 25.4f * 0.95f));
			if (CurrentAxleType == WheelTuningAxleType.Front)
			{
				_carInfo.TuningData.FrontWheelData.BrakesData.Diameter = Mathf.Clamp(_carInfo.TuningData.FrontWheelData.BrakesData.Diameter, 190f, num);
			}
			else
			{
				_carInfo.TuningData.RearWheelData.BrakesData.Diameter = Mathf.Clamp(_carInfo.TuningData.RearWheelData.BrakesData.Diameter, 190f, num);
			}
		}
		SetSliderValues(BrakesDiameterSlider, 190f, num, 1f, roundValue: true, removeSpecialListener: true);
	}

	public void GoToCalipers()
	{
		base.gameObject.SetActive(value: false);
		CalipersVisualTuningPanel.SetActive(active: true);
	}

	public void ChangeType()
	{
		typeText.text = new LocalizedString("Visualtuningwindow.Brakes");
		_visualTuningViewModelComplexOne.Execute(VisualTuningTypes.Brakes, "Init");
		VisualTuningWindow.Revert();
		VisualTuningWindow.ClearCollections();
		VisualTuningWindow.VisualTuningElementSpawner.SpawnList(_brakeTuningConfig.AllBrakes.ToList(), VisualItemElementPrefab, VisualTuningWindow.InitBrakesDisc, cleanPanel: true);
		SetSlidersData();
		VisualTuningWindow.UiScrollRectOcclusiion.RebuildOcclusion().Forget();
	}

	private void OnBrakeDiskDiameterChanged(float value)
	{
		VisualTuningWindow.SetActiveControlButtons(isActive: true);
		_visualTuningViewModelComplexParams.Execute(VisualTuningTypes.Brakes, "SelectBrakeSize", value, false);
	}

	private void ChangeSide(bool isLeft)
	{
		VisualTuningWindow.Revert();
		CurrentAxleType = (WheelTuningAxleType)Mathf.Repeat((float)(CurrentAxleType + ((!isLeft) ? 1 : (-1))), _maxWheelAxleType);
		SideText.text = new LocalizedString("VisualColorPanel." + CurrentAxleType);
		_visualTuningViewModelComplexOne.Execute(VisualTuningTypes.Brakes, "SetAxleType", CurrentAxleType);
		CalculateMaxParams();
		SetSlidersData();
		bool isFront = CurrentAxleType == WheelTuningAxleType.Front || CurrentAxleType == WheelTuningAxleType.Both;
		int discType = (int)_carInfo.TuningData.FrontWheelData.BrakesData.DiscType;
		int discType2 = (int)_carInfo.TuningData.RearWheelData.BrakesData.DiscType;
		VisualTuningWindow.CheckInstalledElement(VisualTuningWindow.GetCachedElements(), isFront, discType, discType2);
	}

	private void SetSlidersData()
	{
		switch (CurrentAxleType)
		{
		case WheelTuningAxleType.Both:
			PrepareSliders(_carInfo.TuningData.FrontWheelData.BrakesData);
			_visualTuningViewModelComplexParams.Execute(VisualTuningTypes.Brakes, "SelectBrakeSize", _carInfo.TuningData.FrontWheelData.BrakesData.Diameter, true);
			break;
		case WheelTuningAxleType.Front:
			PrepareSliders(_carInfo.TuningData.FrontWheelData.BrakesData);
			_visualTuningViewModelComplexParams.Execute(VisualTuningTypes.Brakes, "SelectBrakeSize", _carInfo.TuningData.FrontWheelData.BrakesData.Diameter, true);
			break;
		case WheelTuningAxleType.Rear:
			PrepareSliders(_carInfo.TuningData.RearWheelData.BrakesData);
			_visualTuningViewModelComplexParams.Execute(VisualTuningTypes.Brakes, "SelectBrakeSize", _carInfo.TuningData.RearWheelData.BrakesData.Diameter, true);
			break;
		}
	}

	public void InitWithoutNotify()
	{
		switch (CurrentAxleType)
		{
		case WheelTuningAxleType.Front:
		case WheelTuningAxleType.Both:
			PrepareSliders(_carInfo.TuningData.FrontWheelData.BrakesData);
			break;
		case WheelTuningAxleType.Rear:
			PrepareSliders(_carInfo.TuningData.RearWheelData.BrakesData);
			break;
		}
	}

	private void PrepareSliders(BrakesData brakesData)
	{
		BrakesDiameterSlider.SetValueWithoutNotify(brakesData.Diameter);
		BrakesDiameterSlider.SetTextWithoutNotify(brakesData.Diameter);
	}

	public void SetSliderValue(float value)
	{
		BrakesDiameterSlider.SetValue(value);
	}

	public bool GetBrakesPaid(int index)
	{
		return _wheelsDataService.IsBrakeBought(index);
	}

	private void OnDisable()
	{
		WheelAxleLeftButton.onClick.RemoveAllListeners();
		WheelAxleRightButton.onClick.RemoveAllListeners();
		_visualTuningViewModelComplexOne.Execute(VisualTuningTypes.Brakes, "ShowWheels");
	}
}
