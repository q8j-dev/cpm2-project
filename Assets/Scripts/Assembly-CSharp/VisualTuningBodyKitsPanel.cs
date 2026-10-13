using System.Collections.Generic;
using System.Text.RegularExpressions;
using CPM.Database;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.MenuUI.VisualTuning;
using _Car_Parking.Scripts.UIElement;

public class VisualTuningBodyKitsPanel : MonoBehaviour
{
	[FormerlySerializedAs("visualTuningBodyKitPrefab")]
	public VisualItemBodyKitElement VisualItemBodyKitPrefab;

	[SerializeField]
	private VisualTuningWindow VisualTuningWindow = default;

	[SerializeField]
	private Texture2D[] AirIntakes = default;

	[SerializeField]
	private Texture2D[] DefaultBodyTextures = default;

	[SerializeField]
	private Texture2D[] Spoilers = default;

	private bool _arrayInitialized;

	private IPricesService _pricesService;

	private List<VisualItemBodyKitElement> _elements = new List<VisualItemBodyKitElement>();

	[Inject]
	public void Construct(IPricesService pricesService)
	{
		_pricesService = pricesService;
	}

	public void Init()
	{
		_arrayInitialized = false;
		BodyKits component = V.Car.GetComponent<BodyKits>();
		_elements.Clear();
		if ((bool)component)
		{
			List<BodyKitUIElementData> list = new List<BodyKitUIElementData>();
			CreateBodyKits(component.spoilers, BodyKitType.Spoiler, component.PaidSpoilers.Length, list);
			CreateBodyKits(component.BumperCountFront, BodyKitType.FrontBumper, component.PaidFrontBumpers.Length, list);
			CreateBodyKits(component.BumperCountRear, BodyKitType.RearBumper, component.PaidRearBumpers.Length, list);
			CreateBodyKits(component.SkirtsCount, BodyKitType.Skirt, component.PaidSkirts.Length, list);
			CreateBodyKits(component.HoodCount, BodyKitType.Hood, component.PaidHoods.Length, list);
			CreateBodyKits(component.airIntakesHood, BodyKitType.AirIntakeHood, component.PaidIntakeHood.Length, list);
			CreateBodyKits(component.airIntakeRoof, BodyKitType.AirIntakeRoof, component.PaidIntakeRoof.Length, list);
			CreateBodyKits(component.FenderCount, BodyKitType.Fender, component.PaidFenders.Length, list);
			CreateBodyKits(component.trims, BodyKitType.Trim, component.PaidTrims.Length, list, isLast: true);
			VisualTuningWindow.VisualTuningElementSpawner.SpawnList(list, VisualItemBodyKitPrefab, InitBodyKit);
			UpdateUI();
		}
	}

	public void InitBodyKit(GameObject go, object bItem)
	{
		BodyKitUIElementData bodyKitUIElementData = (BodyKitUIElementData)bItem;
		VisualItemBodyKitElement component = go.GetComponent<VisualItemBodyKitElement>();
		component.Init(bodyKitUIElementData, VisualTuningWindow.GetCarInfo());
		component.OnClicked += OnElementClickedCallback;
		_elements.Add(component);
	}

	private void OnEnable()
	{
		VisualTuningWindow.OnTuningValuesChanged += OnTuningValuesChanged;
	}

	private void OnDisable()
	{
		VisualTuningWindow.OnTuningValuesChanged -= OnTuningValuesChanged;
	}

	private void OnTuningValuesChanged()
	{
		UpdateUI();
	}

	private void OnElementClickedCallback(VisualItemBodyKitElement element)
	{
		VisualTuningWindow.PlaySound(SoundEventsDataHelper.TuningSelectCarbodySwitch);
		VisualTuningWindow.OnClickExecuteWithParams(VisualTuningTypes.BodyKits, "Select", element.BodyKitUIElementData.BodyKitType, element.BodyKitUIElementData.Index);
		VisualTuningWindow.SetSelectedVisualItemElement(element);
	}

	private void CreateBodyKits(Transform[] bodyKitsTransformsArray, BodyKitType bodyKitType, int canBuyCount, List<BodyKitUIElementData> bodyKitUIElementDatas, bool isLast = false)
	{
		int num = ((bodyKitType == BodyKitType.RearBumper) ? canBuyCount : bodyKitsTransformsArray.Length);
		bool flag = num > 0;
		for (int i = 1; i < num; i++)
		{
			if ((bool)bodyKitsTransformsArray[i])
			{
				int.TryParse(Regex.Match(bodyKitsTransformsArray[i].name, "\\d+").Value, out var result);
				SpawnElement(result, bodyKitType, bodyKitUIElementDatas);
			}
		}
		if (!isLast & flag)
		{
			bodyKitUIElementDatas.Add(new BodyKitUIElementData
			{
				Index = -1,
				isSpacer = true
			});
		}
	}

	private void CreateBodyKits(int bodyKitsTransformsArray, BodyKitType bodyKitType, int canBuyCount, List<BodyKitUIElementData> bodyKitUIElementDatas, bool isLast = false)
	{
		int num = ((bodyKitType == BodyKitType.RearBumper) ? canBuyCount : bodyKitsTransformsArray);
		bool flag = num > 0;
		for (int i = (BodyKitForcedDefaults.ShouldForceNonDefault(bodyKitType) ? 1 : 0); i < num; i++)
		{
			SpawnElement(i, bodyKitType, bodyKitUIElementDatas);
		}
		if (!isLast & flag)
		{
			bodyKitUIElementDatas.Add(new BodyKitUIElementData
			{
				Index = -1,
				isSpacer = true
			});
		}
	}

	private void SpawnElement(int partCount, BodyKitType bodyKitType, List<BodyKitUIElementData> bodyKitUIElementDatas)
	{
		BodyKitUIElementData bodyKitUIElementData = new BodyKitUIElementData();
		bodyKitUIElementData.Index = partCount;
		bodyKitUIElementData.BodyKitType = bodyKitType;
		switch (bodyKitType)
		{
		case BodyKitType.Spoiler:
			bodyKitUIElementData.ImgTexture = Spoilers[partCount - 1];
			break;
		case BodyKitType.AirIntakeHood:
			bodyKitUIElementData.ImgTexture = AirIntakes[partCount - 1];
			break;
		case BodyKitType.AirIntakeRoof:
			bodyKitUIElementData.ImgTexture = AirIntakes[partCount - 1];
			break;
		default:
			bodyKitUIElementData.ImgTexture = DefaultBodyTextures[(int)(bodyKitType - 1)];
			bodyKitUIElementData.partNumber = "#" + partCount + 1;
			break;
		}
		bodyKitUIElementData.CombinedPrice = BodyKitsService.GetCombinedPrice(bodyKitType, partCount, _pricesService);
		bodyKitUIElementDatas.Add(bodyKitUIElementData);
	}

	private void UpdateUI()
	{
		if (VisualTuningWindow.BodyKitsVisualScope == null)
		{
			return;
		}
		foreach (VisualItemBodyKitElement element in _elements)
		{
			if (!element.BodyKitUIElementData.isSpacer)
			{
				if (element.BodyKitUIElementData.BodyKitType == BodyKitType.Trim)
				{
					element.SetSelected(VisualTuningWindow.BodyKitsVisualScope.Cart.IsHaveItem(element.BodyKitUIElementData));
				}
				else if (VisualTuningWindow.BodyKitsVisualScope.Cart.BodyKitType == element.BodyKitUIElementData.BodyKitType)
				{
					element.SetSelected(VisualTuningWindow.BodyKitsVisualScope.Cart.IsHaveItem(element.BodyKitUIElementData));
				}
				else
				{
					SetItemDefaultIsOn(element);
				}
			}
		}
		VisualTuningWindow.SetActiveControlButtons(VisualTuningWindow.BodyKitsVisualScope.Cart.IsFilled);
	}

	private void SetItemDefaultIsOn(VisualItemBodyKitElement visualItemBodyKitElement)
	{
		if ((bool)visualItemBodyKitElement && !visualItemBodyKitElement.BodyKitUIElementData.isSpacer)
		{
			bool selected = VisualTuningWindow.GetCarInfo().InstalledBodyKits.IsBodyKitInstalled(visualItemBodyKitElement.BodyKitUIElementData.BodyKitType, visualItemBodyKitElement.BodyKitUIElementData.Index);
			visualItemBodyKitElement.SetSelected(selected);
		}
	}
}
