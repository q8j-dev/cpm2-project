using System;
using System.Collections.Generic;
using System.Linq;
using CPM.Database;
using CPMEngine;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Racing.CameraManagement;
using Racing.CameraManagement.SpecificTypes;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI.VisualTuning;
using _Car_Parking.Scripts.UIElement;

public class VisualColorPanel : MonoBehaviour
{
	private const int DefaultWheelsColorTypeIndex = 2;

	public ColorPicker colorPicker;

	public CarMatType currentColorMatType;

	public ColorsScope.ColorsType currentColorType;

	public RawImage elementImage;

	public ColorsScope.WheelsColorType currentWheelsColorType;

	public BodyKitType currentBodyKitType;

	[SerializeField]
	private VisualTuningWindow _visualTuningWindow;

	[FormerlySerializedAs("visualTuningColorElementPrefab")]
	[SerializeField]
	private VisualItemColorElement VisualItemColorElementPrefab;

	private CarInfo _carInfo;

	[SerializeField]
	private Button resetButton;

	[SerializeField]
	private TMP_Dropdown wheelDropdown;

	[SerializeField]
	private GameObject subMeshPickPanel;

	[SerializeField]
	private Button leftButton;

	[SerializeField]
	private Button rightButton;

	[SerializeField]
	private TextMeshProUGUI colorCategoryTitle;

	[SerializeField]
	private SliderValueHandler ThicknessSlider;

	[SerializeField]
	private SliderValueHandler PowerSlider;

	[SerializeField]
	private GameObject SliderPanel;

	[SerializeField]
	private GameObject AlphaPanel;

	[SerializeField]
	private SliderValueHandler AlphaSlider;

	[SerializeField]
	private Toggle WheelAllColorToggle;

	[SerializeField]
	private Toggle WheelFirstColorToggle;

	[SerializeField]
	private Toggle WheelSecondColorToggle;

	private VisualItemColorElement[] _visualTuningColorElements;

	private int _currentColorWibdowIndex;

	private int _currentColorIndex;

	private int _maxColorIndex;

	private ColorsScope.ColorsType[] _colorsScopes;

	private MenuControl _menuControl;

	private VehicleData _vehicleData;

	private CameraManager _cameraManager;

	private WheelTuningAxleType _axleType;

	private ManagerUI _managerUI;

	private List<BodyKitType> _colorBodyKitTypes = new List<BodyKitType>();

	[Inject]
	private void Construct(MenuControl menuControl, CameraManager cameraManager, ManagerUI managerUI)
	{
		_managerUI = managerUI;
		_cameraManager = cameraManager;
		_menuControl = menuControl;
	}

	private void Start()
	{
		WheelAllColorToggle.onValueChanged.AddListener(SetAllDiskcolor);
		WheelFirstColorToggle.onValueChanged.AddListener(SetFirstDiskcolor);
		WheelSecondColorToggle.onValueChanged.AddListener(SetSecoundDiskColor);
	}

	private void SetAllDiskcolor(bool state)
	{
		if (state)
		{
			_visualTuningWindow.Revert();
			ColorsScope.WheelsColorType colorType = GetColorType(0);
			currentWheelsColorType = colorType;
		}
	}

	private void SetFirstDiskcolor(bool state)
	{
		if (state)
		{
			_visualTuningWindow.Revert();
			ColorsScope.WheelsColorType colorType = GetColorType(1);
			currentWheelsColorType = colorType;
		}
	}

	private void SetSecoundDiskColor(bool state)
	{
		if (state)
		{
			_visualTuningWindow.Revert();
			ColorsScope.WheelsColorType colorType = GetColorType(2);
			currentWheelsColorType = colorType;
		}
	}

	private ColorsScope.WheelsColorType GetColorType(int index)
	{
		switch (_axleType)
		{
		case WheelTuningAxleType.Both:
			switch (index)
			{
			case 0:
				return ColorsScope.WheelsColorType.Both;
			case 1:
				return ColorsScope.WheelsColorType.Both1;
			case 2:
				return ColorsScope.WheelsColorType.Both2;
			}
			break;
		case WheelTuningAxleType.Front:
			switch (index)
			{
			case 0:
				return ColorsScope.WheelsColorType.Front;
			case 1:
				return ColorsScope.WheelsColorType.Front1;
			case 2:
				return ColorsScope.WheelsColorType.Front2;
			}
			break;
		case WheelTuningAxleType.Rear:
			switch (index)
			{
			case 0:
				return ColorsScope.WheelsColorType.Rear;
			case 1:
				return ColorsScope.WheelsColorType.Rear1;
			case 2:
				return ColorsScope.WheelsColorType.Rear2;
			}
			break;
		}
		return ColorsScope.WheelsColorType.Both;
	}

	private void Awake()
	{
		ThicknessSlider.Init(0f, 1f, 0.01f);
		PowerSlider.Init(0f, 1f, 0.01f);
		AlphaSlider.Init(0f, 1f, 0.01f);
		_colorsScopes = (from ColorsScope.ColorsType a in Enum.GetValues(typeof(ColorsScope.ColorsType))
			where a != ColorsScope.ColorsType.None
			select a).ToArray();
		_maxColorIndex = _colorsScopes.Length - 1;
		ThicknessSlider.AddListener(_visualTuningWindow.ChangeIridescent);
		PowerSlider.AddListener(_visualTuningWindow.ChangeIridescent);
		AlphaSlider.AddListener(_visualTuningWindow.ChangeAlpha);
	}

	private VisualItemColorElement[] GetVisualColorElementsData()
	{
		if (_visualTuningColorElements.Length < 1)
		{
			_visualTuningColorElements = _visualTuningWindow.content.GetComponentsInChildren<VisualItemColorElement>(includeInactive: true);
		}
		return _visualTuningColorElements;
	}

	public void SetThicknessAndPower(float thickness, float power)
	{
		ThicknessSlider.SetValue(thickness);
		PowerSlider.SetValue(power);
	}

	public void ChangeSliderStates(bool state)
	{
		SliderPanel.gameObject.SetActive(state);
	}

	public void ChangeAlphaStates(bool state)
	{
		AlphaPanel.gameObject.SetActive(state);
	}

	public (float, float) GetThicknessAndPower()
	{
		return (ThicknessSlider.GetValue(), PowerSlider.GetValue());
	}

	public float GetAlpha()
	{
		return AlphaSlider.GetValue();
	}

	private void SelectMaterialType(CarMatType colorsType)
	{
		VisualItemColorElement[] visualColorElementsData = GetVisualColorElementsData();
		foreach (VisualItemColorElement visualItemColorElement in visualColorElementsData)
		{
			if ((bool)visualItemColorElement)
			{
				visualItemColorElement.gameObject.SetActive(value: true);
				visualItemColorElement.Button.isOn = colorsType == visualItemColorElement.carMatType;
				continue;
			}
			break;
		}
	}

	public void ChangeColorType(bool isLeft)
	{
		RevertColor();
		DisableOrEnableWheelUIElements(state: false);
		if (isLeft)
		{
			_currentColorIndex = (int)Mathf.Repeat(_currentColorIndex - 1, _maxColorIndex + 1);
		}
		else
		{
			_currentColorIndex = (int)Mathf.Repeat(_currentColorIndex + 1, _maxColorIndex + 1);
		}
		ChangeColorType(_colorsScopes[_currentColorIndex]);
		if ((bool)elementImage)
		{
			elementImage.color = colorPicker.color;
		}
		if (_colorsScopes[_currentColorIndex] == ColorsScope.ColorsType.Windows && (float)(int)_carInfo.Colors.WindowColor.A == 0f)
		{
			AlphaSlider.SetValue(0.5f);
			_visualTuningWindow.ChangeAlpha(AlphaSlider.GetValue());
		}
		if (currentColorType == ColorsScope.ColorsType.BodyKits && _colorBodyKitTypes.Count == 0)
		{
			ChangeColorType(isLeft);
		}
	}

	public void OnFirstColorPicked(bool isOn)
	{
		if (isOn)
		{
			ChangeSubMeshId(1);
		}
	}

	public void OnSecondColorPicked(bool isOn)
	{
		if (isOn)
		{
			ChangeSubMeshId(2);
		}
	}

	public void ChangeSubMeshId(int id)
	{
		switch (wheelDropdown.value)
		{
		case 0:
			ChangeWheelsColorType((id == 1) ? ColorsScope.WheelsColorType.Front1 : ColorsScope.WheelsColorType.Front2);
			colorPicker.Init((id == 1) ? _carInfo.TuningData.FrontWheelData.Color1.Color : _carInfo.TuningData.FrontWheelData.Color2.Color);
			SelectMaterialType((id == 1) ? _carInfo.TuningData.FrontWheelData.Color1.CarMatType : _carInfo.TuningData.FrontWheelData.Color2.CarMatType);
			break;
		case 1:
			ChangeWheelsColorType((id == 1) ? ColorsScope.WheelsColorType.Rear1 : ColorsScope.WheelsColorType.Rear2);
			colorPicker.Init((id == 1) ? _carInfo.TuningData.RearWheelData.Color1.Color : _carInfo.TuningData.RearWheelData.Color2.Color);
			SelectMaterialType((id == 1) ? _carInfo.TuningData.RearWheelData.Color1.CarMatType : _carInfo.TuningData.RearWheelData.Color2.CarMatType);
			break;
		case 2:
			break;
		}
	}

	private void ChangeWheelAxle(int sideId)
	{
		_visualTuningWindow.PlaySound(SoundEventsDataHelper.SmallClickSwitch);
		RebuildWheelUI(sideId);
	}

	private void OnEnable()
	{
		resetButton.onClick.AddListener(ResetColor);
		CarInfoExtensions.OnCarInfoUpdated += OnCarInfoUpdated;
	}

	private void OnDisable()
	{
		resetButton.onClick.RemoveListener(ResetColor);
		CarInfoExtensions.OnCarInfoUpdated -= OnCarInfoUpdated;
	}

	public void Init(CarInfo carInfo, ColorsScope.ColorsType colorsType)
	{
		_vehicleData = V.Car.GetComponent<VehicleData>();
		_visualTuningColorElements = new VisualItemColorElement[0];
		_carInfo = carInfo;
		_currentColorIndex = 0;
		ChangeColorType(ColorsScope.ColorsType.Car);
		ChangeMatType(_carInfo.Colors.CarColor.CarMatType);
		SetThicknessAndPower(_carInfo.Colors.CarColor.IridescenceThickness, _carInfo.Colors.CarColor.IridescencePower);
		SpawnMatIcons();
		colorPicker.Init(_carInfo.Colors.CarColor.Color);
		colorPicker.enabled = true;
	}

	public void DisableOrEnableWheelUIElements(bool state)
	{
		wheelDropdown.gameObject.SetActive(state);
		subMeshPickPanel.gameObject.SetActive(state);
		if (state)
		{
			wheelDropdown.value = 2;
			RebuildWheelUI(2);
		}
	}

	private void ChangeBodyKitColorType(int index)
	{
		currentBodyKitType = (BodyKitType)index;
	}

	public void InitBodyKitsColorElements(bool state)
	{
		wheelDropdown.options.Clear();
		wheelDropdown.gameObject.SetActive(state);
		if (state)
		{
			BodyKitType[] array = (from BodyKitType b in Enum.GetValues(typeof(BodyKitType))
				where b != BodyKitType.None
				select b).ToArray();
			for (int num = 0; num < array.Length; num++)
			{
				wheelDropdown.options.Add(new TMP_Dropdown.OptionData(array[num].ToString()));
			}
			wheelDropdown.captionText.text = wheelDropdown.options[0].text;
			wheelDropdown.onValueChanged.RemoveAllListeners();
			wheelDropdown.onValueChanged.AddListener(ChangeBodyKitColorType);
		}
	}

	private void InitWheelColorElements()
	{
		wheelDropdown.options.Clear();
		wheelDropdown.options = new List<TMP_Dropdown.OptionData>();
		wheelDropdown.options.Add(new TMP_Dropdown.OptionData(new LocalizedString("VisualColorPanel.Front")));
		wheelDropdown.options.Add(new TMP_Dropdown.OptionData(new LocalizedString("VisualColorPanel.Rear")));
		wheelDropdown.options.Add(new TMP_Dropdown.OptionData(new LocalizedString("VisualColorPanel.Both")));
		wheelDropdown.captionText.text = wheelDropdown.options[2].text;
		wheelDropdown.onValueChanged.RemoveAllListeners();
		wheelDropdown.onValueChanged.AddListener(ChangeWheelAxle);
	}

	private void RebuildWheelUI(int index)
	{
		_visualTuningWindow.OnClickExecuteWithParams(VisualTuningTypes.Colors, "RevertWheelColors", _carInfo, _vehicleData);
		bool flag = false;
		switch (index)
		{
		case 0:
			flag = V.Car.GetComponent<VehicleData>().WheelComposition.WheelMatFront2 != null;
			ChangeWheelsColorType(ColorsScope.WheelsColorType.Front);
			colorPicker.Init(_carInfo.TuningData.FrontWheelData.Color1.Color, withAction: false);
			_axleType = WheelTuningAxleType.Front;
			break;
		case 1:
			flag = V.Car.GetComponent<VehicleData>().WheelComposition.WheelMatRear2 != null;
			ChangeWheelsColorType(ColorsScope.WheelsColorType.Rear);
			colorPicker.Init(_carInfo.TuningData.RearWheelData.Color1.Color, withAction: false);
			_axleType = WheelTuningAxleType.Rear;
			break;
		case 2:
			ChangeWheelsColorType(ColorsScope.WheelsColorType.Both);
			flag = V.Car.GetComponent<VehicleData>().WheelComposition.WheelMatFront2 != null && V.Car.GetComponent<VehicleData>().WheelComposition.WheelMatRear2 != null;
			colorPicker.Init(_carInfo.TuningData.FrontWheelData.Color1.Color, withAction: false);
			_axleType = WheelTuningAxleType.Both;
			break;
		}
		subMeshPickPanel.SetActive(flag);
		if (flag)
		{
			WheelAllColorToggle.isOn = true;
		}
		FocusCameraOnWheel();
	}

	public void RevertColor()
	{
		if (_vehicleData == null)
		{
			Debug.LogWarning("Color was not reverted, because car data is null. It's ok");
		}
		else if (_colorsScopes != null)
		{
			ChangeColorType(_colorsScopes[_currentColorIndex]);
		}
	}

	private void ChangeMatType(CarMatType carMatType)
	{
		currentColorMatType = carMatType;
	}

	private void ChangeWheelsColorType(ColorsScope.WheelsColorType wheelsColorType)
	{
		currentWheelsColorType = wheelsColorType;
	}

	private void ChangeColorType(ColorsScope.ColorsType colorsType)
	{
		currentColorType = colorsType;
		InitBodyKitsColorElements(state: false);
		_visualTuningWindow.OnClickExecute(VisualTuningTypes.Colors, "SetActiveColorType", currentColorType);
		switch (currentColorType)
		{
		case ColorsScope.ColorsType.Car:
			colorCategoryTitle.text = new LocalizedString("MainController.Car");
			_cameraManager.ActivateCamera(RacingCameraType.GarageLookAround, _vehicleData.transform);
			colorPicker.Init(_carInfo.Colors.CarColor.Color);
			_visualTuningWindow.Revert();
			SelectMaterialType(_carInfo.Colors.CarColor.CarMatType);
			SetThicknessAndPower(_carInfo.Colors.CarColor.IridescenceThickness, _carInfo.Colors.CarColor.IridescencePower);
			break;
		case ColorsScope.ColorsType.Wheels:
			colorCategoryTitle.text = new LocalizedString("VisualColorPanel.Wheels");
			InitWheelColorElements();
			DisableOrEnableWheelUIElements(state: true);
			colorPicker.Init(_carInfo.TuningData.FrontWheelData.Color1.Color, withAction: false);
			ChangeAlphaStates(state: false);
			SelectMaterialType(_carInfo.TuningData.FrontWheelData.Color1.CarMatType);
			SetThicknessAndPower(_carInfo.TuningData.FrontWheelData.Color1.IridescenceThickness, _carInfo.TuningData.FrontWheelData.Color1.IridescencePower);
			break;
		case ColorsScope.ColorsType.Windows:
		{
			colorCategoryTitle.text = new LocalizedString("VisualColorPanel.Windows");
			_cameraManager.ActivateCamera(RacingCameraType.GarageLookAround, _vehicleData.transform);
			colorPicker.Init(_carInfo.Colors.WindowColor);
			AlphaSlider.SetValue(WindowAlphaToSlider((float)(int)_carInfo.Colors.WindowColor.A / 255f));
			VisualItemColorElement[] visualColorElementsData = GetVisualColorElementsData();
			foreach (VisualItemColorElement visualItemColorElement in visualColorElementsData)
			{
				if (visualItemColorElement.carMatType == CarMatType.Glossy)
				{
					visualItemColorElement.Button.isOn = true;
				}
				else
				{
					visualItemColorElement.gameObject.SetActive(value: false);
				}
			}
			break;
		}
		case ColorsScope.ColorsType.Smoke:
		{
			colorCategoryTitle.text = new LocalizedString("VisualColorPanel.Smoke");
			_cameraManager.ActivateCamera(RacingCameraType.GarageLookAround, _vehicleData.transform);
			_cameraManager.garageCam.ResetCameraSmokeColor();
			colorPicker.Init(_carInfo.Colors.SmokeColor);
			ChangeAlphaStates(state: false);
			VisualItemColorElement[] visualColorElementsData = GetVisualColorElementsData();
			for (int i = 0; i < visualColorElementsData.Length; i++)
			{
				visualColorElementsData[i].gameObject.SetActive(value: false);
			}
			break;
		}
		case ColorsScope.ColorsType.BodyKits:
			InitVisualBodyKits();
			colorCategoryTitle.text = new LocalizedString("VisualColorPanel.BodyKits");
			_cameraManager.ActivateCamera(RacingCameraType.GarageLookAround, _vehicleData.transform);
			break;
		}
	}

	public float WindowAlphaFromSlider(float sliderValue)
	{
		if (sliderValue != 0f)
		{
			return sliderValue;
		}
		return 0.01f;
	}

	public float WindowAlphaToSlider(float initValue)
	{
		if (!(initValue < 0.02f) || !(initValue >= 0.005f))
		{
			return initValue;
		}
		return 0f;
	}

	private void ResetColor()
	{
		_visualTuningWindow.ShowUI(state: false);
		_managerUI.ShowConfirmWindow(() =>
		{
			_visualTuningWindow.ShowUI(state: true);
			int num = _axleType switch
			{
				WheelTuningAxleType.Front => 0, 
				WheelTuningAxleType.Rear => 1, 
				WheelTuningAxleType.Both => 2, 
				_ => 0, 
			};
			VehicleStockTuningData defaultCarTuningDataConfig = CarTuningFactory.GetDefaultCarTuningDataConfig(_vehicleData.CarId);
			_visualTuningWindow.ResetColor(defaultCarTuningDataConfig, currentWheelsColorType);
			ChangeColorType(_colorsScopes[_currentColorIndex]);
			if (_colorsScopes[_currentColorIndex] == ColorsScope.ColorsType.Wheels)
			{
				RebuildWheelUI(num);
				wheelDropdown.value = num;
			}
		}, new LocalizedString("Visualtuningwindow.ResetColor"), new LocalizedString("VisualColorPanel.Areyousureyouwanttoresetcolor?"), () =>
		{
			_visualTuningWindow.ShowUI(state: true);
		}).Forget();
	}

	private void SpawnMatIcons()
	{
		List<CarMatType> dataList = Enum.GetValues(typeof(CarMatType)).Cast<CarMatType>().ToList();
		_visualTuningWindow.VisualTuningElementSpawner.SpawnList(dataList, VisualItemColorElementPrefab, _visualTuningWindow.InitColorMatType);
	}

	private void InitVisualBodyKits()
	{
		_colorBodyKitTypes.Clear();
		BodyKits component = _vehicleData.gameObject.GetComponent<BodyKits>();
		for (int i = 0; i < ColorsScopeControllerBodyKits.PossibleBodyKitTypes.Length; i++)
		{
			if ((bool)component.GetInstalledBodyKitValues(ColorsScopeControllerBodyKits.PossibleBodyKitTypes[i]))
			{
				_colorBodyKitTypes.Add(ColorsScopeControllerBodyKits.PossibleBodyKitTypes[i]);
			}
		}
		ChangeAlphaStates(state: false);
		VisualItemColorElement[] visualColorElementsData = GetVisualColorElementsData();
		for (int j = 0; j < visualColorElementsData.Length; j++)
		{
			visualColorElementsData[j].gameObject.SetActive(_colorBodyKitTypes.Count > 0);
		}
		wheelDropdown.options.Clear();
		wheelDropdown.options = new List<TMP_Dropdown.OptionData>();
		wheelDropdown.onValueChanged.RemoveAllListeners();
		wheelDropdown.gameObject.SetActive(value: true);
		if (_colorBodyKitTypes.Count > 0)
		{
			for (int k = 0; k < _colorBodyKitTypes.Count; k++)
			{
				wheelDropdown.options.Add(new TMP_Dropdown.OptionData(new LocalizedString($"VisualColorPanel.BodyKits.{_colorBodyKitTypes[k]}")));
			}
			wheelDropdown.captionText.text = wheelDropdown.options[0].text;
			wheelDropdown.value = 0;
			wheelDropdown.onValueChanged.AddListener(OnChangeBodyKitType);
			OnChangeBodyKitType(0);
		}
		else
		{
			colorPicker.Init(Color.white);
		}
	}

	private void OnChangeBodyKitType(int sideId)
	{
		if (sideId >= 0 && sideId < _colorBodyKitTypes.Count)
		{
			bodyKitValues installedBodyKitValues = _vehicleData.gameObject.GetComponent<BodyKits>().GetInstalledBodyKitValues(_colorBodyKitTypes[sideId]);
			if ((bool)installedBodyKitValues && installedBodyKitValues.MainMaterial != null)
			{
				CarMatType colorsType = (CarMatType)installedBodyKitValues.MainMaterial.GetInt(ShaderPropertyIDs.PaintType);
				colorPicker.Init(installedBodyKitValues.MainMaterial.color, withAction: false);
				SetThicknessAndPower(installedBodyKitValues.MainMaterial.GetFloat(ShaderPropertyIDs.IridescenceThickness), installedBodyKitValues.MainMaterial.GetFloat(ShaderPropertyIDs.IridescencePower));
				SelectMaterialType(colorsType);
				colorPicker.Init(installedBodyKitValues.MainMaterial.color);
			}
			_visualTuningWindow.OnClickExecute(VisualTuningTypes.Colors, "SelectBodyKitType", _colorBodyKitTypes[sideId]);
		}
	}

	private void OnCarInfoUpdated(CarInfo carInfo)
	{
		if (_carInfo != null && carInfo == _carInfo && _visualTuningWindow.VisualTuningTypes == VisualTuningTypes.Colors && currentColorType == ColorsScope.ColorsType.BodyKits)
		{
			OnChangeBodyKitType(wheelDropdown.value);
		}
	}

	private void FocusCameraOnWheel()
	{
		Axles axles = _vehicleData.Axles;
		Transform tempTarget = _menuControl.tempTarget;
		tempTarget.rotation = _vehicleData.transform.rotation;
		if (_axleType == WheelTuningAxleType.Both)
		{
			WheelProvider leftWheel = axles.FrontAxle.LeftWheel;
			WheelProvider leftWheel2 = axles.RearAxle.LeftWheel;
			Vector3 pos = (leftWheel.transform.position + leftWheel2.transform.position) / 2f;
			_menuControl.MoveTempTarget(pos);
			_cameraManager.ActivateCamera(RacingCameraType.GarageLookAround, tempTarget, null, (CinemachineVirtualCameraBase vc) =>
			{
				VisualTuningCameraHandler component = vc.GetComponent<VisualTuningCameraHandler>();
				if ((bool)component)
				{
					component.SetCamMode(VisualTuningCamMode.BothWheel);
				}
			});
			return;
		}
		WheelProvider wheelProvider = ((_axleType == WheelTuningAxleType.Front) ? axles.FrontAxle.LeftWheel : axles.RearAxle.LeftWheel);
		_menuControl.MoveTempTarget(wheelProvider.transform.position);
		_cameraManager.ActivateCamera(RacingCameraType.GarageLookAround, tempTarget, null, (CinemachineVirtualCameraBase vc) =>
		{
			VisualTuningCameraHandler component = vc.GetComponent<VisualTuningCameraHandler>();
			if ((bool)component)
			{
				component.SetCamMode((_axleType == WheelTuningAxleType.Front) ? VisualTuningCamMode.FrontWheel : VisualTuningCamMode.RearWheel);
			}
		});
	}
}
