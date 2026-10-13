using CPMEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Inventory;

public class UIPartElement : MonoBehaviour
{
	[SerializeField]
	private Image PartImage = default;

	[SerializeField]
	private Image PartRarityImage = default;

	[SerializeField]
	private Image PartVendorImage = default;

	[SerializeField]
	private TextMeshProUGUI PartText = default;

	[SerializeField]
	private TextMeshProUGUI PartTypeText = default;

	[SerializeField]
	private TextMeshProUGUI WeightText = default;

	[SerializeField]
	private TextMeshProUGUI MaxTorqueText = default;

	[SerializeField]
	private Button PartButton = default;

	[SerializeField]
	private GameObject detailsGameObject = default;

	[SerializeField]
	private Color WarningColor = default;

	[SerializeField]
	private Color ErrorColor = default;

	[SerializeField]
	private Color NormalColor = default;

	[SerializeField]
	private Image StatusImage = default;

	[SerializeField]
	private CarTuningUISlider CarTuningUISlider = default;

	[SerializeField]
	private Button RemovePartButton = default;

	private PartData _partData;

	private PartItem _partItem;

	private ManagerUI _managerUI;

	private AllParts _allParts;

	private DetailVendorsConfigs _detailVendorsConfigs;

	private CarTuningDetailRaritySpritesConfig _carTuningDetailRaritySpritesConfig;

	private UIPartsController _uiPartsController;

	[Inject]
	public void Construct(ManagerUI managerUI, AllParts allParts, DetailVendorsConfigs detailVendorsConfigs, CarTuningDetailRaritySpritesConfig carTuningDetailRaritySpritesConfig)
	{
		_managerUI = managerUI;
		_allParts = allParts;
		_detailVendorsConfigs = detailVendorsConfigs;
		_carTuningDetailRaritySpritesConfig = carTuningDetailRaritySpritesConfig;
	}

	public void CreateEmptyElement(PartData emptyPartData, IClickable clickable)
	{
		CarTuningUISlider.gameObject.SetActive(value: false);
		PartTypeText.text = emptyPartData.GetPartTypeName();
		PartText.text = "-";
		WeightText.text = "-";
		MaxTorqueText.text = "-";
		detailsGameObject.SetActive(value: false);
		PartButton.onClick.AddListener(() =>
		{
			_managerUI.ShowMarketInventoryWindow(emptyPartData, 0);
			clickable.CloseClickable();
		});
	}

	public void Init(PartData partData, IClickable clickable, UIPartsController uiPartsController)
	{
		_uiPartsController = uiPartsController;
		DisableElements();
		if (partData != null && partData.InstanceID != -1 && partData.ModelId != 0)
		{
			InitNotEmptyElement(partData, clickable);
		}
		else
		{
			CreateEmptyElement(partData, clickable);
		}
	}

	private void DisableElements()
	{
		RemovePartButton.onClick.RemoveAllListeners();
		RemovePartButton.SetActive(active: false);
	}

	private void InitNotEmptyElement(PartData partData, IClickable clickable)
	{
		_partData = partData;
		_partItem = _partData.GetPart(_allParts);
		PartText.text = (_partItem ? _partItem.GetName : string.Empty);
		WeightText.text = (_partItem ? (Utils.GetMass(_partItem.Mass) ?? "") : string.Empty);
		PartTypeText.text = ((_partData != null) ? _partData.GetPartTypeName() : string.Empty);
		MaxTorqueText.text = (_partItem ? (_partItem.GetMaxReliability ?? "") : string.Empty);
		SetSliderColor(_partData.GetDurability());
		PartButton.onClick.RemoveAllListeners();
		PartButton.onClick.AddListener(() =>
		{
			_managerUI.ShowMarketInventoryWindow(_partData, 0);
			clickable.CloseClickable();
		});
		if (_partData != null && (_partData.PartType == PartType.TurboCharger || partData.PartType == PartType.SuperCharger || _partData.PartType == PartType.Intercooler))
		{
			RemovePartButton.SetActive(active: true);
			RemovePartButton.onClick.AddListener(RemovePart);
		}
		if ((bool)_partItem)
		{
			PartImage.sprite = _managerUI.GetEngineIcon((int)_partItem.TypeId);
			PartRarityImage.sprite = _carTuningDetailRaritySpritesConfig.CarTuningDetailRarityDatas[_partItem.Rarity].SquareRaritySprite;
			PartVendorImage.sprite = _detailVendorsConfigs.Vendors[_partItem.VendorId].CarTuningWindowSprite;
		}
		RefreshAssets().Forget();
	}

	private void RemovePart()
	{
		_uiPartsController.RemoveDetail(_partData.PartType);
	}

	private void SetSliderColor(float percent)
	{
		if (percent < 0.5f)
		{
			bool flag = percent > 0.2f;
			StatusImage.gameObject.SetActive(value: true);
			StatusImage.color = (flag ? WarningColor : ErrorColor);
			CarTuningUISlider.Init(flag ? WarningColor : ErrorColor, percent);
		}
		else
		{
			StatusImage.gameObject.SetActive(value: false);
			CarTuningUISlider.Init(NormalColor, percent);
		}
	}

	private async UniTaskVoid RefreshAssets()
	{
		await UniTask.Yield();
		if ((bool)PartTypeText)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(PartTypeText.GetComponent<RectTransform>());
		}
	}
}
