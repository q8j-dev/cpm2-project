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

public class CalipersTuningPanel : MonoBehaviour
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
	[Header("CalipersSliders")]
	private GameObject PistonsSlidersPanel = default;

	[SerializeField]
	private SliderValueHandler PistonsCountSlider = default;

	[SerializeField]
	private SliderValueHandler PistonDiameterSlider = default;

	[SerializeField]
	private Button WheelAxleLeftButton = default;

	[SerializeField]
	private Button WheelAxleRightButton = default;

	[SerializeField]
	private Button BrakeTypeLeftButton = default;

	[SerializeField]
	private Button BrakeTypeRightButton = default;

	[SerializeField]
	private BrakesViusalTuningPanel BrakesVisualTuningPanel = default;

	public WheelTuningAxleType CurrentAxleType;

	private IWheelsDataService _wheelsDataService;

	private int _maxWheelAxleType = 3;

	private CarInfo _carInfo;

	private ICarInfoService _carInfoService;

	private IVisualTuningViewModelComplexOne _visualTuningViewModelComplexOne;

	private IVisualTuningViewModelComplexParams _visualTuningViewModelComplexParams;

	private const float PistonsCountMin = 1f;

	private const float PistonDiameterMin = 20f;

	private BrakesData CurrentBrakesData
	{
		get
		{
			if (CurrentAxleType != WheelTuningAxleType.Rear)
			{
				return _carInfo.TuningData.FrontWheelData.BrakesData;
			}
			return _carInfo.TuningData.RearWheelData.BrakesData;
		}
	}

	private static int GetMaxPistonsForConfig(float discDiameter, float pistonDiameter)
	{
		int a = ((discDiameter < 340f) ? ((discDiameter < 240f) ? 2 : ((!(discDiameter < 290f)) ? 6 : 4)) : ((!(discDiameter < 390f)) ? 10 : 8));
		int b = Mathf.FloorToInt(discDiameter * 0.4f / Mathf.Max(pistonDiameter, 1f)) * 2;
		return Mathf.Clamp(Mathf.Min(a, b), 1, 10);
	}

	private static float GetMaxPistonDiameterForConfig(float discDiameter, int pistonCount)
	{
		float a = ((discDiameter < 340f) ? ((discDiameter < 240f) ? 34f : ((!(discDiameter < 290f)) ? 42f : 38f)) : ((!(discDiameter < 390f)) ? 48f : 46f));
		float b = discDiameter * 0.4f / Mathf.Max((float)pistonCount / 2f, 1f);
		return Mathf.Min(a, b);
	}

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
		VisualTuningWindow.SetCurrentVisualTuningTypes(VisualTuningTypes.Calipers);
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

	private void Start()
	{
		BrakeTypeLeftButton.onClick.AddListener(GoToBrakes);
		BrakeTypeRightButton.onClick.AddListener(GoToBrakes);
	}

	private void GoToBrakes()
	{
		base.gameObject.SetActive(value: false);
		BrakesVisualTuningPanel.gameObject.SetActive(value: true);
	}

	public void Init()
	{
		_carInfo = _carInfoService.GetFromSlot(V.SlotID);
		ChangeType();
		CurrentAxleType = WheelTuningAxleType.Front;
		ChangeSide(isLeft: true);
		PistonsCountSlider.AddListener(OnPistonsCountChanged);
		PistonDiameterSlider.AddListener(OnPistonsDiameterChanged);
	}

	private void SetSliderValues(SliderValueHandler sliderValueHandler, float minVal, float maxValue, float step, bool roundValue = false, bool removeSpecialListener = false, bool ignoreSetDefaultValue = false)
	{
		sliderValueHandler.Init(minVal, maxValue, step, roundValue, removeSpecialListener, 2, ignoreSetDefaultValue);
	}

	public void ChangeType()
	{
		_visualTuningViewModelComplexOne.Execute(VisualTuningTypes.Calipers, "Init");
		VisualTuningWindow.Revert();
		typeText.text = new LocalizedString("CalipersTuningPanel.Calipers");
		PistonsSlidersPanel.gameObject.SetActive(value: true);
		VisualTuningWindow.ClearCollections();
		VisualTuningWindow.VisualTuningElementSpawner.SpawnList(_brakeTuningConfig.AllBrakesCaliperDatas.ToList(), VisualItemElementPrefab, VisualTuningWindow.InitCalipersMesh, cleanPanel: true);
		VisualTuningWindow.UiScrollRectOcclusiion.RebuildOcclusion().Forget();
	}

	private void CalculateCaliperLimits(BrakesData brakesData)
	{
		int maxPistonsForConfig = GetMaxPistonsForConfig(brakesData.Diameter, brakesData.PistonDiameter);
		float maxPistonDiameterForConfig = GetMaxPistonDiameterForConfig(brakesData.Diameter, brakesData.Pistons);
		SetSliderValues(PistonsCountSlider, 1f, maxPistonsForConfig, 1f, roundValue: true, removeSpecialListener: true, ignoreSetDefaultValue: true);
		SetSliderValues(PistonDiameterSlider, 20f, maxPistonDiameterForConfig, 1f, roundValue: true, removeSpecialListener: true, ignoreSetDefaultValue: true);
		brakesData.ClampToDiscLimits();
	}

	private void SetSlidersData()
	{
		switch (CurrentAxleType)
		{
		case WheelTuningAxleType.Front:
		case WheelTuningAxleType.Both:
		{
			BrakesData brakesData2 = _carInfo.TuningData.FrontWheelData.BrakesData;
			CalculateCaliperLimits(brakesData2);
			PrepareSliders(brakesData2);
			_visualTuningViewModelComplexParams.Execute(VisualTuningTypes.Calipers, "SelectPistonsCount", brakesData2.Pistons);
			_visualTuningViewModelComplexParams.Execute(VisualTuningTypes.Calipers, "SelectPistonsDiameter", brakesData2.PistonDiameter);
			break;
		}
		case WheelTuningAxleType.Rear:
		{
			BrakesData brakesData = _carInfo.TuningData.RearWheelData.BrakesData;
			CalculateCaliperLimits(brakesData);
			PrepareSliders(brakesData);
			_visualTuningViewModelComplexParams.Execute(VisualTuningTypes.Calipers, "SelectPistonsCount", brakesData.Pistons);
			_visualTuningViewModelComplexParams.Execute(VisualTuningTypes.Calipers, "SelectPistonsDiameter", brakesData.PistonDiameter);
			break;
		}
		}
	}

	private void OnPistonsCountChanged(float value)
	{
		VisualTuningWindow.SetActiveControlButtons(isActive: true);
		_visualTuningViewModelComplexOne.Execute(VisualTuningTypes.Calipers, "SelectPistonsCount", value);
		float maxPistonDiameterForConfig = GetMaxPistonDiameterForConfig(CurrentBrakesData.Diameter, (int)value);
		float value2 = PistonDiameterSlider.GetValue();
		SetSliderValues(PistonDiameterSlider, 20f, maxPistonDiameterForConfig, 1f, roundValue: true, removeSpecialListener: true, ignoreSetDefaultValue: true);
		float num = Mathf.Clamp(Mathf.Round(value2), 20f, maxPistonDiameterForConfig);
		PistonDiameterSlider.SetValueWithoutNotify(num);
		PistonDiameterSlider.SetTextWithoutNotify(num);
	}

	private void OnPistonsDiameterChanged(float value)
	{
		VisualTuningWindow.SetActiveControlButtons(isActive: true);
		_visualTuningViewModelComplexOne.Execute(VisualTuningTypes.Calipers, "SelectPistonsDiameter", value);
		int maxPistonsForConfig = GetMaxPistonsForConfig(CurrentBrakesData.Diameter, value);
		float value2 = PistonsCountSlider.GetValue();
		SetSliderValues(PistonsCountSlider, 1f, maxPistonsForConfig, 1f, roundValue: true, removeSpecialListener: true, ignoreSetDefaultValue: true);
		float num = Mathf.Clamp(Mathf.Round(value2), 1f, maxPistonsForConfig);
		PistonsCountSlider.SetValueWithoutNotify(num);
		PistonsCountSlider.SetTextWithoutNotify(num);
	}

	private void PrepareSliders(BrakesData brakesData)
	{
		PistonsCountSlider.SetValueWithoutNotify(brakesData.Pistons);
		PistonsCountSlider.SetTextWithoutNotify(brakesData.Pistons);
		PistonDiameterSlider.SetValueWithoutNotify(brakesData.PistonDiameter);
		PistonDiameterSlider.SetTextWithoutNotify(brakesData.PistonDiameter);
	}

	public void SetSliderValue(BrakesData brakesData)
	{
		PistonsCountSlider.SetValue(brakesData.Pistons);
		PistonDiameterSlider.SetValue(brakesData.PistonDiameter);
	}

	public bool GetCalipersPaid(int index)
	{
		return _wheelsDataService.IsCaliperBought(index);
	}

	private bool AreFrontRearDiscDiametersSame()
	{
		float diameter = _carInfo.TuningData.FrontWheelData.BrakesData.Diameter;
		float diameter2 = _carInfo.TuningData.RearWheelData.BrakesData.Diameter;
		return Mathf.Approximately(diameter, diameter2);
	}

	private void ChangeSide(bool isLeft)
	{
		VisualTuningWindow.Revert();
		CurrentAxleType = (WheelTuningAxleType)Mathf.Repeat((float)(CurrentAxleType + ((!isLeft) ? 1 : (-1))), _maxWheelAxleType);
		if (CurrentAxleType == WheelTuningAxleType.Both && !AreFrontRearDiscDiametersSame())
		{
			CurrentAxleType = (WheelTuningAxleType)Mathf.Repeat((float)(CurrentAxleType + ((!isLeft) ? 1 : (-1))), _maxWheelAxleType);
		}
		SideText.text = new LocalizedString("VisualColorPanel." + CurrentAxleType);
		_visualTuningViewModelComplexOne.Execute(VisualTuningTypes.Calipers, "SetAxleType", CurrentAxleType);
		SetSlidersData();
		WheelTuningAxleType currentAxleType = CurrentAxleType;
		bool isFront = currentAxleType == WheelTuningAxleType.Front || currentAxleType == WheelTuningAxleType.Both;
		int caliperMeshIndex = _carInfo.TuningData.FrontWheelData.BrakesData.CaliperMeshIndex;
		int caliperMeshIndex2 = _carInfo.TuningData.RearWheelData.BrakesData.CaliperMeshIndex;
		VisualTuningWindow.CheckInstalledElement(VisualTuningWindow.GetCachedElements(), isFront, caliperMeshIndex, caliperMeshIndex2);
	}

	private void OnDisable()
	{
		WheelAxleLeftButton.onClick.RemoveAllListeners();
		WheelAxleRightButton.onClick.RemoveAllListeners();
		PistonsCountSlider.RemoveAllListeners();
		PistonDiameterSlider.RemoveAllListeners();
		_visualTuningViewModelComplexOne.Execute(VisualTuningTypes.Calipers, "ShowWheels");
	}

	public void InitWithoutNotify()
	{
		switch (CurrentAxleType)
		{
		case WheelTuningAxleType.Front:
		case WheelTuningAxleType.Both:
		{
			BrakesData brakesData2 = _carInfo.TuningData.FrontWheelData.BrakesData;
			CalculateCaliperLimits(brakesData2);
			PrepareSliders(brakesData2);
			break;
		}
		case WheelTuningAxleType.Rear:
		{
			BrakesData brakesData = _carInfo.TuningData.RearWheelData.BrakesData;
			CalculateCaliperLimits(brakesData);
			PrepareSliders(brakesData);
			break;
		}
		}
	}
}
